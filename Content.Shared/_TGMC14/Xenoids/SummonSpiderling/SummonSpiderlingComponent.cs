using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared._TGMC14.Xenoids.SummonSpiderling;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SummonSpiderlingComponent : Component
{
    [DataField]
    public DamageSpecifier Damage = new();

    [DataField, AutoNetworkedField]
    public EntProtoId Prototype = "MobSpiderlingXeno";

    [DataField, AutoNetworkedField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_CM/Voice/alien_growl1.ogg");
}
