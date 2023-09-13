using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using CalamityMod;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class FearValkyrie : ModProjectile
    {
        private int feathimerScary = 120;
        public override string Texture => "CalamityMod/Projectiles/Summon/PowerfulRaven";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Damned Valkyrie");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 24;
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
            if (!SBDPlayer.FearValkyrie)
            {
                Projectile.active = false;
                return;
            }
            if (Projectile.type == ModContent.ProjectileType<FearValkyrie>())
            {
                if (player.dead)
                {
                    SBDPlayer.aScarey = false;
                }
                if (SBDPlayer.aScarey)
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
            Vector2 FValkyrieCenter = Projectile.Center;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, FValkyrieCenter);

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
                        float between = Vector2.Distance(npc.Center, FValkyrieCenter);
                        bool closest = Vector2.Distance(FValkyrieCenter, targetCenter) > between;
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
                feathimerScary--;
                if (distanceFromTarget > 120f)
                {
                    speed = 30f;
                    inertia = 60f;
                    Vector2 direction = targetCenter - FValkyrieCenter;
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
                    speed = 3f;
                    inertia = 6f;
                    Vector2 direction = targetCenter - FValkyrieCenter;
                    direction.Normalize();
                    direction *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                if (distanceToIdlePosition > 600f)
                {
                    speed = 40f;
                    inertia = 80f;
                }
                else
                {
                    speed = 15f;
                    inertia = 30f;
                }

                if (distanceToIdlePosition > 150f)
                {
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
            }
            if (feathimerScary <= 0)
            {
                feathimerScary = 120;
                Projectile.netUpdate = true;
                Vector2 velocity = Vector2.One;
                for (int i = 0; i < 4; i++)
                {
                    int damage = (int)player.GetBestClassDamage().ApplyTo(180f);
                    int fearFethahs = Projectile.NewProjectile(player.GetSource_FromThis(), FValkyrieCenter, velocity, ModContent.ProjectileType<FearsomeFeather>(), damage, 1f, player.whoAmI);
                    if (Main.projectile.IndexInRange(fearFethahs))
                    {
                        Main.projectile[fearFethahs].originalDamage = Projectile.originalDamage;
                        Main.projectile[fearFethahs].velocity = Projectile.DirectionTo(targetCenter) * 8f;
                    }
                }
            }

            //visuals
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = Projectile.direction;
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
