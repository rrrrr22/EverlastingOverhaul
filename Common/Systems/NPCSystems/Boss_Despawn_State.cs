using System;
using Microsoft.Xna.Framework;

namespace EverlastingOverhaul.Common.Systems.NPCReworker
{
    public class Boss_Despawn_State : AIState
    {
        public override void OnStateUpdate(CommonNPCInfo info)
        {
            npc.position.Y += 30;
            npc.velocity = Vector2.Zero;
            npc.EncourageDespawn(30);
        }
    }
}
