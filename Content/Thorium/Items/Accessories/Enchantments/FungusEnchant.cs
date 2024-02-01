using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using static Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FungusEnchant : BaseEnchant
    {
        public override Color nameColor => Color.LightBlue;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FungusEffect>(Item);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ThoriumMod.Items.ThrownItems.FungusHat>()
                .AddIngredient<ThoriumMod.Items.ThrownItems.FungusGuard>()
                .AddIngredient<ThoriumMod.Items.ThrownItems.FungusLeggings>()
                .Register();
        }

        [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
        public class FungusEffect : AccessoryEffect
        {
            public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.MuspelheimHeader>();
            public override int ToggleItemType => ModContent.ItemType<FungusEnchant>();
            public override bool ExtraAttackEffect => true;

            public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
            {
                if (target.TryGetGlobalNPC(out FungusEnemy funguy) && !funguy.Infected && !target.boss && Main.rand.NextBool(10))
                {
                    funguy.infectedBy = player.whoAmI;
                    funguy.Infected = true;
                }
            }
        }
    }

    public class FungusEnemy : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            BestiaryEntry entry = Main.BestiaryDB.FindEntryByNPCID(entity.netID);
            return !(entry.Info.Contains(SurfaceMushroom) || entry.Info.Contains(UndergroundMushroom));
        }
        public bool Infected;
        public int infectedBy;

        public override void OnKill(NPC npc)
        {
            if (Infected)
            {
                for (int i = 0; i < Main.rand.Next(2, 6); i++)
                {
                    Projectile spore = Projectile.NewProjectileDirect(npc.GetSource_Death(), npc.Center + Vector2.UnitY * npc.width / 2, Main.rand.NextVector2Circular(1, 1) * 8f, ModContent.ProjectileType<FungusSpore>(), 25, 1f, infectedBy);
                    spore.velocity.Y = -1.5f * MathF.Abs(spore.velocity.Y);
                }
            }
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            return Infected ? Color.SkyBlue : base.GetAlpha(npc, drawColor);
        }
    }
}
