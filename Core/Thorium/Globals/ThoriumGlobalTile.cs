using Fargowiltas;
using Fargowiltas.Common.Configs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Thorium.Globals;

public class ThoriumGlobalTile : GlobalTile
{
    public override void NearbyEffects(int i, int j, int type, bool closer)
    {
        if (FargoServerConfig.Instance.PermanentStationsNearby)
        {
            int buff = 0;
            SoundStyle? sound = null;
            if (type == ModContent.TileType<ThoriumMod.Tiles.Altar>())
            {
                buff = ModContent.BuffType<ThoriumMod.Buffs.Healer.AltarBuff>();
                sound = SoundID.NPCDeath7;
            }
            else if (type == ModContent.TileType<ThoriumMod.Tiles.ConductorsStand>())
            {
                buff = ModContent.BuffType<ThoriumMod.Buffs.Bard.ConductorsStandBuff>();
                sound = SoundID.NPCDeath7;
            }
            else if (type == ModContent.TileType<ThoriumMod.Tiles.NinjaRack>())
            {
                buff = ModContent.BuffType<ThoriumMod.Buffs.NinjaBuff>();
                sound = SoundID.Item53;
            }
            
            // from https://github.com/Fargowilta/Fargowiltas/blob/master/Items/Tiles/FargoGlobalTile.cs
            if (buff != 0 && Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
            {
                bool noAlchemistNPC = !(ModLoader.HasMod("AlchemistNPC") || ModLoader.HasMod("AlchemistNPCLite")); // because it fucks with buffs for some reason and makes the sound spam WHY WHY WHY WHY WHAT'S WRONG WITH YOU WHY WHY WHY
                if (!Main.LocalPlayer.HasBuff(buff) && sound.HasValue && noAlchemistNPC && Main.LocalPlayer.GetModPlayer<FargoPlayer>().StationSoundCooldown <= 0)
                {
                    SoundEngine.PlaySound(sound.Value, new Vector2(i, j) * 16);
                    Main.LocalPlayer.GetModPlayer<FargoPlayer>().StationSoundCooldown = 60 * 60;
                }
                Main.LocalPlayer.AddBuff(buff, 2);
            }
        }
    }
}