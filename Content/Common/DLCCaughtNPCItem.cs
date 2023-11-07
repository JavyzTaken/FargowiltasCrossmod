using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fargowiltas.Items.CaughtNPCs;
using Fargowiltas.Utilities.Extensions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using ThoriumMod.NPCs;

namespace FargowiltasCrossmod.Content.Common
{
    public class DLCCaughtNPCItem : ModItem
    {
        internal struct NPCInfo
        {
            internal enum Mod { Thorium, Calamity };
            internal NPCInfo(int npcID, int itemID, Mod mod)
            {
                this.npcID = npcID;
                this.itemID = itemID;
                this.mod = mod;
            }
            internal readonly int npcID;
            internal readonly int itemID;
            internal readonly Mod mod;
        }
        internal static Dictionary<int, int> CaughtTownies = new();

        public override string Name => _name;

        public string _name;
        public int AssociatedNpcId;
        public string NpcQuote;

        public DLCCaughtNPCItem()
        {
            _name = base.Name;
            AssociatedNpcId = NPCID.None;
            NpcQuote = "";
        }

        public DLCCaughtNPCItem(string internalName, int associatedNpcId, string npcQuote = "")
        {
            _name = internalName;
            AssociatedNpcId = associatedNpcId;
            NpcQuote = npcQuote;
        }

        public override bool IsLoadingEnabled(Mod mod) => AssociatedNpcId != NPCID.None;

        protected override bool CloneNewInstances => true;

        public override ModItem Clone(Item item)
        {
            CaughtNPCItem clone = base.Clone(item) as CaughtNPCItem;
            clone._name = _name;
            clone.AssociatedNpcId = AssociatedNpcId;
            clone.NpcQuote = NpcQuote;
            return clone;
        }

        public override bool IsCloneable => true;

        public override void Unload()
        {
            CaughtTownies.Clear();
        }

        public override string Texture => AssociatedNpcId < NPCID.Count ? $"Terraria/Images/NPC_{AssociatedNpcId}" : NPCLoader.GetNPC(AssociatedNpcId).Texture;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(6, Main.npcFrameCount[AssociatedNpcId]));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToCapturedCritter(AssociatedNpcId);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item44;
        }

        public override bool CanUseItem(Player player)
        {
            return player.IsTileWithinRange(Player.tileTargetX, Player.tileTargetY) && !WorldGen.SolidTile(Player.tileTargetX, Player.tileTargetY) && NPC.CountNPCS(AssociatedNpcId) < 5;
        }

        public override bool? UseItem(Player player)
        {
            return true;
        }

        public static void RegisterItems()
        {
            CaughtTownies = new();

            if (Core.ModCompatibility.ThoriumMod.Loaded) RegisterThoriumItems();
        }

        private static void RegisterThoriumItems()
        {
            Add("Cobbler", ModContent.NPCType<Cobbler>(), "");
            Add("Desert Acolyte", ModContent.NPCType<DesertAcolyte>(), "");
            Add("Cook", ModContent.NPCType<Cook>(), "'Say my name'");
            Add("Confused Zombie", ModContent.NPCType<ConfusedZombie>(), "'guh?'");
            Add("Blacksmith", ModContent.NPCType<Blacksmith>(), "");
            Add("Tracker", ModContent.NPCType<Tracker>(), "");
            Add("Diverman", ModContent.NPCType<Diverman>(), "'...Sam?'");
            Add("Druid", ModContent.NPCType<Druid>(), "");
            Add("Spiritualist", ModContent.NPCType<Spiritualist>(), "");
            Add("Weapon master", ModContent.NPCType<WeaponMaster>(), "");
        }

        internal static void Add(string internalName, int id, string quote)
        {
            CaughtNPCItem item = new(internalName, id, quote);
            FargowiltasCrossmod.Instance.AddContent(item);
            CaughtTownies.Add(id, item.Type);
        }
    }

    public class DLCCaughtGlobalNPC : GlobalNPC
    {
        private static HashSet<int> npcCatchableWasFalse;

        public override void Load()
        {
            npcCatchableWasFalse = new HashSet<int>();
        }

        public override void Unload()
        {
            if (npcCatchableWasFalse != null)
            {
                foreach (var type in npcCatchableWasFalse)
                {
                    //Failing to unload this properly causes it to bleed into un-fargowiltas gameplay, causing various issues such as clients not being able to join a server
                    Main.npcCatchable[type] = false;
                }
                npcCatchableWasFalse = null;
            }
        }

        public override void SetDefaults(NPC npc)
        {
            int type = npc.type;
            if (DLCCaughtNPCItem.CaughtTownies.ContainsKey(type) && ModContent.GetInstance<Fargowiltas.Common.Configs.FargoServerConfig>().CatchNPCs)
            {
                npc.catchItem = (short)DLCCaughtNPCItem.CaughtTownies.FirstOrDefault(i => i.Key == type).Value;
                if (!Main.npcCatchable[type])
                {
                    npcCatchableWasFalse.Add(type);
                    Main.npcCatchable[type] = true;
                }
            }
        }
    }
}