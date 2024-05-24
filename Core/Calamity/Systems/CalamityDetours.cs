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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Renderers;
using Terraria.Graphics;
using Terraria.Chat;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityDetours : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override void Load()
        {
            On_Player.PickTile += ShootDaggersBreak;
            On_Player.PlaceThing_Tiles_PlaceIt += ShootDaggersPlace;
            MonoModHooks.Modify(typeof(CalamityGlobalNPC).GetMethod(nameof(CalamityGlobalNPC.UpdateLifeRegen)), UpdateLifeRegen_ILEdit);
            On_Projectile.Damage += BigPlayer;
            On_Player.Update_NPCCollision += BigPlayerNPCs;
            On_LegacyPlayerRenderer.DrawPlayer += DrawBigPlayer;

        }

        private TileObject ShootDaggersPlace(On_Player.orig_PlaceThing_Tiles_PlaceIt orig, Player self, bool newObjectType, TileObject data, int tileToCreate)
        {
            TileObject returnvalue = orig(self, newObjectType, data, tileToCreate);
            if (self.HasEffect<MarniteLasersEffect>() && Main.netMode != NetmodeID.Server && Main.tile[Main.MouseWorld.ToTileCoordinates()].HasTile)
            {
               
                MarniteLasersEffect.MarniteTileEffect(self, Main.MouseWorld);
            }
            return returnvalue;
        }

        private void ShootDaggersBreak(On_Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
        {
            if (self.HasEffect<MarniteLasersEffect>() && Main.netMode != NetmodeID.Server)
            {
                MarniteLasersEffect.MarniteTileEffect(self, new Microsoft.Xna.Framework.Vector2(x, y).ToWorldCoordinates());
            }
            orig(self, x, y, pickPower);
        }

        private void DrawBigPlayer(On_LegacyPlayerRenderer.orig_DrawPlayer orig, LegacyPlayerRenderer self, Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float scale)
        {
            if (drawPlayer.HasEffect<TitanHeartEffect>())
                scale = 2;
            orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
        }

        private void BigPlayerNPCs(On_Player.orig_Update_NPCCollision orig, Player self)
        {
            Player player = self;
            Vector2 size = player.Size;
            Vector2 position = player.position;
            if (player.HasEffect<TitanHeartEffect>())
            {
                player.width += 20;
                player.height += 30;
                player.position.X -= 10;
                player.position.Y -= 15;
            }
            orig(self);
            player.width = (int)size.X;
            player.height = (int)size.Y;
            player.position = position;
        }

        private void BigPlayer(On_Projectile.orig_Damage orig, Projectile self)
        {
            Player player = Main.LocalPlayer;
            Vector2 size = player.Size;
            Vector2 position = player.position;
            if (player.HasEffect<TitanHeartEffect>())
            {
                player.width += 20;
                player.height += 30;
                player.position.X -= 10;
                player.position.Y -= 15;
            }
            orig(self);
            player.width = (int)size.X;
            player.height = (int)size.Y;
            player.position = position;
        }

        private void UpdateLifeRegen_ILEdit(ILContext il)
        {
            //try{
            //    ILCursor c = new ILCursor(il);
            //    c.GotoNext(MoveType.After, i => i.MatchStloc3());
            //    c.GotoNext(MoveType.After, i => i.MatchStloc3());
            //    c.GotoNext(MoveType.After, i => i.MatchStloc3());
            //    c.Index++;
            //    c.Emit(OpCodes.Ldarg_0);
            //    c.Emit(OpCodes.Ldloc, 3);
            //    c.EmitDelegate<Func<NPC, double, double>>((npc, number) =>
            //    {
                    
            //        //CalamityGlobalNPC calnpc = null;
            //        //CalGlobalNPC dlcnpc = null;
            //        //calnpc = npc.GetGlobalNPC<CalamityGlobalNPC>();
            //        //dlcnpc = npc.GetGlobalNPC<CalGlobalNPC>();
            //        //Main.NewText("calnpc null test");
            //        //Main.NewText(calnpc != null);
            //        //Main.NewText("dlcnpc null test");
            //        //Main.NewText(dlcnpc != null);
            //        //Main.NewText("Vulnerable null test");
            //        //Main.NewText(calnpc.VulnerableToHeat != null);
            //        //Main.NewText("Vulnerable to heat");
            //        //Main.NewText(calnpc.VulnerableToHeat.Value);
            //        //Main.NewText("Is Scanned");
            //        //Main.NewText(dlcnpc.WulfrumScanned);
            //        //if (
            //        //dlcnpc.WulfrumScanned)
            //        //{
            //        //    Main.NewText("lol");
            //        //    dlcnpc.WulfrumScanned = false;
            //        //    return number + 0.2D;
                        
            //        //}
            //        return number;
            //    });
            //    c.Emit(OpCodes.Stloc, 3);
            //    MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
            //}
            //catch(Exception e)
            //{

            //}
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
            catch (Exception)
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
            catch (Exception)
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<FargowiltasCrossmod>(), il);
            }
        }
    }
}
