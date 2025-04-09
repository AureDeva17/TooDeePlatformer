/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 17.02.2021                                                    **
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
** Class containing a group of folders of obstacles.                         **
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

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Group containing the folders of obstalces
    /// </summary>
    class FolderGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                        //triggered when the visual values of the group has been changed

        //declare the variables
        private string _strName;                                            //contain the name of the object
        private int _intSelectedFolderID;                                   //contain the selected object
        private readonly Dimension _dimParent;                              //contain the parent

        #region Properties
        /// <summary>
        /// Contain all the folders of the group
        /// </summary>
        public List<Folder> List_flFolders
        {
            get
            {
                List<Folder> list_objects_ = new List<Folder>();

                for (int i = 0; i < Folder.List_flFolders.Count; i++)
                    if (Folder.List_flFolders[i].IntDimensionID == DimParent.IntID)
                        list_objects_.Add(Folder.List_flFolders[i]);

                return list_objects_;
            }
        }

        /// <summary>
        /// Indexer of the list of folders
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public Folder this[int intID]
        {
            get
            {
                for (int i = 0; i < List_flFolders.Count; i++)
                    if (List_flFolders[i].IntID == intID)
                        return List_flFolders[i];

                return null;
            }

            set
            {
                for (int i = 0; i < List_flFolders.Count; i++)
                    if (List_flFolders[i].IntID == intID)
                        List_flFolders[i] = value;
            }
        }

        /// <summary>
        /// Get the ID of the selected folder
        /// </summary>
        public int IntSelectedFolderID
        {
            get
            {
                for (int i = 0; i < List_flFolders.Count; i++)
                    if (List_flFolders[i].IntID == _intSelectedFolderID)
                        return _intSelectedFolderID;

                return List_flFolders[0].IntID;
            }

            set => _intSelectedFolderID = value;
        }


        /// <summary>
        /// Get the number of folders
        /// </summary>
        public int IntCount => List_flFolders.Count;

        /// <summary>
        /// Get the name of the group and set the name
        /// </summary>
        public string StrName
        {
            get => _strName;

            set => _strName = value;
        }

        /// <summary>
        /// Parent of the object
        /// </summary>
        public Dimension DimParent
        {
            get => _dimParent;
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => DimParent.DimgrParent.WrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values of the folder group
        /// </summary>
        /// <param name="strName_"></param>
        public FolderGroup(string strName_, Dimension dimParent_)
        {
            this._dimParent = dimParent_;
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
            for (int i = 0; i < List_flFolders.Count; i++)
                if (List_flFolders[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public int GetIDFromIndex(int intIndex) => List_flFolders[intIndex].IntID;
        #endregion

        #region Test Name
        /// <summary>
        /// Test if the new name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strCurrentName"></param>
        /// <returns></returns>
        public bool IsFolderNameTaken(string strNewName, string strCurrentName)
        {
            //search in the moving folders
            for (int i = 0; i < List_flFolders.Count; i++)
                //test if the name is taken exepting the current name
                if (List_flFolders[i].StrName == strNewName && strCurrentName != strNewName)
                    return true;

            return false;
        }
        #endregion

        #region Test of Emptiness
        /// <summary>
        /// Test if the position is empty
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested)
        {
            //search in the obstacles
            for (int i = 0; i < List_flFolders.Count; i++)
                //test if there is an intersection
                if (!List_flFolders[i].TestEmptiness(posTested))
                    //inform the game the new position is not empty
                    return false;

            return true;
        }

        /// <summary>
        /// Test if the position is empty with an exeption
        /// </summary>
        /// <param name="posTested"></param>
        /// <returns></returns>
        public bool TestEmptiness(Position posTested, int intExeption, bool blnIsObstacle)
        {
            //search in the obstacles
            for (int i = 0; i < List_flFolders.Count; i++)
                //test if there is an intersection
                if (!List_flFolders[i].TestEmptiness(posTested, intExeption, blnIsObstacle))
                    //inform the game the new position is not empty
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
            //search in the obstacles
            for (int i = 0; i < List_flFolders.Count; i++)
                //test if there is an intersection
                if (!List_flFolders[i].TestEmptiness(posTested, blnSelection))
                    //inform the game the new position is not empty
                    return false;

            return true;
        }
        #endregion

        #region Select
        /// <summary>
        /// Select all object insides
        /// </summary>
        public void SelectAll()
        {
            for (int i = 0; i < List_flFolders.Count; i++)
                List_flFolders[i].SelectAll();
        }

        /// <summary>
        /// Unselect all object insides
        /// </summary>
        public void UnselectAll()
        {
            for (int i = 0; i < List_flFolders.Count; i++)
                List_flFolders[i].UnselectAll();
        }

        /// <summary>
        /// Select or unselect all the obtacles present inside the selection area
        /// </summary>
        public void SelectWithArea(Position posArea, bool blnSelect)
        {
            for (int i = 0; i < List_flFolders.Count; i++)
                List_flFolders[i].SelectArea(posArea, blnSelect);
        }
        #endregion

        #region Adding and Removing
        /// <summary>
        /// Add a folder
        /// </summary>
        /// <param name="intType_"></param>
        /// <param name="Position_"></param>
        /// <param name="intDimensionHeight"></param>
        public void AddFolder()
        {
            //declare all the variables needed to create an unique name
            string strName_ = null;         //contain the potential name
            int intI = 1;                   //contain the number of the try

            //search an unique name
            do
                //get a new name
                strName_ = $"Folder {intI++}";
            //test if the name is taken
            while (IsFolderNameTaken(strName_, null));

            //the obstacle to the obstacle list
            Folder.List_flFolders.Add(new Folder(strName_, DimParent.IntID));

            Log.AddLog(ObjectType.Folder, EditingType.Add, null, Folder.CreateTextFromObject(Folder.List_flFolders[Folder.List_flFolders.Count - 1]));

            evtValuesChanged?.Invoke(1, this, GetType());
        }
               
        /// <summary>
        /// Remove the selected object
        /// </summary>
        /// <param name="intIndex"></param>
        public void Remove(int intIndex)
        {
            string strSaveLog = Folder.CreateTextFromObject(Folder.List_flFolders[intIndex]);

            Folder.List_flFolders[intIndex].RemoveAll();
            Folder.List_flFolders.RemoveAt(intIndex);

            Log.AddLog(ObjectType.Folder, EditingType.Remove, strSaveLog, null);

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove all the objects of the group
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < Folder.List_flFolders.Count; i++)
                if (Folder.List_flFolders[i].IntDimensionID == DimParent.IntID)
                    Remove(i--);
        }

        /// <summary>
        /// Remove every selected object
        /// </summary>
        public void RemoveSelected()
        {
            string strSaveLog = "";

            for (int i = 0; i < Folder.List_flFolders.Count; i++)
                if (Folder.List_flFolders[i].IntDimensionID == DimParent.IntID)
                {
                    for (int ii = 0; ii < Obstacle.List_obsObstacles.Count; ii++)
                        if (Obstacle.List_obsObstacles[ii].BlnSelected && Obstacle.List_obsObstacles[ii].IntFolderID == Folder.List_flFolders[i].IntID)
                            strSaveLog += Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[ii]) + "\n";

                    for (int ii = 0; ii < Special.List_spcSpecials.Count; ii++)
                        if (Special.List_spcSpecials[ii].BlnSelected && Special.List_spcSpecials[ii].IntFolderID == Folder.List_flFolders[i].IntID)
                            strSaveLog += Special.CreateTextFromObject(Special.List_spcSpecials[ii]) + "\n";

                    Folder.List_flFolders[i].ObsgrObstacles.RemoveSelected();
                    Folder.List_flFolders[i].SpcgrSpecials.RemoveSelected();
                }

            Log.AddLog(ObjectType.SpecialPlusObstacle, EditingType.RemoveGroup, strSaveLog, null);
        }
        #endregion

        #region Moving
        /// <summary>
        /// Start Moving
        /// </summary>
        public void StartMoving()
        {
            //start the moving of all the folders
            for (int i = 0; i < List_flFolders.Count; i++)
                List_flFolders[i].StartMoving();
        }

        /// <summary>
        /// Stop Moving
        /// </summary>
        public void StopMoving()
        {
            //stop the moving of all the folders
            for (int i = 0; i < List_flFolders.Count; i++)
                List_flFolders[i].StopMoving();
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the folder group node
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the list of world node
            List<TreeNode> list_trnFolders = new List<TreeNode>();

            for (int i = 0; i < List_flFolders.Count; i++)
                list_trnFolders.Add(List_flFolders[i].GetTreeNode());

            //retrun the world node
            return new TreeNode(StrName, list_trnFolders.ToArray()) { ContextMenuStrip = GetContextMenu() };
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Get the context menu of the moving folder group
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare the objects for the contextbox
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                 //declare the contextbox control
            List<ToolStripMenuItem> list_tsmiMain = new List<ToolStripMenuItem>();      //declare the list of tools

            //add the new tool used to add a dimension
            list_tsmiMain.Add(new ToolStripMenuItem("Add Folder"));
            list_tsmiMain[0].Click += new EventHandler(AddFolder_Click);
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
        /// Method used the add a folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFolder_Click(object sender, EventArgs e) => AddFolder();

        /// <summary>
        /// Remove all the objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAllMenu(object sender, EventArgs e) => RemoveAll();
        #endregion
        #endregion

        #region Perso Events
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