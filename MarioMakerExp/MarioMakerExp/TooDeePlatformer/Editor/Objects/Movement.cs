/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                         **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 14.06.2021                                                    **
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
** Contains a movement                                                       **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Contains a movement
    /// </summary>
    class Movement
    {
        //declare the static properties
        public const string strCLASSNAME = "Movement";                  //class name
        public const ObjectType objTYPE = ObjectType.Movement;          //object type
        public static int IntNextID { get; set; }                       //next ID to be used
        public static List<Movement> List_frmvMovements { get; set; }   //list of all the movements

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_frmvMovements.Count; i++)
                if (List_frmvMovements[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public static int GetIDFromIndex(int intIndex) => List_frmvMovements[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static Movement GetObjectByID(int intID) => List_frmvMovements[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
            string strText = Main.strVERSION + "\n";

            strText += strCLASSNAME + "\n";
            strText += IntNextID + "\n";
            strText += List_frmvMovements.Count + "\n";

            for (int i = 0; i < List_frmvMovements.Count; i++)
            {
                strText += CreateTextFromObject(List_frmvMovements[i]);

                if (List_frmvMovements.Count != i)
                    strText += "\n";
            }

            return strText;
        }

        /// <summary>
        /// Copy the data from the file to the movements
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrMovementTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_frmvMovements.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_frmvMovements.Add(CreateObjectFromText(tab_strLines[i + 4], false));

                }
                else
                    MessageBox.Show("The file isn't a storage for movements !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a line of text from an object
        /// </summary>
        /// <param name="mvObject"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(Movement mvObject)
        {
            string strText = "";

            strText += mvObject.IntID + $"{Main.chrSEPARATOR}";
            strText += mvObject.IntFolderID + $"{Main.chrSEPARATOR}";
            strText += mvObject.StrName + $"{Main.chrSEPARATOR}";
            strText += mvObject.DisDistance.dblWidth + $"{Main.chrSEPARATOR}";
            strText += mvObject.DisDistance.dblHeight + $"{Main.chrSEPARATOR}";
            strText += mvObject.IntTickGiven + $"{Main.chrSEPARATOR}";
            strText += mvObject.IntTickStop;

            return strText;
        }

        /// <summary>
        /// Create an object from a line of text
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static Movement CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            Movement mvNewMovement = new Movement()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntFolderID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                DisDistance = new Distance()
                {
                    dblWidth = Convert.ToInt32(tab_strObject[3]),
                    dblHeight = Convert.ToInt32(tab_strObject[4])
                },
                IntTickGiven = Convert.ToInt32(tab_strObject[5]),
                IntTickStop = Convert.ToInt32(tab_strObject[6])
            };

            return mvNewMovement;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static Movement()
        {
            IntNextID = 0;
            List_frmvMovements = new List<Movement>();
        }
        #endregion

        //declare the events
        public event dlgPersoEvent evtChangeValues;                     //triggered when the visual values changes

        //declare the properties
        public int IntID { get; set; }                                  //get the ID of the object
        public int IntTickStop { get; set; }                            //number of ticks while the object stops
        public Distance DisDistance { get; set; }                       //distance before the object stop
        public int IntTickGiven { get; set; }                           //number of tick given to do the move
        public int IntFolderID { get; set; }                            //contain the parent

        //declare all the variables
        public Distance _disCurrentLocation;                            //current position of the objects from the starting position
        public int _intTickLeft;                                        //number of ticks left before the move is ended
        public int _intTickSpent;                                       //number of ticks spent from the start of the move

        private string _strName;                                        //contain the name of the object
        private bool _blnIsMoveStarted;                                 //tell if the move has started

        #region Properties
        /// <summary>
        /// Get and set the name of the object
        /// </summary>
        public string StrName
        {
            get => _strName;

            set
            {
                if (!MvgrParent.IsMoveNameTaken(value, StrName))
                {
                    _strName = value;

                    evtChangeValues?.Invoke(IntID, this, GetType());
                }
            }
        }

        /// <summary>
        /// Get if the move is active or not and reset the current positon if the move is activated
        /// </summary>
        public bool BlnIsMoving
        {
            get
            {
                return _blnIsMoveStarted;
            }

            set
            {
                if (value)
                {
                    _disCurrentLocation.dblWidth = 0;
                    _disCurrentLocation.dblHeight = 0;
                }

                _blnIsMoveStarted = value;
            }
        }

        /// <summary>
        /// Get the distance per tick parcoured by the objects
        /// </summary>
        public Distance DisDistancePerTick
        {
            get
            {
                Distance disDistancePerTick_ = new Distance();

                disDistancePerTick_.dblWidth = DisDistance.dblWidth / IntTickGiven;
                disDistancePerTick_.dblHeight = DisDistance.dblHeight / IntTickGiven;

                return disDistancePerTick_;
            }
        }

        /// <summary>
        /// contain the parent
        /// </summary>
        public MoveGroup MvgrParent
        {
            get => Folder.List_flFolders[Folder.GetIndexFromID(IntFolderID)].MvgrMoves;
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => MvgrParent.FlParent.FlgrParent.DimParent.DimgrParent.WrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create a new move
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="intDelay"></param>
        /// <param name="intStopTime"></param>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        public Movement(string strName_, int intDelay, int intStopTime, double dblX, double dblY, int intFolderID_)
        {
            IntFolderID = intFolderID_;
            StrName = strName_;
            DisDistance = new Distance();
            IntID = IntNextID++;
            DisDistance.dblWidth = dblX;
            DisDistance.dblHeight = dblY;
            _disCurrentLocation = new Distance();
            IntTickGiven = intDelay / Main.intTICK;
            _intTickLeft = intDelay / Main.intTICK;
            _intTickSpent = 0;
            IntTickStop = intStopTime / Main.intTICK;

            evtChangeValues += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create a new movement from a file
        /// </summary>
        public Movement()
        {
            _disCurrentLocation = new Distance();
            evtChangeValues += new dlgPersoEvent(UpdateTreeView);
        }
        #endregion

        #region Moving methods
        /// <summary>
        /// Reset the move
        /// </summary>
        public void Reset()
        {
            _disCurrentLocation.dblWidth = 0;
            _disCurrentLocation.dblHeight = 0;
            _intTickSpent = 0;
            _intTickLeft = IntTickGiven;

            BlnIsMoving = false;
        }

        /// <summary>
        /// Add a tick to the move
        /// </summary>
        public void AddTick()
        {
            _disCurrentLocation.dblWidth += DisDistancePerTick.dblWidth;
            _disCurrentLocation.dblHeight += DisDistancePerTick.dblHeight;

            _intTickSpent++;
            _intTickLeft--;
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the move node
        /// </summary>
        public TreeNode GetTreeNode() => new TreeNode(StrName) { ContextMenuStrip = GetContextMenu(), Tag = IntID};
        #endregion

        #region Context menu
        /// <summary>
        /// Get the context menu of the move
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare the variables needed to create the context group
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                 //contain the contextmenu
            List<ToolStripMenuItem> list_tsmiItems = new List<ToolStripMenuItem>();     //contain the list of tools

            //tool used to remove the move
            list_tsmiItems.Add(new ToolStripMenuItem("Remove"));
            list_tsmiItems[0].Tag = IntID;
            list_tsmiItems[0].Click += new EventHandler(RemoveMoveMenu);

            //tool used to change the properties
            list_tsmiItems.Add(new ToolStripMenuItem("Properties"));
            list_tsmiItems[1].Tag = IntID;
            list_tsmiItems[1].Click += new EventHandler(ModifyMovePropertiesMenu);

            //add the tools to the context menu
            contextMenuStrip.Items.AddRange(list_tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Remove the move
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveMoveMenu(object sender, EventArgs e)
        {
            //search for the move
            for (int i = 0; i < List_frmvMovements.Count; i++)
                if (List_frmvMovements[i].IntID == IntID)
                {
                    string strSaveLog = Movement.CreateTextFromObject(Movement.List_frmvMovements[i]);

                    Log.AddLog(ObjectType.Movement, EditingType.Remove, strSaveLog, null);
                    List_frmvMovements.RemoveAt(i);


                    evtChangeValues?.Invoke(IntID, this, GetType());

                    break;
                }
        }

        /// <summary>
        /// Open the properties form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyMovePropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Properties Form
        //declare the controls
        private Form _frmMove;              //move form
        private TextBox _txtbxName;         //textbox containing the name and gives the ability to change it
        private TextBox _txtbxWidth;        //textbox containing the distance in X to travel
        private TextBox _txtbxHeight;       //textbox containing the distance in Y to travel
        private TextBox _txtbxDelay;        //textbox containing the time to execute the move and gives the ability to change it
        private TextBox _txtbxStop;         //textbox containing the delay after every moves and gives the ability to change it
        private Label _lblName;             //tells the user the textbox next to it is the naming one
        private Label _lblWidth;            //tells the user the textbox next to it is the X distance one
        private Label _lblHeight;           //tells the user the textbox next to it is the Y distance one
        private Label _lblDelay;            //tells the user the textbox next to it is the delay one
        private Label _lblStop;             //tells the user the textbox next to it is the stop delay onev
        private Button _btnCancel;          //close the form
        private Button _btnValidate;        //test the validity of the new values
        
        public void OpenPropertiesForm()
        {
            //reset the controls
            _frmMove = new Form();
            _txtbxName = new TextBox();
            _txtbxWidth = new TextBox();
            _txtbxHeight = new TextBox();
            _lblHeight = new Label();
            _lblWidth = new Label();
            _lblName = new Label();
            _btnCancel = new Button();
            _btnValidate = new Button();
            _lblDelay = new Label();
            _lblStop = new Label();
            _txtbxDelay = new TextBox();
            _txtbxStop = new TextBox();

            #region Controls
            //suspend the layout
            _frmMove.SuspendLayout();

            //textbox containing the name and give the ability to change it
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //textbox containing the distance in X and give the ability to change it
            _txtbxWidth.Location = new Point(202, 69);
            _txtbxWidth.Name = "txtbxX";
            _txtbxWidth.Size = new Size(36, 20);
            _txtbxWidth.TabIndex = 1;
            _txtbxWidth.Text = $"{DisDistance.dblWidth}";
            _txtbxWidth.TextAlign = HorizontalAlignment.Right;
            _txtbxWidth.Enabled = true;

            //textbox containing the distance in Y and give the ability to change it
            _txtbxHeight.Location = new Point(202, 95);
            _txtbxHeight.Name = "txtbxY";
            _txtbxHeight.Size = new Size(36, 20);
            _txtbxHeight.TabIndex = 2;
            _txtbxHeight.Text = $"{DisDistance.dblHeight}";
            _txtbxHeight.TextAlign = HorizontalAlignment.Right;
            _txtbxHeight.Enabled = true;

            //textbox containing the time used to do a move and give the ability to change it txtbxDelay
            _txtbxDelay.Location = new Point(202, 121);
            _txtbxDelay.Name = "txtbxDelay";
            _txtbxDelay.Size = new Size(36, 20);
            _txtbxDelay.TabIndex = 2;
            _txtbxDelay.Text = $"{IntTickGiven * Main.intTICK}";
            _txtbxDelay.TextAlign = HorizontalAlignment.Right;
            _txtbxDelay.Enabled = true;

            //textbox containing the time to wait after each move and give the ability to change it
            _txtbxStop.Location = new Point(202, 147);
            _txtbxStop.Name = "txtbxStop";
            _txtbxStop.Size = new Size(36, 20);
            _txtbxStop.TabIndex = 2;
            _txtbxStop.Text = $"{IntTickStop * Main.intTICK}";
            _txtbxStop.TextAlign = HorizontalAlignment.Right;
            _txtbxStop.Enabled = true;

            //label telling the user the textbox next to it is the naming one
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "Name :";

            //label telling the user the textbox next to it is the distance in X one
            _lblWidth.AutoSize = true;
            _lblWidth.Location = new Point(36, 69);
            _lblWidth.Name = "lblX";
            _lblWidth.Size = new Size(41, 13);
            _lblWidth.TabIndex = 5;
            _lblWidth.Text = "Distance in X :";

            //label telling the user the textbox next to it is the Y distance one
            _lblHeight.AutoSize = true;
            _lblHeight.Location = new Point(36, 95);
            _lblHeight.Name = "lblY";
            _lblHeight.Size = new Size(44, 13);
            _lblHeight.TabIndex = 4;
            _lblHeight.Text = "Distance in Y :";

            //label telling the user the textbox next to it is the delay one
            _lblDelay.AutoSize = true;
            _lblDelay.Location = new Point(36, 121);
            _lblDelay.Name = "lblDelay";
            _lblDelay.Size = new Size(44, 13);
            _lblDelay.TabIndex = 4;
            _lblDelay.Text = "Time to do the move (m/s) :";

            //label telling the user the textbox next to it is the stop delay one
            _lblStop.AutoSize = true;
            _lblStop.Location = new Point(36, 147);
            _lblStop.Name = "lblStop";
            _lblStop.Size = new Size(44, 13);
            _lblStop.TabIndex = 4;
            _lblStop.Text = "Delay between moves (m/s) :";

            //close the form
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 173);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(Cancel_Click);

            //validate the new values
            _btnValidate.Location = new Point(121, 173);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Validate";
            _btnValidate.Tag = IntID;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //form
            _frmMove.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmMove.AutoScaleMode = AutoScaleMode.Font;
            _frmMove.AcceptButton = _btnValidate;
            _frmMove.CancelButton = _btnCancel;
            _frmMove.ClientSize = new Size(304, 224);
            _frmMove.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmMove.Name = "frmMove";
            _frmMove.StartPosition = FormStartPosition.CenterParent;
            _frmMove.Text = "Move";
            _frmMove.Tag = IntID;

            //add the controls
            _frmMove.Controls.Add(_btnValidate);
            _frmMove.Controls.Add(_btnCancel);
            _frmMove.Controls.Add(_lblName);
            _frmMove.Controls.Add(_lblWidth);
            _frmMove.Controls.Add(_lblHeight);
            _frmMove.Controls.Add(_txtbxHeight);
            _frmMove.Controls.Add(_txtbxWidth);
            _frmMove.Controls.Add(_txtbxName);
            _frmMove.Controls.Add(_txtbxDelay);
            _frmMove.Controls.Add(_txtbxStop);
            _frmMove.Controls.Add(_lblDelay);
            _frmMove.Controls.Add(_lblStop);

            //resume the layout
            _frmMove.ResumeLayout(false);
            _frmMove.PerformLayout();
            #endregion

            //show the form
            _frmMove.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmMove.Close();

        /// <summary>
        /// Validate the new values, but test their validity first
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate_Click(object sender, EventArgs e)
        {
            //declare the variables neeeded to test the validity
            string strErrorMessage = null;              //contain the error message
            bool blnAreAllValuesValid = true;           //contain the validity of the new values

            string strName_ = _txtbxName.Text;           //contain the new name
            int intDistanceX_ = 0;                      //contain the new X distance
            int intDistanceY_ = 0;                      //contain the new Y distance
            int intDelay_ = 0;                          //contain the time to do a move
            int intStop_ = 0;                           //contain the delay after every move

            #region Verify Validity
            //test if the name is taken
            if (MvgrParent.IsMoveNameTaken($"{strName_}", StrName))
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
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

            //test if the new distance in X is invalid
            if (!int.TryParse(_txtbxWidth.Text, out intDistanceX_))
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
                strErrorMessage += $"The new X distance is not an integrer !\n";
            }

            //test if the new distance in Y is invalid
            if (!int.TryParse(_txtbxHeight.Text, out intDistanceY_))
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
                strErrorMessage += $"The new Y distance is not an integrer !\n";
            }

            //test if the delay is invalid
            if (!int.TryParse(_txtbxDelay.Text, out intDelay_))
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
                strErrorMessage += $"The new delay is not an integrer !\n";
            }
            //test if the delay is not too small
            else if (intDelay_ < Main.intTICK)
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
                strErrorMessage += $"The new delay is too low !\n It must be at least {Main.intTICK} m/s !\n";
            }

            //test if the value is invalid
            if (!int.TryParse(_txtbxStop.Text, out intStop_))
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
                strErrorMessage += $"The new stop delay is not an integrer !\n";
            }
            //test if the stop delay is not too small
            else if (intStop_ < 0)
            {
                //tell the game the values are invalid
                blnAreAllValuesValid = false;

                //tell the player the value is invalid
                strErrorMessage += $"The new stop delay cannot be lower than 0 !\n";
            }
            #endregion

            //test the validity
            if (blnAreAllValuesValid)
            {
                string strSaveLog = CreateTextFromObject(this);

                //change the new values
                StrName = strName_;
                IntTickGiven = intDelay_ / Main.intTICK;
                IntTickStop = intStop_ / Main.intTICK;
                DisDistance.dblWidth = intDistanceX_;
                DisDistance.dblHeight = intDistanceY_;

                string strResultLog = CreateTextFromObject(this);

                Log.AddLog(ObjectType.Movement, EditingType.ChangeProperties, strSaveLog, strResultLog);

                //close the form
                _frmMove.Close();

                //dispose the move
                _frmMove.Dispose();
            }
            else
                //show the message box
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