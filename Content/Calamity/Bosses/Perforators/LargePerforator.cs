using CalamityMod.NPCs.Perforator;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.BaseClasses;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargePerforatorHead : WormHead
    {
        public override string Texture => "CalamityMod/NPCs/Perforator/PerforatorHeadLarge";
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 70;
            NPC.height = 84;
            NPC.scale = 1.2f;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(silver: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.netAlways = true;
            NPC.behindTiles = true;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {

            base.ApplyDifficultyAndPlayerScaling(numPlayers, balance, bossAdjustment);
        }

        public override void AI()
        {
            NPC.netUpdate = true; //fuck you worm mp code
            if (!NPC.AnyNPCs(ModContent.NPCType<PerforatorHive>()))
            {
                NPC.active = false;
            }
            NPC.ai[3]++;

            if (NPC.ai[3] >= 600 && NPC.ai[2] == 0)
            {
                NPC perf = null;
                foreach (NPC n in Main.npc)
                {
                    if (n != null && n.active && n.type == ModContent.NPCType<PerforatorHive>())
                    {
                        perf = n;
                    }
                }
                if (perf != null && perf.active && perf.type == ModContent.NPCType<PerforatorHive>())
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (perf.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20, 0.08f);
                    if (NPC.Distance(perf.Center) <= 20)
                    {
                        NPC.ai[2] = 1;
                    }
                }

            }
            if (NPC.ai[2] == 1)
            {
                NPC perf = null;
                foreach (NPC n in Main.npc)
                {
                    if (n != null && n.active && n.type == ModContent.NPCType<PerforatorHive>())
                    {
                        perf = n;
                    }
                    if (n != null && n.active && (n.type == ModContent.NPCType<LargePerforatorBody>() || n.type == ModContent.NPCType<LargePerforatorBody2>() ||
                        n.type == ModContent.NPCType<LargePerforatorTail>()))
                    {
                        n.velocity = (NPC.Center - n.Center).SafeNormalize(Vector2.Zero) * 20;
                        if (n.type == ModContent.NPCType<LargePerforatorTail>() && n.Distance(NPC.Center) <= 20)
                        {
                            NPC.active = false;
                        }
                    }
                }
                if (perf != null && perf.active && perf.type == ModContent.NPCType<PerforatorHive>())
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, (perf.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20, 0.03f);
                    if (NPC.Distance(perf.Center) <= 30)
                    {
                        NPC.ai[2] = 1;
                    }
                }
                NPC.Center = perf.Center;
            }
        }

        public override int BodyType => ModContent.NPCType<LargePerforatorBody>();
        public override void Init()
        {
            MinSegmentLength = 11;
            MaxSegmentLength = 11;
            CommonWormInit(this);

        }
        internal static void CommonWormInit(Worm worm)
        {
            worm.MoveSpeed = 12f;
            worm.Acceleration = 0.2f;
        }

        public override int TailType => ModContent.NPCType<LargePerforatorTail>();
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargePerforatorBody : WormBody
    {
        public override string Texture => "CalamityMod/NPCs/Perforator/PerforatorBodyLarge";
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.scale = 1.2f;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(silver: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.netAlways = true;
            NPC.behindTiles = true;
        }
        internal override void BodyTailAI()
        {
            if (NPC.realLife >= 0)
            {
                NPC owner = Main.npc[NPC.realLife];
                if (owner != null && owner.ai[2] == 1 && owner.active)
                {
                    return;
                }
            }
            base.BodyTailAI();
        }
        public override void AI()
        {
            NPC.netUpdate = true; //fuck you worm mp code
            if (NPC.realLife >= 0)
            {
                NPC owner = Main.npc[NPC.realLife];
                if (owner != null && owner.ai[2] == 1)
                {
                    return;
                }
            }
            base.AI();
        }
        public override void Init()
        {
            LargePerforatorHead.CommonWormInit(this);
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargePerforatorBody2 : WormBody
    {
        public override string Texture => "CalamityMod/NPCs/Perforator/PerforatorBodyLargeAlt";
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.scale = 1.2f;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(silver: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.netAlways = true;
            NPC.behindTiles = true;
        }
        internal override void BodyTailAI()
        {
            if (NPC.realLife >= 0)
            {
                NPC owner = Main.npc[NPC.realLife];
                if (owner != null && owner.ai[2] == 1 && owner.active)
                {
                    return;
                }
            }
            base.BodyTailAI();
        }
        public override void AI()
        {
            NPC.netUpdate = true; //fuck you worm mp code
            if (NPC.realLife >= 0)
            {
                NPC owner = Main.npc[NPC.realLife];
                if (owner != null && owner.ai[2] == 1)
                {
                    return;
                }
            }
            base.AI();
        }

        public override void Init()
        {
            LargePerforatorHead.CommonWormInit(this);
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class LargePerforatorTail : WormTail
    {
        public override string Texture => "CalamityMod/NPCs/Perforator/PerforatorTailLarge";
        public override void SetStaticDefaults()
        {

            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);

            NPCID.Sets.BossBestiaryPriority.Add(Type);

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 60;
            NPC.scale = 1.2f;
            NPC.lifeMax = 1200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(silver: 10);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.netAlways = true;
            NPC.behindTiles = true;
        }
        internal override void BodyTailAI()
        {
            if (NPC.realLife >= 0)
            {
                NPC owner = Main.npc[NPC.realLife];
                if (owner != null && owner.ai[2] == 1 && owner.active)
                {
                    return;
                }
            }
            base.BodyTailAI();
        }
        public override void AI()
        {
            NPC.netUpdate = true; //fuck you worm mp code
            if (NPC.realLife >= 0)
            {
                NPC owner = Main.npc[NPC.realLife];
                if (owner != null && owner.ai[2] == 1)
                {
                    return;
                }
            }
            //base.AI();
        }
        public override void Init()
        {
            LargePerforatorHead.CommonWormInit(this);
        }
    }
}