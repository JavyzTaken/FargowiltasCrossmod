using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using ReLogic.Content;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class LivingWood_Roots : ModProjectile
    {
        public int FireCD;
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            //Projectile.minion = true;
            //Projectile.damage = 0;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            FireCD = 300;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HasBuff<Buffs.LivingWood_Root_B>())
            {
                Projectile.Center = player.Center + new Vector2(0f, 20);
                return;
            }

            if (!player.GetModPlayer<CrossplayerThorium>().LivingWoodEnch)
                Projectile.Kill();

            Projectile.frame = 1;

            if (FireCD > 0)
            {
                FireCD--;
                return;
            }
            FireCD = 120;

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

            if (target != null)
            {
                bool wizard = player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().ForceEffect(player.GetModPlayer<CrossplayerThorium>().LivingWoodEnchItem.type);
                int projType = wizard ? ProjectileID.BulletHighVelocity : ProjectileID.WoodenArrowFriendly; // Goofy ah
                int damage = wizard ? 50 : 20;
                Vector2 ShootOrigin = Projectile.Center + new Vector2(0, -24);
                Vector2 ShootVec = Vector2.Normalize(target.Center - ShootOrigin) * (wizard ? 12 : 12); // Note: adjust these

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
            Rectangle rect = new(0, 0, 90, 90);
            Vector2 origin = rect.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, drawColor, 0f, origin, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
    [ExtendsFromMod("ThoriumMod")]
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
            Rectangle rect = new(0, frame * 92 + 1, 90, 89); // has pink line if dont do this idk why

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
