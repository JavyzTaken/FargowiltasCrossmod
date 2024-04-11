using CalamityMod;
using CalamityMod.Cooldowns;
using FargowiltasCrossmod.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Cooldowns
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DaedalusPlatformCooldown : CooldownHandler
    {
        public static new string ID => "DaedalusPlatform";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText("Mods.FargowitlasCrossmod.Cooldowns.DaedalusPlatformCooldown");
        public override string OutlineTexture => "FargowiltasSouls/Content/Projectiles/Empty";
        public override string Texture => "Terraria/Images/Item_" + ItemID.DD2ElderCrystal;
        public override Color OutlineColor => Color.White;
        public override Color CooldownStartColor => Color.Pink;
        public override Color CooldownEndColor => Color.White;
    }
}
