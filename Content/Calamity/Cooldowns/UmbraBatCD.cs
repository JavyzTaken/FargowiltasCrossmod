using CalamityMod.Cooldowns;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Cooldowns
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class UmbraBatCD : CooldownHandler
    {
        public new static string ID => "UmbraBatCD";
        public override bool ShouldDisplay => true;
        public override SoundStyle? EndSound => SoundID.NPCHit54;
        public override bool ShouldPlayEndSound => true;
        public override LocalizedText DisplayName => Language.GetText("Mods.FargowiltasCrossmod.Cooldowns.UmbraBatCD");
        public override string OutlineTexture => "FargowiltasCrossmod/Content/Calamity/Cooldowns/UmbraBatCDOutline";
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Cooldowns/UmbraBatCD";
        public override Color OutlineColor => new Color(245, 66, 66);
        public override Color CooldownStartColor => new Color(0, 0, 0);
        public override Color CooldownEndColor => new Color(212, 28, 28);

    }
}
