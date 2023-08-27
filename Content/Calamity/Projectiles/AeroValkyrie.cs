using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod("CalamityMod")]
    public class AeroValkyrie : ModProjectile
    {
        private int feathimer = 90;
        public override string Texture => "CalamityMod/Projectiles/Summon/Valkyrie";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Enchanted Valkyrie");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.timeLeft *= 5;
            Projectile.DamageType = DamageClass.Summon;
        }
        public override bool? CanCutTiles()
        {
            return false;
        }
        public override bool MinionContactDamage()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CrossplayerCalamity SBDPlayer = player.GetModPlayer<CrossplayerCalamity>();

            //active check
            if (!SBDPlayer.AeroValkyrie)
            {
                Projectile.active = false;
                return;
            }
            if (Projectile.type == ModContent.ProjectileType<AeroValkyrie>())
            {
                if (player.dead)
                {
                    SBDPlayer.aValkie = false;
                }
                if (SBDPlayer.aValkie)
                {
                    Projectile.timeLeft = 2;
                }
            }

            //the ai itself
            //general stuff
            Vector2 idlePosition = player.Center;
            idlePosition.Y -= 48f;
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -player.direction;
            idlePosition.X += minionPositionOffsetX;
            Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
            float distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f)
            {
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            //target search
            float distanceFromTarget = 700f;
            Vector2 targetCenter = Projectile.position;
            bool foundTarget = false;
            Vector2 ValkyrieCenter = Projectile.Center;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, ValkyrieCenter);

                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, ValkyrieCenter);
                        bool closest = Vector2.Distance(ValkyrieCenter, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            //movement
            float speed;
            float inertia;
            if (foundTarget)
            {
                feathimer--;
                if (distanceFromTarget > 120f)
                {
                    speed = 10f;
                    inertia = 20f;
                    Vector2 direction = targetCenter - ValkyrieCenter;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
                if (distanceFromTarget < 100f)
                {
                    Projectile.velocity = Vector2.Zero;
                }
                if (distanceFromTarget < 120f && distanceFromTarget > 100f)
                {
                    speed = 1f;
                    inertia = 2f;
                    Vector2 direction = targetCenter - ValkyrieCenter;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                if (distanceToIdlePosition > 600f)
                {
                    speed = 20f;
                    inertia = 40f;
                }
                else
                {
                    speed = 5f;
                    inertia = 10f;
                }

                if (distanceToIdlePosition > 150f)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
            }
            if (feathimer <= 0)
            {
                feathimer = 90;
                Projectile.netUpdate = true;
                Vector2 velocity = Vector2.One;
                for (int j = 0; j < 3; j++)
                {
                    int damage = (int)player.GetBestClassDamage().ApplyTo(25f);
                    int aeroFethahs = Projectile.NewProjectile(player.GetSource_FromThis(), ValkyrieCenter, velocity, ModContent.ProjectileType<StickyFeatherAero>(), damage, 1f, player.whoAmI);
                    if (Main.projectile.IndexInRange(aeroFethahs))
                    {
                        Main.projectile[aeroFethahs].originalDamage = Projectile.originalDamage;
                        Main.projectile[aeroFethahs].velocity = Projectile.DirectionTo(targetCenter) * 8f;
                    }
                }
            }

            //visuals
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
        }
    }
}