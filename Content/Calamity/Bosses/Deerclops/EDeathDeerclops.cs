
using CalamityMod.World;
using CalamityMod;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;
using FargowiltasCrossmod.Core;
using Terraria.ModLoader;
using CalamityMod.Events;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Core.Systems;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.EyeOfCthulhu
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDeerclops : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.Deerclops);
        public override void SetDefaults(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return;
            base.SetDefaults(npc);
            npc.lifeMax = (int)Math.Round(npc.lifeMax * 1.25f);
        }
    }
}
