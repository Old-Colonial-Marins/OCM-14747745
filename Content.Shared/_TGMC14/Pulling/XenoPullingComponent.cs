using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._TGMC14.Pulling;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class XenoPullingComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan StunTime = TimeSpan.FromSeconds(3.5);

    [DataField, AutoNetworkedField]
    public TimeSpan DisarmTime = TimeSpan.FromSeconds(1.5);

    [DataField, AutoNetworkedField]
    public SoundSpecifier StunSound = new SoundPathSpecifier("/Audio/Weapons/punch2.ogg");

    [DataField, AutoNetworkedField]
    public SoundSpecifier DisarmSound = new SoundPathSpecifier("/Audio/Weapons/punchmiss.ogg");

    [DataField, AutoNetworkedField]
    public int DisarmChance = 30;

    [DataField, AutoNetworkedField]
    public EntProtoId GrabEffect = "TGMCEffectGrab";

    [DataField, AutoNetworkedField]
    public EntProtoId DisarmEffect = "TGMCEffectDisarm";

    [DataField, AutoNetworkedField]
    public EntProtoId PunchEffect = "TGMCEffectPouch";
}
