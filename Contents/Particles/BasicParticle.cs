using EverlastingOverhaul.Common.Systems;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace EverlastingOverhaul.Contents.Particles
{
    public class BasicParticle : Particle
    {
        public bool followProjectile = true;
        public override bool PreUpdate()
        {
            if(followProjectile && parentProjectile != -1)
            {
                position = Main.projectile[parentProjectile].Center;
                rotation = Main.projectile[parentProjectile].rotation + MathHelper.Pi;
            }
            return true;
        }
    }
}
