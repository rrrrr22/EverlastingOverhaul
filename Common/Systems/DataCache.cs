using EverlastingOverhaul.Common.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace EverlastingOverhaul.Common.Systems
{
    public class DataCache<T> where T : struct 
    {
        public int size = -1;
        public bool allowArrayResize = false;
        public T[] cache = [];
        public DataCache(int size, T? defaultValue = null, bool allowArrayResize = false) 
        {
            this.size = size;
            this.allowArrayResize = allowArrayResize;
            cache = new T[size];
            if(defaultValue.HasValue)
                FillCache(defaultValue.Value);
        }
        public void InsertCache(T value) 
        {
            cache.Push(value);
        }

        public void ResizeCache(int byHowMuch) 
        {
            if(cache.Length + byHowMuch > 0 && cache.Length + byHowMuch <= size)
                Array.Resize(ref cache, cache.Length + byHowMuch);
        }

        public void PopCache() 
        {
            cache.Pop();
        }

        public void FillCache(T defaultValue) 
        {
            for (int i = 0; i < cache.Length; i++)
                cache[i] = defaultValue;
        }
    }
}
