
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace livestockmarker.Growth;

public class EntityBehaviorLivestockGrow
    : EntityBehaviorGrow
{
    public EntityBehaviorLivestockGrow(Entity entity)
        : base(entity)
    {
    }

    protected override void BecomeAdult(
        Entity adult,
        bool keepTextureIndex)
    {
        // Przenosimy oznaczenie przed zastąpieniem
        // młodego zwierzęcia dorosłym.
        string color = AnimalMarkingService.GetColor(entity);

        if (color != "none")
        {
            AnimalMarkingService.SetColor(adult, color);
        // AnimalMarkingService.CopyMarks(entity, adult);
        }

        // Oryginalna mechanika gry:
        // pokolenie, tekstura, usunięcie młodego,
        // utworzenie dorosłego.
        base.BecomeAdult(adult, keepTextureIndex);
    }
}
