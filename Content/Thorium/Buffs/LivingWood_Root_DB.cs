using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Buffs
{
    public class LivingWood_Root_DB : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderDebuff";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;

        }
    }
    public class LivingWood_Root_B : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;

        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.controlJump = false;
            player.controlDown = false;
            player.controlLeft = false;
            player.controlRight = false;
            player.controlUp = false;
            player.controlUseItem = false;
            player.controlUseTile = false;
            player.controlThrow = false;
            player.controlMount = false;
            player.velocity = player.oldVelocity;
            player.position = player.oldPosition;
        }
        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
