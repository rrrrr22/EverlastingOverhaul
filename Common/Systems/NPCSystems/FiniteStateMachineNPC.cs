using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EverlastingOverhaul.Common.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Systems.NPCReworker
{
    public abstract class FiniteStateMachineNPC : ModNPC, IZDepth
    {
        public NPCAIStates states = null;
        public int currentState
        {
            get
            {
                if (states != null)
                    return states.currentState.type;
                return -1;
            }
        }
        public int currentStateCounter => states.currentState.counter;

        public static int StateType<T>()
            where T : AIState => ModContent.GetInstance<T>().type;

        public sealed override void OnSpawn(IEntitySource source)
        {
            zDepth = 1;
            states = new(NPC, RegisterStates());
            PostOnSpawn(source);
        }

        public virtual void PostOnSpawn(IEntitySource source) { }

        public virtual int[] RegisterStates()
        {
            return [];
        }

        public sealed override bool PreAI()
        {
            PreStateUpdate();
            states.Update();
            //zDepth = MathF.Sin((float)Main.timeForVisualEffects * 0.01f) * 2 + 2;

            PostStateUpdate();
            return false;
        }

        public virtual void PreStateUpdate() { }

        public virtual void PostStateUpdate() { }

        public static int NewProjectileWithMPCheck(
            IEntitySource spawnSource,
            Vector2 position,
            Vector2 velocity,
            int Type,
            int Damage,
            float KnockBack,
            int Owner = -1,
            float ai0 = 0f,
            float ai1 = 0f,
            float ai2 = 0f
        )
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return Projectile.NewProjectile(
                    spawnSource,
                    position,
                    velocity,
                    Type,
                    Damage,
                    KnockBack,
                    Owner,
                    ai0,
                    ai1,
                    ai2
                );
            else
                return -1;
        }

        public static int NewNPCWithMPCheck(
            IEntitySource source,
            Vector2 Position,
            int Type,
            int Start = 0,
            float ai0 = 0f,
            float ai1 = 0f,
            float ai2 = 0f,
            float ai3 = 0f,
            int Target = 255
        )
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return NPC.NewNPC(
                    source,
                    (int)Position.X,
                    (int)Position.Y,
                    Type,
                    Start,
                    ai0,
                    ai1,
                    ai2,
                    ai3,
                    Target
                );
            else
                return -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.aiAction);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.aiAction = reader.ReadInt32();
        }

        public float zDepth { get; set; }
    }
}
