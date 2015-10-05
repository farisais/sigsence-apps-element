using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Data;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualBasic.PowerPacks;

using Sigsence.ApplicationFunction;

namespace Sigsence.ApplicationElements
{
    public enum DataTypes
    {
        StaticSignal,
        RealTimeSignal,
        Image,
        Video,
    }

    [Serializable()]
    public class ObjectSequence: ObjectSignal, ISerializable
    {
        public delegate void ObjectPosistionMove(Object sender, ObjectPositionMoveEventArgs e);

        public event ObjectPosistionMove UpdatePosition;

        public delegate void DataUpdate(object sender, OSequenceDataUpdateEventArgs e);
        public event DataUpdate UpdateData;
        
        public ObjectSequence(object control)
            : base(control)
        {

        }

        public ObjectSequence()
            : base()
        {

        }

        protected override void BasicSetupPerCategory()
        {
            controlCategory  = ControlCategories.Sequence;
            controlBackColorFocus = SystemColors.ControlDark;
            controlBackColorUnfocus = SystemColors.Control;
            lineShapeList = new List<LineShape>();
        }

        protected void InitializeIcon(string imagePath)
        {
            PictureBox pictureBox = new PictureBox();
            pictureBox.Image = Image.FromFile(imagePath);
            pictureBox.Location = new System.Drawing.Point(65, 67);
            pictureBox.Name = "pictureBox1";
            pictureBox.Size = new System.Drawing.Size(43, 43);
            pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            pictureBox.Padding = new Padding(3);


            controlHandle = pictureBox;

            InitControl(controlHandle);

            //Label for the sequence icon
            TextBox textboxSequence = new TextBox();
            textboxSequence.Multiline = true;
            textboxSequence.Size = new System.Drawing.Size(103, 14);
            textboxSequence.Location = new System.Drawing.Point(35, 110);
            //textboxSequence.Location = new System.Drawing.Point(35, 43);
            textboxSequence.Text = controlName;
            textboxSequence.TextAlign = HorizontalAlignment.Center;
            textboxSequence.BorderStyle = BorderStyle.None;
            textboxSequence.ReadOnly = true;
            textboxSequence.BackColor = System.Drawing.Color.White;
            sequenceTextbox = textboxSequence;
        }

        protected void OnDataChange(object sender, OSequenceDataUpdateEventArgs e)
        {
            if (UpdateData != null)
            {
                UpdateData(sender, e);
            }
        }

        protected override void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (controlMouseDown)
            {
                posGraphX = posGraphX + (e.X - clickX);
                posGraphY = posGraphY + (e.Y - clickY);
                ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                sequenceTextbox.Location = new Point(posGraphX - 30, posGraphY + 43);
                UpdateLineShapePoints(centerOfGravity);
            }
            else
            {
                CursorPos = GetCursorSizePos(e.X, e.Y);
            }
            //UpdatePosition(this, new ObjectPositionMoveEventArgs(controlLocation));
        }

        protected override void Control_MouseUp(object sender, MouseEventArgs e)
        {
            controlMouseDown = false;
            if (CursorPos == 9)
            {
                ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                sequenceTextbox.Location = new Point(posGraphX - 30, posGraphY + 43);
                UpdateLineShapePoints(centerOfGravity);
            }
            
            //UpdatePosition(this, new ObjectPositionMoveEventArgs(controlLocation));
        }

        protected override void Control_MouseDown(object sender, MouseEventArgs e)
        {
            base.Control_MouseDown(sender, e);

            if (!controlMouseDown)
            {
                controlMouseDown = true;
                posGraphX = controlLocation.X;
                posGraphY = controlLocation.Y;

                clickX = e.X;
                clickY = e.Y;
                ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                sequenceTextbox.Location = new Point(posGraphX - 30, posGraphY + 43);
                UpdateLineShapePoints(centerOfGravity);
                //UpdatePosition(this, new ObjectPositionMoveEventArgs(controlLocation));
            }
        }

        private void ConnectSequence()
        {
            LineShape lineShape = new LineShape(centerOfGravity.X, centerOfGravity.Y, sequenceSource.centerOfGravity.X, sequenceSource.centerOfGravity.Y);
            sequenceSource.lineShapeList.Add(lineShape);
            lineShapeList.Add(lineShape);
            shapeContainer.Shapes.Add(lineShapeList[lineShapeList.Count - 1]);
            shapeContainer.SendToBack();
            lineShapeList[lineShapeList.Count - 1].SendToBack();
            CheckExistingSequenceOutput(this);
            AddDataTbLineSequence(sequenceSource.controlName, lineShape, this.controlName);
        }

        private void AddDataTbLineSequence(string ObIn, LineShape lineAdd, string ObOut)
        {
            DataRow row = tbLineSequence.NewRow();
            row["ObjectIn"] = ObIn;
            row["Line"] = lineAdd;
            row["ObjectOut"] = ObOut;
            TbLineSequence.Rows.Add(row);
        }

        private void CheckExistingSequenceOutput(ObjectSequence ObSequence)
        {
            List<int> index = new List<int>();
            for (int i = 0; i < TbLineSequence.Rows.Count; i++)
            {
                if (TbLineSequence.Rows[i]["ObjectOut"].ToString() == ObSequence.controlName)
                {
                    index.Add(i);
                }
            }

            if (index.Count > 0)
            {
                for (int i = index.Count - 1; i >= 0; i--)
                {
                    DeleteLineShape(((LineShape)TbLineSequence.Rows[index[i]]["Line"]));
                    TbLineSequence.Rows.RemoveAt(index[i]);
                }
            }
        }

        private void DeleteLineShape(LineShape lShapeDelete)
        {
            shapeContainer.Shapes.Remove(lShapeDelete);
        }

        private void UpdateLineShapePoints(Point newLocation)
        {
            for (int i = 0; i < TbLineSequence.Rows.Count; i++)
            {
                if (TbLineSequence.Rows[i]["ObjectIn"].ToString() == this.controlName)
                {
                    ((LineShape)TbLineSequence.Rows[i]["Line"]).X2 = newLocation.X;
                    ((LineShape)TbLineSequence.Rows[i]["Line"]).Y2 = newLocation.Y;
                }

                if (TbLineSequence.Rows[i]["ObjectOut"].ToString() == this.controlName)
                {
                    ((LineShape)TbLineSequence.Rows[i]["Line"]).X1 = newLocation.X;
                    ((LineShape)TbLineSequence.Rows[i]["Line"]).Y1 = newLocation.Y;
                }
            }
        }

        public override ObjectSequence SequenceSource
        {
            get
            {
                return base.SequenceSource;
            }
            set
            {
                base.SequenceSource = value;
                ConnectSequence();
            }
        }
        
        protected List<LineShape> lineShapeList;
        public List<LineShape> LineShapeList
        {
            get
            {
                return lineShapeList;
            }
            set
            {
                lineShapeList = value;
            }
        }

        private double[] signalData;
        public double[] SignalData
        {
            get
            {
                return signalData;
            }
            set
            {
                signalData = value;
                if (UpdateData != null)
                {
                    UpdateData(this, new OSequenceDataUpdateEventArgs(signalData));
                }
            }
        }

        protected ShapeContainer shapeContainer;
        public ShapeContainer ShapeContainer
        {
            get
            {
                return shapeContainer;
            }
            set
            {
                shapeContainer = value;
            }
        }

        protected DataTable tbLineSequence;
        public DataTable TbLineSequence
        {
            get
            {
                return tbLineSequence;
            }
            set
            {
                tbLineSequence = value;
            }
        }

        protected TextBox sequenceTextbox;
        public TextBox SequenceTextbox
        {
            get
            {
                return sequenceTextbox;
            }
            set
            {
                sequenceTextbox = value;
                textboxText = sequenceTextbox.Text;
            }
        }

        protected string textboxText;
        public string TextboxText
        {
            get
            {
                return textboxText;
            }
            set
            {
                textboxText = value;
            }
        }


        #region ISerializable Members

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("textboxText", textboxText);
            info.AddValue("signalData", signalData);
        }

        #endregion
    }
}
