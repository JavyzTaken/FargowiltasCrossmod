using System;
using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;

namespace FargowiltasCrossmod.Content.Common.Projectiles
{
    public class DLCBloomLine : BloomLine
    {
        public override string Texture => "FargowiltasSouls/Content/Projectiles/BloomLine";

        private int counter;

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
