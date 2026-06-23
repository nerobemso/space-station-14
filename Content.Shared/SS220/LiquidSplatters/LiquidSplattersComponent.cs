using Robust.Shared.GameStates;

namespace Content.Shared.SS220.LiquidSplatters;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
public sealed partial class LiquidSplattersComponent : Component
{
    [DataField("enabled"), AutoNetworkedField]
    public bool Enabled = true;

    [DataField("intensity"), AutoNetworkedField]
    public float Intensity = .5f;
}
