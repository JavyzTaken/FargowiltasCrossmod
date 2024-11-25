using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using ThoriumMod.Empowerments;

namespace FargowiltasCrossmod.Core.Thorium.Modplayers
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ThoriumPlayerBalance : ModPlayer
    {
        private int MaxEmpowerments => Main.hardMode ? (FargowiltasSouls.Core.Systems.WorldSavingSystem.DownedAbom ? 12 : 8) : 4;
        public override void PostUpdate()
        {
            if (FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
                UnstackEmpowerments();
        }

        private void UnstackEmpowerments()
        {
            var thoriumPlayer = Player.Thorium();

            var field = typeof(ThoriumMod.ThoriumPlayer).GetField("Empowerments", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null) return;
            
            var value = field.GetValue(thoriumPlayer);
            if (value is not EmpowermentData data) return;

            if (data.ActiveEmpowerments.Count <= MaxEmpowerments) return;

            data.Timers[data.ActiveEmpowerments[0]].timer = 0;
            // data.ActiveEmpowerments.RemoveAt(0);

            // data.Timers.OrderBy(timer => timer.Value.timer).First().Value.timer = 0;
            // var empToRemove = data.Timers.OrderBy(timer => timer.Value.timer).First();
            // empToRemove.Value.timer = 0;
            // data.ActiveEmpowerments.Remove(empToRemove.Value.Type);
            // data.Timers.Remove(empToRemove.Key);
            // data.Timers[empToRemove.Key].level = 0;
            // data.Timers[empToRemove.Key].Update();
            // field.SetValue(thoriumPlayer, data);
        }
    }
}
