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

using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors.Repository;

using NationalInstruments.UI.WindowsForms;
using NationalInstruments.UI;

using Microsoft.VisualBasic.PowerPacks;

using Sigsence.ApplicationFunction;

namespace Sigsence.ApplicationElements
{
    //public enum ControlCategories
    //{
    //    Sequence = 0,
    //    Indicator = 1
    //}

    //public enum ControlTypeNames
    //{
    //    SignalGenerator = 0,
    //    FFT,
    //    AudioIn,
    //    DeviceNI,
    //    WaveFormGraph,
    //    DigitalWaveFromGraph,
    //    ComplexGraph,
    //    ScatterGraph,
    //    IntensityGraph,
    //    Tank,
    //    Thermometer,
    //    Meter,
    //    Led,
    //    Gauge,
    //    Video
    //}


    //Template class for all function object defined in this application
    [Serializable()]
    public abstract partial class ObjectSignal: ISerializable
    {
        protected int posGraphX = 0;    //Current X position of controlHandle
        protected int posGraphY = 0;    //Current Y position of controlHandle
        protected int clickX = 0;       //Last mouse down position X-Axis
        protected int clickY = 0;       //Last mouse down position Y-Axis
        protected int CursorPos;        //Cursor Type when hovering over the control
     
        protected delegate void UpdateGraphData(double[] data);
        //public delegate void ObjectPosistionMove(Object sender, ObjectPositionMoveEventArgs e);

        //public event ObjectPosistionMove UpdatePosition;

        //Delegate to send the event when control clicked
        public delegate void ObjectClick(object sender, EventArgs e);
        //Event when the control clicked
        public event ObjectClick ObjectClickedEvent;

        //Delegate to send the event when control mouse down
        public delegate void ObjectMouseDown(object sender, EventArgs e);
        //Event to raise when Control Mouse Down
        public event ObjectMouseDown ObjectMouseDownEvent;

        protected delegate void ContextMenuDelegate(object sender, ContextMenuStripEventArgs e);

        protected event ContextMenuDelegate ContextMenuEventRaise;

        /// <summary>
        /// ObjectSignal Class contstructor
        /// </summary>
        /// <param name="control">Control to be handled by this class</param>
        /// <param name="parent">Parent form that hold control handled within ObjectSignal </param>
        public ObjectSignal(object control)
        {
            InitControl(control);
            //parentWindow = parent; //Delete
            objectUserControl = new List<Control>();
            BasicSetupPerCategory();
            ImplementUserControl();
        }

        /// <summary>
        /// ObjectSignal Class contstructor
        /// </summary>
        public ObjectSignal()
        {
            objectUserControl = new List<Control>();
            BasicSetupPerCategory();
            ImplementUserControl();
        }

        /// <summary>
        /// Function to intialize basic setup per category (sequence or indicator)
        /// </summary>
        protected virtual void BasicSetupPerCategory()
        {

        }

        /// <summary>
        /// Function to initialize control inside ObjectSignal
        /// </summary>
        protected virtual void ImplementUserControl()
        {

        }

        /// <summary>
        /// Local variable for parent window
        /// </summary>
        //protected Form1 parentWindow; //Delete
        ///// <summary>
        ///// Public property for parent window
        ///// </summary>
        //public Form1 ParentWindow //Delete
        //{
        //    get
        //    {
        //        return parentWindow;
        //    }
        //    set
        //    {
        //        parentWindow = value;
        //        InitVGridProperties();
        //    }
        //}

        ///// <summary>
        ///// Property of parent user control that handle this object
        ///// </summary>
        //protected UserControlWorkingPanel parentUserControl;
        //public UserControlWorkingPanel ParentUserControl
        //{
        //    get
        //    {
        //        return parentUserControl;
        //    }
        //    set
        //    {
        //        parentUserControl = value;
        //    }
        //}

        /// <summary>
        /// Perform initiation of the control that will be handled by ObjectSignal Class. This method will define 
        /// all the basic properties and store its value on the local properties variable in ObjectSignal
        /// </summary>
        /// <param name="control">Control which will be assigned and initiated</param>
        protected void InitControl(object control)
        {
            //Add control argument passed to control handle
            controlHandle = control;

            //Assign used properties of the control depend on the type of the control
            Type t = control.GetType();
            controlType = t;
           
            //Assign control location on its parent control
            try
            {
                //Get location property info
                PropertyInfo pi = t.GetProperty("Location");
                
                Point Clocation = (Point)pi.GetValue(control, null);
                controlLocation = Clocation;

                //Get width property info
                pi = t.GetProperty("Width");
                int CWidth = (int)pi.GetValue(control, null);
                controlWidth = CWidth;

                //Get Height property info
                pi = t.GetProperty("Height");
                int CHeight = (int)pi.GetValue(control, null);
                controlHeight = CHeight;

                pi = t.GetProperty("Name");
                string CName = (string)pi.GetValue(control, null);
                controlName = CName;

                //Assign center of gravity (center point)
                centerOfGravity = new Point((controlLocation.X + (controlWidth / 2)), (controlLocation.Y + (controlHeight / 2)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Object Signal - Type Cast Error", MessageBoxButtons.OK);
            }

            try
            {
                //Get all event info properties

                //Get mouse_down event of the control
                EventInfo ei = control.GetType().GetEvent("MouseDown");
                MethodInfo mi = this.GetType().GetMethod("Control_MouseDown", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                Delegate del = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                ei.AddEventHandler(control, del);

                //Get mouse_up event of the control
                ei = control.GetType().GetEvent("MouseUp");
                mi = this.GetType().GetMethod("Control_MouseUp", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                del = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                ei.AddEventHandler(control, del);

                //Get mouse_move event of the control
                ei = control.GetType().GetEvent("MouseMove");
                mi = this.GetType().GetMethod("Control_MouseMove", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                del = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                ei.AddEventHandler(control, del);

                //Get mouse click event handler
                ei = control.GetType().GetEvent("Click");
                mi = this.GetType().GetMethod("Control_Click", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                del = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                ei.AddEventHandler(control, del);

                //Get mouse double click event handler
                ei = control.GetType().GetEvent("DoubleClick");
                mi = this.GetType().GetMethod("Control_DoubleClick", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                del = Delegate.CreateDelegate(ei.EventHandlerType, this, mi);
                ei.AddEventHandler(control, del);


            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Object Signal - Event Cast Error", MessageBoxButtons.OK);
            }

            //Update status show that the control data has been inputed
            inputData = 1;
        }

        /// <summary>
        /// Function to handle control double click
        /// </summary>
        /// <param name="sender">Control that send the event</param>
        /// <param name="e">Event arguments that raised when the event is triggered</param>
        protected virtual void Control_DoubleClick(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Control mouse click event handler template
        /// </summary>
        /// <param name="sender">Control that send the event</param>
        /// <param name="e">Event arguments that raised when the event is triggered</param>
        protected virtual void Control_Click(object sender, EventArgs e)
        {
            //no need, i need to change using another method to compensate properties update in the vertical grid in the main form
            //UpdatePanelProperties();
            //
            ObjectClickedEvent(this, e);
            AssignBasicPropertiesPanel();
            AssignCustomPropertiesPanel();

        }

        /// <summary>
        /// Function to change the focus of the control in the working environment
        /// </summary>
        //public virtual void ChangeFocusControl() //Moved
        //{
        //    controlSelected = true;
        //    parentUserControl.UnassignControlFocus();
        //    ControlBackColor = controlBackColorFocus;
        //    parentUserControl.SetSelectedControl(this);
        //}

        //This function should be override in the child class
        //public virtual void UpdatePanelProperties(VGridControl vGridControlProperties) //moved
        //{
        //    parentWindow.vGridControlProperties.FocusedRowChanged -= vGridControlProperties_FocusedRowChanged;
        //    parentWindow.vGridControlProperties.FocusedRow = null;
        //    parentWindow.vGridControlProperties.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(vGridControlProperties_FocusedRowChanged);
        //    parentWindow.categoryRowAppearance.ChildRows.Clear();
        //    parentWindow.categoryRowBehavior.ChildRows.Clear();
        //    parentWindow.categoryRowData.ChildRows.Clear();
        //    parentWindow.categoryRowDesign.ChildRows.Clear();
        //    parentWindow.categoryRowLayout.ChildRows.Clear();
        //    AssignBasicPropertiesPanel();
        //}

        /// <summary>
        /// Control mouse move event handler template
        /// </summary>
        /// <param name="sender">Control that send the event</param>
        /// <param name="e">Event arguments that raised when the event is triggered</param>
        protected virtual void Control_MouseMove(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Control mouse up event handler template
        /// </summary>
        /// <param name="sender">Control that send the event</param>
        /// <param name="e">Event arguments that raised when the event is triggered</param>
        protected virtual void Control_MouseUp(object sender, MouseEventArgs e)
        {
            controlMouseDown = false;
        }

        /// <summary>
        /// Control mouse down event handler template
        /// </summary>
        /// <param name="sender">Control that send the event</param>
        /// <param name="e">Event arguments that raised when the event is triggered</param>
        protected virtual void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (!controlMouseDown)
            {
                
            }
            else
            {
                ((Control)this.controlHandle).BringToFront();
            }
            ObjectMouseDownEvent(this, e);
        }

        private void AssignBasicPropertiesPanel()
        {
            EditorRow editorRow = new EditorRow();
            editorRow.Name = "BackColor";
            editorRow.Properties.Caption = "BackColor";
            editorRow.Properties.RowEdit = VGridMainForm.RepositoryItems["repositoryItemColorEditBackColor"];
            editorRow.Properties.Value = controlBackColor;
            VGridMainForm.Rows["categoryRowAppearance"].ChildRows.Add(editorRow);

            editorRow = new EditorRow();
            editorRow.Name = "Name";
            editorRow.Properties.Caption = "Name";
            editorRow.Properties.Value = controlName;
            VGridMainForm.Rows["categoryRowDesign"].ChildRows.Add(editorRow);

            editorRow = new EditorRow();
            editorRow.Name = "Location";
            editorRow.Properties.Caption = "Location";
            VGridMainForm.Rows["categoryRowLayout"].ChildRows.Add(editorRow);

            EditorRow editorRowSub = new EditorRow();
            editorRow.Name = "X";
            editorRowSub.Properties.Caption = "X";
            editorRowSub.Name = "X";
            editorRowSub.Properties.Value = controlLocation.X;
            editorRow.ChildRows.Add(editorRowSub);

            editorRowSub = new EditorRow();
            editorRow.Name = "Y";
            editorRowSub.Properties.Caption = "Y";
            editorRowSub.Name = "Y";
            editorRowSub.Properties.Value = controlLocation.Y;
            editorRow.ChildRows.Add(editorRowSub);
        }

        public void UpdateBasicProperties(string caption)
        {
            switch (caption)
            {
                case "Name":
                    if (WorkingEnvironmentFunction.validateName(vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value.ToString(),
                        tbControlPanelAssignment))
                    {
                        switch (controlCategory)
                        {
                            case ControlCategories.Indicator:
                                treeViewSolutionList.Nodes[0].Nodes[1].Nodes[controlName].Text = vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value.ToString();
                                treeViewSolutionList.Nodes[0].Nodes[1].Nodes[controlName].Name = vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value.ToString();
                                break;
                            case ControlCategories.Sequence:
                                treeViewSolutionList.Nodes[0].Nodes[0].Nodes[controlName].Text = vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value.ToString();
                                treeViewSolutionList.Nodes[0].Nodes[0].Nodes[controlName].Name = vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value.ToString();
                                break;
                        }
                        ControlName = vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value.ToString();
                    }
                    else
                    {
                        vGridMainForm.Rows["categoryRowDesign"].ChildRows["Name"].Properties.Value = ControlName;
                        return;
                    }
                    break;
                case "X":
                    Point px = new Point(Convert.ToInt16(vGridMainForm.Rows["categoryRowLayout"].ChildRows[0].ChildRows[0].Properties.Value), ControlLocation.Y);
                    ControlLocation = px;
                    break;
                case "Y":
                    Point py = new Point(ControlLocation.X, Convert.ToInt16(vGridMainForm.Rows["categoryRowLayout"].ChildRows[0].ChildRows[1].Properties.Value));
                    ControlLocation = py;
                    break;
            }
        }

        public virtual void UpdateFromGridCategory(string fieldName)
        {

        }

        public virtual void AssignCustomPropertiesPanel()
        {

        }

        protected void AddEditorRowToVGrid(string categoryRow, string fieldName, string caption, object value)
        {
            EditorRow editorRow = new EditorRow();
            editorRow.Name = fieldName;
            editorRow.Properties.Caption = caption;
            editorRow.Properties.Value = value;
            VGridMainForm.Rows[categoryRow.ToString()].ChildRows.Add(editorRow);
        }

        protected void AddComboBoxRowToVGrid(string categoryRow, string fieldName, string caption, object value, object[] option)
        {
            EditorRow editorRow = new EditorRow();
            editorRow.Name = fieldName;
            editorRow.Properties.Caption = caption;
            editorRow.Properties.Value = value;
            editorRow.Properties.RowEdit = VGridMainForm.RepositoryItems["repositoryItemComboBox"];

            RepositoryItemComboBox repositoryItemComboBox =
                (RepositoryItemComboBox)VGridMainForm.RepositoryItems["repositoryItemComboBox"];
            repositoryItemComboBox.Items.Clear();
            for (int i = 0; i < option.Count(); i++)
            {
                repositoryItemComboBox.Items.Add(option[i]);
            }
            VGridMainForm.Rows[categoryRow.ToString()].ChildRows.Add(editorRow);
        }

        protected object GetVGridCategoryRowValue(string categoryRow, string editorFieldName)
        {
            object result = VGridMainForm.Rows[categoryRow].ChildRows[editorFieldName].Properties.Value.ToString();
            return result;
        }

        /// <summary>
        /// Identify position of cursor and assign proper icon 
        /// </summary>
        /// <param name="X">X coordinate</param>
        /// <param name="Y">Y Coordinate</param>
        /// <returns>Status of the cursor icon</returns>
        protected int GetCursorSizePos(int X, int Y)
        {
            int result = 0;

            if ((X <= 2 && Y <= 2) || (X >= (controlWidth - 2) && Y >= (controlHeight - 2)))
            {
                ControlCursor = Cursors.SizeNWSE;
                if (X <= 2 && Y <= 2)
                {
                    result = 1;
                }
                else
                {
                    result = 5;
                }
            }
            else if ((X >= (controlWidth - 2) && Y <= 2) || (X <= 2 && Y >= (controlHeight - 2)))
            {
                ControlCursor = Cursors.SizeNESW;

                if (X >= (controlWidth - 2) && Y <= 2)
                {
                    result = 3;
                }
                else
                {
                    result = 7;
                }
            }
            else if (X <= 2 || X >= (controlWidth - 2))
            {
                ControlCursor = Cursors.SizeWE;

                if (X <= 2)
                {
                    result = 8;
                }
                else
                {
                    result = 4;
                }
            }
            else if (Y <= 2 || Y >= (controlHeight - 2))
            {
                ControlCursor = Cursors.SizeNS;

                if (Y <= 2)
                {
                    result = 2;
                }
                else
                {
                    result = 6;
                }
            }
            else
            {
                ControlCursor = Cursors.Hand;
                result = 9;
            }
            return result;
        }

        /// <summary>
        /// Function to init properties of vertical grid for the information purpose of the ObjectSignal
        /// </summary>
        //private void InitVGridProperties()//moved
        //{
        //    parentWindow.vGridControlProperties.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(vGridControlProperties_FocusedRowChanged);
        //    parentWindow.vGridControlProperties.EditorKeyDown += new KeyEventHandler(vGridControlProperties_EditorKeyDown);
        //}

        //protected virtual void vGridControlProperties_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e) //moved
        //{
        //    if (((ObjectSignal)parentUserControl.tbControlPanelAssignment.Rows[parentUserControl.GetSelectedControlIndex()]["control"]).controlName == this.controlName)
        //    {
        //        if (e.OldRow != null && e.OldRow.Index >= 0)
        //        {
        //            string caption = e.OldRow.Properties.Caption;
        //            UpdateBasicProperties(caption);
        //        }
        //    }
        //}

        /// <summary>
        /// Function to handle event that raised when key is down in the editor
        /// </summary>
        /// <param name="sender">Object that send the event</param>
        /// <param name="e">Arguments that passed by the object</param>
        //protected virtual void vGridControlProperties_EditorKeyDown(object sender, KeyEventArgs e) //moved
        //{
        //    if (((ObjectSignal)parentUserControl.tbControlPanelAssignment.Rows[parentUserControl.GetSelectedControlIndex()]["control"]).controlName == this.controlName)
        //    {
        //        string caption = parentWindow.vGridControlProperties.FocusedRow.Properties.Caption;
        //        if (e.KeyData == Keys.Enter)
        //        {
        //            UpdateBasicProperties(caption);
        //        }
        //    }
        //}

        /// <summary>
        /// Update the objectsignal information hierarchy on the treeview
        /// </summary>
        /// <param name="caption">Control type name</param>
        //public void UpdateBasicProperties(string caption)//Moved
        //{
        //    switch (caption)
        //    {
        //        case "Name":
        //            if (EnvironmentFunction.validateName(parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value.ToString(), 
        //                parentUserControl.tbControlPanelAssignment))
        //            {
        //                switch (controlCategory)
        //                {
        //                    case ControlCategories.Indicator:
        //                        parentWindow.treeViewSolutionList.Nodes[0].Nodes[1].Nodes[controlName].Text = parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value.ToString();
        //                        parentWindow.treeViewSolutionList.Nodes[0].Nodes[1].Nodes[controlName].Name = parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value.ToString();
        //                        break;
        //                    case ControlCategories.Sequence:
        //                        parentWindow.treeViewSolutionList.Nodes[0].Nodes[0].Nodes[controlName].Text = parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value.ToString();
        //                        parentWindow.treeViewSolutionList.Nodes[0].Nodes[0].Nodes[controlName].Name = parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value.ToString();
        //                        break;
        //                }
        //                ControlName = parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value.ToString();
        //            }
        //            else
        //            {
        //                parentWindow.categoryRowDesign.ChildRows["Name"].Properties.Value = controlName;
        //                return;
        //            }
        //            break;
        //        case "X":
        //            Point px = new Point(Convert.ToInt16(parentWindow.categoryRowLayout.ChildRows[0].ChildRows[0].Properties.Value), ControlLocation.Y);
        //            ControlLocation = px;
        //            break;
        //        case "Y":
        //            Point py = new Point(ControlLocation.X, Convert.ToInt16(parentWindow.categoryRowLayout.ChildRows[0].ChildRows[1].Properties.Value));
        //            ControlLocation = py;
        //            break;
        //    }
        //}

        /// <summary>
        /// Function to update treeview to get all the information hierarchy of the object within the working environment panel
        /// </summary>
        //private void UpdateTreeview()//Moved
        //{
        //    switch (controlCategory)
        //    {
        //        case ControlCategories.Indicator:
        //            treeViewSolutionList.SelectedNode = treeViewSolutionList.Nodes[0].Nodes[1].Nodes[controlName];
        //            break;
        //        case ControlCategories.Sequence:
        //            treeViewSolutionList.SelectedNode = treeViewSolutionList.Nodes[0].Nodes[0].Nodes[controlName];
        //            break;
        //    }
        //    treeViewSolutionList.Select();
        //}

        /// <summary>
        /// Function to assign basic properties of the objectsignal in the listview grid
        /// </summary>
        //protected virtual void AssignBasicPropertiesPanel()//Moved
        //{
        //    EditorRow editorRow = new EditorRow();
        //    editorRow.Name = "BackColor";
        //    editorRow.Properties.Caption = "BackColor";
        //    editorRow.Properties.RowEdit = parentWindow.repositoryItemColorEditBackColor;
        //    editorRow.Properties.Value = controlBackColor;
        //    parentWindow.categoryRowAppearance.ChildRows.Add(editorRow);

        //    editorRow = new EditorRow();
        //    editorRow.Name = "Name";
        //    editorRow.Properties.Caption = "Name";
        //    editorRow.Properties.Value = controlName;
        //    parentWindow.categoryRowDesign.ChildRows.Add(editorRow);

        //    editorRow = new EditorRow();
        //    editorRow.Name = "Location";
        //    editorRow.Properties.Caption = "Location";
        //    parentWindow.categoryRowLayout.ChildRows.Add(editorRow);

        //    EditorRow editorRowSub = new EditorRow();
        //    editorRow.Name = "X";
        //    editorRowSub.Properties.Caption = "X";
        //    editorRowSub.Properties.Value = controlLocation.X;
        //    editorRow.ChildRows.Add(editorRowSub);

        //    editorRowSub = new EditorRow();
        //    editorRow.Name = "Y";
        //    editorRowSub.Properties.Caption = "Y";
        //    editorRowSub.Properties.Value = controlLocation.Y;
        //    editorRow.ChildRows.Add(editorRowSub);
        //}

        /// <summary>
        /// Function to set the data to data variable within the object signal
        /// </summary>
        /// <param name="data"></param>
        protected virtual void setData(double[] data)
        {
            
        }

        /// <summary>
        /// Status whether the control is currently clicked or not
        /// </summary>
        protected bool controlMouseDown;
        public bool ControlMouseDown
        {
            get
            {
                return controlMouseDown;
            }
            set
            {
                controlMouseDown = value;
            }
        }

        /// <summary>
        /// Indicate whether the control is currently selected or not
        /// </summary>
        protected bool controlSelected;
        public bool ControlSelected
        {
            get
            {
                return controlSelected;
            }
            set
            {
                controlSelected = value;
            }
        }

        /// <summary>
        /// Indicate the index of the control on the application control list
        /// </summary>
        protected int indexList;
        public int IndexList
        {
            get
            {
                return indexList;
            }
            set
            {
                indexList = value;
            }
        }

        /// <summary>
        /// Type of the control
        /// </summary>
        protected Type controlType;
        public Type ControlType
        {
            get
            {
                return controlType;
            }
        }

        /// <summary>
        /// Control that handled by ObjectSignal
        /// </summary>
        protected object controlHandle;
        public object ControlHandle
        {
            get
            {
                return controlHandle;
            }
            set
            {
                controlHandle = value;
                InitControl(controlHandle);
            }
        }

        /// <summary>
        /// ContextMenuStrip that included on ObjectSignal Class
        /// </summary>
        protected ContextMenuStrip indicatorMenuStrip;
        public ContextMenuStrip IndicatorMenuStrip
        {
            get
            {
                return indicatorMenuStrip;
            }
            set
            {
                indicatorMenuStrip = value;
            }
        }

        /// <summary>
        /// Location of the controlHandle
        /// </summary>
        protected Point controlLocation;
        public Point ControlLocation
        {
            get
            {
                return controlLocation;
            }
            set
            {
                controlLocation = value;
                SetPropertyControl("Location", controlLocation);
                UpdateCOG(controlLocation);
            }
        }

        /// <summary>
        /// Width properties of the controlHandle
        /// </summary>
        protected int controlWidth;
        public int ControlWidth
        {
            get
            {
                return controlWidth;
            }
            set
            {
                controlWidth = value;
                SetPropertyControl("Width", controlWidth);
            }
        }


        /// <summary>
        /// Height properties of the controlHandle
        /// </summary>
        protected int controlHeight;
        public int ControlHeight
        {
            get
            {
                return controlHeight;
            }
            set
            {
                controlHeight = value;
                SetPropertyControl("Height", controlHeight);
            }
        }

        /// <summary>
        /// Location of center of gravity
        /// </summary>
        protected Point centerOfGravity;
        public Point CenterOfGravity
        {
            get
            {
                return centerOfGravity;
            }
            set
            {
                centerOfGravity = value;
            }
        }

        /// <summary>
        /// Local property of type of parent window
        /// </summary>
        protected Type parentWindowType; //Delete
        /// <summary>
        /// Public property of type of parent window
        /// </summary>
        public Type ParentWindowType //Delete
        {
            get
            {
                return parentWindowType;
            }
            set
            {
                parentWindowType = value;
            }
        }

        //public T GetPropertiesParentWindow<T>(string propertyName)
        //{
        //    object result = null;

        //    PropertyInfo pInfo = parentWindowType.GetProperty(propertyName);
        //    result = pInfo.GetValue(parentWindow, null);

        //    return (T)result;
        //}

        /// <summary>
        /// Function to update the value of COG when the object moves
        /// </summary>
        /// <param name="newLocation">New location of the using System.Drawing.Point</param>
        private void UpdateCOG(Point newLocation)
        {
            centerOfGravity = new Point(newLocation.X + (controlWidth / 2), newLocation.Y + (controlHeight / 2));
        }

        /// <summary>
        /// Set property of control to the local variable on ObjectSignal
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="value">Value of the property to be assigned</param>
        protected void SetPropertyControl(string propertyName, object value)
        {
            controlHandle.GetType().GetProperty(propertyName).SetValue(controlHandle, value, null);
            if (controlCategory != null && controlCategory == ControlCategories.Sequence && propertyName == "Location")
            {
                ObjectSequence OSequence = (ObjectSequence)this;
                Point location = (Point)value;
                OSequence.SequenceTextbox.Location = new Point(location.X - 30, location.Y + 43);
            }
        }

        /// <summary>
        /// Cursor type when the mouse hover over the control
        /// </summary>
        protected System.Windows.Forms.Cursor controlCursor;
        public System.Windows.Forms.Cursor ControlCursor
        {
            get
            {
                return controlCursor;
            }
            set
            {
                controlCursor = value;
                SetPropertyControl("Cursor", controlCursor);
            }
        }

        /// <summary>
        /// Backcolor property of the control
        /// </summary>
        protected Color controlBackColor;
        public Color ControlBackColor
        {
            get
            {
                return controlBackColor;
            }
            set
            {
                controlBackColor = value;
                SetPropertyControl("BackColor", controlBackColor);
            }
        }

        /// <summary>
        /// Property name of the control
        /// </summary>
        protected string controlName;
        public string ControlName
        {
            get
            {
                return controlName;
            }
            set
            {
                controlName = value;
                SetPropertyControl("Name", controlName);
            }
        }

        /// <summary>
        /// Indicate status whether all basic property has been added or not
        /// </summary>
        protected int inputData;
        public int InputData
        {
            get
            {
                return inputData;
            }
            set
            {
                inputData = value;
            }
        }

        /// <summary>
        /// Usercontrol that used to interface all properties of the control
        /// </summary>
        protected List<Control> objectUserControl;
        public List<Control> ObjectUserControl
        {
            get
            {
                return objectUserControl;
            }
            set
            {
                objectUserControl = value;
            }
        }

        /// <summary>
        /// Type of the Object Control defined in this application e.g AudioIn, FFT, SignalGenerator, etc
        /// </summary>
        protected ControlTypeNames? controlTypeName;
        public ControlTypeNames? ControlTypeName
        {
            get
            {
                return controlTypeName;
            }
            set
            {
                controlTypeName = value;
            }
        }

        /// <summary>
        /// Color when the control is selected
        /// </summary>
        protected Color controlBackColorFocus;
        public Color ControlBackColorFocus
        {
            get
            {
                return controlBackColorFocus;
            }
            set
            {
                controlBackColorFocus = value;
            }
        }

        /// <summary>
        /// Color when the control is not selected
        /// </summary>
        protected Color controlBackColorUnfocus;
        public Color ControlBackColorUnfocus
        {
            get
            {
                return controlBackColorUnfocus;
            }
            set
            {
                controlBackColorUnfocus = value;
            }
        }

        /// <summary>
        /// Category of the conrol (Sequence or Indicator)
        /// </summary>
        protected ControlCategories? controlCategory;
        public ControlCategories? ControlCategory
        {
            get
            {
                return controlCategory;
            }
            set
            {
                controlCategory = value;
            }
        }

        /// <summary>
        /// Temporarily not used - possibly use for 
        /// </summary>
        protected DataTable tbPropertiesControl;
        public DataTable TbPropertiesControl
        {
            get
            {
                return tbPropertiesControl;
            }
            set
            {
                tbPropertiesControl = value;
            }
        }

        /// <summary>
        /// Input ObjectSequence to deliver data input to this object
        /// </summary>
        protected ObjectSequence sequenceSource;
        public virtual ObjectSequence SequenceSource
        {
            get
            {
                return sequenceSource;
            }
            set
            {
                DisposeSequenceSource();
                sequenceSource = value;
                setProperties(sequenceSource);
                AssignSequence();
            }
        }

        protected VGridControl vGridMainForm;
        public VGridControl VGridMainForm
        {
            get
            {
                return vGridMainForm;
            }
            set
            {
                vGridMainForm = value;
            }
        }

        protected DataTable tbControlPanelAssignment;
        public DataTable TbControlPanelAssignment
        {
            get
            {
                return tbControlPanelAssignment;
            }
            set
            {
                tbControlPanelAssignment = value;
            }
        }

        protected TreeView treeViewSolutionList;
        public TreeView TreeViewSolutionList
        {
            get
            {
                return treeViewSolutionList;
            }
            set
            {
                treeViewSolutionList = value;
            }
        }

        protected Panel panelIndicator;
        public Panel PanelIndicator
        {
            get
            {
                return panelIndicator;
            }
            set
            {
                panelIndicator = value;
            }
        }

        protected Panel panelSequence;
        public Panel PanelSequence
        {
            get
            {
                return panelSequence;
            }
            set
            {
                panelSequence = value;
            }
        }

        /// <summary>
        /// Temporarily not used
        /// </summary>
        protected string objectName;
        public string ObjectName
        {
            get
            {
                return objectName;
            }
            set
            {
                objectName = value;
            }
        }

        /// <summary>
        /// Status whether the object in the recording mode or not
        /// </summary>
        protected bool isRecording;
        public bool IsRecording
        {
            get
            {
                return isRecording;
            }
            set
            {
                isRecording = value;
            }
        }

        /// <summary>
        /// ObjectRecording that this object will use (WAV or Video)
        /// </summary>
        protected ObjectRecording obRecord;
        public ObjectRecording ObRecord
        {
            get
            {
                return obRecord;
            }
            set
            {
                obRecord = value;
            }
        }

        /// <summary>
        /// Temporarily not used
        /// </summary>
        protected bool dataPlaybackStatus;
        public bool DataPlaybackStatus
        {
            get
            {
                return dataPlaybackStatus;
            }
            set
            {
                dataPlaybackStatus = value;
            }
        }

        /// <summary>
        /// Temporarily not used
        /// </summary>
        /// <param name="Osignal"></param>
        protected virtual void setProperties(ObjectSignal Osignal)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void DisposeSequenceSource()
        {

        }

        /// <summary>
        /// Function to initate sequence data when the sequence source is assigned to this object
        /// </summary>
        protected virtual void AssignSequence()
        {

        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("controlLocation", controlLocation);
            info.AddValue("controlWidth", controlWidth);
            info.AddValue("controlHeight", controlHeight);
            info.AddValue("controlBackColor", controlBackColor);
            info.AddValue("controlCategory", controlCategory);
            info.AddValue("controlType", controlType);
            info.AddValue("controlName", controlName);
            info.AddValue("sequenceSource", sequenceSource);
        }

        #endregion

        public virtual void Dispose()
        {

            if (controlHandle != null)
            {
                Control controlDispose = controlHandle as Control;
                controlDispose.Dispose();
                controlDispose = null;
            }

            if (sequenceSource != null)
            {
                sequenceSource = null;
            }

            GC.SuppressFinalize(this);
        }

    }
}
