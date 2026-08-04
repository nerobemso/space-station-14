using Content.Shared.SS220.Recolorable;
using Content.Shared.Verbs;
using Robust.Client.GameObjects;

namespace Content.Client.SS220.Recolorable;

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

        SubscribeLocalEvent<RecolorableComponent, GetVerbsEvent<AlternativeVerb>>(AddRecolorVerb);
    }

    private void AddRecolorVerb(Entity<RecolorableComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        AlternativeVerb verb = new()
        {
            Text = Loc.GetString("recolorable-verb-get-data-text"),
            Act = () =>
            {
                var window = new LayerColorEditor();
                window.SetTarget(ent.Owner);
                window.OpenCentered();
            },
        };

        args.Verbs.Add(verb);
    }
}
