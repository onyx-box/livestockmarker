
using System;
using System.Text;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using livestockmarker.Rendering;

namespace livestockmarker;

public class EntityBehaviorLivestockMarker : EntityBehavior
{

    private bool meshLogged;
    private ITexPositionSource? markedTextureSource;
    private string lastRenderedColor = "none";

    public EntityBehaviorLivestockMarker(Entity entity)
        : base(entity)
    {
    }

    public override void Initialize(EntityProperties properties, JsonObject attributes)
    {
        base.Initialize(properties, attributes);

        if (entity.Api.Side != EnumAppSide.Client)
        {
            return;
        }

        lastRenderedColor = AnimalMarkingService.GetColor(entity);

        entity.WatchedAttributes.RegisterModifiedListener(
            "livestockmarker:color",
            OnMarkingChanged
        );
    }

    private void OnMarkingChanged()
    {
        string newColor = AnimalMarkingService.GetColor(entity);

        if (newColor == lastRenderedColor)
        {
            return;
        }

        lastRenderedColor = newColor;
        markedTextureSource = null;

        entity.MarkShapeModified();

        entity.Api.Logger.Notification(
            $"[LivestockMarker] Marking changed: entity={entity.EntityId}, color={newColor}"
        );
    }

    public override string PropertyName()
    {
        return "livestockmarker";
    }


    public override void GetInfoText(StringBuilder info)
    {
        string color = AnimalMarkingService.GetColor(entity);

        if (string.IsNullOrEmpty(color) || color == "none")
        {
            return;
        }

        string translatedColor = Lang.Get(
            $"livestockmarker:color-{color}"
        );

        info.AppendLine(
            Lang.Get("livestockmarker:mark-color", translatedColor)
        );
    }

    public override void OnTesselation(
        ref Shape entityShape,
        string shapePathForLogging,
        ref bool shapeIsCloned,
        ref string[] willDeleteElements)
    {
        base.OnTesselation(
            ref entityShape,
            shapePathForLogging,
            ref shapeIsCloned,
            ref willDeleteElements
        );

        if (meshLogged || !entity.Code.Path.StartsWith("goat-"))
        {
            return;
        }

        meshLogged = true;

        LogShapeElements(entityShape);

        string color = AnimalMarkingService.GetColor(entity);

        entity.Api.Logger.Notification(
            "[LivestockMarker] Goat mesh: " +
            $"id={entity.EntityId}, " +
            $"color={color}, " +
            $"shape={shapePathForLogging}, " +
            $"elements={entityShape.Elements?.Length ?? 0}"
        );
    }

    private void LogShapeElements(Shape shape)
    {
        if (shape.Elements == null) return;

        foreach (var element in shape.Elements)
        {
            LogElement(element, 0);
        }
    }

    private void LogElement(ShapeElement element, int depth)
    {
        string indent = new string(' ', depth * 2);

        entity.Api.Logger.Notification(
            $"[LM SHAPE] {indent}{element.Name} " +
            $"from=[{string.Join(", ", element.From ?? [])}] " +
            $"to=[{string.Join(", ", element.To ?? [])}]"
        );

        if (element.Children == null) return;

        foreach (var child in element.Children)
        {
            LogElement(child, depth + 1);
        }
    }
    
   
    public override ITexPositionSource? GetTextureSource(
    ref EnumHandling handling)
    {
        if (!TryGetAnimalDescriptor(out string species, out string variant, out string age))
        {
            return null;
        }

        // Na razie wspieramy tylko dorosłe kozy Valais
        if (species != "goat" || variant != "valais" || age != "adult")
        {
            return null;
        }

        string color = AnimalMarkingService.GetColor(entity);

        if (string.IsNullOrWhiteSpace(color) || color == "none")
        {
            return null;
        }

        if (entity.Api is not ICoreClientAPI capi)
        {
            return null;
        }

        if (markedTextureSource == null)
        {
            TextureAtlasPosition? position = livestockmarkerModSystem.TextureService?
                .GetOrCreate(species, variant, age, color);

            if (position == null)
            {
                return null;
            }

            int textureIndex = entity.WatchedAttributes.GetInt("textureIndex");

            ITexPositionSource original =
                capi.Tesselator.GetTextureSource(entity, null, textureIndex);

            markedTextureSource = new LivestockMarkerTextureSource(
                original,
                position
            );

            capi.Logger.Notification(
                $"[LivestockMarker] Texture override: entity={entity.EntityId}, type={entity.Code}, color={color}"
            );
        }

        handling = EnumHandling.PreventSubsequent;
        return markedTextureSource;
    }

    private bool TryGetAnimalDescriptor(
    out string species,
    out string variant,
    out string age)
    {
        species = "";
        variant = "";
        age = "";

        string path = entity.Code?.Path ?? "";

        // przykład: goat-valais-adult-female
        string[] parts = path.Split('-', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
        {
            return false;
        }

        species = parts[0];
        variant = parts[1];
        age = parts[2];

        return true;
    }
}
