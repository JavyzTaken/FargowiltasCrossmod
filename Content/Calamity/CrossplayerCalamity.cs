using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FargowiltasSouls;
using CalamityMod.World;
using Terraria.GameInput;
using Terraria.DataStructures;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.ModPlayers;


namespace FargowiltasCrossmod.Content.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public partial class CrossplayerCalamity : ModPlayer
    {
        //effect booleans
        public bool RideOfTheValkyrie;
        public bool Marnite;
        public bool WulfrumOverpower;
        public bool ProwlinOnTheFools;
        public bool ExploEffects;

        public bool ReaverHage;
        public bool ReaverHageBuff;
        public bool ButterBeeSwarm;
        public bool AyeCicle;
        public bool AyeCicleSmol;
        public bool AtaxiaEruption;
        public bool DevastEffects;

        public bool Empyrean;
        public bool OmegaBlue;
        public int OmegaGreenCounter;
        public bool VictideSwimmin;
        public bool Mollusk;
        public bool SulphurBubble;
        public bool FathomBubble;
        public bool DoctorBeeKill;
        public bool DesolEffects;

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
            RideOfTheValkyrie = false;
            Marnite = false;
            WulfrumOverpower = false;
            ProwlinOnTheFools = false;

            ReaverHage = false;
            ReaverHageBuff = false;
            ButterBeeSwarm = false;
            AyeCicle = false;
            AyeCicleSmol = false;
            AtaxiaEruption = false;
            if (AtaxiaCooldown > 0) AtaxiaCooldown--;
            if (AtaxiaCountdown > 0) AtaxiaCountdown--;

            Empyrean = false;
            OmegaBlue = false;
            if (OmegaGreenCounter > 0) OmegaGreenCounter--;
            VictideSwimmin = false;
            Mollusk = false;
            SulphurBubble = false;
            FathomBubble = false;
            DoctorBeeKill = false;

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

            Brimflame = false;
            if (BrimflameCooldown > 0)
                BrimflameCooldown--;
            Demonshade = false;
            Prismatic = false;
            FearOfTheValkyrie = false;
            Crocket = false;
            if (!Gemtech)
            {
                GemTechTimer = 0;
                HeldItem = null;
            }
            Gemtech = false;
            

            ExploEffects = false;
            DevastEffects = false;
            DesolEffects = false;
            ExaltEffects = false;
            AnnihilEffects = false;

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
            if (ReaverHage)
            {
                ReaverHurtEffect();
            }
            if (Demonshade)
            {
                DemonshadeHurtEffect(info.Damage);
            }
            if (AtaxiaEruption)
            {
                AtaxiaHurt();
            }
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (Prismatic)
            {
                modifiers.FinalDamage.Flat = PrismaticeHitEffects((int)modifiers.FinalDamage.Flat, npc);
            }
        }
        public override void UpdateDead()
        {
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
            
        }
        public override void PostUpdateEquips()
        {
            //EXPLORATION (3/4)

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

            //DEVASTATION (4/4)

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



            //DESOLATION (2/5)

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

            //sulphurous. possibly one of the enchantments I'm most proud of
            if (SulphurBubble && !FathomBubble)
            {
                SulphurEffects();
            }

            //fathom swarmer
            if (FathomBubble)
            {
                FathomSwarmerEffects();
            }

            //EXALTATION (1/5)

            //silva
            if (Silva && Player.GetToggleValue("SilvaCrystal"))
            {
                SilvaEffects();
            }


            //ANNIHILATION (1/4)

            //brimflame
            if (Brimflame)
            {
                BrimflameBuffActivate();
            }
            if (Demonshade)
            {
                DemonshadeEffects();
            }
            //fearmonger
            if (FearOfTheValkyrie)
            {
                FearmongerEffects();
            }
            if (Gemtech)
            {
                GemTechEffects();
            }
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Item, consider using OnHitNPC instead */
        {
            //desert prowler enchantment
            if (ProwlinOnTheFools && Player.GetToggleValue("Tornadoes"))
            {
                ProwlerHitEffect();
            }
            //bringer bees. based off of fargo souls' bee enchantment effect
            if (ButterBeeSwarm)
            {
                PlaguebringerHitEffect(item, target, damageDone);
            }
            //Empyrean
            if (Empyrean)
            {
                EmpyreanHitEffect();
            }
            //omega blue
            if (OmegaBlue)
            {
                OmegaBlueHitEffects();
            }
            //hydrothermic. just based
            if (AtaxiaEruption && Player.GetToggleValue("HydrothermicHits"))
            {
                HydrothermicHitEffect(target, damageDone);
            }

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
            if (Demonshade)
            {
                DemonshadeHitEffect(damageDone);
            }
            if (Prismatic)
            {
                PrismaticAttackEffects(damageDone);
            }
        }
        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
        {
            //Plague Reaper conditions
            if (DoctorBeeKill)
            {
                PlagueReaperHitEffect(target);
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)/* tModPorter If you don't need the Projectile, consider using OnHitNPC instead */
        {
            //desert prowler enchantment
            if (ProwlinOnTheFools && Player.GetToggleValue("Tornadoes"))
            {
                ProwlerProjHitEffect(proj);
            }

            //bringer bees. you've read the part on top
            if (ButterBeeSwarm)
            {
                PlaguebringerProjHitEffect(proj, target, damageDone);
            }
            //Empyrean
            if (Empyrean)
            {
                EmpyreanHitEffect();
            }
            //omega blue
            if (OmegaBlue)
            {
                OmegaBlueHitEffects();
            }
            //hydroth   ermic
            if (AtaxiaEruption && Player.GetToggleValue("HydrothermicHits"))
            {
                HydrothermicProjHitEffect(target, damageDone);
            }
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
            if (Demonshade)
            {
                DemonshadeHitEffect(damageDone);
            }
            if (Prismatic)
            {
                PrismaticAttackEffects(damageDone);
            }
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Projectile, consider using ModifyHitNPC instead */
        {
            //plague reaper
            if (DoctorBeeKill)
            {
                PlagueReaperProjHitEffect(target);
            }
        }
        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Empyrean)
            {
                EmpyreanAttackEffects(source, damage, knockback);
            }
            if (OmegaBlue)
            {
                OmegaBlueAttackEffects(source, damage, knockback);
            }
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (Silva && Player.GetToggleValue("SilvaCrystal"))
            {
                SilvaTrigger();
            }
            if (Prismatic)
            {
                PrismaticTrigger();
            }
        }
        
    }
    
}