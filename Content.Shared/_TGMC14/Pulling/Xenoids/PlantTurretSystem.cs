using Robust.Shared.Network;
using Content.Shared.DoAfter;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._TGMC14.Xenoids.PlantTurret;

public sealed class PlantTurretSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    
    public override void Initialize()
    {
        SubscribeLocalEvent<PlantTurretComponent, PlantTurretActionEvent>(OnPlantTurretAction);
        SubscribeLocalEvent<PlantTurretComponent, PlantTurretDoAfterEvent>(OnPlantTurretDoAfterEvent);
    }

    private void OnPlantTurretAction(Entity<PlantTurretComponent> xeno, ref PlantTurretActionEvent args)
    {
        if(args.Handled)
            return;

        if(Transform(args.Target.EntityId).GridUid == null)
            return;

        args.Handled = true;

        var ev = new PlantTurretDoAfterEvent(GetNetCoordinates(args.Target));
        var doAfter = new DoAfterArgs(EntityManager, xeno, xeno.Comp.PlantDelay, ev, xeno)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
        };

        _doAfter.TryStartDoAfter(doAfter);
    }

    private void OnPlantTurretDoAfterEvent(Entity<PlantTurretComponent> xeno, ref PlantTurretDoAfterEvent args)
    {
        if (args.Cancelled)
            return;

        if(_net.IsServer)
        {
            Spawn(xeno.Comp.Prototype, GetCoordinates(args.Coordinates));
            _audio.PlayPvs(xeno.Comp.Sound, xeno);
        }
    }
}
