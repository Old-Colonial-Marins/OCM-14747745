using Content.Shared._TGMC14.Xenoids.Widow;
using Content.Shared.Mobs;

namespace Content.Shared._TGMC14.Xenoids.Spiderling;

public sealed class SpiderlingSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<SpiderlingComponent, MobStateChangedEvent>(OnSpiderlingDead);
        SubscribeLocalEvent<SpiderlingComponent, ComponentRemove>(OnSpiderlingDelete);
    }

    private void OnSpiderlingDead(Entity<SpiderlingComponent> spider, ref MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        var wComp = EnsureComp<WidowComponent>(spider.Comp.Widow);
        wComp.Spiders.Remove(spider);
    }

    private void OnSpiderlingDelete(Entity<SpiderlingComponent> spider, ref ComponentRemove args)
    {
        var wComp = EnsureComp<WidowComponent>(spider.Comp.Widow);
        wComp.Spiders.Remove(spider);
    }
}
