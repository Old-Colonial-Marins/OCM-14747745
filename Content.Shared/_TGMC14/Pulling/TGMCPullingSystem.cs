using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Audio.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Network;

using Content.Shared._CM14.Xeno;
using Robust.Shared.Random;

namespace Content.Shared._TGMC14.Pulling;

public sealed class RMCPullingSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    
    public override void Initialize()
    {
        SubscribeLocalEvent<MarinePullingComponent, PullAttemptEvent>(OnMarinePullAttempt);
        SubscribeLocalEvent<XenoPullingComponent, PullAttemptEvent>(OnXenoPullAttempt);
    }

    private void OnMarinePullAttempt(Entity<MarinePullingComponent> marine, ref PullAttemptEvent args)
    {
        var target = args.PulledUid;

        if (_net.IsServer)
            SpawnAttachedTo(marine.Comp.GrabEffect, Transform(target).Coordinates);

        if (!TryComp<XenoPullingComponent>(target, out var xpComp))
            return;

        if (!_mobState.IsAlive(target))
            return;

        args.Cancelled = true;

        _stun.TryParalyze(marine, xpComp.StunTime, true);
        if (_net.IsServer)
        {
            _audio.PlayPvs(xpComp.StunSound, target);
            SpawnAttachedTo(xpComp.PunchEffect, Transform(marine).Coordinates);
        }
    }

    private void OnXenoPullAttempt(Entity<XenoPullingComponent> xeno, ref PullAttemptEvent args)
    {
        var target = args.PulledUid;

        if(_net.IsServer)
            SpawnAttachedTo(xeno.Comp.GrabEffect, Transform(target).Coordinates);

        if (!TryComp<MarinePullingComponent>(target, out var mpComp))
            return;

        var chance = _random.Next(1, 100);
        if (chance > xeno.Comp.DisarmChance)
            return;

        _stun.TryParalyze(target, xeno.Comp.DisarmTime, true);
        if (_net.IsServer)
        {
            SpawnAttachedTo(xeno.Comp.DisarmEffect, Transform(target).Coordinates);
            _audio.PlayPvs(xeno.Comp.DisarmSound, target);
        }
    }
}
