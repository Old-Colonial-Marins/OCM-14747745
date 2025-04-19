namespace Content.Shared._TGMC14.Xenoids.Cannibalise;

public sealed class CannibaliseSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<CannibaliseComponent, CannibaliseActionEvent>(OnCannibalise);
    }

    private void OnCannibalise(Entity<CannibaliseComponent> xeno, ref CannibaliseActionEvent args)
    {
        //TODO хил xeno на значение из SpiderlingComponent * xeno.Comp.RegenMultiply и удаление args.Target.EntityId
        return;
    }
}
