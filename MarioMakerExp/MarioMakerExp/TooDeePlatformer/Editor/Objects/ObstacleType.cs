/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 11.06.2021                                                    **
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
** This class contains all the obstacle types.                               **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// This class contains all the obstacle types.
    /// </summary>
    class ObstacleType
    {
        //declare the static properties
        public const string strCLASSNAME = "ObstacleType";                  //class name
        public const ObjectType objTYPE = ObjectType.ObstacleType;          //object type
        public static int IntNextID { get; set; }                           //next ID to be used
        public static List<ObstacleType> List_obstypTypes { get; set; }     //list of all the types 

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_obstypTypes.Count; i++)
                if (List_obstypTypes[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_obstypTypes[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static ObstacleType GetObjectByID(int intID) => List_obstypTypes[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
                string strText = Main.strVERSION + "\n";

                strText += strCLASSNAME + "\n";
                strText += IntNextID + "\n";
                strText += List_obstypTypes.Count + "\n";

                for (int i = 0; i< List_obstypTypes.Count; i++)
                {
                    strText += CreateTextFromObject(List_obstypTypes[i]);

                    if (List_obstypTypes.Count != i)
                        strText += "\n";
                }

                return strText;
        }

        /// <summary>
        /// Copy the data from the file to the obstacle type
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrObstacleTypeTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_obstypTypes.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_obstypTypes.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                }
                else
                    MessageBox.Show("The file isn't a storage for obstacle types !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a line of text from an object
        /// </summary>
        /// <param name="obstypType"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(ObstacleType obstypType)
        {
            string strText = "";

            strText += obstypType.IntID + $"{Main.chrSEPARATOR}";
            strText += obstypType.IntWorldID + $"{Main.chrSEPARATOR}";
            strText += obstypType.StrName + $"{Main.chrSEPARATOR}";
            strText += obstypType.IntSolidness + $"{Main.chrSEPARATOR}";
            strText += obstypType.IntBounciness + $"{Main.chrSEPARATOR}";
            strText += obstypType.BlnDoBounce;

            return strText;
        }

        /// <summary>
        /// Create an object from a line of text
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static ObstacleType CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            ObstacleType obstypNewObstacleType = new ObstacleType()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntWorldID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                IntSolidness = Convert.ToInt32(tab_strObject[3]),
                IntBounciness = Convert.ToInt32(tab_strObject[4]),
                BlnDoBounce = Convert.ToBoolean(tab_strObject[5]),
                ImglytLayout = ImageLayout.Tile,
            };


            if (File.Exists(Main.StrOTImagesPath + @"\" + tab_strObject[2] + ".png"))
            {
                Image imgNewImage = Image.FromFile(Main.StrOTImagesPath + @"\" + tab_strObject[2] + ".png");
                obstypNewObstacleType.ImgTexture = new Bitmap(imgNewImage);
                imgNewImage.Dispose();
            }
            else
                obstypNewObstacleType.ImgTexture = null;


            return obstypNewObstacleType;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static ObstacleType()
        {
            IntNextID = 0;
            List_obstypTypes = new List<ObstacleType>();
        }
        #endregion

        //declare the events
        public event dlgPersoEvent evtValuesChanged;        //triggered when visual values are changed

        //declare the properties
        public int IntID { get; set; }                      //id of the object
        public int IntSolidness { get; set; }               //contains the solidness of the type
        public int IntBounciness { get; set; }              //contains the bounciness of the type
        public bool BlnDoBounce { get; set; }               //does people bounce on the type
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
                _strName = value;

                evtValuesChanged?.Invoke(IntID, this, GetType());
            }
        }

        /// <summary>
        /// Parent of the object
        /// </summary>
        public ObstacleTypeGroup ObstypgrParent
        {
            get => World.List_wrldWorlds[World.GetIndexFromID(IntWorldID)].ObstypgrTypes;
        }

        /// <summary>
        /// In world
        /// </summary>
        public World WrldInWorld
        {
            get => ObstypgrParent.WrldParent;
        }

        /// <summary>
        /// Path of the background image
        /// </summary>
        public string StrTexturePath
        {
            get => Main.StrOTImagesPath + @"\" + StrName + @".png";
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
        public ObstacleType(string strName_, int intSolidness_, int intBounciness_, bool blnDoBounce_, Image imgTexture_, ImageLayout imglytLayout_, int intWorldID_)
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

            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create an obstacle type from a storage of obstacle types
        /// </summary>
        public ObstacleType() => evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node representing the obstacle
        /// </summary>
        public TreeNode GetTreeNode() => new TreeNode(StrName) { ContextMenuStrip = GetContextMenu() };
        #endregion

        #region Image
        /// <summary>
        /// Save the texture to the file
        /// </summary>
        public void SaveTexture()
        {
            if (ImgTexture != null)
                using (Image imgToSave = new Bitmap(ImgTexture))
                    imgToSave.Save(StrTexturePath, ImageFormat.Png);
        }
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
        private void SelectTypeMenu(object sender, EventArgs e) => WrldInWorld.IntObstacleTypeID = IntID;

        /// <summary>
        /// Remove the clicked obstacle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveTypeMenu(object sender, EventArgs e)
        {
            //search for the obstacle
            for (int i = 0; i < List_obstypTypes.Count; i++)
                if (List_obstypTypes[i].IntID == IntID)
                {
                    ObstypgrParent.Remove(i);
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

        #region Properties Form
        //declare the control object used to display the properties form
        private Form _frmType;                      //Type properties form
        private TextBox _txtbxName;                 //textbox used to change the name of the type
        private TextBox _txtbxSolidness;            //textbox used to change the solidness
        private TextBox _txtbxBounciness;           //textbox used to change the bounciness
        private CheckBox _chkbxDoBounce;            //checkbox used to tell if things can bounce on the type
        private PictureBox _pcbxTexture;            //picturebox used to display the current texture
        private Label _lblBounciness;               //label informing that the textbox next to it is for the bounciness
        private Label _lblSolidness;                //label informing that the textbox next to it is for the solidness
        private Label _lblName;                     //label informing that the textbox next to it is for the name
        private Button _btnGetImage;                //button used to start the searching of a texture
        private Button _btnCancel;                  //button used to close the properties form
        private Button _btnValidate;                //button used to validate the new values

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
            _chkbxDoBounce = new CheckBox();
            _pcbxTexture = new PictureBox();
            _lblBounciness = new Label();
            _lblSolidness = new Label();
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
            _chkbxDoBounce.TabIndex = 0;
            _chkbxDoBounce.Text = $"Does the type make things bounce ?";
            _chkbxDoBounce.Checked = BlnDoBounce;

            //set the default values of the Texture picturebox
            _pcbxTexture.Location = new Point(100, 138);
            _pcbxTexture.Name = "pcbxTexture";
            _pcbxTexture.Size = new Size(100, 79);
            _pcbxTexture.TabIndex = 3;
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
            _btnGetImage.Location = new Point(91, 218);
            _btnGetImage.Name = "btnGetImage";
            _btnGetImage.Size = new Size(120, 23);
            _btnGetImage.TabIndex = 7;
            _btnGetImage.Text = "Search a texture";
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
            _frmType.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmType.AutoScaleMode = AutoScaleMode.Font;
            _frmType.AcceptButton = _btnValidate;
            _frmType.CancelButton = _btnCancel;
            _frmType.ClientSize = new Size(304, 303);
            _frmType.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmType.Name = "frmObstacleType";
            _frmType.StartPosition = FormStartPosition.CenterParent;
            _frmType.Text = "Obstacle Type";
            _frmType.Tag = IntID;

            //add the controls to the form
            _frmType.Controls.Add(_btnValidate);
            _frmType.Controls.Add(_btnCancel);
            _frmType.Controls.Add(_btnGetImage);
            _frmType.Controls.Add(_lblName);
            _frmType.Controls.Add(_lblSolidness);
            _frmType.Controls.Add(_lblBounciness);
            _frmType.Controls.Add(_pcbxTexture);
            _frmType.Controls.Add(_txtbxBounciness);
            _frmType.Controls.Add(_txtbxSolidness);
            _frmType.Controls.Add(_txtbxName);
            _frmType.Controls.Add(_chkbxDoBounce);

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
            bool blnAreNumsValid = true;            //used to tell if all the numerical values are valid

            //declare the variables storing the new values
            string strName_ = _txtbxName.Text;       //store the new name
            int intSolidness_;                      //store the new solidness
            int intBounciness_;                     //store the new bounciness
            Image imgTexture_ = null;              //store the new background

            //test if there is a new image
            if (_pcbxTexture.BackgroundImage != null)
                //get the new background
                imgTexture_ = _pcbxTexture.BackgroundImage;

            #region Verify Validity
            //test if the new name is taken
            if (ObstypgrParent.IsTypeNameTaken($"{strName_}", StrName))
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

            //test if all the numerical values are converted successfully
            if (blnAreNumsValid)
            {
                //test if the new width is smaller that the minimum size
                if (intSolidness_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new Solidness cannot be less than 0 !\n";
                }

                //test if the new height is smaller than the minimum size
                if (intBounciness_ < 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the value is not valid
                    strErrorMessage += $"The new bounciness cannot be less than 0 !\n";
                }
            }

            //test if the new background is not equal to null
            if (imgTexture_ != null)
                //test if the new background is not valid
                if (imgTexture_.Width % Main.intUNITSIZE != 0 && imgTexture_.Height % Main.intUNITSIZE != 0)
                {
                    //inform the game that not all the values are valid
                    blnAreAllValuesValid = false;

                    //inform the user that the image size is not valid
                    strErrorMessage += $"The image doesn't have the right size, the image height and width must be a multiple of {Main.intUNITSIZE}\n";
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
                ImgTexture = imgTexture_;
                BlnDoBounce = _chkbxDoBounce.Checked;

                for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                    if (Obstacle.List_obsObstacles[i].IntTypeID == IntID)
                        Obstacle.List_obsObstacles[i].ResetImage();

                string strResutlLog = CreateTextFromObject(this);

                Log.AddLog(ObjectType.ObstacleType, EditingType.ChangeProperties, strSaveLog, strResutlLog, imgSave, ImgTexture);

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