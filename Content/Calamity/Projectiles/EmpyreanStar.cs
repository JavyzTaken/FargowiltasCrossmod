using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.LunarEvents.Vortex;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EmpyreanStar : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return true;
        }
        public override string Texture => "CalamityMod/Projectiles/Typeless/EmpyreanStellarDetritus";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.usesLocalNPCImmunity = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Color color = lightColor;
            //FargoSoulsUtil.GenericProjectileDraw(Projectile, lightColor, t.Value);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, null, Color.PaleGreen with { A = 100 } * (1 - i / 8f), Projectile.oldRot[i], t.Size()/2, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size()/2, Projectile.scale, Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            
            return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft < 220;
        }
        
        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (target != null && target.active)
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 800, 20, 20);
                //Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.AngleTo(target.Center).ToRotationVector2() * 20, 0.03f);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            
        }
    }
}
