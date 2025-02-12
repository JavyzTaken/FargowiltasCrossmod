using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.NPCs
{
    public  class MutantBR : ModNPC
    {
        public override string Texture => "Fargowiltas/NPCs/Mutant";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 2;
            NPCID.Sets.TrailCacheLength[Type] = 10;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        public override void AI()
        {
            base.AI();
        }
    }
}
