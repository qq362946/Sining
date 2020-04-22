using System;
using System.Collections.Generic;
using Sining.Tools;

namespace Sining
{
    public class DictionaryComponent<TM,TN> : IDisposable
    {
        public readonly Dictionary<TM, TN> Dictionary = new Dictionary<TM, TN>();
        
        public void Dispose()
        {
            Dictionary.Clear();
            
            ObjectPool<DictionaryComponent<TM,TN>>.Return(this);
        }
    }
}