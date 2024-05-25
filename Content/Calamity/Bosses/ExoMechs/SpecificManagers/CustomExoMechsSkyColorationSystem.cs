using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.SpecificManagers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CustomExoMechsSkyColorationSystem : ModSystem
    {
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            backgroundColor = Color.Lerp(backgroundColor, new(52, 52, 62), CustomExoMechsSky.Opacity);
            tileColor = Color.Lerp(tileColor, new(52, 52, 62), CustomExoMechsSky.Opacity * 0.85f);

            float redSirenInterpolant = CustomExoMechsSky.RedSirensIntensity * CustomExoMechsSky.Opacity * 0.7f;
            backgroundColor = Color.Lerp(backgroundColor, new(255, 79, 72), redSirenInterpolant);
            tileColor = Color.Lerp(tileColor, new(206, 97, 95), redSirenInterpolant);
        }
    }
}
