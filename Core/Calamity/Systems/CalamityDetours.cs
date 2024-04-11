using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Mono.Cecil.Cil;
using Terraria;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using CalamityMod.NPCs;
using CalamityMod;
using FargowiltasCrossmod.Content.Calamity;
using FargowiltasCrossmod.Content.Calamity.Projectiles;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityDetours : ModSystem
    {
        public override void Load()
        {
            IL_Player.PickTile += ShootDaggersBreak;
            IL_Player.PlaceThing_Tiles_PlaceIt += ShootDaggersPlace;
            MonoModHooks.Modify(typeof(CalamityGlobalNPC).GetMethod(nameof(CalamityGlobalNPC.UpdateLifeRegen)), UpdateLifeRegen_ILEdit);
        }

        private void UpdateLifeRegen_ILEdit(ILContext il)
        {
            try{
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After, i => i.MatchStloc3());
                c.GotoNext(MoveType.After, i => i.MatchStloc3());
                c.GotoNext(MoveType.After, i => i.MatchStloc3());
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 3);
                c.EmitDelegate<Func<NPC, double, double>>((npc, number) =>
                {
                    
                    //CalamityGlobalNPC calnpc = null;
                    //CalGlobalNPC dlcnpc = null;
                    //calnpc = npc.GetGlobalNPC<CalamityGlobalNPC>();
                    //dlcnpc = npc.GetGlobalNPC<CalGlobalNPC>();
                    //Main.NewText("calnpc null test");
                    //Main.NewText(calnpc != null);
                    //Main.NewText("dlcnpc null test");
                    //Main.NewText(dlcnpc != null);
                    //Main.NewText("Vulnerable null test");
                    //Main.NewText(calnpc.VulnerableToHeat != null);
                    //Main.NewText("Vulnerable to heat");
                    //Main.NewText(calnpc.VulnerableToHeat.Value);
                    //Main.NewText("Is Scanned");
                    //Main.NewText(dlcnpc.WulfrumScanned);
                    //if (
                    //dlcnpc.WulfrumScanned)
                    //{
                    //    Main.NewText("lol");
                    //    dlcnpc.WulfrumScanned = false;
                    //    return number + 0.2D;
                        
                    //}
                    return number;
                });
                c.Emit(OpCodes.Stloc, 3);
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
            }
            catch(Exception e)
            {

            }
        }

        private void ShootDaggersBreak(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,i => i.MatchLdcI4(100));
                c.Index++;
                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldarg_1);
                c.Emit(OpCodes.Ldarg_2);
                c.Emit(OpCodes.Ldarg_3);
                c.EmitDelegate((Player player, int x, int y, int pickpower) =>
                {
                    if (player.HasEffect<MarniteLasersEffect>() && Main.netMode != NetmodeID.Server)
                    {
                        MarniteLasersEffect.MarniteTileEffect(player, new Microsoft.Xna.Framework.Vector2(x, y).ToWorldCoordinates());
                    }
                }
                );
                
            }
            catch (Exception e)
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
            }
        }
        private void ShootDaggersPlace(ILContext il)
        {
            try
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After, i => i.MatchStloc3());
                c.GotoNext(MoveType.After, i => i.MatchStloc3());
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate((Player player) =>
                {
                    if (player.HasEffect<MarniteLasersEffect>() && Main.netMode != NetmodeID.Server)
                    {
                        MarniteLasersEffect.MarniteTileEffect(player, new Microsoft.Xna.Framework.Vector2(Player.tileTargetX, Player.tileTargetY).ToWorldCoordinates());
                    }
                });
            }
            catch (Exception e)
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
            }
        }
    }
}
