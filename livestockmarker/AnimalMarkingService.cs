
using Vintagestory.API.Common.Entities;

namespace livestockmarker;

public static class AnimalMarkingService
{
    private const string ColorKey = "livestockmarker:color";

    public static string GetColor(Entity entity)
    {
        return entity.WatchedAttributes
            .GetString(ColorKey, "none");
    }

    public static void SetColor(Entity entity, string color)
    {
        entity.WatchedAttributes.SetString(ColorKey, color);
        entity.WatchedAttributes.MarkPathDirty(ColorKey);
    }

    public static void Clear(Entity entity)
    {
        entity.WatchedAttributes.RemoveAttribute(ColorKey);
        entity.WatchedAttributes.MarkPathDirty(ColorKey);
    }
}
