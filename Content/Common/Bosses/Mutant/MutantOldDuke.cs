using FargowiltasSouls.Content.Bosses.MutantBoss;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    public class MutantOldDuke : MutantFishron
    {
        public override string Texture => "CalamityMod/NPCs/OldDuke/OldDuke";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 7;
        }
        public override bool PreAI()
        {
            return true;
        }
    }
}
