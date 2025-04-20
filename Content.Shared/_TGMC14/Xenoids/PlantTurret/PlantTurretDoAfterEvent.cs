using Content.Shared.DoAfter;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._TGMC14.Xenoids.PlantTurret;

[Serializable, NetSerializable]
public sealed partial class PlantTurretDoAfterEvent : SimpleDoAfterEvent
{
    [DataField]
    public NetCoordinates Coordinates;

    public PlantTurretDoAfterEvent(NetCoordinates coordinates)
    {
        Coordinates = coordinates;
    }
}