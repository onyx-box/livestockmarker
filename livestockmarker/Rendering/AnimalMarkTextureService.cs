using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace livestockmarker.Rendering;

public sealed class AnimalMarkTextureService
{
    private readonly ICoreClientAPI capi;

    private readonly Dictionary<string, TextureAtlasPosition> cache = new();

    private static readonly HashSet<string> SupportedColors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "red",
            "blue",
            "green",
            "yellow",
            "white",
            "black"
        };

    public AnimalMarkTextureService(ICoreClientAPI capi)
    {
        this.capi = capi;
    }

    public TextureAtlasPosition? GetOrCreate(
        string species,
        string variant,
        string age,
        string color,
        string pattern = "spot")
    {
        if (string.IsNullOrWhiteSpace(color) || color == "none")
        {
            return null;
        }

        color = color.ToLowerInvariant();

        if (!SupportedColors.Contains(color))
        {
            capi.Logger.Warning(
                $"[LivestockMarker] Unsupported color '{color}', skipping texture override."
            );
            return null;
        }

        string key = $"{species}:{variant}:{age}:{color}:{pattern}";

        if (cache.TryGetValue(key, out var cachedPosition))
        {
            return cachedPosition;
        }

        // Na razie wersja 1: tylko dorosła koza Valais
        if (species != "goat" || variant != "valais" || age != "adult")
        {
            return null;
        }

        var location = new AssetLocation(
            "livestockmarker",
            $"textures/entity/goat/marks/valais-adult-{color}.png"
        );

        if (!capi.Assets.Exists(location))
        {
            capi.Logger.Error(
                $"[LivestockMarker] Missing texture asset: {location}"
            );
            return null;
        }

        var texture = new CompositeTexture(location);
        texture.RuntimeBake(capi, capi.EntityTextureAtlas);

        int subId = texture.Baked.TextureSubId;
        TextureAtlasPosition position = capi.EntityTextureAtlas.Positions[subId];

        cache[key] = position;

        capi.Logger.Notification(
            $"[LivestockMarker] Cached texture: {key}, subId={subId}"
        );

        return position;
    }

    public void Clear()
    {
        cache.Clear();
    }
}