using System.Collections.Generic;
using System.Linq;
using Content.Shared.Clothing;
using Content.Shared.Inventory.Events;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Content.Shared.SS220.LiquidSplatters;

namespace Content.Client.SS220.LiquidSplatters;

public sealed class LiquidSplattersSystem : EntitySystem
{
    private static readonly ProtoId<ShaderPrototype> Shader = "LiquidSplatters";

    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly SpriteSystem _sprite = default!;

    private readonly Dictionary<EntityUid, ShaderInstance> _shaders = new();
    private readonly Dictionary<EntityUid, EquippedLayers> _equippedLayers = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<LiquidSplattersComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<LiquidSplattersComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<LiquidSplattersComponent, AfterAutoHandleStateEvent>(OnAfterAutoHandleState);
        SubscribeLocalEvent<LiquidSplattersComponent, EquipmentVisualsUpdatedEvent>(OnEquipmentVisualsUpdated);
        SubscribeLocalEvent<LiquidSplattersComponent, GotUnequippedEvent>(OnGotUnequipped);
    }

    private void OnAfterAutoHandleState(Entity<LiquidSplattersComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        SetShader(ent, ent.Comp.Enabled);
    }

    private void OnStartup(Entity<LiquidSplattersComponent> ent, ref ComponentStartup args)
    {
        SetShader(ent, ent.Comp.Enabled);
    }

    private void OnShutdown(Entity<LiquidSplattersComponent> ent, ref ComponentShutdown args)
    {
        SetShader(ent, false);
    }

    private void OnEquipmentVisualsUpdated(Entity<LiquidSplattersComponent> ent, ref EquipmentVisualsUpdatedEvent args)
    {
        _equippedLayers[ent] = new EquippedLayers(args.Equipee, new HashSet<string>(args.RevealedLayers));
        SetShader(ent, ent.Comp.Enabled);
    }

    private void OnGotUnequipped(Entity<LiquidSplattersComponent> ent, ref GotUnequippedEvent args)
    {
        _equippedLayers.Remove(ent);
    }

    private void SetShader(Entity<LiquidSplattersComponent> ent, bool enabled)
    {
        TryComp(ent, out SpriteComponent? sprite);

        if (!enabled)
        {
            if (sprite != null)
                ClearLayerShaders(sprite);

            if (_equippedLayers.TryGetValue(ent, out var equipped))
                ClearEquipmentLayerShaders(equipped);

            _shaders.Remove(ent);
            return;
        }

        if (!_shaders.TryGetValue(ent, out var shader))
        {
            shader = _protoMan.Index(Shader).InstanceUnique();
            _shaders[ent] = shader;
        }

        shader.SetParameter("intensity", ent.Comp.Intensity);

        if (sprite != null)
            ApplyLayerShaders(sprite, shader);

        if (_equippedLayers.TryGetValue(ent, out var equippedLayers))
            ApplyEquipmentLayerShaders(equippedLayers, shader);
    }

    private static void ClearLayerShaders(SpriteComponent sprite)
    {
        if (!sprite.AllLayers.Any())
            return;

        if (sprite[0] is not SpriteComponent.Layer layer || layer.ShaderPrototype != Shader.Id)
            return;

        sprite.LayerSetShader(0, null, null);
    }

    private static void ApplyLayerShaders(SpriteComponent sprite, ShaderInstance shader)
    {
        if (!sprite.AllLayers.Any())
            return;

        sprite.LayerSetShader(0, shader, Shader.Id);
    }

    private void ApplyEquipmentLayerShaders(EquippedLayers equipped, ShaderInstance shader)
    {
        if (!TryComp(equipped.Wearer, out SpriteComponent? sprite))
            return;

        foreach (var key in equipped.LayerKeys)
        {
            if (!_sprite.LayerMapTryGet((equipped.Wearer, sprite), key, out var layer, false))
                continue;

            sprite.LayerSetShader(layer, shader, Shader.Id);
        }
    }

    private void ClearEquipmentLayerShaders(EquippedLayers equipped)
    {
        if (!TryComp(equipped.Wearer, out SpriteComponent? sprite))
            return;

        foreach (var key in equipped.LayerKeys)
        {
            if (!_sprite.LayerMapTryGet((equipped.Wearer, sprite), key, out var layerIndex, false))
                continue;

            if (sprite[layerIndex] is not SpriteComponent.Layer layer || layer.ShaderPrototype != Shader.Id)
                continue;

            sprite.LayerSetShader(layerIndex, null, null);
        }
    }

    private sealed record EquippedLayers(EntityUid Wearer, HashSet<string> LayerKeys);
}
