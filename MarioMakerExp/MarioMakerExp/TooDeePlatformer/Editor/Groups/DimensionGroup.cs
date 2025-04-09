/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
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
** This class is a group of dimensions, it contains and manages dimensions.  **
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
using System.IO;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// This class is a group of dimensions, it contains and manages dimensions.
    /// </summary>
    class DimensionGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                     //triggered when visual values are changed

        //declare the properties
        public TabControl TbcDimensions { get; set; }                   //tabcontrol containing in each tab a dimension

        //declare the fields
        private string _strName;                                        //contain the name of the group
        private readonly World _wrldParent;                             //contain the parent

        #region Properties
        /// <summary>
        /// Contain all the dimension of the group
        /// </summary>
        public List<Dimension> List_dimDimensions
        {
            get
            {
                List<Dimension> list_dimDimensions_ = new List<Dimension>();

                for (int i = 0; i < Dimension.List_dimDimensions.Count; i++)
                    if (Dimension.List_dimDimensions[i].IntWorldID == WrldParent.IntID)
                        list_dimDimensions_.Add(Dimension.List_dimDimensions[i]);

                return list_dimDimensions_;
            }
        }

        /// <summary>
        /// Indexer of the list of dimensions
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public Dimension this[int intID]
        {
            get
            {
                for (int i = 0; i < List_dimDimensions.Count; i++)
                    if (List_dimDimensions[i].IntID == intID)
                        return List_dimDimensions[i];

                return null;
            }

            set
            {
                for (int i = 0; i < List_dimDimensions.Count; i++)
                    if (List_dimDimensions[i].IntID == intID)
                        List_dimDimensions[i] = value;
            }
        }

        /// <summary>
        /// Get and set the name of the group
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
        /// Get the displayed dimension and set the displayed dimension
        /// </summary>
        public int IntSelectedDimension
        {
            get => TbcDimensions.SelectedIndex;

            set
            {
                //set the dimension tab
                TbcDimensions.SelectedIndex = value;

                evtValuesChanged?.Invoke(-1, this, GetType());
            }
        }

        /// <summary>
        /// Parent of the object
        /// </summary>
        public World WrldParent
        {
            get => _wrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values with a previously setted name
        /// </summary>
        /// <param name="strName_"></param>
        public DimensionGroup(string strName_, World wrldParent_)
        {
            StrName = strName_;
            TbcDimensions = new TabControl();
            this._wrldParent = wrldParent_;

            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);

            //create the tabcontrol
            TbcDimensions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            TbcDimensions.Location = new Point(0, 30);
            TbcDimensions.Name = StrName;
            TbcDimensions.Size = new Size(1759 - 90, 747);
            TbcDimensions.TabIndex = 0;
            TbcDimensions.SelectedIndexChanged += new EventHandler(ChangeTab);
            TbcDimensions.Selecting += (s, e) =>
            {
                if (WrldParent.BlnPressingCtrl || WrldParent.BlnPressingShift)
                    e.Cancel = true;
            };

            IntSelectedDimension = 0;
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
        public int GetIDFromIndex(int intIndex) => List_dimDimensions[intIndex].IntID;
        #endregion

        #region TestName
        /// <summary>
        /// Test if the name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strOldName"></param>
        /// <returns></returns>
        public bool IsDimensionNameTaken(string strNewName, string strOldName)
        {
            //search in the moves
            for (int i = 0; i < List_dimDimensions.Count; i++)
                //test if the name is taken
                if (List_dimDimensions[i].StrName == strNewName && strOldName != strNewName)
                    return true;

            return false;
        }
        #endregion

        #region Adding and removing
        /// <summary>
        /// Add a dimension
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void AddDimension(int intWidth, int intHeight)
        {
            //declare the counter 
            int intI = 1;                           //add a number each time the name was taken
            string strNewName;

            //try a new name
            do
            {
                //set the new attempted name
                strNewName = $"Dimension {intI}";

                //increase the suffix number
                intI++;
            }
            //if the name is taken try again
            while (IsDimensionNameTaken(strNewName, null));

            //add the new dimension using the automaticly generated name with the given size
            Dimension.List_dimDimensions.Add(new Dimension(strNewName, new Distance() { dblHeight = intHeight, dblWidth = intWidth}, WrldParent.IntID));

            Log.AddLog(ObjectType.Dimension, EditingType.Add, null, Dimension.CreateTextFromObject(Dimension.List_dimDimensions[Dimension.List_dimDimensions.Count - 1]), null, Dimension.List_dimDimensions[Dimension.List_dimDimensions.Count - 1].ImgBackgroundImage);

            //add a new tab with the last dimension created to the tabcontrol
            TbcDimensions.Controls.Add(List_dimDimensions[List_dimDimensions.Count - 1].TbpDimension);

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove the selected dimension
        /// </summary>
        /// <param name="intIndex"></param>
        public void Remove(int intIndex)
        {
            string strSaveLog = Dimension.CreateTextFromObject(Dimension.List_dimDimensions[intIndex]);
            Image imgSave = Dimension.List_dimDimensions[intIndex].ImgBackgroundImage;

            //Dimension.List_dimDimensions[intIndex].SelectAll();
            //Dimension.List_dimDimensions[intIndex].RemoveAllSelected();

            for (int i = 0; i < Dimension.List_dimDimensions[intIndex].FlgrFolderGroup.List_flFolders.Count; i++)
                Dimension.List_dimDimensions[intIndex].FlgrFolderGroup.Remove(Folder.GetIndexFromID(Dimension.List_dimDimensions[intIndex].FlgrFolderGroup.List_flFolders[i].IntID));

            Dimension.List_dimDimensions[intIndex].PicBoxSelectedArea.Dispose();
            Dimension.List_dimDimensions[intIndex].PnlDimension.Dispose();
            Dimension.List_dimDimensions[intIndex].TbpDimension.Dispose();
            if (File.Exists(Dimension.List_dimDimensions[intIndex].StrBackgroundPath))
                File.Delete(Dimension.List_dimDimensions[intIndex].StrBackgroundPath);

            Dimension.List_dimDimensions.RemoveAt(intIndex);

            Log.AddLog(ObjectType.Dimension, EditingType.Remove, strSaveLog, null, imgSave, null);

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove all objects
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < List_dimDimensions.Count; i++)
                if (Dimension.List_dimDimensions[i].IntWorldID == WrldParent.IntID)
                    Remove(i);
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the world node
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the list of world node
            List<TreeNode> list_trnDimensions = new List<TreeNode>();

            for (int i = 0; i < List_dimDimensions.Count; i++)
                list_trnDimensions.Add(List_dimDimensions[i].GetTreeNode());

            //retrun the world node
            return new TreeNode(StrName, list_trnDimensions.ToArray()) { ContextMenuStrip = GetContextMenu() };
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

            //add the new tool used to add a dimension
            list_tsmiMain.Add(new ToolStripMenuItem("Add Dimension"));
            list_tsmiMain[0].Click += new EventHandler(AddDimension_Click);
            list_tsmiMain[0].Tag = StrName;

            //remove all
            list_tsmiMain.Add(new ToolStripMenuItem("Remove all"));
            list_tsmiMain[1].Tag = StrName;
            list_tsmiMain[1].Click += new EventHandler(RemoveAllMenu);

            //add all the tools to the contextbox
            contextMenuStrip.Items.AddRange(list_tsmiMain.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Method used the add a dimension
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDimension_Click(object sender, EventArgs e)
        {
            //add a new dimension
            AddDimension(64, 64);

            //open a properties form used to modify the properties of the last added dimension
            List_dimDimensions[List_dimDimensions.Count - 1].OpenPropertiesForm();
        }

        /// <summary>
        /// Remove all the objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAllMenu(object sender, EventArgs e) => RemoveAll();
        #endregion
        #endregion

        #region Perso Event
        /// <summary>
        /// Event getting activate when the dimension's or the world's values are changed
        /// </summary>
        /// <param name="intID_"></param>
        /// <param name="objSender"></param>
        /// <param name="typSender"></param>
        private void UpdateTreeView(int intSenderID, object objSender, Type typSender) 
        {
            if (Main.WrldWorld != null)
                WrldParent.UpdateTreeNodes();
        }

        /// <summary>
        /// Event reseting the dimension dragging variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeTab(object sender, EventArgs e)
        {
            for (int i = 0; i < List_dimDimensions.Count; i++)
                List_dimDimensions[i].StopDragging(null,null);
        }
        #endregion
    }
}