using Robust.Shared.GameStates;

namespace Content.Shared._TGMC14.Xenoids.Spiderling;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpiderlingComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid Widow;
}
