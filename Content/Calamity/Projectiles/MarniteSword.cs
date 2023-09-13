using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;

using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Calamity.Buffs;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class MarniteSword : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Marnite Sword");
            //Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            //Projectile.minion = true;
            Projectile.DamageType = DamageClass.Generic;
            //Projectile.minionSlots = 0;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(t.Width() / 2 + 1, t.Height() - 5), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //return base.Colliding(projHitbox, targetHitbox);

            return Collision.CheckAABBvLineCollision2(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(0, -50 * Projectile.scale).RotatedBy(Projectile.rotation));
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Projectile.owner != -1)
            {
                Player owner = Main.player[Projectile.owner];
                NPC target2 = target;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active)

                        if (Main.npc[i].CanBeChasedBy(ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ArcZap>()) && (Main.npc[i].Distance(Projectile.Center) < target2.Distance(Projectile.Center) && i != target.whoAmI || target2.whoAmI == target.whoAmI) && Main.npc[i].Distance(Projectile.Center) <= 500 && i != target.whoAmI)
                        {
                            target2 = Main.npc[i];

                        }
                }
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, -2), ModContent.ProjectileType<CalamityMod.Projectiles.Typeless.ArcZap>(), 3, 0, Main.myPlayer, target2.whoAmI, 1f);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            if (Projectile.owner != -1)
            {
                Player owner = Main.player[Projectile.owner];

                Projectile.ai[0] = owner.ownedProjectileCounts[Type] > 0 ? 1.1f : -1;
            }
        }
        public override void AI()
        {
            if (Projectile.owner != -1)
            {
                Player owner = Main.player[Projectile.owner];
                CheckActive(owner);
                Projectile.Center = owner.Center + new Vector2(0, -40).RotatedBy(Projectile.rotation);
                Projectile.rotation += MathHelper.ToRadians(2 * Projectile.ai[0]);
            }
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active || !owner.GetModPlayer<CrossplayerCalamity>().Marnite || !owner.GetToggleValue("MarniteSwords"))

            {
                owner.ClearBuff(ModContent.BuffType<MarniteSwordsBuff>());
                return false;
            }
            if (owner.HasBuff(ModContent.BuffType<MarniteSwordsBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            return true;
        }
    }
}
