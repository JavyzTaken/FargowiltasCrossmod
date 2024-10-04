using System.Collections.Generic;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class IceChain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.scale = 1;
            Projectile.tileCollide = false;
            

            Projectile.light = 0.5f;
            Projectile.penetrate = -1;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 0)
            {
                Projectile.Kill();
                return false;
            }
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>())
            {

                Projectile.Kill();
                return false;

            }

            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.instance.LoadProjectile(ProjectileID.CultistBossIceMist);
            Asset<Texture2D> ice = TextureAssets.Projectile[ProjectileID.CultistBossIceMist];

            ref float timer = ref Projectile.ai[2];
            if (true)
            {
                for (int i = 0; i < Projectile.Distance(owner.Center); i += (int)(48 * Projectile.scale))
                {
                    Vector2 pos = Projectile.Center + new Vector2(-i, 0).RotatedBy(Projectile.AngleFrom(owner.Center));
                    for (int j = 0; j < 12; j++)
                    {
                        Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                        Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                        Main.EntitySpriteDraw(t.Value, pos + afterimageOffset - Main.screenPosition, null, glowColor * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
                    }
                    Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()) * Projectile.Opacity, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);

                }
            }
            if (timer > 300)
            {
                for (int i = 0; i < Projectile.Distance(owner.Center); i += (int)(17 * Projectile.scale))
                {
                    Vector2 pos = Projectile.Center + new Vector2(-i, 0).RotatedBy(Projectile.AngleFrom(owner.Center));
                    for (int j = 0; j < 12; j++)
                    {
                        Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                        Color glowColor = Color.Blue with { A = 0 } * 0.7f;


                        Main.EntitySpriteDraw(ice.Value, pos + afterimageOffset - Main.screenPosition, null, glowColor * 0.5f, Projectile.rotation + i, ice.Size() / 2, Projectile.scale * 0.6f, SpriteEffects.None);
                    }
                    Main.EntitySpriteDraw(ice.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()) * 0.5f, Projectile.rotation + i, ice.Size() / 2, (Projectile.scale * 0.6f), SpriteEffects.None);

                }
            }
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[0] < 0)
            {
                Projectile.Kill();
                return false;
            }
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>())
            {

                Projectile.Kill();
                return false;

            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, owner.Center) && Projectile.ai[2] > 300;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public const int ActiveTime = 600;
        public override void AI()
        {
            Projectile.hide = true;
            if (Projectile.ai[0] < 0)
            {

                Projectile.Kill();
                return;
            }
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            ref float timer = ref Projectile.ai[2];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Cryogen.Cryogen>())
            {

                Projectile.Kill();
                return;

            }
            else
            {
                Projectile.timeLeft = 2;
            }
            Projectile.rotation = Projectile.AngleFrom(owner.Center);

            if (timer < 50)
            {
                timer++;
                Projectile.velocity = new Vector2(0, CryogenEternity.ArenaRadius / 50f).RotatedBy(Projectile.ai[1]);
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
            }
            if (timer >= 200)
            {
                Projectile.Opacity = 1f;
                timer++;
            }
            else
            {
                Projectile.Opacity = 0.2f;
            }
            if (timer >= 200 && timer <= 300)
            {

                for (int i = 0; i < 5; i++)
                    Dust.NewDustDirect(owner.Center - Projectile.Size / 2 + new Vector2(0, -((timer - 200) * Projectile.Distance(owner.Center) / 100)).RotatedBy(Projectile.rotation + MathHelper.PiOver2), Projectile.width, Projectile.height, DustID.SnowflakeIce, Scale: 2).noGravity = true;

            }
            if (timer == 300)
            {
                for (int i = 0; i < Projectile.Distance(owner.Center); i += 3)
                {
                    Dust.NewDustDirect(owner.Center - Projectile.Size / 2 + new Vector2(0, -i).RotatedBy(Projectile.rotation + MathHelper.PiOver2), Projectile.width, Projectile.height, DustID.SnowflakeIce, Scale: 2).noGravity = true;
                }
                //SoundEngine.PlaySound(SoundID.Item28, owner.Center);
            }
            if (timer >= 100 + ActiveTime)
            {
                timer = 100;
                for (int i = 0; i < Projectile.Distance(owner.Center); i += 3)
                {
                    Dust.NewDustDirect(owner.Center - Projectile.Size + new Vector2(0, -i).RotatedBy(Projectile.rotation + MathHelper.PiOver2), Projectile.width * 2, Projectile.height * 2, DustID.SnowflakeIce, Scale: 2).noGravity = true;
                }
                SoundEngine.PlaySound(CalamityMod.NPCs.Cryogen.CryogenShield.BreakSound, owner.Center);
            }

            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, Scale: 2).noGravity = true;
            base.AI();
        }
    }
}
