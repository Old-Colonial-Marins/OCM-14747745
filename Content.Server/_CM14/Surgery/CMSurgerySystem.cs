using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared._CM14.Marine;
using Content.Shared._CM14.Surgery;
using Content.Shared._CM14.Surgery.Conditions;
using Content.Shared._CM14.Surgery.Effects.Step;
using Content.Shared._CM14.Surgery.Tools;
using Content.Shared.Damage;
using Content.Shared.Interaction;
using Content.Shared.Prototypes;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._CM14.Surgery;

public sealed class CMSurgerySystem : SharedCMSurgerySystem
{
    [Dependency] private readonly BloodstreamSystem _bloodstream = default!;
    [Dependency] private readonly BodySystem _body = default!;
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;

    private readonly List<EntProtoId> _surgeries = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CMSurgeryToolComponent, AfterInteractEvent>(OnToolAfterInteract);
        SubscribeLocalEvent<CMSurgeryStepBleedEffectComponent, CMSurgeryStepEvent>(OnStepBleedComplete);
        SubscribeLocalEvent<CMSurgeryStepEmoteEffectComponent, CMSurgeryStepEvent>(OnStepScreamComplete);
        SubscribeLocalEvent<CMSurgeryStepDamageEffectComponent, CMSurgeryStepEvent>(OnStepsDamageComplete);

        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypesReloaded);

        LoadPrototypes();
    }
    protected override void RefreshUI(EntityUid body)
    {
        if (!HasComp<CMSurgeryTargetComponent>(body))
            return;

        var surgeries = new Dictionary<NetEntity, List<EntProtoId>>();
        foreach (var surgery in _surgeries)
        {
            if (GetSingleton(surgery) is not { } surgeryEnt)
                continue;

            foreach (var part in _body.GetBodyChildren(body))
            {
                var ev = new CMSurgeryValidEvent(body, part.Id);
                RaiseLocalEvent(surgeryEnt, ref ev);

                if (ev.Cancelled)
                    continue;

                surgeries.GetOrNew(GetNetEntity(part.Id)).Add(surgery);
            }
        }

        _ui.SetUiState(body, CMSurgeryUIKey.Key, new CMSurgeryBuiState(surgeries));
    }

    private void OnToolAfterInteract(Entity<CMSurgeryToolComponent> ent, ref AfterInteractEvent args)
    {
        if (args.Handled ||
            args.Target == null ||
            !TryComp(args.User, out ActorComponent? actor) ||
            !HasComp<CMSurgeryTargetComponent>(args.Target))
        {
            return;
        }

        if (!TryComp(args.User, out SkillsComponent? skills) ||
            skills.Surgery < 1)
        {
            _popup.PopupEntity("Вы не знаете, как проводить хирургические операции!", args.User, args.User);
            return;
        }

        if (args.User == args.Target)
        {
            _popup.PopupEntity("Вы не можете прооперировать самого себя!", args.User, args.User);
            return;
        }

        args.Handled = true;
        _ui.OpenUi(args.Target.Value, CMSurgeryUIKey.Key, actor.PlayerSession);

        RefreshUI(args.Target.Value);
    }

    private void OnStepBleedComplete(Entity<CMSurgeryStepBleedEffectComponent> ent, ref CMSurgeryStepEvent args)
    {
        _bloodstream.TryModifyBleedAmount(args.Body, ent.Comp.Amount);
    }

    private void OnStepScreamComplete(Entity<CMSurgeryStepEmoteEffectComponent> ent, ref CMSurgeryStepEvent args)
    {
        _chat.TryEmoteWithChat(args.Body, ent.Comp.Emote);
    }
    private void OnStepsDamageComplete(Entity<CMSurgeryStepDamageEffectComponent> ent, ref CMSurgeryStepEvent args)
    {
        Log.Debug("Change damage.");
        Log.Debug(ent.Comp.Damage.GetTotal().ToString());
        _damageable.TryChangeDamage(ent, ent.Comp.Damage, true, false);
    }


    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<EntityPrototype>())
            LoadPrototypes();
    }

    private void LoadPrototypes()
    {
        _surgeries.Clear();

        foreach (var entity in _prototypes.EnumeratePrototypes<EntityPrototype>())
        {
            if (entity.HasComponent<CMSurgeryComponent>())
                _surgeries.Add(new EntProtoId(entity.ID));
        }
    }
}
