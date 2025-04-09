/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                         **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 10.02.2021                                                    **
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
** This class is a group of moves, it contains and manages moves.            **
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
    /// This class is a group of moves, it contains and manges moves
    /// </summary>
    class MoveGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                 //triggered when the visual values of the group are changed

        //declare all the variables
        private string _strName;                                    //contain the name of the group
        private readonly Folder _flParent;                          //contain the parent

        #region Properties
        /// <summary>
        /// Contain all the movements of the group
        /// </summary>
        public List<Movement> List_frmvMoves
        {
            get
            {
                List<Movement> list_objects_ = new List<Movement>();

                for (int i = 0; i < Movement.List_frmvMovements.Count; i++)
                    if (Movement.List_frmvMovements[i].IntFolderID == FlParent.IntID)
                        list_objects_.Add(Movement.List_frmvMovements[i]);

                return list_objects_;
            }
        }

        /// <summary>
        /// Indexer of the list of movements
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public Movement this[int intID]
        {
            get
            {
                for (int i = 0; i < List_frmvMoves.Count; i++)
                    if (List_frmvMoves[i].IntID == intID)
                        return List_frmvMoves[i];

                return null;
            }

            set
            {
                for (int i = 0; i < List_frmvMoves.Count; i++)
                    if (List_frmvMoves[i].IntID == intID)
                        List_frmvMoves[i] = value;
            }
        }

        /// <summary>
        /// Contain the name of the group
        /// </summary>
        public string StrName
        {
            get => _strName;

            set
            {
                _strName = value;
                evtValuesChanged?.Invoke(-1, this, GetType());
            }
        }

        /// <summary>
        /// Get the number of moves
        /// </summary>
        public int IntCount => List_frmvMoves.Count;

        /// <summary>
        /// Contain the parent
        /// </summary>
        public Folder FlParent
        {
            get => _flParent;
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => FlParent.FlgrParent.DimParent.DimgrParent.WrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values of the group
        /// </summary>
        /// <param name="strName_"></param>
        public MoveGroup(string strName_, Folder flParent_)
        {
            this._flParent = flParent_;
            StrName = strName_;

            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }
        #endregion

        #region ID and Index
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public int GetIndexFromID(int intID)
        {
            for (int i = 0; i < List_frmvMoves.Count; i++)
                if (List_frmvMoves[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public int GetIDFromIndex(int intIndex) => List_frmvMoves[intIndex].IntID;
        #endregion

        #region TestName
        /// <summary>
        /// Test if the name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strOldName"></param>
        /// <returns></returns>
        public bool IsMoveNameTaken(string strNewName, string strOldName)
        {
            //search in the moves
            for (int i = 0; i < List_frmvMoves.Count; i++)
                //test if the name is taken
                if (List_frmvMoves[i].StrName == strNewName && strOldName != strNewName)
                    return true;

            return false;
        }
        #endregion

        #region Adding and Removing
        /// <summary>
        /// Add a move
        /// </summary>
        /// <param name="intDelay"></param>
        /// <param name="intStopTime"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void AddMove(int intDelay, int intStopTime, int intX, int intY)
        {
            //declare all the variables needed to create a unique name
            string strName_ = null;             //name of the move
            int intI = 1;                       //suffix number

            //search for a unique name
            do
                //create the new name
                strName_ = $"Move {intI++}";
            //test if the name is taken
            while (IsMoveNameTaken(strName_, null));

            //add the move
            Movement.List_frmvMovements.Add(new Movement(strName_, intDelay, intStopTime, intX, intY, FlParent.IntID));

            Log.AddLog(ObjectType.Movement, EditingType.Add, null, Movement.CreateTextFromObject(Movement.List_frmvMovements[Movement.List_frmvMovements.Count - 1]));

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove all moves in the group
        /// </summary>
        public void RemoveAll()
        {
            //clear the list of moves
            for (int i = 0; i < Movement.List_frmvMovements.Count; i++)
                if (Movement.List_frmvMovements[i].IntFolderID == FlParent.IntID)
                {
                    string strSaveLog = Movement.CreateTextFromObject(Movement.List_frmvMovements[i]);

                    Log.AddLog(ObjectType.Movement, EditingType.Remove, strSaveLog, null);
                    Movement.List_frmvMovements.RemoveAt(i);

                    Movement.List_frmvMovements.RemoveAt(i--);
                }

            evtValuesChanged?.Invoke(-1, this, GetType());
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node representing the move group
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the list of nodes used to create the main move group node
            List<TreeNode> list_trnMoves = new List<TreeNode>();            //contain the move nodes

            //group the move nodes
            for (int i = 0; i < List_frmvMoves.Count; i++)
                list_trnMoves.Add(List_frmvMoves[i].GetTreeNode());

            //return the movable group node node
            return new TreeNode(StrName, list_trnMoves.ToArray()) { ContextMenuStrip = GetContextMenu(), Tag = StrName };
        }
        #endregion

        #region ContextMenu
        /// <summary>
        /// Get the context menu of the movable group
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare everything needed to create a context menu
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                     //contain the context menu
            List<ToolStripMenuItem> list_tsmiItems = new List<ToolStripMenuItem>();         //contain all the items of the context menu

            //tool used to add a move to the movable group
            list_tsmiItems.Add(new ToolStripMenuItem("Add Move"));
            list_tsmiItems[0].Tag = StrName;
            list_tsmiItems[0].Click += new EventHandler(AddMoveMenu);

            //tool used to remove the movable group
            list_tsmiItems.Add(new ToolStripMenuItem("Remove All"));
            list_tsmiItems[1].Tag = StrName;
            list_tsmiItems[1].Click += new EventHandler(RemoveAllMenu);

            //tool used to modify the properties of the object
            list_tsmiItems.Add(new ToolStripMenuItem("Properties"));
            list_tsmiItems[2].Tag = StrName;
            list_tsmiItems[2].Click += new EventHandler(ModifyPropertiesMenu);

            //add the tools to the context menu
            contextMenuStrip.Items.AddRange(list_tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Add a move to the clicked movable group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddMoveMenu(object sender, EventArgs e) => AddMove(100, 0, 1, 1);

        /// <summary>
        /// Remove all the moves
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAllMenu(object sender, EventArgs e) => RemoveAll();

        private void ModifyPropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Properties Form
        //declare all the control variables
        private Form _frmMoveGroup;                      //properties form
        private CheckBox _chkbxLoop;                     //does the group loop
        private CheckBox _chkbxComeBack;                 //does the group come back
        private CheckBox _chkbxOnlyWhenSeen;             //does the group move only when visible
        private Button _btnCancel;                       //cancel button
        private Button _btnValidate;                     //validation button

        /// <summary>
        /// Open the properties form of the selected object group
        /// </summary>
        /// <param name="intIndex"></param>
        public void OpenPropertiesForm()
        {
            //reset the controls
            _frmMoveGroup = new Form();
            _chkbxLoop = new CheckBox();
            _chkbxComeBack = new CheckBox();
            _chkbxOnlyWhenSeen = new CheckBox();
            _btnCancel = new Button();
            _btnValidate = new Button();

            #region Controls
            //suspend the layout
            _frmMoveGroup.SuspendLayout();

            //contain the information about the looping
            _chkbxLoop.Location = new Point(50, 17);
            _chkbxLoop.Name = "chkbxLoop";
            _chkbxLoop.Size = new Size(200, 20);
            _chkbxLoop.TabIndex = 0;
            _chkbxLoop.Text = $"Does the folder move in a loop ?";
            _chkbxLoop.Checked = FlParent.BlnLoop;

            //contain the information about the comming back
            _chkbxComeBack.Location = new Point(50, 43);
            _chkbxComeBack.Name = "chkbxComeBack";
            _chkbxComeBack.Size = new Size(200, 20);
            _chkbxComeBack.TabIndex = 0;
            _chkbxComeBack.Text = $"Does the folder move come back ?";
            _chkbxComeBack.Checked = FlParent.BlnComeBack;

            //contain the information about the activity
            _chkbxOnlyWhenSeen.Location = new Point(50, 69);
            _chkbxOnlyWhenSeen.Name = "chkbxOnlyWhenSeen";
            _chkbxOnlyWhenSeen.Size = new Size(300, 20);
            _chkbxOnlyWhenSeen.TabIndex = 0;
            _chkbxOnlyWhenSeen.Text = $"Does the folder move only when visible ?";
            _chkbxOnlyWhenSeen.Checked = FlParent.BlnOnlyWhenSeen;

            //cancel the changing of properties
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Location = new Point(202, 95);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Size = new Size(75, 23);
            _btnCancel.TabIndex = 8;
            _btnCancel.Text = "Cancel";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += new EventHandler(Cancel_Click);

            //validate the changing of properties
            _btnValidate.Location = new Point(126, 95);
            _btnValidate.Name = "btnValidate";
            _btnValidate.Size = new Size(75, 23);
            _btnValidate.TabIndex = 9;
            _btnValidate.Text = "Validate";
            _btnValidate.Tag = StrName;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //form containing the controls
            _frmMoveGroup.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmMoveGroup.AutoScaleMode = AutoScaleMode.Font;
            _frmMoveGroup.AcceptButton = _btnValidate;
            _frmMoveGroup.CancelButton = _btnCancel;
            _frmMoveGroup.ClientSize = new Size(304, 125);
            _frmMoveGroup.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmMoveGroup.Name = "frmFolder";
            _frmMoveGroup.StartPosition = FormStartPosition.CenterParent;
            _frmMoveGroup.Text = "Folder";
            _frmMoveGroup.Tag = StrName;

            //add the controls
            _frmMoveGroup.Controls.Add(_btnValidate);
            _frmMoveGroup.Controls.Add(_btnCancel);
            _frmMoveGroup.Controls.Add(_chkbxLoop);
            _frmMoveGroup.Controls.Add(_chkbxComeBack);
            _frmMoveGroup.Controls.Add(_chkbxOnlyWhenSeen);

            //resume layout
            _frmMoveGroup.ResumeLayout(false);
            _frmMoveGroup.PerformLayout();
            #endregion

            //show the form
            _frmMoveGroup.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmMoveGroup.Close();

        /// <summary>
        /// Validate the changing of data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate_Click(object sender, EventArgs e)
        {
            string strSaveLog = Folder.CreateTextFromObject(FlParent);

            FlParent.BlnComeBack = _chkbxComeBack.Checked;
            FlParent.BlnLoop = _chkbxLoop.Checked;
            FlParent.BlnOnlyWhenSeen = _chkbxOnlyWhenSeen.Checked;

            string strResultLog = Folder.CreateTextFromObject(FlParent);

            Log.AddLog(ObjectType.Folder, EditingType.ChangeProperties, strSaveLog, strResultLog);

            _frmMoveGroup.Close();
        }
        #endregion
        #endregion

        #region Perso Event
        /// <summary>
        /// Values changed
        /// </summary>
        private void UpdateTreeView(int intSenderID, object objSender, Type typSender) 
        {
            if (Main.WrldWorld != null)
                WrldInWorld.UpdateTreeNodes();
        }
        #endregion
    }
}