using FargowiltasCrossmod.Content.Thorium.Items.Accessories;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.EternityMode.Enemy
{
    [ExtendsFromMod("ThoriumMod")]
    public class Myna : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModLoader.GetMod("ThoriumMod").Find<ModNPC>("Myna").Type
        );

        public bool DroppedSummon;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.scale = 5;
            npc.lifeMax = 640;
            npc.damage = 24;
            npc.friendly = false;
            npc.noTileCollide = true;
        }

        public override void SetStaticDefaults()
        {
            Main.npcCatchable[ModLoader.GetMod("ThoriumMod").Find<ModNPC>("Myna").Type] = false;
        }

        public override void FindFrame(NPC npc, int frameHeight)
        {
            if ((int)npc.ai[0] != 0)
            {
                npc.frame.Y = 44;
            }
            else
            {
                npc.frame.Y = 0;
            }
        }

        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<Buffs.MynaDB>(), 600);
            base.OnHitPlayer(npc, target, hurtInfo);
        }

        public override bool SafePreAI(NPC npc)
        {
            float num = 6f;
            float num4 = 8f;
            float num6 = 250f;
            float veloScale = 2f;

            if (!npc.HasPlayerTarget)
            {
                npc.TargetClosest();
            }
            if (npc.HasPlayerTarget)
            {
                Player player = Main.player[npc.target];
                float num7 = npc.Distance(player.Center);
                Rectangle rectangle = new((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                Rectangle value = new((int)player.position.X, (int)player.position.Y, player.width, player.height);
                if (rectangle.Intersects(value))
                {
                    npc.noTileCollide = true;
                    if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num4)
                    {
                        npc.velocity *= 1.1f * veloScale;
                    }
                    if (npc.velocity.Length() > num4)
                    {
                        npc.velocity *= num4 * veloScale / npc.velocity.Length();
                    }
                }
                else if (num7 > num6)
                {
                    Vector2 value2;
                    value2 = npc.DirectionTo(player.Center);
                    npc.velocity = Vector2.Lerp(npc.velocity, value2 * num * veloScale, 0.15f);
                }
                else
                {
                    npc.noTileCollide = true;
                    Vector2 vector;
                    vector = npc.DirectionTo(player.Center);
                    npc.velocity += new Vector2(Math.Sign(vector.X), Math.Sign(vector.Y)) * 0.35f * veloScale;
                    if (npc.velocity.Length() > num4)
                    {
                        npc.velocity *= num4 * veloScale / npc.velocity.Length();
                    }
                }
                npc.rotation = npc.velocity.X / veloScale * 0.1f - MathF.PI / 2;
                npc.direction = npc.velocity.X > 0f ? 1 : -1;
                npc.spriteDirection = npc.velocity.X > 0f ? 1 : -1;
                return false;
            }
            npc.noTileCollide = true;

            return false;
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);

            FargowiltasSouls.FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<ThoriumMod.Items.Donate.ExoticMynaEgg>(), 1));
            FargowiltasSouls.FargoSoulsUtil.EModeDrop(npcLoot, ItemDropRule.Common(ModContent.ItemType<MynaAccessory>(), 1));
        }
    }
}
