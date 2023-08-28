using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

// god this is such a mess
namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    
    public struct DragonData
    {
        public int parent = -1, child = -1, head = -1, position = -1;
        public bool secondDragon = false;
        public static DragonData FromProjIndex(int i)
        {
            Projectile proj = Main.projectile[i];
            if (DragonMinion.DragonTypes.Contains(proj.type) && proj.ModProjectile is DragonMinion dragonPiece)
            {
                return dragonPiece.data;
            }
            else return new();
        }
        public DragonData Next(int identity) => new(head, position + 1, identity, -1);
        public DragonData(int h, int pos, int p = -1, int c = -1)
        {
            head = h;
            position = pos;
            parent = p;
            child = c;
        }
    }
    [ExtendsFromMod("ThoriumMod")]
    public abstract class DragonMinion : ModProjectile
    {
        public static int HeadType => ModContent.ProjectileType<DragonMinionHead>();
        public static int BodyType1 => ModContent.ProjectileType<DragonMinionBody>();
        public static int BodyType2 => ModContent.ProjectileType<DragonMinionBody2>();
        public static int TailType => ModContent.ProjectileType<DragonMinionTail>();
        public static List<int> DragonTypes => new() { HeadType, BodyType1, BodyType2, TailType };

        public DragonData data;

        public const float minimumBonus = 1.1f;

        protected DragonMinionHead _head;
        public DragonMinionHead Head
        {
            get
            {
                if (_head == null)
                {
                    _head = Main.projectile[data.head].ModProjectile as DragonMinionHead;
                }
                return _head;
            }
        }

        public void CommonAI()
        {
            Player player = Main.player[Projectile.owner];
            var modPlayer = player.GetModPlayer<CrossplayerThorium>();
            if ((int)Main.timeForVisualEffects % 120 == 0)
            {
                Projectile.netUpdate = true;
            }

            // kill if player dies or doesnt have the enchant on
            if (modPlayer.DragonEnch && player.active && !player.dead)
            {
                Projectile.timeLeft = 2;
            }
            else return;

            // spawn dust particles
            if (Main.rand.NextBool(30))
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 0, default, 2f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].fadeIn = 2f;
                Point point = Main.dust[dustIndex].position.ToTileCoordinates();
                if (WorldGen.InWorld(point.X, point.Y, 5) && WorldGen.SolidTile(point.X, point.Y))
                {
                    Main.dust[dustIndex].noLight = true;
                }
            }
        }

        public void BodyTailAI()
        {
            bool flag2 = false;
            Vector2 parentPos = Vector2.Zero;
            float parentRotation = 0f;
            float scaleFactor2 = 0f;
            float scaleFactor3 = 1f;
            int num17 = 30; // some kind of width scaling factor

            // ?
            if (Projectile.ai[1] == 1f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            // not sure if byUUID is nessicary or what it does
            int byUUID = Projectile.GetByUUID(Projectile.owner, data.parent);
            if (Main.projectile.IndexInRange(byUUID))
            {
                Projectile parent = Main.projectile[byUUID];
                if (parent.active && (parent.type == HeadType || parent.type == BodyType1 || parent.type == BodyType2))
                {
                    flag2 = true;
                    parentPos = parent.Center;
                    parentRotation = parent.rotation;
                    scaleFactor3 = MathHelper.Clamp(parent.scale, 0f, 50f);
                    scaleFactor2 = 16f;
                    parent.localAI[0] = Projectile.localAI[0] + 1f; // ??

                    if (parent.type != HeadType)
                    {
                        DragonData parentData = DragonData.FromProjIndex(parent.whoAmI);
                        // TODO: test is this references data correctly
                        parentData.child = Projectile.whoAmI; // parent's child should be us
                    }

                    // kill if there's only a head and tail
                    if (Projectile.owner == Main.myPlayer && Projectile.type == TailType && parent.type == HeadType)
                    {
                        Main.NewText("TailHead test");
                        parent.Kill();
                        Projectile.Kill();
                        return;
                    }
                }
            }
            if (!flag2)
            {
                // find the projectile this is meant to be following, in case of detachments for whatever reason
                for (int k = 0; k < 1000; k++)
                {
                    Projectile potentialParent;
                    potentialParent = Main.projectile[k];
                    DragonData potentialParentData = DragonData.FromProjIndex(k);
                    if (potentialParent.active && potentialParent.owner == Projectile.owner && DragonTypes.Contains(potentialParent.type) && potentialParentData.child == Projectile.whoAmI)
                    {
                        data.parent = potentialParent.projUUID;
                        Projectile.netUpdate = true;
                        break;
                    }
                }
                return;
            }

            Projectile.velocity = Vector2.Zero;
            Vector2 toParent = parentPos - Projectile.Center;

            // adjust target rotation to not be too far from our current rotation
            if (parentRotation != Projectile.rotation)
            {
                float num16;
                num16 = MathHelper.WrapAngle(parentRotation - Projectile.rotation);
                toParent = toParent.RotatedBy(num16 * 0.1f);
            }

            Projectile.rotation = toParent.ToRotation() + (float)Math.PI / 2f; // + 90 degrees anti(?)Clockwise
            Projectile.position = Projectile.Center;
            Projectile.scale = scaleFactor3;
            Projectile.width = Projectile.height = (int)(num17 * Projectile.scale);
            Projectile.Center = Projectile.position;
            if (toParent != Vector2.Zero)
            {
                // actually moving
                Projectile.Center = parentPos - Vector2.Normalize(toParent) * scaleFactor2 * scaleFactor3;
            }
            Projectile.spriteDirection = toParent.X > 0f ? 1 : -1;

        }

        public override void OnSpawn(IEntitySource source)
        {
            if (source is DragonSpawnSource dragonSource)
                data = dragonSource.data;
            else if (Projectile.type != HeadType) return;

            if (Projectile.type == TailType)
            {
                Main.NewText("tail spawned");
                return;
            }
            else if (Projectile.type == HeadType)
            {
                Main.NewText("head spawned");
                data = new(Projectile.whoAmI, 0);
                SpawnSegment(BodyType1);
            }
            else
            {
                int length = Head.Bonuses[DamageClass.Summon] >= minimumBonus ? 4 : 6;
                if (data.position >= length)
                {
                    SpawnSegment(TailType);
                    return;
                }

                if (Projectile.type == BodyType1)
                {
                    Main.NewText("body1 spawned");
                    SpawnSegment(BodyType2);
                }
                else if (Projectile.type == BodyType2)
                {
                    Main.NewText("body2 spawned");
                    SpawnSegment(BodyType1);
                }
            }
        }

        internal void SpawnSegment(int type)
        {
            data.child = Projectile.NewProjectile(new DragonSpawnSource(Projectile, data.Next(Projectile.whoAmI)), Projectile.position, Vector2.Zero, type, 45, 0, Projectile.owner);
        }

        public override void AI()
        {
            CommonAI();
            BodyTailAI();

            // making sure it stays in the world
            Projectile.position.X = MathHelper.Clamp(Projectile.position.X, 160f, Main.maxTilesX * 16 - 160);
            Projectile.position.Y = MathHelper.Clamp(Projectile.position.Y, 160f, Main.maxTilesY * 16 - 160);
        }

        //     public override void ModifyDamageScaling(ref float damageScale)
        //     {
        //damageScale += (Head.Bonuses[DamageClass.Generic] - 1);
        //     }
    }
    [ExtendsFromMod("ThoriumMod")]
    public class DragonSpawnSource : EntitySource_Parent
    {
        public readonly DragonData data;
        public DragonSpawnSource(Entity entity, DragonData data, string context = null) : base(entity, context)
        {
            this.data = data;
        }
    }
    [ExtendsFromMod("ThoriumMod")]
    public partial class DragonMinionHead : DragonMinion
    {
        // sheinanigans to make it not have to get mod player and get damage as much
        private Dictionary<DamageClass, float> _modes = null;
        public Dictionary<DamageClass, float> Bonuses
        {
            get
            {
                if (_modes == null)
                {
                    Player player = Main.player[Projectile.owner];
                    _modes = new();
                    _modes.Add(DamageClass.Melee, player.GetDamage(DamageClass.Melee).Additive);
                    _modes.Add(DamageClass.Magic, player.GetDamage(DamageClass.Magic).Additive);
                    _modes.Add(DamageClass.Summon, player.GetDamage(DamageClass.Summon).Additive);
                    _modes.Add(DamageClass.Ranged, player.GetDamage(DamageClass.Ranged).Additive);
                }
                return _modes;
            }
        }

        public override void AI()
        {
            CommonAI();
            HeadAI();

            Projectile.position.X = MathHelper.Clamp(Projectile.position.X, 160f, Main.maxTilesX * 16 - 160);
            Projectile.position.Y = MathHelper.Clamp(Projectile.position.Y, 160f, Main.maxTilesY * 16 - 160);

            _modes = null;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);

            if (Head.Bonuses[DamageClass.Melee] >= minimumBonus) modifiers.ScalingBonusDamage += Head.Bonuses[DamageClass.Melee] - 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            data = new(Projectile.whoAmI, 0);
            SpawnSegment(BodyType1);
        }

        bool retreating;
        Vector2 targetPos;
        public void HeadAI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 playerPos = player.Center;

            // snap back to player
            if (Projectile.Distance(playerPos) > 2000f)
            {
                Projectile.Center = playerPos;
                retreating = false;
                Projectile.netUpdate = true;
            }


            if (retreating)
            {
                if (Projectile.Center.Distance(targetPos) < 128) retreating = false;
                if (targetPos.Distance(playerPos) > 2000f) SetRetreatPos();
                targetPos += targetPos.DirectionTo(playerPos) * 16; // curves
                HeadAttack();
            }
            else
            {
                HeadTarget();
                if (targetPos != Vector2.Zero)
                {
                    HeadAttack();
                }
                else
                {
                    HeadIdle();
                }
            }

            HeadMovement();
        }

        void SetRetreatPos()
        {
            HeadTarget();
            if (targetPos == Vector2.Zero) targetPos = Main.player[Projectile.owner].Center;
            targetPos += Main.rand.NextVector2CircularEdge(768, 768);
        }

        // sets targetposition to where we want to go
        void HeadTarget()
        {
            targetPos = Vector2.Zero;

            float maxTargetedNPCDistance = 700f;
            float maxNPCDistFromPlayer = 1000f;
            Player player = Main.player[Projectile.owner];

            // target player chosen npc if there is one
            NPC ownerMinionAttackTargetNPC;
            ownerMinionAttackTargetNPC = Projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(Projectile) && Projectile.Distance(ownerMinionAttackTargetNPC.Center) < maxTargetedNPCDistance * 2f)
            {
                targetPos = ownerMinionAttackTargetNPC.Center;
                return;
            }

            // target npc ourselves
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC nPC;
                nPC = Main.npc[i];
                if (nPC.CanBeChasedBy(Projectile) && player.Distance(nPC.Center) < maxNPCDistFromPlayer && Projectile.Distance(nPC.Center) < maxTargetedNPCDistance)
                {
                    targetPos = nPC.Center;
                    return;
                }
            }
        }

        void HeadAttack()
        {
            Vector2 toTarget = targetPos - Projectile.Center;
            float distanceToTarget = toTarget.Length();

            // velocity multiplier depending on how far it is from target
            float scaleFactor;
            scaleFactor = 0.4f;
            if (distanceToTarget < 600f)
            {
                scaleFactor = 0.6f;
            }
            //if (distanceToTarget < 300f)
            //{
            //	scaleFactor = 0.8f;
            //}

            // general movement
            if (distanceToTarget > 16f)
            {
                Projectile.velocity += Vector2.Normalize(toTarget) * scaleFactor * 1.5f;
                // slower if the dragon isn't going directly towards the target
                if (Vector2.Dot(Projectile.velocity, toTarget) < 0.25f)
                {
                    Projectile.velocity *= 0.6f;
                }
            }

            float maxVelocity = 15f;
            if (Projectile.velocity.Length() > maxVelocity)
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * maxVelocity;
            }
        }

        void HeadIdle()
        {
            float VelocityScale = 0.2f;
            Player player = Main.player[Projectile.owner];
            Vector2 playerPos = player.Center;
            Vector2 toPlayer = playerPos - Projectile.Center;
            if (toPlayer.Length() < 200f)
            {
                VelocityScale = 0.12f;
            }
            if (toPlayer.Length() < 140f)
            {
                VelocityScale = 0.06f;
            }

            // some kind of small x and y velocity bonus
            if (toPlayer.Length() > 100f)
            {
                if (Math.Abs(playerPos.X - Projectile.Center.X) > 20f)
                {
                    Projectile.velocity.X += VelocityScale * Math.Sign(playerPos.X - Projectile.Center.X);
                }
                if (Math.Abs(playerPos.Y - Projectile.Center.Y) > 10f)
                {
                    Projectile.velocity.Y += VelocityScale * Math.Sign(playerPos.Y - Projectile.Center.Y);
                }
            }
            else if (Projectile.velocity.Length() > 2f)
            {
                Projectile.velocity *= 0.96f;
            }
            if (Math.Abs(Projectile.velocity.Y) < 1f)
            {
                Projectile.velocity.Y -= 0.1f;
            }


            float maxReturningVelocity = 15f;
            if (Projectile.velocity.Length() > maxReturningVelocity)
            {
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * maxReturningVelocity;
            }
        }

        void HeadMovement()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathF.PI / 2f;
            int direction = Projectile.direction;
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            if (direction != Projectile.direction)
            {
                Projectile.netUpdate = true;
            }

            // some kind of weird scaling stuff, not sure what localAI means here
            float num12 = MathHelper.Clamp(Projectile.localAI[0], 0f, 50f);
            int num17 = 30;
            Projectile.position = Projectile.Center;
            Projectile.scale = 1f + num12 * 0.01f;
            Projectile.width = Projectile.height = (int)(num17 * Projectile.scale);
            Projectile.Center = Projectile.position;

            // stardust dragon does a fading ina and out thing as an effect
            if (Projectile.alpha > 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    int num13;
                    num13 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.GreenTorch, 0f, 0f, 100, default, 2f);
                    Main.dust[num13].noGravity = true;
                    Main.dust[num13].noLight = true;
                }
                Projectile.alpha -= 42;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
        }

        public override void SetDefaults()
        {
            //Projectile.CloneDefaults(ProjectileID.StardustDragon1);
            Projectile.aiStyle = -1;
            Projectile.width = 32;
            Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.damage = 45;
            Projectile.hide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }
    }
    [ExtendsFromMod("ThoriumMod")]
    public class DragonMinionBody : DragonMinion
    {
        public override void SetDefaults()
        {
            //Projectile.CloneDefaults(ProjectileID.StardustDragon2);
            Projectile.aiStyle = -1;
            Projectile.width = 32;
            Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.damage = 45;
            Projectile.hide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }
    }
    [ExtendsFromMod("ThoriumMod")]
    public class DragonMinionBody2 : DragonMinion
    {
        public override void SetDefaults()
        {
            //Projectile.CloneDefaults(ProjectileID.StardustDragon2);
            Projectile.aiStyle = -1;
            Projectile.width = 32;
            Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.damage = 45;
            Projectile.hide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }
    }

    public class DragonMinionTail : DragonMinion
    {
        public override void SetDefaults()
        {
            //Projectile.CloneDefaults(ProjectileID.StardustDragon4);
            Projectile.aiStyle = -1;
            Projectile.width = 32;
            Projectile.height = 66;
            Projectile.tileCollide = false;
            Projectile.damage = 45;
            Projectile.hide = false;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
        }
    }
}