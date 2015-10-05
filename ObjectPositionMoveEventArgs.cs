using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Sigsence.ApplicationElements
{
    public class ObjectPositionMoveEventArgs: EventArgs
    {
        public ObjectPositionMoveEventArgs(Point _position)
        {
            position = _position;
        }

        private Point position;
        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}
