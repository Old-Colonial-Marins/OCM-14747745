using Robust.Shared.GameStates;

namespace Content.Shared._TGMC14.Xenoids.Widow;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WidowComponent : Component
{
    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> Spiders = new();

    [DataField, AutoNetworkedField]
    public int MaxSpiders = 10;
}
