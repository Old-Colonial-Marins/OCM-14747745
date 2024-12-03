using Content.Shared._CM14.Xeno.Action;
using Robust.Shared.Prototypes;

namespace Content.Shared._CM14.Xeno.BuildTurret;

[RegisterComponent]
public sealed partial class XenoBuildTurretComponent : XenoActionComponent
{
    [DataField]
    public override float PlasmaCost { get; set; } = 3f;

    [DataField]
    public EntProtoId Action = "ActionXenoBuildTurret";

    [DataField]
    public EntProtoId WallPrototype = "WeaponTurretXeno";

    [DataField]
    public float TimeUsage = 4.5f;
}
