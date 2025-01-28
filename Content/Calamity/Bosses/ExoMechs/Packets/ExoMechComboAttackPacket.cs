using FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.ComboAttacks;
using FargowiltasCrossmod.Core.Calamity.Systems;
using System.IO;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs
{
    public class ExoMechComboAttackPacket : BaseDLCPacket
    {
        public override void Write(ModPacket packet, params object[] context)
        {
            int comboAttackIndex = ExoMechComboAttackManager.RegisteredComboAttacks.IndexOf(ExoMechComboAttackManager.CurrentState);
            packet.Write(comboAttackIndex);
        }

        public override void Read(BinaryReader reader)
        {
            int comboAttackIndex = reader.ReadInt32();
            if (comboAttackIndex >= 0 && comboAttackIndex < ExoMechComboAttackManager.RegisteredComboAttacks.Count)
                ExoMechComboAttackManager.CurrentState = ExoMechComboAttackManager.RegisteredComboAttacks[comboAttackIndex];
            else
                ExoMechComboAttackManager.CurrentState = ExoMechComboAttackManager.NullComboState;
        }
    }
}
