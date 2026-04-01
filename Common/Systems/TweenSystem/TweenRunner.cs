using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Systems.TweenSystem
{
    public class TweenRunner : ModSystem
    {
        public List<ITween> tweens = [];
        public override void PreUpdateEntities()
        {
            for (int i = 0; i < tweens.Count; i++) 
            { 
                tweens[i].Update(); 
            }

            tweens.RemoveAll(t => !t.Active);
            
        }
    }
}
