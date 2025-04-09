/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 22.01.2021                                                    **
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
** This class is used to store the a world's data and all the dimensions it  **
** contains. It also contains the methods of using to add dimensions.        **
** It's there that the treeview, the tabcontrol, the main panel and the menu **
** strip is created.                                                         **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// This class contain everything needed to create a world. 
    /// It has dimensions, a main panel, a treeview and a possibility to travel trought dimensions with a tabcontrol.
    /// </summary>
    class World
    {
        //declare the static properties
        public const string strCLASSNAME = "World";                 //class name
        public const ObjectType objTYPE = ObjectType.World;         //object type
        public static int IntNextID { get; set; }                   //next ID to be used
        public static List<World> List_wrldWorlds { get; set; }     //list of all the worlds

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_wrldWorlds.Count; i++)
                if (List_wrldWorlds[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_wrldWorlds[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static World GetObjectByID(int intID) => List_wrldWorlds[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
            string strText = Main.strVERSION + "\n";

            strText += strCLASSNAME + "\n";
            strText += IntNextID + "\n";
            strText += List_wrldWorlds.Count + "\n";

            for (int i = 0; i < List_wrldWorlds.Count; i++)
            {
                strText += CreateTextFromObject(List_wrldWorlds[i]);

                if (List_wrldWorlds.Count != i)
                    strText += "\n";
            }

            return strText;
        }

        /// <summary>
        /// Copy the data from the file to the world
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrWorldTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_wrldWorlds.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_wrldWorlds.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                }
                else
                    MessageBox.Show("The file isn't a storage for worlds !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a new world form a line of text
        /// </summary>
        /// <param name="strObject"></param>
        /// <returns></returns>
        public static World CreateObjectFromText(string strObject, bool blnOnlyRead)
        {
            string[] tab_strObject = strObject.Split(Main.chrSEPARATOR);

            World wrldNewWorld = new World()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                StrName = tab_strObject[1],
                _intDefaultPlayerDimensionID = Convert.ToInt32(tab_strObject[2]),
                _intDefaultPlayerX = Convert.ToInt32(tab_strObject[3]),
                _intDefaultPlayerY = Convert.ToInt32(tab_strObject[4])
            };

            return wrldNewWorld;
        }

        /// <summary>
        /// Create a new line of text from a world
        /// </summary>
        /// <param name="wrldWorld"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(World wrldWorld)
        {
            string strText = "";

            strText += wrldWorld.IntID + $"{Main.chrSEPARATOR}";
            strText += wrldWorld.StrName + $"{Main.chrSEPARATOR}";
            strText += wrldWorld.IntDefaultPlayerDimensionID + $"{Main.chrSEPARATOR}";
            strText += wrldWorld.IntDefaultPlayerX + $"{Main.chrSEPARATOR}";
            strText += wrldWorld.IntDefaultPlayerY;

            return strText;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static World()
        {
            IntNextID = 0;
            List_wrldWorlds = new List<World>();
        }
        #endregion

        //declare the events
        public event dlgPersoEvent evtChangeValues;                 //triggered when visual values are changed

        //declare the properties
        public int IntID { get; set; }                              //contain the ID of the object
        public DimensionGroup DimgrDimensions { get; set; }         //contain the name of the dimension
        public ObstacleTypeGroup ObstypgrTypes { get; set; }        //contain the types
        public SpecialTypeGroup SpctypgrTypes { get; set; }         //group of special types
        public Panel PnlWorld { get; set; }                         //panel containing the treeview and the tabcontrol with the dimensions
        public int IntObstacleTypeID { get; set; }                  //contain the ID of the selected obstacle type
        public int IntSpecialTypeID { get; set; }                   //contain the ID of the selected special type
        public bool BlnStructCreated { get; set; }                  //is the storage structure created ?
        public PictureBox PcbxMario { get; set; }                   //picturebox containing mario
        public MenuStrip MnstrMain { get; set; }                    //menu strip in top of the form
        public bool BlnPressingCtrl { get; set; }                   //contain the information about the state of the key ctrl
        public bool BlnPressingShift { get; set; }                  //contain the information about the state of the key shift
        public bool BlnNeedSaving { get; set; }                     //tell if the world needs to be saved

        //declare the varialbes
        private readonly TreeView _trvWorld;                        //contain a visual representation in the form a tree of all the objects
        private string _strName;                                    //contain the name of the world
        private int _intDefaultPlayerDimensionID;                   //contain the id of the starting dimension
        private int _intDefaultPlayerX;                             //contain the coordinates of the player
        private int _intDefaultPlayerY;                             //contain the coordinates of the player

        #region Properties
        /// <summary>
        /// Name of the world
        /// </summary>
        public string StrName
        {
            get => _strName;

            set
            {
                //set the name to the controls
                _strName = value;
                PnlWorld.Name = value;

                evtChangeValues?.Invoke(IntID, this, GetType());
            }
        }

        /// <summary>
        /// Get the id of the starting dimension ID and set the id while modifying the placement of the displayed player
        /// </summary>
        public int IntDefaultPlayerDimensionID
        {
            get
            {
                for (int i = 0; i < Dimension.List_dimDimensions.Count; i++)
                    if (Dimension.List_dimDimensions[i].IntID == _intDefaultPlayerDimensionID)
                        return _intDefaultPlayerDimensionID;

                try
                {
                    return Dimension.List_dimDimensions[0].IntID;
                }
                catch
                {
                    return -1;
                }
            }

            set
            {
                int intOldID = _intDefaultPlayerDimensionID;
                _intDefaultPlayerDimensionID = value;

                if (IntDefaultPlayerDimensionID != -1)
                {
                    DimgrDimensions[intOldID].PnlDimension.Controls.Remove(PcbxMario);
                    DimgrDimensions[IntDefaultPlayerDimensionID].PnlDimension.Controls.Add(PcbxMario);

                    PcbxMario.Location = new Position()
                    {
                        DisDistance = new Distance()
                        {
                            dblWidth = 1,
                            dblHeight = 2
                        },

                        LocLocation = new Location()
                        {
                            dblX = IntDefaultPlayerX,
                            dblY = IntDefaultPlayerY
                        }
                    }.ToPoint(DimgrDimensions[IntDefaultPlayerDimensionID].IntHeight);
                }
            }
        }

        /// <summary>
        /// Get the X and set the X while modifying the placement of the displayed player
        /// </summary>
        public int IntDefaultPlayerX
        {
            get => _intDefaultPlayerX;

            set
            {
                _intDefaultPlayerX = value;

                if (IntDefaultPlayerDimensionID != -1)
                {
                    PcbxMario.Location = new Position()
                    {
                        DisDistance = new Distance()
                        {
                            dblWidth = 1,
                            dblHeight = 2
                        },

                        LocLocation = new Location()
                        {
                            dblX = IntDefaultPlayerX,
                            dblY = IntDefaultPlayerY
                        }
                    }.ToPoint(DimgrDimensions[IntDefaultPlayerDimensionID].IntHeight);
                }
            }
        }

        /// <summary>
        /// Get the Y and set the Y while modifying the placement of the displayed player
        /// </summary>
        public int IntDefaultPlayerY
        {
            get => _intDefaultPlayerY;

            set
            {
                _intDefaultPlayerY = value;

                if (IntDefaultPlayerDimensionID != -1)
                {
                    PcbxMario.Location = new Position()
                    {
                        DisDistance = new Distance()
                        {
                            dblWidth = 1,
                            dblHeight = 2
                        },

                        LocLocation = new Location()
                        {
                            dblX = IntDefaultPlayerX,
                            dblY = IntDefaultPlayerY
                        }
                    }.ToPoint(DimgrDimensions[IntDefaultPlayerDimensionID].IntHeight);
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values of the world
        /// </summary>
        /// <param name="strName"></param>
        public World(string strName)
        {
            _strName = strName;

            BlnStructCreated = false;

            //set the default values
            IntID = IntNextID++;
            ObstypgrTypes = new ObstacleTypeGroup("Obstacle Types", this);
            SpctypgrTypes = new SpecialTypeGroup("Special Types", this);
            DimgrDimensions = new DimensionGroup("Dimensions", this);
            PnlWorld = new Panel();
            BlnPressingCtrl = false;
            BlnPressingShift = false;

            //set the default values of the player
            DimgrDimensions.AddDimension(64, 64);

            try
            {
                _intDefaultPlayerDimensionID = DimgrDimensions.List_dimDimensions[0].IntID;
            }
            catch
            {
                _intDefaultPlayerDimensionID = 0;
            }

            _intDefaultPlayerX = 0;
            _intDefaultPlayerY = 0;

            //set the default name
            StrName = strName;

            #region Controls
            //create the treeview and expand it all
            _trvWorld = GetTrvWorld();
            _trvWorld.ExpandAll();

            //create the main panel
            PnlWorld.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            PnlWorld.Location = new Point(0, 0);
            PnlWorld.Name = strName;
            PnlWorld.Size = new Size(1759 + _trvWorld.Width + 10, DimgrDimensions.TbcDimensions.Height + 24 + 6);

            //add the controls to the main panel
            PnlWorld.Controls.Add(DimgrDimensions.TbcDimensions);
            PnlWorld.Controls.Add(_trvWorld);
            DimgrDimensions.TbcDimensions.Left += _trvWorld.Width + 15;
            DimgrDimensions.TbcDimensions.Width -= _trvWorld.Width + 10;

            PcbxMario = new PictureBox();
            PcbxMario.Size = new Size(1 * Main.intUNITSIZE, 2 * Main.intUNITSIZE);
            PcbxMario.Location = new Point(0,0);
            PcbxMario.BackgroundImage = Image.FromFile(Main.StrImagesPath + @"\Mario.png");
            #endregion

            //set the events
            evtChangeValues += new dlgPersoEvent(UpdateTreeView);

            Main.FrmEditor.KeyDown += new KeyEventHandler(PressKeyDown);
            Main.FrmEditor.KeyUp += new KeyEventHandler(PressKeyUp);
            Main.FrmEditor.Leave += new EventHandler(LeaveForm);

            MnstrMain = new MenuStrip();
            PnlWorld.Controls.Add(MnstrMain);
            InitializeMainMenu();
        }

        /// <summary>
        /// Constructor used to create a world from a file
        /// </summary>
        private World()
        {
            BlnStructCreated = true;

            //set the default values
            ObstypgrTypes = new ObstacleTypeGroup("Obstacle Types", this);
            SpctypgrTypes = new SpecialTypeGroup("Special Types", this);
            DimgrDimensions = new DimensionGroup("Dimensions", this);
            PnlWorld = new Panel();
            MnstrMain = new MenuStrip();
            PnlWorld.Controls.Add(MnstrMain);
            BlnPressingCtrl = false;
            BlnPressingShift = false;

            try
            {
                _intDefaultPlayerDimensionID = DimgrDimensions.List_dimDimensions[0].IntID;
            }
            catch
            {
                _intDefaultPlayerDimensionID = 0;
            }

            _intDefaultPlayerX = 0;
            _intDefaultPlayerY = 0;

            #region Controls
            //create the treeview and expand it all
            _trvWorld = GetTrvWorld();
            _trvWorld.ExpandAll();

            //create the main panel
            PnlWorld.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            PnlWorld.Location = new Point(0, 0);
            PnlWorld.Size = new Size(1759 + _trvWorld.Width + 10, DimgrDimensions.TbcDimensions.Height + 24 + 6);

            //add the controls to the main panel
            PnlWorld.Controls.Add(DimgrDimensions.TbcDimensions);
            PnlWorld.Controls.Add(_trvWorld);
            DimgrDimensions.TbcDimensions.Left += _trvWorld.Width + 15;
            DimgrDimensions.TbcDimensions.Width -= _trvWorld.Width + 10;

            PcbxMario = new PictureBox();
            PcbxMario.Size = new Size(1 * Main.intUNITSIZE, 2 * Main.intUNITSIZE);
            PcbxMario.Location = new Point(0, 0);
            PcbxMario.BackgroundImage = Image.FromFile(Main.StrImagesPath + @"\Mario.png");
            #endregion

            Main.FrmEditor.KeyDown += new KeyEventHandler(PressKeyDown);
            Main.FrmEditor.KeyUp += new KeyEventHandler(PressKeyUp);
            Main.FrmEditor.Leave += new EventHandler(LeaveForm);

            //set the events
            evtChangeValues += new dlgPersoEvent(UpdateTreeView);
        }
        #endregion

        #region TreeView
        /// <summary>
        /// Get the Treeview containing all the nodes
        /// </summary>
        public TreeView GetTrvWorld()
        {
            //declare the treeview
            TreeView trvMain = new TreeView();      //control containg a visual representation of all the objects of the world

            //set the default properties
            trvMain.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left);
            trvMain.Size = new Size(275, 747);
            trvMain.Location = new Point(10, 30);
            trvMain.BackColor = Color.White;
            trvMain.ShowPlusMinus = false;

            //add the world node to the treeview
            trvMain.Nodes.Add(GetTreeNode());

            //retrun the treeview
            return trvMain;
        }

        /// <summary>
        /// Update the treeview
        /// </summary>
        public void UpdateTreeNodes()
        {
            if (_trvWorld != null)
            {
                //clear all the nodes
                _trvWorld.Nodes.Clear();

                //add the updated nodes
                _trvWorld.Nodes.Add(GetTreeNode());

                //expand all the nodes
                _trvWorld.ExpandAll();
            }
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the world node
        /// </summary>
        private TreeNode GetTreeNode()
        {
            //declare the list of world node
            List<TreeNode> list_trnWorld_ = new List<TreeNode>();

            list_trnWorld_.Add(DimgrDimensions.GetTreeNode());
            list_trnWorld_.Add(ObstypgrTypes.GetTreeNode());
            list_trnWorld_.Add(SpctypgrTypes.GetTreeNode());

            //retrun the world node
            return new TreeNode(StrName, list_trnWorld_.ToArray()) { ContextMenuStrip = GetContextMenu() };
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Get the context menu of the world
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare the objects for the contextbox
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();             //declare the contextbox control
            List<ToolStripMenuItem> list_tsmiMain = new List<ToolStripMenuItem>();       //declare the list of tools

            //add the new tool used to modify the properties
            list_tsmiMain.Add(new ToolStripMenuItem("Properties"));
            list_tsmiMain[0].Click += new EventHandler(ModifyWorldProperties_Click);
            list_tsmiMain[0].Tag = StrName;

            //add all the tools to the contextbox
            contextMenuStrip.Items.AddRange(list_tsmiMain.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Open the world properties form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyWorldProperties_Click(object sender, EventArgs e) => OpenWorldForm();
        #endregion
        #endregion

        #region Main Menu
        /// <summary>
        /// Create the new menu strip to change general values
        /// </summary>
        public void InitializeMainMenu()
        {
            //set the default values of the menu strip
            MnstrMain.Location = new Point(0, 0);
            MnstrMain.Name = "MnstrMain";
            MnstrMain.Size = new Size(PnlWorld.Width, 24);
            MnstrMain.TabIndex = 1;
            MnstrMain.Text = "Menu";
            MnstrMain.BackColor = SystemColors.ControlLight;

            List<ToolStripMenuItem> list_tsmiSections = new List<ToolStripMenuItem>();
            int intI = 0;

            List<ToolStripMenuItem> list_tsmiFile = new List<ToolStripMenuItem>();
            int intI2 = 0;

            list_tsmiFile.Add(new ToolStripMenuItem() { Text = "Open or Create" });
            list_tsmiFile[intI2++].Click += (s, e) => { Application.Restart(); };
            list_tsmiFile.Add(new ToolStripMenuItem() { Text = "Delete World" });
            list_tsmiFile[intI2++].Click += (s, e) =>
            {
                if (DialogResult.Yes == MessageBox.Show("Are you sure you want to delete the world ?", "Deleting World", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    Main.DeleteWorldFiles();
                    Main.FrmEditor.FormClosing -= Main.CloseEvent;
                    Application.Restart();
                }
            };

            list_tsmiSections.Add(new ToolStripMenuItem() { Text = "File" });
            list_tsmiSections[intI++].DropDownItems.AddRange(list_tsmiFile.ToArray());

            List<ToolStripMenuItem> list_tsmiEdit = new List<ToolStripMenuItem>();
            int int3 = 0;

            list_tsmiEdit.Add(new ToolStripMenuItem() { Text = "Save World Ctrl + S" });
            list_tsmiEdit[int3++].Click += (s, e) => Main.SaveWorld();
            list_tsmiEdit.Add(new ToolStripMenuItem() { Text = "Cancel Ctrl + Z" });
            list_tsmiEdit[int3++].Click += (s, e) => Log.Cancel();
            list_tsmiEdit.Add(new ToolStripMenuItem() { Text = "Uncancel Ctrl + Y" });
            list_tsmiEdit[int3++].Click += (s, e) => Log.UnCancel();

            list_tsmiSections.Add(new ToolStripMenuItem() { Text = "Edit" });
            list_tsmiSections[intI++].DropDownItems.AddRange(list_tsmiEdit.ToArray());

            List<ToolStripMenuItem> list_tsmiDisplay = new List<ToolStripMenuItem>();
            int intI4 = 0;

            list_tsmiDisplay.Add(new ToolStripMenuItem() { Text = "Set Background to Empty Ctrl + E" });
            list_tsmiDisplay[intI4++].Click += (s, e) => DimgrDimensions[DimgrDimensions.IntSelectedDimension].SetBackgroundToEmpty();
            list_tsmiDisplay.Add(new ToolStripMenuItem() { Text = "Set Background to Grid Ctrl + G" });
            list_tsmiDisplay[intI4++].Click += (s, e) => DimgrDimensions[DimgrDimensions.IntSelectedDimension].SetBackgroundToGrid();
            list_tsmiDisplay.Add(new ToolStripMenuItem() { Text = "Set Background to Image Ctrl + I" });
            list_tsmiDisplay[intI4++].Click += (s, e) => DimgrDimensions[DimgrDimensions.IntSelectedDimension].SetBackgroundToImage();

            list_tsmiSections.Add(new ToolStripMenuItem() { Text = "Display" });
            list_tsmiSections[intI++].DropDownItems.AddRange(list_tsmiDisplay.ToArray());

            MnstrMain.Items.AddRange(list_tsmiSections.ToArray());
        }
        #endregion

        #region Properties Form
        //declare the control object used to display the properties form
        private Form _frmWorld;                     //world properties form
        private TextBox _txtbxName;                 //textbox used to change the name of the world
        private Label _lblName;                     //label informing that the textbox next to it is for the name
        private ComboBox _cbbxDimension;            //combobox containing the dimension to travel to
        private Label _lblDimension;                //tells the user the combo box next to it is to choose in wich dimension you will appear
        private TextBox _txtbxX;                    //textbox containing the to X location
        private TextBox _txtbxY;                    //textbox containing the to Y location
        private Label _lblX;                        //tells the user the textbox next to it is the X location one
        private Label _lblY;                        //tells the user the textbox next to it is the Y location onne
        private Label _lblDefaultPosition;          //label informing that the checkboxes underneath are for a the default player location
        private Button _btnCancel;                  //button used to close the properties form
        private Button _btnValidate;                //button used to validate the new valuess

        /// <summary>
        /// Open the world properties form
        /// </summary>
        public void OpenWorldForm()
        {
            //reset all the controls
            _frmWorld = new Form();
            _txtbxName = new TextBox();
            _lblName = new Label();
            _cbbxDimension = new ComboBox();
            _lblDimension = new Label();
            _lblDefaultPosition = new Label();
            _lblX = new Label();
            _lblY = new Label();
            _txtbxX = new TextBox();
            _txtbxY = new TextBox();
            _btnCancel = new Button();
            _btnValidate = new Button();

            #region Controls
            //suspend the layout
            _frmWorld.SuspendLayout();

            //set the default values of the name textbox
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //set the default values of the name label
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "Name :";

            //set the default values of the Touch Event label
            _lblDefaultPosition.AutoSize = true;
            _lblDefaultPosition.Location = new Point(36, 73);
            _lblDefaultPosition.Name = "lblDefaultPosition";
            _lblDefaultPosition.Text = "Default player position:";

            //combobox containing the dimensions and give the ability to change it
            _cbbxDimension.Location = new Point(100, 99);
            _cbbxDimension.Name = "cbbxDimension";
            _cbbxDimension.Size = new Size(138, 20);
            _cbbxDimension.TabIndex = 3;

            List<string> list_strDimensions = new List<string>();

            for (int i = 0; i < DimgrDimensions.List_dimDimensions.Count; i++)
                list_strDimensions.Add(DimgrDimensions.List_dimDimensions[i].StrName);

            _cbbxDimension.Items.AddRange(list_strDimensions.ToArray());

            for (int i = 0; i < DimgrDimensions.List_dimDimensions.Count; i++)
                if (DimgrDimensions.List_dimDimensions[i].IntID == IntDefaultPlayerDimensionID)
                    _cbbxDimension.SelectedIndex = i;

            if (DimgrDimensions.List_dimDimensions.Count > 0)
                _cbbxDimension.Enabled = true;
            else
                _cbbxDimension.Enabled = false;

            //To dimension label
            _lblDimension.AutoSize = true;
            _lblDimension.Location = new Point(36, 99);
            _lblDimension.Name = "lblDimension";
            _lblDimension.Text = "Appear in :";

            //textbox containing the Y location and give the ability to change it
            _txtbxX.Location = new Point(202, 125);
            _txtbxX.Name = "txtbxToX";
            _txtbxX.Size = new Size(36, 20);
            _txtbxX.TabIndex = 2;
            _txtbxX.Text = $"{IntDefaultPlayerX}";
            _txtbxX.TextAlign = HorizontalAlignment.Right;
            _txtbxX.Enabled = false;

            if (DimgrDimensions.List_dimDimensions.Count > 0)
                _txtbxX.Enabled = true;
            else
                _txtbxX.Enabled = false;

            //To X pos label
            _lblX.AutoSize = true;
            _lblX.Location = new Point(36, 125);
            _lblX.Name = "lblX";
            _lblX.Text = "Appear in X location :";

            //textbox containing the X location and give the ability to change it
            _txtbxY.Location = new Point(202, 151);
            _txtbxY.Name = "txtbxY";
            _txtbxY.Size = new Size(36, 20);
            _txtbxY.TabIndex = 2;
            _txtbxY.Text = $"{IntDefaultPlayerY}";
            _txtbxY.TextAlign = HorizontalAlignment.Right;
            _txtbxY.Enabled = false;

            if (DimgrDimensions.List_dimDimensions.Count > 0)
                _txtbxY.Enabled = true;
            else
                _txtbxY.Enabled = false;

            //To Y pos label
            _lblY.AutoSize = true;
            _lblY.Location = new Point(36, 151);
            _lblY.Name = "lblToY";
            _lblY.Text = "Appear in Y location :";

            //set the default values of the cancel button
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 181);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(WorldCancel_Click);

            //set the default values of the validation button
            _btnValidate.Location = new Point(126, 181);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Validate";
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(WorldValidate_Click);

            //set the default values of the world properties form
            _frmWorld.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmWorld.AutoScaleMode = AutoScaleMode.Font;
            _frmWorld.AcceptButton = _btnValidate;
            _frmWorld.CancelButton = _btnCancel;
            _frmWorld.ClientSize = new Size(304, 210);
            _frmWorld.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmWorld.Name = "frmWorld";
            _frmWorld.StartPosition = FormStartPosition.CenterParent;
            _frmWorld.Text = "World";

            //add the controls
            _frmWorld.Controls.Add(_btnValidate);
            _frmWorld.Controls.Add(_btnCancel);
            _frmWorld.Controls.Add(_lblName);
            _frmWorld.Controls.Add(_txtbxName);
            _frmWorld.Controls.Add(_txtbxX);
            _frmWorld.Controls.Add(_txtbxY);
            _frmWorld.Controls.Add(_lblX);
            _frmWorld.Controls.Add(_lblY);
            _frmWorld.Controls.Add(_cbbxDimension);
            _frmWorld.Controls.Add(_lblDimension);
            _frmWorld.Controls.Add(_lblDefaultPosition);

            //resume the layout
            _frmWorld.ResumeLayout(false);
            _frmWorld.PerformLayout();
            #endregion

            //show the form
            _frmWorld.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorldCancel_Click(object sender, EventArgs e) => _frmWorld.Close();

        /// <summary>
        /// Validate the new world values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorldValidate_Click(object sender, EventArgs e)
        {
            //declare the variables used to validate the new values
            string strErrorMessage = "";            //used to tell the user all of his mistakes
            bool blnAreAllValuesValid = true;       //used to tell if all the new values are valid
            bool blnAreNumsValid = true;            //used to tell if all the numerical values are valid

            //declare the variables storing the new values
            string strName_ = _txtbxName.Text;       //store the new name
            int intX_;                              //contain the new X position
            int intY_;                              //contain the new Y position
            int intDimensionID;                     //contain the dimension the player will travel to

            try
            {
                intDimensionID = DimgrDimensions.List_dimDimensions[_cbbxDimension.SelectedIndex].IntID;
            }
            catch
            {
                intDimensionID = 0;
            }

            #region Verify Validity
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

            //test if the new name is already taken
            if (Main.IsWorldNameTaken(strName_, StrName))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the user that not all of his values are valid
                strErrorMessage += $"The new name is already taken !\n";
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

            if (blnAreNumsValid && intDimensionID != -1)
            {
                //test if the new X is smaller that the minimum size
                if (intX_ < 0 || intX_ > (int)DimgrDimensions[intDimensionID].IntWidth - 1)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;
                    //inform the user that the value is not valid
                    strErrorMessage += $"The new X cannot be outside of the targeted dimension !\n";
                }

                //test if the new Y is smaller than the minimum size
                if (intY_ < 0 || intY_ > (int)DimgrDimensions[intDimensionID].IntHeight - 1)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;
                    MessageBox.Show("ehy" + (int)DimgrDimensions[intDimensionID].IntHeight);
                    //inform the user that the value is not valid
                    strErrorMessage += $"The new Y cannot be outside of the targeted dimension !\n";
                }
            }
            #endregion

            //test if all of the values are valid
            if (blnAreAllValuesValid)
            {
                string strSaveLog = CreateTextFromObject(this);

                IntDefaultPlayerDimensionID = intDimensionID;
                IntDefaultPlayerX = intX_;
                IntDefaultPlayerY = intY_;

                string strFormerName = Main.StrCurrentWorldPath;

                //change the values
                StrName = strName_;

                string strResultLog = CreateTextFromObject(this);

                Log.AddLog(ObjectType.World, EditingType.ChangeProperties, strSaveLog, strResultLog);

                //close the form
                _frmWorld.Close();

                //dispose all the data of the form
                _frmWorld.Dispose();

                UpdateTreeNodes();
            }
            else
                //inform the user of all of his mistakes
                MessageBox.Show(strErrorMessage, "One or multiple values are invalid !", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Event getting activate when the dimension's or the world's values are changed
        /// </summary>
        /// <param name="intID_"></param>
        /// <param name="objSender"></param>
        /// <param name="typSender"></param>
        public void UpdateTreeView(int intSenderID, object objSender, Type typSender) 
        {
            if (Main.WrldWorld != null)
                UpdateTreeNodes();
        }

        /// <summary>
        /// Activated when pressing a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void PressKeyDown(object sender, KeyEventArgs eventArgs)
        {
            eventArgs.Handled = true;

            switch (eventArgs.KeyCode)
            {
                //Add an obstacle
                case Keys.Q:
                    try
                    {
                        DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].AddObstacleFromSelection();
                    }
                    catch
                    {
                        MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                //Add a special object
                case Keys.W:
                    try
                    {
                        DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].AddSpecialFromMousePosition();
                    }
                    catch
                    {
                        MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                //detect if pressing the control key
                case Keys.ControlKey:
                    BlnPressingCtrl = true;
                    break;

                //detect if pressing the shift key
                case Keys.ShiftKey:
                    BlnPressingShift = true;
                    break;

                //detect if pressing control and save or select
                case Keys.S:
                    if (BlnPressingCtrl)
                        Main.SaveWorld();
                    else
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].Select();
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    break;

                //detect if pressing control and save or unselect
                case Keys.U:
                    try
                    {
                        DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].UnSelect();
                    }
                    catch
                    {
                        MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;

                //detect if pressing control and set the background to empty
                case Keys.E:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].SetBackgroundToEmpty();
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                //detect if pressing control and set the background to grid
                case Keys.G:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].SetBackgroundToGrid();
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                //detect if pressing control and set the background to image
                case Keys.I:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].SetBackgroundToImage();
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                //cancel last action
                case Keys.Z:
                    if (BlnPressingCtrl)
                        Log.Cancel();
                    break;

                //uncancel last uncancellation
                case Keys.Y:
                    if (BlnPressingCtrl)
                        Log.UnCancel();
                    break;

                //delete all selected
                case Keys.Delete:
                    try
                    {
                        DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].RemoveAllSelected();
                    }
                    catch
                    {
                        MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                //delete all selected
                case Keys.Back:
                    try
                    {
                        DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].RemoveAllSelected();
                    }
                    catch
                    {
                        MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                //move the selection up
                case Keys.Up:
                    if (BlnPressingCtrl && !BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].MoveSelection(0, 1);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].GrowSelection(0, 1);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].ShrinkSelection(0, -1);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                //move the selection down
                case Keys.Down:
                    if (BlnPressingCtrl && !BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].MoveSelection(0, -1);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].GrowSelection(0, -1);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].ShrinkSelection(0, 1);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                //move the selection left
                case Keys.Left:
                    if (BlnPressingCtrl && !BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].MoveSelection(-1, 0);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].GrowSelection(-1, 0);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].ShrinkSelection(1, 0);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                //move the selection right
                case Keys.Right:
                    if (BlnPressingCtrl && !BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].MoveSelection(1, 0);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (!BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].GrowSelection(1, 0);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (BlnPressingCtrl && BlnPressingShift)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].ShrinkSelection(-1, 0);
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.Escape:
                    try
                    {
                        DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].UnSelectAll();
                    }
                    catch
                    {
                        MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case Keys.A:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            DimgrDimensions.List_dimDimensions[DimgrDimensions.IntSelectedDimension].SelectAll();
                        }
                        catch
                        {
                            MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D1:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[0].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[0].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D2:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[1].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[1].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D3:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[2].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[2].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D4:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[3].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[3].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D5:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[4].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[4].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D6:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[5].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[5].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D7:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[6].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[6].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D8:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[7].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[7].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D9:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[8].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[8].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case Keys.D0:
                    if (BlnPressingCtrl)
                    {
                        try
                        {
                            IntSpecialTypeID = SpctypgrTypes.List_spctypTypes[9].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No special type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            IntObstacleTypeID = ObstypgrTypes.List_obstypTypes[9].IntID;
                        }
                        catch
                        {
                            MessageBox.Show("No obstacle type found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Activated when unpressing a key
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void PressKeyUp(object sender, KeyEventArgs eventArgs)
        {
            switch (eventArgs.KeyCode)
            {
                //detect if unpressing the control key
                case Keys.ControlKey:
                    BlnPressingCtrl = false;
                    break;

                //detect if pressing the shift key
                case Keys.ShiftKey:
                    BlnPressingShift = false;
                    break;
            }
        }

        /// <summary>
        /// Reset the key pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void LeaveForm(object sender, EventArgs eventArgs)
        {
            BlnPressingCtrl = false;
            BlnPressingShift = false;
        }
        #endregion
    }
}