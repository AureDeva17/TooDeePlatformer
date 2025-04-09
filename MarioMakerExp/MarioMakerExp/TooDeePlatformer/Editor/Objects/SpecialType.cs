/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                         **
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
** This class contains all the special types                                 **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// This class contains all the special types.
    /// </summary>
    class SpecialType
    {
        //declare the static properties
        public const string strCLASSNAME = "SpecialType";                   //class name
        public const ObjectType objTYPE = ObjectType.SpecialType;           //object type
        public static int IntNextID { get; set; }                           //next ID to be used
        public static List<SpecialType> List_spctypTypes { get; set; }      //list of all the types 

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_spctypTypes.Count; i++)
                if (List_spctypTypes[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_spctypTypes[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static SpecialType GetObjectByID(int intID) => List_spctypTypes[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
            string strText = Main.strVERSION + "\n";

            strText += strCLASSNAME + "\n";
            strText += IntNextID + "\n";
            strText += List_spctypTypes.Count + "\n";

            for (int i = 0; i < List_spctypTypes.Count; i++)
            {
                strText += CreateTextFromObject(List_spctypTypes[i]);

                if (List_spctypTypes.Count != i)
                    strText += "\n";
            }

            return strText;
        }

        /// <summary>
        /// Copy the data from the file to the special type
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrSpecialTypeTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_spctypTypes.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_spctypTypes.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                    
                }
                else
                    MessageBox.Show("The file isn't a storage for special types !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a line of text from an object
        /// </summary>
        /// <param name="spctypType"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(SpecialType spctypType)
        {
            string strText = "";

            strText += spctypType.IntID + $"{Main.chrSEPARATOR}";
            strText += spctypType.IntWorldID + $"{Main.chrSEPARATOR}";
            strText += spctypType.StrName + $"{Main.chrSEPARATOR}";
            strText += spctypType.IntSolidness + $"{Main.chrSEPARATOR}";
            strText += spctypType.IntBounciness + $"{Main.chrSEPARATOR}";
            strText += spctypType.BlnDoBounce + $"{Main.chrSEPARATOR}";
            strText += spctypType.IntWidth + $"{Main.chrSEPARATOR}";
            strText += spctypType.IntHeight + $"{Main.chrSEPARATOR}";
            strText += spctypType.BlnKillPlayer + $"{Main.chrSEPARATOR}";
            strText += spctypType.BlnTravelTo + $"{Main.chrSEPARATOR}";
            strText += spctypType.BlnStartGroupMove + $"{Main.chrSEPARATOR}";
            strText += spctypType.BlnWinGame + $"{Main.chrSEPARATOR}";
            strText += spctypType.BlnOnlyOnEnter;

            return strText;
        }

        /// <summary>
        /// Create an object from a line of text
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static SpecialType CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            SpecialType spctypNewSpecialType = new SpecialType()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntWorldID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                IntSolidness = Convert.ToInt32(tab_strObject[3]),
                IntBounciness = Convert.ToInt32(tab_strObject[4]),
                BlnDoBounce = Convert.ToBoolean(tab_strObject[5]),
                IntWidth = Convert.ToInt32(tab_strObject[6]),
                IntHeight = Convert.ToInt32(tab_strObject[7]),
                BlnKillPlayer = Convert.ToBoolean(tab_strObject[8]),
                BlnTravelTo = Convert.ToBoolean(tab_strObject[9]),
                BlnStartGroupMove = Convert.ToBoolean(tab_strObject[10]),
                BlnWinGame = Convert.ToBoolean(tab_strObject[11]),
                BlnOnlyOnEnter = Convert.ToBoolean(tab_strObject[12]),
                ImglytLayout = ImageLayout.Zoom,
            };

            if (File.Exists(spctypNewSpecialType.StrTexturePath))
            {
                Image imgNewImage = Image.FromFile(spctypNewSpecialType.StrTexturePath);
                spctypNewSpecialType.ImgTexture = new Bitmap(imgNewImage);
                imgNewImage.Dispose();
            }
            else
                spctypNewSpecialType.ImgTexture = null;

            return spctypNewSpecialType;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static SpecialType()
        {
            IntNextID = 0;
            List_spctypTypes = new List<SpecialType>();
        }
        #endregion

        //declare the events
        public event dlgPersoEvent evtValuesChanged;        //triggered when visual values are changed

        //declare the properties
        public int IntID { get; set; }                      //id of the object
        public int IntSolidness { get; set; }               //contains the solidness of the type
        public int IntBounciness { get; set; }              //contains the bounciness of the type
        public bool BlnDoBounce { get; set; }               //does people bounce on the type
        public int IntWidth { get; set; }                   //width of the object
        public int IntHeight { get; set; }                  //height of the object
        public bool BlnKillPlayer { get; set; }             //does touching the object kill the player
        public bool BlnTravelTo { get; set; }               //does touching the object make you travel
        public bool BlnStartGroupMove { get; set; }         //does touching the object make a group move
        public bool BlnWinGame { get; set; }                //does touching the object make you win the game
        public bool BlnOnlyOnEnter { get; set; }            //does the event only activate on enter
        public Image ImgTexture { get; set; }               //contains the image of the type
        public ImageLayout ImglytLayout { get; set; }       //contains the layout of the image
        public int IntWorldID { get; set; }                 //contain the parent

        //declare the fields
        private string _strName;                            //name of the object

        #region Properties
        /// <summary>
        /// Get and set the name of the object
        /// </summary>
        public string StrName
        {
            get => _strName;

            set
            {
                if (!SpctypgrParent.IsTypeNameTaken(value, StrName))
                {
                    _strName = value;

                    evtValuesChanged?.Invoke(IntID, this, GetType());
                }
            }
        }

        /// <summary>
        /// Parent of the object
        /// </summary>
        public SpecialTypeGroup SpctypgrParent
        {
            get => World.List_wrldWorlds[World.GetIndexFromID(IntWorldID)].SpctypgrTypes;
        }

        /// <summary>
        /// In world
        /// </summary>
        public World WrldInWorld
        {
            get => SpctypgrParent.WrldParent;
        }

        /// <summary>
        /// Path of the background image
        /// </summary>
        public string StrTexturePath
        {
            get => Main.StrSTImagesPath + @"\" + StrName + @".png";
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the object
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="intSolidness_"></param>
        /// <param name="intBounciness_"></param>
        /// <param name="blnDoBounce_"></param>
        /// <param name="imgTexture_"></param>
        /// <param name="imglytLayout_"></param>
        /// <param name="intWorldID_"></param>
        public SpecialType(string strName_, int intWidth, int intHeight, bool blnKill, bool blnTravel, bool blnActFolder, bool blnWin, bool blnEnter, int intSolidness_, int intBounciness_, bool blnDoBounce_, Image imgTexture_, ImageLayout imglytLayout_, int intWorldID_)
        {
            //set the defaults variable
            IntWorldID = intWorldID_;
            IntID = IntNextID++;
            this._strName = strName_;
            IntSolidness = intSolidness_;
            IntBounciness = intBounciness_;
            BlnDoBounce = blnDoBounce_;
            ImgTexture = imgTexture_;
            ImglytLayout = imglytLayout_;
            BlnWinGame = blnWin;
            BlnTravelTo = blnTravel;
            BlnOnlyOnEnter = blnEnter;
            BlnStartGroupMove = blnActFolder;
            BlnKillPlayer = blnKill;
            this.IntHeight = intHeight;
            this.IntWidth = intWidth;

            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create a special type from a file
        /// </summary>
        public SpecialType() => evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node representing the obstacle
        /// </summary>
        public TreeNode GetTreeNode() => new TreeNode(StrName) { ContextMenuStrip = GetContextMenu() };
        #endregion

        #region Context box
        private ContextMenuStrip GetContextMenu()
        {
            //declare everything needed to create a context menu
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                     //contain the context menu
            List<ToolStripMenuItem> tsmiItems = new List<ToolStripMenuItem>();              //contain all the items used to create the context menu

            //tool used to select the type
            tsmiItems.Add(new ToolStripMenuItem("Select"));
            tsmiItems[0].Tag = IntID;
            tsmiItems[0].Click += new EventHandler(SelectTypeMenu);

            //tool used to remove the type
            tsmiItems.Add(new ToolStripMenuItem("Remove"));
            tsmiItems[1].Tag = IntID;
            tsmiItems[1].Click += new EventHandler(RemoveTypeMenu);

            //tool used to access the properties of the type
            tsmiItems.Add(new ToolStripMenuItem("Properties"));
            tsmiItems[2].Tag = IntID;
            tsmiItems[2].Click += new EventHandler(ModifyTypePropertiesMenu);

            //add the tools to the context menu
            contextMenuStrip.Items.AddRange(tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Select as the type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectTypeMenu(object sender, EventArgs e) => WrldInWorld.IntSpecialTypeID = IntID;

        /// <summary>
        /// Remove the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveTypeMenu(object sender, EventArgs e)
        {
            //search for the obstacle
            for (int i = 0; i < List_spctypTypes.Count; i++)
                if (List_spctypTypes[i].IntID == IntID)
                {
                    SpctypgrParent.Remove(i);
                    evtValuesChanged?.Invoke(IntID, this, GetType());

                    break;
                }
        }

        /// <summary>
        /// Open the properties form of the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyTypePropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Image
        /// <summary>
        /// Save the texture to the file
        /// </summary>
        public void SaveTexture()
        {
            if (ImgTexture != null)
            {
                using (Image imgToSave = new Bitmap(ImgTexture))
                    imgToSave.Save(StrTexturePath, ImageFormat.Png);
            }
        }
        #endregion

        #region Properties Form
        //declare the control object used to display the properties form
        private Form _frmType;                          //Type properties form
        private TextBox _txtbxName;                     //textbox used to change the name of the type
        private TextBox _txtbxSolidness;                //textbox used to change the solidness
        private TextBox _txtbxBounciness;               //textbox used to change the bounciness
        private TextBox _txtbxWidth;                    //textbox used to change the width
        private TextBox _txtbxHeight;                   //textbox used to change the height
        private CheckBox _chkbxDoBounce;                //checkbox used to tell if things can bounce on the type
        private CheckBox _chkbxDoKill;                  //checkbox used to tell if a player get killed by stepping on the object
        private CheckBox _chkbxDoWin;                   //checkbox used to tell if a player win the game by stepping on the object
        private CheckBox _chkbxDoTravel;                //checkbox used to tell if a player travel to a new position by stepping on the object
        private CheckBox _chkbxDoStartMoving;           //checkbox used to tell if a player activate a folder's move group by stepping on the object
        private CheckBox _chkbxOnlyOnEnter;             //checkbox used to tell if the event is only activated on enter
        private PictureBox _pcbxTexture;                //picturebox used to display the current texture
        private Label _lblBounciness;                   //label informing that the textbox next to it is for the bounciness
        private Label _lblSolidness;                    //label informing that the textbox next to it is for the solidness
        private Label _lblWidth;                        //label informing the width of the type
        private Label _lblHeight;                       //label informing the height of the type
        private Label _lblName;                         //label informing that the textbox next to it is for the name
        private Label _lblTouchEvent;                   //label informing that the checkboxes underneath are for a touching event
        private Button _btnGetImage;                    //button used to start the searching of a texture
        private Button _btnCancel;                      //button used to close the properties form
        private Button _btnValidate;                    //button used to validate the new values

        /// <summary>
        /// Open the properties dimension form
        /// </summary>
        /// <param name="intIndex"></param>
        public void OpenPropertiesForm()
        {
            //reset all the controls
            _frmType = new Form();
            _txtbxName = new TextBox();
            _txtbxSolidness = new TextBox();
            _txtbxBounciness = new TextBox();
            _txtbxWidth = new TextBox();
            _txtbxHeight = new TextBox();
            _chkbxDoBounce = new CheckBox();
            _chkbxDoBounce = new CheckBox();
            _chkbxDoKill = new CheckBox();
            _chkbxDoWin = new CheckBox();
            _chkbxDoTravel = new CheckBox();
            _chkbxDoStartMoving = new CheckBox();
            _chkbxOnlyOnEnter = new CheckBox();
            _pcbxTexture = new PictureBox();
            _lblTouchEvent = new Label();
            _lblBounciness = new Label();
            _lblSolidness = new Label();
            _lblWidth = new Label();
            _lblHeight = new Label();
            _lblName = new Label();
            _btnGetImage = new Button();
            _btnCancel = new Button();
            _btnValidate = new Button();

            #region Controls
            //suspend the layout
            _frmType.SuspendLayout();

            //set the default values of the name textbox
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //set the default values of the Solidness textbox
            _txtbxSolidness.Location = new Point(202, 69);
            _txtbxSolidness.Name = "txtbxSolidness";
            _txtbxSolidness.Size = new Size(36, 20);
            _txtbxSolidness.TabIndex = 1;
            _txtbxSolidness.Text = $"{IntSolidness}";
            _txtbxSolidness.TextAlign = HorizontalAlignment.Right;

            //set the default values of the Bounciness textbox
            _txtbxBounciness.Location = new Point(202, 95);
            _txtbxBounciness.Name = "txtbxBounciness";
            _txtbxBounciness.Size = new Size(36, 20);
            _txtbxBounciness.TabIndex = 2;
            _txtbxBounciness.Text = $"{IntBounciness}";
            _txtbxBounciness.TextAlign = HorizontalAlignment.Right;

            //contain the information about the looping
            _chkbxDoBounce.Location = new Point(50, 121);
            _chkbxDoBounce.Name = "chkbxDoBounce";
            _chkbxDoBounce.Size = new Size(200, 20);
            _chkbxDoBounce.TabIndex = 3;
            _chkbxDoBounce.Text = $"Does the type make things bounce ?";
            _chkbxDoBounce.Checked = BlnDoBounce;

            //set the default values of the width textbox
            _txtbxWidth.Location = new Point(202, 147);
            _txtbxWidth.MaxLength = 20;
            _txtbxWidth.Name = "txtbxWidth";
            _txtbxWidth.Size = new Size(36, 20);
            _txtbxWidth.TabIndex = 4;
            _txtbxWidth.Text = $"{IntWidth}";
            _txtbxWidth.TextAlign = HorizontalAlignment.Right;

            //set the default values of the width label
            _lblWidth.AutoSize = true;
            _lblWidth.Location = new Point(36, 147);
            _lblWidth.Name = "lblWidth";
            _lblWidth.Text = "Width :";

            //set the default values of the Height textbox
            _txtbxHeight.Location = new Point(202, 173);
            _txtbxHeight.MaxLength = 20;
            _txtbxHeight.Name = "txtbxHeight";
            _txtbxHeight.Size = new Size(36, 20);
            _txtbxHeight.TabIndex = 5;
            _txtbxHeight.Text = $"{IntHeight}";
            _txtbxHeight.TextAlign = HorizontalAlignment.Right;

            //set the default values of the Height label
            _lblHeight.AutoSize = true;
            _lblHeight.Location = new Point(36, 173);
            _lblHeight.Name = "lblHeight";
            _lblHeight.Text = "Height :";

            //set the default values of the Touch Event label
            _lblTouchEvent.AutoSize = true;
            _lblTouchEvent.Location = new Point(36, 199);
            _lblTouchEvent.Name = "lblTouchEvent";
            _lblTouchEvent.Text = "Touch Events:";

            //contain the information about the kill event
            _chkbxDoKill.Location = new Point(50, 225);
            _chkbxDoKill.Name = "chkbxDoKill";
            _chkbxDoKill.Size = new Size(200, 20);
            _chkbxDoKill.TabIndex = 4;
            _chkbxDoKill.Text = $"Does the player die ?";
            _chkbxDoKill.Checked = BlnKillPlayer;

            //contain the information about the winning event
            _chkbxDoWin.Location = new Point(50, 251);
            _chkbxDoWin.Name = "chkbxDoWin";
            _chkbxDoWin.Size = new Size(200, 20);
            _chkbxDoWin.TabIndex = 5;
            _chkbxDoWin.Text = $"Does the player win ?";
            _chkbxDoWin.Checked = BlnWinGame;

            //contain the information about the winning event
            _chkbxDoTravel.Location = new Point(50, 277);
            _chkbxDoTravel.Name = "chkbxDoTravel";
            _chkbxDoTravel.Size = new Size(200, 20);
            _chkbxDoTravel.TabIndex = 6;
            _chkbxDoTravel.Text = $"Can the player travel ?";
            _chkbxDoTravel.Checked = BlnTravelTo;

            //contain the information about the starting of a folder's movements event
            _chkbxDoStartMoving.Location = new Point(50, 303);
            _chkbxDoStartMoving.Name = "chkbxDoStartMoving";
            _chkbxDoStartMoving.Size = new Size(200, 20);
            _chkbxDoStartMoving.TabIndex = 7;
            _chkbxDoStartMoving.Text = $"Can it activate a folder's movements ?";
            _chkbxDoStartMoving.Checked = BlnStartGroupMove;

            //contain the information about the starting of a folder's movements event
            _chkbxOnlyOnEnter.Location = new Point(50, 329);
            _chkbxOnlyOnEnter.Name = "chkbxOnlyOnEnter";
            _chkbxOnlyOnEnter.Size = new Size(200, 20);
            _chkbxOnlyOnEnter.TabIndex = 7;
            _chkbxOnlyOnEnter.Text = $"Only activated on enter ?";
            _chkbxOnlyOnEnter.Checked = BlnOnlyOnEnter;

            //set the default values of the Texture picturebox
            _pcbxTexture.Location = new Point(100, 355);
            _pcbxTexture.Name = "pcbxTexture";
            _pcbxTexture.Size = new Size(100, 79);
            _pcbxTexture.BackgroundImageLayout = ImageLayout.Zoom;
            _pcbxTexture.BackgroundImage = ImgTexture;
            _pcbxTexture.TabStop = false;

            //set the default values of the Bounciness label
            _lblBounciness.AutoSize = true;
            _lblBounciness.Location = new Point(36, 95);
            _lblBounciness.Name = "lblBounciness";
            _lblBounciness.Size = new Size(44, 13);
            _lblBounciness.TabIndex = 4;
            _lblBounciness.Text = "Bounciness :";

            //set the default values of the Solidness label
            _lblSolidness.AutoSize = true;
            _lblSolidness.Location = new Point(36, 69);
            _lblSolidness.Name = "lblSolidness";
            _lblSolidness.Size = new Size(41, 13);
            _lblSolidness.TabIndex = 5;
            _lblSolidness.Text = "Solidness :";

            //set the default values of the name label
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "Name :";

            //set the default values of the image giver button
            _btnGetImage.Location = new Point(91, 435);
            _btnGetImage.Name = "btnGetImage";
            _btnGetImage.Size = new Size(120, 23);
            _btnGetImage.TabIndex = 8;
            _btnGetImage.Text = "Search a texture";
            _btnGetImage.UseVisualStyleBackColor = true;
            _btnGetImage.Click += new EventHandler(btnGetImage_Click);

            //set the default values of the cancel button
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 486);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 9;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(Cancel_Click);

            //set the default values of the validation button
            _btnValidate.Location = new Point(121, 486);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 10;
            _btnValidate.Text = "Validate";
            _btnValidate.Tag = IntID;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //set the default values of the main properties form
            _frmType.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmType.AutoScaleMode = AutoScaleMode.Font;
            _frmType.AcceptButton = _btnValidate;
            _frmType.CancelButton = _btnCancel;
            _frmType.ClientSize = new Size(304, 521);
            _frmType.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmType.Name = "frmSpecialType";
            _frmType.StartPosition = FormStartPosition.CenterParent;
            _frmType.Text = "Special Type";
            _frmType.Tag = IntID;

            //add the controls to the form
            _frmType.Controls.Add(_btnValidate);
            _frmType.Controls.Add(_btnCancel);
            _frmType.Controls.Add(_btnGetImage);
            _frmType.Controls.Add(_lblName);
            _frmType.Controls.Add(_lblSolidness);
            _frmType.Controls.Add(_lblBounciness);
            _frmType.Controls.Add(_lblWidth);
            _frmType.Controls.Add(_lblHeight);
            _frmType.Controls.Add(_txtbxWidth);
            _frmType.Controls.Add(_txtbxHeight);
            _frmType.Controls.Add(_pcbxTexture);
            _frmType.Controls.Add(_txtbxBounciness);
            _frmType.Controls.Add(_txtbxSolidness);
            _frmType.Controls.Add(_txtbxName);
            _frmType.Controls.Add(_chkbxDoBounce);
            _frmType.Controls.Add(_chkbxDoKill);
            _frmType.Controls.Add(_chkbxDoStartMoving);
            _frmType.Controls.Add(_chkbxDoTravel);
            _frmType.Controls.Add(_chkbxDoWin);
            _frmType.Controls.Add(_chkbxOnlyOnEnter);
            _frmType.Controls.Add(_lblTouchEvent);

            //resume the layout
            _frmType.ResumeLayout(false);
            _frmType.PerformLayout();
            #endregion

            //show the form
            _frmType.ShowDialog();
        }

        /// <summary>
        /// Get an image from the user's files
        /// </summary>
        /// <returns></returns>
        public Bitmap GetImage()
        {
            //declare the control used to get an image from a file
            OpenFileDialog ofdDialog = new OpenFileDialog();        //used to search a image file

            //set the title
            ofdDialog.Title = "Open Image";

            //set the filter
            ofdDialog.Filter = "Image files|*.png";

            //test if the file result is ok
            if (ofdDialog.ShowDialog() == DialogResult.OK)
            {
                //return the image
                return new Bitmap(ofdDialog.FileName);
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
            _pcbxTexture.BackgroundImage = GetImage();

            if (_pcbxTexture.BackgroundImage == null)
                _pcbxTexture.BackgroundImage = ImgTexture;

            //set the layout
            _pcbxTexture.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmType.Close();

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
            bool blnAreNumsValid = true;             //used to tell if all the numerical values are valid

            //declare the variables storing the new values
            string strName_ = _txtbxName.Text;       //store the new name
            int intSolidness_;                      //store the new solidness
            int intBounciness_;                     //store the new bounciness
            int intWidth_;                           //store the new width
            int intHeight_;                         //store the new height
            Image imgTexture_ = null;              //store the new background

            //test if there is a new image
            if (_pcbxTexture.BackgroundImage != null)
                //get the new background
                imgTexture_ = _pcbxTexture.BackgroundImage;

            #region Verify Validity
            //test if the new name is taken
            if (SpctypgrParent.IsTypeNameTaken($"{strName_}", StrName))
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

            if (strName_.ToLower().Contains($"{$"{Main.chrSEPARATOR}"}"))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the user that not all of his values are valid
                strErrorMessage += $"The new name must not have a \"{$"{$"{Main.chrSEPARATOR}"}"}\" inside !\n";
            }

            for (int i = 0; i < Path.GetInvalidPathChars().Length; i++)
                if (strName_.Contains($"{Path.GetInvalidPathChars()[i]}"))
                {
                    //tell the game that the values are invalid
                    blnAreAllValuesValid = false;

                    //tell the user the value is invalid
                    strErrorMessage += $"The new name is not compatible for a file name !\n";

                    break;
                }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxSolidness.Text, out intSolidness_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new Solidness is not an integrer !\n";
            }

            //test if the value cannot be converted
            if (!int.TryParse(_txtbxBounciness.Text, out intBounciness_))
            {
                //inform the game that not all the values are valid
                blnAreAllValuesValid = false;

                //inform the game that the numerical values cannot be converted
                blnAreNumsValid = false;

                //inform the user that the new value cannot be converted
                strErrorMessage += $"The new bounciness is not an integrer !\n";
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

            //test if all the numerical values are converted successfully
            if (blnAreNumsValid)
            {
                //test if the new solidness is smaller that the minimum size
                if (intSolidness_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new Solidness cannot be less than 0 !\n";
                }

                //test if the new bounciness is smaller than the minimum size
                if (intBounciness_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new bounciness cannot be less than 0 !\n";
                }

                //test if the new width is smaller than the minimum size
                if (intWidth_ <= 0 && intWidth_ > 10)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new width must be more than 0 and less than 10 !\n";
                }

                //test if the new height is smaller than the minimum size
                if (intHeight_ <= 0 && intHeight_ > 10)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new height must be more than 0 and less than 10 !\n";
                }

                //test if the size can be changed
                if (intHeight_ != IntHeight || intWidth_ != IntWidth)
                    for (int i = 0; i < Special.List_spcSpecials.Count; i++)
                        if (Special.List_spcSpecials[i].IntSpecialTypeID == IntID)
                        {
                            //inform the game that not all the values are valid
                            blnAreAllValuesValid = false;

                            //inform the user that the value is not valid
                            strErrorMessage += $"You cannot change the size of the object if there are already objects of this type !\n";

                            break;
                        }
            }

            //test if the new background is not equal to null
            if (imgTexture_ != null)
                //test if the new background is not valid
                if (imgTexture_.Width != intWidth_ * Main.intUNITSIZE || imgTexture_.Height != intHeight_ * Main.intUNITSIZE)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the image size is not valid
                    strErrorMessage += $"The image must be {intWidth_ * Main.intUNITSIZE}x{intHeight_ * Main.intUNITSIZE}p\n";
                }
            #endregion

            //test if all the values are valid
            if (blnAreAllValuesValid)
            {
                string strSaveLog = CreateTextFromObject(this);
                Image imgSave = new Bitmap(ImgTexture);

                //change all the values
                StrName = strName_;
                IntSolidness = intSolidness_;
                IntBounciness = intBounciness_;
                IntWidth = intWidth_;
                IntHeight = intHeight_;
                ImgTexture = imgTexture_;
                BlnDoBounce = _chkbxDoBounce.Checked;
                BlnKillPlayer = _chkbxDoKill.Checked;
                BlnStartGroupMove = _chkbxDoStartMoving.Checked;
                BlnTravelTo = _chkbxDoTravel.Checked;
                BlnWinGame = _chkbxDoWin.Checked;
                BlnOnlyOnEnter = _chkbxOnlyOnEnter.Checked;

                for (int i = 0; i < Special.List_spcSpecials.Count; i++)
                    if (Special.List_spcSpecials[i].IntSpecialTypeID == IntID)
                        Special.List_spcSpecials[i].ResetImage();

                Log.AddLog(ObjectType.SpecialType, EditingType.ChangeProperties, strSaveLog, CreateTextFromObject(this), imgSave, ImgTexture);

                //close the form
                _frmType.Close();

                //dispose all the data
                _frmType.Dispose();
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