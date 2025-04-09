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
** This class contain all the worlds and create the default world.           **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace TooDeePlatformer.Editor
{
    //declare the delegates
    public delegate void dlgPersoEvent(int intSenderID, object objSender, Type typSender);          //Used to inform the parent of a change of visual values

    /// <summary>
    /// This is the main class containing the worlds
    /// </summary>
    class Main
    {
        //declare the constants
        public const string strVERSION = "V0.0.1";          //current version
        public const char chrSEPARATOR = ',';               //separator
        public const int intMAXNAMESIZE = 20;               //max size of a name
        public const int intMINNAMESIZE = 2;                //min size of a name
        public const int intMAXSIZE = 128;                  //max size of a dimension
        public const int intMINSIZE = 4;                    //minimum size of a dimension
        public const int intTICK = 100;                     //how long is a tick in miliseconds
        public const int intUNITSIZE = 64;                  //size of one unit in pixels

        //declare properties
        public static frmEditor FrmEditor { get; set; }     //contain the parent

        //declare private fields
        private static string _strWorldName;                //contain the name of the current world

        #region Properties
        /// <summary>
        /// Get the current world
        /// </summary>
        public static World WrldWorld
        {
            get
            {
                for (int i = 0; i < World.List_wrldWorlds.Count; i++)
                    if (World.List_wrldWorlds[i].StrName == _strWorldName)
                        return World.List_wrldWorlds[i];

                return null;
            }
        }

        /// <summary>
        /// list of the world folders
        /// </summary>
        public static List<string> List_strWorlds
        {
            get => new List<string>(Directory.GetDirectories(StrWorldFolderPath));
        }

        /// <summary>
        /// Path of the folder of the app
        /// </summary>
        public static string StrFolderPath
        {
            get => Directory.GetParent(Application.StartupPath).FullName;
        }

        /// <summary>
        /// Path of the default resources
        /// </summary>
        public static string StrResourcesPath
        {
            get => StrFolderPath + @"\Resources";
        }

        /// <summary>
        /// Path of the folder of worlds
        /// </summary>
        public static string StrWorldFolderPath
        {
            get => StrFolderPath + @"\Worlds";
        }

        /// <summary>
        /// Path of the default images
        /// </summary>
        public static string StrImagesPath
        {
            get => StrResourcesPath + @"\Images";
        }

        /// <summary>
        /// Path of the current world folder
        /// </summary>
        public static string StrCurrentWorldPath
        {
            get => StrWorldFolderPath + @"\" + _strWorldName;
        }

        /// <summary>
        /// Path of the images of the current world
        /// </summary>
        public static string StrWorldObjectsPath
        {
            get => StrCurrentWorldPath + @"\Objects";
        }

        /// <summary>
        /// Path of the images of the current world
        /// </summary>
        public static string StrWorldImagesPath
        {
            get => StrCurrentWorldPath + @"\Images";
        }

        /// <summary>
        /// Path of the background images
        /// </summary>
        public static string StrBGImagesPath
        {
            get => StrWorldImagesPath + @"\Backgrounds";
        }

        /// <summary>
        /// Path of the obstacle type images
        /// </summary>
        public static string StrOTImagesPath
        {
            get => StrWorldImagesPath + @"\ObstacleTypes";
        }

        /// <summary>
        /// Path of the  special type images
        /// </summary>
        public static string StrSTImagesPath
        {
            get => StrWorldImagesPath + @"\SpecialTypes";
        }

        /// <summary>
        /// Path of the obstacle storage
        /// </summary>
        public static string StrObstacleTXT
        {
            get => StrWorldObjectsPath + @"\Obstacle.txt";
        }

        /// <summary>
        /// Path of the world storage
        /// </summary>
        public static string StrWorldTXT
        {
            get => StrWorldObjectsPath + @"\World.txt";
        }

        /// <summary>
        /// Path of the dimension storage
        /// </summary>
        public static string StrDimensionTXT
        {
            get => StrWorldObjectsPath + @"\Dimension.txt";
        }

        /// <summary>
        /// Path of the folder storage
        /// </summary>
        public static string StrFolderTXT
        {
            get => StrWorldObjectsPath + @"\Folder.txt";
        }

        /// <summary>
        /// Path of the movement storage
        /// </summary>
        public static string StrMovementTXT
        {
            get => StrWorldObjectsPath + @"\Movement.txt";
        }

        /// <summary>
        /// Path of the special objects storage
        /// </summary>
        public static string StrSpecialTXT
        {
            get => StrWorldObjectsPath + @"\Special.txt";
        }

        /// <summary>
        /// Path of the special object types storage
        /// </summary>
        public static string StrSpecialTypeTXT
        {
            get => StrWorldObjectsPath + @"\SpecialType.txt";
        }

        /// <summary>
        /// Path of the obstacle types storage
        /// </summary>
        public static string StrObstacleTypeTXT
        {
            get => StrWorldObjectsPath + @"\ObstacleType.txt";
        }
        #endregion

        #region Launch method
        /// <summary>
        /// Add the default world
        /// </summary>
        public static void Launch(frmEditor frmEditor)
        {
            FrmEditor = frmEditor;
            FrmEditor.FormClosing += new FormClosingEventHandler(CloseEvent);

            OpenStartForm();
        }
        #endregion

        #region Start Form
        //declare the controls
        private static Form _frmWorld;           //obstacle form
        private static TextBox _txtbxName;       //textbox containing the name and gives the ability to change it
        private static ComboBox _cbbxWorlds;     //combobox containing the type of the part and gives the ability to change it
        private static Label _lblName;           //tells the user the textbox next to it is the naming one
        private static Label _lblWorld;          //tells the user the combobox next to it is the type one
        private static Button _btnNewWorld;      //close the form
        private static Button _btnValidate;      //test the validity of the new values

        private static void OpenStartForm()
        {
            //reset the controls
            _frmWorld = new Form();
            _txtbxName = new TextBox();
            _lblName = new Label();
            _btnNewWorld = new Button();
            _cbbxWorlds = new ComboBox();
            _lblWorld = new Label();
            _btnValidate = new Button();

            #region Controls
            //suspend the layout
            _frmWorld.SuspendLayout();

            //textbox containing the name and give the ability to change it
            _txtbxName.Location = new Point(110, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //combobox containing the worlds and give the ability to change it
            _cbbxWorlds.Location = new Point(110, 95);
            _cbbxWorlds.Name = "cbbxWorlds";
            _cbbxWorlds.Size = new Size(138, 20);
            _cbbxWorlds.TabIndex = 3;

            if (List_strWorlds.Count > 0)
            {
                List<string> list_strWorldNames = new List<string>();

                for (int i = 0; i < Main.List_strWorlds.Count; i++)
                    list_strWorldNames.Add(Path.GetFileNameWithoutExtension(Main.List_strWorlds[i]));

                _cbbxWorlds.Items.AddRange(list_strWorldNames.ToArray());

                _cbbxWorlds.SelectedIndex = 0;
            }
            else
                _cbbxWorlds.Enabled = false;

            //label telling the textbox next to it is the naming one
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "New World :";

            //label telling the the combobox next to it is the type one
            _lblWorld.AutoSize = true;
            _lblWorld.Location = new Point(36, 95);
            _lblWorld.Name = "lblWorld";
            _lblWorld.Size = new Size(44, 13);
            _lblWorld.TabIndex = 5;
            _lblWorld.Text = "World :";

            //create a new world the form
            _btnNewWorld.DialogResult = DialogResult.Cancel;
            _btnNewWorld.Location = new Point(260, 43);
            _btnNewWorld.Name = "btnCancel";
            _btnNewWorld.Size = new Size(75, 23);
            _btnNewWorld.TabIndex = 8;
            _btnNewWorld.Text = "Create";
            _btnNewWorld.UseVisualStyleBackColor = true;
            _btnNewWorld.Click += new EventHandler(NewWorld_Click);

            //validate the new values
            _btnValidate.Location = new Point(260, 95);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Open";
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            if (List_strWorlds.Count > 0)
                _btnValidate.Enabled = true;
            else
                _btnValidate.Enabled = false;

            //form
            _frmWorld.FormClosed += new FormClosedEventHandler(Close);
            _frmWorld.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmWorld.AutoScaleMode = AutoScaleMode.Font;

            if (!_btnValidate.Enabled)
                _frmWorld.AcceptButton = _btnNewWorld;
            else
                _frmWorld.AcceptButton = _btnValidate;

            _frmWorld.ClientSize = new Size(380, 150);
            _frmWorld.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmWorld.Name = "frmWorld";
            _frmWorld.StartPosition = FormStartPosition.CenterParent;
            _frmWorld.Text = "Choosing World";

            //add the controls
            _frmWorld.Controls.Add(_btnNewWorld);
            _frmWorld.Controls.Add(_btnValidate);
            _frmWorld.Controls.Add(_lblName);
            _frmWorld.Controls.Add(_lblWorld);
            _frmWorld.Controls.Add(_cbbxWorlds);
            _frmWorld.Controls.Add(_txtbxName);

            //resume the layout
            _frmWorld.ResumeLayout(false);
            _frmWorld.PerformLayout();
            #endregion

            //show the form
            _frmWorld.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Close(object sender, EventArgs e) => Application.Exit();

        /// <summary>
        /// Create a new world
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void NewWorld_Click(object sender, EventArgs e)
        {
            //declare all the variables needed to test the validity
            string strErrorMessage = "";            //contain the error message
            bool blnAreAllValuesValid = true;       //contain the validity of the value

            string strNewName_ = _txtbxName.Text;    //new world name

            #region Verify Validity
            //test if the name is equal to null
            if (strNewName_ == null)
            {
                //tell the game that the values are invalid
                blnAreAllValuesValid = false;

                //tell the user the value is invalid
                strErrorMessage += $"The new name cannot be null !\n";
            }

            //test if the name is equal to null
            if (strNewName_.Length > Main.intMAXNAMESIZE || strNewName_.Length < Main.intMINNAMESIZE)
            {
                //tell the game that the values are invalid
                blnAreAllValuesValid = false;

                //tell the user the value is invalid
                strErrorMessage += $"The name's number of characters must be minimum {Main.intMINNAMESIZE} and maximum {Main.intMAXNAMESIZE} !\n";
            }

            //test if the name is already taken
            if (IsWorldNameTaken(strNewName_, null))
            {
                //tell the game that the values are invalid
                blnAreAllValuesValid = false;

                //tell the user the value is invalid
                strErrorMessage += $"The new name is already taken !\n";
            }

            for (int i = 0; i < Path.GetInvalidPathChars().Length; i++)
                if (strNewName_.Contains($"{Path.GetInvalidPathChars()[i]}"))
                {
                    //tell the game that the values are invalid
                    blnAreAllValuesValid = false;

                    //tell the user the value is invalid
                    strErrorMessage += $"The new name is not compatible for a file name !\n";

                    break;
                }
            #endregion

            if (blnAreAllValuesValid)
            {
                _strWorldName = strNewName_;
                World.List_wrldWorlds.Add(new World(strNewName_));
                FrmEditor.Controls.Add(WrldWorld.PnlWorld);

                Log.RemoveAll();

                _frmWorld.FormClosed -= new FormClosedEventHandler(Close);
                _frmWorld.Close();
            }
            else
                MessageBox.Show(strErrorMessage, "One or multiple values are invalid !", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Validate the chosen world
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Validate_Click(object sender, EventArgs e)
        {
            try
            {
                _strWorldName = Path.GetFileNameWithoutExtension(List_strWorlds[_cbbxWorlds.SelectedIndex]);

                World.CopyFromFile();

                FrmEditor.Controls.Add(WrldWorld.PnlWorld);

                Dimension.CopyFromFile();
                SpecialType.CopyFromFile();
                ObstacleType.CopyFromFile();
                Folder.CopyFromFile();
                Obstacle.CopyFromFile();
                Special.CopyFromFile();
                Movement.CopyFromFile();

                WrldWorld.UpdateTreeNodes();
                WrldWorld.IntDefaultPlayerDimensionID = WrldWorld.IntDefaultPlayerDimensionID;
                WrldWorld.InitializeMainMenu();

                Log.RemoveAll();

                _frmWorld.FormClosed -= new FormClosedEventHandler(Close);
                _frmWorld.Close();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("World Not Found", "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        #endregion

        #region Test Name
        /// <summary>
        /// Test if the new name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strCurrentName"></param>
        /// <returns></returns>
        public static bool IsWorldNameTaken(string strNewName, string strCurrentName)
        {
            //search in the moving folders
            for (int i = 0; i < List_strWorlds.Count; i++)
                //test if the name is taken exepting the current name
                if (Path.GetFileNameWithoutExtension(List_strWorlds[i]) == strNewName && strCurrentName != strNewName)
                    return true;

            return false;
        }
        #endregion

        #region World File
        /// <summary>
        /// Save the world to it's folder
        /// </summary>
        public static void SaveWorld()
        {
            if (!WrldWorld.BlnStructCreated)
            {
                Directory.CreateDirectory(StrCurrentWorldPath);
                Directory.CreateDirectory(StrWorldObjectsPath);
                Directory.CreateDirectory(StrWorldImagesPath);
                Directory.CreateDirectory(StrOTImagesPath);
                Directory.CreateDirectory(StrSTImagesPath);
                Directory.CreateDirectory(StrBGImagesPath);

                WrldWorld.BlnStructCreated = true;
            }

            File.WriteAllText(StrDimensionTXT, Dimension.GetDataText());
            File.WriteAllText(StrFolderTXT, Folder.GetDataText());
            File.WriteAllText(StrMovementTXT, Movement.GetDataText());
            File.WriteAllText(StrObstacleTXT, Obstacle.GetDataText());
            File.WriteAllText(StrObstacleTypeTXT, ObstacleType.GetDataText());
            File.WriteAllText(StrSpecialTXT, Special.GetDataText());
            File.WriteAllText(StrSpecialTypeTXT, SpecialType.GetDataText());
            File.WriteAllText(StrWorldTXT, World.GetDataText());

            List<string> list_strImgFiles = new List<string>();
            list_strImgFiles.AddRange(Directory.GetFiles(StrBGImagesPath));
            list_strImgFiles.AddRange(Directory.GetFiles(StrOTImagesPath));
            list_strImgFiles.AddRange(Directory.GetFiles(StrSTImagesPath));

            for (int i = 0; i < list_strImgFiles.Count; i++)
                File.Delete(list_strImgFiles[i]);

            for (int i = 0; i < ObstacleType.List_obstypTypes.Count; i++)
                ObstacleType.List_obstypTypes[i].SaveTexture();

            for (int i = 0; i < SpecialType.List_spctypTypes.Count; i++)
                SpecialType.List_spctypTypes[i].SaveTexture();

            for (int i = 0; i < Dimension.List_dimDimensions.Count; i++)
                Dimension.List_dimDimensions[i].SaveBackground();

            WrldWorld.BlnNeedSaving = false;
            MessageBox.Show($"The world has been saved to {StrCurrentWorldPath}", "World Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Delete the files of the current world
        /// </summary>
        public static void DeleteWorldFiles()
        {
            if (WrldWorld.BlnStructCreated)
            {
                File.Delete(StrDimensionTXT);
                File.Delete(StrFolderTXT);
                File.Delete(StrMovementTXT);
                File.Delete(StrObstacleTXT);
                File.Delete(StrObstacleTypeTXT);
                File.Delete(StrSpecialTXT);
                File.Delete(StrSpecialTypeTXT);
                File.Delete(StrWorldTXT);

                List<string> list_strImgFiles = new List<string>();
                list_strImgFiles.AddRange(Directory.GetFiles(StrBGImagesPath));
                list_strImgFiles.AddRange(Directory.GetFiles(StrOTImagesPath));
                list_strImgFiles.AddRange(Directory.GetFiles(StrSTImagesPath));

                for (int i = 0; i < list_strImgFiles.Count; i++)
                    File.Delete(list_strImgFiles[i]);

                Directory.Delete(StrBGImagesPath);
                Directory.Delete(StrSTImagesPath);
                Directory.Delete(StrOTImagesPath);
                Directory.Delete(StrWorldImagesPath);
                Directory.Delete(StrWorldObjectsPath);
                Directory.Delete(StrCurrentWorldPath);
            }
        }
        #endregion

        #region Close
        /// <summary>
        /// Event activated when closing the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        public static void CloseEvent(object sender, FormClosingEventArgs eventArgs)
        {
            var result = MessageBox.Show("Will the world be saved ?", "Exit?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                SaveWorld();
                for (int i = 0; i < WrldWorld.DimgrDimensions.List_dimDimensions.Count; i++)
                    for (int ii = 0; ii < WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders.Count; ii++)
                    {
                        if (WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPartMoves != null)
                            WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPartMoves.Abort();
                        if (WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPicBxMoves != null)
                            WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPicBxMoves.Abort();
                    }
            }
            else if (result == DialogResult.No)
            {
                for (int i = 0; i < WrldWorld.DimgrDimensions.List_dimDimensions.Count; i++)
                    for (int ii = 0; ii < WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders.Count; ii++)
                    {
                        if (WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPartMoves != null)
                            WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPartMoves.Abort();
                        if (WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPicBxMoves != null)
                            WrldWorld.DimgrDimensions.List_dimDimensions[i].FlgrFolderGroup.List_flFolders[ii].ThrPicBxMoves.Abort();
                    }
            }
            else if (result == DialogResult.Cancel)
                eventArgs.Cancel = true;
        }
        #endregion
    }
}