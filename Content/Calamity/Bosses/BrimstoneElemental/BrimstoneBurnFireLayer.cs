using CalamityMod.CalPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using Microsoft.Xna.Framework;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BrimstoneBurnFireLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.shadow == 0f;

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player drawPlayer = drawInfo.drawPlayer;
            BrimstoneBurnPlayer modPlayer = drawPlayer.GetModPlayer<BrimstoneBurnPlayer>();

            modPlayer.BrimstoneBurnEffectDrawer.DrawSet(drawPlayer.Bottom - Vector2.UnitY * 10f);
            modPlayer.BrimstoneBurnEffectDrawer.SpawnAreaCompactness = 18f;
            modPlayer.BrimstoneBurnEffectDrawer.RelativePower = 0.4f;
        }
    }
}
