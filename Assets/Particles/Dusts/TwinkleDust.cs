using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Assets.Particles.Dusts
{
    public class TwinkleDust : ModDust
    {
        public override void OnSpawn(Dust dust) { }

        public override bool Update(Dust dust)
        {
            if (dust.customData is not int)
                dust.customData = 0;

            int time = (int)dust.customData;

            dust.position += dust.velocity;
            dust.velocity *= 0.97f;
            dust.scale *= MathHelper.Lerp(0.92f, 1.05f, LumUtils.Cos01(time / 7f + dust.dustIndex * 17f));
            dust.customData = (int)dust.customData + 1;

            if (time >= 90 && dust.scale <= 0.09f)
                dust.active = false;

            return false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(255, 255, 255, dust.alpha).MultiplyRGBA(dust.color) * LumUtils.InverseLerp(0.2f, 0.5f, dust.scale);
    }
}
