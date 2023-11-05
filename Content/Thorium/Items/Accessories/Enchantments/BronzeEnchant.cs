using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    public class BronzeEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.Gold;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.BronzeEnch && DLCPlayer.GraniteEnch;

        protected override Color SynergyColor1 => Color.Gold with { A = 0 };
        protected override Color SynergyColor2 => Color.DarkBlue with { A = 0 };
        internal override int SynergyEnch => ModContent.ItemType<GraniteEnchant>();

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.BronzeEnch = true;
            DLCPlayer.BronzeEnchItem = Item;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<FargowiltasSouls.Content.Items.Accessories.Enchantments.CopperEnchant>()
                .AddIngredient<FargowiltasSouls.Content.Items.Accessories.Enchantments.TinEnchant>()
                .AddTile(TileID.Hellforge)
                .Register()
                ;
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        internal int bronzeSynergyCD;
        public void BronzeEffect(Item item, Vector2 pos, int damage)
        {
            if (SynergyEffect(BronzeEnchItem.type))
            {
                if (bronzeSynergyCD >= 300)
                {
                    bronzeSynergyCD = 0;
                    Projectile.NewProjectile(Player.GetSource_Accessory(BronzeEnchItem), pos, Vector2.Normalize(Main.MouseWorld - pos) * 8f, ModContent.ProjectileType<Projectiles.DLCLightStrike>(), damage / 2, 0.5f, Player.whoAmI, 3f, 1f);
                }
            }
            else
            {
                if (Main.rand.NextBool(5))
                {
                    Projectile.NewProjectile(Player.GetSource_Accessory(BronzeEnchItem), pos, Vector2.Normalize(Main.MouseWorld - pos) * 8f, ModContent.ProjectileType<Projectiles.DLCLightStrike>(), damage / 2, 0.5f, Player.whoAmI, 0f);
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DLCLightStrike : ModProjectile
    {
        public override string Texture => "ThoriumMod/Projectiles/LightStrike";
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] > 0f)
            {
                NPC nextTarget = FindNearestOtherTarget(target);

                Vector2 direction = nextTarget == null ? Main.rand.NextVector2CircularEdge(1, 1) : Projectile.Center.DirectionTo(nextTarget.Center);
                direction *= Projectile.velocity.Length();

                Projectile.ai[0]--;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, Projectile.type, (int)(Projectile.damage * 0.8f), Projectile.knockBack * 0.8f, Projectile.owner, Projectile.ai[0], Projectile.ai[1]);
            }

            if (Projectile.ai[1] == 1f)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<GraniteExplosion>(), Projectile.damage * 2, 0f, Projectile.owner, 0.5f);
            }

            //npc.immune[base.Projectile.owner] = 2;
        }

        NPC FindNearestOtherTarget(NPC ignore)
        {
            float bestDist = 640;
            float bestWithSurgeDist = 640;
            NPC best = null;
            NPC bestWithSurge = null;

            int surgeType = ModContent.BuffType<ThoriumMod.Buffs.GraniteSurge>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (i == ignore.whoAmI || !Main.npc[i].active) continue;

                float newDist = Projectile.Center.Distance(Main.npc[i].Center);

                if (Main.npc[i].HasBuff(surgeType) && newDist < bestWithSurgeDist)
                {
                    bestWithSurge = Main.npc[i];
                    bestWithSurgeDist = newDist;
                }
                else
                {
                    if (newDist < bestDist)
                    {
                        best = Main.npc[i];
                        bestDist = newDist;
                    }
                }
            }

            if (best == null) return bestWithSurge;

            return best;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 3;
            Projectile.aiStyle = Terraria.ID.ProjAIStyleID.Arrow;
            Projectile.timeLeft = 300;
            //Projectile.extraUpdates = 6;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 30;
            AIType = 14;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color?(Color.White);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(base.Projectile.position, base.Projectile.width, base.Projectile.height, 159, (float)Main.rand.Next(-6, 6), (float)Main.rand.Next(-6, 6), 125, default(Color), 1f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void PostAI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
        }
    }
}