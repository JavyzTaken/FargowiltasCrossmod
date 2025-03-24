using CalamityMod.NPCs.Crabulon;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
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
    public class MushroomSpear2 : ModProjectile
    {
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/MushroomSpear";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {

            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.height = 22;
            Projectile.width = 22;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1;
            Projectile.scale = 2.5f;
            Projectile.hide = true;

            Projectile.FargoSouls().DeletionImmuneRank = 2;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> head = TextureAssets.Projectile[Type];
            Asset<Texture2D> stick = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/MushroomSpearStick");
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Crabulon.Crabulon>())
            {
                Projectile.Kill();
                return false;
            }
            Vector2 originPos = owner.Center;
            for (int i = 0; i < Projectile.Center.Distance(originPos) / 24 + 1; i++)
            {
                Main.EntitySpriteDraw(stick.Value, Projectile.Center - Main.screenPosition + new Vector2(i*24, 0).RotatedBy(Projectile.AngleTo(originPos)), null, lightColor * Projectile.Opacity, Projectile.rotation, stick.Size() / 2, Projectile.scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(head.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver4, head.Size() / 2, Projectile.scale, SpriteEffects.None);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Crabulon.Crabulon>())
            {
                Projectile.Kill();
                return false;
            }
            if (Projectile.ai[2] < 60)
            {
                return false;
            }
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, owner.Center);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomSpray, Alpha: 120, Scale: 2).noGravity = true;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {

            for (int i = 0; i < 10; i++)
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomSpray, Alpha: 120, Scale: 2, SpeedY: Main.rand.Next(-10, 0));
                d.noGravity = true;
                d.velocity = d.velocity.RotatedBy(Projectile.ai[1]);
            }
            //Projectile.Center = new Vector2(Projectile.Center.X, FindGround(Projectile.Center));
            //Projectile.ai[2] = Projectile.Center.Y;
        }
        //ai0: owner whoami
        //ai1: rotation offset from owner
        //ai2: timer
        public override void AI()
        {
            int vel = WorldSavingSystem.MasochistModeReal ? 40 : 25;
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            //Main.NewText("h");
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.Crabulon.Crabulon>())
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 10;
            }
            Projectile.ai[2]++;
            Vector2 telegraphPos = owner.Center + new Vector2(0, 120).RotatedBy(Projectile.ai[1] + owner.rotation);
            Vector2 fullPos = owner.Center + new Vector2(0, 320).RotatedBy(Projectile.ai[1] + owner.rotation);
            Projectile.rotation = Projectile.ai[1] + owner.rotation + MathF.PI;
            if (Projectile.ai[2] < 20)
            {
                Projectile.Center =  Vector2.Lerp(owner.Center, telegraphPos, Projectile.ai[2] / 20f);
            }else if (Projectile.ai[2] < 50)
            {
                Projectile.Center = telegraphPos;
            }else if (Projectile.ai[2] < 80)
            {
                
                Projectile.Center =  Vector2.Lerp(telegraphPos, fullPos, (Projectile.ai[2] - 50) / 30f);
            }
            else
            {
                Projectile.Center = fullPos;
            }
            if (Projectile.ai[2] == 80) {
                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Pitch = 0.5f }, Projectile.Center);
            }
            if (MathF.Abs(Projectile.Center.Y - owner.Center.Y) >= 100 && !Collision.CanHitLine(Projectile.Center, 1, 1, owner.Center, 1, 1) && owner.velocity.Y > -10)
            {
                //Main.NewText("j");
                //owner.velocity.Y -= 0.5f;
            }
        }
    }
}
