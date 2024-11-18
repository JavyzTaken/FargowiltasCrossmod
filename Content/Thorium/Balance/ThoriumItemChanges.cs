
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Balance
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    class ThoriumItemChanges : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return true;
        }
    }
}

