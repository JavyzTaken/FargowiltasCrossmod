using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using ThoriumMod.Items.BardItems;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.Misc;
using ThoriumMod.Items.Thorium;
using ThoriumMod.Items.TransformItems;

namespace FargowiltasCrossmod.Core.Thorium.Systems;

public class ThoriumContainerRecipeSystem : ModSystem
{
    public override void AddRecipes()
    {
        AddCrateRecipes();
    }

    private static void AddCrateRecipes()
    {
        int depthsCrate = ModContent.ItemType<AquaticDepthsCrate>();
        int depthsCrateHm = ModContent.ItemType<AbyssalCrate>();
        
        CreateCrateRecipe(ModContent.ItemType<MagicConch>(), depthsCrate, 5, depthsCrateHm);
        CreateCrateRecipe(ModContent.ItemType<SeaTurtlesBulwark>(), depthsCrate, 5, depthsCrateHm);
        CreateCrateRecipe(ModContent.ItemType<AnglerBowl>(), depthsCrate, 5, depthsCrateHm);
        CreateCrateRecipe(ModContent.ItemType<RainStone>(), depthsCrate, 5, depthsCrateHm);
        CreateCrateRecipe(ModContent.ItemType<SteelDrum>(), depthsCrate, 5, depthsCrateHm);

        int scarletCrate = ModContent.ItemType<ScarletCrate>();
        int scarletCrateHM = ModContent.ItemType<SinisterCrate>();
        
        CreateCrateRecipe(ModContent.ItemType<MixTape>(), scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ModContent.ItemType<LootRang>(), scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ModContent.ItemType<MagmaCharm>(), scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ModContent.ItemType<SpringSteps>(), scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ModContent.ItemType<DeepStaff>(), scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ItemID.LavaCharm, scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ModContent.ItemType<SpringHook>(), scarletCrate, 5, scarletCrateHM);
        CreateCrateRecipe(ModContent.ItemType<MagmaLocket>(), scarletCrate, 5, scarletCrateHM);
        
        int strangeCrate = ModContent.ItemType<StrangeCrate>();
        int strangeCrateHM = ModContent.ItemType<WondrousCrate>();
        
        CreateCrateRecipe(ModContent.ItemType<HightechSonarDevice>(), strangeCrate, 5, strangeCrateHM);
        CreateCrateRecipe(ModContent.ItemType<DrownedDoubloon>(), strangeCrate, 5, strangeCrateHM);
    }
    
    // From fargowiltas, https://github.com/Fargowilta/Fargowiltas/blob/master/Common/Systems/Recipes/ContainerRecipeSystem.cs#L424
    private static void CreateCrateRecipe(int result, int crate, int crateAmount, int hardmodeCrate, int extraItem = -1, params Condition[] conditions)
    {
        if (crate != -1)
        {
            var recipe = Recipe.Create(result);
            recipe.AddIngredient(crate, crateAmount);
            if (extraItem != -1)
            {
                recipe.AddIngredient(extraItem);
            }
            recipe.AddTile(TileID.WorkBenches);
            foreach (Condition condition in conditions)
            {
                recipe.AddCondition(condition);
            }
            recipe.DisableDecraft();
            recipe.Register();
        }

        if (hardmodeCrate != -1)
        {
            var recipe = Recipe.Create(result);
            recipe.AddIngredient(hardmodeCrate, crateAmount);
            if (extraItem != -1)
            {
                recipe.AddIngredient(extraItem);
            }
            recipe.AddTile(TileID.WorkBenches);
            foreach (Condition condition in conditions)
            {
                recipe.AddCondition(condition);
            }
            recipe.DisableDecraft();
            recipe.Register();
        }
    }

}