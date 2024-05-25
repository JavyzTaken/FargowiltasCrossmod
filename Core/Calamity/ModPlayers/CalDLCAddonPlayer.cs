using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.ModPlayers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonPlayer : ModPlayer
    {
        public float AeroCritBoost;
        public int FeatherJumpsRemaining;
        public int usedWeaponTimer;
        public float ProwlerCharge;
        public bool AutoProwler = false;
        public int PlagueCharge;
        public int DaedalusHeight;
        public int ReaverBuff;
        public bool ReaverHide;
        public int ThermalCharge;
        public bool Overheating;
        public bool HydrothermicHide;
        public int NumJumpsUsed = 0;
        public bool AllowJumpsUsedInc = false;
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override void PostUpdateEquips()
        {
            if (Player.HasEffect<DesertProwlerEffect>())
            {
                AutoProwler = Player.autoJump;
                Player.autoJump = false;

            }
        }
        public override void PreUpdate()
        {
           if (Player.HasEffect<DesertProwlerEffect>())
            {
                DesertProwlerEffect.ProwlerEffect(Player);
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //titan heart
            //Vector2 vector = drawInfo.Position + drawInfo.drawPlayer.Size * new Vector2(0.5f, 1f) - Main.screenPosition;
            //for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            //{
            //    DrawData value = drawInfo.DrawDataCache[i];
            //    Vector2 vector2 = value.position - vector;
            //    value.position = vector + vector2 * 2;
            //    value.scale *= 2;
            //    drawInfo.DrawDataCache[i] = value;
            //}
            // drawInfo.Position += new Vector2(20, 20);
        }
    }
}
