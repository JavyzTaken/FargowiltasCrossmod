using Fargowiltas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Buffs.Placeables;
using Fargowiltas.Common.Configs;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCTileChanges : GlobalTile
    {
        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            BuffStationEffect(i, j, type, closer);
        }

        public static void BuffStationEffect(int i, int j, int type, bool closer)
        {
            if (!FargoServerConfig.Instance.PermanentStationsNearby)
                return;

            Player lp = Main.LocalPlayer;

            int blueCandleBuff = ModContent.BuffType<CirrusBlueCandleBuff>();
            int purpleCandleBuff = ModContent.BuffType<CirrusPurpleCandleBuff>();
            int pinkCandleBuff = ModContent.BuffType<CirrusPinkCandleBuff>();
            int yellowCandleBuff = ModContent.BuffType<CirrusYellowCandleBuff>();

            int buff = 0;
            SoundStyle? sound = null;
            bool mutexCandle = false;

            if (type == ModContent.TileType<BlueCandle>())
            {
                mutexCandle = true;
                buff = blueCandleBuff;
                sound = BlueCandle.ActivationSound;
            }
            else if (type == ModContent.TileType<PurpleCandle>())
            {
                mutexCandle = true;
                buff = purpleCandleBuff;
                sound = PurpleCandle.ActivationSound;
            }
            else if (type == ModContent.TileType<PinkCandle>())
            {
                mutexCandle = true;
                buff = pinkCandleBuff;
                sound = PinkCandle.ActivationSound;
            }
            else if (type == ModContent.TileType<YellowCandle>())
            {
                mutexCandle = true;
                buff = yellowCandleBuff;
                sound = YellowCandle.ActivationSound;
            }

            if (buff != 0 && lp.active && !lp.dead && !lp.ghost)
            {
                bool anyCandles = lp.HasBuff(blueCandleBuff) || lp.HasBuff(purpleCandleBuff) || lp.HasBuff(pinkCandleBuff) || lp.HasBuff(yellowCandleBuff);

                if (!(mutexCandle && anyCandles))
                {
                    bool noAlchemistNPC = !(ModLoader.HasMod("AlchemistNPC") || ModLoader.HasMod("AlchemistNPCLite")); // because it fucks with buffs for some reason and makes the sound spam WHY WHY WHY WHY WHAT'S WRONG WITH YOU WHY WHY WHY

                    if (!lp.HasBuff(buff) && sound.HasValue && noAlchemistNPC && lp.GetModPlayer<FargoPlayer>().StationSoundCooldown <= 0)
                    {
                        SoundEngine.PlaySound(sound.Value, new Vector2(i, j) * 16);
                        lp.GetModPlayer<FargoPlayer>().StationSoundCooldown = 60 * 60;
                    }
                    lp.AddBuff(buff, 108000);
                }
            }
        }
    }
}
