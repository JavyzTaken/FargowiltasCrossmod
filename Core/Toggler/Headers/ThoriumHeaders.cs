using FargowiltasSouls.Core.Toggler.Content;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Forces;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Toggler.Content
{
    public class AlfheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<AlfheimForce>();
    }
    public class AsgardHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<AsgardForce>();
    }
    public class JotunheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<JotunheimForce>();
    }
    public class VanaheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<VanaheimForce>();
    }
    public class NiflheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<NiflheimForce>();
    }
    public class MuspelheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<MuspelheimForce>();
    }
    public class HelheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<HelheimForce>();
    }
    public class MidgardHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<MidgardForce>();
    }
    public class SvartalfheimHeader : EnchantHeader
    {
        public override int Item => ModContent.ItemType<SvartalfheimForce>();
    }
}