using Terraria.ModLoader;
using System;
using System.Reflection;
using Terraria;
using FargowiltasCrossmod.Core;
using ThoriumMod.NPCs;

namespace FargowiltasCrossmod.Content.Thorium
{
    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    [JITWhenModsEnabled(ModCompatibility.ThoriumMod.Name)]
    public class ThoriumLoadTasks : ModSystem
    {
        public override void PostSetupContent()
        {
            AddThoriumStatSheet();
            Core.Globals.DevianttGlobalNPC.AddThoriumShop();
        }

        public override void OnModLoad()
        {
            Assembly ThoriumAssembly = ModLoader.GetMod("ThoriumMod").GetType().Assembly;
            Type thoriumProjExtensions = ThoriumAssembly.GetType("ThoriumMod.Utilities.ProjectileHelper", true);

            Projectiles.DLCHealing.HealMethod = thoriumProjExtensions.GetMethod("ThoriumHeal", BindingFlags.Static | BindingFlags.NonPublic);
            Projectiles.DLCHealing.CustomHealingType = thoriumProjExtensions.GetNestedType("CustomHealing", BindingFlags.NonPublic);
            MonoModHooks.Modify(thoriumProjExtensions.GetMethod("ThoriumHealTarget", BindingFlags.Static | BindingFlags.NonPublic), Projectiles.DLCHealing.DLCOnHealEffects_ILEdit);

            if (FargowiltasCrossmod.CaughtTownies != null)
                RegisterThoriumCaughtNPCs();
        }

        public void RegisterThoriumCaughtNPCs()
        {
            void Add(string internalName, int id, string quote)
            {
                Fargowiltas.Items.CaughtNPCs.CaughtNPCItem item = new(internalName, id, quote);
                Mod.AddContent(item);
                FargowiltasCrossmod.CaughtTownies.Add(id, item.Type);
            }

            Add("Cobbler", ModContent.NPCType<Cobbler>(), "''");
            Add("DesertAcolyte", ModContent.NPCType<DesertAcolyte>(), "''");
            Add("Cook", ModContent.NPCType<Cook>(), "'I am the danger'");
            Add("ConfusedZombie", ModContent.NPCType<ConfusedZombie>(), "'Guh?!'");
            Add("Blacksmith", ModContent.NPCType<Blacksmith>(), "''");
            Add("Tracker", ModContent.NPCType<Tracker>(), "''");
            Add("Diverman", ModContent.NPCType<Diverman>(), "'Sam?'");
            Add("Druid", ModContent.NPCType<Druid>(), "''");
            Add("Spiritualist", ModContent.NPCType<Spiritualist>(), "''");
            Add("WeaponMaster", ModContent.NPCType<WeaponMaster>(), "''");
        }

        public static void AddThoriumStatSheet()
        {
            double Damage(DamageClass damageClass) => Math.Round(Main.LocalPlayer.GetTotalDamage(damageClass).Additive * Main.LocalPlayer.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
            int Crit(DamageClass damageClass) => (int)Main.LocalPlayer.GetTotalCritChance(damageClass);

            void Add<T>(Func<string> func) where T : ModItem => ModCompatibility.MutantMod.Mod.Call("AddStat", ModContent.ItemType<T>(), func);

            Add<ThoriumMod.Items.HealerItems.WoodenBaton>(() => $"Radiant Damage: {Damage(ModContent.GetInstance<ThoriumMod.HealerDamage>())}%");
            Add<ThoriumMod.Items.HealerItems.WoodenBaton>(() => $"Radiant Critical: {Crit(ModContent.GetInstance<ThoriumMod.HealerDamage>())}%");

            Add<ThoriumMod.Items.BardItems.WoodenWhistle>(() => $"Synphonic Damage: {Damage(ModContent.GetInstance<ThoriumMod.BardDamage>())}%");
            Add<ThoriumMod.Items.BardItems.WoodenWhistle>(() => $"Synphonic Critical: {Crit(ModContent.GetInstance<ThoriumMod.BardDamage>())}%");

            Add<ThoriumMod.Items.ThrownItems.StoneThrowingSpear>(() => $"Throwing Damage: {Damage(DamageClass.Throwing)}%");
            Add<ThoriumMod.Items.ThrownItems.StoneThrowingSpear>(() => $"Throwing Critical: {Crit(DamageClass.Throwing)}%");

            Add<ThoriumMod.Items.BossMini.TheGoodBook>(() => $"Bonus Healing: {Main.LocalPlayer.Thorium().healBonus}");
            Add<ThoriumMod.Items.HealerItems.HoneyHeart>(() => $"Life recovery: {Main.LocalPlayer.Thorium().lifeRecovery}/ sec");

            Add<ThoriumMod.Items.BardItems.InspirationFragment>(() => $"Inspiration: {Main.LocalPlayer.Thorium().bardResourceMax2}");
            Add<ThoriumMod.Items.BardItems.InspirationNote>(() => $"Inspiration Regen: {Main.LocalPlayer.Thorium().inspirationRegenBonus}/ sec");
        }
    }
}