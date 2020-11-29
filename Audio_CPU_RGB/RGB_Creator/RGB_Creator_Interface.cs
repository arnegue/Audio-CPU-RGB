using System;
using System.Threading;

namespace AudioCPURGB.RGB_Creator
{
    abstract class RGB_Creator_Interface : IDisposable
    {
        private ManualResetEvent _pauseEvent;
        protected RGB_Output.RGB_Output_Interface _rgbOutput;
        private Thread _workerThread;
        private static Mutex _callback_mutex = new Mutex();

        public RGB_Creator_Interface()
        {
            _pauseEvent = new ManualResetEvent(false);
            // Create a new Thread
            _workerThread = new Thread(worker_thread)
            {
                IsBackground = true
            };

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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pauseEvent.Dispose();
            }
            else
            {
                //
            }
        }

        private RGB_Value getNextFadeIteration(RGB_Value oldValue, RGB_Value newValue)
        {
            int rFactor = 1;
            int gFactor = 1;
            int bFactor = 1;
            byte temp_r = oldValue.r, temp_g = oldValue.g, temp_b = oldValue.b;

            // Look if decrement or increment
            if (oldValue.r > newValue.r)
            {
                rFactor = -1;
            }
            if (oldValue.g > newValue.g)
            {
                gFactor = -1;
            }
            if (oldValue.b > newValue.b)
            {
                bFactor = -1;
            }

            if (oldValue.r != newValue.r)
            {
                temp_r += (byte)rFactor;
            }
            if (oldValue.g != newValue.g)
            {
                temp_g += (byte)gFactor;
            }
            if (oldValue.b != newValue.b)
            {
                temp_b += (byte)bFactor;
            }
            return new RGB_Value(temp_r, temp_g, temp_b);
        }

        private bool rgbs_are_equal(RGB_Value[] oldValues, RGB_Value[] newValues)
        {
            if (oldValues.Length != newValues.Length)
            {
                return false;
            }

            for (int i = 0; i < newValues.Length; i++)
            {
                if (!oldValues[i].Equals(newValues[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public void Fade(RGB_Value[] oldValues, RGB_Value[] newValues, int fade_time_ms = 50)
        {
            while (!rgbs_are_equal(oldValues, newValues))
            {
                for (int i = 0; i < newValues.Length; i++)
                {
                    oldValues[i] = getNextFadeIteration(oldValues[i], newValues[i]);
                }
                _rgbOutput.ShowRGBs(oldValues);
                Thread.Sleep(fade_time_ms);
            }
        }

        public void Fade(RGB_Value oldValue, RGB_Value newValue, int fade_time_ms = 50)
        {
            RGB_Value lastRGB = new RGB_Value();
            lastRGB.copy_values(oldValue);

            _rgbOutput.ShowRGB(lastRGB);
            while (!lastRGB.Equals(newValue))
            {
                lastRGB = getNextFadeIteration(lastRGB, newValue);

                _rgbOutput.ShowRGB(lastRGB);
                // Wait a few Millisec to fade to new Color
                Thread.Sleep(fade_time_ms);
            }
        }
    }
}
