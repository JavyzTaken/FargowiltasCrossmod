using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CrabulonJumpTelegraph : ModProjectile
    {
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {

            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.height = 10;
            Projectile.width = 10;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1;
            Projectile.scale = 1f;
            Projectile.hide = true;
            Projectile.Opacity = 0;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            int npcID = (int)Projectile.ai[0];
            if (!npcID.IsWithinBounds(Main.maxNPCs))
            {
                Projectile.Kill();
                return;
            }
            NPC npc = Main.npc[npcID];
            if (!npc.Alive())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = npc.Center;
            if (Projectile.Opacity < 1)
                Projectile.Opacity += 0.025f;
            else
                Projectile.Opacity = 1;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> line = TextureAssets.Extra[178];

            float opacity = Projectile.Opacity;
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition, null, Color.DarkCyan * opacity, Projectile.velocity.ToRotation(), new Vector2(0, line.Height() * 0.5f), new Vector2(0.4f, Projectile.scale * 30), SpriteEffects.None);
            return false;
        }
    }
}
