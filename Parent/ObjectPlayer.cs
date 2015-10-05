using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using MicroLibrary;

namespace Sigsence.ApplicationElements
{
    public class ObjectPlayer
    {

        public delegate void DataUpdate(object sender, OSequenceDataUpdateEventArgs e);
        public event DataUpdate UpdateData;

        /// <summary>
        /// Constructor of this class
        /// </summary>
        /// <param name="_fileName">filename that this object calls</param>
        /// <param name="_recordTimerStart">Recording time start from the file</param>
        /// <param name="_recordTimerEnd">Recording time end from the file</param>
        public ObjectPlayer(string _fileName, DateTime _recordTimerStart, DateTime _recordTimerEnd)
        {
            fileName = _fileName;
            recordTimeStart = _recordTimerStart;
            recordTimeEnd = _recordTimerEnd;
            InitComponent();
        }

        /// <summary>
        /// Component inisiation function
        /// </summary>
        private void InitComponent()
        {
            playerTimer = new MicroTimer(1);
            playerTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(playerTimer_MicroTimerElapsed);
        }

        /// <summary>
        /// Event raise when the time has elapsed
        /// </summary>
        /// <param name="sender">Object that send the event</param>
        /// <param name="e">Argument of the event that passed</param>
        void playerTimer_MicroTimerElapsed(object sender, MicroTimerEventArgs e)
        {
            //add some logic here to get data every 1ms!
            currentPlayerTime = Convert.ToDateTime(e.TimerCount);
            dataBuffer = UpdateDataBuffer(elapsedTime);
            OnDataChange(this, new OSequenceDataUpdateEventArgs(dataBuffer));
        }

        /// <summary>
        /// Function dedicated for external call to access the internal event raise function
        /// </summary>
        /// <param name="sender">The external sender of the event</param>
        /// <param name="e">External argument that passed</param>
        protected void OnDataChange(object sender, OSequenceDataUpdateEventArgs e)
        {
            UpdateData(sender, e);
        }

        /// <summary>
        /// Function to update the buffer data
        /// </summary>
        /// <param name="elapsed">Elapsed time</param>
        /// <returns>Return double type array of the new data from the file reader</returns>
        protected virtual double[] UpdateDataBuffer(int elapsed)
        {
            double[] result = null;
            return result;
        }

        /// <summary>
        /// Span time for every raising event timer
        /// </summary>
        protected int elapsedTime;
        public int ElapsedTime
        {
            get
            {
                return elapsedTime;
            }
            set
            {
                elapsedTime = value;
            }
        }

        /// <summary>
        /// Get or Set the current time that this player reference to
        /// </summary>
        protected DateTime currentPlayerTime;
        public DateTime CurrentPlayerTime
        {
            get
            {
                return currentPlayerTime;
            }
            set
            {
                currentPlayerTime = value;
            }
        }

        /// <summary>
        /// Name of the file that this player get the data from
        /// </summary>
        protected string fileName;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        /// <summary>
        /// actual time that this player start read the data
        /// </summary>
        protected DateTime recordTimeStart;
        public DateTime RecordTimeStart
        {
            get
            {
                return recordTimeStart;
            }
            set
            {
                recordTimeStart = value;
            }
        }

        /// <summary>
        /// actual end time from the file that this player reference to
        /// </summary>
        protected DateTime recordTimeEnd;
        public DateTime RecordTimeEnd
        {
            get
            {
                return recordTimeEnd;
            }
            set
            {
                recordTimeEnd = value;
            }
        }

        /// <summary>
        /// Timer engine to give a clock to read the data
        /// </summary>
        protected MicroTimer playerTimer;
        public MicroTimer PlayerTimer
        {
            get
            {
                return playerTimer;
            }
            set
            {
                playerTimer = value;
            }
        }

        /// <summary>
        /// Local stored data read from the player with size of 1ms according to the engine timer
        /// </summary>
        protected double[] dataBuffer;
        public double[] DataBuffer
        {
            get
            {
                return dataBuffer;
            }
            set
            {
                dataBuffer = value;
            }
        }

        /// <summary>
        /// Object sequence that this object get the data from
        /// </summary>
        protected ObjectSequence sequenceCorelate;
        public ObjectSequence SequenceCorelate
        {
            get
            {
                return sequenceCorelate;
            }
            set
            {
                sequenceCorelate = value;
            }
        }

        /// <summary>
        /// Object indicator that this player object show the data to
        /// </summary>
        protected ObjectIndicator indicatorPlayer;
        public ObjectIndicator IndicatorPlayer
        {
            get
            {
                return indicatorPlayer;
            }
            set
            {
                indicatorPlayer = value;
            }
        }

        /// <summary>
        /// Sample Rate of the current file
        /// </summary>
        protected Int32 sampleRate;
        public Int32 SampleRate
        {
            get
            {
                return sampleRate;
            }
            set
            {
                sampleRate = value;
            }
        }

        /// <summary>
        /// Bits per sample of the current file
        /// </summary>
        protected int bitsPerSample;
        public int BitsPerSample
        {
            get
            {
                return bitsPerSample;
            }
            set
            {
                bitsPerSample = value;
            }
        }
    }
}
