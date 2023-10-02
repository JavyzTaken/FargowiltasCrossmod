using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    //so i can slightly change how projectiles work without making a new one
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationGlobalProjectile : GlobalProjectile
    {
        public override void SetDefaults(Projectile projectile)
        {
            base.SetDefaults(projectile);
        }
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);

        }
        //dont run normal kill method for copper coin projectiles so it doesnt spawn items
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            if (projectile.type == ModContent.ProjectileType<CalamityMod.Projectiles.Ranged.RicoshotCoin>() && projectile.ai[1] == -1)
            {
                return false;
            }
            return base.PreKill(projectile, timeLeft);
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (projectile.type == ProjectileID.GladiusStab && projectile.ai[0] == 50)
            {
                Texture2D texture = TextureAssets.Projectile[ProjectileID.GladiusStab].Value;
                Main.EntitySpriteDraw(texture, projectile.Top - Main.screenPosition, null, lightColor, projectile.rotation, new Vector2(11, 11), projectile.scale, SpriteEffects.FlipVertically, 0);
                return false;
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public override void AI(Projectile projectile)
        {

            base.AI(projectile);

        }
        public override bool ShouldUpdatePosition(Projectile projectile)
        {

            return base.ShouldUpdatePosition(projectile);
        }

        public override bool PreAI(Projectile projectile)
        {

            if (projectile.type == ModContent.ProjectileType<CalamityMod.Projectiles.Ranged.RicoshotCoin>() && NPC.AnyNPCs(ModContent.NPCType<ChampionofExploration>()))
            {
                projectile.timeLeft = 1000;
            }

            return base.PreAI(projectile);
        }

        public override void PostAI(Projectile projectile)
        {

        }

    }
}
