/******************************************************************************
** PROGRAMME MarioMaker Experimental Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 13.02.2021                                                    **
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
** Class containing a group of static folder of obstacles.                   **
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
    /// Group containing the static folder of obstalces
    /// </summary>
    class StaticFolderGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                        //triggered when the visual values of the group has been changed

        //declare the properties
        public int intID { get; set; }                                      //containing the ID of the object
        public List<StaticFolder> list_stflStaticFolders { get; set; }      //contain the static folders

        //declare the variables
        private string strName_;                                            //contain the name of the object
        private int intSelectedObject_;                                      //contain the selected object

        #region Properties
        /// <summary>
        /// Indexer of the groups
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public StaticFolder this[int intIndex]
        {
            get => list_stflStaticFolders[intIndex];

            set => list_stflStaticFolders[intIndex] = value;
        }

        /// <summary>
        /// Get the index of the selected object without getting under 0 or higher that the number in the list and set the index
        /// </summary>
        public int intSelectedObject
        {
            get
            {
                //test if the index is under 0
                if (intSelectedObject_ < 0)
                    return 0;

                //test if the index is too high
                if (intSelectedObject_ >= this.intCount)
                    //return the last item in the list
                    return this.intCount - 1;

                //return the index
                return intSelectedObject_;
            }

            set => intSelectedObject_ = value;
        }


        /// <summary>
        /// Get the number of static folders
        /// </summary>
        public int intCount => list_stflStaticFolders.Count;

        /// <summary>
        /// Get the name of the group and set the name
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
        /// Set the default values of the folder group
        /// </summary>
        /// <param name="strName_"></param>
        public StaticFolderGroup(string strName_)
        {
            intID = GeneralData.intNextID++;
            strName = strName_;
            list_stflStaticFolders = new List<StaticFolder>();
            AddStaticFolder();
        }

        /// <summary>
        /// Create a new null static folder group
        /// </summary>
        public StaticFolderGroup()
        {

        }
        #endregion

        #region Test Name
        /// <summary>
        /// Test if the new name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strCurrentName"></param>
        /// <returns></returns>
        private bool IsStaticFolderNameTaken(string strNewName, string strCurrentName)
        {
            //search in the obstacles
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                //test if the name is taken exepting the current name
                if (list_stflStaticFolders[i].strName == strNewName && strCurrentName != strNewName)
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
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                //test if there is an intersection
                if (!list_stflStaticFolders[i].TestEmptiness(posTested))
                    //inform the game the new position is not empty
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
            //search in the obstacles
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                //test if there is an intersection
                if (!list_stflStaticFolders[i].TestEmptiness(posTested, intExeption))
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
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                //test if there is an intersection
                if (!list_stflStaticFolders[i].TestEmptiness(posTested, blnSelection))
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
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                list_stflStaticFolders[i].SelectAll();
        }

        /// <summary>
        /// Unselect all object insides
        /// </summary>
        public void UnselectAll()
        {
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                list_stflStaticFolders[i].UnselectAll();
        }

        /// <summary>
        /// Select or unselect all the obtacles present inside the selection area
        /// </summary>
        public void SelectWithArea(Position posArea, bool blnSelect)
        {
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                list_stflStaticFolders[i].SelectArea(posArea, blnSelect);
        }
        #endregion

        #region Adding and Removing
        /// <summary>
        /// Add a static folder
        /// </summary>
        /// <param name="intType_"></param>
        /// <param name="Position_"></param>
        /// <param name="intDimensionHeight"></param>
        public void AddStaticFolder()
        {
            //declare all the variables needed to create an unique name
            string strName_ = null;         //contain the potential name
            int intI = 1;                   //contain the number of the try

            //search an unique name
            do
                //get a new name
                strName_ = $"Static Folder {list_stflStaticFolders.Count + intI++}";
            //test if the name is taken
            while (IsStaticFolderNameTaken(strName_, null));

            //the obstacle to the obstacle list
            list_stflStaticFolders.Add(new StaticFolder(strName_));

            //set the events
            list_stflStaticFolders[list_stflStaticFolders.Count - 1].evtValuesChanged += new dlgPersoEvent(ChildValuesChanged);
            list_stflStaticFolders[list_stflStaticFolders.Count - 1].dlgTryNewName += new dlgTryNewNameEvent(IsStaticFolderNameTaken);
            list_stflStaticFolders[list_stflStaticFolders.Count - 1].evtRemove += new dlgPersoEvent(RemoveObject);

            evtValuesChanged?.Invoke(intID, this, GetType());
        }

        /// <summary>
        /// Remove the selected object
        /// </summary>
        /// <param name="intIndex"></param>
        public void Remove(int intIndex)
        {
            //hide the picturebox
            list_stflStaticFolders[intIndex].RemoveAll();

            //remove the obstacle from the list
            list_stflStaticFolders.RemoveAt(intIndex);

            evtValuesChanged?.Invoke(intID, this, GetType());
        }

        /// <summary>
        /// Remove all the objects of the group
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                Remove(i);

            evtValuesChanged?.Invoke(intID, this, GetType());
        }

        /// <summary>
        /// Remove every selected object
        /// </summary>
        public void RemoveSelected()
        {
            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                list_stflStaticFolders[i].RemoveSelected();
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the static folder group node
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the list of world node
            List<TreeNode> list_trnStaticFolders = new List<TreeNode>();

            for (int i = 0; i < list_stflStaticFolders.Count; i++)
                list_trnStaticFolders.Add(list_stflStaticFolders[i].GetTreeNode());

            //retrun the world node
            return new TreeNode(strName, list_trnStaticFolders.ToArray()) { ContextMenuStrip = GetContextMenu() };
        }
        #endregion

        #region Context Menu
        /// <summary>
        /// Get the context menu of the static folder group
        /// </summary>
        /// <returns></returns>
        private ContextMenuStrip GetContextMenu()
        {
            //declare the objects for the contextbox
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();             //declare the contextbox control
            List<ToolStripMenuItem> list_tsmiMain = new List<ToolStripMenuItem>();       //declare the list of tools

            //add the new tool used to add a dimension
            list_tsmiMain.Add(new ToolStripMenuItem("Add Static Folder"));
            list_tsmiMain[0].Click += new EventHandler(AddStaticFolder_Click);
            list_tsmiMain[0].Tag = intID;

            //remove all
            list_tsmiMain.Add(new ToolStripMenuItem("Remove all"));
            list_tsmiMain[1].Tag = intID;
            list_tsmiMain[1].Click += new EventHandler(RemoveAllMenu);

            //add all the tools to the contextbox
            contextMenuStrip.Items.AddRange(list_tsmiMain.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Method used the add a static folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddStaticFolder_Click(object sender, EventArgs e) => AddStaticFolder();

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
        /// Triggered after a childs visual value has been changed
        /// </summary>
        /// <param name="intSenderID"></param>
        /// <param name="objSender"></param>
        /// <param name="typSender"></param>
        private void ChildValuesChanged(int intSenderID, object objSender, Type typSender) => evtValuesChanged?.Invoke(intSenderID, objSender, typSender);

        /// <summary>
        /// Remove the Triggering object
        /// </summary>
        /// <param name="intSenderID"></param>
        /// <param name="objSender"></param>
        /// <param name="typSender"></param>
        private void RemoveObject(int intSenderID, object objSender, Type typSender)
        {
            //test if the object is an obstacle
            if (new StaticFolder().GetType() == typSender)
            {
                //search for the obstacle
                for (int i = 0; i < list_stflStaticFolders.Count; i++)
                    if (list_stflStaticFolders[i].intID == intSenderID)
                    {
                        Remove(i);

                        break;
                    }
            }
        }
        #endregion
    }
}