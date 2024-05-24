using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasCrossmod.Core.Calamity
{
    public class CalDLCSets : ModSystem
    {
        public class Items
        {

        }
        public class NPCs
        {

        }
        public class Projectiles
        {
            public static bool[] ProfanedCrystalProj;
            public static bool[] EternityBookProj;
            public static bool[] AngelAllianceProj;
        }
        public override void PostSetupContent()
        {
            #region Items
            SetFactory itemFactory = new(ItemLoader.ItemCount);
            #endregion
            #region Projectiles
            SetFactory projectileFactory = new(ProjectileLoader.ProjectileCount);
            Projectiles.ProfanedCrystalProj = projectileFactory.CreateBoolSet(false,
                ProjectileType<ProfanedCrystalMageFireball>(),
                ProjectileType<ProfanedCrystalMageFireballSplit>(),
                ProjectileType<ProfanedCrystalMeleeSpear>(),
                ProjectileType<ProfanedCrystalRangedHuges>(),
                ProjectileType<ProfanedCrystalRangedSmalls>(),
                ProjectileType<ProfanedCrystalRogueShard>(),
                ProjectileType<ProfanedCrystalWhip>(),
                ProjectileType<MiniGuardianAttack>(),
                ProjectileType<MiniGuardianDefense>(),
                ProjectileType<MiniGuardianFireball>(),
                ProjectileType<MiniGuardianFireballSplit>(),
                ProjectileType<MiniGuardianHealer>(),
                ProjectileType<MiniGuardianHolyRay>(),
                ProjectileType<MiniGuardianRock>(),
                ProjectileType<MiniGuardianSpear>(),
                ProjectileType<MiniGuardianStars>()
                );
            Projectiles.EternityBookProj = projectileFactory.CreateBoolSet(false,
                ProjectileType<EternityCircle>(),
                ProjectileType<EternityCrystal>(),
                ProjectileType<EternityHex>(),
                ProjectileType<EternityHoming>()
                );
            Projectiles.AngelAllianceProj = projectileFactory.CreateBoolSet(false,
                ProjectileType<AngelBolt>(),
                ProjectileType<AngelicAllianceArchangel>(),
                ProjectileType<AngelOrb>(),
                ProjectileType<AngelRay>()
                );

            #endregion
        }
    }
}
