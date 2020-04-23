using System;
using System.Collections.Generic;
using Sining.Tools;

namespace Sining
{
    public class ListComponent<T> : IDisposable
    {
        public readonly List<T> List = new List<T>();

        public void Dispose()
        {
            ObjectPool<ListComponent<T>>.Return(this);
            List.Clear();
        }
    }
}