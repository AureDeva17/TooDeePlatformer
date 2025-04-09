/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                         **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 07.02.2021                                                    **
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
** This class is a group of obstacles. It is used to manage the obstacles.   **
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
    /// This class is a group of obstacles. It is used to manage the obstacles.
    /// </summary>
    class ObstacleGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                //triggered when the visual values are changed

        //declare the fields
        private string _strName;                                    //contain the name of the group
        private readonly Folder _flParent;                          //contain the parent

        #region Properties
        /// <summary>
        /// Contain all the obstacles of the group
        /// </summary>
        public List<Obstacle> List_obsObstacles
        {
            get
            {
                List<Obstacle> list_objects_ = new List<Obstacle>();

                for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                    if (Obstacle.List_obsObstacles[i].IntFolderID == FlParent.IntID)
                        list_objects_.Add(Obstacle.List_obsObstacles[i]);

                return list_objects_;
            }
        }

        /// <summary>
        /// Indexer of the list of obstacles
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public Obstacle this[int intID]
        {
            get
            {
                for (int i = 0; i < List_obsObstacles.Count; i++)
                    if (List_obsObstacles[i].IntID == intID)
                        return List_obsObstacles[i];

                return null;
            }

            set
            {
                for (int i = 0; i < List_obsObstacles.Count; i++)
                    if (List_obsObstacles[i].IntID == intID)
                        List_obsObstacles[i] = value;
            }
        }

        /// <summary>
        /// Get the name of the group and set the name
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
        public ObstacleGroup(string strName_, Folder flParent_)
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
            for (int i = 0; i < List_obsObstacles.Count; i++)
                if (List_obsObstacles[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public int GetIDFromIndex(int intIndex) => List_obsObstacles[intIndex].IntID;
        #endregion

        #region Add to Location
        /// <summary>
        /// Add to the location of every obstacles
        /// </summary>
        /// <param name="dblX"></param>
        /// <param name="dblY"></param>
        public void AddToLocation(double dblX, double dblY)
        {
            for (int i = 0; i < List_obsObstacles.Count; i++)
                List_obsObstacles[i].AddToLocation(dblX, dblY);
        }
        #endregion

        #region Test Name
        /// <summary>
        /// Test if the new name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strCurrentName"></param>
        /// <returns></returns>
        public bool IsObstacleNameTaken(string strNewName, string strCurrentName)
        {
            //search in the obstacles
            for (int i = 0; i < List_obsObstacles.Count; i++)
                //test if the name is taken exepting the current name
                if (List_obsObstacles[i].StrName == strNewName && strCurrentName != strNewName)
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
            for (int i = 0; i < List_obsObstacles.Count; i++)
            {
                //declare the variable containing the position of the currently tested obstacle
                Position posPosition_ = new Position();                //contain the position of the tested obstacle

                //set the position
                posPosition_.LocLocation.dblX = List_obsObstacles[i].IntX;
                posPosition_.LocLocation.dblY = List_obsObstacles[i].IntY;
                posPosition_.DisDistance.dblHeight = List_obsObstacles[i].IntHeight;
                posPosition_.DisDistance.dblWidth = List_obsObstacles[i].IntWidth;

                //test if there is an intersection
                if (posPosition_.Rectangle().IntersectsWith(posTested.Rectangle()) || posTested.Rectangle().IntersectsWith(posPosition_.Rectangle()))
                    //inform the game the new position is not empty
                    return false;
            }

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
            for (int i = 0; i < List_obsObstacles.Count; i++)
                if (intExeption != List_obsObstacles[i].IntID || !blnIsObstacle)
                {
                    //declare the variable containing the position of the currently tested obstacle
                    Position posPosition_ = new Position();                //contain the position of the tested obstacle

                    //set the position
                    posPosition_.LocLocation.dblX = List_obsObstacles[i].IntX;
                    posPosition_.LocLocation.dblY = List_obsObstacles[i].IntY;
                    posPosition_.DisDistance.dblHeight = List_obsObstacles[i].IntHeight;
                    posPosition_.DisDistance.dblWidth = List_obsObstacles[i].IntWidth;

                    //test if there is an intersection
                    if (posPosition_.Rectangle().IntersectsWith(posTested.Rectangle()) || posTested.Rectangle().IntersectsWith(posPosition_.Rectangle()))
                        //inform the game the new position is not empty
                        return false;
                }

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
            for (int i = 0; i < List_obsObstacles.Count; i++)
                if (List_obsObstacles[i].BlnSelected && blnSelection || !List_obsObstacles[i].BlnSelected && !blnSelection)
                {
                    //declare the variable containing the position of the currently tested obstacle
                    Position posPosition_ = new Position();                //contain the position of the tested obstacle

                    //set the position
                    posPosition_.LocLocation.dblX = List_obsObstacles[i].IntX;
                    posPosition_.LocLocation.dblY = List_obsObstacles[i].IntY;
                    posPosition_.DisDistance.dblHeight = List_obsObstacles[i].IntHeight;
                    posPosition_.DisDistance.dblWidth = List_obsObstacles[i].IntWidth;

                    //test if there is an intersection
                    if (posPosition_.Rectangle().IntersectsWith(posTested.Rectangle()) || posTested.Rectangle().IntersectsWith(posPosition_.Rectangle()))
                        //inform the game the new position is not empty
                        return false;
                }

            return true;
        }
        #endregion

        #region Select
        /// <summary>
        /// Select all the obtacles present inside the selection area
        /// </summary>
        public void SelectArea(Position posArea, bool blnSelect)
        {
            for (int i = 0; i < List_obsObstacles.Count; i++)
                List_obsObstacles[i].SelectArea(posArea, blnSelect);
        }
        #endregion

        #region Adding and removing
        /// <summary>
        /// Add an obstacle of the chosen type, size and location
        /// </summary>
        /// <param name="intType_"></param>
        /// <param name="Position_"></param>
        /// <param name="intDimensionHeight"></param>
        public void AddObstacle(int intType_, Position Position_)
        {
            //declare all the variables needed to create an unique name
            string strName_ = null;         //contain the potential name
            int intI = 1;                   //contain the number of the try

            //search an unique name
            do
                //get a new name
                strName_ = $"{ObstacleType.List_obstypTypes[ObstacleType.GetIndexFromID(intType_)].StrName} {intI++}";
            //test if the name is taken
            while (IsObstacleNameTaken(strName_, null));

            //the obstacle to the obstacle list
            Obstacle.List_obsObstacles.Add(new Obstacle(strName_, intType_, Position_, FlParent.IntID));

            Log.AddLog(ObjectType.Obstacle, EditingType.Add, null, Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[Obstacle.List_obsObstacles.Count - 1]));

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove the selected obstacle
        /// </summary>
        /// <param name="intIndex"></param>
        public void Remove(int intIndex)
        {
            string strSaveLog = Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[intIndex]);

            Log.AddLog(ObjectType.Obstacle, EditingType.Remove, strSaveLog, null);
            Obstacle.List_obsObstacles[intIndex].DeletePictureBox();
            Obstacle.List_obsObstacles.RemoveAt(intIndex);


            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove all the obstacles of the group
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                if (Obstacle.List_obsObstacles[i].IntFolderID == FlParent.IntID)
                {
                    Obstacle.List_obsObstacles[i].DeletePictureBox();
                    Obstacle.List_obsObstacles.RemoveAt(i--);
                }
            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove every selected obstacle
        /// </summary>
        public void RemoveSelected()
        {
            for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                if (Obstacle.List_obsObstacles[i].IntFolderID == FlParent.IntID && Obstacle.List_obsObstacles[i].BlnSelected)
                {
                    Obstacle.List_obsObstacles[i].DeletePictureBox();
                    Obstacle.List_obsObstacles.RemoveAt(i--);
                }
            evtValuesChanged?.Invoke(-1, this, GetType());
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node containg a movable group node and an obstacles node
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the new node
            List<TreeNode> list_trnObstacles = new List<TreeNode>();        //contain the obstacle nodes

            //group the obstacles nodes
            for (int i = 0; i < List_obsObstacles.Count; i++)
                list_trnObstacles.Add(List_obsObstacles[i].GetTreeNode());

            //return the new node
            return new TreeNode(StrName, list_trnObstacles.ToArray()) { ContextMenuStrip = GetContextMenu(), Tag = StrName };
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

            //tool made to add an obstacle
            tsmiItems.Add(new ToolStripMenuItem("Add Obstacle From Selection"));
            tsmiItems[0].Tag = StrName;
            tsmiItems[0].Click += new EventHandler(AddObstacleMenu);

            //remove all
            tsmiItems.Add(new ToolStripMenuItem("Remove all"));
            tsmiItems[1].Tag = StrName;
            tsmiItems[1].Click += new EventHandler(RemoveAllMenu);

            //tool made to select the object
            tsmiItems.Add(new ToolStripMenuItem("Select"));
            tsmiItems[2].Tag = StrName;
            tsmiItems[2].Click += new EventHandler(SelectObstaclesMenu);

            //tool made to unselect the object
            tsmiItems.Add(new ToolStripMenuItem("Unselect"));
            tsmiItems[3].Tag = StrName;
            tsmiItems[3].Click += new EventHandler(UnselectObstaclesMenu);

            //add the tools to the context box
            contextMenuStrip.Items.AddRange(tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Add an obstacle from the selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddObstacleMenu(object objSender, EventArgs e)
        {
            FlParent.FlgrParent.IntSelectedFolderID = FlParent.IntID;
            FlParent.FlgrParent.DimParent.AddObstacleFromSelection();
        }

        /// <summary>
        /// Remove all obstacles
        /// </summary>
        /// <param name="objSender"></param>
        /// <param name="e"></param>
        private void RemoveAllMenu(object objSender, EventArgs e) => RemoveAll();

        /// <summary>
        /// Select all the obstacles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectObstaclesMenu(object sender, EventArgs e) => SelectAll();

        /// <summary>
        /// Unselect all the obstacles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectObstaclesMenu(object sender, EventArgs e) => UnselectAll();
        #endregion
        #endregion

        #region Select
        /// <summary>
        /// Select every obstacles
        /// </summary>
        public void SelectAll()
        {
            for (int i = 0; i < List_obsObstacles.Count; i++)
                List_obsObstacles[i].BlnSelected = true;
        }

        /// <summary>
        /// Unselect every obstacles
        /// </summary>
        public void UnselectAll()
        {
            for (int i = 0; i < List_obsObstacles.Count; i++)
                List_obsObstacles[i].BlnSelected = false;
        }
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