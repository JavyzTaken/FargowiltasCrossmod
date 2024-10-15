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
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;
using CalamityMod.NPCs.TownNPCs;
using FargowiltasSouls.Core.ModPlayers;
using Terraria.Localization;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Core.Calamity.Systems
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonDetours : ModSystem
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override void Load()
        {
            On_Player.PickTile += ShootDaggersBreak;
            On_Player.PlaceThing_Tiles_PlaceIt += ShootDaggersPlace;
            On_Projectile.Damage += BigPlayer;
            On_Player.Update_NPCCollision += BigPlayerNPCs;
            On_LegacyPlayerRenderer.DrawPlayer += DrawBigPlayer;
            On_Player.RefreshDoubleJumps += ResetAeroCrit;

        }

        private void ResetAeroCrit(On_Player.orig_RefreshDoubleJumps orig, Player self)
        {
            CalDLCAddonPlayer addonPlayer = self.CalamityAddon();
            if (addonPlayer.NumJumpsUsed > 0)
            {
                int critPerJump = self.ForceEffect<AerospecJumpEffect>() ? 10 : 5;
                int critLost = critPerJump * addonPlayer.NumJumpsUsed;
                addonPlayer.NumJumpsUsed = 0;
                CombatText.NewText(self.Hitbox, Color.OrangeRed, Language.GetTextValue("Mods.FargowiltasCrossmod.Items.AerospecEnchant.CritReset", critLost), true);
            }
            orig(self);
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
    }
}
