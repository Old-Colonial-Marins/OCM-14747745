using Robust.Shared.Audio;

namespace Content.Shared._TGMC14.Pulling;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class TGMCPullingComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan StunPullingTime = TimeSpan.FromSeconds(5.5);

    [DataField, AutoNetworkedField]
    public SoundSpecifier SoundOnStun = new SoundPathSpecifier("/Audio/_CM/Voice/alien_growl1.ogg");
}