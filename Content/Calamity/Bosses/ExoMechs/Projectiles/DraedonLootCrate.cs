using CalamityMod.Items.TreasureBags;
using FargowiltasCrossmod.Core;
using Luminance.Common.DataStructures;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Projectiles
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DraedonLootCrate : ModProjectile, IProjOwnedByBoss<CalamityMod.NPCs.ExoMechs.Draedon>
    {
        /// <summary>
        /// The player that shall receive the spoils of this loot crate.
        /// </summary>
        public Player Recipient => Main.player[Projectile.owner];

        /// <summary>
        /// How long this loot crate has existed, in frames.
        /// </summary>
        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 54;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 3600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            Time++;

            Projectile.Opacity = Utilities.InverseLerp(0f, 45f, Time);
            Projectile.Center = new Vector2(Recipient.Center.X, Projectile.Center.Y);
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 24f, 0.006f);

            if (Projectile.WithinRange(Recipient.Center, 25f))
            {
                if (Main.myPlayer == Projectile.owner)
                    Item.NewItem(Projectile.GetSource_FromThis(), Recipient.Hitbox, ModContent.ItemType<DraedonBag>(), noGrabDelay: true);

                Projectile.Kill();
            }
        }
    }
}
