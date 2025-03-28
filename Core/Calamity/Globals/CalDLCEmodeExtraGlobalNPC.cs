using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using FargowiltasSouls.Content.Items;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    /// <summary>
    /// For when several NPC types are needed to run the same code. 
    /// Example use case is modifying the damage reduction of the entire head, body and tail of a worm.
    /// </summary>
    public abstract class CalDLCEmodeExtraGlobalNPC : GlobalNPC
    {
        public NPCMatcher Matcher;

        public override bool InstancePerEntity => true;

        public sealed override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return lateInstantiation && Matcher.Satisfies(entity.type);
        }

        public override void Load()
        {
            Matcher = CreateMatcher();
            base.Load();
        }


        public abstract NPCMatcher CreateMatcher();
        public virtual bool RequiresEternityPriority => true;
        public override GlobalNPC NewInstance(NPC target) //the cursed beast
        {
            return ((RequiresEternityPriority && CalDLCWorldSavingSystem.E_EternityRev) || WorldSavingSystem.EternityMode) && ExtraRequirements() ? base.NewInstance(target) : null;
        }

        /// <summary>
        /// The behaviour only runs code if this returns true. Returns true by default.
        /// </summary>
        public virtual bool ExtraRequirements() { return true; }

        public bool FirstTick = true;
        public virtual void OnFirstTick(NPC npc) { }

        public virtual bool SafePreAI(NPC npc) => base.PreAI(npc);
        public sealed override bool PreAI(NPC npc)
        {
            if (!(CalDLCWorldSavingSystem.E_EternityRev && ExtraRequirements()))
            {
                return true;
            }
            if (FirstTick)
            {
                FirstTick = false;

                OnFirstTick(npc);
            }
            return SafePreAI(npc);
        }
        public virtual void SafePostAI(NPC npc) => base.PostAI(npc);
        public sealed override void PostAI(NPC npc)
        {
            if (!(CalDLCWorldSavingSystem.E_EternityRev && ExtraRequirements()))
            {
                return;
            }
            SafePostAI(npc);
            return;
        }

        protected static void NetSync(NPC npc, bool onlySendFromServer = true)
        {
            if (onlySendFromServer && Main.netMode != NetmodeID.Server)
                return;

            if (Main.netMode != NetmodeID.SinglePlayer)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI);
        }
    }
}
