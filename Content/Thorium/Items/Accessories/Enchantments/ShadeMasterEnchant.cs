using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameInput;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ShadeMasterEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Maroon;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.ShadeMasterEnch = true;
            DLCPlayer.ShadeMasterEnchItem = Item;

            if (DLCPlayer.ShadeMode)
            {
                player.GetDamage(DamageClass.Generic) += 0.2f;
                player.statDefense -= 50;
                player.endurance -= 0.1f;
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        void ShadeMasterEnter()
        {
            if (shadeMasterDuration >= 0 && shadeMasterDuration < 898)
            {
                ShadeMasterExit();
                return;
            }
            shadeMasterBodyCenter = Player.Center;
            shadeMasterDuration = 60 * 15;
            shadeMasterCurrentRadius = 0;

			if (shadeMasterDummy == null) shadeMasterDummy = new();
            shadeMasterDummy.head = Player.head;
            shadeMasterDummy.body = Player.body;
            shadeMasterDummy.legs = Player.legs;
            shadeMasterDummy.direction = Player.direction;
            shadeMasterDummy.Center = Player.Center;

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.RedMoss);
            }
        }

        internal Vector2 shadeMasterBodyCenter;
        private Player shadeMasterDummy = null; // is this alright? it seemed better than creating a player each frame.
        int shadeMasterDuration = -1;
        int shadeMasterCurrentRadius = 0;
        internal bool ShadeMode => shadeMasterDuration > 0;

        void ShadeMasterEffect()
        {
            if (!ShadeMasterEnch || shadeMasterDuration == -1)
            {
                if (shadeMasterDuration > 0)
                    ShadeMasterExit();
                return;
            }

            shadeMasterDuration--;
            if (shadeMasterDuration == 0)
            {
                ShadeMasterExit();
                return;
            }

            int x = 60 * 15 - shadeMasterDuration;
            if (x <= 30) shadeMasterCurrentRadius = (int)MathHelper.Lerp(0, 640, x / 30f);
            else if (x > 90) shadeMasterCurrentRadius = 640 - (int)(MathF.Pow((x - 90f) / (60f * 13.5f), 2) * 640f); //(int)MathHelper.Lerp(640, 0, (x - 90f) / (60f * 14f));
            else shadeMasterCurrentRadius = 640; // sanity case

            const float speed = 17f;
            FargowiltasSouls.FargoSoulsUtil.AuraDust(shadeMasterDummy, shadeMasterCurrentRadius, DustID.RedMoss);

            // Adapted from SoulsMod BaseArena.cs 
            float distance = shadeMasterBodyCenter.Distance(Player.Center);
            if (distance > shadeMasterCurrentRadius)
            {
                if (distance > shadeMasterCurrentRadius * 1.5f)
                {
                    ShadeMasterExit();
                    return;
                }

                Vector2 movement = shadeMasterBodyCenter - Player.Center;
                float difference = movement.Length() - shadeMasterCurrentRadius;
                movement.Normalize();
                movement *= MathF.Min(difference, speed);
                Player.position += movement;
            }
        }

        internal Rectangle GetShadeMasterHitBox()
        {
            return new((int)shadeMasterBodyCenter.X - Player.Hitbox.Width / 2, (int)shadeMasterBodyCenter.Y - Player.Hitbox.Height / 2, Player.Hitbox.Width, Player.Hitbox.Height);
        }

        void ShadeMasterExit()
        {
            shadeMasterDuration = -1;
            Player.Center = shadeMasterBodyCenter;

            for (int i = 0; i < 15; i++)
            {
                Dust.NewDust(Player.position, Player.width, Player.height, DustID.RedMoss);
            }
        }
    }
}

namespace FargowiltasCrossmod.Core.Globals
{
    // this is the stupidest code I have written but it works perfectly. Only case I can think of is another player being in the same position as the shade master ghost.
    [ExtendsFromMod("ThoriumMod")]
    public class ShadeMasterGlobalProj : GlobalProjectile
    {
        public override bool? Colliding(Projectile projectile, Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox == Main.LocalPlayer.Hitbox && Main.LocalPlayer.ThoriumDLC().ShadeMode)
            {
                Rectangle newBox = Main.LocalPlayer.ThoriumDLC().GetShadeMasterHitBox();
                return Collision.CheckAABBvAABBCollision(projHitbox.TopLeft(), projHitbox.Size(), newBox.TopLeft(), newBox.Size());
            }
            return base.Colliding(projectile, projHitbox, targetHitbox);
        }
    }

    [ExtendsFromMod("ThoriumMod")]
    public class ShadeMasterGlobalNPC : GlobalNPC
    {
        public override bool ModifyCollisionData(NPC npc, Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {
            if (victimHitbox == Main.LocalPlayer.Hitbox && Main.LocalPlayer.ThoriumDLC().ShadeMode)
            {
                var DLCPlayer = Main.LocalPlayer.ThoriumDLC();
                npcHitbox.X -= (int)(DLCPlayer.shadeMasterBodyCenter.X - DLCPlayer.Player.Center.X);
                npcHitbox.X -= (int)(DLCPlayer.shadeMasterBodyCenter.Y - DLCPlayer.Player.Center.Y);
            }
            return base.ModifyCollisionData(npc, victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }
    }
}