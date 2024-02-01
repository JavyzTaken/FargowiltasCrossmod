using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using ReLogic.Content;
using Terraria.GameContent;
using FargowiltasSouls;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LivingWood_Roots : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            //Projectile.minion = true;
            //Projectile.damage = 0;
            Projectile.timeLeft = 36000;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HasBuff<Buffs.LivingWood_Root_B>())
            {
                Projectile.Center = player.Center + new Vector2(0f, 20);
                return;
            }

            if (!player.HasEffect<LivingWoodEffect>())
                Projectile.Kill();

            Projectile.frame = 1;


            #region Behavior
            float targetDistance = 700f;
            NPC target = null;

            // player - targeted npc
            if (player.HasMinionAttackTargetNPC)
            {
                target = Main.npc[player.MinionAttackTargetNPC];
                targetDistance = Vector2.Distance(target.Center, Projectile.Center);

                if (targetDistance > 2000f)
                {
                    // so it doesnt go for targeted npc ages away
                    target = null;
                }
            }

            if (target == null)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float distance2 = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = targetDistance > distance2;
                        bool inRange = distance2 < 2000f;
                        bool lineOfSight = Collision.CanHitLine(Projectile.Center + new Vector2(0, -24), 5, 5, npc.position, npc.width, npc.height);

                        if (closest && inRange && lineOfSight)
                        {
                            target = npc;
                            targetDistance = distance2;
                        }
                    }
                }
            }

            Vector2 ShootOrigin = Projectile.Center + new Vector2(0, -24);
            if (target != null)
            {
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, (target.Center - ShootOrigin).ToRotation(), 0.02f);
            }
            else
            {
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, MathF.Abs(Projectile.rotation % MathF.Tau) < MathHelper.PiOver2 ? 0f : MathF.PI, 0.02f);
            }

            if (target != null && ++Projectile.ai[0] >= 90)
            {
                Projectile.ai[0] = 0;
                bool wizard = player.ForceEffect<LivingWoodEffect>();
                int projType = wizard ? ProjectileID.BulletHighVelocity : ProjectileID.WoodenArrowFriendly; 
                int damage = wizard ? 80 : 20;
                Vector2 ShootVec = Vector2.Normalize(target.Center - ShootOrigin) * (wizard ? 16 : 12); 

                // I couldn't get the math for aiming the arrow correct so it has a tendency to miss in non-force mode.
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), ShootOrigin, ShootVec, projType, damage, 5f, Projectile.owner);

                if (ShootVec.X > 0)
                {
                    Projectile.direction = 1;
                }
                else Projectile.direction = -1;
            }
            #endregion
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.frame == 0) return false;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rect = new(0, 0, 90, 88);
            Vector2 origin = rect.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, drawColor, 0f, origin, 1f, SpriteEffects.None, 0);

            bool force = Main.player[Projectile.owner].FargoSouls().ForceEffect(ModContent.ItemType<Items.Accessories.Enchantments.LivingWoodEnchant>());
            Texture2D gunTexture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.SniperRifle}").Value;
            Texture2D bowTexture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.WoodenBow}").Value;
            Rectangle source = force ? gunTexture.Bounds : bowTexture.Bounds;
            SpriteEffects effect = MathF.Abs(Projectile.rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(force ? gunTexture : bowTexture, Projectile.Center - Main.screenPosition + new Vector2(0, -24), source, drawColor, Projectile.rotation, source.Size() / 2, 1f, effect);
            return false;
        }
    }
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class LivingWoodDrawLayer : PlayerDrawLayer
    {
        private Asset<Texture2D> GrowingRoots;
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.HasBuff<Buffs.LivingWood_Root_B>();
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (!GetDefaultVisibility(drawInfo)) return;

            if (GrowingRoots == null)
            {
                GrowingRoots = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Thorium/Projectiles/LivingWood_GrowRoots");
            }

            int frame;
            if (drawInfo.drawPlayer.HasBuff<Buffs.LivingWood_Root_B>())
            {
                int bufftime = drawInfo.drawPlayer.buffTime[drawInfo.drawPlayer.FindBuffIndex(ModContent.BuffType<Buffs.LivingWood_Root_B>())];
                frame = (int)MathF.Floor((300 - bufftime) / 75f);
            }
            else frame = 4;

            Vector2 position = drawInfo.drawPlayer.Center - Main.screenPosition;
            position = new Vector2((int)position.X, (int)position.Y + 20);
            Rectangle rect = new(0, frame * 92 + 1, 90, 89); 

            drawInfo.DrawDataCache.Add(new DrawData(
                GrowingRoots.Value,
                position,
                rect,
                Lighting.GetColor((int)drawInfo.drawPlayer.Center.X / 16, (int)drawInfo.drawPlayer.Center.Y / 16),
                0f,
                rect.Size() * 0.5f,
                1f,
                SpriteEffects.None,
                0
            ));
        }
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);
    }
}
