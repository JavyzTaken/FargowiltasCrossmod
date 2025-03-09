using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCEmodeBehaviorManager : GlobalNPC
    {
        /// <summary>
        /// The relationship of NPC ID to corresponding override.
        /// </summary>
        internal static readonly Dictionary<int, CalDLCEmodeBehavior> CalDLCBehaviorRelationship = [];

        /// <summary>
        /// The behavior override that governs the behavior of a given NPC.
        /// </summary>
        internal CalDLCEmodeBehavior? DLCBehaviour;

        public override bool InstancePerEntity => true;

        public bool ShouldBeActive => DLCBehaviour is not null && CalDLCWorldSavingSystem.E_EternityRev && DLCBehaviour.ExtraRequirements();

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (DLCBehaviour == null && CalDLCBehaviorRelationship.TryGetValue(npc.type, out CalDLCEmodeBehavior? behaviorOverride) && CalDLCWorldSavingSystem.E_EternityRev && behaviorOverride.ExtraRequirements())
            {
                DLCBehaviour = behaviorOverride!.Clone(npc);
                DLCBehaviour.OnSpawn(source);
            }
        }

        public override void SetBestiary(NPC npc, BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            if (CalDLCBehaviorRelationship.TryGetValue(npc.type, out CalDLCEmodeBehavior? behaviorOverride) && CalDLCWorldSavingSystem.E_EternityRev && behaviorOverride.ExtraRequirements())
                behaviorOverride?.SetBestiary(database, bestiaryEntry);
        }
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            if (DLCBehaviour == null && CalDLCBehaviorRelationship.TryGetValue(npc.type, out CalDLCEmodeBehavior? behaviorOverride) && CalDLCWorldSavingSystem.E_EternityRev && behaviorOverride.ExtraRequirements())
            {
                DLCBehaviour = behaviorOverride!.Clone(npc);
                DLCBehaviour?.SetDefaults();
            }
        }

        public override bool PreAI(NPC npc)
        {
            if (ShouldBeActive)
            {
                if (DLCBehaviour.FirstTick)
                {
                    DLCBehaviour.FirstTick = false;

                    DLCBehaviour.OnFirstTick();
                }
                return DLCBehaviour.PreAI();
            }

            return true;
        }
        public override void PostAI(NPC npc)
        {
            if (!ShouldBeActive)
                return;
            DLCBehaviour?.PostAI();
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.FindFrame(frameHeight);
        }

        public override void BossHeadSlot(NPC npc, ref int index)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.BossHeadSlot(ref index);
        }

        public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.ModifyTypeName(ref typeName);
        }

        public override bool PreKill(NPC npc)
        {
            if (!ShouldBeActive)
                return true;

            return DLCBehaviour?.PreKill() ?? true;
        }
        public override void OnKill(NPC npc)
        {
            if (!ShouldBeActive)
                return;
            DLCBehaviour?.OnKill();
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.ModifyNPCLoot(npcLoot);
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) => DLCBehaviour?.SendExtraAI(bitWriter, binaryWriter);

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) => DLCBehaviour?.ReceiveExtraAI(bitReader, binaryReader);

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.ModifyHitByProjectile(projectile, ref modifiers);
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.ModifyHitByItem(player, item, ref modifiers);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (!ShouldBeActive)
                return;
            DLCBehaviour?.OnHitByProjectile(projectile, hit, damageDone);
        }

        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (!ShouldBeActive)
                return base.CanHitPlayer(npc, target, ref cooldownSlot);
            return DLCBehaviour?.CanHitPlayer(target, ref cooldownSlot) ?? true;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!ShouldBeActive)
                return;
            DLCBehaviour?.UpdateLifeRegen(ref damage);
        }

        public override Color? GetAlpha(NPC npc, Color drawColor)
        {
            if (ShouldBeActive)
                return DLCBehaviour.GetAlpha(drawColor);

            return null;
        }

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (!ShouldBeActive)
                return;

            DLCBehaviour?.HitEffect(hit);
        }

        public override bool CheckDead(NPC npc)
        {
            if (!ShouldBeActive)
                return true;

            return DLCBehaviour?.CheckDead() ?? true;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!ShouldBeActive || npc.IsABestiaryIconDummy)
                return true;

            return DLCBehaviour?.PreDraw(spriteBatch, screenPos, drawColor) ?? true;
        }
        public override void DrawBehind(NPC npc, int index)
        {
            if (!ShouldBeActive)
                return;
            DLCBehaviour?.DrawBehind(index);
        }
    }
}
