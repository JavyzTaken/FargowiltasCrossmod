using Terraria;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using CalamityMod.World;
using Terraria.GameInput;
using Terraria.DataStructures;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.ModPlayers;
using ThoriumMod.PlayerLayers;
using System.Collections.Generic;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Content.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public partial class CrossplayerCalamity : ModPlayer
    {
        //effect booleans
        //force of exploration
        public bool RideOfTheValkyrie;
        public bool Marnite;
        public bool WulfrumOverpower;
        public bool ProwlinOnTheFools;
        public bool ExploEffects;
        //force of devestation
        public bool ReaverHage;
        public bool ReaverHageBuff;
        public bool ButterBeeSwarm;
        public bool AyeCicle;
        public bool AyeCicleSmol;
        public bool AtaxiaEruption;
        public bool DevastEffects;
        //force og desolation
        public bool Empyrean;
        public bool OmegaBlue;
        public int OmegaGreenCounter;
        public bool VictideSwimmin;
        public bool Mollusk;
        public bool SulphurBubble;
        public bool FathomBubble;
        public bool DoctorBeeKill;
        public bool TitanHeart;
        public int TitanGuardCooldown;
        public bool Astral;
        public bool DesolEffects;
        //force of exaltation
        public bool Tarragon;
        public int TarragonTimer;
        public bool UmbraCrazyRegen;
        public bool BFCrazierRegen;
        public bool Silva;
        public int SilvaTimer;
        public bool StatigelNinjaStyle;
        public bool GodSlayerMeltdown;
        public bool SlayerCD;
        public bool Auric;
        public bool ExaltEffects;
        //force of annihilation
        public bool Lunic;
        public bool Prismatic;
        public int PrismaticCharge;
        public bool Brimflame;
        public int BrimflameCooldown;
        public bool Demonshade;
        public int DemonshadeLevel;
        public float DemonshadeXP;
        public bool FearOfTheValkyrie;
        public bool Crocket;
        public bool Gemtech;
        public int RangedGemTimer;
        public int MeleeGemTimer;
        public bool AnnihilEffects;
        //Soul of the Tyrant
        public bool AncestralCharm;

        //mostly booleans for active checks
        public bool aValkie;
        public bool aSword;
        public bool AeroValkyrie;
        public bool MarniteSwords;
        public bool DirtyPop;
        public bool NastyPop;
        public bool aScarey;
        public bool FearValkyrie;

        //mostly timers
        public int SDIcicleCooldown;
        public int ButterBeeCD;
        public int AtaxiaCooldown;
        public int AtaxiaCountdown;
        public int AtaxiaDR;
        public int UmbraBuffTimer;
        public int BloodBuffTimer;
        public int LifestealCD;
        public int kunaiKuldown;

        public Vector2 bubbleOffset;

        public override void ResetEffects()
        {
            //force of exploration
            RideOfTheValkyrie = false;
            Marnite = false;
            WulfrumOverpower = false;
            ProwlinOnTheFools = false;
            //force of devastation
            ReaverHage = false;
            ReaverHageBuff = false;
            ButterBeeSwarm = false;
            AyeCicle = false;
            AyeCicleSmol = false;
            AtaxiaEruption = false;
            if (AtaxiaCooldown > 0) AtaxiaCooldown--;
            if (AtaxiaCountdown > 0 && AtaxiaDR == 5) AtaxiaCountdown--;
            //force of desolation
            Empyrean = false;
            OmegaBlue = false;
            if (OmegaGreenCounter > 0) OmegaGreenCounter--;
            VictideSwimmin = false;
            Mollusk = false;
            SulphurBubble = false;
            FathomBubble = false;
            TitanHeart = false;
            Astral = false;
            if (TitanGuardCooldown > 0) TitanGuardCooldown--;
            DoctorBeeKill = false;
            //force of exaltation
            Tarragon = false;
            if (TarragonTimer > 0) TarragonTimer--;
            UmbraCrazyRegen = false;
            BFCrazierRegen = false;
            if (LifestealCD > 0) LifestealCD--;
            Silva = false;
            if (SilvaTimer > 0) SilvaTimer--;
            StatigelNinjaStyle = false;
            if (kunaiKuldown > 0) kunaiKuldown--;
            GodSlayerMeltdown = false;
            SlayerCD = false;
            Auric = false;
            //force of annihilation
            Brimflame = false;
            if (BrimflameCooldown > 0)
                BrimflameCooldown--;
            Demonshade = false;
            Lunic = false;
            Prismatic = false;
            FearOfTheValkyrie = false;
            Crocket = false;
            Gemtech = false;
            //Soul of the Tyrant
            AncestralCharm = false;
            //forces
            ExploEffects = false;
            DevastEffects = false;
            DesolEffects = false;
            ExaltEffects = false;
            AnnihilEffects = false;
            //minions active
            aValkie = false;
            aScarey = false;
            aSword = false;
            AeroValkyrie = false;
            FearValkyrie = false;
            MarniteSwords = false;

            if (Player.GetModPlayer<FargoSoulsPlayer>().WizardEnchantActive)
            {
                Player.GetModPlayer<FargoSoulsPlayer>().WizardEnchantActive = false;
                for (int i = 3; i <= 9; i++)
                {
                    if (!Player.armor[i].IsAir && (Player.armor[i].type == ModContent.ItemType<FargowiltasSouls.Content.Items.Accessories.Enchantments.WizardEnchant>() || Player.armor[i].type == ModContent.ItemType<FargowiltasSouls.Content.Items.Accessories.Forces.CosmoForce>()))
                    {
                        Player.GetModPlayer<FargoSoulsPlayer>().WizardEnchantActive = true;
                        ExploEffects = true;
                        DevastEffects = true;
                        DesolEffects = true;
                        ExaltEffects = true;
                        AnnihilEffects = true;
                        break;
                    }
                }
            }
        }
        /*private bool CalamityPreHurt()
        {
            if (SBDHandleDodges())
            {
                Player.GetModPlayer<CalamityPlayer>().justHitByDefenseDamage = false;
                Player.GetModPlayer<CalamityPlayer>().defenseDamageToTake = 0;
                return false;
            }
            return true;
        }
        private bool SBDHandleDodges()
        {
            if (Player.whoAmI != Main.myPlayer || Player.GetModPlayer<CalamityPlayer>().disableAllDodges) return false;
            if (SBDHandleDashDodges()) 
            {
                Main.NewText("HandleDodges works!");
                return true;
            }
            return false;
        }
        private bool SBDHandleDashDodges()
        {
            bool dashFlag = Player.pulley || (Player.grappling[0] == -1 && !Player.tongued);
            if (dashFlag && Player.GetModPlayer<CalamityPlayer>().DashID == Slayer_Dash.ID && GodSlayerMeltdown && Player.dashDelay < 0 && !SlayerCD) 
            {
                Main.NewText("HandleDashDodges works!");
                CounterDodge();
                return true;
            }
            return false;
        }
        private void CounterDodge()
        {
            Player.AddBuff(ModContent.BuffType<Slayer_Cooldown>(), 1800);
            Player.GiveIFrames(Player.longInvince ? 100 : 60, blink: true);
            for (int i = 0; i < 100; i++)
            {
                int dodgeDustType = Main.rand.Next(new int[3] { 180, 173, 244 });
                int num = Dust.NewDust(Player.position, Player.width, Player.height, dodgeDustType, 0f, 0f, 100, default, 2f);
                Dust dodgeDust = Main.dust[num];
                dodgeDust.position.X += Main.rand.Next(-20, 21);
                dodgeDust.position.Y += Main.rand.Next(-20, 21);
                dodgeDust.velocity *= 0.4f;
                dodgeDust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                dodgeDust.shader = GameShaders.Armor.GetSecondaryShader(Player.cWaist, Player);
                if (Main.rand.NextBool(2))
                {
                    dodgeDust.scale *= 1f + Main.rand.Next(40) * 0.01f;
                    dodgeDust.noGravity = true;
                }
            }
            NetMessage.SendData(MessageID.Dodge, -1, -1, null, Player.whoAmI, 1f);
        }*/

        public override void OnHurt(Player.HurtInfo info)
        {
            //Devastion
            if (ReaverHage)
            {
                ReaverHurtEffect();
            }
            if (AtaxiaEruption)
            {
                AtaxiaHurt();
            }
            //annihilation
            if (Demonshade)
            {
                DemonshadeHurtEffect(info.Damage);
            }
            
            //Desolation
            if (TitanHeart && Player.GetToggleValue("AstralShield"))
            {
                TitanHeartHurtEffects();
            }
            if (Astral && Player.GetToggleValue("AstralShield"))
            {
                AstralHurtEffects();
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            //Annihilation
            if (Prismatic)
            {
                modifiers.FinalDamage.Flat = PrismaticeHitEffects((int)modifiers.FinalDamage.Flat, npc);
            }
            
        }
        public override void UpdateDead()
        {
            //kill timers on death
            SDIcicleCooldown = 0;
            LifestealCD = 0;
            ButterBeeCD = 0;
            AtaxiaCooldown = 0;
            kunaiKuldown = 0;
            DemonshadeLevel = 0;
            DemonshadeXP = 0;
        }
        public override void PostUpdate()
        {
            //Desolation
            //titan heart is here because player.bodyframe is changed for shield visual which gets overridden if its in update equips because of update orders
            if (TitanHeart && Player.GetToggleValue("AstralShield"))
            {
                TitanHeartPostUpdate();
            }
            if (Astral && Player.GetToggleValue("AstralShield"))
            {
                AstralPostUpdate();
            }
        }
        
        public override void PostUpdateEquips()
        {
            //EXPLORATION

            //aerospec
            if (RideOfTheValkyrie && Player.GetToggleValue("Valkyrie"))
            {
                AerospecEffects();
            }

            //marnite
            //has 2 toggles so checks are in the method
            if (Marnite)
            {
                MarniteEffects();
            }

            //wulfrum
            if (WulfrumOverpower && Player.GetToggleValue("WulfrumBuff"))
            {
                WulfrumEffects();
            }

            //DEVASTATION

            //snow ruffian. based off of Soul of Cryogen's code
            if (AyeCicleSmol)
            {
                RuffianEffects();
            }

            //daedalus. based off of Soul of Cryogen's code
            if (AyeCicle)
            {
                DaedalusEffects();
            }

            //reaver
            if (ReaverHage)
            {
                ReaverEffects();
            }

            //plaguebringer
            if (ButterBeeSwarm)
            {
                PlaguebringerEffects();
            }

            if (AtaxiaEruption)
            {
                AtaxiaEffects();
            }



            //DESOLATION 

            //victide
            if (VictideSwimmin)
            {
                VictideEffects();
            }

            //mollusk
            if (Mollusk)
            {
                MolluskEffects();
            }
            if (Astral)
            {
                AstralEffects();
            }
            if (TitanHeart)
            {
                TitanHeartEffects();
            }
            //sulphurous. possibly one of the enchantments I'm most proud of
            if (SulphurBubble && !FathomBubble && Player.GetToggleValue("FathomBubble"))
            {
                SulphurEffects();
            }

            //fathom swarmer
            if (FathomBubble && Player.GetToggleValue("FathomBubble"))
            {
                FathomSwarmerEffects();
            }
            
            //EXALTATION 

            //silva
            if (Silva && Player.GetToggleValue("SilvaCrystal"))
            {
                SilvaEffects();
            }


            //ANNIHILATION 

            if (Gemtech && Player.GetToggleValue("ChargeAttacks")){
                GemTechEffects();
            }
            {
                
            }
            //brimflame
            if (Brimflame && Player.GetToggleValue("Enrage"))
            {
                BrimflameBuffActivate();
            }
            if (Demonshade && Player.GetToggleValue("Enrage"))
            {
                DemonshadeEffects();
            }
            //fearmonger
            if (FearOfTheValkyrie)
            {
                FearmongerEffects();
            }
            
        }
        
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
        {
            //Exploration
            //desert prowler enchantment
            if (ProwlinOnTheFools && Player.GetToggleValue("Tornadoes"))
            {
                ProwlerHitEffect();
            }
            //Devastation
            //bringer bees. based off of fargo souls' bee enchantment effect
            if (ButterBeeSwarm)
            {
                PlaguebringerHitEffect(item, target, damageDone);
            }
            //hydrothermic. just based
            if (AtaxiaEruption && Player.GetToggleValue("HydrothermicHits"))
            {
                HydrothermicHitEffect(target, damageDone);
            }
            //Desolation
            //Empyrean
            if (Empyrean && Player.GetToggleValue("AbyssalMadness"))
            {
                EmpyreanHitEffect();
            }
            //omega blue
            if (OmegaBlue && Player.GetToggleValue("AbyssalMadness"))
            {
                OmegaBlueHitEffects();
            }
            
            //Exaltation
            //umbra and blood timer calculus
            UmbraphileCalc(damageDone);
            BloodflareCalc(damageDone);

            //Umbraphile conditions
            if (UmbraCrazyRegen)
            {
                UmbraphileHitEffect(damageDone);
            }

            //Bloodflare conditions
            if (BFCrazierRegen && Player.GetToggleValue("BloodflareLifesteal"))
            {
                BloodflareHitEffect(target, damageDone);
            }

            //Statigel kunai
            if (StatigelNinjaStyle)
            {
                StatigelHitEffect(target, damageDone);
            }

            //God Slayer star
            if (GodSlayerMeltdown && Player.GetToggleValue("SlayerStars"))
            {
                GodSlayerHitEffect(target, damageDone);
            }
            //Annihilation
            if (Demonshade && Player.GetToggleValue("RageBuff"))
            {
                DemonshadeHitEffect(damageDone);
            }
            if (Lunic && Player.GetToggleValue("PrismaticRocket"))
            {
                LunicAttackEffects(damageDone);
            }
            if (Prismatic && Player.GetToggleValue("PrismaticRocket"))
            {
                PrismaticAttackEffects(damageDone);
            }
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
        {
            //Desolation
            //Plague Reaper conditions
            if (DoctorBeeKill && Player.GetToggleValue("Instakills"))
            {
                PlagueReaperHitEffect(target);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            //Exploration
            //desert prowler enchantment
            if (ProwlinOnTheFools && Player.GetToggleValue("Tornadoes"))
            {
                ProwlerProjHitEffect(proj);
            }
            //Devastation
            //bringer bees. you've read the part on top
            if (ButterBeeSwarm)
            {
                PlaguebringerProjHitEffect(proj, target, damageDone);
            }
            //hydroth   ermic
            if (AtaxiaEruption && Player.GetToggleValue("HydrothermicHits"))
            {
                HydrothermicProjHitEffect(target, damageDone);
            }
            //Desolation
            //Empyrean
            if (Empyrean && Player.GetToggleValue("AbyssalMadness"))
            {
                EmpyreanHitEffect();
            }
            //omega blue
            if (OmegaBlue && Player.GetToggleValue("AbyssalMadness"))
            {
                OmegaBlueHitEffects();
            }
            //Exaltation
            //umbra blood timer
            UmbraphileCalc(damageDone);
            BloodflareCalc(damageDone);

            //umbra
            if (UmbraCrazyRegen)
            {
                UmbraphileProjHitEffect(damageDone);

            }

            //bloodflare
            if (BFCrazierRegen)
            {
                BloodflareProjHitEffect(target, damageDone);
            }
            //statigel
            if (StatigelNinjaStyle)
            {
                StatigelProjHitEffect(proj, target, damageDone, hit.Crit);
            }

            //slayer
            if (GodSlayerMeltdown && Player.GetToggleValue("SlayerStars"))
            {
                GodSlayerProjHitEffect(proj, target, damageDone, hit.Crit);
            }
            //Annihilation
            if (Demonshade && Player.GetToggleValue("RageBuff"))
            {
                DemonshadeHitEffect(damageDone);
            }
            if (Lunic && Player.GetToggleValue("PrismaticRocket"))
            {
                LunicAttackEffects(damageDone);
            }
            if (Prismatic && Player.GetToggleValue("PrismaticRocket"))
            {
                PrismaticAttackEffects(damageDone);
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        {
            //Desolation
            //plague reaper
            if (DoctorBeeKill)
            {
                PlagueReaperProjHitEffect(target);
            }
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Desolation
            if (Empyrean && Player.GetToggleValue("AbyssTentacles"))
            {
                EmpyreanAttackEffects(source, damage, knockback);
            }
            if (OmegaBlue && Player.GetToggleValue("AbyssTentacles"))
            {
                OmegaBlueAttackEffects(source, damage, knockback);
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //Exaltation
            if (Silva && Player.GetToggleValue("SilvaCrystal"))
            {
                SilvaTrigger();
            }
            //Annihilation
            if (Lunic && Player.GetToggleValue("RageBuff"))
            {
                LunicTrigger();
            }
            if (Prismatic && Player.GetToggleValue("PrismaticRocket"))
            {
                PrismaticTrigger();
            }
        }
        public static List<int> DesolationForce = new List<int>()
        {
            ModContent.ItemType<EmpyreanEnchantment>(),
            ModContent.ItemType<OmegaBlueEnchantment>(),
            ModContent.ItemType<MolluskEnchantment>(),
            ModContent.ItemType<VictideEnchantment>(),
            ModContent.ItemType<SulphurEnchantment>(),
            ModContent.ItemType<FathomEnchantment>(),
            ModContent.ItemType<PlagueReaperEnchantment>(),
            ModContent.ItemType<TitanHeartEnchantment>(),
            ModContent.ItemType<AstralEnchantment>()
        };
        public static List<int> AnnihilationForce = new List<int>()
        {
            ModContent.ItemType<LunicCorpsEnchantment>(),
            ModContent.ItemType<PrismaticEnchantment>(),
            ModContent.ItemType<BrimflameEnchantment>(),
            ModContent.ItemType<DemonshadeEnchantment>(),
            ModContent.ItemType<GemtechEnchantment>(),
            ModContent.ItemType<FearmongerEnchantment>(),
        };
        public static List<int> ExplorationForce = new List<int>()
        {
            ModContent.ItemType<AerospecEnchantment>(),
            ModContent.ItemType<WulfrumEnchantment>(),
            ModContent.ItemType<MarniteEnchantment>(),
            ModContent.ItemType<ProwlerEnchantment>(),
        };
        public static List<int> DevastationForce = new List<int>()
        {
            ModContent.ItemType<BringerEnchantment>(),
            ModContent.ItemType<ReaverEnchantment>(),
            ModContent.ItemType<HydrothermicEnchantment>(),
            ModContent.ItemType<DaedalusEnchantment>(),
        };
        public static List<int> ExaltationForce = new List<int>()
        {
            ModContent.ItemType<TarragonEnchantment>(),
            ModContent.ItemType<BloodflareEnchantment>(),
            ModContent.ItemType<SilvaEnchantment>(),
            ModContent.ItemType<SlayerEnchantment>(),
            ModContent.ItemType<AuricEnchantment>(),
        };
        public bool ForceEffect(int ench)
        {
            FargoSoulsPlayer modplayer = Player.GetModPlayer<FargoSoulsPlayer>();
            if (modplayer.WizardEnchantActive && modplayer.WizardedItem != null && !modplayer.WizardedItem.IsAir && modplayer.WizardedItem.type == ench){
                return true;
            }
            else if (DesolEffects && DesolationForce.Contains(ench))
            {
                return true;
            }else if(AnnihilEffects && AnnihilationForce.Contains(ench))
            {
                return true;
            }
            else if (ExploEffects && ExplorationForce.Contains(ench))
            {
                return true;
            }
            else if (DevastEffects && DevastationForce.Contains(ench))
            {
                return true;
            }
            else if (ExaltEffects && ExaltationForce.Contains(ench))
            {
                return true;
            }
            return false;
        }
    }
    
}