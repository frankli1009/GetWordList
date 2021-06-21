using System;
using System.Threading;

namespace GetWordsStartWithLetter
{
    internal class StatusChecker
    {
        private int invokeCount;
        private int maxCount;

        private static int _totalCount = 0;

        public StatusChecker(int count)
        {
            invokeCount = 0;
            maxCount = count;
        }

        // This method is called by the timer delegate.
        public void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
            Console.WriteLine("{0} Checking status {1,2} [Total count {2}].",
                DateTime.Now.ToString("h:mm:ss.fff"),
                (++invokeCount).ToString(), (++_totalCount).ToString());

            if (invokeCount >= maxCount)
            {
                // Reset the counter and signal the waiting thread.
                invokeCount = 0;
                autoEvent.Set();
            }
        }

        public static void ResetTotalCount()
        {
            _totalCount = 0;
        }
    }
}