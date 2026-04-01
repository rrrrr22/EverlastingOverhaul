using EverlastingOverhaul.Common.Utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Systems.NPCReworker
{
    public class AIState : ILoadable
    {
        public int npcID = -1;
        public int npcWhoAmI = -1;
        public int preStateChangeState = -1;
        #region Sprite and Animation Handling

        public int currentFrame = 0;
        public int frameHeight = 128;
        public int frameWidth = 128;
        public int startingFrame = 0;
        public int animationSpeed = 14;
        public int maxFrames = 0;
        public Rectangle frameRect = new();
        public Asset<Texture2D> npcTexture;
        public int customFrameIndex = -1;
        public DrawData UpdateCurrentSprite()
        {
            return new DrawData((npcTexture == null ? TextureAssets.Npc[npcID].Value : npcTexture.Value), Vector2.Zero, frameRect, Color.White, npc.rotation, new Vector2(frameWidth / 2f, frameHeight / 2f), npc.scale, npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }

        public void UpdateSpriteFields(int startingFrame = 0, int maxFrames = -1, int animationSpeed = 28, int frameHeight = -1) 
        {
            this.startingFrame = startingFrame;
            this.animationSpeed = animationSpeed;
            Texture2D texture = (npcTexture == null ? TextureAssets.Npc[npcID].Value : npcTexture.Value);

            if (frameHeight == -1)
                this.frameHeight = texture.Height / Main.npcFrameCount[npcID];
            else
                this.frameHeight = frameHeight;

            if (maxFrames == -1)
                this.maxFrames = Main.npcFrameCount[npcID];
            else
                this.maxFrames = maxFrames;

            frameWidth = texture.Width;

        }

        public void UpdateCurrentSpriteFrame() 
        {
            if (customFrameIndex == -1)
                currentFrame = (int)(MathHelper.Lerp(startingFrame, startingFrame + maxFrames, (counter % animationSpeed) / (float)animationSpeed));
            else 
            {
                currentFrame = customFrameIndex;
                customFrameIndex = -1;
            }
            frameRect = new Rectangle(0, currentFrame * this.frameHeight, this.frameWidth, this.frameHeight);
            
        }
        #endregion
        public Player Target
        {
            get
            {
                npc.TargetClosest();
                if (npc.HasValidTarget)
                {
                    Player player = Main.player[npc.target];
                    if (player.Center.X == npc.Center.X)
                        npc.direction = player.direction;
                    return player;
                }
                return null;
            }
        }
        public NPC npc
        {
            get { return Main.npc[npcWhoAmI]; }
        }
        public int type { get; private set; }
        public int counter
        {
            get { return stateCounter; }
            set { stateCounter = value; }
        }
        public float zDepth
        {
            get => ((FiniteStateMachineNPC)npc.ModNPC).zDepth;
            set { ((FiniteStateMachineNPC)npc.ModNPC).zDepth = value; }
        }
        public virtual void Load() { }

        public virtual void SetDefaults() { }

        public static AIState NewAIState(NPC npc, int type)
        {
            AIState state = (AIState)AIStates[type].MemberwiseClone();
            state.npcID = npc.type;
            state.stateCounter = 0;
            state.npcWhoAmI = npc.whoAmI;
            state.SetDefaults();
            return state;
        }

        public virtual void StatePostDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor
        )
        { }

        public virtual bool StatePreDraw(
            ref DrawData mainSprite,
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor
        )
        { return true; }


        public static int StateType<T>()
            where T : AIState => ModContent.GetInstance<T>().type;

        public static readonly List<AIState> AIStates = new List<AIState>();
        public int stateCounter;

        public Action<AIState> changeState;

        public void ChangeState(int state)
        {
            if(preStateChangeState == -1) 
            {
                npc.aiAction = state;
                changeState.Invoke(this);
            }
            else 
            {
                int overrideState = preStateChangeState;
                preStateChangeState = -1;
                npc.aiAction = overrideState;
                changeState.Invoke(this);
            }
        }

        public virtual void OnEntered(int oldState) { }
        /// <summary>
        /// mainly used for polymorphism setup for AIStates
        /// </summary>
        public virtual void OnPostStateUpdate() { }
        /// <summary>
        /// mainly used for polymorphism setup for AIStates, return true to run OnStateUpdate
        /// </summary>
        public virtual bool OnPreStateUpdate() { return true; }
        public virtual void OnStateUpdate(CommonNPCInfo info) { }

        public virtual void OnExited(int newState) { }
        public void Load(Mod mod)
        {
            type = AIStates.Count;
            AIStates.Add(this);
            Load();
        }

        public void Unload() { }

        public bool ChangeStateIfTargetNull(int state)
        {
            if (Target == null)
            {
                ChangeState(state);
                return true;
            }

            return false;
        }
    }
}
