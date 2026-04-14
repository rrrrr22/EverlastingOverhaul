using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using EverlastingOverhaul.Common.Graphics;

namespace EverlastingOverhaul.Common.Graphics {
	public interface IOutlineDrawer {
		public Color SetOutlineColor(float progress);
		public DrawData[] OutlineDrawDatas { get; }
		public int OutlineSteps { get; }
		public float OutlineOffset { get; }
		public ModdedShaderHandler Shader => EffectsLoader.shaderHandlers["Outline"];
	}

	public class OutlineDrawerSystem : ModSystem {

		public static HashSet<IOutlineDrawer> needsOutline = [];

		public override void Load() {
			
			On_Main.DoDraw_DrawNPCsOverTiles += DrawOutline;
		}
        public override void PreUpdateEntities()
        {
            needsOutline.Clear();
        }
        private void DrawOutline(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self) {
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

			foreach (IOutlineDrawer needsOutline in needsOutline) {
				int step = 0;
				ModdedShaderHandler shader = needsOutline.Shader;
				for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / needsOutline.OutlineSteps) {
					step++;
					float progress = (float)step / needsOutline.OutlineSteps;
                    shader.setProperties(needsOutline.SetOutlineColor(progress));
					shader.apply();
                    for (int j = 0; j < needsOutline.OutlineDrawDatas.Length; j++) {
						DrawData drawData = needsOutline.OutlineDrawDatas[j];
						drawData.position += new Vector2(MathF.Sin(i), MathF.Cos(i)) * needsOutline.OutlineOffset;
						Main.EntitySpriteDraw(drawData);
					}
				}
			}
			Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.End();
			orig.Invoke(self);

		}
	}

	public static class OutlineDrawer {
		public static void DrawOutline(this IOutlineDrawer needsOutline) {
			OutlineDrawerSystem.needsOutline.Add(needsOutline);
		}
	}
}

//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using Terraria;
//using Terraria.DataStructures;
//using Terraria.ModLoader;
//using EverlastingOverhaul.Common.Graphics;

//namespace EverlastingOverhaul.Common.Graphics
//{
//    public interface IOutlineDrawer
//    {
//        public Color SetOutlineColor(float progress);
//        public DrawData[] OutlineDrawDatas { get; }
//        public int OutlineSteps { get; }
//        public float OutlineOffset { get; }
//        public ModdedShaderHandler Shader => EffectsLoader.shaderHandlers["Outline"];
//    }

//    public static class OutlineDrawer
//    {
//        public static void DrawOutline(this IOutlineDrawer needsOutline, SpriteBatch sb, Vector2 posOffset)
//        {
//            int step = 0;
//            ModdedShaderHandler shader = needsOutline.Shader;
//            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / needsOutline.OutlineSteps)
//            {
//                step++;
//                float progress = (float)step / needsOutline.OutlineSteps;

//                for (int j = 0; j < needsOutline.OutlineDrawDatas.Length; j++)
//                {
//                    DrawData drawData = needsOutline.OutlineDrawDatas[j];
//                    drawData.position += new Vector2(MathF.Sin(i), MathF.Cos(i)) * needsOutline.OutlineOffset;
//                    drawData.position += posOffset;
//                    drawData.Draw(sb);
//                }
//            }

//            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
//        }
//    }
//}