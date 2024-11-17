using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Items;
using CalamityMod.Items.Potions;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class WulfrumScanner : ModProjectile
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        //public override string Texture => "CalamityMod/NPCs/NormalNPCs/WulfrumDrone";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 12;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.height = Projectile.width = 30;
            Projectile.tileCollide = false;
            Projectile.minion = true;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public VertexStrip d;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = TextureAssets.Projectile[Type].Value;
            int height = t.Height / Main.projFrames[Type];
            int width = t.Width;
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition, new Rectangle(0, height * Projectile.frame, width, height), lightColor, Projectile.rotation, new Vector2(32, 34) * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            Asset<Texture2D> line = TextureAssets.Extra[178];
            
            if (Projectile.ai[1] < 0 || Main.npc[(int)Projectile.ai[1]] == null || !Main.npc[(int)Projectile.ai[1]].active )
            {
                return false;
            }
            NPC target = Main.npc[(int)Projectile.ai[1]];
            //float y = MathHelper.Lerp(-target.height/2, target.height/2, Projectile.localAI[0] < 0.5f ? 2 * Projectile.localAI[0] * Projectile.localAI[0] : 1 - (float)Math.Pow(-2 * Projectile.localAI[0] + 2, 2) / 2);
            //Main.EntitySpriteDraw(line.Value, target.Center + new Vector2(0, y) - Main.screenPosition, null, Color.Aqua, 0, new Vector2(0, line.Height() / 2), new Vector2(0.01f * target.width/5, 1), SpriteEffects.None);
            //Main.EntitySpriteDraw(line.Value, target.Center + new Vector2(0, y) - Main.screenPosition, null, Color.Aqua, MathHelper.Pi, new Vector2(0, line.Height() / 2), new Vector2(0.01f * target.width / 5, 1), SpriteEffects.None);
            //DLCUtils.DrawBackglow(line, Color.Aqua, target.Center + new Vector2(0, y), new Vector2(0, line.Height() / 2), new Vector2(0.01f * target.width / 5, 1));
            //DLCUtils.DrawBackglow(line, Color.Aqua, target.Center + new Vector2(0, y), new Vector2(0, line.Height() / 2), new Vector2(0.01f * target.width / 5, 1), MathHelper.Pi);

            Asset<Texture2D> heat = ModContent.Request<Texture2D>("CalamityMod/UI/DebuffSystem/HeatDebuffType");
            Asset<Texture2D> cold = ModContent.Request<Texture2D>("CalamityMod/UI/DebuffSystem/ColdDebuffType");
            Asset<Texture2D> sick = ModContent.Request<Texture2D>("CalamityMod/UI/DebuffSystem/SicknessDebuffType");
            Asset<Texture2D> water = ModContent.Request<Texture2D>("CalamityMod/UI/DebuffSystem/WaterDebuffType");
            Asset<Texture2D> elec = ModContent.Request<Texture2D>("CalamityMod/UI/DebuffSystem/ElectricityDebuffType");
            bool?[] weakness = { target.Calamity().VulnerableToHeat, target.Calamity().VulnerableToCold, target.Calamity().VulnerableToSickness, target.Calamity().VulnerableToWater, target.Calamity().VulnerableToElectricity };
            int amount = 0;
            for (int i = 0; i < weakness.Length; i++)
            {
                if (weakness[i] != null)
                {
                    amount++;
                }
            }
            int pos = (amount-1) * -15;
            Color weakColor = Color.Lime * 0.7f;
            Color resistColor = Color.IndianRed * 0.7f;
            if (weakness[0] != null)
            {
                Main.EntitySpriteDraw(heat.Value, Projectile.Center + new Vector2(pos, -30) - Main.screenPosition, null, (bool)weakness[0] ? weakColor : resistColor, 0, heat.Size()/2, 1, SpriteEffects.None);
                pos += 30;
            }
            if (weakness[1] != null)
            {
                Main.EntitySpriteDraw(cold.Value, Projectile.Center + new Vector2(pos, -30) - Main.screenPosition, null, (bool)weakness[1] ? weakColor : resistColor, 0, cold.Size() / 2, 1, SpriteEffects.None);
                pos += 30;
            }
            if (weakness[2] != null)
            {
                Main.EntitySpriteDraw(sick.Value, Projectile.Center + new Vector2(pos, -30) - Main.screenPosition, null, (bool)weakness[2] ? weakColor : resistColor, 0, sick.Size() / 2, 1, SpriteEffects.None);
                pos += 30;
            }
            if (weakness[3] != null)
            {
                Main.EntitySpriteDraw(water.Value, Projectile.Center + new Vector2(pos, -30) - Main.screenPosition, null, (bool)weakness[3] ? weakColor : resistColor, 0, water.Size() / 2, 1, SpriteEffects.None);
                pos += 30;
            }
            if (weakness[4] != null)
            {
                Main.EntitySpriteDraw(elec.Value, Projectile.Center + new Vector2(pos, -30) - Main.screenPosition, null, (bool)weakness[4] ? weakColor : resistColor, 0, elec.Size() / 2, 1, SpriteEffects.None);
                pos += 30;
            }

            return false;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (owner == null || owner.dead || !owner.active || !owner.HasEffect<WulfrumEffect>())
            {
                Projectile.Kill();
                return;

            }
            else
            {
                Projectile.timeLeft = 2;
            }
            
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.localAI[1] == 0)
            {
                Projectile.localAI[0] += 0.01f;
                if (Projectile.localAI[0] >= 1)
                {
                    Projectile.localAI[1] = 1;
                }
            }
            else
            {
                Projectile.localAI[0] -= 0.01f;
                if (Projectile.localAI[0] <= 0)
                {
                    Projectile.localAI[1] = 0;
                }
            }

            int speed = owner.ForceEffect<WulfrumEffect>() ? 12 : 6;
            float thing = speed == 6 ? 0.03f : 0.06f;
                Projectile.ai[0]++;
                if (Main.myPlayer == owner.whoAmI)
                {
                    Vector2 targetPos = Main.MouseWorld + new Vector2(-40 * (Main.MouseWorld.X > owner.Center.X ? 1 : -1), -30);
                    Projectile.velocity = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * (Projectile.Distance(targetPos) / 20f);
                    Projectile.spriteDirection = Main.MouseWorld.X > owner.Center.X ? 1 : -1;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                }
                if (Projectile.ai[0] >= 60)
                {
                    Projectile.ai[0] = 0;
                if (Main.myPlayer == owner.whoAmI)
                {
                    NPC prev = null;
                    if (Projectile.ai[1] >= 0) prev = Main.npc[(int)Projectile.ai[1]];
                    if (prev == null || !prev.active || Main.MouseWorld.Distance(prev.Center) > 200)
                    {
                        Projectile.ai[1] = -1;
                    }
                    bool fard = false;
                    NPC target = Main.npc[0];

                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        NPC j = Main.npc[i];
                        //Main.NewText(j );
                        if (target != null && j != null && j.active && (j.lifeMax >= target.lifeMax || target.friendly || target.lifeMax <= 5) && j.Distance(Main.MouseWorld) < 100 && j.GetGlobalNPC<CalDLCAddonGlobalNPC>().WulfrumScanned == -1 && !j.friendly && j.lifeMax > 5)
                        {
                            target = j;
                            fard = true;

                        }

                    }
                    if (target != null && target.active && fard)
                    {

                        if (Projectile.ai[1] != target.whoAmI)
                        {
                            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/WulfrumDroidHurry" + Main.rand.Next(1, 3)), Projectile.Center);
                        }
                        Projectile.ai[1] = target.whoAmI;
                        Projectile.ai[2] = 1;
                        //Main.NewText(target.FullName);
                        target.GetGlobalNPC<CalDLCAddonGlobalNPC>().WulfrumScanned = Projectile.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, target.whoAmI);

                    }
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, Projectile.whoAmI);
                }
            }
            if (Projectile.ai[1] >= 0 && Main.npc[(int)Projectile.ai[1]].active)
            {
                //Main.NewText("hi");
                Main.npc[(int)Projectile.ai[1]].GetGlobalNPC<CalDLCAddonGlobalNPC>().WulfrumScanned = Projectile.whoAmI;
            }
            base.AI();
        }
    }
}
