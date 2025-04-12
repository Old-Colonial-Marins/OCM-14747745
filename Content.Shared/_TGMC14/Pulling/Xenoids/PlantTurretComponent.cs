using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared._TGMC14.Xenoids.PlantTurret;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlantTurretComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan PlantDelay = TimeSpan.FromSeconds(15);

    [DataField, AutoNetworkedField]
    public EntProtoId Prototype = "WeaponTurretXeno";

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Effects/Fluids/splat.ogg");
}