using Content.Shared._CM14.Xeno;
using Content.Shared._CM14.XenoPullingRules.Components;
using Content.Shared.Movement.Pulling.Events;
using Robust.Shared.Physics.Systems;
using System.Linq;

namespace Content.Shared._CM14.XenoPullingRules.Systems;

/// <summary>
/// Allows only pull entities that's not in blacklist, to work need via PullingSystem. Only works on xenos
/// </summary>
public sealed class XenoPullingRulesSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        UpdatesAfter.Add(typeof(SharedPhysicsSystem));
        UpdatesOutsidePrediction = true;

        SubscribeLocalEvent<XenoPullerRulesComponent, PullAttemptEvent>(OnPullableMoveInput);
    }

    private void OnPullableMoveInput(Entity<XenoPullerRulesComponent> ent, ref PullAttemptEvent args)
    {
        var (_, comp) = ent;

        if (!comp.Enabled)
            return;

        if (!TryComp<MetaDataComponent>(args.PulledUid, out var metaData))
            return;

        if (comp.IsCheckXeno && !TryComp<XenoComponent>(args.PullerUid, out _))
            return;

        if (metaData?.EntityPrototype is null)
            return;


        args.Cancelled = comp.Blacklist.Contains(metaData.EntityPrototype.ID);
    }
}
