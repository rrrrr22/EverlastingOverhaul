using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Graphics
{
    public interface IZDepth
    {
        float zDepth { get; set; }
    }

    public class ZDepthsSystem : ModSystem 
    {
        private static GraphicsDevice gd => Main.instance.GraphicsDevice;
        public static Matrix VanillaTransformWithZDepth(float zDepth) 
        {
            Vector2 vector = new Vector2(gd.Viewport.Width, gd.Viewport.Height);
            Vector2 vector2 = vector * 0.5f;
            Vector2 translation = vector2 - vector2 / (Main.GameViewMatrix.Zoom * new Vector2(zDepth));
            return Matrix.CreateTranslation(-translation.X, -translation.Y, 0) * Matrix.CreateScale(Main.GameViewMatrix.Zoom.X * zDepth, Main.GameViewMatrix.Zoom.Y * zDepth, 1f);
        }
    }
}
