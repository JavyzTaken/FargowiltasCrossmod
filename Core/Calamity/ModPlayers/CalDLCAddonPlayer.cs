using CalamityMod.Particles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasSouls.Content.Patreon.Potato;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using Luminance.Common.Utilities;
using CalamityMod.Projectiles.Ranged;
using CalamityMod;
using CalamityMod.Events;
using Terraria.GameInput;
using System.Reflection;
using CalamityMod.UI.Rippers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using CalamityMod.Projectiles.Rogue;

namespace FargowiltasCrossmod.Core.Calamity.ModPlayers
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalDLCAddonPlayer : ModPlayer
    {
        public float AeroCritBoost;
        public int FeatherJumpsRemaining;
        public float ExploFeatherCount;
        public int usedWeaponTimer;
        public float ProwlerCharge;
        public bool AutoProwler = false;
        public int PlagueCharge;
        public int DaedalusTimer;
        public bool ReaverToggle;
        public float HydrothermicHeat;
        public float HydrothermicFlareCooldown;
        public bool HydrothermicOverheat;
        public int ElementsAirTime;
        public int NumJumpsUsed = 0;
        public bool AllowJumpsUsedInc = false;
        public bool RuffianModifiedRotation = false;
        public float BrimflameDefenseTimer = 0;
        public float BrimflameShootingTimer = 0;
        public int MaxDefense = 0;
        public int ClamSlamTime = 0;
        public float ClamSlamHorizontalSpeed = 0;
        public int ClamSlamCooldown = 0;
        public int ClamSlamIframes = 0;
        public int BatTime = 0;
        public int BatCooldown = 0;
        public int BatStartupTimer = 0;
        public int BatHitCD = 0;
        public int MarniteTimer;
        public bool TitanHeartAdrenaline;
        public bool EmpyreanRage;
        public int EmpyreanSlowTimer = 0;
        public bool EmpyreanEmpowered = true;
        public int EmpyreanCooldown = 0;
        public int EmpyreanCooldownMax = 0;

        public override void ResetEffects()
        {
            if (BrimflameDefenseTimer > 0)
                BrimflameDefenseTimer--;
            if (BrimflameShootingTimer > 0)
                BrimflameShootingTimer--;
            if (ClamSlamCooldown > 0)
                ClamSlamCooldown--;
            if (ClamSlamIframes > 0)
                ClamSlamIframes--;
            if (BatTime > 0)
                BatTime--;
            if (BatCooldown > 0 && BatTime == 0)
                BatCooldown--;
            if (BatHitCD > 0)
                BatHitCD--;
            if (BatStartupTimer > 0)
            {
                BatStartupTimer--;
            }
            if (EmpyreanCooldown > 0)
                EmpyreanCooldown--;
            base.ResetEffects();
        }
        public override bool IsLoadingEnabled(Mod mod)
        {
            //return FargowiltasCrossmod.EnchantLoadingEnabled;
            return true;
        }
        public override void PostUpdateEquips()
        {
            if (Player.HasEffect<DesertProwlerEffect>())
            {
                AutoProwler = Player.autoJump;
                Player.autoJump = false;

            }
            bool ruffianFlight = Player.jump == 0 && Player.wingTime <= 0 && Player.controlJump && Player.velocity.X != 0;
            if ((!Player.HasEffect<SnowRuffianEffect>() || !ruffianFlight) && RuffianModifiedRotation)
            {
                Player.fullRotation = 0;
                RuffianModifiedRotation = false;
            }
            if (BatTime > 0 && BatStartupTimer == 0)
            {
                Player.AddImmuneTime(ImmunityCooldownID.Bosses, 2);
                Player.immuneNoBlink = true;
                Player.immuneTime = 5;
                Player.immune = true;
            }

            if (Player.HasEffect<TitanHeartEffect>())
            {
                if (!TitanHeartAdrenaline)
                {
                    TitanHeartAdrenaline = true;
                    typeof(RipperUI).GetField("adrenBarTex", LumUtils.UniversalBindingFlags).SetValue(null, ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/ExtraTextures/TitanHeartAdrenalineBar", AssetRequestMode.ImmediateLoad).Value);
                }
            }
            else
            {
                if (TitanHeartAdrenaline)
                {
                    TitanHeartAdrenaline = false;
                    typeof(RipperUI).GetField("adrenBarTex", LumUtils.UniversalBindingFlags).SetValue(null, ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/AdrenalineBar", AssetRequestMode.ImmediateLoad).Value);
                }
            }
            if (Player.HasEffect<EmpyreanEffect>())
            {
                if (!EmpyreanRage)
                {
                    EmpyreanRage = true;
                    typeof(RipperUI).GetField("rageBarTex", LumUtils.UniversalBindingFlags).SetValue(null, ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/ExtraTextures/EmpyreanRageBar", AssetRequestMode.ImmediateLoad).Value);
                }
            }
            else
            {
                if (EmpyreanRage)
                {
                    EmpyreanRage = false;
                    typeof(RipperUI).GetField("rageBarTex", LumUtils.UniversalBindingFlags).SetValue(null, ModContent.Request<Texture2D>("CalamityMod/UI/Rippers/RageBar", AssetRequestMode.ImmediateLoad).Value);
                }
            }
        }
        public override void PostUpdateMiscEffects()
        {
            if (!Player.HasEffect<DaedalusEffect>() && DaedalusTimer > 0)
                DaedalusTimer--;
        }
        public override void PostUpdateRunSpeeds()
        {
            if (RuffianModifiedRotation)
            {
                Player.runAcceleration = 0.1264f;
                Player.runSlowdown = 0.2f;
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            /*
            FieldInfo installKey = typeof(FargowiltasSouls.FargowiltasSouls).GetField("DebuffInstallKey", BindingFlags.NonPublic | BindingFlags.Static);
            FieldInfo sDashKey = typeof(FargowiltasSouls.FargowiltasSouls).GetField("SpecialDashKey", BindingFlags.NonPublic | BindingFlags.Static);
            if (installKey != null && installKey.GetValue(installKey) != null) {
                ModKeybind value = (ModKeybind)installKey.GetValue(installKey);
                if (value.JustPressed)
                {
                    if (Player.HasEffect<BrimflameEffect>())
                    {
                        BrimflameEffect.BrimflameTrigger(Player);
                    }
                }   
            }
            if (sDashKey != null && sDashKey.GetValue(sDashKey) != null)
            {
                ModKeybind value = (ModKeybind)sDashKey.GetValue(sDashKey);
                if (value.JustPressed)
                {
                    if (Player.HasEffect<MolluskEffect>())
                    {
                        MolluskEffect.MolluskTrigger(Player);
                    }
                }
            }
            */
            base.ProcessTriggers(triggersSet);
        }
        public override void PreUpdate()
        {
            if (Player.HasEffect<DesertProwlerEffect>())
            {
                DesertProwlerEffect.ProwlerEffect(Player);
            }
        }
        public override void MeleeEffects(Item item, Rectangle hitbox)
        {
            if (Player.HasEffect<SulphurEffect>())
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile projectile = Main.projectile[i];

                    if (projectile.TypeAlive<SulphurBubble>() && hitbox.Intersects(projectile.Hitbox))
                    {
                        if (projectile.ai[1] <= 0 && Main.myPlayer == projectile.owner)
                            projectile.As<SulphurBubble>().OnHitEffect(item.damage);
                    }
                }

            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (EmpyreanEmpowered)
            {
                modifiers.ScalingBonusDamage += 1f;
                if (Player.ForceEffect<EmpyreanEffect>())
                {
                    modifiers.ScalingBonusDamage += 1f;
                }
            }
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            EmpyreanEffect.EmpyreanProjectileEffect(Player, item.GetSource_OnHit(target), target.Center, damageDone);
            base.OnHitNPCWithItem(item, target, hit, damageDone);
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            EmpyreanEffect.EmpyreanProjectileEffect(Player, proj.GetSource_OnHit(target), proj.Center, damageDone);
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EmpyreanEffect.EmpyreanHitEffect(Player);
        }
        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //titan heart
            //Vector2 vector = drawInfo.Position + drawInfo.drawPlayer.Size * new Vector2(0.5f, 1f) - Main.screenPosition;
            //for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
            //{
            //    DrawData value = drawInfo.DrawDataCache[i];
            //    Vector2 vector2 = value.position - vector;
            //    value.position = vector + vector2 * 2;
            //    value.scale *= 2;
            //    drawInfo.DrawDataCache[i] = value;
            //}
            // drawInfo.Position += new Vector2(20, 20);
        }
    }
}
