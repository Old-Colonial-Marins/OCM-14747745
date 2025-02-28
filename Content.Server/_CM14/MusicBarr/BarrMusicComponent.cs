using Robust.Shared.Audio;

namespace Content.Server._CM14.BarrMusic;

[RegisterComponent]
public sealed partial class BarrMusicComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan PlayMusicTime = TimeSpan.FromMinutes(25);

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan PlayMusicNext = TimeSpan.FromSeconds(0);

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public List<SoundPathSpecifier> MusicPool =
    [
        new SoundPathSpecifier("/Audio/Misc/notice1.ogg"),
    ];

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool Enabled = true;
}
