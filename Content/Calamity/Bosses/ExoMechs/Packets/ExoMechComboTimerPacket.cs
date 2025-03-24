using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Core.Calamity.Systems;
using System.IO;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    public class ExoMechComboTimerPacket : BaseDLCPacket
    {
        public override void Write(ModPacket packet, params object[] context) =>
            packet.Write(ExoMechComboAttackManager.ComboAttackTimer);

        public override void Read(BinaryReader reader) =>
            ExoMechComboAttackManager.ComboAttackTimer = reader.ReadInt32();
    }
}
