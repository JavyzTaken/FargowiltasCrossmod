using System.IO;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.QueenSlime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathQS : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.QueenSlimeBoss;
        public override void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(slam);
        }
        public override void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader)
        {
            slam = binaryReader.ReadBoolean();
        }
        public bool slam = false;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            if (NPC.ai[0] == 4)
            {
                slam = true;

            }
            else if (slam)
            {
                if (DLCUtils.HostCheck)
                    for (int i = 0; i < 11; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-10, 0).RotatedBy(MathHelper.ToRadians(180 / 10f * i)), ModContent.ProjectileType<HallowedSlimeSpike>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
                    }

                slam = false;
            }
            return true;
        }
    }
}
