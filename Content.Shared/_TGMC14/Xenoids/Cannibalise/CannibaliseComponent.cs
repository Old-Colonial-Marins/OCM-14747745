using Robust.Shared.GameStates;

namespace Content.Shared._TGMC14.Xenoids.Cannibalise;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CannibaliseComponent : Component
{
    [DataField, AutoNetworkedField]
    public float RegenMultiply = 1;
}
