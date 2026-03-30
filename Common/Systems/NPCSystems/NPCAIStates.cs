using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader.Config;

namespace EverlastingOverhaul.Common.Systems.NPCReworker
{

    public class NPCAIStates
    {
        public int[] preStateChangeStates = [];
        public AIState currentState = null;
        public int npcWhoAmI = -1;
        private Dictionary<int, AIState> states = [];
        public bool justHitTheGround = false;
        public bool inGround = false;
        public bool justMovedAwayFromTheGround = false;
        public bool afterimages = false;
        public int npcID = -1;
        /// <summary>
        /// Make sure to apply screenPos and drawColor before using it
        /// </summary>
        public DrawData currentSprite = new DrawData();
        public NPC npc
        {
            get
            {
                return Main.npc[npcWhoAmI];
            }
        }
        public NPCAIStates(NPC npc, params int[] states)
        {
            npcWhoAmI = npc.whoAmI;
            npcID = npc.type;
            foreach (int type in states)
            {
                this.states[type] = AIState.NewAIState(npc, type);
                this.states[type].changeState += OnStateChange;
            }
            currentState = this.states[states[0]];
            currentState.OnEntered(-1);
            currentState.stateCounter.Start();
            
            currentState.UpdateCurrentSpriteFrame();
        }
        public void Update()
        {
            if (currentState.OnPreStateUpdate())
            {
                if (justHitTheGround)
                {
                    justHitTheGround = false;

                }

                if (npc.collideY && !inGround)
                {

                    justHitTheGround = true;
                    inGround = true;
                }

                if (justMovedAwayFromTheGround)
                {
                    justMovedAwayFromTheGround = false;
                }

                if (!npc.collideY && inGround)
                {
                    inGround = false;
                    justMovedAwayFromTheGround = true;
                }

                CommonNPCInfo info = new CommonNPCInfo();
                info.justMovedAwayFromTheGround = justMovedAwayFromTheGround;
                info.inGround = inGround;
                info.justHitTheGround = justHitTheGround;
                currentState.OnStateUpdate(info);

            }
            currentState.OnPostStateUpdate();
            currentState.UpdateCurrentSpriteFrame();
            currentSprite = currentState.UpdateCurrentSprite();
        }
        public void OnStateChange(AIState oldState)
        {
            oldState.OnExited(npc.aiAction);
            AIState newState = states[npc.aiAction];
            currentState = newState;
            currentState.stateCounter.Start();
            currentState.OnEntered(oldState.type);

        }

    }

}
