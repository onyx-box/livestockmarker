
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Server;
using livestockmarker.Rendering;
using System.Linq;
using livestockmarker.Growth;

namespace livestockmarker;

public class livestockmarkerModSystem : ModSystem
{
    public override void Start(ICoreAPI api)
    {
        base.Start(api);

        api.RegisterEntityBehaviorClass(
            "livestockmarkergrow",
            typeof(EntityBehaviorLivestockGrow)
        );

        api.RegisterEntityBehaviorClass(
            "livestockmarker",
            typeof(EntityBehaviorLivestockMarker)
        );
    }

        public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);

        TextureService = new AnimalMarkTextureService(api);
    }

    public override void Dispose()
    {
        TextureService = null;
        base.Dispose();
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);

        var command = api.ChatCommands.Create("lm")
            .WithDescription("Livestock Marker commands")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(OnMainCommand);

        command.BeginSubCommand("info")
            .WithDescription("Show animal information")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(OnInfoCommand);

        command.BeginSubCommand("mark")
            .WithDescription("Mark animal with a color")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.chat)
            .WithArgs(api.ChatCommands.Parsers.Word("color"))
            .HandleWith(OnMarkCommand);

        command.BeginSubCommand("clear")
            .WithDescription("Remove animal marking")
            .RequiresPlayer()
            .RequiresPrivilege(Privilege.chat)
            .HandleWith(OnClearCommand);

        api.Logger.Notification(
            "[LivestockMarker] Commands registered"
        );
    }

    private static Entity? GetTarget(
        TextCommandCallingArgs args)
    {
        var entity = args.Caller.Player?
            .CurrentEntitySelection?.Entity;

        if (entity is not EntityAgent ||
            entity is EntityPlayer)
        {
            return null;
        }

        return entity;
    }

    private TextCommandResult OnMainCommand(
        TextCommandCallingArgs args)
    {
        return TextCommandResult.Success(
            "Livestock Marker v0.1.0\n" +
            "/lm info\n" +
            "/lm mark <color>\n" +
            "/lm clear"
        );
    }

    private TextCommandResult OnInfoCommand(
        TextCommandCallingArgs args)
    {
        var entity = GetTarget(args);

        if (entity == null)
        {
            return TextCommandResult.Error(
                "Look at an animal first."
            );
        }

        var color = AnimalMarkingService.GetColor(entity);

        return TextCommandResult.Success(
            $"Animal: {entity.Code}\n" +
            $"ID: {entity.EntityId}\n" +
            $"Mark: {color}"
        );
    }

    private TextCommandResult OnMarkCommand(
        TextCommandCallingArgs args)
    {
        var entity = GetTarget(args);

        if (entity == null)
        {
            return TextCommandResult.Error(
                "Look at an animal first."
            );
        }

        string color = args[0]?.ToString()?
            .Trim().ToLowerInvariant() ?? "";

        string[] allowedColors =
        {
            "red", "blue", "green",
            "yellow", "white", "black"
        };

        if (!allowedColors.Contains(color))
        {
            return TextCommandResult.Error(
                "Unknown color. Available: " +
                string.Join(", ", allowedColors)
            );
        }

        AnimalMarkingService.SetColor(entity, color);

        return TextCommandResult.Success(
            $"Animal {entity.EntityId} marked: {color}"
        );
    }

    private TextCommandResult OnClearCommand(
        TextCommandCallingArgs args)
    {
        var entity = GetTarget(args);

        if (entity == null)
        {
            return TextCommandResult.Error(
                "Look at an animal first."
            );
        }

        AnimalMarkingService.Clear(entity);

        return TextCommandResult.Success(
            $"Mark removed from animal {entity.EntityId}"
        );
    }

    
    public static AnimalMarkTextureService? TextureService
    {
        get;
        private set;
    }

}
