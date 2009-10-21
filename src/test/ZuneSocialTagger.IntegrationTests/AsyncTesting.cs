using System.Threading;
using NUnit.Framework;

namespace ZuneSocialTagger.IntegrationTests
{
    /// <summary>
    /// Provies a wrapper around AutoResetEvent which blocks the current thread until another one has finished
    /// Useful for testing events where you want to wait for a event to complete before asserting
    /// </summary>
    public class AsyncTesting
    {
        private readonly AutoResetEvent _autoResetEvent;

        public AsyncTesting()
        {
            _autoResetEvent = new AutoResetEvent(false);
        }

        public bool Set()
        {
            return _autoResetEvent.Set();
        }

        public bool WaitOne(int milisecondsTimeout,bool exitContext)
        {
            return _autoResetEvent.WaitOne(milisecondsTimeout, exitContext);
        }

        public bool WaitOne(int milisecondsTimeout)
        {
            return _autoResetEvent.WaitOne(milisecondsTimeout);
        }

        public void WaitOneWith500MsTimeoutAndDefaultFailMessage()
        {
            if (!WaitOne(500, false))
                Assert.Fail("test timed out");
        }

        public void WaitOneWith500MsTimeoutAnd(string message)
        {
            if (!WaitOne(500, false))
                Assert.Fail(message);
        }

        protected void WaitOne(int timeout, string message)
        {
            if (!WaitOne(timeout, false))
                Assert.Fail(message);
        }
    }
}