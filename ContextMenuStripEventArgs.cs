using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sigsence.ApplicationElements
{
    public class ContextMenuStripEventArgs: EventArgs
    {
        public ContextMenuStripEventArgs(string description)
        {
            eventDescription = description;
        }

        private string eventDescription;
        public string EventDescription
        {
            get
            {
                return eventDescription;
            }
            set
            {
                eventDescription = value;
            }
        }
    }
}
