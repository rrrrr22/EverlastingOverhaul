using EverlastingOverhaul.Common.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EverlastingOverhaul.Common.Systems
{
    public class DataCache<T> where T : struct 
    {
        public T[] cache = [];
        public DataCache(int size, T? defaultValue = null) 
        {
            cache = new T[size];
            if(defaultValue.HasValue)
                FillCache(defaultValue.Value);
        }
        public void InsertCache(T value) 
        {
            cache.Push(value);
        }

        public void FillCache(T defaultValue) 
        {
            for (int i = 0; i < cache.Length; i++)
                cache[i] = defaultValue;
        }
    }
}
