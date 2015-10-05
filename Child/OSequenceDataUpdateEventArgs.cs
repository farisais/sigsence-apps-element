using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sigsence.ApplicationElements
{
    public class OSequenceDataUpdateEventArgs: EventArgs
    {
        public OSequenceDataUpdateEventArgs(double[] data)
        {
            dataUpdate = data;
        }

        private double[] dataUpdate;
        public double[] DataUpdate
        {
            get
            {
                return dataUpdate;
            }
            set
            {
                dataUpdate = value;
            }
        }
    }
}
