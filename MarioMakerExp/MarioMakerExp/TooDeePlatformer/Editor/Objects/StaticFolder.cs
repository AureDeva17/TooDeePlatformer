/******************************************************************************
** PROGRAMME MarioMaker Experimental Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 11.02.2021                                                    **
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
** This class is a group of group of objects                                 **
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
using System.Windows.Forms;
using System.Drawing;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Folder of objects
    /// </summary>
    class StaticFolder
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                //triggered when visual values are changed
        public event dlgPersoEvent evtRemove;                       //triggered when trying to remove the object

        //declare the properties
        public int intID { get; set; }                              //contain the ID of the object
        public ObstacleGroup obsgrObstacles { get; set; }           //contain the group of obstacles
        public dlgTryNewNameEvent dlgTryNewName { get; set; }       //triggered when trying a new name

        //declare the fields
        private string strName_;                                    //contain the name of the folder

        #region Properties
        /// <summary>
        /// Get and set the name of the object
        /// </summary>
        public string strName
        {
            get => strName_;

            set
            {
                strName_ = value;
                evtValuesChanged?.Invoke(intID, this, GetType());
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values
        /// </summary>
        /// <param name="strName_"></param>
        public StaticFolder(string strName_)
        {
            //tell the game to use another ID next time
            intID = GeneralData.intNextID++;

            //set the default values
            strName = strName_;
            obsgrObstacles = new ObstacleGroup();
            obsgrObstacles.evtValuesChanged += new dlgPersoEvent(ValuesChanged);

            //set the events
            obsgrObstacles.evtValuesChanged += new dlgPersoEvent(ValuesChanged);
        }

        /// <summary>
        /// Create a null static folder
        /// </summary>
        public StaticFolder()
        {

        }
        #endregion

        #region Adding and Removing
        /// <summary>
        /// Add a new obstacle
        /// </summary>
        /// <param name="posNewPosition"></param>
        /// <param name="intDimensionHeight"></param>
        public void AddObstacle(int intType, Position posNewPosition, int intDimensionHeight) => obsgrObstacles.AddObstacle(intType, posNewPosition, intDimensionHeight);

        /// <summary>
        /// Remove all objects in the folder
        /// </summary>
        public void RemoveAll() => obsgrObstacles.RemoveAll();

        /// <summary>
        /// Remove every selected object
        /// </summary>
        public void RemoveSelected() => obsgrObstacles.RemoveSelected();
        #endregion

        #region Test of Emptiness
        /// <summary>
        /// Test if the position is empty
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested)
        {
            if (!obsgrObstacles.TestEmptiness(posTested))
                return false;

            return true;
        }

        /// <summary>
        /// Test if the position is empty with an exeption
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested, int intExeption)
        {
            if (!obsgrObstacles.TestEmptiness(posTested, intExeption))
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
            if (!obsgrObstacles.TestEmptiness(posTested, blnSelection))
                return false;

            return true;
        }
        #endregion

        #region Select
        /// <summary>
        /// Select all
        /// </summary>
        public void SelectAll()
        {
            obsgrObstacles.SelectAll();
        }

        /// <summary>
        /// Unselect all
        /// </summary>
        public void UnselectAll()
        {
            obsgrObstacles.UnselectAll();
        }

        /// <summary>
        /// Select all the obtacles present inside the selection area
        /// </summary>
        public void SelectArea(Position posArea, bool blnSelect)
        {
            obsgrObstacles.SelectArea(posArea, blnSelect);
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node containg the node of the group
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the new node
            List<TreeNode> list_trnStaticFolder = new List<TreeNode>();     //contain the main nodes

            list_trnStaticFolder.Add(obsgrObstacles.GetTreeNode());

            //return the new node
            return new TreeNode(strName, list_trnStaticFolder.ToArray()) { ContextMenuStrip = GetContextMenu(), Tag = intID };
        }
        #endregion

        #region Context menu
        /// <summary>
        /// Get the context menu of the object group
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //create the context box
            //declare the variables made to create the context box
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();             //contain the context box
            List<ToolStripMenuItem> tsmiItems = new List<ToolStripMenuItem>();      //contain all the tools

            //tool made to select the object
            tsmiItems.Add(new ToolStripMenuItem("Select All"));
            tsmiItems[0].Tag = intID;
            tsmiItems[0].Click += new EventHandler(SelectAllMenu);

            //tool made to unselect the object
            tsmiItems.Add(new ToolStripMenuItem("Unselect All"));
            tsmiItems[1].Tag = intID;
            tsmiItems[1].Click += new EventHandler(UnselectAllMenu);

            //tool made to remove the object
            tsmiItems.Add(new ToolStripMenuItem("Remove"));
            tsmiItems[2].Tag = intID;
            tsmiItems[2].Click += new EventHandler(RemoveStaticFolderMenu);

            //tool made to see and change the properties of the object
            tsmiItems.Add(new ToolStripMenuItem("Properties"));
            tsmiItems[3].Tag = intID;
            tsmiItems[3].Click += new EventHandler(ModifyPropertiesMenu);

            //add the tools to the context box
            contextMenuStrip.Items.AddRange(tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Remove the static folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveStaticFolderMenu(object sender, EventArgs e) => evtRemove?.Invoke(intID, this, GetType());

        /// <summary>
        /// Select all the objects inside the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllMenu(object sender, EventArgs e) => SelectAll();

        /// <summary>
        /// Unselect all the objects inside the static folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectAllMenu(object sender, EventArgs e) => UnselectAll();

        /// <summary>
        /// Open the properties form of the object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyPropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Perso Events
        /// <summary>
        /// Invoke the value changed event when values are changed
        /// </summary>
        /// <param name="intID_"></param>
        /// <param name="objSender"></param>
        /// <param name="typSender"></param>
        private void ValuesChanged(int intSenderID, object objSender, Type typSender) => evtValuesChanged?.Invoke(intSenderID, objSender, typSender);
        #endregion

        #region Properties Form
        //declare all the control variables
        private Form frmStaticFolder;                   //properties form
        private TextBox txtbxName;                      //name textbox
        private TextBox txtbxNbObstacles;               //number of obstacles textbox
        private Label lblNbObstacles;                   //number of obstacles label
        private Label lblName;                          //name label
        private Button btnCancel;                       //cancel button
        private Button btnValidate;                     //validation button

        /// <summary>
        /// Open the properties form of the selected object group
        /// </summary>
        /// <param name="intIndex"></param>
        public void OpenPropertiesForm()
        {
            //reset the controls
            frmStaticFolder = new Form();
            txtbxName = new TextBox();
            txtbxNbObstacles = new TextBox();
            lblNbObstacles = new Label();
            lblName = new Label();
            btnCancel = new Button();
            btnValidate = new Button();

            #region Controls
            //suspend the layout
            frmStaticFolder.SuspendLayout();

            //contain the name of the object group
            txtbxName.Location = new Point(100, 43);
            txtbxName.MaxLength = 20;
            txtbxName.Name = "txtbxName";
            txtbxName.Size = new Size(138, 20);
            txtbxName.TabIndex = 0;
            txtbxName.Text = $"{strName}";
            txtbxName.TextAlign = HorizontalAlignment.Right;

            //contain the number of obstacles
            txtbxNbObstacles.Location = new Point(202, 69);
            txtbxNbObstacles.Name = "txtbxNbObstacles";
            txtbxNbObstacles.Size = new Size(36, 20);
            txtbxNbObstacles.TabIndex = 2;
            txtbxNbObstacles.Text = $"{obsgrObstacles.intCount}";
            txtbxNbObstacles.TextAlign = HorizontalAlignment.Right;
            txtbxNbObstacles.Enabled = false;

            //inform the user the textbox next to it contain the number of obstacles
            lblNbObstacles.AutoSize = true;
            lblNbObstacles.Location = new Point(36, 69);
            lblNbObstacles.Name = "lblNbObstacles";
            lblNbObstacles.Size = new Size(44, 13);
            lblNbObstacles.TabIndex = 4;
            lblNbObstacles.Text = "Number of Obstacles :";

            //inform the user the textbox next to it contain the name
            lblName.AutoSize = true;
            lblName.Location = new Point(36, 43);
            lblName.Name = "lblName";
            lblName.Size = new Size(41, 13);
            lblName.TabIndex = 6;
            lblName.Text = "Name :";

            //cancel the changing of properties
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(202, 95);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += new EventHandler(Cancel_Click);

            //validate the changing of properties
            btnValidate.Location = new Point(126, 95);
            btnValidate.Name = "btnValidate";
            btnValidate.Size = new Size(75, 23);
            btnValidate.TabIndex = 9;
            btnValidate.Text = "Validate";
            btnValidate.Tag = intID;
            btnValidate.UseVisualStyleBackColor = true;
            btnValidate.Click += new EventHandler(Validate_Click);

            //form containing the controls
            frmStaticFolder.AutoScaleDimensions = new SizeF(6F, 13F);
            frmStaticFolder.AutoScaleMode = AutoScaleMode.Font;
            frmStaticFolder.CancelButton = btnCancel;
            frmStaticFolder.ClientSize = new Size(304, 125);
            frmStaticFolder.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            frmStaticFolder.Name = "frmStaticFolder";
            frmStaticFolder.StartPosition = FormStartPosition.CenterParent;
            frmStaticFolder.Text = "Static Folder";
            frmStaticFolder.Tag = intID;

            //add the controls
            frmStaticFolder.Controls.Add(btnValidate);
            frmStaticFolder.Controls.Add(btnCancel);
            frmStaticFolder.Controls.Add(lblName);
            frmStaticFolder.Controls.Add(lblNbObstacles);
            frmStaticFolder.Controls.Add(txtbxNbObstacles);
            frmStaticFolder.Controls.Add(txtbxName);

            //resume layout
            frmStaticFolder.ResumeLayout(false);
            frmStaticFolder.PerformLayout();
            #endregion

            //show the form
            frmStaticFolder.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => frmStaticFolder.Close();

        /// <summary>
        /// Validate the changing of data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate_Click(object sender, EventArgs e)
        {
            //declare the variables
            string strErrorMessage = "";            //contain the error message
            bool blnAreAllValuesValid = true;       //contain the information about the validity of all the informations

            string strName_ = txtbxName.Text;       //get the new name

            #region Verify Validity
            //test if the new name is taken
            if (dlgTryNewName != null)
                if (dlgTryNewName.Invoke($"{strName_}", strName))
                {
                    //inform the game the value is invalid
                    blnAreAllValuesValid = false;

                    //inform the user the value is invalid
                    strErrorMessage += $"The new name is already taken !\n";
                }

            //test if the new name is null
            if (strName_ == null)
            {
                //inform the game the value is invalid
                blnAreAllValuesValid = false;

                //inform the user the value is invalid
                strErrorMessage += $"The new name cannot be equal to null !\n";
            }
            #endregion

            //test if all values are valid
            if (blnAreAllValuesValid)
            {
                //set the new name
                strName = strName_;

                //close the form
                frmStaticFolder.Close();

                //dispose the form of all data
                frmStaticFolder.Dispose();
            }
            else
                //show the error message
                MessageBox.Show(strErrorMessage, "One or multiple values are invalid !");
        }
        #endregion
        #endregion
    }
}