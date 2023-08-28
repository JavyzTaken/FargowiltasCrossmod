using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using static FargowiltasCrossmod.Content.Thorium.Projectiles.ValaChunkCollisions;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class Valadium_Chunk : ModProjectile
    {
        // Size is ai[0], Mass is ai[1]
        //public List<int> ActiveChunks = new();
        public const float ChunkAttractConst = 4f;
        public const float PlayerMass = 1200f;
        public const float PlayerAttractConst = 12f;
        public const float MaxSpeed = 24; // do NOT make this any lower

        private int hitCD;

        public float Mass
        {
            get
            {
                return Projectile.ai[0] switch
                {
                    1 => 100f,
                    2 => 400f,
                    3 => 800f,
                    4 => 1600f,
                    _ => 000f,
                };
            }
        }
        public int Radius
        {
            get
            {
                return Projectile.ai[0] switch
                {
                    1 => 12,
                    2 => 18,
                    3 => 24,
                    4 => 30,
                    _ => 0,
                };
            }
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 0f) return;
            var DLCPlayer = Main.player[Projectile.owner].GetModPlayer<CrossplayerThorium>();
            DLCPlayer.ActiveValaChunks.Add(Projectile.whoAmI);
            Projectile.ai[1] = Mass;
            Projectile.width = Radius * 2;
            Projectile.height = Radius * 2;
            hitCD = 15;
            //Projectile.frame = (int)Projectile.ai[0] - 1;
        }

        public override void Kill(int timeLeft)
        {
            //Main.NewText($"Chunk killed: {timeLeft}");
            var DLCPlayer = Main.player[Projectile.owner].GetModPlayer<CrossplayerThorium>();
            DLCPlayer.ActiveValaChunks.Remove(Projectile.whoAmI);
            if (timeLeft >= 0) Split();
        }

        private void Split()
        {
            if (Projectile.ai[0] > 1f)
            {
                // Spawns two chunks of 1 smaller size and velocities at 45 degree angles to the original's. Velocities of new chunks are two thirds of the original
                float Sqrt2on3 = 1.41421f / 3;
                Vector2 veloA = new Vector2(Projectile.velocity.X - Projectile.velocity.Y, Projectile.velocity.X + Projectile.velocity.Y) * Sqrt2on3;
                Vector2 veloB = new Vector2(Projectile.velocity.X + Projectile.velocity.Y, Projectile.velocity.Y - Projectile.velocity.X) * Sqrt2on3;

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, veloA, Projectile.type, Projectile.damage * 2 / 3, Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 1);
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, veloB, Projectile.type, Projectile.damage * 2 / 3, Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 1);
            }
        }


        public override bool? CanHitNPC(NPC target) => hitCD <= 0;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage.Flat = (int)(Projectile.velocity.LengthSquared() * Mass / 300);
        }

        public override bool PreAI()
        {
            if (Projectile.position.X < 128 || Projectile.position.X > Main.tile.Width * 16 - 128 || Projectile.position.Y < 128 || Projectile.position.Y > Main.tile.Height * 16 - 128)
            {
                // Chunks existing out of bounds was causing rendering issues
                CombatText.NewText(Projectile.Hitbox, Color.Red, "Scrongbongled", true);
                Kill(-69420);
                return false;
            }
            return true;
        }

        public override void AI()
        {
            if (Projectile.ai[0] <= 0)
            {
                Projectile.timeLeft = 0;
                return;
            }

            if (hitCD > 0) hitCD--;

            Player player = Main.player[Projectile.owner];
            if (!player.dead && player.active && player.GetModPlayer<CrossplayerThorium>().ValadiumEnch)
            {
                Projectile.timeLeft = Main.rand.Next(3, 10);
            }

            // player attraction
            float playerDistSQ = (player.Center - Projectile.Center).LengthSquared();
            if (playerDistSQ > 6400f)
            {
                Vector2 force = PlayerMass * Projectile.ai[1] / (playerDistSQ - MathF.Sqrt(playerDistSQ)) * Vector2.Normalize(player.Center - Projectile.Center) * PlayerAttractConst;
                Projectile.velocity += force / Projectile.ai[1];
            }

            // if its too far (offscreen) from the player then ignore other chunks
            if (playerDistSQ > 980100f)
            {
                Projectile.velocity += Projectile.Center.DirectionTo(player.Center) / 4f;
                return;
            }

            var DLCPlayer = Main.player[Projectile.owner].GetModPlayer<CrossplayerThorium>();
            if (DLCPlayer.ActiveValaChunks.Count <= 1) return;

            // chunk attraction
            foreach (int index in DLCPlayer.ActiveValaChunks)
            {
                if (index <= Projectile.whoAmI) continue;

                Projectile proj = Main.projectile[index];

                float distSQ = (Projectile.Center - proj.Center).LengthSquared();

                if (distSQ < 256)
                {
                    if (Main.player[Projectile.owner].GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().WizardEnchantActive)
                    {
                        Collide(Projectile, proj);
                    }
                    continue;
                }

                Vector2 force = proj.ai[1] * Projectile.ai[1] / distSQ * Vector2.Normalize(Projectile.Center - proj.Center) * ChunkAttractConst;

                proj.velocity += force / proj.ai[1];
                Projectile.velocity += -force / Projectile.ai[1];
            }
        }

        public override void PostAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.LengthSquared() > MaxSpeed * MaxSpeed)
            {
                Projectile.velocity.Normalize();
                Projectile.velocity *= MaxSpeed;
            }
            //Player player = Main.player[Projectile.owner];
            //float playerDistSQ = (player.Center - Projectile.Center).LengthSquared();
            //if (playerDistSQ > 980100f)
            //{
            //    Projectile.velocity += Projectile.Center.DirectionTo(player.Center) / 4f;
            //}
            Projectile.position += Projectile.velocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 0) return false;
            if (Projectile.position.X < 128 || Projectile.position.X > Main.tile.Width * 16 - 128 || Projectile.position.Y < 128 || Projectile.position.Y > Main.tile.Height * 16 - 128)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle rect = new(0, (int)(Projectile.ai[0] - 1) * 64, 64, 64);
            Vector2 origin = rect.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rect, drawColor, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);

            return false;
        }
    }
}