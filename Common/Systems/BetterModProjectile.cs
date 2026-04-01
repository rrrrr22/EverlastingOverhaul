using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Systems
{
    public abstract class BetterModProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;
        public Vector2 spawnPosition = Vector2.Zero;
        public int maxTimeLeft = -1;
        public float ProjectileLifeProgress { get => (float)Projectile.timeLeft / maxTimeLeft; }
        public virtual int TrailLength => 30;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            BetterSetDefaults();
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = TrailLength;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }
        public virtual void BetterSetDefaults()
        {
        
        }
        public override sealed void OnSpawn(IEntitySource source)
        {
            spawnPosition = Projectile.Center;
            maxTimeLeft = (maxTimeLeft == -1 ? Projectile.timeLeft : maxTimeLeft);
            OnSpawn(source, spawnPosition);
        }

        public virtual void OnSpawn(IEntitySource source, Vector2 spawnPosition) 
        { 
        
        
        }
    }
}
