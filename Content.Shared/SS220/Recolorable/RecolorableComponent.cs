using Robust.Shared.GameStates;

namespace Content.Shared.SS220.Recolorable;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class RecolorableComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public List<int> ColorableLayers = new();

    [DataField, ViewVariables(VVAccess.ReadWrite), AutoNetworkedField]
    public Color Color = Color.Red;
}
