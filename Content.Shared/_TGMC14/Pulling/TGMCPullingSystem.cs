using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Audio.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Network;

using Content.Shared._CM14.Xeno;

namespace Content.Shared._TGMC14.Pulling;

public sealed class RMCPullingSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;    
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;    
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    
    public override void Initialize()
    {
        SubscribeLocalEvent<TGMCPullingComponent, PullAttemptEvent>(OnParalyzeOnPull);
    }

    private void OnParalyzeOnPull(Entity<TGMCPullingComponent> ent, ref PullAttemptEvent args)
    {
        var target = args.PulledUid;

        if(_mobState.IsDead(target) || !HasComp<XenoComponent>(target))
            return;

        args.Cancelled = true;

        if (_net.IsServer)
            _audio.PlayPvs(ent.Comp.SoundOnStun, ent);
        _stun.TryParalyze(ent, ent.Comp.StunPullingTime, true);
    }
}
// /---PASXALOCHKO BY FUFELSHMERZ---\
// |                                |
// |         DEMUR STRONGER         |
// |                                |
// \--------------------------------/