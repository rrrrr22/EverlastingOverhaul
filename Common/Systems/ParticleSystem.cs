using EverlastingOverhaul.Common.Graphics;
using EverlastingOverhaul.Common.Graphics.Primitives;
using EverlastingOverhaul.Common.Utils;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Systems
{

    public class ParticleSystem : ModSystem
    {
        public const int MAX_PARTICLES = Main.maxDust;
        public Particle[] particles = new Particle[MAX_PARTICLES];
        public override void PreUpdateProjectiles()
        {
            for(int i = 0; i < MAX_PARTICLES; i++)
            {

                if (particles[i] == null || particles[i].isDone)
                {
                    continue;
                }
                particles[i].Update();

            }
        }

        public override void Load()
        {
            base.Load();
            On_Main.DoDraw_DrawNPCsOverTiles += On_Main_DoDraw_DrawNPCsOverTiles;
        }

        private void On_Main_DoDraw_DrawNPCsOverTiles(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                if (particles[i] == null || particles[i].isDone)
                    continue;

                particles[i].Draw(Main.spriteBatch);
            }
            Main.spriteBatch.End();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

        }
    }

    public abstract class Particle : ILoadable
    {
        public int type { get; private set; }
        private readonly static List<Particle> particleInstances = new();
        public Vector2 position;
        public Vector2 velocity;
        public float rotation;
        public float size;
        public bool isDone = false;
        public float timeleft;
        public Color color;
        public float opacity;
        public ParticlesAttributes particlesAttributes;
        public DataCache<Vector2> oldPositionsCache;
        public DataCache<float> oldRotationsCache;
        public virtual string Texture => "";
        public Asset<Texture2D> loadedTexture = null;
        public float timeleftPercent { get => MathHelper.Clamp(timeleft / particlesAttributes.lifetime, 0f, 1f); }
        public ModdedShaderHandler shader;
        public ModdedShaderHandler stripShader;
        public bool dontDrawSelf;
        public Vector2 spawnPosition;
        public float maxTimeleft;
        internal static VertexRectangle vertexRect = new();
        public Vector2 vertexRectSize;
        public int whoAmI = -1;
        internal static VertexStrip vertexStrip = new();
        public virtual void OnSpawn()
        {

        }
        public static int ParticleType<T>() where T : Particle => ModContent.GetInstance<T>().type;

        public static Particle NewParticle(int type, Vector2 position, ParticlesAttributes p)
        {
            Particle particle = (Particle)particleInstances[type].MemberwiseClone();
            particle.position = position;
            particle.velocity = p.velocity;
            particle.rotation = p.rotation;
            particle.size = p.startSize;
            particle.timeleft = p.lifetime;
            particle.color = p.startColor;
            particle.vertexRectSize = p.vertexRectSize;
            //particle.parentProjectile = parentProjectile == null ? -1 : parentProjectile.Projectile.whoAmI;
            if (p.shaderID != string.Empty)
            {
                EffectsLoader.shaderHandlers.TryGetValue(p.shaderID, out ModdedShaderHandler shader);
                particle.shader = shader;
            }
            if (p.stripShaderID != string.Empty)
            {
                EffectsLoader.shaderHandlers.TryGetValue(p.stripShaderID, out ModdedShaderHandler shader);
                particle.stripShader = shader;
            }
            particle.dontDrawSelf = p.dontDrawSelf;
            particle.particlesAttributes = p;
            particle.spawnPosition = position;
            //particle.oldPositions = new Vector2[30];
            //particle.oldRotation = new float[30];
            //for (int i = 0; i < particle.oldPositions.Length; i++)
            //{
            //    particle.oldPositions[i] = position;
            //    particle.oldRotation[i] = p.rotation;
            //}
            particle.oldPositionsCache = new DataCache<Vector2>(30,particle.position);
            particle.oldRotationsCache = new DataCache<float>(30,particle.rotation);
            // simply dont dont add it if max particles is reached cuz idk lol dont spam particles 
            for (int i = 0; i < ParticleSystem.MAX_PARTICLES; i++)
            {
                if (ModContent.GetInstance<ParticleSystem>().particles[i] == null || ModContent.GetInstance<ParticleSystem>().particles[i].isDone)
                {
                    ModContent.GetInstance<ParticleSystem>().particles[i] = particle;
                    particle.whoAmI = i;
                    particle.OnSpawn();
                    break;
                }
            }
            return particle;
        }

        public virtual bool PreUpdate() => !isDone;
        public virtual void PostUpdate() { }

        public void Update()
        {
            if (!PreUpdate())
                return;
            GeneralMovementUpdate();
            GeneralLerpValuesUpdate();
            ParticleLifetimeUpdate();
            PostUpdate();
        }
        public virtual void ParticleLifetimeUpdate()
        {
            timeleft--;

            if (timeleft <= 0)
            {
                isDone = true;
            }
        }
        public virtual void GeneralLerpValuesUpdate()
        {
            color = Color.Lerp(particlesAttributes.endColor, particlesAttributes.startColor, timeleftPercent);
            opacity = MathHelper.Clamp(MathHelper.Lerp(particlesAttributes.endOpacity, particlesAttributes.startOpacity, timeleftPercent), 0f, 1f);
            size = MathHelper.Lerp(particlesAttributes.endSize, particlesAttributes.startSize, timeleftPercent);
        }
        public virtual void GeneralMovementUpdate()
        {
            oldPositionsCache.InsertCache(position);
            oldRotationsCache.InsertCache(rotation);
            position += velocity;
            velocity *= particlesAttributes.velocitySlowdown;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            ExtraDrawing(spriteBatch);

            if (stripShader != null)
            {
                stripShader.setProperties(color, loadedTexture?.Value, shaderData: new Vector4(opacity, timeleftPercent, 0, 0));
                stripShader.apply();
                vertexStrip.PrepareStripWithProceduralPadding(oldPositionsCache.cache, oldRotationsCache.cache, (_) => color, (p) => MathHelper.Lerp(particlesAttributes.stripWidth * size, particlesAttributes.stripEndWidth * size, p), -Main.screenPosition, true);
                vertexStrip.DrawTrail();
            }
            if (shader != null)
            {
                shader.setProperties(color, loadedTexture?.Value, shaderData: new Vector4(opacity, timeleftPercent, 0, 0));
                shader.apply();
                vertexRect.Draw(position - Main.screenPosition, Color.White, vertexRectSize * size, rotation, position - Main.screenPosition);
            }



            if (!dontDrawSelf)
                spriteBatch.Draw(loadedTexture.Value, position - Main.screenPosition, null, color * MathHelper.Lerp(0, 1, opacity), rotation, loadedTexture.Size() / 2f, size, SpriteEffects.None, 0f);

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
        public virtual void ExtraDrawing(SpriteBatch spriteBatch) { }
        public void Load(Mod mod)
        {
            type = particleInstances.Count;
            particleInstances.Add(this);
            if (Texture != string.Empty)
                loadedTexture = ModContent.Request<Texture2D>(Texture);
        }

        public void Unload()
        {

        }
    }

    public record struct ParticlesAttributes
    {
        public int lifetime = 60;
        public Vector2 velocity = Vector2.Zero;
        public float startSize = 1f;
        public float endSize = 0f;
        public float rotation = 0f;
        public Color startColor = Color.White;
        public Color endColor = Color.Green;
        public float startOpacity = 1f;
        public float endOpacity = 0f;
        public float velocitySlowdown = 0.975f;
        public string shaderID = "";
        public bool dontDrawSelf = false;
        public Vector2 vertexRectSize = Vector2.One * 16;
        public string stripShaderID = "";
        public float stripWidth = 32f;
        public float stripEndWidth = 32f;
        public ParticlesAttributes() { }


    }
}

