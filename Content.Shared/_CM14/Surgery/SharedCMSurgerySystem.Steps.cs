using Content.Shared._CM14.Marine;
using Content.Shared._CM14.Surgery.Conditions;
using Content.Shared._CM14.Surgery.Steps;
using Content.Shared._CM14.Surgery.Tools;
using Content.Shared._CM14.Xeno.Components;
using Content.Shared.Armor;
using Content.Shared.Body.Part;
using Content.Shared.Buckle.Components;
using Content.Shared.DoAfter;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Surgery;

public abstract partial class SharedCMSurgerySystem
{
    private void InitializeSteps()
    {
        SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryStepEvent>(OnToolStep);
        SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryStepCompleteCheckEvent>(OnToolCheck);
        SubscribeLocalEvent<CMSurgeryStepComponent, CMSurgeryCanPerformStepEvent>(OnToolCanPerform);

        SubSurgery<CMSurgeryCutLarvaRootsStepComponent>(OnCutLarvaRootsStep, OnCutLarvaRootsCheck);

        SubscribeLocalEvent<InventoryComponent, CMSurgeryCanPerformStepEvent>(_inventory.RelayEvent);
        SubscribeLocalEvent<ArmorComponent, InventoryRelayedEvent<CMSurgeryCanPerformStepEvent>>(OnArmorCanPerformStep);

        Subs.BuiEvents<CMSurgeryTargetComponent>(CMSurgeryUIKey.Key, sub =>
        {
            sub.Event<CMSurgeryStepChosenBuiMessage>(OnSurgeryTargetStepChosen);
        });
    }

    private void SubSurgery<TComp>(EntityEventRefHandler<TComp, CMSurgeryStepEvent> onStep,
        EntityEventRefHandler<TComp, CMSurgeryStepCompleteCheckEvent> onComplete) where TComp : IComponent
    {
        SubscribeLocalEvent(onStep);
        SubscribeLocalEvent(onComplete);
    }

    private void OnToolStep(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryStepEvent args)
    {
        if (ent.Comp.Tool != null)
        {
            foreach (var reg in ent.Comp.Tool.Values)
            {
                if (!AnyHaveComp(args.Tools, reg.Component, out var tool))
                    return;

                if (_net.IsServer &&
                    TryComp(tool, out CMSurgeryToolComponent? toolComp) &&
                    toolComp.EndSound != null)
                {
                    _audio.PlayEntity(toolComp.EndSound, args.User, tool);
                }
            }
        }

        if (ent.Comp.Add != null)
        {
            foreach (var reg in ent.Comp.Add.Values)
            {
                var compType = reg.Component.GetType();
                if (HasComp(args.Part, compType))
                    continue;

                AddComp(args.Part, _compFactory.GetComponent(compType));
            }
        }

        if (ent.Comp.Remove != null)
        {
            foreach (var reg in ent.Comp.Remove.Values)
            {
                RemComp(args.Part, reg.Component.GetType());
            }
        }

        if (ent.Comp.BodyRemove != null)
        {
            foreach (var reg in ent.Comp.BodyRemove.Values)
            {
                RemComp(args.Body, reg.Component.GetType());
            }
        }
    }

    private void OnToolCheck(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryStepCompleteCheckEvent args)
    {
        if (ent.Comp.Add != null)
        {
            foreach (var reg in ent.Comp.Add.Values)
            {
                if (!HasComp(args.Part, reg.Component.GetType()))
                {
                    args.Cancelled = true;
                    return;
                }
            }
        }

        if (ent.Comp.Remove != null)
        {
            foreach (var reg in ent.Comp.Remove.Values)
            {
                if (HasComp(args.Part, reg.Component.GetType()))
                {
                    args.Cancelled = true;
                    return;
                }
            }
        }

        if (ent.Comp.BodyRemove != null)
        {
            foreach (var reg in ent.Comp.BodyRemove.Values)
            {
                if (HasComp(args.Body, reg.Component.GetType()))
                {
                    args.Cancelled = true;
                    return;
                }
            }
        }
    }

    private void OnToolCanPerform(Entity<CMSurgeryStepComponent> ent, ref CMSurgeryCanPerformStepEvent args)
    {
        if (!TryComp(args.User, out SkillsComponent? skills) ||
            skills.Surgery < ent.Comp.MinSkill)
        {
            args.Invalid = StepInvalidReason.MissingSkills;
            return;
        }

        if (HasComp<CMSurgeryOperatingTableConditionComponent>(ent))
        {
            if (!TryComp(args.Body, out BuckleComponent? buckle) ||
                !HasComp<CMOperatingTableComponent>(buckle.BuckledTo))
            {
                args.Invalid = StepInvalidReason.NeedsOperatingTable;
                return;
            }
        }

        RaiseLocalEvent(args.Body, ref args);

        if (args.Invalid != StepInvalidReason.None)
            return;

        if (ent.Comp.Tool != null)
        {
            args.ValidTools ??= new HashSet<EntityUid>();

            foreach (var reg in ent.Comp.Tool.Values)
            {
                if (!AnyHaveComp(args.Tools, reg.Component, out var withComp))
                {
                    args.Invalid = StepInvalidReason.MissingTool;

                    if (reg.Component is ICMSurgeryToolComponent tool)
                        args.Popup = $"Вам требуется {tool.ToolName} для выполнения этого этапа!";

                    return;
                }

                args.ValidTools.Add(withComp);
            }
        }
    }

    private void OnCutLarvaRootsStep(Entity<CMSurgeryCutLarvaRootsStepComponent> ent, ref CMSurgeryStepEvent args)
    {
        if (TryComp(args.Body, out HuggerOnFaceComponent? hugged))
        {
            hugged.RootsCut = true;
        }
    }

    private void OnCutLarvaRootsCheck(Entity<CMSurgeryCutLarvaRootsStepComponent> ent, ref CMSurgeryStepCompleteCheckEvent args)
    {
        if (!TryComp(args.Body, out HuggerOnFaceComponent? hugged) || !hugged.RootsCut)
            args.Cancelled = true;
    }

    private void OnArmorCanPerformStep(Entity<ArmorComponent> ent, ref InventoryRelayedEvent<CMSurgeryCanPerformStepEvent> args)
    {
        if (args.Args.Invalid == StepInvalidReason.None)
            args.Args.Invalid = StepInvalidReason.Armor;
    }

    private void OnSurgeryTargetStepChosen(Entity<CMSurgeryTargetComponent> ent, ref CMSurgeryStepChosenBuiMessage args)
    {
        var user = args.Actor;
        if (GetEntity(args.Entity) is not { Valid: true } body ||
            !IsSurgeryValid(body, args.Part, args.Surgery, args.Step, out var surgery, out var part, out var step))
        {
            return;
        }

        if (!PreviousStepsComplete(body, part, surgery, args.Step) ||
            IsStepComplete(body, part, args.Step))
        {
            return;
        }

        if (!CanPerformStep(user, body, part.Comp.PartType, step, true, out _, out _, out var validTools))
            return;

        if (_net.IsServer && validTools?.Count > 0)
        {
            foreach (var tool in validTools)
            {
                if (TryComp(tool, out CMSurgeryToolComponent? toolComp) &&
                    toolComp.EndSound != null)
                {
                    _audio.PlayEntity(toolComp.StartSound, user, tool);
                }
            }
        }

        var doAfterTime = 2f;
        if (_prototypes.Index<EntityPrototype>(args.Step).TryGetComponent<CMSurgeryStepComponent>(out var comp))
        {
            doAfterTime = comp.DoAfter;
        }



        var ev = new CMSurgeryDoAfterEvent(GetNetEntity(part), args.Surgery, args.Step);
        var doAfter = new DoAfterArgs(EntityManager, user, doAfterTime, ev, body, body)
        {
            BreakOnMove = true,
        };
        _doAfter.TryStartDoAfter(doAfter);
    }

    private (Entity<CMSurgeryComponent> Surgery, int Step)? GetNextStep(EntityUid body, EntityUid part, Entity<CMSurgeryComponent?> surgery, List<EntityUid> requirements)
    {
        if (!Resolve(surgery, ref surgery.Comp))
            return null;

        if (requirements.Contains(surgery))
            throw new ArgumentException($"Surgery {surgery} has a requirement loop: {string.Join(", ", requirements)}");

        requirements.Add(surgery);

        if (surgery.Comp.Requirement is { } requirementId &&
            GetSingleton(requirementId) is { } requirement &&
            GetNextStep(body, part, requirement, requirements) is { } requiredNext)
        {
            return requiredNext;
        }

        for (var i = 0; i < surgery.Comp.Steps.Count; i++)
        {
            var surgeryStep = surgery.Comp.Steps[i];
            if (!IsStepComplete(body, part, surgeryStep))
                return ((surgery, surgery.Comp), i);
        }

        return null;
    }

    public (Entity<CMSurgeryComponent> Surgery, int Step)? GetNextStep(EntityUid body, EntityUid part, EntityUid surgery)
    {
        return GetNextStep(body, part, surgery, new List<EntityUid>());
    }

    public bool PreviousStepsComplete(EntityUid body, EntityUid part, Entity<CMSurgeryComponent> surgery, EntProtoId step)
    {
        // TODO CM14 use index instead of the prototype id
        if (surgery.Comp.Requirement is { } requirement)
        {
            if (GetSingleton(requirement) is not { } requiredEnt ||
                !TryComp(requiredEnt, out CMSurgeryComponent? requiredComp) ||
                !PreviousStepsComplete(body, part, (requiredEnt, requiredComp), step))
            {
                return false;
            }
        }

        foreach (var surgeryStep in surgery.Comp.Steps)
        {
            if (surgeryStep == step)
                break;

            if (!IsStepComplete(body, part, surgeryStep))
                return false;
        }

        return true;
    }

    public bool CanPerformStep(EntityUid user, EntityUid body, BodyPartType part, EntityUid step, bool doPopup, out string? popup, out StepInvalidReason reason, out HashSet<EntityUid>? validTools)
    {
        var slot = part switch
        {
            BodyPartType.Head => SlotFlags.HEAD,
            BodyPartType.Torso => SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING,
            BodyPartType.Arm => SlotFlags.OUTERCLOTHING | SlotFlags.INNERCLOTHING,
            BodyPartType.Hand => SlotFlags.GLOVES,
            BodyPartType.Leg => SlotFlags.OUTERCLOTHING | SlotFlags.LEGS,
            BodyPartType.Foot => SlotFlags.FEET,
            BodyPartType.Tail => SlotFlags.NONE,
            BodyPartType.Other => SlotFlags.NONE,
            _ => SlotFlags.NONE
        };

        var check = new CMSurgeryCanPerformStepEvent(user, body, GetTools(user), slot);
        RaiseLocalEvent(step, ref check);
        popup = check.Popup;
        validTools = check.ValidTools;

        if (check.Invalid != StepInvalidReason.None)
        {
            if (doPopup && check.Popup != null)
                _popup.PopupEntity(check.Popup, user, PopupType.SmallCaution);

            reason = check.Invalid;
            return false;
        }

        reason = default;
        return true;
    }

    public bool CanPerformStep(EntityUid user, EntityUid body, BodyPartType part, EntityUid step, bool doPopup)
    {
        return CanPerformStep(user, body, part, step, doPopup, out _, out _, out _);
    }

    public bool IsStepComplete(EntityUid body, EntityUid part, EntProtoId step)
    {
        if (GetSingleton(step) is not { } stepEnt)
            return false;

        var ev = new CMSurgeryStepCompleteCheckEvent(body, part);
        RaiseLocalEvent(stepEnt, ref ev);

        return !ev.Cancelled;
    }

    private bool AnyHaveComp(List<EntityUid> tools, IComponent component, out EntityUid withComp)
    {
        foreach (var tool in tools)
        {
            if (HasComp(tool, component.GetType()))
            {
                withComp = tool;
                return true;
            }
        }

        withComp = default;
        return false;
    }
}
