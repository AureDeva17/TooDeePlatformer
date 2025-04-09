/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                         **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 02.06.2021                                                    **
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
** Obstacle class serving to define an obstacle. An obstacle being an object **
** keeping the player from passing through it.                               **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Obstacle class serving to define an obstacle. An obstacle being an object keeping the player from passing through it.
    /// </summary>
    class Obstacle
    {
        //declare the static properties
        public const string strCLASSNAME = "Obstacle";                  //class name
        public const ObjectType objTYPE = ObjectType.Obstacle;          //object type
        public static int IntNextID { get; set; }                       //next ID to be used
        public static List<Obstacle> List_obsObstacles { get; set; }    //list of all the obstacles

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_obsObstacles.Count; i++)
                if (List_obsObstacles[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_obsObstacles[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static Obstacle GetObjectByID(int intID) => List_obsObstacles[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
                string strText = Main.strVERSION + "\n";

                strText += strCLASSNAME + "\n";
                strText += IntNextID + "\n";
                strText += List_obsObstacles.Count + "\n";

                for (int i = 0; i<List_obsObstacles.Count; i++)
                {
                    strText += CreateTextFromObject(List_obsObstacles[i]);

                    if (List_obsObstacles.Count != i)
                        strText += "\n";
                }

                return strText;
        }

        /// <summary>
        /// Copy the data from the file to the obstacles
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrObstacleTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_obsObstacles.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_obsObstacles.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                    
                }
                else
                    MessageBox.Show("The file isn't a storage for obstacles !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a line of text from an object
        /// </summary>
        /// <param name="obsObject"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(Obstacle obsObject)
        {
            string strText = "";

            strText += obsObject.IntID + $"{Main.chrSEPARATOR}";
            strText += obsObject.IntFolderID + $"{Main.chrSEPARATOR}";
            strText += obsObject.StrName + $"{Main.chrSEPARATOR}";
            strText += obsObject.IntTypeID + $"{Main.chrSEPARATOR}";
            strText += obsObject.IntX + $"{Main.chrSEPARATOR}";
            strText += obsObject.IntY + $"{Main.chrSEPARATOR}";
            strText += obsObject.IntWidth + $"{Main.chrSEPARATOR}";
            strText += obsObject.IntHeight;

            return strText;
        }

        /// <summary>
        /// Create an object from a line of text
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static Obstacle CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            Obstacle obsNewObstacle = new Obstacle()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntFolderID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                IntTypeID = Convert.ToInt32(tab_strObject[3]),
                _intDefaultX = Convert.ToInt32(tab_strObject[4]),
                _intDefaultY = Convert.ToInt32(tab_strObject[5]),
                _posPosition = new Position()
                {
                    DisDistance = new Distance()
                    {
                        dblWidth = Convert.ToInt32(tab_strObject[6]),
                        dblHeight = Convert.ToInt32(tab_strObject[7])
                    },
                    LocLocation = new Location()
                    {
                        dblX = Convert.ToInt32(tab_strObject[4]),
                        dblY = Convert.ToInt32(tab_strObject[5])
                    },
                },
            };

            if (!blnOnlyRead)
            {
                //get the picture box of the obstacle
                obsNewObstacle._pcbxPictureBox.MouseMove += new MouseEventHandler(obsNewObstacle.ObsgrParent.FlParent.FlgrParent.DimParent.Drag);
                obsNewObstacle._pcbxPictureBox.MouseUp += new MouseEventHandler(obsNewObstacle.ObsgrParent.FlParent.FlgrParent.DimParent.StopDragging);
                obsNewObstacle._pcbxPictureBox.MouseDown += new MouseEventHandler(obsNewObstacle.ClickObject);

                //create the picture box
                obsNewObstacle._pcbxPictureBox.Size = obsNewObstacle._posPosition.ToSize();
                obsNewObstacle._pcbxPictureBox.Location = obsNewObstacle._posPosition.ToPoint(obsNewObstacle.IntDimensionHeight);
                obsNewObstacle._pcbxPictureBox.ContextMenuStrip = obsNewObstacle.GetContextMenu();
                obsNewObstacle._pcbxPictureBox.Name = obsNewObstacle.ObstypType.StrName;
                obsNewObstacle._pcbxPictureBox.BackgroundImage = obsNewObstacle.ObstypType.ImgTexture;
                obsNewObstacle._pcbxPictureBox.BackgroundImageLayout = obsNewObstacle.ObstypType.ImglytLayout;

                obsNewObstacle.ObsgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Add(obsNewObstacle.GetPictureBox());
            }

            return obsNewObstacle;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static Obstacle()
        {
            IntNextID = 0;
            List_obsObstacles = new List<Obstacle>();
        }
        #endregion

        //declare events
        public event dlgPersoEvent evtValuesChanged;             //triggered when a property is changed
        
        //declare the properties
        public int IntID { get; set; }                          //get the ID of the object
        public int IntFolderID { get; set; }                    //contain the parent

        //declare all the variables
        private PictureBox _pcbxPictureBox;                     //get the picture box of the obstacle
        private Position _posPosition;                          //position of the obstacle
        private int _intDefaultY;                               //default Y position
        private int _intDefaultX;                               //default X position
        private string _strName;                                //name of the obstacle
        private bool _blnSelected;                              //is selected
        private int _intTypeID;                                 //type of the obstacle             

        #region Properties
        /// <summary>
        /// height of the dimension the object is in
        /// </summary>
        private int IntDimensionHeight
        {
            get => ObsgrParent.FlParent.FlgrParent.DimParent.IntHeight;
        }

        /// <summary>
        /// Get the width of the obstacle and set the width to the value and the picturebox
        /// </summary>
        public int IntWidth
        {
            get => (int)_posPosition.DisDistance.dblWidth;

            set
            {
                //set the new picture width
                _posPosition.DisDistance.dblWidth = value;
                _pcbxPictureBox.Width = value * Main.intUNITSIZE;
            }
        }

        /// <summary>
        /// Get the height of the obstacle and set the height to the value and the picturebox
        /// </summary>
        public int IntHeight
        {
            get => (int)_posPosition.DisDistance.dblHeight;

            set
            {
                //set the new obstacle height
                _posPosition.DisDistance.dblHeight = value;
                _pcbxPictureBox.Height = value * Main.intUNITSIZE;
            }
        }

        /// <summary>
        /// Get the X positon of the obstacle and set the position of the obstacle
        /// </summary>
        public int IntX
        {
            get => _intDefaultX;

            set
            {
                //set the new X
                _intDefaultX = value;
                _posPosition.LocLocation.dblX = value;
                UpdatePictureBox();
            }
        }

        /// <summary>
        /// Get the Y positon of the obstacle and set the position of the obstacle
        /// </summary>
        public int IntY
        {
            get => _intDefaultY;

            set
            {
                //set the new Y
                _intDefaultY = value;
                _posPosition.LocLocation.dblY = value;
                UpdatePictureBox();
            }
        }

        /// <summary>
        /// Get and set the new name then invoke the values changed event
        /// </summary>
        public string StrName
        {
            get => _strName;
            
            set
            {
                if (!ObsgrParent.IsObstacleNameTaken(value, StrName))
                {
                    //set the new name
                    _strName = value;
                    evtValuesChanged?.Invoke(IntID, this, GetType());
                }
            }
        }

        /// <summary>
        /// Get the selected status of the obstacle and set the status while changing the image
        /// </summary>
        public bool BlnSelected
        {
            get => _blnSelected;

            set
            {
                //reset the image
                if (_blnSelected && !value)
                    ResetImage();

                //put a new blue filter on the image
                if (!_blnSelected && value)
                {
                    //bitmap of the image
                    Bitmap Bitmap = new Bitmap(_pcbxPictureBox.BackgroundImage);

                    //color of the current pixel
                    Color Color;
                    for (int i = 0; i < Bitmap.Height; i++)
                        for (int ii = 0; ii < Bitmap.Width; ii++)
                        {
                            Color = Bitmap.GetPixel(ii, i);
                            if (Color.B + 200 < 255)
                                Color = Color.FromArgb(Color.R, Color.G, Color.B + 200);
                            else
                                Color = Color.FromArgb(Color.R, Color.G, 255);
                            Bitmap.SetPixel(ii, i, Color);
                        }

                    //set the new background
                    _pcbxPictureBox.BackgroundImage = Bitmap;
                }

                //set the status
                _blnSelected = value;
            }
        }

        /// <summary>
        /// Get and set the type while updating its image
        /// </summary>
        public int IntTypeID
        {
            get => _intTypeID;

            //set the new type
            set
            {
                //verify the validity of the type
                int intType__ = value;

                if (WrldInWorld.ObstypgrTypes[value] == null)
                    intType__ = 0;

                //set the new type
                _intTypeID = intType__;

                //reset the image
                if (_pcbxPictureBox != null)
                    ResetImage();
            }
        }

        /// <summary>
        /// Get and set the type
        /// </summary>
        public ObstacleType ObstypType
        {
            get => ObstacleType.List_obstypTypes[ObstacleType.GetIndexFromID(IntTypeID)];
        }

        /// <summary>
        /// Get the parent
        /// </summary>
        public ObstacleGroup ObsgrParent
        {
            get => Folder.List_flFolders[Folder.GetIndexFromID(IntFolderID)].ObsgrObstacles;
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => ObsgrParent.FlParent.FlgrParent.DimParent.DimgrParent.WrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values of the obstacle
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="intType"></param>
        /// <param name="Position_"></param>
        /// <param name="intDimensionHeight_"></param>
        public Obstacle(string strName_, int intType, Position Position_, int intFolderID)
        {
            //set the default values
            IntFolderID = intFolderID;
            IntID = IntNextID++;
            IntTypeID = intType;
            StrName = strName_;
            _posPosition = Position_;
            _intDefaultX = (int)Position_.LocLocation.dblX;
            _intDefaultY = (int)Position_.LocLocation.dblY;

            //create the picture box
            _pcbxPictureBox = new PictureBox
            {
                Size = Position_.ToSize(),
                Location = Position_.ToPoint(IntDimensionHeight),
                ContextMenuStrip = GetContextMenu(),
                Name = ObstypType.StrName,
                BackgroundImage = ObstypType.ImgTexture,
                BackgroundImageLayout = ObstypType.ImglytLayout,
                Cursor = Cursors.Hand
            };

            _pcbxPictureBox.MouseMove += new MouseEventHandler(ObsgrParent.FlParent.FlgrParent.DimParent.Drag);
            _pcbxPictureBox.MouseUp += new MouseEventHandler(ObsgrParent.FlParent.FlgrParent.DimParent.StopDragging);
            _pcbxPictureBox.MouseDown += new MouseEventHandler(ClickObject);

            //set it to unselected
            BlnSelected = false;
            ObsgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Add(GetPictureBox());

            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create an obstacle from a file
        /// </summary>
        public Obstacle()
        {
            _pcbxPictureBox = new PictureBox() { Cursor = Cursors.Hand };
            BlnSelected = false;
            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }
        #endregion

        #region Location
        /// <summary>
        /// Add to the default location of the obstacle
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void AddToDefaultLocation(int intX, int intY)
        {
            this.IntX += intX;
            this.IntY += intY;
            _posPosition.LocLocation.dblX = this.IntX;
            _posPosition.LocLocation.dblY = this.IntY;
            _pcbxPictureBox.Size = _posPosition.ToSize();
            _pcbxPictureBox.Location = _posPosition.ToPoint(IntDimensionHeight);
        }

        /// <summary>
        /// Set the default location of the obstacle
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void SetDefaultLocation(int intX, int intY)
        {
            this.IntX = intX;
            this.IntY = intY;
            _posPosition.LocLocation.dblX = this.IntX;
            _posPosition.LocLocation.dblY = this.IntY;
            _pcbxPictureBox.Size = _posPosition.ToSize();
            _pcbxPictureBox.Location = _posPosition.ToPoint(IntDimensionHeight);
        }

        /// <summary>
        /// Add to the virtual location 
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        public void AddToLocation(double dblX, double dblY)
        {
            _posPosition.LocLocation.dblX += dblX;
            _posPosition.LocLocation.dblY += dblY;
            _pcbxPictureBox.Size = _posPosition.ToSize();
            _pcbxPictureBox.Location = _posPosition.ToPoint(IntDimensionHeight);
        }

        /// <summary>
        /// Set the virutal location
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        public void SetLocation(double dblX, double dblY)
        {
            _posPosition.LocLocation.dblX = dblX;
            _posPosition.LocLocation.dblY = dblY;
            _pcbxPictureBox.Size = _posPosition.ToSize();
            _pcbxPictureBox.Location = _posPosition.ToPoint(IntDimensionHeight);
        }

        /// <summary>
        /// Get the temp X location
        /// </summary>
        /// <returns></returns>
        public double GetTempX() => _posPosition.LocLocation.dblX;

        /// <summary>
        /// Get the temp Y location
        /// </summary>
        /// <returns></returns>
        public double GetTempY() => _posPosition.LocLocation.dblY;
        #endregion

        #region Picture Box
        /// <summary>
        /// Reset the background image to take out the blue filter
        /// </summary>
        public void ResetImage()
        {
            _pcbxPictureBox.BackgroundImage = ObstypType.ImgTexture;
            _pcbxPictureBox.BackgroundImageLayout = ObstypType.ImglytLayout;
        }

        /// <summary>
        /// Update the size and the position of the picturebox
        /// </summary>
        public void UpdatePictureBox()
        {
            _pcbxPictureBox.Size = _posPosition.ToSize();
            _pcbxPictureBox.Location = _posPosition.ToPoint(IntDimensionHeight);
        }

        /// <summary>
        /// Get the picture box
        /// </summary>
        /// <returns></returns>
        public PictureBox GetPictureBox() => _pcbxPictureBox;

        /// <summary>
        /// Delete the picture box
        /// </summary>
        public void DeletePictureBox()
        {
            _pcbxPictureBox.Hide();
            _pcbxPictureBox.Dispose();
        }

        /// <summary>
        /// Activated when grabbing the object
        /// </summary>
        private void ClickObject(object sender, MouseEventArgs e)
        {
            if (!BlnSelected)
                ObsgrParent.FlParent.FlgrParent.DimParent.GrabObject(this);
            else
                ObsgrParent.FlParent.FlgrParent.DimParent.GrabSelection();

            if (WrldInWorld.BlnPressingCtrl)
                BlnSelected = !BlnSelected;
        }
        #endregion

        #region Select
        /// <summary>
        /// Select the obtacle present inside the selection area
        /// </summary>
        public void SelectArea(Position posArea, bool blnSelect)
        {
            //test if the position is intersecting the selection area
            if (_posPosition.Rectangle().IntersectsWith(posArea.Rectangle()) || posArea.Rectangle().IntersectsWith(_posPosition.Rectangle()))
                //select the obstacle
                BlnSelected = blnSelect;
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node representing the obstacle
        /// </summary>
        public TreeNode GetTreeNode() => new TreeNode(StrName) { ContextMenuStrip = GetContextMenu()};
        #endregion

        #region Context box
        /// <summary>
        /// Get the context menu of the object
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare everything needed to create a context menu
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                     //contain the context menu
            List<ToolStripMenuItem> tsmiItems = new List<ToolStripMenuItem>
            {

                //tool used to select the obstacle
                new ToolStripMenuItem("Select Ctrl + Click")
            };              //contain all the items used to create the context menu
            tsmiItems[0].Tag = IntID;
            tsmiItems[0].Click += new EventHandler(SelectObstacleMenu);

            //tool used to unselect the obstacle
            tsmiItems.Add(new ToolStripMenuItem("Unselect Ctrl + Click"));
            tsmiItems[1].Tag = IntID;
            tsmiItems[1].Click += new EventHandler(UnselectObstacleMenu);

            //tool used to remove the obstacle
            tsmiItems.Add(new ToolStripMenuItem("Remove"));
            tsmiItems[2].Tag = IntID;
            tsmiItems[2].Click += new EventHandler(RemoveObstacleMenu);

            //tool used to access the properties of the obstacle
            tsmiItems.Add(new ToolStripMenuItem("Properties"));
            tsmiItems[3].Tag = IntID;
            tsmiItems[3].Click += new EventHandler(ModifyObstaclePropertiesMenu);

            //add the tools to the context menu
            contextMenuStrip.Items.AddRange(tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Remove the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveObstacleMenu(object sender, EventArgs e)
        {
            //search for the obstacle
            for (int i = 0; i < List_obsObstacles.Count; i++)
                if (List_obsObstacles[i].IntID == IntID)
                {
                    ObsgrParent.Remove(i);
                    evtValuesChanged?.Invoke(IntID, this, GetType());

                    break;
                }
        }

        /// <summary>
        /// Select the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectObstacleMenu(object sender, EventArgs e) => BlnSelected = true;

        /// <summary>
        /// Unselect the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectObstacleMenu(object sender, EventArgs e) => BlnSelected = false;

        /// <summary>
        /// Open the properties form of the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyObstaclePropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Properties Form
        //declare the controls
        private Form _frmObstacle;          //obstacle form
        private TextBox _txtbxName;         //textbox containing the name and gives the ability to change it
        private TextBox _txtbxWidth;        //textbox containing the width
        private TextBox _txtbxHeight;       //textbox containing the height
        private TextBox _txtbxX;            //textbox containing the X location
        private TextBox _txtbxY;            //textbox containing the Y location
        private ComboBox _cbbxType;         //combobox containing the type of the part and gives the ability to change it
        private Label _lblName;             //tells the user the textbox next to it is the naming one
        private Label _lblWidth;            //tells the user the textbox next to it is the width
        private Label _lblHeight;           //tells the user the textbox next to it is the height
        private Label _lblX;                //tells the user the textbox next to it is the X location one
        private Label _lblY;                //tells the user the textbox next to it is the Y location one
        private Label _lblType;             //tells the user the combobox next to it is the type one
        private Button _btnCancel;          //close the form
        private Button _btnValidate;        //test the validity of the new values

        public void OpenPropertiesForm()
        {
            //reset the controls
            _frmObstacle = new Form();
            _txtbxName = new TextBox();
            _txtbxWidth = new TextBox();
            _txtbxHeight = new TextBox();
            _lblHeight = new Label();
            _lblWidth = new Label();
            _lblName = new Label();
            _btnCancel = new Button();
            _btnValidate = new Button();
            _cbbxType = new ComboBox();
            _txtbxX = new TextBox();
            _txtbxY = new TextBox();
            _lblX = new Label();
            _lblY = new Label();
            _lblType = new Label();

            #region Controls
            //suspend the layout
            _frmObstacle.SuspendLayout();

            //textbox containing the name and give the ability to change it
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //textbox containing the width and give the ability to change it
            _txtbxWidth.Location = new Point(202, 69);
            _txtbxWidth.Name = "txtbxWidth";
            _txtbxWidth.Size = new Size(36, 20);
            _txtbxWidth.TabIndex = 1;
            _txtbxWidth.Text = $"{IntWidth}";
            _txtbxWidth.TextAlign = HorizontalAlignment.Right;
            _txtbxWidth.Enabled = true;

            //textbox containing the height and give the ability to change it
            _txtbxHeight.Location = new Point(202, 95);
            _txtbxHeight.Name = "txtbxHeight";
            _txtbxHeight.Size = new Size(36, 20);
            _txtbxHeight.TabIndex = 2;
            _txtbxHeight.Text = $"{IntHeight}";
            _txtbxHeight.TextAlign = HorizontalAlignment.Right;
            _txtbxHeight.Enabled = true;

            //textbox containing the X location and give the ability to change it
            _txtbxX.Location = new Point(202, 121);
            _txtbxX.Name = "txtbxX";
            _txtbxX.Size = new Size(36, 20);
            _txtbxX.TabIndex = 2;
            _txtbxX.Text = $"{IntX}";
            _txtbxX.TextAlign = HorizontalAlignment.Right;
            _txtbxX.Enabled = true;

            //textbox containing the Y location and give the ability to change it
            _txtbxY.Location = new Point(202, 147);
            _txtbxY.Name = "txtbxY";
            _txtbxY.Size = new Size(36, 20);
            _txtbxY.TabIndex = 2;
            _txtbxY.Text = $"{IntY}";
            _txtbxY.TextAlign = HorizontalAlignment.Right;
            _txtbxY.Enabled = true;

            //combobox containing the types and give the ability to change it
            _cbbxType.Location = new Point(100, 173);
            _cbbxType.Name = "txtbxHeight";
            _cbbxType.Size = new Size(138, 20);
            _cbbxType.TabIndex = 3;
            List<string> list_strTypes = new List<string>();
            for (int i = 0; i < WrldInWorld.ObstypgrTypes.List_obstypTypes.Count; i++)
                list_strTypes.Add(WrldInWorld.ObstypgrTypes.List_obstypTypes[i].StrName);
            _cbbxType.Items.AddRange(list_strTypes.ToArray());
            for (int i = 0; i < WrldInWorld.ObstypgrTypes.List_obstypTypes.Count; i++)
                if (WrldInWorld.ObstypgrTypes.List_obstypTypes[i] == WrldInWorld.ObstypgrTypes[IntTypeID])
                    _cbbxType.SelectedIndex = i;

            //label telling the textbox next to it is the naming one
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "Name :";

            //label telling the textbox next to it is the width one
            _lblWidth.AutoSize = true;
            _lblWidth.Location = new Point(36, 69);
            _lblWidth.Name = "lblWidth";
            _lblWidth.Size = new Size(41, 13);
            _lblWidth.TabIndex = 5;
            _lblWidth.Text = "Width :";

            //label telling the textbox next to it is the height one
            _lblHeight.AutoSize = true;
            _lblHeight.Location = new Point(36, 95);
            _lblHeight.Name = "lblHeight";
            _lblHeight.Size = new Size(44, 13);
            _lblHeight.TabIndex = 4;
            _lblHeight.Text = "Height :";

            //label telling the textbox next to it is the X location one
            _lblX.AutoSize = true;
            _lblX.Location = new Point(36, 121);
            _lblX.Name = "lblX";
            _lblX.Size = new Size(41, 13);
            _lblX.TabIndex = 5;
            _lblX.Text = "Location in X :";

            //label telling the textbox next to it is the Y location one
            _lblY.AutoSize = true;
            _lblY.Location = new Point(36, 147);
            _lblY.Name = "lblY";
            _lblY.Size = new Size(41, 13);
            _lblY.TabIndex = 5;
            _lblY.Text = "Location in Y :";

            //label telling the the combobox next to it is the type one
            _lblType.AutoSize = true;
            _lblType.Location = new Point(36, 173);
            _lblType.Name = "lblType";
            _lblType.Size = new Size(44, 13);
            _lblType.TabIndex = 5;
            _lblType.Text = "Type :";

            //close the form
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 199);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(Cancel_Click);

            //validate the new values
            _btnValidate.Location = new Point(121, 199);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Validate";
            _btnValidate.Tag = IntID;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //form
            _frmObstacle.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmObstacle.AutoScaleMode = AutoScaleMode.Font;
            _frmObstacle.AcceptButton = _btnValidate;
            _frmObstacle.CancelButton = _btnCancel;
            _frmObstacle.ClientSize = new Size(304, 250);
            _frmObstacle.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmObstacle.Name = "frmObstacle";
            _frmObstacle.StartPosition = FormStartPosition.CenterParent;
            _frmObstacle.Text = "Obstacle";
            _frmObstacle.Tag = IntID;

            //add the controls
            _frmObstacle.Controls.Add(_btnValidate);
            _frmObstacle.Controls.Add(_btnCancel);
            _frmObstacle.Controls.Add(_lblName);
            _frmObstacle.Controls.Add(_lblWidth);
            _frmObstacle.Controls.Add(_lblHeight);
            _frmObstacle.Controls.Add(_txtbxHeight);
            _frmObstacle.Controls.Add(_lblX);
            _frmObstacle.Controls.Add(_lblY);
            _frmObstacle.Controls.Add(_txtbxX);
            _frmObstacle.Controls.Add(_txtbxY);
            _frmObstacle.Controls.Add(_lblType);
            _frmObstacle.Controls.Add(_cbbxType);
            _frmObstacle.Controls.Add(_txtbxWidth);
            _frmObstacle.Controls.Add(_txtbxName);

            //resume the layout
            _frmObstacle.ResumeLayout(false);
            _frmObstacle.PerformLayout();
            #endregion

            //show the form
            _frmObstacle.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the part form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmObstacle.Close();

        /// <summary>
        /// Test the validity of the new values and change them if they are valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate_Click(object sender, EventArgs e)
        {
            //declare all the variables needed to test the validity
            string strErrorMessage = "";            //contain the error message
            bool blnAreAllValuesValid = true;       //contain the validity of the value
            bool blnAreNumsValid = true;             //used to tell if all the numerical values are valid

            string strNewName_ = _txtbxName.Text;        //contain the new name of the part
            int intWidth_;                              //contain the new width
            int intHeight_;                             //contain the new height
            int intX_;                                  //contain the new X position
            int intY_;                                  //contain the new Y position

            #region Verify Validity
            //test if the name is taken
            if (ObsgrParent.IsObstacleNameTaken($"{strNewName_}", StrName))
            {
                //tell the game that the values are invalid
                blnAreAllValuesValid = false;

                //tell the user the value is invalid
                strErrorMessage += $"The new name is already taken !\n";
            }

            //test if the new name is null
            if (strNewName_.Length < Main.intMINNAMESIZE || strNewName_.Length > Main.intMAXNAMESIZE)
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the user that not all of his values are valid
                strErrorMessage += $"The new name must have at least {Main.intMINNAMESIZE} characters and max {Main.intMAXNAMESIZE} characters !\n";
            }

            if (strNewName_.ToLower().Contains($"{Main.chrSEPARATOR}"))
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
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new Width is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxHeight.Text, out intHeight_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new Height is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxX.Text, out intX_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new X is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxY.Text, out intY_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new Y is not an integrer !\n";
            }

            //test if all the numerical values are converted successfully
            if (blnAreNumsValid)
            {
                //test if the new solidness is smaller that the minimum size
                if (intX_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new X cannot be less than 0 !\n";
                }

                //test if the new bounciness is smaller than the minimum size
                if (intY_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new Y cannot be less than 0 !\n";
                }

                //test if the new width is smaller than the minimum size
                if (intWidth_ <= 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new width must be more than 0 !\n";
                }

                //test if the new height is smaller than the minimum size
                if (intHeight_ <= 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new height must be more than 0 !\n";
                }

                Position posNewPos = new Position();
                posNewPos.DisDistance.dblWidth = intWidth_;
                posNewPos.DisDistance.dblHeight = intHeight_;
                posNewPos.LocLocation.dblX = intX_;
                posNewPos.LocLocation.dblY = intY_;

                //test if the new position is valid
                if (!ObsgrParent.FlParent.FlgrParent.DimParent.TestEmptiness(posNewPos, IntID, true))
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new position must not be already taken !\n";
                }

                //test if the new position is in the dimension
                if (intX_ < 0 || intY_ < 0 || intX_ + intWidth_ > ObsgrParent.FlParent.FlgrParent.DimParent.IntWidth || intY_ + intHeight_ > ObsgrParent.FlParent.FlgrParent.DimParent.IntHeight)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new position must not be outside of the dimension !\n";
                }

            }
            #endregion

            //test if all the new values are valid
            if (blnAreAllValuesValid)
            {
                string strSaveLog = CreateTextFromObject(this);

                //change the values
                IntTypeID = WrldInWorld.ObstypgrTypes.List_obstypTypes[_cbbxType.SelectedIndex].IntID;
                StrName = strNewName_;
                IntWidth = intWidth_;
                IntHeight = intHeight_;
                IntX = intX_;
                IntY = intY_;

                string strResultLog = CreateTextFromObject(this);

                Log.AddLog(ObjectType.Obstacle, EditingType.ChangeProperties, strSaveLog, strResultLog);

                //close the form
                _frmObstacle.Close();

                //dispose it of all values
                _frmObstacle.Dispose();
            }
            else
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