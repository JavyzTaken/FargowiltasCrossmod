using System;
using System.Collections.Generic;
using System.IO;
using FargowiltasSouls.Core.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    public class PacketManager : ModSystem
    {

        internal static Dictionary<string, BaseDLCPacket> RegisteredPackets = [];

        public override void OnModLoad()
        {
            RegisteredPackets = [];
            foreach (Type t in AssemblyManager.GetLoadableTypes(Mod.Code))
            {
                if (!t.IsSubclassOf(typeof(BaseDLCPacket)) || t.IsAbstract)
                    continue;

                BaseDLCPacket packet = Activator.CreateInstance(t) as BaseDLCPacket;
                RegisteredPackets[t.FullName] = packet;
            }
        }

        internal static void PreparePacket(BaseDLCPacket packet, object[] context, short? sender = null)
        {
            // Don't try to send packets in singleplayer.
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            // Assume the sender is the current client if nothing else is supplied.
            sender ??= (short)Main.myPlayer;

            ModPacket wrapperPacket = FargowiltasCrossmod.Instance.GetPacket();

            // Write the identification header. This is necessary to ensure that on the receiving end the reader know how to interpret the packet.
            wrapperPacket.Write(packet.GetType().FullName);

            // Write the sender if the packet needs to be re-sent from the server.
            if (packet.ResendFromServer)
                wrapperPacket.Write(sender.Value);

            // Write the requested packet data.
            packet.Write(wrapperPacket, context);

            // Send the packet.
            wrapperPacket.Send(-1, sender.Value);
        }

        public static void SendPacket<T>(params object[] context) where T : BaseDLCPacket
        {
            // Verify that the packet is registered before trying to send it.
            string packetName = typeof(T).FullName;
            if (Main.netMode == NetmodeID.SinglePlayer || !RegisteredPackets.TryGetValue(packetName, out BaseDLCPacket packet))
                return;

            PreparePacket(packet, context);
        }

        public static void ReceivePacket(BinaryReader reader)
        {
            // Read the identification header to determine how the packet should be processed.
            string packetName = reader.ReadString();

            // If no valid packet could be found, get out of here.
            // There will inevitably be a reader underflow error caused by TML's packet policing, but there aren't any clear-cut solutions that
            // I know of that adequately addresses that problem, and from what I can tell it's never catastrophic when it happens.
            if (!RegisteredPackets.TryGetValue(packetName, out BaseDLCPacket packet))
                return;

            // Determine who sent this packet if it needs to resend.
            short sender = -1;
            if (packet.ResendFromServer)
                sender = reader.ReadInt16();

            // Read off requested packet data.
            packet.Read(reader);

            // If this packet was received server-side and the packet needs to be re-sent, send it back to all the clients, with the
            // exception of the one that originally brought this packet to the server.

            // TODO -- Through this the original context is destroyed. How would this be best addressed? By forcing packets to glue the context back together
            // in the Read hook? That seems a bit stupid, but I don't know what options there are that are actually reasonable.
            if (Main.netMode == NetmodeID.Server && packet.ResendFromServer)
                PreparePacket(packet, Array.Empty<object>(), sender);
        }
    }
    public abstract class BaseDLCPacket
    {
        //Yes this is shamelessly stolen from infernum (with permission)

        // This determines whether a packet should be sent back to clients once on the server. This applies in cases where a client
        // needs to inform the server of a change, and the packet can't be sent from the server itself (such as if a player makes a left click).
        // This is important because it isn't enough to just send a packet and be done with it, as TML has hidden rules with its packet structure:
        // 1. Packets sent from clients go to the server.
        // 2. Packets sent from the server go to the clients (with the optional exception of one client if you supply a client that should not recieve the packet).

        // As a rule of thumb, leave this as true if you're doing anything client-specific related, such as taking in player inputs.
        // If it's for something server-specific (such as world operations) this doesn't matter much.
        public virtual bool ResendFromServer => true;

        // RULES FOR SETTING UP PACKETS
        // 1: Make sure to read things in the order they were written in, like a conveyer belt. The computer is not a magic device that can infer usage context, it simply
        // receives a stream of bytes. As such, it's your job to ensure that the data is interpreted correctly.

        // 1.1: Be careful when reading data in conditionals, as they might not be triggered, resulting in violations of rule 1.

        // 1.2: Never use reader.ReadInt32() or similar things inside of a loop iterator directly. If you need to keep a counter for a loop, store it as a separate local variable.
        // Using the Read methods inside of a loop check will involve going through 4 bytes *for every loop iteration*, which is pretty much never
        // the behavior you want (and if it is, you'd best leave a comment to ensure that readers know why).

        // 2: Do your best to ensure that ALL data is read, even in failure cases. If you can, try to collect all BinaryReader information in local variables at the top
        // of the Read hook, and then once that's complete perform any necessary early returns/failure cases if said data is garbage.
        public abstract void Write(ModPacket packet, params object[] context);

        public abstract void Read(BinaryReader reader);
    }
    public class EternityRevPacket : BaseDLCPacket
    {
        public override void Write(ModPacket packet, params object[] context)
        {
            BitsByte containmentFlagWrapper = new()
            {
                [0] = CalDLCWorldSavingSystem.EternityRev,
                [1] = WorldSavingSystem.EternityMode,
                [2] = WorldSavingSystem.ShouldBeEternityMode
            };
            packet.Write(containmentFlagWrapper);
        }

        public override void Read(BinaryReader reader)
        {
            BitsByte containmentFlagWrapper = reader.ReadByte();
            CalDLCWorldSavingSystem.EternityRev = containmentFlagWrapper[0];
            WorldSavingSystem.EternityMode = containmentFlagWrapper[1];
            WorldSavingSystem.ShouldBeEternityMode = containmentFlagWrapper[2];
            if (WorldSavingSystem.ShouldBeEternityMode)
                WorldSavingSystem.SpawnedDevi = true;
        }
    }
    public class EternityDeathPacket : BaseDLCPacket
    {
        public override void Write(ModPacket packet, params object[] context)
        {
            BitsByte containmentFlagWrapper = new()
            {
                [0] = CalDLCWorldSavingSystem.EternityDeath,
                [1] = CalDLCWorldSavingSystem.EternityRev,
                [2] = WorldSavingSystem.EternityMode,
                [3] = WorldSavingSystem.ShouldBeEternityMode
            };
            packet.Write(containmentFlagWrapper);
        }

        public override void Read(BinaryReader reader)
        {
            BitsByte containmentFlagWrapper = reader.ReadByte();
            CalDLCWorldSavingSystem.EternityDeath = containmentFlagWrapper[0];
            CalDLCWorldSavingSystem.EternityRev = containmentFlagWrapper[1];
            WorldSavingSystem.EternityMode = containmentFlagWrapper[2];
            WorldSavingSystem.ShouldBeEternityMode = containmentFlagWrapper[3];
            if (WorldSavingSystem.ShouldBeEternityMode)
                WorldSavingSystem.SpawnedDevi = true;
        }
    }
}
