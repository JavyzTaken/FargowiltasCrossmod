using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class Tentacle : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/OmegaBlueTentacle";
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.hide = true;
            base.SetDefaults();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc == null || npc.type != ModContent.NPCType<BabyColossalSquid>() || !npc.active)
            {

                return true;
            }

            Asset<Texture2D> ball = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/OmegaBlueTentacleSegment");
            for (int i = 0; i < 5; i++)
            {
                Vector2 between = npc.Center - Projectile.Center;
                Vector2 pos = between / 5 * i + Projectile.Center;
                Main.EntitySpriteDraw(ball.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), 0, ball.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            }
            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc == null || npc.type != ModContent.NPCType<BabyColossalSquid>() || !npc.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.rotation = Projectile.Center.AngleFrom(npc.Center);
            Vector2 pos = npc.Center + new Vector2(0, 100).RotatedBy(Projectile.ai[1]);
            if (Projectile.Distance(pos) > 10)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
            }
            Player target = Main.player[npc.target];
            if (Projectile.localAI[0] == 0 && Projectile.Distance(target.Center) < 200 && Projectile.Distance(pos) < 20)
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20;
                Projectile.localAI[0] = 120;
            }
            if (Projectile.localAI[0] > 0)
            {
                Projectile.localAI[0]--;
            }
        }
    }
}
