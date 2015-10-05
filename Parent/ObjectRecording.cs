using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Data;
using System.Xml;

using AviFile;
using wavfile;

namespace Sigsence.ApplicationElements
{
    public class ObjectRecording
    {
        public ObjectRecording()
        {
            InitObjectRecording();
        }

        public ObjectRecording(string _recordWrapFile)
        {
            recordingWrapFile = _recordWrapFile;
            InitObjectRecording();
        }

        public ObjectRecording(string filename, string filenameTemp, string sequenceName ,string _dataType)
        {
            InitObjectRecording();
            recordFileName = filename;
            tempFile = filenameTemp;
            dataType = _dataType;
            oSequenceParentName = sequenceName;
            AddRowInitTbResource();
            //
            // Initiate CreateProjectRecording on each heritance object
            //
        }

        //parent
        private void InitObjectRecording()
        {
            tbResourceFile = new DataTable();
            tbResourceFile.Columns.Add("file_name");
            tbResourceFile.Columns.Add("file_extension");
            tbResourceFile.Columns.Add("data_type");//2D-signal, video
            tbResourceFile.Columns.Add("file_temp");
            tbResourceFile.Columns.Add("ObjectSequence");
        }

        protected void AddRowInitTbResource()
        {
            DataRow row = tbResourceFile.NewRow();
            row["file_name"] = recordFileName;
            switch (dataType)
            {
                case "video":
                    row["file_extension"] = ".avi";
                    break;
                case "signal":
                    row["file_extension"] = ".wav";
                    break;
            }
            row["data_type"] = dataType;
            row["file_temp"] = tempFile;
            row["ObjectSequence"] = oSequenceParentName;
            tbResourceFile.Rows.Add(row);
        }

        #region method

        //parent - abstract - to be override in child
        protected virtual void CreateProjectRecording()
        {
               
        } 

        //parent - abstract class - to be override in child
        public virtual void AddBitmapToTempFile(Bitmap bmp)
        {

        }

 

        #endregion

        #region properties

        //parent
        protected string recordingWrapFile;
        public string RecordingWrapFile
        {
            get
            {
                return recordingWrapFile;
            }
            set
            {
                recordingWrapFile = value;
            }
        }

        //parent
        protected DateTime recordDate;
        public DateTime RecordDate
        {
            get
            {
                return recordDate;
            }
            set
            {
                recordDate = value;
            }
        }

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

        //parent-general
        protected string recordFileName;
        public string RecordFileName
        {
            get
            {
                return recordFileName;
            }
            set
            {
                recordFileName = value;
                CreateProjectRecording();
            }
        }

        /// <summary>
        /// video or signal
        /// </summary>
        protected string dataType;
        public string DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;
            }
        }

        //parent
        protected DataTable tbResourceFile;
        public DataTable TbResourceFile
        {
            get
            {
                return tbResourceFile;
            }
            set
            {
                tbResourceFile = value;
            }
        }

        //parent - general
        protected string tempFile;
        public string TempFile
        {
            get
            {
                return tempFile;
            }
            set
            {
                tempFile = value;
            }
        }

        //parent (change name to sample rate)
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

        //parent
        protected short bitSample;
        public short BitSample
        {
            get
            {
                return bitSample;
            }
            set
            {
                bitSample = value;
            }
        }


        //parent
        protected bool isStarted;
        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
            set
            {
                isStarted = value;
            }
        }

        //parent
        protected bool isSampled;
        public bool IsSampled
        {
            get
            {
                return isSampled;
            }
            set
            {
                isSampled = value;
            }
        }

       

        //parent (change name only tempFileStream)
        protected FileStream tempFileStream;
        public FileStream TempFileStream
        {
            get
            {
                return tempFileStream;
            }
            set
            {
                tempFileStream = value;
            }
        }

        //parent
        protected Int32 bmpLength;
        public Int32 BmpLength
        {
            get
            {
                return bmpLength;
            }
            set
            {
                bmpLength = value;
            }
        }

        protected string oSequenceParentName;
        public string OSequenceParentName
        {
            get
            {
                return oSequenceParentName;
            }
            set
            {
                oSequenceParentName = value;
            }
        }
        
        #endregion

        
    }
}
