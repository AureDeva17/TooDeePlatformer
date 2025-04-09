/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 23.01.2021                                                    **
**                                                                           **
** Modifications                                                             **
**   Auteur  :                                                               **
**   Version : X.X                                                           **
**   Date    :                                                               **
**   Raisons :                                                               **
**                                                                           **
**                                                                           **
******************************************************************************/

/******************************************************************************
** DESCRIPTION                                                               **
** This class is used to store all the data of a dimension like the object   **
** groups and the controls like a panel, a tabpage and a selection area.     **
** It also countain every method to add the obstacles and the movable move.  **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// This class contain everything needed to create a dimension (a level)
    /// like the controls and all the data.
    /// </summary>
    class Dimension
    {
        //declare the static properties
        public const string strCLASSNAME = "Dimension";                 //class name
        public const ObjectType objTYPE = ObjectType.Dimension;         //object type
        public static int IntNextID { get; set; }                       //next ID to be used
        public static List<Dimension> List_dimDimensions { get; set; }  //list of all the worlds

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_dimDimensions.Count; i++)
                if (List_dimDimensions[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_dimDimensions[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static Dimension GetObjectByID(int intID) => List_dimDimensions[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
            string strText = Main.strVERSION + "\n";

            strText += strCLASSNAME + "\n";
            strText += IntNextID + "\n";
            strText += List_dimDimensions.Count + "\n";

            for (int i = 0; i < List_dimDimensions.Count; i++)
            {
                strText += CreateTextFromObject(List_dimDimensions[i]);

                if (List_dimDimensions.Count != i)
                    strText += "\n";
            }

            return strText;
        }

        /// <summary>
        /// Copy the data from the file to the dimension
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrDimensionTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_dimDimensions.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_dimDimensions.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                }
                else
                    MessageBox.Show("The file isn't a storage for dimensions !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a text line from an object
        /// </summary>
        /// <returns></returns>
        public static string CreateTextFromObject(Dimension dimObject)
        {
            string strText = "";

            strText += dimObject.IntID + $"{Main.chrSEPARATOR}";
            strText += dimObject.IntWorldID + $"{Main.chrSEPARATOR}";
            strText += dimObject.StrName + $"{Main.chrSEPARATOR}";
            strText += dimObject.IntWidth + $"{Main.chrSEPARATOR}";
            strText += dimObject.IntHeight;

            return strText;
        }

        /// <summary>
        /// Create an object from a text line
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static Dimension CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            Dimension dimNewDimension = new Dimension()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntWorldID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                IntWidth = Convert.ToInt32(tab_strObject[3]),
                IntHeight = Convert.ToInt32(tab_strObject[4]),
            };

            if (File.Exists(dimNewDimension.StrBackgroundPath))
            {
                Image imgNewImage = Image.FromFile(dimNewDimension.StrBackgroundPath);
                dimNewDimension.ImgBackgroundImage = new Bitmap(imgNewImage);
                imgNewImage.Dispose();
            }

            dimNewDimension.PnlDimension.Size = dimNewDimension._posPosition.ToSize();
            dimNewDimension.TbpDimension.Text = dimNewDimension.StrName;

            if (!blnOnlyRead)
            {
                //add the control to the panel
                dimNewDimension.TbpDimension.Controls.Add(dimNewDimension.PnlDimension);
                dimNewDimension.DimgrParent.TbcDimensions.Controls.Add(dimNewDimension.TbpDimension);
            }

            return dimNewDimension;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static Dimension()
        {
            IntNextID = 0;
            List_dimDimensions = new List<Dimension>();
        }
        #endregion

        //declare the events
        public event dlgPersoEvent evtValuesChanged;                    //triggered when visual values are changed

        //declare the properties
        public int IntID { get; set; }                                  //get the ID of the object       
        public FolderGroup FlgrFolderGroup { get; set; }                //contain the folders
        public Panel PnlDimension { get; set; }                         //contain all the obstacles in a panel
        public PictureBox PicBoxSelectedArea  { get; set; }             //contain the picture box used to represent the selected area
        public TabPage TbpDimension { get; set; }                       //contain the tabpage of the dimension
        public int IntWorldID { get; set; }                             //contain the parent

        //declare the variables
        private string _strName;                                        //name of the dimension
        private Position _posPosition;                                  //contain the size of the dimension
        private Position _posSelection;                                 //contain the information about the selected area
        private Image _imgBackgroundImage;                              //contain the background
        private bool _blnIsBackGroundOn;                                //contain the information about if the background is on

        //Grabbing and dragging
        private bool _blnDragging;                                      //boolean telling if the user is dragging an object
        private bool _blnIsGrabbedSelection;                            //boolean telling if the user grabbed a selection
        private bool _blnIsGrabbedObs;                                  //boolean telling if the grabbed object an obstacle
        private Obstacle _obsGrabbed;                                   //obstacle grabbed
        private Special _spcGrabbed;                                    //special grabbed
        private int _intFromX;                                          //first X position
        private int _intFromY;                                          //first Y position
        private int _intTotalX;                                         //total distance traveled in X
        private int _intTotalY;                                         //total distance traveled in Y

        #region Properties
        //declare the properties
        /// <summary>
        /// Get the name and set the name of the dimension and of the tabpage and panel
        /// </summary>
        public string StrName
        {
            get => _strName;

            set
            {
                if (!DimgrParent.IsDimensionNameTaken(value, StrName))
                {
                    _strName = value;
                    TbpDimension.Text = value;
                    TbpDimension.Name = value;
                    PnlDimension.Name = value;

                    evtValuesChanged?.Invoke(IntID, this, GetType());
                }
            }
        }

        /// <summary>
        /// Get the width of the dimension and set the width to the value and the panel
        /// </summary>
        public int IntWidth
        {
            get => (int)_posPosition.DisDistance.dblWidth;

            set
            {
                //set the new dimension width
                _posPosition.DisDistance.dblWidth = value;
                PnlDimension.Size = new Size(value * Main.intUNITSIZE, PnlDimension.Size.Height);
            }
        }

        /// <summary>
        /// Get the height of the dimension and set the height to the value and the panel
        /// </summary>
        public int IntHeight
        {
            get => (int)_posPosition.DisDistance.dblHeight;
            
            set
            {
                //search in the moving folders
                for (int i = 0; i < FlgrFolderGroup.List_flFolders.Count; i++)
                    for (int ii = 0; ii < FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles.Count; ii++)
                    {
                        //reset the diemension height
                        FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].UpdatePictureBox();
                    }

                //set the new dimension height
                _posPosition.DisDistance.dblHeight = value;
                PnlDimension.Size = new Size(PnlDimension.Size.Width, value * Main.intUNITSIZE);
            }
        }

        /// <summary>
        /// Get the background of the panel and set the background of the panel
        /// </summary>
        public Image ImgBackgroundImage
        {
            get => _imgBackgroundImage;
            
            set
            {
                _imgBackgroundImage = value;
                if (_blnIsBackGroundOn)
                    PnlDimension.BackgroundImage = value;
            }
        }

        /// <summary>
        /// Get the parent
        /// </summary>
        public DimensionGroup DimgrParent
        {
            get
            {
                try
                {
                    return World.List_wrldWorlds[World.GetIndexFromID(IntWorldID)].DimgrDimensions;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => DimgrParent.WrldParent;
        }

        /// <summary>
        /// Path of the background image
        /// </summary>
        public string StrBackgroundPath
        {
            get => Main.StrBGImagesPath + @"\" + StrName + @".png";
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default variables
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="distance"></param>
        public Dimension(string strName_, Distance distance, int intWorldID_)
        {
            //set the default variables
            IntWorldID = intWorldID_;
            PnlDimension = new Panel();
            TbpDimension = new TabPage();
            _posPosition = new Position();
            _posSelection = new Position();
            FlgrFolderGroup = new FolderGroup("Folders", this);
            FlgrFolderGroup.AddFolder();

            _blnDragging = false;
            _blnIsGrabbedObs = true;
            _blnIsGrabbedSelection = false;
            _obsGrabbed = null;
            _spcGrabbed = null;
            _intFromX = 0;
            _intFromY = 0;
            _intTotalX = 0;
            _intTotalY = 0;

            IntID = IntNextID++;
            _blnIsBackGroundOn = false;
            _posPosition.DisDistance.dblWidth = distance.dblWidth;
            _posPosition.DisDistance.dblHeight = distance.dblHeight;
            this._strName = strName_;

            TbpDimension.AutoScroll = true;
            TbpDimension.Text = StrName;

            #region Context Menu Strip
            //declare the variables used to create a contextmenustrip
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                 //contain the menu
            List<ToolStripMenuItem> tsmiMain = new List<ToolStripMenuItem>();           //contain the main items
            List<ToolStripMenuItem> tsmiSelect = new List<ToolStripMenuItem>();         //contain the selection items
            List<ToolStripMenuItem> tsmiUnselect = new List<ToolStripMenuItem>();       //contain the unselection items
            List<ToolStripMenuItem> tsmiSelection = new List<ToolStripMenuItem>();      //contain the adding of objects using the collection
            List<ToolStripMenuItem> tsmiRemove = new List<ToolStripMenuItem>();         //contain the removing items
            int intMainTool = 0;                                                        //contain the index of the main items

            //this tool add an obstacle from the selection area
            tsmiSelection.Add(new ToolStripMenuItem("Obstacle Q"));
            tsmiSelection[0].Tag = IntID;
            tsmiSelection[0].Click += new EventHandler(AddObstacleFromSelection_Click);

            //this tool add a special object from the selection area
            tsmiSelection.Add(new ToolStripMenuItem("Special Object W"));
            tsmiSelection[1].Tag = IntID;
            tsmiSelection[1].Click += new EventHandler(AddSpecialFromSelection_Click);

            //add objects from the selection area
            tsmiMain.Add(new ToolStripMenuItem("Add From Selection"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiSelection.ToArray());
            tsmiMain[intMainTool].Tag = IntID;
            intMainTool++;

            //select objects from selection
            tsmiSelect.Add(new ToolStripMenuItem("From Selection S"));
            tsmiSelect[0].Tag = IntID;
            tsmiSelect[0].Click += new EventHandler(Select_Click);

            //select all objects
            tsmiSelect.Add(new ToolStripMenuItem("All Ctrl + A"));
            tsmiSelect[1].Tag = IntID;
            tsmiSelect[1].Click += new EventHandler(SelectAll_Click);

            //open the selection menu
            tsmiMain.Add(new ToolStripMenuItem("Select"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiSelect.ToArray());
            tsmiMain[intMainTool].Tag = IntID;
            intMainTool++;

            //unselect objects from selection
            tsmiUnselect.Add(new ToolStripMenuItem("From Selection U"));
            tsmiUnselect[0].Tag = IntID;
            tsmiUnselect[0].Click += new EventHandler(Unselect_Click);

            //unselect all objects
            tsmiUnselect.Add(new ToolStripMenuItem("All Esc"));
            tsmiUnselect[1].Tag = IntID;
            tsmiUnselect[1].Click += new EventHandler(UnselectAll_Click);

            //open the unselect menu
            tsmiMain.Add(new ToolStripMenuItem("Unselect"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiUnselect.ToArray());
            tsmiMain[intMainTool].Tag = IntID;
            intMainTool++;

            //remove everything selected
            tsmiRemove.Add(new ToolStripMenuItem("From Selection Delete"));
            tsmiRemove[0].Tag = IntID;
            tsmiRemove[0].Click += new EventHandler(RemoveFromSelection_Click);

            //open the removing menu
            tsmiMain.Add(new ToolStripMenuItem("Remove"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiRemove.ToArray());
            tsmiMain[intMainTool].Tag = IntID;
            intMainTool++;

            //add the main items to the context menu
            contextMenuStrip.Items.AddRange(tsmiMain.ToArray());

            //add the menu on the panel
            PnlDimension.ContextMenuStrip = contextMenuStrip;
            #endregion

            #region Panel
            //set the properties
            PnlDimension.BackgroundImage = Image.FromFile(Main.StrImagesPath + @"\Square1x1.png");
            PnlDimension.BorderStyle = BorderStyle.FixedSingle;
            PnlDimension.Location = new Point(0, 0);
            PnlDimension.Name = "Dimension";
            PnlDimension.Size = _posPosition.ToSize();
            PnlDimension.TabIndex = 0;
            PnlDimension.Cursor = Cursors.Cross;

            //add the events
            PnlDimension.MouseDown += new MouseEventHandler(pnlDimension_MouseDown);
            PnlDimension.MouseMove += new MouseEventHandler(pnlDimension_MouseMove);
            PnlDimension.MouseUp += new MouseEventHandler(pnlDimension_MouseUp);

            //add the control to the panel
            TbpDimension.Controls.Add(PnlDimension);
            #endregion

            #region Selection PictureBox
            //setting the custom default values to the selection area
            PicBoxSelectedArea = new PictureBox
            {
                BackColor = Color.FromArgb(50, 0, 0, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(0, 0),
                Name = "Selection",
                Size = new Size(0, 0),
                TabIndex = 0
            };

            //adding the picturebox to the panel
            PnlDimension.Controls.Add(PicBoxSelectedArea);
            #endregion

            PnlDimension.MouseMove += new MouseEventHandler(Drag);
            PnlDimension.MouseUp += new MouseEventHandler(StopDragging);
            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create a new dimension from a file
        /// </summary>
        private Dimension()
        {
            //set the default variables
            PnlDimension = new Panel();
            TbpDimension = new TabPage();
            _posPosition = new Position();
            _posSelection = new Position();
            FlgrFolderGroup = new FolderGroup("Folders", this);

            _blnDragging = false;
            _blnIsGrabbedObs = true;
            _blnIsGrabbedSelection = false;
            _obsGrabbed = null;
            _spcGrabbed = null;
            _intFromX = 0;
            _intFromY = 0;
            _intTotalX = 0;
            _intTotalY = 0;

            _blnIsBackGroundOn = false;

            TbpDimension.AutoScroll = true;

            #region Context Menu Strip
            //declare the variables used to create a contextmenustrip
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                 //contain the menu
            List<ToolStripMenuItem> tsmiMain = new List<ToolStripMenuItem>();           //contain the main items
            List<ToolStripMenuItem> tsmiSelect = new List<ToolStripMenuItem>();         //contain the selection items
            List<ToolStripMenuItem> tsmiUnselect = new List<ToolStripMenuItem>();       //contain the unselection items
            List<ToolStripMenuItem> tsmiSelection = new List<ToolStripMenuItem>();      //contain the adding of objects using the collection
            List<ToolStripMenuItem> tsmiRemove = new List<ToolStripMenuItem>();         //contain the removing items
            int intMainTool = 0;                                                        //contain the index of the main items

            //this tool add an obstacle from the selection area
            tsmiSelection.Add(new ToolStripMenuItem("Obstacle Q"));
            tsmiSelection[0].Click += new EventHandler(AddObstacleFromSelection_Click);

            //this tool add a special object from the selection area
            tsmiSelection.Add(new ToolStripMenuItem("Special Object W"));
            tsmiSelection[1].Click += new EventHandler(AddSpecialFromSelection_Click);

            //add objects from the selection area
            tsmiMain.Add(new ToolStripMenuItem("Add From Selection"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiSelection.ToArray());
            intMainTool++;

            //select objects from selection
            tsmiSelect.Add(new ToolStripMenuItem("From Selection S"));
            tsmiSelect[0].Click += new EventHandler(Select_Click);

            //select all objects
            tsmiSelect.Add(new ToolStripMenuItem("All Ctrl + A"));
            tsmiSelect[1].Click += new EventHandler(SelectAll_Click);

            //open the selection menu
            tsmiMain.Add(new ToolStripMenuItem("Select"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiSelect.ToArray());
            intMainTool++;

            //unselect objects from selection
            tsmiUnselect.Add(new ToolStripMenuItem("From Selection U"));
            tsmiUnselect[0].Click += new EventHandler(Unselect_Click);

            //unselect all objects
            tsmiUnselect.Add(new ToolStripMenuItem("All Esc"));
            tsmiUnselect[1].Click += new EventHandler(UnselectAll_Click);

            //open the unselect menu
            tsmiMain.Add(new ToolStripMenuItem("Unselect"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiUnselect.ToArray());
            intMainTool++;

            //remove everything selected
            tsmiRemove.Add(new ToolStripMenuItem("From Selection Delete"));
            tsmiRemove[0].Click += new EventHandler(RemoveFromSelection_Click);

            //open the removing menu
            tsmiMain.Add(new ToolStripMenuItem("Remove"));
            tsmiMain[intMainTool].DropDownItems.AddRange(tsmiRemove.ToArray());
            intMainTool++;

            //add the main items to the context menu
            contextMenuStrip.Items.AddRange(tsmiMain.ToArray());

            //add the menu on the panel
            PnlDimension.ContextMenuStrip = contextMenuStrip;
            #endregion

            #region Panel
            //set the properties
            PnlDimension.BackgroundImage = Image.FromFile(Main.StrImagesPath + @"\Square1x1.png");
            PnlDimension.BorderStyle = BorderStyle.FixedSingle;
            PnlDimension.Location = new Point(0, 0);
            PnlDimension.Name = "Dimension";
            PnlDimension.TabIndex = 0;
            PnlDimension.Cursor = Cursors.Cross;

            //add the events
            PnlDimension.MouseDown += new MouseEventHandler(pnlDimension_MouseDown);
            PnlDimension.MouseMove += new MouseEventHandler(pnlDimension_MouseMove);
            PnlDimension.MouseUp += new MouseEventHandler(pnlDimension_MouseUp);
            #endregion

            #region Selection PictureBox
            //setting the default values to the selection area
            PicBoxSelectedArea = new PictureBox
            {

                //setting the custom default values to the selection area
                BackColor = Color.FromArgb(50, 0, 0, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(0, 0),
                Name = "Selection",
                Size = new Size(0, 0),
                TabIndex = 0
            };

            //adding the picturebox to the panel
            PnlDimension.Controls.Add(PicBoxSelectedArea);
            #endregion

            PnlDimension.MouseMove += new MouseEventHandler(Drag);
            PnlDimension.MouseUp += new MouseEventHandler(StopDragging);
            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }
        #endregion

        #region Background
        /// <summary>
        /// Set the background to empty
        /// </summary>
        public void SetBackgroundToEmpty() => PnlDimension.BackgroundImage = null;

        /// <summary>
        /// Set the background to image
        /// </summary>
        public void SetBackgroundToImage() => PnlDimension.BackgroundImage = ImgBackgroundImage;

        /// <summary>
        /// Set the background to grid
        /// </summary>
        public void SetBackgroundToGrid() => PnlDimension.BackgroundImage = Image.FromFile(Main.StrImagesPath + @"\Square1x1.png");

        /// <summary>
        /// Save the background to the assigned file
        /// </summary>
        public void SaveBackground()
        {
            if (ImgBackgroundImage != null)
            {
                using (Image imgToSave = new Bitmap(ImgBackgroundImage))
                    imgToSave.Save(StrBackgroundPath, ImageFormat.Png);
            }
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the dimension node
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the new node
            List<TreeNode> list_trnDimension_ = new List<TreeNode>
            {
                FlgrFolderGroup.GetTreeNode()
            };      //contain the main nodes

            //return the list of nodes
            return new TreeNode(StrName, list_trnDimension_.ToArray()) { ContextMenuStrip = GetContextMenu(), Tag = IntID};
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Get the context menu of the dimension
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare the contextbox and the list of items
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();             //create the contextbox
            List<ToolStripMenuItem> list_tsmiItems = new List<ToolStripMenuItem>
            {

                //add an item used to remove the selected dimension
                new ToolStripMenuItem("Remove")
            }; //create the list of items
            list_tsmiItems[0].Tag = IntID;
            list_tsmiItems[0].Click += new EventHandler(RemoveDimension_Click);

            //add an item used to modify the properties of the selected dimension
            list_tsmiItems.Add(new ToolStripMenuItem("Properties"));
            list_tsmiItems[1].Tag = IntID;
            list_tsmiItems[1].Click += new EventHandler(ModifyDimensionProperties_Click);

            //add the items to the contextbox
            contextMenuStrip.Items.AddRange(list_tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Remove the dimension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveDimension_Click(object sender, EventArgs e)
        {
            //search for the object group
            for (int i = 0; i < List_dimDimensions.Count; i++)
                if (List_dimDimensions[i].IntID == IntID)
                {
                    DimgrParent.Remove(i);

                    break;
                }
        }

        /// <summary>
        /// Open the dimension form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyDimensionProperties_Click(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Events
        //declare all variables needed to create the selection area
        private bool _blnIsChangingSelection = false;        //tell the game if the area is getting selected
        private int _intX = 0;                               //first position in X
        private int _intY = 0;                               //first position in Y
        private int _intX2 = 0;                              //second position in X
        private int _intY2 = 0;                              //second position in Y

        #region Context Menu Strip
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSpecialFromSelection_Click(object sender, EventArgs e) => AddSpecialFromSelection();

        /// <summary>
        /// Remove all selected objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFromSelection_Click(object sender, EventArgs e) => RemoveAllSelected();

        /// <summary>
        /// Unselect every objects from the dimension's selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Unselect_Click(object sender, EventArgs e) => UnSelect();

        /// <summary>
        /// Unselect every objects from the dimension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectAll_Click(object sender, EventArgs e) => UnSelectAll();

        /// <summary>
        /// Select every objects from the dimension's selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Select_Click(object sender, EventArgs e) => Select();

        /// <summary>
        /// Select every objects from the dimension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAll_Click(object sender, EventArgs e) => SelectAll();

        /// <summary>
        /// Add an obstacle from the selection area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddObstacleFromSelection_Click(object sender, EventArgs e) => AddObstacleFromSelection();
        #endregion

        /// <summary>
        /// Set the first position of the selection area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlDimension_MouseDown(object sender, MouseEventArgs e)
        {
            //set the first position
            _intX = PnlDimension.PointToClient(Cursor.Position).X;
            _intY = PnlDimension.PointToClient(Cursor.Position).Y;

            //tell the game the selection started
            _blnIsChangingSelection = true;
        }

        /// <summary>
        /// Set the second position of the selection area every time the mouse move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlDimension_MouseMove(object sender, MouseEventArgs e)
        {
            //detect if the user is selecting an area
            if (_blnIsChangingSelection)
            {
                //set the second position
                _intX2 = PnlDimension.PointToClient(Cursor.Position).X;
                _intY2 = PnlDimension.PointToClient(Cursor.Position).Y;

                //update the selection area
                SelectArea(_intX, _intY, _intX2, _intY2);
            }
        }

        /// <summary>
        /// Stop the changing of the selected area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlDimension_MouseUp(object sender, MouseEventArgs e) => _blnIsChangingSelection = false;
        #endregion

        #region Selection
        /// <summary>
        /// Select the the area using the given points
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// <param name="intX2"></param>
        /// <param name="intY2"></param>
        private void SelectArea(int intX, int intY, int intX2, int intY2)
        {
            //transform the two points into a position
            _posSelection = Position.Convert2pLocationToPosition(intX, intY, intX2, intY2).InvertY((int)_posPosition.DisDistance.dblHeight);

            //convert the position for the picturebox
            PicBoxSelectedArea.Size = _posSelection.ToSize();
            PicBoxSelectedArea.Location = _posSelection.ToPoint((int)_posPosition.DisDistance.dblHeight);

            //send the picture box behind the obstacles
            PicBoxSelectedArea.SendToBack();

            //show the picture box
            PicBoxSelectedArea.Show();
        }

        /// <summary>
        /// Grow the size of the selection
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void GrowSelection(int intX, int intY)
        {
            if (intX >= 0)
            {
                if (_posSelection.LocLocation.dblX >= 0 && _posSelection.LocLocation.dblY >= 0 && _posSelection.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - (_posSelection.DisDistance.dblWidth + intX) && _posSelection.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - _posSelection.DisDistance.dblHeight)
                    //add to the selection
                    _posSelection.DisDistance.dblWidth += intX;
            }
            else if (_posSelection.LocLocation.dblX + intX >= 0 && _posSelection.LocLocation.dblY >= 0 && _posSelection.LocLocation.dblX + intX <= _posPosition.DisDistance.dblWidth - (_posSelection.DisDistance.dblWidth - intX) && _posSelection.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - _posSelection.DisDistance.dblHeight)
            {
                //add to the selection
                _posSelection.DisDistance.dblWidth -= intX;
                _posSelection.LocLocation.dblX += intX;
            }

            if (intY >= 0)
            {
                if (_posSelection.LocLocation.dblX >= 0 && _posSelection.LocLocation.dblY >= 0 && _posSelection.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - _posSelection.DisDistance.dblWidth && _posSelection.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - (_posSelection.DisDistance.dblHeight + intY))
                    //add to the selection
                    _posSelection.DisDistance.dblHeight += intY;
            }
            else if (_posSelection.LocLocation.dblX >= 0 && _posSelection.LocLocation.dblY + intY >= 0 && _posSelection.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - _posSelection.DisDistance.dblWidth && _posSelection.LocLocation.dblY + intY <= _posPosition.DisDistance.dblHeight - (_posSelection.DisDistance.dblHeight - intY))
            {
                //add to the selection
                _posSelection.DisDistance.dblHeight -= intY;
                _posSelection.LocLocation.dblY += intY;
            }

            if (_posSelection.DisDistance.dblWidth < 0)
                _posSelection.DisDistance.dblWidth = 0;

            if (_posSelection.DisDistance.dblHeight < 0)
                _posSelection.DisDistance.dblHeight = 0;

            //convert the position for the picturebox
            PicBoxSelectedArea.Size = _posSelection.ToSize();
            PicBoxSelectedArea.Location = _posSelection.ToPoint((int)_posPosition.DisDistance.dblHeight);

            //send the picture box behind the obstacles
            PicBoxSelectedArea.SendToBack();

            //show the picture box
            PicBoxSelectedArea.Show();
        }

        /// <summary>
        /// Shrink the size of the selection
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void ShrinkSelection(int intX, int intY)
        {
            if (intX >= 0)
            {
                if (_posSelection.LocLocation.dblX >= 0 && _posSelection.LocLocation.dblY >= 0 && _posSelection.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - (_posSelection.DisDistance.dblWidth - intX) && _posSelection.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - _posSelection.DisDistance.dblHeight)
                    //add to the selection
                    _posSelection.DisDistance.dblWidth -= intX;
            }
            else if (_posSelection.LocLocation.dblX - intX >= 0 && _posSelection.LocLocation.dblY >= 0 && _posSelection.LocLocation.dblX - intX <= _posPosition.DisDistance.dblWidth - (_posSelection.DisDistance.dblWidth + intX) && _posSelection.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - _posSelection.DisDistance.dblHeight)
            {
                //add to the selection
                _posSelection.DisDistance.dblWidth += intX;
                _posSelection.LocLocation.dblX -= intX;
            }

            if (intY >= 0)
            {
                if (_posSelection.LocLocation.dblX >= 0 && _posSelection.LocLocation.dblY >= 0 && _posSelection.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - _posSelection.DisDistance.dblWidth && _posSelection.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - (_posSelection.DisDistance.dblHeight - intY))
                    //add to the selection
                    _posSelection.DisDistance.dblHeight -= intY;
            }
            else if (_posSelection.LocLocation.dblX >= 0 && _posSelection.LocLocation.dblY - intY >= 0 && _posSelection.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - _posSelection.DisDistance.dblWidth && _posSelection.LocLocation.dblY - intY <= _posPosition.DisDistance.dblHeight - (_posSelection.DisDistance.dblHeight + intY))
            {
                //add to the selection
                _posSelection.DisDistance.dblHeight += intY;
                _posSelection.LocLocation.dblY -= intY;
            }

            if (_posSelection.DisDistance.dblWidth < 0)
                _posSelection.DisDistance.dblWidth = 0;

            if (_posSelection.DisDistance.dblHeight < 0)
                _posSelection.DisDistance.dblHeight = 0;

            //convert the position for the picturebox
            PicBoxSelectedArea.Size = _posSelection.ToSize();
            PicBoxSelectedArea.Location = _posSelection.ToPoint((int)_posPosition.DisDistance.dblHeight);

            //send the picture box behind the obstacles
            PicBoxSelectedArea.SendToBack();

            //show the picture box
            PicBoxSelectedArea.Show();
        }

        /// <summary>
        /// Delete the selection area
        /// </summary>
        private void DelSelectArea()
        {
            //hide the picture box
            PicBoxSelectedArea.Hide();

            //make it uncapable to do anything
            _posSelection = new Position();
            PicBoxSelectedArea.Size = new Size(0, 0);
            PicBoxSelectedArea.Location = new Point(0, 0);
        }

        /// <summary>
        /// Select all the obtacles present inside the selection area
        /// </summary>
        public void Select()
        {
            //test if the picture box area is visible
            if (PicBoxSelectedArea.Visible == true)
            {
                FlgrFolderGroup.SelectWithArea(_posSelection, true);

                //reset the selection area
                DelSelectArea();
            }
        }

        /// <summary>
        /// Unelect all the obtacles present inside the selection area
        /// </summary>
        public void UnSelect()
        {
            //test if the picture box area is visible
            if (PicBoxSelectedArea.Visible == true)
            {
                FlgrFolderGroup.SelectWithArea(_posSelection, false);

                //reset the selection area
                DelSelectArea();
            }
        }

        /// <summary>
        /// Select every obstacles present inside the dimension
        /// </summary>
        public void SelectAll()
        {
            FlgrFolderGroup.SelectAll();

            //reset the selection the area
            DelSelectArea();
        }

        /// <summary>
        /// Unselect every obstacles present inside the dimension
        /// </summary>
        public void UnSelectAll()
        {
            FlgrFolderGroup.UnselectAll();

            //reset the selection the area
            DelSelectArea();
        }
        #endregion

        #region Grabbing and dragging
        /// <summary>
        /// Activated when grabbing an object
        /// </summary>
        /// <param name="intID"></param>
        public void GrabObject(Obstacle obsObstacle)
        {
            _blnDragging = true;
            _blnIsGrabbedSelection = false;
            _blnIsGrabbedObs = true;
            _obsGrabbed = obsObstacle;

            _intFromX = PnlDimension.PointToClient(Cursor.Position).X;
            _intFromY = PnlDimension.PointToClient(Cursor.Position).Y;
        }

        /// <summary>
        /// Activated when grabbing an object
        /// </summary>
        /// <param name="intID"></param>
        public void GrabObject(Special spcSpecial)
        {
            _blnDragging = true;
            _blnIsGrabbedSelection = false;
            _blnIsGrabbedObs = false;
            _spcGrabbed = spcSpecial;

            _intFromX = PnlDimension.PointToClient(Cursor.Position).X;
            _intFromY = PnlDimension.PointToClient(Cursor.Position).Y;
        }

        /// <summary>
        /// Activated when dragging a selection
        /// </summary>
        public void GrabSelection()
        {
            _blnDragging = true;
            _blnIsGrabbedSelection = true;

            _intFromX = PnlDimension.PointToClient(Cursor.Position).X;
            _intFromY = PnlDimension.PointToClient(Cursor.Position).Y;
        }

        /// <summary>
        /// Activated when stopping to drag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void StopDragging(object sender, MouseEventArgs e)
        {
            if (_blnDragging)
            {

                string strText = "";

                if (_blnIsGrabbedSelection)
                {
                    for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                        if (Obstacle.List_obsObstacles[i].BlnSelected && Obstacle.List_obsObstacles[i].ObsgrParent.FlParent.FlgrParent.DimParent.IntID == IntID)
                            strText += Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[i]) + "\n";

                    for (int i = 0; i < Special.List_spcSpecials.Count; i++)
                        if (Special.List_spcSpecials[i].BlnSelected && Special.List_spcSpecials[i].SpcgrParent.FlParent.FlgrParent.DimParent.IntID == IntID)
                            strText += Special.CreateTextFromObject(Special.List_spcSpecials[i]) + "\n";
                }
                else
                {
                    if (_blnIsGrabbedObs)
                        strText += Obstacle.CreateTextFromObject(_obsGrabbed) + "\n";
                    else
                        strText += Special.CreateTextFromObject(_spcGrabbed) + "\n";
                }

                if (_intTotalX != 0 || _intTotalY != 0)
                    Log.AddLog(ObjectType.SpecialPlusObstacle, EditingType.MoveGroup, strText, $"{_intTotalX}{Main.chrSEPARATOR}{_intTotalY}");

                ResetDragging();
            }
        }

        /// <summary>
        /// Activated when moving the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Drag(object sender, MouseEventArgs e)
        {
            if (_blnDragging)
            {
                if (Math.Abs(PnlDimension.PointToClient(Cursor.Position).X - _intFromX) >= Main.intUNITSIZE || Math.Abs(_intFromY - PnlDimension.PointToClient(Cursor.Position).Y) >= Main.intUNITSIZE)
                {
                    bool blnIsMoveSuccessful = false;

                    int intXMove = (PnlDimension.PointToClient(Cursor.Position).X - _intFromX) / Main.intUNITSIZE;
                    int intYMove = (_intFromY - PnlDimension.PointToClient(Cursor.Position).Y) / Main.intUNITSIZE;

                    if (!_blnIsGrabbedSelection)
                    {
                        //activated if the thing that is dragged is an obstacle
                        if (_blnIsGrabbedObs)
                        {
                            Position posPosition = new Position()
                            {
                                LocLocation = new Location()
                                {
                                    dblX = _obsGrabbed.IntX + intXMove,
                                    dblY = _obsGrabbed.IntY + intYMove
                                },
                                DisDistance = new Distance()
                                {
                                    dblWidth = _obsGrabbed.IntWidth,
                                    dblHeight = _obsGrabbed.IntHeight
                                }
                            };

                            if (TestEmptiness(posPosition, _obsGrabbed.IntID, true) && posPosition.LocLocation.dblX >= 0 && posPosition.LocLocation.dblY >= 0 && posPosition.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - posPosition.DisDistance.dblWidth && posPosition.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - posPosition.DisDistance.dblHeight)
                            {
                                _obsGrabbed.AddToDefaultLocation(intXMove, intYMove);
                                _intTotalX += intXMove;
                                _intTotalY += intYMove;
                                blnIsMoveSuccessful = true;
                            }
                        }
                        //activated if the thing that is dragged is a special object
                        else
                        {
                            Position posPosition = new Position()
                            {
                                LocLocation = new Location()
                                {
                                    dblX = _spcGrabbed.IntX + intXMove,
                                    dblY = _spcGrabbed.IntY + intYMove
                                },
                                DisDistance = new Distance()
                                {
                                    dblWidth = _spcGrabbed.IntWidth,
                                    dblHeight = _spcGrabbed.IntHeight
                                }
                            };

                            if (TestEmptiness(posPosition, _spcGrabbed.IntID, false) && posPosition.LocLocation.dblX >= 0 && posPosition.LocLocation.dblY >= 0 && posPosition.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - posPosition.DisDistance.dblWidth && posPosition.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - posPosition.DisDistance.dblHeight)
                            {
                                _spcGrabbed.AddToDefaultLocation(intXMove, intYMove);
                                _intTotalX += intXMove;
                                _intTotalY += intYMove;
                                blnIsMoveSuccessful = true;
                            }
                        }
                    }
                    //activated if the thing that is dragged is a selection
                    else
                    {
                        blnIsMoveSuccessful = MoveSelection(intXMove, intYMove);
                        if (blnIsMoveSuccessful)
                        {
                            _intTotalX += intXMove;
                            _intTotalY += intYMove;
                        }
                    }

                    //reset the last movement points
                    if (blnIsMoveSuccessful)
                    {
                        if (intXMove != 0)
                            _intFromX = PnlDimension.PointToClient(Cursor.Position).X - (PnlDimension.PointToClient(Cursor.Position).X - _intFromX) % Main.intUNITSIZE;
                        if (intYMove != 0)
                            _intFromY = PnlDimension.PointToClient(Cursor.Position).Y - (PnlDimension.PointToClient(Cursor.Position).Y - _intFromY) % Main.intUNITSIZE;
                    }
                }
            }
        }

        /// <summary>
        /// Reset the dragging
        /// </summary>
        public void ResetDragging()
        {
            _blnDragging = false;
            _blnIsGrabbedObs = true;
            _blnIsGrabbedSelection = false;
            _obsGrabbed = null;
            _spcGrabbed = null;
            _intFromX = 0;
            _intFromY = 0;
            _intTotalX = 0;
            _intTotalY = 0;
        }
        #endregion

        #region Adding and removing
        /// <summary>
        /// Remove every selected objects
        /// </summary>
        public void RemoveAllSelected() => FlgrFolderGroup.RemoveSelected();

        /// <summary>
        /// Add an obstacle from the selection area
        /// </summary>
        public void AddObstacleFromSelection()
        {
            //test if the area is at least 1x1
            if (_posSelection.DisDistance.dblWidth != 0 && _posSelection.DisDistance.dblHeight != 0 && FlgrFolderGroup.IntCount != 0)
            {
                //add an obstacle using the selection area position
                AddObstacle(_posSelection);

                //reset the selection area
                DelSelectArea();
            }
            else if (FlgrFolderGroup.IntCount == 0)
                MessageBox.Show("You must have at least one folder to add objects in !", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Add special from mouse position
        /// </summary>
        public void AddSpecialFromMousePosition()
        {
            //test if the area is at least 1x1
            if (FlgrFolderGroup.IntCount != 0)
            {
                Position posNewPos = new Position();

                posNewPos.LocLocation.dblX = PnlDimension.PointToClient(Cursor.Position).X / Main.intUNITSIZE;
                posNewPos.LocLocation.dblY = IntHeight - 1 - PnlDimension.PointToClient(Cursor.Position).Y / Main.intUNITSIZE;

                AddSpecial(WrldInWorld.IntSpecialTypeID, (int)posNewPos.LocLocation.dblX, (int)posNewPos.LocLocation.dblY);
            }
            else
                MessageBox.Show("You must have at least one folder to add objects in !", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Add a special object from the selection area
        /// </summary>
        public void AddSpecialFromSelection()
        {
            //test if the area is at least 1x1
            if (_posSelection.DisDistance.dblWidth != 0 && _posSelection.DisDistance.dblHeight != 0 && FlgrFolderGroup.IntCount != 0)
            {
                //add an obstacle using the selection area position
                AddSpecial(WrldInWorld.IntSpecialTypeID, (int)_posSelection.LocLocation.dblX, (int)_posSelection.LocLocation.dblY);

                //reset the selection area
                DelSelectArea();
            }
            else if (FlgrFolderGroup.IntCount == 0)
                MessageBox.Show("You must have at least one folder to add objects in !", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Add a special object area
        /// </summary>
        private void AddSpecial(int intType_,int intX_, int intY_)
        {
            Position posNewPos = new Position();
            posNewPos.DisDistance.dblWidth = SpecialType.List_spctypTypes[SpecialType.GetIndexFromID(intType_)].IntWidth;
            posNewPos.DisDistance.dblHeight = SpecialType.List_spctypTypes[SpecialType.GetIndexFromID(intType_)].IntHeight;
            posNewPos.LocLocation.dblX = intX_;
            posNewPos.LocLocation.dblY = intY_;

            //test if the current position is taken
            if (TestEmptiness(posNewPos) && posNewPos.LocLocation.dblX >= 0 && posNewPos.LocLocation.dblY >= 0 && posNewPos.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - posNewPos.DisDistance.dblWidth && posNewPos.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - posNewPos.DisDistance.dblHeight)
                //add a special object to the list
                FlgrFolderGroup[FlgrFolderGroup.IntSelectedFolderID].AddSpecial(WrldInWorld.IntSpecialTypeID, intX_, intY_);
        }

        /// <summary>
        /// Add an obstacle and it's picturebox to the panel
        /// </summary>
        /// <param name="posPosition"></param>
        private void AddObstacle(Position posPosition)
        {
            //test if the current position is taken
            if (TestEmptiness(posPosition) && posPosition.LocLocation.dblX >= 0 && posPosition.LocLocation.dblY >= 0 && posPosition.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - posPosition.DisDistance.dblWidth && posPosition.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - posPosition.DisDistance.dblHeight)
                //add an obstacle to the list
                FlgrFolderGroup[FlgrFolderGroup.IntSelectedFolderID].AddObstacle(WrldInWorld.IntObstacleTypeID, posPosition);
        }

        /// <summary>
        /// Start the moving of all the moving folders
        /// </summary>
        public void StartMovableObstacles() => FlgrFolderGroup.StartMoving();

        /// <summary>
        /// Stop the moving of all the moving folders
        /// </summary>
        public void StopMovableObstacles() => FlgrFolderGroup.StopMoving();
        #endregion

        #region Test of Emptiness
        /// <summary>
        /// Test if the position is empty
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested)
        {
            if (!FlgrFolderGroup.TestEmptiness(posTested))
                return false;

            return true;
        }

        /// <summary>
        /// Test if the position is empty with an exeption
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested, int intExeption, bool blnIsObstacle)
        {
            if (!FlgrFolderGroup.TestEmptiness(posTested, intExeption, blnIsObstacle))
                return false;

            return true;
        }

        /// <summary>
        /// Test if the position is empty without taking in count the selected obstacles
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested, bool blnSelection)
        {
            if (!FlgrFolderGroup.TestEmptiness(posTested, blnSelection))
                return false;

            return true;
        }
        #endregion

        #region Move Obstacles
        /// <summary>
        /// Move the whole selection of static and movables obstacles
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public bool MoveSelection(int intX, int intY)
        {
            //test if the selection can move
            if (IsSelectionMovable(intX, intY))
            {
                //search in the moving folders
                for (int i = 0; i < FlgrFolderGroup.List_flFolders.Count; i++)
                    for (int ii = 0; ii < FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles.Count; ii++)
                        //test if the obstacle is selected
                        if (FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].BlnSelected)
                            //add to the obstacle
                            FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].AddToDefaultLocation(intX, intY);

                for (int i = 0; i < FlgrFolderGroup.List_flFolders.Count; i++)
                    for (int ii = 0; ii < FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials.Count; ii++)
                        //test if the obstacle is selected
                        if (FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].BlnSelected)
                            //add to the obstacle
                            FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].AddToDefaultLocation(intX, intY);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Detect if the whole selection can move
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// <returns></returns>
        private bool IsSelectionMovable(int intX, int intY)
        {
            //search in folders
            for (int i = 0; i < FlgrFolderGroup.List_flFolders.Count; i++)
                //search in obstacles
                for (int ii = 0; ii < FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles.Count; ii++)
                    //test if the obstacle is selected
                    if (FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].BlnSelected)
                    {
                        //get the position of the last selected obstacle
                        Position posPosition_ = new Position();            //contain the position of the selected obstacle

                        //set the position
                        posPosition_.LocLocation.dblX = FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].IntX;
                        posPosition_.LocLocation.dblY = FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].IntY;
                        posPosition_.DisDistance.dblWidth = FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].IntWidth;
                        posPosition_.DisDistance.dblHeight = FlgrFolderGroup.List_flFolders[i].ObsgrObstacles.List_obsObstacles[ii].IntHeight;

                        //test if the obstacle is movable
                        if (!IsMovable(posPosition_, intX, intY))
                            return false;
                    }

            //search in folders
            for (int i = 0; i < FlgrFolderGroup.List_flFolders.Count; i++)
                //search in the specials
                for (int ii = 0; ii < FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials.Count; ii++)
                    //test if the special object is selected
                    if (FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].BlnSelected)
                    {
                        //get the position of the last selected obstacle
                        Position posPosition_ = new Position();            //contain the position of the selected obstacle

                        //set the position
                        posPosition_.LocLocation.dblX = FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].IntX;
                        posPosition_.LocLocation.dblY = FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].IntY;
                        posPosition_.DisDistance.dblWidth = FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].IntWidth;
                        posPosition_.DisDistance.dblHeight = FlgrFolderGroup.List_flFolders[i].SpcgrSpecials.List_spcSpecials[ii].IntHeight;

                        //test if the obstacle is movable
                        if (!IsMovable(posPosition_, intX, intY))
                            return false;
                    }

            return true;
        }

        /// <summary>
        /// Detect if the obstacle can move
        /// </summary>
        /// <param name="posPosition_"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// <returns></returns>
        private bool IsMovable(Position posPosition_, int intX, int intY)
        {
            //create the new position
            posPosition_.LocLocation.dblX += intX;
            posPosition_.LocLocation.dblY += intY;

            //test if the new position is free
            if (TestEmptiness(posPosition_, false) && posPosition_.LocLocation.dblX >= 0 && posPosition_.LocLocation.dblY >= 0 && posPosition_.LocLocation.dblX <= _posPosition.DisDistance.dblWidth - posPosition_.DisDistance.dblWidth && posPosition_.LocLocation.dblY <= _posPosition.DisDistance.dblHeight - posPosition_.DisDistance.dblHeight)
                return true;

            return false;
        }
        #endregion

        #region Properties Form
        //declare the control object used to display the properties form
        private Form _frmDimension;                 //dimension properties form
        private TextBox _txtbxName;                 //textbox used to change the name of the dimension or the world
        private TextBox _txtbxWidth;                //textbox used to change the width of the dimension
        private TextBox _txtbxHeight;               //textbox used to change the height of the dimension
        private PictureBox _pcbxBackground;         //picturebox used to display the current dimension's background
        private Label _lblHeight;                   //label informing that the textbox next to it is for the height
        private Label _lblWidth;                    //label informing that the textbox next to it is for the width
        private Label _lblName;                     //label informing that the textbox next to it is for the name
        private Button _btnGetImage;                //button used to start the searching of a background
        private Button _btnCancel;                  //button used to close the properties form
        private Button _btnValidate;                //button used to validate the new values

        /// <summary>
        /// Open the properties dimension form
        /// </summary>
        /// <param name="intIndex"></param>
        public void OpenPropertiesForm()
        {
            //reset all the controls
            _frmDimension = new Form();
            _txtbxName = new TextBox();
            _txtbxWidth = new TextBox();
            _txtbxHeight = new TextBox();
            _pcbxBackground = new PictureBox();
            _lblHeight = new Label();
            _lblWidth = new Label();
            _lblName = new Label();
            _btnGetImage = new Button();
            _btnCancel = new Button();
            _btnValidate = new Button();

            #region Controls
            //suspend the layout
            _frmDimension.SuspendLayout();

            //set the default values of the name textbox
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //set the default values of the width textbox
            _txtbxWidth.Location = new Point(202, 69);
            _txtbxWidth.Name = "txtbxWidth";
            _txtbxWidth.Size = new Size(36, 20);
            _txtbxWidth.TabIndex = 1;
            _txtbxWidth.Text = $"{IntWidth}";
            _txtbxWidth.TextAlign = HorizontalAlignment.Right;

            //set the default values of the height textbox
            _txtbxHeight.Location = new Point(202, 95);
            _txtbxHeight.Name = "txtbxHeight";
            _txtbxHeight.Size = new Size(36, 20);
            _txtbxHeight.TabIndex = 2;
            _txtbxHeight.Text = $"{IntHeight}";
            _txtbxHeight.TextAlign = HorizontalAlignment.Right;

            //set the default values of the background picturebox
            _pcbxBackground.Location = new Point(100, 138);
            _pcbxBackground.Name = "pcbxBackground";
            _pcbxBackground.Size = new Size(100, 79);
            _pcbxBackground.TabIndex = 3;
            _pcbxBackground.BackgroundImageLayout = ImageLayout.Zoom;
            _pcbxBackground.BackgroundImage = ImgBackgroundImage;
            _pcbxBackground.TabStop = false;

            //set the default values of the height label
            _lblHeight.AutoSize = true;
            _lblHeight.Location = new Point(36, 95);
            _lblHeight.Name = "lblHeight";
            _lblHeight.Size = new Size(44, 13);
            _lblHeight.TabIndex = 4;
            _lblHeight.Text = "Height :";

            //set the default values of the width label
            _lblWidth.AutoSize = true;
            _lblWidth.Location = new Point(36, 69);
            _lblWidth.Name = "lblWidth";
            _lblWidth.Size = new Size(41, 13);
            _lblWidth.TabIndex = 5;
            _lblWidth.Text = "Width :";

            //set the default values of the name label
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "Name :";

            //set the default values of the image giver button
            _btnGetImage.Location = new Point(91, 218);
            _btnGetImage.Name = "btnGetImage";
            _btnGetImage.Size = new Size(120, 23);
            _btnGetImage.TabIndex = 7;
            _btnGetImage.Text = "Search a background";
            _btnGetImage.UseVisualStyleBackColor = true;
            _btnGetImage.Click += new EventHandler(btnGetImage_Click);

            //set the default values of the cancel button
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 269);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(Cancel_Click);

            //set the default values of the validation button
            _btnValidate.Location = new Point(121, 268);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Validate";
            _btnValidate.Tag = IntID;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //set the default values of the main properties form
            _frmDimension.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmDimension.AutoScaleMode = AutoScaleMode.Font;
            _frmDimension.AcceptButton = _btnValidate;
            _frmDimension.CancelButton = _btnCancel;
            _frmDimension.ClientSize = new Size(304, 303);
            _frmDimension.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmDimension.Name = "frmDimension";
            _frmDimension.StartPosition = FormStartPosition.CenterParent;
            _frmDimension.Text = "Dimension";
            _frmDimension.Tag = IntID;

            //add the controls to the form
            _frmDimension.Controls.Add(_btnValidate);
            _frmDimension.Controls.Add(_btnCancel);
            _frmDimension.Controls.Add(_btnGetImage);
            _frmDimension.Controls.Add(_lblName);
            _frmDimension.Controls.Add(_lblWidth);
            _frmDimension.Controls.Add(_lblHeight);
            _frmDimension.Controls.Add(_pcbxBackground);
            _frmDimension.Controls.Add(_txtbxHeight);
            _frmDimension.Controls.Add(_txtbxWidth);
            _frmDimension.Controls.Add(_txtbxName);

            //resume the layout
            _frmDimension.ResumeLayout(false);
            _frmDimension.PerformLayout();
            #endregion

            //show the form
            _frmDimension.ShowDialog();
        }

        /// <summary>
        /// Get an image from the user's files
        /// </summary>
        /// <returns></returns>
        public Image GetImage()
        {
            //declare the control used to get an image from a file
            OpenFileDialog ofdDialog = new OpenFileDialog
            {

                //set the title
                Title = "Open Image",

                //set the filter
                Filter = "Image files|*.png"
            };        //used to search a image file

            //test if the file result is ok
            if (ofdDialog.ShowDialog() == DialogResult.OK)
            {
                //return the image
                return Image.FromFile(ofdDialog.FileName);
            }
            else
            {
                //inform the user the file is invalid
                MessageBox.Show("File invalid");
                return null;
            }
        }

        #region Events
        /// <summary>
        /// Search an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetImage_Click(object sender, EventArgs e)
        {
            //get the image
            _pcbxBackground.BackgroundImage = GetImage();

            if (_pcbxBackground == null)
                _pcbxBackground.BackgroundImage = ImgBackgroundImage;

            //set the layout
            _pcbxBackground.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmDimension.Close();

        /// <summary>
        /// Validate the values if they are valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate_Click(object sender, EventArgs e)
        {
            //declare the variables used to test the validity
            string strErrorMessage = "";            //used to tell all the errors
            bool blnAreAllValuesValid = true;       //used to tell if all the values are valid
            bool blnIsSizeValid = true;             //used to tell if all the numerical values are valid

            //declare the variables storing the new values
            string strName_ = _txtbxName.Text;       //store the new name
            int intWidth_;                          //store the new width
            int intHeight_;                         //store the new height
            Image imgBackgroundImage_ = null;     //store the new background

            //test if there is a new image
            if (_pcbxBackground.BackgroundImage != null)
                //get the new background
                imgBackgroundImage_ = _pcbxBackground.BackgroundImage;

            #region Verify Validity
            //test if the new name is taken
            if (DimgrParent.IsDimensionNameTaken($"{strName_}", StrName))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the user that the new name is taken
                strErrorMessage += $"The new name is already taken !\n";
            }

            //test if the new name is null
            if (strName_.Length < Main.intMINNAMESIZE || strName_.Length > Main.intMAXNAMESIZE)
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the user that not all of his values are valid
                strErrorMessage += $"The new name must have at least {Main.intMINNAMESIZE} characters and max {Main.intMAXNAMESIZE} characters !\n";
            }

            if (strName_.ToLower().Contains($"{Main.chrSEPARATOR}"))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the user that not all of his values are valid
                strErrorMessage += $"The new name must not have a \"{$"{Main.chrSEPARATOR}"}\" inside !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxWidth.Text, out intWidth_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnIsSizeValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new width is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxHeight.Text, out intHeight_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnIsSizeValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new height is not an integrer !\n";
            }

            //test if all the numerical values are converted successfully
            if (blnIsSizeValid)
            {
                //test if the new width is smaller that the minimum size
                if (intWidth_ < Main.intMINSIZE)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new width cannot be less than {Main.intMINSIZE} !\n";
                }

                //test if the new width is bigger that the maximum size
                if (intWidth_ > Main.intMAXSIZE)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new width cannot be more than {Main.intMAXSIZE} !\n";
                }

                //test if the new height is smaller than the minimum size
                if (intHeight_ < Main.intMINSIZE)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new height cannot be less than {Main.intMINSIZE} !\n";
                }

                //test if the new height is bigger thatn the maximum size
                if (intHeight_ > Main.intMAXSIZE)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new height cannot be more than {Main.intMAXSIZE} !\n";
                }
            }

            //test if the new background is not equal to null
            if (imgBackgroundImage_ != null)
                //test if the new background is not valid
                if (blnIsSizeValid && Main.intUNITSIZE * intWidth_ != imgBackgroundImage_.Width && Main.intUNITSIZE * intHeight_ != imgBackgroundImage_.Height)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the image size is not valid
                    strErrorMessage += $"The image doesn't have the right size, \nThe image must be {imgBackgroundImage_.Width}x{imgBackgroundImage_.Height}p\n";
                }
            #endregion

            //test if all the values are valid
            if (blnAreAllValuesValid)
            {
                string strSaveLog = CreateTextFromObject(this);
                Image imgSave = null;

                if (ImgBackgroundImage != null)
                    imgSave = new Bitmap(ImgBackgroundImage);
                else
                    imgSave = null;

                //change all the values
                StrName = strName_;
                IntWidth = intWidth_;
                IntHeight = intHeight_;
                ImgBackgroundImage = imgBackgroundImage_;

                Log.AddLog(ObjectType.Dimension, EditingType.ChangeProperties, strSaveLog, CreateTextFromObject(this), imgSave, ImgBackgroundImage);

                //close the form
                _frmDimension.Close();

                //dispose all the data
                _frmDimension.Dispose();
            }
            else
                //inform the user of all the errors
                MessageBox.Show(strErrorMessage, "One or multiple values are invalid !", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
        #endregion

        #region Perso Event
        /// <summary>
        /// Triggered after a visual value has been changed
        /// </summary>
        /// <param name="intSenderID"></param>
        /// <param name="objSender"></param>
        /// <param name="typSender"></param>
        private void UpdateTreeView(int intSenderID, object objSender, Type typSender) 
        {
            if (Main.WrldWorld != null)
                WrldInWorld.UpdateTreeNodes(); 
        }
        #endregion
    }
}