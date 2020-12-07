using System;
using System.Threading;

namespace AudioCPURGB.RGBCreator
{
    abstract class IRGBCreator : IDisposable
    {
        private ManualResetEvent _pauseEvent;
        protected RGBOutput.IRGBOutput _rgbOutput;
        private Thread _workerThread;
        private static Mutex _callback_mutex = new Mutex();

        public IRGBCreator()
        {
            _pauseEvent = new ManualResetEvent(false);         
        }

        public virtual void Initialize()
        {
            // Create a new Thread
            _workerThread = new Thread(WorkerThread)
            {
                IsBackground = true
            };

            Pause(); // Don't let the thread run
            _workerThread.Start(); // But start it (until it comes to the pauseEvent)
        }

        /// <summary>
        /// Thread initiated by object instantiation but is paused until called by start and holt again by pause
        /// </summary>
        private void WorkerThread()
        {
            while (true)
            {                
                _pauseEvent.WaitOne();
                if (_rgbOutput != null && _rgbOutput.IsEnabled())
                {
                    _callback_mutex.WaitOne();
                    Callback();
                    _callback_mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Callback To be implemented which is called by pausable worker_thread
        /// </summary>
        protected abstract void Callback();

        public virtual void SetRGBOutput(RGBOutput.IRGBOutput rgbOutput)
        {
            _rgbOutput = rgbOutput;
        }

        public virtual void Start()
        {
            _pauseEvent.Set();
        }

        public virtual void Pause()
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

        static private RGBValue GetNextFadeIteration(RGBValue oldValue, RGBValue newValue)
        {
            int rFactor = 1;
            int gFactor = 1;
            int bFactor = 1;
            byte temp_r = oldValue.R, temp_g = oldValue.G, temp_b = oldValue.B;

            // Look if decrement or increment
            if (oldValue.R > newValue.R)
            {
                rFactor = -1;
            }
            if (oldValue.G > newValue.G)
            {
                gFactor = -1;
            }
            if (oldValue.B > newValue.B)
            {
                bFactor = -1;
            }

            if (oldValue.R != newValue.R)
            {
                temp_r += (byte)rFactor;
            }
            if (oldValue.G != newValue.G)
            {
                temp_g += (byte)gFactor;
            }
            if (oldValue.B != newValue.B)
            {
                temp_b += (byte)bFactor;
            }
            return new RGBValue(temp_r, temp_g, temp_b);
        }

        public void Fade(RGBValue[] oldValues, RGBValue[] newValues, int fade_time_ms = 50)
        {            
            while (!RGBValue.Equals(oldValues, newValues))
            {
                for (int i = 0; i < newValues.Length; i++)
                {
                    oldValues[i] = GetNextFadeIteration(oldValues[i], newValues[i]);
                }
                _rgbOutput.ShowRGBs(oldValues);
                Thread.Sleep(fade_time_ms);
            }
        }

        public void Fade(RGBValue oldValue, RGBValue newValue, int fade_time_ms = 50)
        {
            RGBValue lastRGB = new RGBValue();
            lastRGB.CopyValues(oldValue);

            while (!lastRGB.Equals(newValue))
            {
                lastRGB = GetNextFadeIteration(lastRGB, newValue);

                _rgbOutput.ShowRGB(lastRGB);
                Thread.Sleep(fade_time_ms);
            }
        }
    }

    [Serializable()]
    public class RGBCreatorException : Exception
    {
        public RGBCreatorException()
        {
            // Add any type-specific logic, and supply the default message.
        }

        public RGBCreatorException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        public RGBCreatorException(string message, Exception innerException) :
           base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        protected RGBCreatorException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }

    [Serializable()]
    public class InitializationException : RGBCreatorException
    {
        public InitializationException()
        {
            // Add any type-specific logic, and supply the default message.
        }

        public InitializationException(string message) : base(message)
        {
            // Add any type-specific logic.
        }

        public InitializationException(string message, Exception innerException) :
           base(message, innerException)
        {
            // Add any type-specific logic for inner exceptions.
        }

        protected InitializationException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            // Implement type-specific serialization constructor logic.
        }
    }
}
