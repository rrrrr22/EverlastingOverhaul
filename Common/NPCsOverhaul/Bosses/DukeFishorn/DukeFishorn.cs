using EverlastingOverhaul.Common.Graphics;
using EverlastingOverhaul.Common.Graphics.Primitives;
using EverlastingOverhaul.Common.Systems;
using EverlastingOverhaul.Common.Systems.NPCReworker;
using EverlastingOverhaul.Common.Systems.TweenSystem;
using EverlastingOverhaul.Common.Utils;
using EverlastingOverhaul.Content.Particles;
using EverlastingOverhaul.Contents.Particles;
using EverlastingOverhaul.Texture;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.NPCsOverhaul.Bosses.DukeFishorn
{
    public class DukeFishorn : NPCReworkerFSM
    {

        public override int VanillaNPCType => NPCID.DukeFishron;
        public override bool UseCustomAnimation() => true;
        public override int frameHeight => 162;
        public override int startingFrame => 0;
        public override int animationSpeed => 4;
        public override int maxFrames => states != null && states.currentState.type == AIState.StateType<Duke_Spawn>() ? 7 : 5;
        public override int[] RegisterStates() =>
            [AIState.StateType<Duke_Super_Tornado>(),
            AIState.StateType<Boss_Despawn_State>(),
            AIState.StateType<Duke_Dash>(),
            AIState.StateType<Duke_Circle1>(),
            AIState.StateType<Duke_Super_Tornado>(),
            AIState.StateType<Duke_Phase2_Transition>(),
            AIState.StateType<Duke_Phase3_Transition>()
        ];
        public override void ReworkedAI(ref NPC npc)
        {
            base.ReworkedAI(ref npc);
            states.Update();
        }

    }
    public class DukeExplosionVortex : BetterModProjectile 
    {

        public override string Texture => ModTexture.CommonTextureStringPattern + "DukeExplosion";
        Tween<float> explosionAlpha;
        Tween<Vector2> explosionScale;
        public override void BetterSetDefaults()
        {
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 60;
            explosionScale = new Tween<Vector2>(Vector2.Lerp).TweenProperty([new(Vector2.Zero,new(.25f,0.25f),false,TweenEaseType.InExpo,10), new(new(1,0.25f), new(.7f, 0.7f), false, TweenEaseType.InExpo, 10),new(new Vector2(.7f,0.7f),new Vector2(1,1),false,TweenEaseType.InExpo,30)]);
            explosionAlpha = new Tween<float>(MathHelper.Lerp).TweenProperty([new(1,0,false,TweenEaseType.InExpo,60)]);
        }
        public override void AI()
        {
        }
        private static VertexRectangle rect = new();
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            var col = Color.White * 20 * explosionAlpha.currentProgress;
            ModdedShaderHandler shader = EffectsLoader.shaderHandlers["MagicExplosion"];
            shader.setProperties(Color.Turquoise, TextureAssets.Extra[193].Value);
            shader.apply();
            rect.Draw(Projectile.Center - Main.screenPosition,Color.White,new Vector2(256,256) * explosionScale, 0, Projectile.Center - Main.screenPosition);
            // Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,Projectile.Center - Main.screenPosition,null, col, 0, TextureAssets.Projectile[Type].Size()/2f,explosionScale.currentProgress,SpriteEffects.None);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        }
    }
    public class Dukenado : BetterModProjectile
    {
        private static Asset<Texture2D> grad;
        private static Asset<Texture2D> grad2;
        public override void Load()
        {
            base.Load();
            grad = ModContent.Request<Texture2D>("EverlastingOverhaul/Texture/Gradient");
            grad2 = ModContent.Request<Texture2D>("EverlastingOverhaul/Texture/Gradient2");

        }
        public override void BetterSetDefaults()
        {
            base.BetterSetDefaults();
            Projectile.timeLeft = 600;
        }
        private static VertexRectangle rect = new();
        public override void PostDraw(Color lightColor)
        {
            ModdedShaderHandler tornadoShader = EffectsLoader.shaderHandlers["DukeTornado"];
            tornadoShader.setProperties(Color.Turquoise, TextureAssets.Extra[193].Value, grad.Value, grad2.Value, new Vector4(0, MathHelper.Clamp((MathHelper.Lerp(0, 1, (Projectile.timeLeft - 450f) / 150f)), 0, 1), 0, 0));
            tornadoShader.apply();
            rect.Draw(Projectile.Center - Main.screenPosition, size: new Vector2(256 * 4, 1024), rotationCenter: Projectile.Center - Main.screenPosition);
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }
    }
    public class DukeShadows : BetterModProjectile
    {

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.EntitySpriteDraw(TextureAssets.DukeFishron.Value,Projectile.Center);
            return base.PreDraw(ref lightColor);
        }

    }
    public class DukeBulletVortex : BetterModProjectile
    {
        public override int TrailLength => 1;
        public override void BetterSetDefaults()
        {
            base.BetterSetDefaults();
            Projectile.timeLeft = 240;
            Projectile.hostile = true;
            Projectile.friendly = false;
        }
        private VertexRectangle rect = new();
        public override bool PreDraw(ref Color lightColor)
        {
            ModdedShaderHandler shader = EffectsLoader.shaderHandlers["DukeBullet"];
            shader.setProperties(Color.Blue, TextureAssets.Extra[193].Value);
            shader.apply();
            rect.Draw(Projectile.Center - Main.screenPosition, Color.White, new Vector2(256 * 1.1f,128 * 1.25f),Projectile.rotation + MathHelper.Pi, Projectile.Center - Main.screenPosition);
            shader.setProperties(Color.Turquoise, TextureAssets.Extra[193].Value);
            shader.apply();
            rect.Draw(Projectile.Center - Main.screenPosition, Color.White, new Vector2(256, 128), Projectile.rotation + MathHelper.Pi, Projectile.Center - Main.screenPosition);

            return false;
        }
        Vector2 velLength;
        public override void AI()
        {
            base.AI();

            if (maxTimeLeft == Projectile.timeLeft)
            {
                velLength = Projectile.velocity;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();

            Particle.NewParticle(Particle.ParticleType<BasicParticle>(), Projectile.Center - Main.rand.NextVector2Circular(32, 32), ParticleTemplates._default with { shaderID = "DukeWaterStream", lifetime = 32, dontDrawSelf = true, rotation = Projectile.rotation, velocitySlowdown = .97f, velocity = Projectile.velocity, startColor = Color.Cyan, endColor = Color.Cyan, endOpacity = 0, startOpacity = 1, startSize = Main.rand.NextFloat() * 16f, endSize = 0 });

        }

    }
    public class DukeBiome : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<DukeBG>();

        public override float GetWeight(Player player)
        {
            return 1f;
        }
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override bool IsBiomeActive(Player player)
        {
            DisableWorldBackgroundElements();
            return true;
        }

        public static void DisableWorldBackgroundElements()
        {

            for (int i = 0; i < Main.maxClouds; i++)
            {
                Main.cloud[i].active = false;
            }
            Main.cloudBGActive = 0;



        }
        public override void OnInBiome(Player player)
        {
            //Particle.NewParticle(Particle.ParticleType<DukeLightingBGParticle1>(),Main.screenPosition + new Vector2(Main.rand.Next(0,Main.ScreenSize.X), 0), ParticleTemplates._default with { dontDrawSelf = true, stripShaderID = "DukeLightingBG", vertexRectSize = new Vector2(256), rotation = Vector2.UnitY.ToRotation(), stripWidth = 6 , endOpacity = 1, endSize = 1f, startColor = Color.Cyan, endColor = Color.Turquoise, lifetime = 60,velocity = Vector2.UnitY * 35 });
        }
    }
    public class DukeLightingBGParticle1 : Particle
    {
        Vector2 direction = Vector2.Zero;
        Vector2 fixedPos = Vector2.Zero;
        public override void OnSpawn()
        {
            base.OnSpawn();
            direction = velocity.RotatedByRandom(0.2f);

        }
        public override void PostUpdate()
        {


            if (timeleft % 3 != 0)
                return;

            if (velocity.ToRotation() > direction.ToRotation())
                velocity = direction.RotatedBy(0.4f * Main.rand.NextFloat() + 0.025f);
            else
                velocity = direction.RotatedBy(-.4f * Main.rand.NextFloat() - .025f);

            rotation = velocity.ToRotation();


        }
    }
    public class DukeLightingBGParticle2 : Particle
    {

    }

    public class DukeBG : ModSurfaceBackgroundStyle
    {


        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
        }
        private static VertexRectangle rect = new();

        public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
        {
            ModdedShaderHandler handler = EffectsLoader.shaderHandlers["DukeBG"];

            handler.setProperties(Color.White, TextureAssets.Extra[193].Value, shaderData: new Vector4(Main.Camera.Center.X, Main.Camera.Center.Y, Main.ScreenSize.X, Main.ScreenSize.Y));
            handler.apply();

            rect.Draw(Main.Camera.Center - Main.screenPosition, Color.White,new(Main.ScreenSize.X,Main.ScreenSize.Y),0, Main.Camera.Center - Main.screenPosition);

            return false;
        }


    }
    public class Duke_AIState : AIState
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            UpdateSpriteFields(0, 6);
            dashDelayBasedOnPhase = 15;
        }
        internal int dashDelayBasedOnPhase = 15;
        public override void OnPostStateUpdate()
        {
            if (npc.ai[0] == 0 && npc.life <= npc.lifeMax / 2f) // 50%
            {
                npc.ai[0]++;
                preStateChangeState = StateType<Duke_Phase2_Transition>();
                Main.NewText("The Winds grows stronger...",Color.Turquoise);
                dashDelayBasedOnPhase = 10;
            }
            if (npc.ai[0] == 1 && npc.life <= npc.lifeMax / 4f) // 25%
            {
                dashDelayBasedOnPhase = 1;
                npc.ai[0]++;
                preStateChangeState = StateType<Duke_Phase3_Transition>();
                Main.NewText("You suddenly almost feel like drowning...", Color.Turquoise);
            }
        }

        internal VertexRectangle rectDash = new();
        private Asset<Texture2D> dukeAura;
        public override void Load()
        {
            base.Load();
            dukeAura = ModContent.Request<Texture2D>(ModTexture.CommonTextureStringPattern + "DukeAura");
        }
        internal Vector2 currentDashTrailScale = Vector2.Zero;
        public override bool StatePreDraw(ref DrawData mainSprite, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            return true;
        }
        public override void StatePostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.ai[1] == 2)
                spriteBatch.Draw(TextureAssets.DukeFishron.Value, npc.Center - screenPos, frameRect, Color.White, npc.rotation, frameRect.Size() / 2f, npc.scale, npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

            if (npc.ai[2] == 0)
                return;
            ModdedShaderHandler shader = EffectsLoader.shaderHandlers["DukeTornado"];
            shader.setProperties(color: Color.Turquoise);
            shader.apply();

            rectDash.Draw(npc.Center - screenPos, rotation: npc.rotation - (-npc.spriteDirection == -1 ? 3.1415f : 0), size: currentDashTrailScale, rotationCenter: npc.Center - screenPos);

        }

    }
    public class Duke_Phase2_Transition : Duke_AIState
    {
        int delay = 300;
        public override void OnStateUpdate(CommonNPCInfo info)
        {
            delay--;
            npc.velocity *= 0.98f;
            if (delay == 150)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath29, npc.Center);
                PunchCameraModifier p = new(npc.Center, Main.rand.NextVector2Circular(3, 3), 15, 10, 60, 100000);
                Main.instance.CameraModifiers.Add(p);
                npc.ai[1] = 1;
            }
            if (delay == 0)
                ChangeState(StateType<Duke_Dash>());
        }

    }
    public class Duke_Phase3_Transition : Duke_AIState
    {
        int delay = 300;
        public override void OnStateUpdate(CommonNPCInfo info)
        {
            delay--;
            npc.velocity *= 0.98f;
            if (delay <= 150)
                customFrameIndex = 7;
            if (delay == 150)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath29, npc.Center);
                PunchCameraModifier p = new(npc.Center, Main.rand.NextVector2Circular(3, 3), 15, 10, 60, 100000);
                Main.instance.CameraModifiers.Add(p);
                npc.ai[1] = 2;
            }
            if (delay == 0)
                ChangeState(StateType<Duke_Dash>());
        }

    }
    public class Duke_Super_Tornado : Duke_AIState
    {

        Vector2 startingPos = Vector2.Zero;
        public override void OnEntered(int oldState)
        {
            base.OnEntered(oldState);
            PunchCameraModifier p = new(npc.Center, Main.rand.NextVector2Circular(3, 3), 15, 10, 60, 100000);
            Main.instance.CameraModifiers.Add(p);
            startingPos = Target.Center - new Vector2(0, 250);
            NPCReworkerFSM.NewProjectileWithMPCheck(npc.GetSource_FromAI(), startingPos, Vector2.Zero, ModContent.ProjectileType<Dukenado>(), 100, 0);

        }

        public override void OnStateUpdate(CommonNPCInfo info)
        {
            base.OnStateUpdate(info);
            npc.velocity *= 0.94f;
            npc.direction = npc.Center.X > startingPos.X ? 1 : -1;
            if (counter == 180)
            {
                npc.velocity = new Vector2(Target.Center.X > npc.Center.X ? -48 : 48, 0);
            }

            if (counter < 180)
                npc.Center = Vector2.Lerp(npc.Center, startingPos + new Vector2(MathF.Cos(counter * MathHelper.Lerp(0.1f, 0.3f, MathHelper.Clamp(counter / 160f, 0, 1))) * 256, MathF.Sin(counter * MathHelper.Lerp(0.1f, 0.3f, MathHelper.Clamp(counter / 160f, 0, 1))) * 64), 0.7f);

            if (counter == 240)
            {
                ChangeState(StateType<Duke_Dash>());

            }
        }
    }
    public class Duke_Spawn : Duke_AIState
    {

        public override void OnEntered(int oldState)
        {
            UpdateSpriteFields(0, 7);
        }

        public override void OnStateUpdate(CommonNPCInfo info)
        {
            npc.velocity = -Vector2.UnitY * 2;
            if (counter == 14 * 4)
                ChangeState(StateType<Duke_Dash>());
        }

    }

    public class Duke_Circle1 : Duke_AIState
    {
        int fireDelay = 0;
        int recoilCounter = 0;
        int currentCircleAngle = 0;
        float currentCircleRot = 0;
        int fireCounter = 0;
        int rotDir = 1;
        float rotEase = 0;
        Vector2[] clonesPos = new Vector2[5];
        Tween<float> clonesFadeAlpha;
        public override void OnEntered(int oldState)
        {
            base.OnEntered(oldState);
            npc.velocity = Vector2.Zero;
            currentCircleRot = npc.DirectionFrom(Target.Center).ToRotation();
            fireDelay = Main.rand.Next(60, 100);
            clonesFadeAlpha = new Tween<float>(MathHelper.Lerp).TweenProperty([new(1, 0, false, TweenEaseType.InExpo, 15), new(0, 1, false, TweenEaseType.InExpo, 25)]);
            for (int i = 0; i < 60; i++)
            {
                Particle.NewParticle(Particle.ParticleType<BasicParticle>(), npc.Center, ParticleTemplates._default with { shaderID = "DukeWaterStream", lifetime = 32, dontDrawSelf = true, rotation = npc.rotation - (npc.spriteDirection == -1 ? MathHelper.Pi : 0), vertexRectSize = new Vector2(128, 128), velocitySlowdown = .7f, velocity = (npc.rotation.ToRotationVector2().RotatedBy(Main.rand.NextVector2Circular(16, 16).ToRotation())) * 24, startColor = Color.Yellow, endColor = Color.Yellow, endOpacity = 0, startOpacity = 1, startSize = Main.rand.NextFloat() * 2f, endSize = 0 });

            }

        }
        public override void OnStateUpdate(CommonNPCInfo info)
        {
            int loop = 0;
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / clonesPos.Length) 
            {
                
                clonesPos[loop] = Target.Center + new Vector2(npc.Distance(Target.Center), 0).RotatedBy((Target.DirectionTo(npc.Center).ToRotation() + i));
                loop++;
            }
            if (recoilCounter > -25)
            {
                recoilCounter--;
                npc.Center += npc.DirectionFrom(Target.Center) * (MathF.Abs(35/(float)recoilCounter) * 2);
                customFrameIndex = 7;
                npc.rotation = npc.DirectionTo(Target.Center).ToRotation() + (npc.spriteDirection == 1 ? MathHelper.Pi : 0);

                return;
            }
            currentCircleRot += MathHelper.TwoPi / 30f * MathHelper.Clamp(MathHelper.Lerp(5, 0, ModUtils.InExpo(fireDelay / 120f, 11f)), 0, 1) * rotDir;
            npc.Center = Vector2.Lerp(npc.Center, Target.Center + new Vector2(700, 0).RotatedBy(currentCircleRot), 0.2f);
            rotEase = (npc.Center - npc.oldPosition).Length();
            npc.FaceTarget();
            npc.spriteDirection = npc.direction * -1;
            npc.rotation = npc.DirectionTo(Target.Center).ToRotation() + (npc.spriteDirection == 1 ? MathHelper.Pi : 0);


            if (fireCounter >= 7)
            {
                fireDelay = 0;
                recoilCounter = 0;
                fireCounter = 0;
                npc.rotation = 0;
                ChangeState(StateType<Duke_Dash>());
            }

            fireDelay++;

            if (fireDelay > 145)
            {
                npc.rotation = npc.DirectionTo(Target.Center).ToRotation() + (npc.spriteDirection == 1 ? MathHelper.Pi : 0);
                npc.spriteDirection = npc.direction * -1;
                if (fireDelay < 175)
                {
                    npc.Center -= npc.DirectionFrom(Target.Center) * 15f;
                    return;
                }

                fireCounter++;
                recoilCounter = 0;
                SoundEngine.PlaySound(SoundID.Item115 with { Pitch = 0.25f},Target.Center);

                for (int l = 0; l < clonesPos.Length; l++) 
                {
                    NPCReworkerFSM.NewProjectileWithMPCheck(npc.GetSource_FromAI(), clonesPos[l], clonesPos[l].DirectionTo(Target.Center).RotatedBy(0) * 25, ModContent.ProjectileType<DukeBulletVortex>(), 50, 1);
                    for (int i = 0; i < 60; i++)
                    {
                        Particle.NewParticle(Particle.ParticleType<BasicParticle>(), clonesPos[l], ParticleTemplates._default with { shaderID = "DukeWaterStream", lifetime = 32, dontDrawSelf = true, rotation = npc.rotation - (npc.spriteDirection == -1 ? MathHelper.Pi : 0), vertexRectSize = new Vector2(128, 128), velocitySlowdown = .7f, velocity = (npc.rotation.ToRotationVector2().RotatedBy(Main.rand.NextVector2Circular(16,16).ToRotation())) * 24, startColor = Color.Cyan, endColor = Color.Cyan, endOpacity = 0, startOpacity = 1, startSize = Main.rand.NextFloat() * 2f, endSize = 0 });

                    }
                }
                fireDelay = Main.rand.Next(115, 120);
                rotDir = Main.rand.NextBool() == true ? -1 : 1;
                clonesFadeAlpha = new Tween<float>(MathHelper.Lerp).TweenProperty([new(1, 0, false, TweenEaseType.InExpo, 15), new(0, 1, false, TweenEaseType.InExpo, 50)]);
                dingStarScale = new Tween<Vector2>(Vector2.Lerp).TweenProperty([new(new(0.25f, 2), Vector2.One, false, TweenEaseType.InExpo, 15), new(new(3, 0.25f), new(1, 2), false, TweenEaseType.None, 10), new(Vector2.Zero, new(2, 0.25f), false, TweenEaseType.InExpo, 15)]);

            }


        }
        private static VertexRectangle rect = new();
        private Tween<Vector2> dingStarScale;
        public override bool StatePreDraw(ref DrawData mainSprite, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            for (int i = 0; i < clonesPos.Length; i++)
            {

                mainSprite.position = clonesPos[i] - screenPos;
                bool flip = clonesPos[i].X < Target.Center.X;
                mainSprite.rotation = clonesPos[i].DirectionTo(Target.Center).ToRotation() + (flip ? MathHelper.Pi : 0);
                mainSprite.effect = flip ? SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically : SpriteEffects.FlipVertically;
                if(clonesFadeAlpha != null)
                    mainSprite.color = (Color.Lerp(Color.Transparent, i == 0 ? drawColor : Color.White * 0.5f, clonesFadeAlpha.currentProgress));
                mainSprite.Draw(spriteBatch);


            }
            ModdedShaderHandler shader = EffectsLoader.shaderHandlers["MagicExplosion"];
            shader.setProperties(Color.Yellow, TextureAssets.Extra[193].Value);
            shader.apply();
            rect.Draw(npc.Center - screenPos, Color.White, new Vector2(256, 256) * dingStarScale, 0, npc.Center - screenPos);
            // Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,Projectile.Center - Main.screenPosition,null, col, 0, TextureAssets.Projectile[Type].Size()/2f,explosionScale.currentProgress,SpriteEffects.None);

            return false;
        }

    }
    public class Duke_Dash : Duke_AIState
    {

        int dashCounter = 0;
        int dashDelay = 0;
        Vector2 offset = Vector2.Zero;
        Vector2 dashDir = Vector2.Zero;
        Tween<Vector2> dashTrailScale;
        bool isSuper = false;
        public override void OnStateUpdate(CommonNPCInfo info)
        {
            
            if (counter == 1)
            {
                offset = new Vector2(256 * -npc.spriteDirection, Main.rand.Next(-256, 256));
                dashDir = npc.DirectionTo(Target.Center);

            }

            if (counter <= 15)
            {
                npc.FaceTarget();
                npc.spriteDirection = npc.direction * -1;
                npc.Center = Vector2.Lerp(Vector2.Lerp(npc.Center, npc.Center - Vector2.UnitY * 128, counter / 35f), Target.Center + offset + (npc.Center.X > Target.Center.X ? Vector2.UnitX * 512 : Vector2.UnitX * -512), counter / 60f);
                dashDir = npc.DirectionTo(Target.Center);
                return;
            }

            dashDelay++;

            if (dashDelay < dashDelayBasedOnPhase)
            {
                npc.rotation = npc.DirectionTo(Target.Center).ToRotation() + (npc.spriteDirection == 1 ? MathHelper.Pi : 0);
                npc.velocity = Vector2.Lerp(npc.DirectionTo(Target.Center) * -45, npc.DirectionTo(Target.Center) * 16, dashDelay / 15f);
                return;
            }

            npc.velocity.MoveTowards(npc.velocity * 1.2f, 40);

            if (dashDelay == dashDelayBasedOnPhase)
            {
                npc.velocity = dashDir * 125;
                npc.rotation = dashDir.ToRotation() + (npc.spriteDirection == 1 ? MathHelper.Pi : 0);
                npc.spriteDirection = npc.velocity.X < 0 ? 1 : -1;
                npc.ai[2] = 1;
                dashTrailScale = new Tween<Vector2>(Vector2.Lerp).TweenProperty([
                    new(Vector2.Zero,new Vector2(512,1500),false,TweenEaseType.None,2),
                    new(new Vector2(256,1255),new Vector2(4120,1024),false,TweenEaseType.None, 5),
                new(new Vector2(4120,1024),new Vector2(4120,0),false,TweenEaseType.None, 15)]);
                PunchCameraModifier p = new(npc.Center, Main.rand.NextFloatDirection().ToRotationVector2() * 16, 7, 10, 15, -1);
                Main.instance.CameraModifiers.Add(p);
                SoundEngine.PlaySound(SoundID.NPCDeath43 with { Pitch = -.5f}, npc.Center);
                SoundEngine.PlaySound(SoundID.NPCDeath38 with { Pitch = .5f}, npc.Center);
                SoundEngine.PlaySound(SoundID.Roar with { Pitch = .8f, MaxInstances = 0, Volume = 0.5f}, npc.Center);
                for(int i = 0; i < 60; i++)
                {
                    Particle.NewParticle(Particle.ParticleType<BasicParticle>(), npc.Center - Main.rand.NextVector2Circular(0, 64) - npc.velocity.SafeNormalize(Vector2.UnitY) * 256, ParticleTemplates._default with { shaderID = "DukeWaterStream", lifetime = 32, dontDrawSelf = true, rotation = npc.rotation - (npc.spriteDirection == -1 ? MathHelper.Pi : 0), vertexRectSize = new Vector2(2566, 256), velocitySlowdown = .97f, velocity = npc.velocity * (i / 60f) * 1.25f, startColor = Color.Cyan, endColor = Color.Cyan, endOpacity = 0, startOpacity = 1, startSize = Main.rand.NextFloat() * 2f, endSize = 0 });

                }

            }

            if (dashDelay >= dashDelayBasedOnPhase)
            {
                currentDashTrailScale = dashTrailScale.currentProgress;
            }

            if (counter > 45)
                npc.velocity *= 0.96f;

            if (counter == 65)
            {
                npc.velocity = Vector2.Zero;
                npc.rotation = 0;
                dashDelay = 0;
                dashCounter++;
                npc.ai[2] = 0;
                if (dashCounter % 8 != 0) 
                    ChangeState(StateType<Duke_Dash>());
                else
                {
                    ChangeState(StateType<Duke_Circle1>());
                    dashCounter = 0;
                }




            }

        }

    }
}
