using Content.Server.PowerCell;
using Content.Shared.Weapons.Misc;
using Robust.Shared.Physics.Components;
using Content.Shared.PowerCell;

namespace Content.Server.Weapons.Misc;

public sealed class TetherGunSystem : SharedTetherGunSystem
{
    [Dependency] private readonly PowerCellSystem _cell = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<TetherGunComponent, PowerCellSlotEmptyEvent>(OnGunEmpty);
        SubscribeLocalEvent<ForceGunComponent, PowerCellSlotEmptyEvent>(OnGunEmpty);
    }

    private void OnGunEmpty(EntityUid uid, BaseForceGunComponent component, ref PowerCellSlotEmptyEvent args)
    {
        StopTether(uid, component);
    }

    protected override bool CanTether(EntityUid uid, BaseForceGunComponent component, EntityUid target, EntityUid? user)
    {
        if (!base.CanTether(uid, component, target, user))
            return false;

        if (!_cell.HasDrawCharge(uid, user: user))
            return false;

        return true;
    }

    protected override void StartTether(EntityUid gunUid, BaseForceGunComponent component, EntityUid target, EntityUid? user,
        PhysicsComponent? targetPhysics = null, TransformComponent? targetXform = null)
    {
        base.StartTether(gunUid, component, target, user, targetPhysics, targetXform);
        _cell.SetPowerCellDrawEnabled(gunUid, true);
    }

    protected override void StopTether(EntityUid gunUid, BaseForceGunComponent component, bool land = true, bool transfer = false)
    {
        base.StopTether(gunUid, component, land, transfer);
        _cell.SetPowerCellDrawEnabled(gunUid, false);
    }
}
