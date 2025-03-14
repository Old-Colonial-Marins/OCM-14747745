using Content.Server.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared._CM14.Xeno;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using Content.Server._CM14.Xeno.Actions.Components;

namespace Content.Server._CM14.Xeno.Actions.Systems;

public sealed class XenoPsychicCureSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageableSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoPsychicCureComponent, XenoPsychicCureEvent>(OnPsychicCure);
        SubscribeLocalEvent<XenoPsychicCureComponent, XenoPsychicCureDoAfterEvent>(OnPsychicCureDoAfter);
        SubscribeLocalEvent<XenoRejuvenateProjComponent, StartCollideEvent>(OnCollide);
    }

    private void OnPsychicCureDoAfter(EntityUid uid, XenoPsychicCureComponent component,
        XenoPsychicCureDoAfterEvent args)
    {
        if (!args.Cancelled)
        {
            if (args.Target != null)
            {
                Heal((EntityUid) args.Target, component.HealAmount);
            }
        }
    }

    private void OnPsychicCure(EntityUid uid, XenoPsychicCureComponent component, XenoPsychicCureEvent args)
    {
        if (!HasComp<XenoComponent>(args.Target) || !HasComp<DamageableComponent>(args.Target))
            return;

        var doAfterEventArgs =
            new DoAfterArgs(EntityManager, uid, TimeSpan.FromSeconds(6.5f), new XenoPsychicCureDoAfterEvent(), uid,
                target: args.Target, used: uid)
            {
                BreakOnMove = true,
                NeedHand = false,
                BreakOnDamage = true,
            };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
    }

    private void Heal(EntityUid target, float healAmount)
    {
        if (!HasComp<XenoComponent>(target) || !TryComp<DamageableComponent>(target, out var damageable))
            return;

        _bloodstreamSystem.TryModifyBleedAmount(target, -healAmount);

        foreach (var type in damageable.Damage.DamageDict.Keys)
        {
            var damageInType = damageable.Damage.DamageDict[type];
            if (damageInType == 0)
                continue;

            _damageableSystem.TryChangeDamage(target,
                new DamageSpecifier(_proto.Index<DamageTypePrototype>(type), -healAmount), true);
        }
    }

    private void OnCollide(EntityUid uid, XenoRejuvenateProjComponent component, ref StartCollideEvent args)
    {
        Heal(args.OtherEntity, component.HealAmount);
    }
}
