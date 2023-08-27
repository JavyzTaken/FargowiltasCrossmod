using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System.IO;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class AnnihilationLaser : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_632";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }
        public override void SetDefaults()
        {

            Projectile.scale = 2.5f;
            Projectile.timeLeft = 120;
            Projectile.damage = 10;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.width = 10;
            Projectile.height = 10;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            // writer.Write7BitEncodedInt(Projectile.timeLeft);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            // Projectile.timeLeft = reader.Read7BitEncodedInt();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();

        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.localAI[1] = Main.rand.Next(1, 3);
                Projectile.netUpdate = true;
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.localAI[1] = 3;
            }
            else if (Projectile.ai[0] == 2)
            {
                Projectile.localAI[1] = 4;
            }
            else if (Projectile.ai[0] == 3)
            {
                Projectile.localAI[1] = 5;
                Projectile.timeLeft = 150;
                Projectile.netUpdate = true;
            }
            Projectile.ai[0] = 30;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.rotation = Projectile.ai[1];
                Projectile.netUpdate = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Color color = new Color(0, 244, 255);
            if (Projectile.localAI[1] == 1)
            {
                color = new Color(244, 0, 255);
            }
            if (Projectile.localAI[1] == 3 || Projectile.localAI[1] == 4 || Projectile.localAI[1] == 5)
            {
                color = new Color(150, 0, 150);
            }
            Rectangle begin = new Rectangle(0, 0, 26, 22);
            Rectangle middle = new Rectangle(0, 24, 26, 30);
            Rectangle end = new Rectangle(0, 56, 26, 22);
            Asset<Texture2D> t = TextureAssets.Projectile[Type];

            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, begin, color, Projectile.rotation, new Vector2(begin.X + begin.Width / 2, begin.Y + begin.Height / 2), new Vector2(Projectile.localAI[0], Projectile.scale), SpriteEffects.None, 0);
            for (int i = 0; i < Projectile.ai[0]; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 48 * Projectile.scale + i * 29 * Projectile.scale).RotatedBy(Projectile.rotation), middle, color, Projectile.rotation, new Vector2(middle.X + middle.Width / 2, middle.Y + middle.Height / 2), new Vector2(Projectile.localAI[0], Projectile.scale), SpriteEffects.None, 0);
                if (i == Projectile.ai[0] - 1)
                {
                    Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 106 * Projectile.scale + i * 29 * Projectile.scale).RotatedBy(Projectile.rotation), end, color, Projectile.rotation, new Vector2(end.X + end.Width / 2, end.Y + end.Height / 2), new Vector2(Projectile.localAI[0], Projectile.scale), SpriteEffects.None, 0);
                }
            }
            Projectile.netUpdate = true;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(0, 30 * Projectile.scale * Projectile.ai[0]).RotatedBy(Projectile.rotation));
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.ai[1];
            if (Projectile.localAI[0] == 0 && Projectile.timeLeft > 10)
            {
                SoundEngine.PlaySound(SoundID.Zombie104 with { SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest }, Projectile.Center);
            }
            if (Projectile.localAI[0] < Projectile.scale && Projectile.timeLeft > 10)
            {
                Projectile.localAI[0] += 0.5f;
            }
            if (Projectile.timeLeft <= 10)
            {
                Projectile.localAI[0] -= Projectile.scale / 10f;
            }
            if (Projectile.timeLeft <= 60 && Projectile.localAI[1] == 4)
            {
                Projectile.rotation = Projectile.ai[1].AngleLerp(Projectile.ai[1] + (float)Math.PI / 2, (float)Math.Sin((1 - Projectile.timeLeft / 60f) * (float)Math.PI / 2f));
                if (Projectile.timeLeft % 10 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(10, 0).RotatedBy(Projectile.rotation), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.BrimstoneBarrage>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400), 0, Main.myPlayer);
                }
            }
            if (Projectile.localAI[1] == 5)
            {
                int ownerIndex = NPC.FindFirstNPC(ModContent.NPCType<ChampionofAnnihilation>());
                if (ownerIndex != -1)
                {
                    NPC owner = Main.npc[ownerIndex];
                    Projectile.Center = owner.Center;
                }
            }

        }

    }
}
