using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AudioCPURGB.RGB_Creator
{
    abstract class RGB_Creator_Interface : IDisposable
    {
        private ManualResetEvent _pauseEvent = new ManualResetEvent(false);
        protected RGB_Output.RGB_Output_Interface _rgbOutput;
        private Thread _workerThread;
        private static Mutex _callback_mutex = new Mutex();

        public RGB_Creator_Interface()
        {
            // Create a new Thread
            _workerThread = new Thread(worker_thread);
            _workerThread.IsBackground = true;

            pause(); // Don't let the thread run
            _workerThread.Start(); // But start it (until it comes to the pauseEvent)
        }

        /// <summary>
        /// Thread initiated by object instantiation but is paused until called by start and holt again by pause
        /// </summary>
        private void worker_thread()
        {
            while (true)
            {                
                _pauseEvent.WaitOne();
                if (_rgbOutput != null && _rgbOutput.IsEnabled())
                {
                    _callback_mutex.WaitOne();
                    callback();
                    _callback_mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Callback To be implemented which is called by pausable worker_thread
        /// </summary>
        protected abstract void callback();

        public virtual void setRGBOutput(RGB_Output.RGB_Output_Interface rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }

        public virtual void start()
        {
            _pauseEvent.Set();
        }

        public virtual void pause()
        {
            _pauseEvent.Reset();
            if (_workerThread.ThreadState == (System.Threading.ThreadState.Unstarted | System.Threading.ThreadState.Background))
            {
                return;
            }
            while (_workerThread.ThreadState != (System.Threading.ThreadState.WaitSleepJoin | System.Threading.ThreadState.Background))
            {
                Thread.Sleep(100);
            }
        }
        void IDisposable.Dispose()
        {
            _pauseEvent.Dispose();
        }
    }
}
