using Robust.Shared.Prototypes;

namespace Content.Shared._TGMC14.Pulling;

[RegisterComponent, AutoGenerateComponentState]
public sealed partial class MarinePullingComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntProtoId GrabEffect = "TGMCEffectGrab";
}
