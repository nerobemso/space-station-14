using Robust.Client.GameObjects;

namespace Content.Shared.SS220.Recolorable;

/// <summary>
/// This handles...
/// </summary>
public sealed class RecolorableSystem : EntitySystem
{

    [Dependency] private SpriteSystem _spriteSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RecolorableComponent, ComponentInit>(OnInitialize);
    }

    private void OnInitialize(Entity<RecolorableComponent> ent, ref ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(ent.Owner, out var sprite))
            return;

        ent.Comp.ColorableLayers.Add(0);

        for (int i = 0; i < ent.Comp.ColorableLayers.Count; i++)
        {
            var layer = ent.Comp.ColorableLayers[i];
            _spriteSystem.LayerSetColor((ent, sprite), layer, ent.Comp.Color);
        }
    }

}
