using System;

namespace Sining.Tools
{
    public static class TimeHelper
    {
        private const long Epoch = 621355968000000000L;
        public static long Now => (DateTime.Now.ToUniversalTime().Ticks - Epoch) / 10000;
        public static int NowSeconds => Convert.ToInt32((DateTime.Now.ToUniversalTime().Ticks - Epoch) / 10000000);
        public static long Transition(DateTime dateTime)
        {   
            return (dateTime.ToUniversalTime().Ticks - Epoch) / 10000;
        }
        public static long TransitionToSeconds(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - Epoch) / 10000000;
        }
        public static DateTime Transition(long timespan)
        {   
            return new DateTime(Epoch + timespan * 10000).ToUniversalTime();
        }
        public static DateTime TransitionToSeconds(long timespan)
        {
            return new DateTime(Epoch + timespan * 10000000).ToUniversalTime();
        }
    }
}