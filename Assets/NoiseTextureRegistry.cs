using Luminance.Assets;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Assets
{
    public class NoiseTexturesRegistry : ModSystem
    {
        #region Texture Path Constants

        public const string ExtraTexturesPath = "FargowiltasCrossmod/Assets/Textures";

        #endregion Texture Path Constants

        #region Noise Textures

        public static readonly LazyAsset<Texture2D> BinaryPoem = LoadDeferred($"{ExtraTexturesPath}/BinaryPoem");

        public static readonly LazyAsset<Texture2D> BubblyNoise = LoadDeferred($"{ExtraTexturesPath}/BubblyNoise");

        public static readonly LazyAsset<Texture2D> CloudDensityMap = LoadDeferred($"{ExtraTexturesPath}/CloudDensityMap");

        public static readonly LazyAsset<Texture2D> CrackedNoiseA = LoadDeferred($"{ExtraTexturesPath}/CrackedNoiseA");

        public static readonly LazyAsset<Texture2D> ElectricNoise = LoadDeferred($"{ExtraTexturesPath}/ElectricNoise");

        public static readonly LazyAsset<Texture2D> FireParticleA = LoadDeferred($"{ExtraTexturesPath}/FireParticleA");

        public static readonly LazyAsset<Texture2D> FireParticleB = LoadDeferred($"{ExtraTexturesPath}/FireParticleA");

        public static readonly LazyAsset<Texture2D> PerlinNoise = LoadDeferred($"{ExtraTexturesPath}/PerlinNoise");

        #endregion Noise Textures

        #region Loader Utility

        private static LazyAsset<Texture2D> LoadDeferred(string path)
        {
            // Don't attempt to load anything server-side.
            if (Main.netMode == NetmodeID.Server)
                return default;

            return LazyAsset<Texture2D>.Request(path, AssetRequestMode.ImmediateLoad);
        }

        #endregion Loader Utility
    }
}
