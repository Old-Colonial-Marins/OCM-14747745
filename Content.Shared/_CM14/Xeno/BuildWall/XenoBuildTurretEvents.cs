using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CM14.Xeno.BuildTurret;

public sealed partial class XenoBuildTurretActionEvent : WorldTargetActionEvent;

[Serializable, NetSerializable]
public sealed partial class XenoBuildTurretDoAfterEvent : SimpleDoAfterEvent
{
    public readonly MapCoordinates Coordinates;

    public XenoBuildTurretDoAfterEvent(MapCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}
