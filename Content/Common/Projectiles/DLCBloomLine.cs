using FargowiltasSouls.Content.Bosses.BanishedBaron;
using FargowiltasSouls.Content.Bosses.Lifelight;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Content.Common.Bosses.Mutant;

namespace FargowiltasCrossmod.Content.Common.Projectiles
{
    public class DLCBloomLine : BloomLine
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BloomLine";

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(counter);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            counter = reader.ReadInt32();
            base.ReceiveExtraAI(reader);
        }
        private int counter;
        public override void AI()
        {
            int maxTime = 60;
            float alphaModifier = 3;



            switch ((int)Projectile.ai[0])
            {
                case 1: //mutant scal line
                    {
                        if (!ModCompatibility.Calamity.Loaded)
                        {
                            Projectile.Kill();
                            return;
                        }
                        Projectile.position -= Projectile.velocity;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        color = Color.Red;
                        alphaModifier = 1;
                        Projectile.scale = 1f;
                        maxTime = (int)Projectile.ai[2];
                    }
                    break;
                default:
                    Main.NewText("bloom line: you shouldnt be seeing this text, show javyz");
                    break;
            }

            if (++counter > maxTime)
            {
                Projectile.Kill();
                return;
            }

            if (alphaModifier >= 0)
            {
                Projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * counter) * alphaModifier);
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }

            color.A = 0;
        }
    }
}
