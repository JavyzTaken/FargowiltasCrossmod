using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ArtemisAndApollo;
using FargowiltasCrossmod.Core.Calamity.Systems;
using System.IO;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    public class ExoMechExoTwinStatePacket : BaseDLCPacket
    {
        public override void Write(ModPacket packet, params object[] context) => ExoTwinsStateManager.SharedState.WriteTo(packet);

        public override void Read(BinaryReader reader) => ExoTwinsStateManager.SharedState.ReadFrom(reader);
    }
}
