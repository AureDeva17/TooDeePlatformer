/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 16.06.2021                                                    **
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
**  Class made to store special objects like gates to other dimensions or    **
**  the ending flag                                                          **
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
    /// Class made to store special objects like gates to other dimensions or the ending flag 
    /// </summary>
    class Special
    {
        //declare the static properties
        public const string strCLASSNAME = "Special";                   //class name
        public const ObjectType objTYPE = ObjectType.Special;           //object type
        public static int IntNextID { get; set; }                       //next ID to be used
        public static List<Special> List_spcSpecials { get; set; }      //list of all the specials

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_spcSpecials.Count; i++)
                if (List_spcSpecials[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static Special GetObjectByID(int intID) => List_spcSpecials[GetIndexFromID(intID)];

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_spcSpecials[intIndex].IntID;

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
            string strText = Main.strVERSION + "\n";

            strText += strCLASSNAME + "\n";
            strText += IntNextID + "\n";
            strText += List_spcSpecials.Count + "\n";

            for (int i = 0; i < List_spcSpecials.Count; i++)
            {
                strText += CreateTextFromObject(List_spcSpecials[i]);

                if (List_spcSpecials.Count != i)
                    strText += "\n";
            }

            return strText;
        }


        /// <summary>
        /// Copy the data from the file to the obstacles
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrSpecialTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_spcSpecials.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_spcSpecials.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                }
                else
                    MessageBox.Show("The file isn't a storage for special objects !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a line of text from an object
        /// </summary>
        /// <param name="spcObject"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(Special spcObject)
        {
            string strText = "";

            strText += spcObject.IntID + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntFolderID + $"{Main.chrSEPARATOR}";
            strText += spcObject.StrName + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntSpecialTypeID + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntX + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntY + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntToDimensionID + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntToX + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntToY + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntActivateFolderID + $"{Main.chrSEPARATOR}";
            strText += spcObject.IntNumberLaps;

            return strText;
        }

        /// <summary>
        /// Create an object from a line of text
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static Special CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            Special spcNewSpecial = new Special()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntFolderID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                _intSpecialTypeID = Convert.ToInt32(tab_strObject[3]),
                _intDefaultX = Convert.ToInt32(tab_strObject[4]),
                _intDefaultY = Convert.ToInt32(tab_strObject[5]),
                _posPosition = new Position()
                {
                    LocLocation = new Location()
                    {
                        dblX = Convert.ToInt32(tab_strObject[4]),
                        dblY = Convert.ToInt32(tab_strObject[5])
                    },
                    DisDistance = new Distance()
                },
                IntToDimensionID = Convert.ToInt32(tab_strObject[6]),
                IntToX = Convert.ToInt32(tab_strObject[7]),
                IntToY = Convert.ToInt32(tab_strObject[8]),
                IntActivateFolderID = Convert.ToInt32(tab_strObject[9]),
                IntNumberLaps = Convert.ToInt32(tab_strObject[10]),
            };

            spcNewSpecial.IntSpecialTypeID = spcNewSpecial.IntSpecialTypeID;

            if (!blnOnlyRead)
            {
                //add the methods to the events
                spcNewSpecial._pcbxPictureBox.MouseMove += new MouseEventHandler(spcNewSpecial.SpcgrParent.FlParent.FlgrParent.DimParent.Drag);
                spcNewSpecial._pcbxPictureBox.MouseUp += new MouseEventHandler(spcNewSpecial.SpcgrParent.FlParent.FlgrParent.DimParent.StopDragging);
                spcNewSpecial._pcbxPictureBox.MouseDown += new MouseEventHandler(spcNewSpecial.ClickObject);

                //create the picture box
                spcNewSpecial._pcbxPictureBox.Size = spcNewSpecial._posPosition.ToSize();
                spcNewSpecial._pcbxPictureBox.Location = spcNewSpecial._posPosition.ToPoint(spcNewSpecial.IntDimensionHeight);
                spcNewSpecial._pcbxPictureBox.ContextMenuStrip = spcNewSpecial.GetContextMenu();
                spcNewSpecial._pcbxPictureBox.Name = spcNewSpecial.SpctypType.StrName;
                spcNewSpecial._pcbxPictureBox.BackgroundImage = spcNewSpecial.SpctypType.ImgTexture;
                spcNewSpecial._pcbxPictureBox.BackgroundImageLayout = spcNewSpecial.SpctypType.ImglytLayout;

                //set it to unselected
                spcNewSpecial.SpcgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Add(spcNewSpecial.GetPictureBox());
            }

            return spcNewSpecial;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static Special()
        {
            IntNextID = 0;
            List_spcSpecials = new List<Special>();
        }
        #endregion

        //declare events
        public event dlgPersoEvent evtChangeValues;             //triggered when a property is changed

        //declare the properties
        public int IntID { get; set; }                          //get the ID of the object
        public int IntFolderID { get; set; }                    //contain the parent
        public int IntActivateFolderID { get; set; }            //folder to be activated in case of an event
        public int IntNumberLaps { get; set; }                  //number of laps to be done
        public int IntToX { get; set; }                         //X position to travel to
        public int IntToY { get; set; }                         //Y position to travel to
        public int IntToDimensionID { get; set; }               //Dimension to travel to

        //declare all the variables
        private readonly PictureBox _pcbxPictureBox;            //get the picture box of the special object
        private Position _posPosition;                          //position of the special object
        private int _intDefaultY;                               //default Y position
        private int _intDefaultX;                               //default X position
        private string _strName;                                //name of the obstacle
        private bool _blnSelected;                              //is selected
        private int _intSpecialTypeID;                          //type of the special object           

        #region Properties
        /// <summary>
        /// height of the dimension the object is in
        /// </summary>
        private int IntDimensionHeight
        {
            get => SpcgrParent.FlParent.FlgrParent.DimParent.IntHeight;
        }

        /// <summary>
        /// Get the width of the obstacle
        /// </summary>
        public int IntWidth
        {
            get => (int)_posPosition.DisDistance.dblWidth;
        }

        /// <summary>
        /// Get the height of the obstacle
        /// </summary>
        public int IntHeight
        {
            get => (int)_posPosition.DisDistance.dblHeight;
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
                if (!SpcgrParent.IsSpecialNameTaken(value, StrName))
                {
                    //set the new name
                    _strName = value;
                    evtChangeValues?.Invoke(IntID, this, GetType());
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
        public int IntSpecialTypeID
        {
            get => _intSpecialTypeID;

            //set the new type
            set
            {
                //verify the validity of the type
                int intType__ = value;

                if (WrldInWorld.SpctypgrTypes[value] == null)
                    intType__ = WrldInWorld.SpctypgrTypes[0].IntID;

                //set the new type
                _intSpecialTypeID = intType__;

                _posPosition.DisDistance.dblWidth = WrldInWorld.SpctypgrTypes[_intSpecialTypeID].IntWidth;
                _posPosition.DisDistance.dblHeight = WrldInWorld.SpctypgrTypes[_intSpecialTypeID].IntHeight;
                UpdatePictureBox();

                //reset the image
                if (_pcbxPictureBox != null)
                    ResetImage();
            }
        }

        /// <summary>
        /// Get and set the type
        /// </summary>
        public SpecialType SpctypType
        {
            get => SpecialType.List_spctypTypes[SpecialType.GetIndexFromID(IntSpecialTypeID)];
        }

        /// <summary>
        /// Get the parent
        /// </summary>
        public SpecialGroup SpcgrParent
        {
            get => Folder.List_flFolders[Folder.GetIndexFromID(IntFolderID)].SpcgrSpecials;
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => SpcgrParent.FlParent.FlgrParent.DimParent.DimgrParent.WrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values of the special objects
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="intType_"></param>
        /// <param name="intX_"></param>
        /// <param name="intY_"></param>
        /// <param name="intFolderID_"></param>
        public Special(string strName_, int intType_, int intX_, int intY_, int intFolderID_)
        {
            //set the default values
            IntFolderID = intFolderID_;
            IntID = IntNextID++;
            _intSpecialTypeID = intType_;
            StrName = strName_;
            _posPosition = new Position();
            _posPosition.DisDistance.dblWidth = WrldInWorld.SpctypgrTypes[IntSpecialTypeID].IntWidth;
            _posPosition.DisDistance.dblHeight = WrldInWorld.SpctypgrTypes[IntSpecialTypeID].IntHeight;
            _posPosition.LocLocation.dblX = intX_;
            _posPosition.LocLocation.dblY = intY_;
            _intDefaultX = (int)_posPosition.LocLocation.dblX;
            _intDefaultY = (int)_posPosition.LocLocation.dblY;

            IntToDimensionID = Dimension.GetIDFromIndex(0);
            IntToX = 0;
            IntToY = 0;
            IntNumberLaps = 1;
            IntActivateFolderID = SpcgrParent.FlParent.FlgrParent.List_flFolders[0].IntID;

            //create the picture box
            _pcbxPictureBox = new PictureBox
            {
                Size = _posPosition.ToSize(),
                Location = _posPosition.ToPoint(IntDimensionHeight),
                ContextMenuStrip = GetContextMenu(),
                Name = SpctypType.StrName,
                BackgroundImage = SpctypType.ImgTexture,
                BackgroundImageLayout = SpctypType.ImglytLayout,
                Cursor = Cursors.Hand
            };

            _pcbxPictureBox.MouseMove += new MouseEventHandler(SpcgrParent.FlParent.FlgrParent.DimParent.Drag);
            _pcbxPictureBox.MouseUp += new MouseEventHandler(SpcgrParent.FlParent.FlgrParent.DimParent.StopDragging);
            _pcbxPictureBox.MouseDown += new MouseEventHandler(ClickObject);

            //set it to unselected
            BlnSelected = false;
            SpcgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Add(GetPictureBox());

            evtChangeValues += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create a special object from a file
        /// </summary>
        public Special()
        {
            _pcbxPictureBox = new PictureBox()
            {
                Cursor = Cursors.Hand
            };

            //set it to unselected
            BlnSelected = false;
            evtChangeValues += new dlgPersoEvent(UpdateTreeView);
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
            _pcbxPictureBox.BackgroundImage = SpctypType.ImgTexture;
            _pcbxPictureBox.BackgroundImageLayout = SpctypType.ImglytLayout;
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
                SpcgrParent.FlParent.FlgrParent.DimParent.GrabObject(this);
            else
                SpcgrParent.FlParent.FlgrParent.DimParent.GrabSelection();

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
        public TreeNode GetTreeNode() => new TreeNode(StrName) { ContextMenuStrip = GetContextMenu() };
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
            tsmiItems[0].Click += new EventHandler(SelectSpecialMenu);

            //tool used to unselect the obstacle
            tsmiItems.Add(new ToolStripMenuItem("Unselect Ctrl + Click"));
            tsmiItems[1].Tag = IntID;
            tsmiItems[1].Click += new EventHandler(UnselectSpecialMenu);

            //tool used to remove the obstacle
            tsmiItems.Add(new ToolStripMenuItem("Remove"));
            tsmiItems[2].Tag = IntID;
            tsmiItems[2].Click += new EventHandler(RemoveObstacleMenu);

            //tool used to access the properties of the obstacle
            tsmiItems.Add(new ToolStripMenuItem("Properties"));
            tsmiItems[3].Tag = IntID;
            tsmiItems[3].Click += new EventHandler(ModifySpecialPropertiesMenu);

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
            for (int i = 0; i < List_spcSpecials.Count; i++)
                if (List_spcSpecials[i].IntID == IntID)
                {
                    SpcgrParent.Remove(i);
                    evtChangeValues?.Invoke(IntID, this, GetType());

                    break;
                }
        }

        /// <summary>
        /// Select the clicked object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSpecialMenu(object sender, EventArgs e) => BlnSelected = true;

        /// <summary>
        /// Unselect the clicked object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectSpecialMenu(object sender, EventArgs e) => BlnSelected = false;

        /// <summary>
        /// Open the properties form of the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifySpecialPropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Properties Form
        //declare the controls
        private Form _frmSpecial;               //special form
        private TextBox _txtbxName;             //textbox containing the name and gives the ability to change it
        private TextBox _txtbxX;                //textbox containing the X location
        private TextBox _txtbxY;                //textbox containing the Y location
        private TextBox _txtbxToX;              //textbox containing the to X location
        private TextBox _txtbxToY;              //textbox containing the to Y location
        private TextBox _txtbxNbLaps;           //textbox containing the number of laps of the folder
        private ComboBox _cbbxType;             //combobox containing the type of the part and gives the ability to change it
        private ComboBox _cbbxToDimension;      //combobox containing the dimension to travel to
        private ComboBox _cbbxActFolder;        //combobox containing the folder to activate
        private Label _lblToDimension;          //tells the user the combo box next to it is to choose in wich dimension you will travel to
        private Label _lblActFolder;            //tells the user the combo box next to it is to choose wich folder should be activated
        private Label _lblToX;                  //tells the user the textbox next to it is the to X location one
        private Label _lblToY;                  //tells the user the textbox next to it is the to Y location one
        private Label _lblTouchEvent;           //label informing that the checkboxes underneath are for a touching event
        private Label _lblNbLaps;               //tells the user the textbox next to it is used to inform of the number of laps of the folder
        private Label _lblName;                 //tells the user the textbox next to it is the naming one
        private Label _lblX;                    //tells the user the textbox next to it is the X location one
        private Label _lblY;                    //tells the user the textbox next to it is the Y location one
        private Label _lblType;                 //tells the user the combobox next to it is the type one
        private Button _btnCancel;              //close the form
        private Button _btnValidate;            //test the validity of the new values

        public void OpenPropertiesForm()
        {
            //reset the controls
            _frmSpecial = new Form();
            _txtbxName = new TextBox();
            _lblName = new Label();
            _btnCancel = new Button();
            _btnValidate = new Button();
            _cbbxType = new ComboBox();
            _txtbxX = new TextBox();
            _txtbxY = new TextBox();
            _lblX = new Label();
            _lblY = new Label();
            _lblType = new Label();
            _cbbxActFolder = new ComboBox();
            _cbbxToDimension = new ComboBox();
            _lblToX = new Label();
            _lblToY = new Label();
            _lblNbLaps = new Label();
            _txtbxToX = new TextBox();
            _txtbxToY = new TextBox();
            _txtbxNbLaps = new TextBox();
            _lblToDimension = new Label();
            _lblActFolder = new Label();
            _lblTouchEvent = new Label();

            #region Controls
            //suspend the layout
            _frmSpecial.SuspendLayout();

            //textbox containing the name and give the ability to change it
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //textbox containing the X location and give the ability to change it
            _txtbxX.Location = new Point(202, 69);
            _txtbxX.Name = "txtbxX";
            _txtbxX.Size = new Size(36, 20);
            _txtbxX.TabIndex = 2;
            _txtbxX.Text = $"{IntX}";
            _txtbxX.TextAlign = HorizontalAlignment.Right;

            //textbox containing the Y location and give the ability to change it
            _txtbxY.Location = new Point(202, 95);
            _txtbxY.Name = "txtbxY";
            _txtbxY.Size = new Size(36, 20);
            _txtbxY.TabIndex = 2;
            _txtbxY.Text = $"{IntY}";
            _txtbxY.TextAlign = HorizontalAlignment.Right;

            //combobox containing the types and give the ability to change it
            _cbbxType.Location = new Point(100, 121);
            _cbbxType.Name = "cbbxType";
            _cbbxType.Size = new Size(138, 20);
            _cbbxType.TabIndex = 3;
            List<string> list_strTypes = new List<string>();
            for (int i = 0; i < WrldInWorld.SpctypgrTypes.List_spctypTypes.Count; i++)
                list_strTypes.Add(WrldInWorld.SpctypgrTypes.List_spctypTypes[i].StrName);
            _cbbxType.Items.AddRange(list_strTypes.ToArray());
            for (int i = 0; i < WrldInWorld.SpctypgrTypes.List_spctypTypes.Count; i++)
                if (WrldInWorld.SpctypgrTypes.List_spctypTypes[i].IntID == IntSpecialTypeID)
                    _cbbxType.SelectedIndex = i;

            //set the default values of the Touch Event label
            _lblTouchEvent.AutoSize = true;
            _lblTouchEvent.Location = new Point(36, 147);
            _lblTouchEvent.Name = "lblTouchEvent";
            _lblTouchEvent.Text = "Touch Event's variables:";

            //combobox containing the dimensions and give the ability to change it
            _cbbxToDimension.Location = new Point(100, 173);
            _cbbxToDimension.Name = "cbbxToDimension";
            _cbbxToDimension.Size = new Size(138, 20);
            _cbbxToDimension.TabIndex = 3;
            List<string> list_strDimensions = new List<string>();
            for (int i = 0; i < WrldInWorld.DimgrDimensions.List_dimDimensions.Count; i++)
                list_strDimensions.Add(WrldInWorld.DimgrDimensions.List_dimDimensions[i].StrName);
            _cbbxToDimension.Items.AddRange(list_strDimensions.ToArray());
            for (int i = 0; i < WrldInWorld.DimgrDimensions.List_dimDimensions.Count; i++)
                if (WrldInWorld.DimgrDimensions.List_dimDimensions[i].IntID == IntToDimensionID)
                    _cbbxToDimension.SelectedIndex = i;
            if (WrldInWorld.SpctypgrTypes[IntSpecialTypeID].BlnTravelTo)
                _cbbxToDimension.Enabled = true;
            else
                _cbbxToDimension.Enabled = false;

            //To dimension label
            _lblToDimension.AutoSize = true;
            _lblToDimension.Location = new Point(36, 173);
            _lblToDimension.Name = "lblToDimension";
            _lblToDimension.Text = "Travel to :";

            //textbox containing the Y location and give the ability to change it
            _txtbxToX.Location = new Point(202, 199);
            _txtbxToX.Name = "txtbxToX";
            _txtbxToX.Size = new Size(36, 20);
            _txtbxToX.TabIndex = 2;
            _txtbxToX.Text = $"{IntToX}";
            _txtbxToX.TextAlign = HorizontalAlignment.Right;
            _txtbxToX.Enabled = false;

            if (WrldInWorld.SpctypgrTypes[IntSpecialTypeID].BlnTravelTo)
                _txtbxToX.Enabled = true;
            else
                _txtbxToX.Enabled = false;

            //To X pos label
            _lblToX.AutoSize = true;
            _lblToX.Location = new Point(36, 199);
            _lblToX.Name = "lblToX";
            _lblToX.Text = "Travel to X location :";

            //textbox containing the X location and give the ability to change it
            _txtbxToY.Location = new Point(202, 225);
            _txtbxToY.Name = "txtbxToY";
            _txtbxToY.Size = new Size(36, 20);
            _txtbxToY.TabIndex = 2;
            _txtbxToY.Text = $"{IntToY}";
            _txtbxToY.TextAlign = HorizontalAlignment.Right;
            _txtbxToY.Enabled = false;

            if (WrldInWorld.SpctypgrTypes[IntSpecialTypeID].BlnTravelTo)
                _txtbxToY.Enabled = true;
            else
                _txtbxToY.Enabled = false;

            //To Y pos label
            _lblToY.AutoSize = true;
            _lblToY.Location = new Point(36, 225);
            _lblToY.Name = "lblToY";
            _lblToY.Text = "Travel to Y location :";

            //combobox containing the folders and give the ability to change it
            _cbbxActFolder.Location = new Point(100, 251);
            _cbbxActFolder.Name = "cbbxActFolder";
            _cbbxActFolder.Size = new Size(138, 20);
            _cbbxActFolder.TabIndex = 3;
            List<string> list_strFolders = new List<string>();
            for (int i = 0; i < SpcgrParent.FlParent.FlgrParent.List_flFolders.Count; i++)
                list_strFolders.Add(SpcgrParent.FlParent.FlgrParent.List_flFolders[i].StrName);
            _cbbxActFolder.Items.AddRange(list_strFolders.ToArray());
            for (int i = 0; i < SpcgrParent.FlParent.FlgrParent.List_flFolders.Count; i++)
                if (SpcgrParent.FlParent.FlgrParent.List_flFolders[i].IntID == IntActivateFolderID)
                    _cbbxActFolder.SelectedIndex = i;
            if (WrldInWorld.SpctypgrTypes[IntSpecialTypeID].BlnStartGroupMove)
                _cbbxActFolder.Enabled = true;
            else
                _cbbxActFolder.Enabled = false;

            //Activate folder label
            _lblActFolder.AutoSize = true;
            _lblActFolder.Location = new Point(36, 251);
            _lblActFolder.Name = "lblActFolder";
            _lblActFolder.Text = "Activate :";

            //textbox containing the X location and give the ability to change it
            _txtbxNbLaps.Location = new Point(202, 277);
            _txtbxNbLaps.Name = "txtbxNbLaps";
            _txtbxNbLaps.Size = new Size(36, 20);
            _txtbxNbLaps.TabIndex = 2;
            _txtbxNbLaps.Text = $"{IntNumberLaps}";
            _txtbxNbLaps.TextAlign = HorizontalAlignment.Right;
            _txtbxNbLaps.Enabled = false;

            if (WrldInWorld.SpctypgrTypes[IntSpecialTypeID].BlnStartGroupMove)
                _txtbxNbLaps.Enabled = true;
            else
                _txtbxNbLaps.Enabled = false;

            //number of laps label
            _lblNbLaps.AutoSize = true;
            _lblNbLaps.Location = new Point(36, 277);
            _lblNbLaps.Name = "lblNbLaps";
            _lblNbLaps.Text = "Number of laps :";

            //label telling the textbox next to it is the naming one
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Text = "Name :";

            //label telling the textbox next to it is the X location one
            _lblX.AutoSize = true;
            _lblX.Location = new Point(36, 69);
            _lblX.Name = "lblX";
            _lblX.Text = "Location in X :";

            //label telling the textbox next to it is the Y location one
            _lblY.AutoSize = true;
            _lblY.Location = new Point(36, 95);
            _lblY.Name = "lblY";
            _lblY.Size = new Size(41, 13);
            _lblY.Text = "Location in Y :";

            //label telling the the combobox next to it is the type one
            _lblType.AutoSize = true;
            _lblType.Location = new Point(36, 121);
            _lblType.Name = "lblType";
            _lblType.Size = new Size(44, 13);
            _lblType.Text = "Type :";

            //close the form
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 303);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(Cancel_Click);

            //validate the new values
            _btnValidate.Location = new Point(121, 303);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Validate";
            _btnValidate.Tag = IntID;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //form
            _frmSpecial.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmSpecial.AutoScaleMode = AutoScaleMode.Font;
            _frmSpecial.AcceptButton = _btnValidate;
            _frmSpecial.CancelButton = _btnCancel;
            _frmSpecial.ClientSize = new Size(304, 340);
            _frmSpecial.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmSpecial.Name = "frmSpecial";
            _frmSpecial.StartPosition = FormStartPosition.CenterParent;
            _frmSpecial.Text = "Special Object";
            _frmSpecial.Tag = IntID;

            //add the controls
            _frmSpecial.Controls.Add(_btnValidate);
            _frmSpecial.Controls.Add(_btnCancel);
            _frmSpecial.Controls.Add(_lblName);
            _frmSpecial.Controls.Add(_lblX);
            _frmSpecial.Controls.Add(_lblY);
            _frmSpecial.Controls.Add(_txtbxX);
            _frmSpecial.Controls.Add(_txtbxY);
            _frmSpecial.Controls.Add(_lblType);
            _frmSpecial.Controls.Add(_cbbxType);
            _frmSpecial.Controls.Add(_txtbxName);
            _frmSpecial.Controls.Add(_txtbxNbLaps);
            _frmSpecial.Controls.Add(_txtbxToX);
            _frmSpecial.Controls.Add(_txtbxToY);
            _frmSpecial.Controls.Add(_lblNbLaps);
            _frmSpecial.Controls.Add(_lblToX);
            _frmSpecial.Controls.Add(_lblToY);
            _frmSpecial.Controls.Add(_lblActFolder);
            _frmSpecial.Controls.Add(_lblToDimension);
            _frmSpecial.Controls.Add(_cbbxActFolder);
            _frmSpecial.Controls.Add(_cbbxToDimension);
            _frmSpecial.Controls.Add(_lblTouchEvent);

            //resume the layout
            _frmSpecial.ResumeLayout(false);
            _frmSpecial.PerformLayout();
            #endregion

            //show the form
            _frmSpecial.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the part form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmSpecial.Close();

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
            bool blnAreNumsValid = true;            //used to tell if all the numerical values are valid

            string strNewName_ = _txtbxName.Text;    //contain the new name of the part
            int intX_;                              //contain the new X position
            int intY_;                              //contain the new Y position
            int intToX_;                            //contain the new to X position
            int intToY_;                            //contain the new to Y position
            int intNbLaps_;                         //contain the number of laps of the activated folder
            int intDimensionID;                     //contain the dimension the player will travel to
            int intSpecialTypeID_;                  //contain the special object type chosen

            try
            {
                intSpecialTypeID_ = WrldInWorld.SpctypgrTypes.List_spctypTypes[_cbbxType.SelectedIndex].IntID;
            }
            catch
            {
                intSpecialTypeID_ = 0;
            }

            try
            {
                intDimensionID = WrldInWorld.DimgrDimensions.List_dimDimensions[_cbbxToDimension.SelectedIndex].IntID;
            }
            catch
            {
                intDimensionID = 0;
            }

            #region Verify Validity
            //test if the name is taken
            if (SpcgrParent.IsSpecialNameTaken($"{strNewName_}", StrName))
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

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxToX.Text, out intToX_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new to X is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxToY.Text, out intToY_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new to Y is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxNbLaps.Text, out intNbLaps_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new number of laps is not an integrer !\n";
            }

            if (blnAreNumsValid)
            {
                //test if the new X is smaller that the minimum size
                if (intX_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new X cannot be less than 0 !\n";
                }

                //test if the new Y is smaller than the minimum size
                if (intY_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new Y cannot be less than 0 !\n";
                }

                //test if the new number of laps is smaller that the minimum size
                if (intNbLaps_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new number of laps cannot be less than 0 !\n";
                }

                //test if the position is outisde of the dimension
                if (intToX_ < 0 || intToY_ < 0 || intToX_ >= SpcgrParent.FlParent.FlgrParent.DimParent.DimgrParent[intDimensionID].IntWidth || intToY_ >= SpcgrParent.FlParent.FlgrParent.DimParent.DimgrParent[intDimensionID].IntHeight)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new position is positioned outside of the targeted dimension !\n";
                }

                Position posNewPos = new Position();
                posNewPos.DisDistance.dblWidth = WrldInWorld.SpctypgrTypes[intSpecialTypeID_].IntWidth;
                posNewPos.DisDistance.dblHeight = WrldInWorld.SpctypgrTypes[intSpecialTypeID_].IntHeight;
                posNewPos.LocLocation.dblX = intX_;
                posNewPos.LocLocation.dblY = intY_;

                //test if the new position is valid
                if (!SpcgrParent.FlParent.FlgrParent.DimParent.TestEmptiness(posNewPos, IntID, false))
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new position must not be already taken !\n";
                }

                //test if the new position is in the dimension
                if (intX_ < 0 || intY_ < 0 || intX_ + IntWidth > SpcgrParent.FlParent.FlgrParent.DimParent.IntWidth || intY_ + IntHeight > SpcgrParent.FlParent.FlgrParent.DimParent.IntHeight)
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
                IntSpecialTypeID = intSpecialTypeID_;
                StrName = strNewName_;
                IntToDimensionID = intDimensionID;

                try
                {
                    IntActivateFolderID = SpcgrParent.FlParent.FlgrParent.List_flFolders[_cbbxActFolder.SelectedIndex].IntID;
                }
                catch
                {
                    IntActivateFolderID = 0;
                }

                IntX = intX_;
                IntY = intY_;
                IntToX = intToX_;
                IntToY = intToY_;
                IntNumberLaps = intNbLaps_;

                UpdatePictureBox();

                string strResultLog = CreateTextFromObject(this);

                Log.AddLog(ObjectType.Special, EditingType.ChangeProperties, strSaveLog, strResultLog);

                //close the form
                _frmSpecial.Close();

                //dispose it of all values
                _frmSpecial.Dispose();
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