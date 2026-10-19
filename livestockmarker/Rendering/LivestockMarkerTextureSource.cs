
using System;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace livestockmarker.Rendering;

public sealed class LivestockMarkerTextureSource : ITexPositionSource
{
    private readonly ITexPositionSource original;
    private readonly TextureAtlasPosition markedHide;

    public LivestockMarkerTextureSource(
        ITexPositionSource original,
        TextureAtlasPosition markedHide)
    {
        this.original = original
            ?? throw new ArgumentNullException(nameof(original));

        this.markedHide = markedHide
            ?? throw new ArgumentNullException(nameof(markedHide));
    }

    public Size2i? AtlasSize => original.AtlasSize;

    public TextureAtlasPosition? this[string textureCode]
    {
        get
        {
            if (textureCode == "hide")
            {
                return markedHide;
            }

            return original[textureCode];
        }
    }
}
