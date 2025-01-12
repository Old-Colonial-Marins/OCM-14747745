using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Shared._CM14.XenoPullingRules.Components;

/// <summary>
/// Specifies xenos blacklist of items it cannot pull by PullerComponent and PullingSystem
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(Systems.XenoPullingRulesSystem))]
public sealed partial class XenoPullerRulesComponent : Component
{
    /// <summary>
    /// Is it enabled
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Enabled = true;

    /// <summary>
    /// Is the puller should be xenos
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public bool IsCheckXeno = true;

    /// <summary>
    /// Blacklist of pullable entities prototype by xenos
    /// </summary>
    [DataField, AutoNetworkedField]
    [ViewVariables(VVAccess.ReadWrite)]
    public List<EntProtoId> Blacklist = []; // TODO: should be IList or ICollection, but otherwise string prototypes doesn't work
}
