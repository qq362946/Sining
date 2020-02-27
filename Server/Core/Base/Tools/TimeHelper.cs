using System;

namespace Sining.Tools
{
    public static class TimeHelper
    {
        private static readonly long Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).Ticks;

        public static long Now => (DateTime.Now.Ticks - Epoch) / 10000;
        
        public static long NowSeconds => (DateTime.Now.Ticks - Epoch) / 10000000;
    }
}