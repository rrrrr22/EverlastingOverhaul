using EverlastingOverhaul.Common.Graphics;
using EverlastingOverhaul.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace EverlastingOverhaul.Common.Utils
{
    public static partial class ModUtils
    {
        public static float Frac(float x) 
        {
            return x - MathF.Floor(x);
        }
        public static DrawData[] DrawData_Spliting(DrawData data, int numberOfImages, float rotation, Color color, float offset)
        {

            var datas = new DrawData[numberOfImages];
            for (int i = 0; i < numberOfImages; i++)
            {
                var newPosition = data.position + new Vector2(1, 0).RotatedBy((float)i / numberOfImages * MathHelper.TwoPi + rotation);
                datas[i] = data.CopyAndChangeDrawDataValues(positionData: newPosition + data.position.DirectionTo(newPosition) * offset, colorData: color);

            }

            return datas;
        }

        public static DrawData CopyAndChangeDrawDataValues(this DrawData data, Texture2D textureData = null, Vector2? positionData = null, Color? colorData = null, Rectangle? destinationRectData = null, Rectangle? sourceRectData = null, float? rotationData = null, Vector2? originData = null, Vector2? scaleData = null, SpriteEffects? spriteEffectsData = null)
        {
            return new(textureData ?? data.texture, positionData ?? data.position, sourceRectData ?? data.sourceRect, colorData ?? data.color, rotationData ?? data.rotation, originData ?? data.origin, scaleData ?? data.scale, spriteEffectsData ?? data.effect);
        }

        public static DrawData[] DrawData_AfterImage(DrawData data, int maxNumberOfImagesAtOnce, Color color, Vector2 directionAndLength, bool animate)
        {

            var datas = new DrawData[maxNumberOfImagesAtOnce];
            for (int i = 0; i < maxNumberOfImagesAtOnce; i++)
            {
                float progress = (float)i / maxNumberOfImagesAtOnce;
                var newPosition = Vector2.Zero;
                if (animate)
                    newPosition = Vector2.Lerp(data.position + directionAndLength, data.position, progress);
                else
                    newPosition = Vector2.Lerp(data.position + Vector2.Lerp(Vector2.Zero, directionAndLength, progress * (float)Main.timeForVisualEffects % 1), data.position, progress);
                datas[i] = data.CopyAndChangeDrawDataValues(positionData: newPosition, colorData: color * progress);

            }
            return datas;
        }
        public static void ApplyZDepthColor(this IZDepth zDepthHolder, ref Color drawColor)
        {
            drawColor = new Color((zDepthHolder.zDepth * 2f) * (drawColor.R / 255f), (zDepthHolder.zDepth * 2f) * (drawColor.G / 255f), (zDepthHolder.zDepth * 2f) * (drawColor.B / 255f), 1);
        }
        public static void ApplyZDepthScale(this IZDepth zDepthHolder, ref Vector2 scale) => scale *= new Vector2(MathHelper.Clamp(MathHelper.Lerp(0.0f, 1f, (zDepthHolder.zDepth)), 0, 1));
    
        public static void Insert<T>(this T[] array, T value)
        {
            Array.Resize(ref array, array.Length + 1);
            array.Push(value);
        }
        public static void Push<T>(this T[] array, T value)
        {
            Array.Copy(array, 0, array, 1, array.Length - 1);
            array[0] = value;
        }
        /// <summary>
        /// removes the first element and resizes the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static T? Pop<T>(this T[] array) where T : struct
        {
            if (array.Length == 0)
                return null;
            T value = array[0];
            Array.Copy(array, 0, array, 1, array.Length - 2);
            return value;
        }
    }
}
