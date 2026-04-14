using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.NPCsOverhaul
{
    public class NPCPlayer : ModPlayer
    {
        
        public bool fightingDuke = false;
        public override void ResetEffects()
        {
            fightingDuke = false;
        }


    }
}
