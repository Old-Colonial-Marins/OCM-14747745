using Content.Shared._CM14.Xeno;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared._CM14.Xeno.Components;
using Content.Shared._CM14.Xenos;
using Robust.Server.GameObjects;

namespace Content.Server._CM14.Xeno;

public sealed partial class XenoSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoComponent, ComponentStartup>(OnStartup);
    }

    //<OCM
    public bool CanAbilityAttackTarget(EntityUid xeno, EntityUid target)
    {
        if (xeno == target)
            return false;

        if (HasComp<XenoComponent>(target))
            return false;

        if (HasComp<MarineComponent>(target))
            return true;
    }
    //OCM>

    private void OnStartup(Entity<XenoComponent> xeno, ref ComponentStartup args)
    {
        OnActionsStartup(xeno, ref args);
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<XenoComponent>();
        while (query.MoveNext(out var uid, out var xeno))
        {
            // Update resting
            xeno.OnResting = HasComp<XenoRestingComponent>(uid);

            // Update weeds
            xeno.OnWeeds = false;
            foreach (var contact in _physics.GetContactingEntities(uid))
            {
                if (HasComp<XenoWeedsComponent>(contact))
                {
                    xeno.OnWeeds = true;
                    break;
                }
            }

            // Update regen
            var healthRegen = xeno.HealthRegen;
            if (xeno.OnResting)
            {
                healthRegen = xeno.OnWeeds ? xeno.HealthRegenOnWeeds : xeno.HealthRegenOnRest;
            }

            if (TryComp<DamageableComponent>(uid, out var damageable))
                HealDamage((uid, damageable), healthRegen * frameTime);

            var ev = new XenoRegenEvent();
            RaiseLocalEvent(uid, ref ev);

            Dirty(uid, xeno);
        }
    }

    public void HealDamage(Entity<DamageableComponent?> ent, float amount)
    {
        if (!Resolve(ent, ref ent.Comp))
            return;

        var heal = new DamageSpecifier();
        foreach (var (type, typeAmount) in ent.Comp.Damage.DamageDict)
        {
            var total = heal.GetTotal();
            if (typeAmount + total >= amount)
            {
                var change = -FixedPoint2.Min(typeAmount, amount - total);
                if (!heal.DamageDict.TryAdd(type, change))
                    heal.DamageDict[type] += change;

                break;
            }

            if (!heal.DamageDict.TryAdd(type, -typeAmount))
                heal.DamageDict[type] += -typeAmount;
        }

        if (heal.GetTotal() < FixedPoint2.Zero)
            _damageable.TryChangeDamage(ent, heal);
    }
}
