using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Items.Donate;

namespace FargowiltasCrossmod.Core.Thorium.Globals;

[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
public class ThoriumItemNerfs : GlobalItem
{
    public override bool AppliesToEntity(Item entity, bool lateInstantiation)
    {
        return WorldSavingSystem.EternityMode;
    }

    public override void SetDefaults(Item entity)
    {
        if (entity.type == ModContent.ItemType<BalanceBloom>())
        {
            entity.damage = (int)(entity.damage * (2f / 3f));
        }
    }
}