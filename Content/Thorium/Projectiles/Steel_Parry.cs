using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class Steel_Parry : ModProjectile
    {
        public bool IsDarkSteel = false;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.damage = 0;
            Projectile.timeLeft = 30;
            Projectile.width = 58;
            Projectile.height = 42;
        }

        public override void OnSpawn(IEntitySource source)
        {
            IsDarkSteel = Projectile.ai[0] != 0f;
            Projectile.rotation = Projectile.ai[1];
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
            Projectile.velocity = Projectile.oldVelocity;

            int maxDist = IsDarkSteel ? 2304 : 4096; // 3 block : 4 blocks (squared)
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (i == Projectile.whoAmI) continue;
                Projectile proj = Main.projectile[i];
                if (proj.friendly && !IsDarkSteel) continue;

                if (player.Center.DistanceSQ(proj.Center) < maxDist && proj.TryGetGlobalProjectile(out ParriedProjectile parried) && !parried.alreadyParried)
                {
                    if (proj.friendly)
                    {
                        if (IsDarkSteel)
                        {
                            CombatText.NewText(new((int)(player.position.X - 16), (int)(player.position.Y - 48), player.width + 32, 32), Color.Orange, "+ProBoost");
                            proj.velocity *= 2;
                            proj.damage = (int)(proj.damage * 2f);
                        }
                    }
                    else
                    {
                        CombatText.NewText(new((int)(player.position.X - 16), (int)(player.position.Y - 48), player.width + 32, 32), Color.Orange, "+Parried", true);
                        proj.velocity *= -2;
                        proj.damage *= IsDarkSteel ? 8 : 5;
                    }

                    parried.alreadyParried = true;
                    parried.explodeOnDeath = player.GetModPlayer<FargowiltasSouls.Core.ModPlayers.FargoSoulsPlayer>().WizardEnchantActive;
                    proj.friendly = true;
                    proj.hostile = false;

                    // if the parry was succesful, the player gets a bonus to their cooldown.
                    int buffType = ModContent.BuffType<Buffs.SteelParry_CD>();
                    if (player.HasBuff(buffType) && player.buffTime[player.FindBuffIndex(buffType)] > 300) player.buffTime[player.FindBuffIndex(buffType)] -= 60;
                }
            }
        }
    }
    [ExtendsFromMod("ThoriumMod")]
    public class ParriedProjectile : GlobalProjectile
    {
        public bool alreadyParried;
        public bool explodeOnDeath;
        public override bool InstancePerEntity => true;

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (timeLeft != 0 && explodeOnDeath)
            {
                Projectile.NewProjectileDirect(projectile.GetSource_Death(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<MidExplosion>(), (int)(projectile.damage * 0.75f), 5f, projectile.owner);
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (alreadyParried) modifiers.SetCrit();
        }
    }

    public class MidExplosion : FargowiltasSouls.Content.Projectiles.Masomode.MoonLordSunBlast
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {
            base.AI();
            Projectile.scale = 1f; // base Ai changes scale which i didn't want.
        }
    }
}
