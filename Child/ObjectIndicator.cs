using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Data;

using Sigsence.ApplicationFunction;

namespace Sigsence.ApplicationElements
{
    public class ObjectIndicator : ObjectSignal
    {
        public bool isSnapX = false;
        public bool isSnapY = false;
        public bool isSnapU = false;
        public bool isSnapD = false;
        public bool isSnapL = false;
        public bool isSnapR = false;
        public bool isSnapRD = false;
        public bool isSnapRU = false;
        public bool isSnapLU = false;
        public bool isSnapLD = false;
        public bool isSnapUL = false;
        public bool isSnapUR = false;
        public bool isSnapDR = false;
        public bool isSnapDL = false;

        public ObjectIndicator(object control)
            : base(control)
        {

        }

        public ObjectIndicator()
            : base()
        {

        }

        public void RefreshData()
        {
            AssignSequence();
        }

        protected override void BasicSetupPerCategory()
        {
            controlCategory = ControlCategories.Indicator;
            controlBackColorFocus = SystemColors.ControlDark;
            controlBackColorUnfocus = SystemColors.InfoText;

            tbControlSnap = new DataTable();
            DataColumn col = new DataColumn("control", typeof(ObjectIndicator));
            tbControlSnap.Columns.Add(col);
            tbControlSnap.Columns.Add("position");
        }

        public void initContextMenuStrip()
        {
            ToolStripMenuItem deleteToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem inputToolStripMenuItem = new ToolStripMenuItem();
            
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // inputToolStripMenuItem
            // 

            ToolStripMenuItem signalGeneratorToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem fFTToolStripMenuItem = new ToolStripMenuItem();

            // 
            // signalGeneratorToolStripMenuItem
            // 
            signalGeneratorToolStripMenuItem.Name = "signalGeneratorToolStripMenuItem";
            signalGeneratorToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            signalGeneratorToolStripMenuItem.Text = "Signal Generator";
            // 
            // fFTToolStripMenuItem
            // 
            fFTToolStripMenuItem.Name = "fFTToolStripMenuItem";
            fFTToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            fFTToolStripMenuItem.Text = "Power Spectrum";

            inputToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            signalGeneratorToolStripMenuItem,
            fFTToolStripMenuItem});
            inputToolStripMenuItem.Name = "inputToolStripMenuItem";
            inputToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            inputToolStripMenuItem.Text = "Input";

            indicatorMenuStrip = new ContextMenuStrip();
            indicatorMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            deleteToolStripMenuItem,
            inputToolStripMenuItem});
            indicatorMenuStrip.Name = "contextMenuStripIndicator";
            indicatorMenuStrip.Size = new System.Drawing.Size(108, 48);

            ((Control)controlHandle).ContextMenuStrip = indicatorMenuStrip;
            indicatorMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(indicatorMenuStrip_Opening);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteIndicatoronPanel(WorkingEnvironmentFunction.GetSelectedControlIndex(tbControlPanelAssignment), (Control)controlHandle);
        }

        public void DeleteIndicatoronPanel(int indexIndicator, Control control)
        {
            WorkingEnvironmentFunction.RemoveItemDatatableProperties(indexIndicator, tbControlPanelAssignment);
            WorkingEnvironmentFunction.UnSubscribeObjectTreeview(controlName, controlCategory, treeViewSolutionList);
            panelIndicator.Controls.Remove(control);
            control.Dispose();
            WorkingEnvironmentFunction.DisposingObject(this);
        }

        void indicatorMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToolStripMenuItem itemMenu = indicatorMenuStrip.Items[1] as ToolStripMenuItem;
            if (itemMenu.DropDownItems.Count > 2)
            {
                int itemsTotal = itemMenu.DropDownItems.Count;
                for (int i = itemsTotal - 1 ; i >= 2; i--)
                {
                    itemMenu.DropDownItems.RemoveAt(i);                   
                }
            }

            int count = 0;
            for (int i = 0; i < tbControlPanelAssignment.Rows.Count; i++)
            {
                if (((ObjectSignal)tbControlPanelAssignment.Rows[i]["control"]).ControlCategory == ControlCategories.Sequence)
                {
                    itemMenu.DropDownItems.Add(((ObjectSequence)tbControlPanelAssignment.Rows[i]["control"]).ControlName);
                    itemMenu.DropDownItems[count + 2].Click += new EventHandler(ObjectIndicator_Click);
                    count++;
                }
                
            }
        }

        void ObjectIndicator_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem itemClick = sender as ToolStripMenuItem;

            ObjectSequence result = null;

            for (int i = 0; i < tbControlPanelAssignment.Rows.Count; i++)
            {
                if (((ObjectSignal)tbControlPanelAssignment.Rows[i]["control"]).ControlCategory == ControlCategories.Sequence)
                {
                    if (((ObjectSequence)tbControlPanelAssignment.Rows[i]["control"]).ControlName == itemClick.Text)
                    {
                        result = (ObjectSequence)tbControlPanelAssignment.Rows[i]["control"];
                        break;
                    }
                }
            }
            SequenceSource = result;
        }

        protected override void Control_Click(object sender, EventArgs e)
        {
            base.Control_Click(sender, e);
        }

        protected override void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (controlMouseDown)
            {
                switch (CursorPos)
                {
                    case 1:
                        posGraphX = posGraphX + (e.X - clickX);
                        posGraphY = posGraphY + (e.Y - clickY);
                        int tempPosGraphX = posGraphX;
                        int tempPosGraphY = posGraphY;
                        List<int> tempLoc = ControlSnap(posGraphX, posGraphY);
                        posGraphX = tempLoc[0];
                        posGraphY = tempLoc[1];
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        ControlWidth = controlWidth - ((e.X - clickX) + (posGraphX - tempPosGraphX));
                        ControlHeight = controlHeight - ((e.Y - clickY) + (posGraphY - tempPosGraphY));
                        break;
                    case 2:
                        posGraphY = posGraphY + (e.Y - clickY);
                        int tempPosGraphY2 = posGraphY;
                        List<int> tempLoc2 = ControlSnap(posGraphX, posGraphY);
                        posGraphY = tempLoc2[1];
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        ControlHeight = controlHeight - ((e.Y - clickY) + (posGraphY - tempPosGraphY2));
                        break;
                    case 3:
                        posGraphY = posGraphY + (e.Y - clickY);
                        int tempPosGraphY3 = posGraphY;
                        List<int> tempLoc3 = ControlSnap((posGraphX + (e.X - clickX)), posGraphY);
                        posGraphY = tempLoc3[1];
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        ControlWidth = controlWidth + (tempLoc3[0] - posGraphX);
                        ControlHeight = controlHeight - ((e.Y - clickY) + (posGraphY - tempPosGraphY3));
                        clickX = ControlWidth;
                        break;
                    case 4:
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        List<int> tempLoc4 = ControlSnap((posGraphX + (e.X - clickX)), posGraphY);
                        ControlWidth = controlWidth + (tempLoc4[0] - posGraphX);
                        clickX = ControlWidth;
                        break;
                    case 5:
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        List<int> tempLoc5 = ControlSnap((posGraphX + (e.X - clickX)), (posGraphY + (e.Y - clickY)));
                        ControlWidth = controlWidth + (tempLoc5[0] - posGraphX);
                        ControlHeight = controlHeight + (tempLoc5[1] - posGraphY);
                        clickX = ControlWidth;
                        clickY = ControlHeight;
                        break;
                    case 6:
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        List<int> tempLoc6 = ControlSnap(posGraphX, (posGraphY + (e.Y - clickY)));
                        ControlHeight = controlHeight + (tempLoc6[1] - posGraphY);
                        clickY = ControlHeight;
                        break;
                    case 7:
                        posGraphX = posGraphX + (e.X - clickX);
                        int tempPosGraphX7 = posGraphX;
                        List<int> tempLoc7 = ControlSnap(posGraphX, (posGraphY + (e.Y - clickY)));
                        posGraphX = tempLoc7[0];
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        ControlWidth = controlWidth - ((e.X - clickX) + (posGraphX - tempPosGraphX7));
                        ControlHeight = controlHeight + (tempLoc7[1] - posGraphY);
                        clickY = ControlHeight;
                        break;
                    case 8:
                        posGraphX = posGraphX + (e.X - clickX);
                        int tempPosGraphX8 = posGraphX;
                        List<int> tempLoc8 = ControlSnap(posGraphX, posGraphY);
                        posGraphX = tempLoc8[0];
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        ControlWidth = controlWidth - ((e.X - clickX) + (posGraphX - tempPosGraphX8));
                        break;
                    case 9:
                        posGraphX = posGraphX + (e.X - clickX);
                        posGraphY = posGraphY + (e.Y - clickY);
                        List<int> tempLoc9 = ControlSnap(posGraphX, posGraphY);
                        posGraphX = tempLoc9[0];
                        posGraphY = tempLoc9[1];
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        break;
                }
            }
            else
            {
                CursorPos = GetCursorSizePos(e.X, e.Y);
            }
        }

        private void addControlSnap(ObjectIndicator objSignal, string position)
        {
            DataRow rowadd = tbControlSnap.NewRow();
            rowadd["control"] = objSignal;
            rowadd["position"] = position;
            tbControlSnap.Rows.Add(rowadd);
        }

        private bool isObjSnap(ObjectIndicator indicator, string position)
        {
            bool result = false;
            for (int i = 0; i < tbControlSnap.Rows.Count; i++)
            {
                if (((ObjectIndicator)tbControlSnap.Rows[i]["control"]).controlName == indicator.controlName)
                {
                    if((tbControlSnap.Rows[i]["position"].ToString() == position))
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public void UnSnapControl(ObjectIndicator indicator, string position)
        {
            int index = -1;
            for (int i = 0; i < tbControlSnap.Rows.Count; i++)
            {
                if (((ObjectIndicator)tbControlSnap.Rows[i]["control"]).controlName == indicator.controlName)
                {
                    if ((tbControlSnap.Rows[i]["position"].ToString() == position))
                    {
                        index = i;
                        break;
                    }
                }
            }
            if (index > -1)
            {
                tbControlSnap.Rows.RemoveAt(index);
            }
        }

        public bool checkControlSnapByControl(string posSnap, ObjectIndicator obIndicator)
        {
            for (int i = 0; i < tbControlSnap.Rows.Count; i++)
            {
                if (tbControlSnap.Rows[i]["position"].ToString() == posSnap)
                {
                    if (((ObjectIndicator)tbControlSnap.Rows[i]["control"]).controlName != obIndicator.controlName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void UnsnapAll(string posSnap)
        {
            List<ObjectIndicator> ListControlSnap = new List<ObjectIndicator>();
            List<int> Index = new List<int>();
            for (int i = 0; i < tbControlSnap.Rows.Count; i++)
            {
                if (tbControlSnap.Rows[i]["position"].ToString() == posSnap)
                {
                    ListControlSnap.Add((ObjectIndicator)tbControlSnap.Rows[i]["control"]);
                    Index.Add(i);
                }
            }
            for (int i = 0; i < Index.Count; i++)
            {
                tbControlSnap.Rows.RemoveAt(Index[i]);
            }
            for (int i = 0; i < ListControlSnap.Count; i++)
            {
                ListControlSnap[i].UnSnapControl(this, "R");
            }
        }

        public int CountControlSnap(string posSnap)
        {
            int count = 0;
            for (int i = 0; i < tbControlSnap.Rows.Count; i++)
            {
                if (tbControlSnap.Rows[i]["position"].ToString() == posSnap)
                {
                    count++;
                }
            }
            return count;
        }

        private List<int> ControlSnap(int xLoc, int yLoc)
        {
            List<int> cLoc = new List<int>();
            cLoc.Add(xLoc);
            cLoc.Add(yLoc);
            for (int i = 0; i < tbControlPanelAssignment.Rows.Count; i++)
            {
                if (((ObjectSignal)tbControlPanelAssignment.Rows[i]["control"]).ControlCategory == ControlCategories.Indicator)
                {
                    ObjectIndicator ObIndicator = (ObjectIndicator)tbControlPanelAssignment.Rows[i]["control"];
                    if (ObIndicator.controlName != this.controlName)
                    {
                        //Check X Axis
                        if (Math.Abs(xLoc - (ObIndicator.controlLocation.X + ObIndicator.controlWidth)) < 20)
                        {
                            if ((yLoc < (ObIndicator.controlLocation.Y + ObIndicator.controlHeight)) && (yLoc > ObIndicator.controlLocation.Y - this.controlHeight))
                            {
                                cLoc[0] = (ObIndicator.controlLocation.X + ObIndicator.controlWidth);
                                if (Math.Abs(yLoc - ObIndicator.controlLocation.Y) < 20)
                                {
                                    cLoc[1] = ObIndicator.controlLocation.Y;
                                }
                                else if ((Math.Abs((ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (yLoc + this.controlHeight))) < 20)
                                {
                                    cLoc[1] = (ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (this.controlHeight);
                                }
                            }
                        }

                        if (Math.Abs(ObIndicator.controlLocation.X - (xLoc + this.controlWidth)) < 20)
                        {
                            if ((yLoc < (ObIndicator.controlLocation.Y + ObIndicator.controlHeight))
                                && (yLoc > ObIndicator.controlLocation.Y - this.controlHeight))
                            {
                                cLoc[0] = ObIndicator.controlLocation.X - (this.controlWidth);
                                if (Math.Abs(yLoc - ObIndicator.controlLocation.Y) < 20)
                                {
                                    cLoc[1] = ObIndicator.controlLocation.Y;
                                }
                                else if ((Math.Abs((ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (yLoc + this.controlHeight))) < 20)
                                {
                                    cLoc[1] = (ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (this.controlHeight);
                                }
                            }
                        }
                        else
                        {
                            isSnapR = false;
                        }
                        //Check Y Axis
                        //Check control above
                        if (Math.Abs(yLoc - (ObIndicator.controlLocation.Y + ObIndicator.controlHeight)) < 20)
                        {
                            if ((xLoc < (ObIndicator.controlLocation.X + ObIndicator.controlWidth)) && (xLoc > ObIndicator.controlLocation.X - this.controlWidth))
                            {
                                cLoc[1] = (ObIndicator.controlLocation.Y + ObIndicator.controlHeight);
                                if (Math.Abs(xLoc - ObIndicator.controlLocation.X) < 20)
                                {
                                    cLoc[0] = ObIndicator.controlLocation.X;
                                }
                                else if (Math.Abs((ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (xLoc + this.controlWidth)) < 20)
                                {
                                    cLoc[0] = (ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (this.controlWidth);
                                }

                            }
                        }
                        //Check control below
                        if (Math.Abs(ObIndicator.controlLocation.Y - (yLoc + this.controlHeight)) < 20)
                        {
                            if ((xLoc < (ObIndicator.controlLocation.X + ObIndicator.controlWidth)) && (xLoc > ObIndicator.controlLocation.X - this.controlWidth))
                            {
                                cLoc[1] = ObIndicator.controlLocation.Y - (this.controlHeight);
                                if (Math.Abs(xLoc - ObIndicator.controlLocation.X) < 20)
                                {
                                    cLoc[0] = ObIndicator.controlLocation.X;
                                    isSnapDL = true;
                                }
                                else
                                {
                                    isSnapDL = false;
                                }

                                if (Math.Abs((ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (xLoc + this.controlWidth)) < 20)
                                {
                                    cLoc[0] = (ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (this.controlWidth);
                                    isSnapDR = true;
                                }
                                else
                                {
                                    isSnapDR = false;
                                }
                            }
                        }
                    }
                }
            }
            return cLoc;
        }
        private List<int> ControlSnapX(int xLoc, int yLoc)
        {
            List<int> cLoc = new List<int>();
            cLoc.Add(xLoc);
            cLoc.Add(yLoc);
            for (int i = 0; i < tbControlPanelAssignment.Rows.Count; i++)
            {
                if (((ObjectSignal)tbControlPanelAssignment.Rows[i]["control"]).ControlCategory == ControlCategories.Indicator)
                {
                    ObjectIndicator ObIndicator = (ObjectIndicator)tbControlPanelAssignment.Rows[i]["control"];
                    if (ObIndicator.controlName != this.controlName)
                    {
                        //Check X Axis
                        if (Math.Abs(xLoc - (ObIndicator.controlLocation.X + ObIndicator.controlWidth)) < 20)
                        {
                            if ((yLoc < (ObIndicator.controlLocation.Y + ObIndicator.controlHeight)) && (yLoc > ObIndicator.controlLocation.Y - this.controlHeight))
                            {

                                cLoc[0] = (ObIndicator.controlLocation.X + ObIndicator.controlWidth);

                                if (!isObjSnap(ObIndicator, "L"))
                                {
                                    addControlSnap(ObIndicator, "L");
                                    ObIndicator.addControlSnap(this, "R");
                                }


                                if (isObjSnap(ObIndicator, "L"))
                                {
                                    if (Math.Abs(yLoc - ObIndicator.controlLocation.Y) < 20)
                                    {
                                        cLoc[1] = ObIndicator.controlLocation.Y;
                                        if (!isObjSnap(ObIndicator, "U"))
                                        {
                                            addControlSnap(ObIndicator, "U");
                                            ObIndicator.addControlSnap(this, "U");
                                        }
                                    }
                                    else if ((Math.Abs((ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (yLoc + this.controlHeight))) < 20)
                                    {
                                        cLoc[1] = (ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (this.controlHeight);
                                        if (!isObjSnap(ObIndicator, "D"))
                                        {
                                            addControlSnap(ObIndicator, "D");
                                            ObIndicator.addControlSnap(this, "D");
                                        }
                                    }
                                    else
                                    {
                                        if (isObjSnap(ObIndicator, "U"))
                                        {
                                            UnSnapControl(ObIndicator, "U");
                                            ObIndicator.UnSnapControl(this, "U");
                                        }
                                        else if (isObjSnap(ObIndicator, "D"))
                                        {
                                            UnSnapControl(ObIndicator, "D");
                                            ObIndicator.UnSnapControl(this, "D");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isObjSnap(ObIndicator, "L"))
                            {
                                UnSnapControl(ObIndicator, "L");
                                ObIndicator.UnSnapControl(this, "R");
                            }
                        }

                        if (Math.Abs(ObIndicator.controlLocation.X - (xLoc + this.controlWidth)) < 20)
                        {
                            if ((yLoc < (ObIndicator.controlLocation.Y + ObIndicator.controlHeight)) 
                                && (yLoc > ObIndicator.controlLocation.Y - this.controlHeight))
                            {
                                    if (!checkControlSnapByControl("R", ObIndicator))
                                    {
                                        cLoc[0] = ObIndicator.controlLocation.X - (this.controlWidth);
                                        if (!isObjSnap(ObIndicator, "R"))
                                        {
                                            addControlSnap(ObIndicator, "R");
                                            ObIndicator.addControlSnap(this, "L");
                                        }
                                    }

                                if (isObjSnap(ObIndicator, "R"))
                                {
                                    if (Math.Abs(yLoc - ObIndicator.controlLocation.Y) < 20)
                                    {
                                        cLoc[1] = ObIndicator.controlLocation.Y;
                                        if (!isObjSnap(ObIndicator, "U"))
                                        {
                                            addControlSnap(ObIndicator, "U");
                                            ObIndicator.addControlSnap(this, "U");
                                        }
                                    }
                                    else if ((Math.Abs((ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (yLoc + this.controlHeight))) < 20)
                                    {
                                        cLoc[1] = (ObIndicator.controlLocation.Y + ObIndicator.controlHeight) - (this.controlHeight);
                                        if (!isObjSnap(ObIndicator, "D"))
                                        {
                                            addControlSnap(ObIndicator, "D");
                                            ObIndicator.addControlSnap(this, "D");
                                        }
                                    }

                                    else
                                    {
                                        if (isObjSnap(ObIndicator, "U"))
                                        {
                                            UnSnapControl(ObIndicator, "U");
                                            ObIndicator.UnSnapControl(this, "U");
                                        }
                                        else if (isObjSnap(ObIndicator, "D"))
                                        {
                                            UnSnapControl(ObIndicator, "D");
                                            ObIndicator.UnSnapControl(this, "D");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(isObjSnap(ObIndicator, "R"))
                            {
                                UnSnapControl(ObIndicator, "R");
                                ObIndicator.UnSnapControl(this, "L");
                            }
                        }
                        //Check Y Axis
                        //Check control above
                        if (Math.Abs(yLoc - (ObIndicator.controlLocation.Y + ObIndicator.controlHeight)) < 20)
                        {
                            if ((xLoc < (ObIndicator.controlLocation.X + ObIndicator.controlWidth)) && (xLoc > ObIndicator.controlLocation.X - this.controlWidth))
                            {
                                if (!checkControlSnapByControl("U", ObIndicator))
                                {
                                    cLoc[1] = (ObIndicator.controlLocation.Y + ObIndicator.controlHeight);
                                    if (!isObjSnap(ObIndicator, "U"))
                                    {
                                        addControlSnap(ObIndicator, "U");
                                        ObIndicator.addControlSnap(this, "D");
                                    }
                                }

                                if (isObjSnap(ObIndicator, "U"))
                                {
                                    if (Math.Abs(xLoc - ObIndicator.controlLocation.X) < 20)
                                    {
                                        cLoc[0] = ObIndicator.controlLocation.X;
                                        if (!isObjSnap(ObIndicator, "L"))
                                        {
                                            addControlSnap(ObIndicator, "L");
                                            ObIndicator.addControlSnap(this, "L");
                                        }
                                    }
                                    else if (Math.Abs((ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (xLoc + this.controlWidth)) < 20)
                                    {
                                        cLoc[0] = (ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (this.controlWidth);
                                        if (!isObjSnap(ObIndicator, "R"))
                                        {
                                            addControlSnap(ObIndicator, "R");
                                            ObIndicator.addControlSnap(this, "R");
                                        }
                                    }
                                    else
                                    {
                                        if (isObjSnap(ObIndicator, "R"))
                                        {
                                            UnSnapControl(ObIndicator, "R");
                                            ObIndicator.UnSnapControl(this, "R");
                                        }
                                        else if (isObjSnap(ObIndicator, "L"))
                                        {
                                            UnSnapControl(ObIndicator, "L");
                                            ObIndicator.UnSnapControl(this, "L");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if(isObjSnap(ObIndicator, "U"))
                            {
                                UnSnapControl(ObIndicator, "U");
                                ObIndicator.UnSnapControl(this, "D");
                            }
                        }
                        //Check control below
                        if (Math.Abs(ObIndicator.controlLocation.Y - (yLoc + this.controlHeight)) < 20)
                        {
                            if ((xLoc < (ObIndicator.controlLocation.X + ObIndicator.controlWidth)) && (xLoc > ObIndicator.controlLocation.X - this.controlWidth))
                            {
                                if (!checkControlSnapByControl("D", ObIndicator))
                                {
                                    cLoc[1] = ObIndicator.controlLocation.Y - (this.controlHeight);
                                    if (!isObjSnap(ObIndicator, "D"))
                                    {
                                        addControlSnap(ObIndicator, "D");
                                        ObIndicator.addControlSnap(this, "U");
                                    }
                                }
                                if (isObjSnap(ObIndicator, "D"))
                                {
                                    if (Math.Abs(xLoc - ObIndicator.controlLocation.X) < 20)
                                    {
                                        cLoc[0] = ObIndicator.controlLocation.X;
                                        if (!isObjSnap(ObIndicator, "L"))
                                        {
                                            addControlSnap(ObIndicator, "L");
                                            ObIndicator.addControlSnap(this, "L");
                                        }
                                    }
                                    else if (Math.Abs((ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (xLoc + this.controlWidth)) < 20)
                                    {
                                        cLoc[0] = (ObIndicator.controlLocation.X + ObIndicator.controlWidth) - (this.controlWidth);
                                        if (!isObjSnap(ObIndicator, "R"))
                                        {
                                            addControlSnap(ObIndicator, "R");
                                            ObIndicator.addControlSnap(this, "R");
                                        }
                                    }

                                    else
                                    {
                                        if (isObjSnap(ObIndicator, "L"))
                                        {
                                            UnSnapControl(ObIndicator, "L");
                                            ObIndicator.UnSnapControl(this, "L");
                                        }
                                        else if (isObjSnap(ObIndicator, "R"))
                                        {
                                            UnSnapControl(ObIndicator, "R");
                                            ObIndicator.UnSnapControl(this, "R");
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isObjSnap(ObIndicator, "D"))
                            {
                                UnSnapControl(ObIndicator, "D");
                                ObIndicator.UnSnapControl(this, "U");
                            }
                        }
                    }
                }
            }
            return cLoc;
        }

        protected override void Control_MouseUp(object sender, MouseEventArgs e)
        {
            controlMouseDown = false;
            if (CursorPos == 9)
            {
                ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
            }
        }

        protected override void Control_MouseDown(object sender, MouseEventArgs e)
        {
            base.Control_MouseDown(sender, e);

            if (!controlMouseDown)
            {
                controlMouseDown = true;
                posGraphX = controlLocation.X;
                posGraphY = controlLocation.Y;

                switch (CursorPos)
                {

                    case 1:
                        clickX = 0; 
                        clickY = 0;
                        break;
                    case 2:
                        clickX = e.X;
                        clickY = 0;
                        break;
                    case 3:
                        clickX = controlWidth;
                        clickY = 0;
                        break;
                    case 4:
                        clickX = controlWidth;
                        clickY = e.Y;
                        break;
                    case 5:
                        clickX = controlWidth;
                        clickY = controlHeight;
                        break;
                    case 6:
                        clickX = e.X;
                        clickY = controlHeight;
                        break;
                    case 7:
                        clickX = 0;
                        clickY = controlHeight;
                        break;
                    case 8:
                        clickX = 0;
                        clickY = e.Y;
                        break;
                    case 9:
                        clickX = e.X;
                        clickY = e.Y;
                        ControlLocation = new System.Drawing.Point(posGraphX, posGraphY);
                        break;
                }
            }
        }
        private DataTable tbControlSnap;
        public DataTable TbControlSnap
        {
            get
            {
                return tbControlSnap;
            }
            set
            {
                tbControlSnap = value;
            }
        }

        protected ObjectPlayer playerSource;
        public ObjectPlayer PlayerSource
        {
            get
            {
                return playerSource;
            }
            set
            {
                playerSource = value;
            }
        }

        protected bool onPlayerMode;
        public bool OnPlayerMode
        {
            get
            {
                return onPlayerMode;
            }
            set
            {
                onPlayerMode = value;
            }
        }

        protected virtual void InitPlayerMode()
        {
            
        }
    }
}
