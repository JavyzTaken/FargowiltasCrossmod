using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameInput;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Thorium.Buffs;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Thorium.NPCs;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;

namespace FargowiltasCrossmod.Content.Thorium
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public partial class CrossplayerThorium : ModPlayer
    {
        #region enchants
        public bool TemplarEnch;
        public Item TemplarEnchItem;
        public bool WhiteKnightEnch;
        public bool ValadiumEnch;
        public Item ValadiumEnchItem;
        public bool NagaSkinEnch;
        public bool TitanEnch;
        public bool WhisperingEnch;
        public Item WhisperingEnchItem;
        public bool JesterEnch;
        public Item JesterEnchItem;

        public bool HelheimForce;
        public bool SvartalfheimForce;

        public List<int> LodeStonePlatforms = new();
        public List<int> ActiveValaChunks = new();

        internal int TemplarCD = 360;
        internal int ValadiumCD = 240;
        internal int AstroLaserCD = 60;
        internal int BronzeCD = 45;
        internal int warlockReleaseTimer;
        
        public bool WasInDashState = false; //for tide hunter mainly

        internal int SteelTeir;
        internal bool soulEssenceHit;

        internal Vector2[] nagaSkinLegs = new Vector2[4];
        internal Vector2[] nagaSkinLegTargets = new Vector2[4];
        #endregion

        public bool GildedMonicle;
        public bool GildedBinoculars;
        public bool MynaAccessory;

        public Item TempleCoreItem;
        public int TempleCoreCounter;

        public override void ResetEffects()
        {
            TemplarEnch = false;
            TemplarEnchItem = null;
            WhiteKnightEnch = false;
            ValadiumEnch = false;
            ValadiumEnchItem = null;
            NagaSkinEnch = false;
            TitanEnch = false;
            WhisperingEnch = false;
            WhisperingEnchItem = null;
            JesterEnch = false;
            JesterEnchItem = null;
            
            HelheimForce = false;
            SvartalfheimForce = false;

            GildedMonicle = false;
            GildedBinoculars = false;
            MynaAccessory = false;

            TempleCoreItem = null;

            SteelTeir = 0;
        }

        public override void UpdateLifeRegen()
        {
            if (DepthBubble > 0)
            {
                Player.lifeRegen += 4;
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ThoriumKeybinds.LivingWoodBind.JustPressed)
            {
                if (Player.HasEffect<LivingWoodEffect>()) LivingWoodKey();
                else if (Player.HasEffect<LifeBloomEffect>()) LifeBloomKey();
            }
            if (ThoriumKeybinds.SteelParryBind.JustPressed && Player.HasEffect<SteelEffect>())
            {
                ParryKey();
            }
            if (ThoriumMod.ThoriumHotkeySystem.AccessoryKey.JustPressed)
            {
                if (Player.HasEffect<FallenPaladinEffect>())
                {
                    FallenPaladinKey();
                }

                if (Player.HasEffect<WarlockEffect>() || Player.HasEffect<SacredEffect>())
                {
                    int type = ModContent.ProjectileType<DLCShadowWisp>();

                    if (Player.ownedProjectileCounts[type] >= 24)
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            Projectile projectile = Main.projectile[i];
                            if (projectile.active && projectile.owner == Main.myPlayer && projectile.type == type)
                            {
                                projectile.Kill();
                            }
                        }

                        warlockReleaseTimer = (Player.HasEffect<WarlockEffect>() && Player.HasEffect<SacredEffect>()) ? 180 : 120;
                    }
                    else
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            Projectile projectile = Main.projectile[i];
                            if (projectile.active && projectile.owner == Main.myPlayer && projectile.type == type)
                            {
                                projectile.localAI[0] = 1f;
                            }
                        }
                    }
                }
            }

            if (ThoriumMod.ThoriumHotkeySystem.EncaseKey.JustPressed)
            {
                if (Player.HasEffect<ShadeMasterEffect>())
                {
                    ShadeMasterEnter();
                }
            }
        }

        public override void PreUpdate()
        {
            if (TempleCoreItem != null)
            {
                TempleCoreEffect();
            }
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (Player.HasEffect<YewWoodEffect>())
            {
                YewWoodShootStats(item, ref position, ref velocity, ref type, ref damage, ref knockback);
            }
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (NoviceClericCrosses > 0) mult = 0f;
        }

        public override void PostUpdateEquips()
        {
            WasInDashState = Player.FargoSouls().IsInADashState;

            // needs to be outside of effect class so it actually triggers
            if (!Player.HasEffect<ShadeMasterEffect>() || shadeMasterDuration == -1)
            {
                if (shadeMasterDuration > 0)
                    ShadeMasterExit();
            }

            if (!Player.HasEffect<NoviceClericEffect>())
            {
                NoviceClericCrosses = 0;
            }
        }

        public override void PostUpdate()
        {
            Core.Thorium.Globals.ThoriumPotionNerfs.MurderBuffs(Player);
        }

        public override void FrameEffects()
        {
            if (ShadeMode && Player.HasEffect<ShadeMasterEffect>())
            {
                // replace with modded vanity
                Player.head = 120; // ghost vanity
                Player.body = 81; 
                Player.legs = 169;

                Player.wings = 11; // specter wings
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (ShadeMode && drawInfo.shadow == 0)
            {
                Main.spriteBatch.End();

                // draws the dummy / 'body'.
                Main.PlayerRenderer.DrawPlayer(Main.Camera, shadeMasterDummy, shadeMasterDummy.position, shadeMasterDummy.fullRotation, shadeMasterDummy.fullRotationOrigin);

                Main.spriteBatch.Begin();
            }
        }
    }
}
