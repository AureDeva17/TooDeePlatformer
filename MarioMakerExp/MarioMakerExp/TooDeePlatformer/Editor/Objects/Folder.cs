/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 12.02.2021                                                    **
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
** Folder of objects                                                         **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Folder of objects
    /// </summary>
    class Folder
    {
        //declare the static properties
        public const string strCLASSNAME = "Folder";            //class name
        public const ObjectType objTYPE = ObjectType.Folder;    //object type
        public static int IntNextID { get; set; }               //next ID to be used
        public static List<Folder> List_flFolders { get; set; } //list of all the folders

        #region Static Method
        /// <summary>
        /// Return the index of the object targeted by an ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static int GetIndexFromID(int intID)
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
        public static int GetIDFromIndex(int intIndex) => List_flFolders[intIndex].IntID;

        /// <summary>
        /// Get the object in the list by using the ID
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public static Folder GetObjectByID(int intID) => List_flFolders[GetIndexFromID(intID)];

        /// <summary>
        /// Create the text file of the class
        /// </summary>
        public static string GetDataText()
        {
            string strText = Main.strVERSION + "\n";

            strText += strCLASSNAME + "\n";
            strText += IntNextID + "\n";
            strText += List_flFolders.Count + "\n";

            for (int i = 0; i < List_flFolders.Count; i++)
            {
                strText += CreateTextFromObject(List_flFolders[i]);

                if (List_flFolders.Count != i)
                    strText += "\n";
            }

            return strText;
        }

        /// <summary>
        /// Copy the data from the file to the folders
        /// </summary>
        public static void CopyFromFile()
        {
            string[] tab_strLines = File.ReadAllLines(Main.StrFolderTXT);

            if (tab_strLines[0] == Main.strVERSION)
            {
                if (tab_strLines[1] == strCLASSNAME)
                {
                    List_flFolders.Clear();

                    IntNextID = Convert.ToInt32(tab_strLines[2]);

                    int intNbObjects = Convert.ToInt32(tab_strLines[3]);

                    for (int i = 0; i < intNbObjects; i++)
                        List_flFolders.Add(CreateObjectFromText(tab_strLines[i + 4], false));
                }
                else
                    MessageBox.Show("The file isn't a storage for folders !");
            }
            else
                MessageBox.Show("The version of the file is obselete !");
        }

        /// <summary>
        /// Create a line of text from an object
        /// </summary>
        /// <param name="flObject"></param>
        /// <returns></returns>
        public static string CreateTextFromObject(Folder flObject)
        {
            string strText = "";

            strText += flObject.IntID + $"{Main.chrSEPARATOR}";
            strText += flObject.IntDimensionID + $"{Main.chrSEPARATOR}";
            strText += flObject.StrName + $"{Main.chrSEPARATOR}";
            strText += flObject.BlnLoop + $"{Main.chrSEPARATOR}";
            strText += flObject.BlnComeBack + $"{Main.chrSEPARATOR}";
            strText += flObject.BlnOnlyWhenSeen;

            return strText;
        }

        /// <summary>
        /// Create an object from a line of text
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static Folder CreateObjectFromText(string strText, bool blnOnlyRead)
        {
            string[] tab_strObject = strText.Split(Main.chrSEPARATOR);

            Folder flNewFolder = new Folder()
            {
                IntID = Convert.ToInt32(tab_strObject[0]),
                IntDimensionID = Convert.ToInt32(tab_strObject[1]),
                StrName = tab_strObject[2],
                BlnLoop = Convert.ToBoolean(tab_strObject[3]),
                BlnComeBack = Convert.ToBoolean(tab_strObject[4]),
                BlnOnlyWhenSeen = Convert.ToBoolean(tab_strObject[5]),
            };

            return flNewFolder;
        }
        #endregion

        #region Static Constructor
        /// <summary>
        /// constructor of the static part of the class
        /// </summary>
        static Folder()
        {
            IntNextID = 0;
            List_flFolders = new List<Folder>();
        }
        #endregion

        //declare the events
        public event dlgPersoEvent evtValuesChanged;                //triggered when visual values are changed

        //declare the properties
        public int IntID { get; set; }                              //contain the ID of the object
        public ObstacleGroup ObsgrObstacles { get; set; }           //contain the group of obstacles
        public SpecialGroup SpcgrSpecials { get; set; }             //contain the special objects
        public MoveGroup MvgrMoves { get; set; }                    //contain the moves
        public bool BlnLoop { get; set; }                           //does the group loop after having finished every moves or having come back
        public bool BlnOnlyWhenSeen { get; set; }                   //is the group only active when seen
        public bool BlnComeBack { get; set; }                       //does the group come back after having finished every moves
        public int IntDimensionID { get; set; }                     //contain the dimension
        public Thread ThrPartMoves { get; set; }                    //contain the thread that moves the parts
        public Thread ThrPicBxMoves { get; set; }                   //contain the thread that moves the pictureboxes

        //declare the fields
        private string _strName;                                    //contain the name of the folder
        private int _intCurrentMove;                                //contain the index of the current move
        private bool _blnComingBack;                                //contain the information about if the group is comming back
        private bool _blnIsMoving;                                  //contain the state of the moves

        #region Properties
        /// <summary>
        /// Get and set the name of the object
        /// </summary>
        public string StrName
        {
            get => _strName;

            set
            {
                if (!FlgrParent.IsFolderNameTaken(value, StrName))
                {
                    _strName = value;
                    evtValuesChanged?.Invoke(IntID, this, GetType());
                }
            }
        }

        /// <summary>
        /// Contain the parent
        /// </summary>
        public FolderGroup FlgrParent
        {
            get => Dimension.List_dimDimensions[Dimension.GetIndexFromID(IntDimensionID)].FlgrFolderGroup;
        }

        /// <summary>
        /// Get the world
        /// </summary>
        private World WrldInWorld
        {
            get => FlgrParent.DimParent.DimgrParent.WrldParent;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Set the default values
        /// </summary>
        /// <param name="strName_"></param>
        public Folder(string strName_, int intDimensionID_)
        {
            IntDimensionID = intDimensionID_;

            //tell the game to use another ID next time
            IntID = IntNextID++;

            //set the default values
            this._strName = strName_;
            ObsgrObstacles = new ObstacleGroup("Obstacles", this);
            SpcgrSpecials = new SpecialGroup("Special Objects", this);
            MvgrMoves = new MoveGroup("Moves", this);

            //set the events
            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }

        /// <summary>
        /// Create a folder from a file
        /// </summary>
        public Folder()
        {
            //initialize the objects
            ObsgrObstacles = new ObstacleGroup("Obstacles", this);
            SpcgrSpecials = new SpecialGroup("Special Objects", this);
            MvgrMoves = new MoveGroup("Moves", this);

            //set the events
            evtValuesChanged += new dlgPersoEvent(UpdateTreeView);
        }
        #endregion

        #region Adding and Removing
        /// <summary>
        /// Add a new obstacle
        /// </summary>
        /// <param name="posNewPosition"></param>
        public void AddObstacle(int intType, Position posNewPosition) => ObsgrObstacles.AddObstacle(intType, posNewPosition);

        /// <summary>
        /// Add a new special
        /// </summary>
        /// <param name="intID_"></param>
        /// <param name="intX_"></param>
        /// <param name="intY_"></param>
        public void AddSpecial(int intType_, int intX_, int intY_) => SpcgrSpecials.AddSpecial(intType_, intX_, intY_);

        /// <summary>
        /// Remove all objects in the folder
        /// </summary>
        public void RemoveAll()
        {
            string strSaveLog = "";
            bool blnDoLog = false;

            for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                if (Obstacle.List_obsObstacles[i].IntFolderID == IntID)
                {
                    strSaveLog += Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[i]) + "\n";
                    blnDoLog = true;
                }

            for (int i = 0; i < Special.List_spcSpecials.Count; i++)
                if (Special.List_spcSpecials[i].IntFolderID == IntID)
                {
                    strSaveLog += Special.CreateTextFromObject(Special.List_spcSpecials[i]) + "\n";
                    blnDoLog = true;
                }

            ObsgrObstacles.RemoveAll();
            SpcgrSpecials.RemoveAll();

            if (blnDoLog)
                Log.AddLog(ObjectType.SpecialPlusObstacle, EditingType.RemoveGroup, strSaveLog, null);
        }

        /// <summary>
        /// Remove every selected object
        /// </summary>
        public void RemoveSelected()
        {
            string strSaveLog = "";
            bool blnDoLog = false;

            for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                if (Obstacle.List_obsObstacles[i].BlnSelected)
                {
                    strSaveLog += Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[i]) + "\n";
                    blnDoLog = true;
                }

            for (int i = 0; i < Special.List_spcSpecials.Count; i++)
                if (Special.List_spcSpecials[i].BlnSelected)
                {
                    strSaveLog += Special.CreateTextFromObject(Special.List_spcSpecials[i]) + "\n";
                    blnDoLog = true;
                }

            ObsgrObstacles.RemoveSelected();
            SpcgrSpecials.RemoveSelected();

            if (blnDoLog)
                Log.AddLog(ObjectType.SpecialPlusObstacle, EditingType.RemoveGroup, strSaveLog, null);
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
            if (!ObsgrObstacles.TestEmptiness(posTested))
                return false;

            if (!SpcgrSpecials.TestEmptiness(posTested))
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
            if (!ObsgrObstacles.TestEmptiness(posTested, intExeption, blnIsObstacle))
                return false;

            if (!SpcgrSpecials.TestEmptiness(posTested, intExeption, blnIsObstacle))
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
            if (!ObsgrObstacles.TestEmptiness(posTested, blnSelection))
                return false;

            if (!SpcgrSpecials.TestEmptiness(posTested, blnSelection))
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
            ObsgrObstacles.SelectAll();
            SpcgrSpecials.SelectAll();
        }

        /// <summary>
        /// Unselect all
        /// </summary>
        public void UnselectAll()
        {
            ObsgrObstacles.UnselectAll();
            SpcgrSpecials.UnselectAll();
        }

        /// <summary>
        /// Select all the obtacles present inside the selection area
        /// </summary>
        public void SelectArea(Position posArea, bool blnSelect) 
        { 
            ObsgrObstacles.SelectArea(posArea, blnSelect);
            SpcgrSpecials.SelectArea(posArea, blnSelect);
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get a node containg the node of the group
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the new node
            List<TreeNode> list_trnFolder = new List<TreeNode>
            {
                ObsgrObstacles.GetTreeNode(),
                SpcgrSpecials.GetTreeNode(),
                MvgrMoves.GetTreeNode()
            };     //contain the main nodes

            //return the new node
            return new TreeNode(StrName, list_trnFolder.ToArray()) { ContextMenuStrip = GetContextMenu(), Tag = IntID };
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
            List<ToolStripMenuItem> tsmiItems = new List<ToolStripMenuItem>
            {

                //tool made to select the object
                new ToolStripMenuItem("Select All")
            };      //contain all the tools
            tsmiItems[0].Tag = IntID;
            tsmiItems[0].Click += new EventHandler(SelectAllMenu);

            //tool made to unselect the object
            tsmiItems.Add(new ToolStripMenuItem("Unselect All"));
            tsmiItems[1].Tag = IntID;
            tsmiItems[1].Click += new EventHandler(UnselectAllMenu);

            //tool made to unselect the object
            tsmiItems.Add(new ToolStripMenuItem("Select"));
            tsmiItems[2].Tag = IntID;
            tsmiItems[2].Click += new EventHandler(SelectMenu);

            //tool made to start the movements of the the object
            tsmiItems.Add(new ToolStripMenuItem("Start Demo"));
            tsmiItems[3].Tag = IntID;
            tsmiItems[3].Click += new EventHandler(StartMenu);

            //tool made to stop the movements of the the object
            tsmiItems.Add(new ToolStripMenuItem("Stop Demo"));
            tsmiItems[4].Tag = IntID;
            tsmiItems[4].Click += new EventHandler(StopMenu);

            //tool made to remove the object
            tsmiItems.Add(new ToolStripMenuItem("Remove"));
            tsmiItems[5].Tag = IntID;
            tsmiItems[5].Click += new EventHandler(RemoveFolderMenu);

            //tool made to see and change the properties of the object
            tsmiItems.Add(new ToolStripMenuItem("Properties"));
            tsmiItems[6].Tag = IntID;
            tsmiItems[6].Click += new EventHandler(ModifyPropertiesMenu);

            //add the tools to the context box
            contextMenuStrip.Items.AddRange(tsmiItems.ToArray());

            return contextMenuStrip;
        }

        #region Events
        /// <summary>
        /// Remove the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFolderMenu(object sender, EventArgs e)
        {
            //search for the obstacle
            for (int i = 0; i < List_flFolders.Count; i++)
                if (List_flFolders[i].IntID == IntID)
                {
                    FlgrParent.Remove(i);

                    break;
                }
        }

        /// <summary>
        /// Select all the objects inside the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllMenu(object sender, EventArgs e) => SelectAll();

        /// <summary>
        /// Unselect all the objects inside the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnselectAllMenu(object sender, EventArgs e) => UnselectAll();

        /// <summary>
        /// Select to add object in this folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectMenu(object sender, EventArgs e) => FlgrParent.IntSelectedFolderID = IntID;

        /// <summary>
        /// Start the movements of the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartMenu(object sender, EventArgs e) => StartMoving();

        /// <summary>
        /// Stop the movements of the folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopMenu(object sender, EventArgs e) => StopMoving();

        /// <summary>
        /// Open the properties form of the object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyPropertiesMenu(object sender, EventArgs e) => OpenPropertiesForm();
        #endregion
        #endregion

        #region Moves
        /// <summary>
        /// Loop moving the parts
        /// </summary>
        private void MoveParts()
        {
            //loop till the moving is stopped
            while (true)
            {
                //test if the move is not finished
                if (MvgrMoves.List_frmvMoves[_intCurrentMove]._intTickLeft != 0)
                {
                    //test if the group is not comming back
                    if (!_blnComingBack)
                    {
                        //add a tick
                        MvgrMoves.List_frmvMoves[_intCurrentMove].AddTick();

                        //search in the parts
                        for (int i = 0; i < ObsgrObstacles.List_obsObstacles.Count; i++)
                            //move the part
                            ObsgrObstacles.List_obsObstacles[i].AddToLocation(MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblWidth, MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblHeight);
                        //search in the parts
                        for (int i = 0; i < SpcgrSpecials.List_spcSpecials.Count; i++)
                            //move the part
                            SpcgrSpecials.List_spcSpecials[i].AddToLocation(MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblWidth, MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblHeight);
                    }
                    else
                    {
                        //add a tick
                        MvgrMoves.List_frmvMoves[_intCurrentMove].AddTick();

                        //move the obstacles
                        ObsgrObstacles.AddToLocation(MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblWidth * -1, MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblHeight * -1);
                        SpcgrSpecials.AddToLocation(MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblWidth * -1, MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistancePerTick.dblHeight * -1);
                    }
                }
                else
                {
                    //test if the group is not comming back
                    if (!_blnComingBack)
                    {
                        //search in the parts
                        for (int i = 0; i < ObsgrObstacles.List_obsObstacles.Count; i++)
                            //move the parts
                            ObsgrObstacles.List_obsObstacles[i].SetLocation(ObsgrObstacles.List_obsObstacles[i].GetTempX() - MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblWidth + MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblWidth, ObsgrObstacles.List_obsObstacles[i].GetTempY() - MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblHeight + MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblHeight);
                        //search in the parts
                        for (int i = 0; i < SpcgrSpecials.List_spcSpecials.Count; i++)
                            //move the parts
                            SpcgrSpecials.List_spcSpecials[i].SetLocation(SpcgrSpecials.List_spcSpecials[i].GetTempX() - MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblWidth + MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblWidth, SpcgrSpecials.List_spcSpecials[i].GetTempY() - MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblHeight + MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblHeight);

                        //reset the move
                        MvgrMoves.List_frmvMoves[_intCurrentMove].Reset();

                        //start the delay
                        Thread.Sleep(MvgrMoves.List_frmvMoves[_intCurrentMove].IntTickStop * Main.intTICK);

                        //test if the current move is the last of the list
                        if (_intCurrentMove == MvgrMoves.IntCount - 1)
                        {
                            //test if the group can come back
                            if (BlnComeBack)
                            {
                                //tell the game the group is comming back
                                _blnComingBack = true;
                                _intCurrentMove = MvgrMoves.IntCount - 1;
                            }
                            else
                            {
                                //test if the group loops
                                if (BlnLoop)
                                    //continue the moving
                                    ResetPartsPos();
                                else
                                    //stop the moving
                                    StopMoving();
                            }
                        }
                        else
                            _intCurrentMove++;
                    }
                    else
                    {
                        //search in the parts
                        for (int i = 0; i < ObsgrObstacles.List_obsObstacles.Count; i++)
                            //move the part
                            ObsgrObstacles.List_obsObstacles[i].SetLocation(ObsgrObstacles.List_obsObstacles[i].GetTempX() + MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblWidth - MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblWidth, ObsgrObstacles.List_obsObstacles[i].GetTempY() + MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblHeight - MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblHeight);
                        //search in the parts
                        for (int i = 0; i < SpcgrSpecials.List_spcSpecials.Count; i++)
                            //move the part
                            SpcgrSpecials.List_spcSpecials[i].SetLocation(SpcgrSpecials.List_spcSpecials[i].GetTempX() + MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblWidth - MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblWidth, SpcgrSpecials.List_spcSpecials[i].GetTempY() + MvgrMoves.List_frmvMoves[_intCurrentMove]._disCurrentLocation.dblHeight - MvgrMoves.List_frmvMoves[_intCurrentMove].DisDistance.dblHeight);

                        //reset the move
                        MvgrMoves.List_frmvMoves[_intCurrentMove].Reset();

                        //start the delay
                        Thread.Sleep(MvgrMoves.List_frmvMoves[_intCurrentMove].IntTickStop * Main.intTICK);

                        //test if the move is the last one
                        if (_intCurrentMove - 1 == -1)
                        {
                            //test if the group can loop
                            if (BlnLoop)
                                //continue the moving
                                ResetPartsPos();
                            else
                                //reset the group
                                StopMoving();
                        }
                        else
                        {
                            //tell the game to pass to another move
                            _intCurrentMove--;
                        }
                    }
                }

                if (!_blnIsMoving)
                {
                    ResetPartsPos();
                    break;
                }

                //delay the group
                Thread.Sleep(Main.intTICK);
            }
        }

        /// <summary>
        /// Start the moving of the group
        /// </summary>
        public void StartMoving()
        {
            //test if there are moves
            if (MvgrMoves.List_frmvMoves.Count > 0)
            {
                //reset the group
                ResetPartsPos();

                //stop the thread erros
                Control.CheckForIllegalCrossThreadCalls = false;

                //tell the game the group is moving
                _blnIsMoving = true;

                //reset the threads
                ThrPartMoves = new Thread(MoveParts);
                ThrPicBxMoves = new Thread(MovingPicBx);

                //start the threads
                ThrPartMoves.Start();
                ThrPicBxMoves.Start();
            }
        }

        /// <summary>
        /// Reset the moving
        /// </summary>
        public void ResetPartsPos()
        {
            //reset the values
            _blnComingBack = false;
            _intCurrentMove = 0;

            //reset the moves
            for (int i = 0; i < MvgrMoves.List_frmvMoves.Count; i++)
                MvgrMoves.List_frmvMoves[i].Reset();

            //tell the game the move currently in charge is the first in the list
            if (MvgrMoves.List_frmvMoves.Count > 0)
                MvgrMoves.List_frmvMoves[_intCurrentMove].BlnIsMoving = true;

            //reset the locations
            for (int i = 0; i < ObsgrObstacles.List_obsObstacles.Count; i++)
                ObsgrObstacles.List_obsObstacles[i].SetLocation(ObsgrObstacles.List_obsObstacles[i].IntX, ObsgrObstacles.List_obsObstacles[i].IntY);
            //reset the locations
            for (int i = 0; i < SpcgrSpecials.List_spcSpecials.Count; i++)
                SpcgrSpecials.List_spcSpecials[i].SetLocation(SpcgrSpecials.List_spcSpecials[i].IntX, SpcgrSpecials.List_spcSpecials[i].IntY);
        }

        /// <summary>
        /// Move the picturebox
        /// </summary>
        private void MovingPicBx()
        {
            //test if the group is moving
            while (true)
            {
                //search in the parts and test if the group is moving
                for (int i = 0; i < ObsgrObstacles.List_obsObstacles.Count; i++)
                    ObsgrObstacles.List_obsObstacles[i].UpdatePictureBox();
                for (int i = 0; i < SpcgrSpecials.List_spcSpecials.Count; i++)
                    SpcgrSpecials.List_spcSpecials[i].UpdatePictureBox();

                if (!_blnIsMoving)
                    break;

                //delay the thread
                Thread.Sleep(Main.intTICK);
            }

            //reset the default locations
            for (int i = 0; i < ObsgrObstacles.List_obsObstacles.Count; i++)
                ObsgrObstacles.List_obsObstacles[i].SetLocation(ObsgrObstacles.List_obsObstacles[i].IntX, ObsgrObstacles.List_obsObstacles[i].IntY);
            for (int i = 0; i < SpcgrSpecials.List_spcSpecials.Count; i++)
                SpcgrSpecials.List_spcSpecials[i].SetLocation(SpcgrSpecials.List_spcSpecials[i].IntX, SpcgrSpecials.List_spcSpecials[i].IntY);
        }

        /// <summary>
        /// Stop the moving
        /// </summary>
        public void StopMoving()
        {
            //tell the game the group isn't moving anymore
            _blnIsMoving = false;

            //reset the group
            ResetPartsPos();
        }
        #endregion

        #region Properties Form
        //declare all the control variables
        private Form _frmFolder;                            //properties form
        private TextBox _txtbxName;                         //name textbox
        private TextBox _txtbxNbObstacles;                  //number of obstacles textbox
        private Label _lblNbObstacles;                      //number of obstacles label
        private Label _lblName;                             //name label
        private Button _btnCancel;                          //cancel button
        private Button _btnValidate;                        //validation button

        /// <summary>
        /// Open the properties form of the selected object group
        /// </summary>
        /// <param name="intIndex"></param>
        public void OpenPropertiesForm()
        {
            //reset the controls
            _frmFolder = new Form();
            _txtbxName = new TextBox();
            _txtbxNbObstacles = new TextBox();
            _lblNbObstacles = new Label();
            _lblName = new Label();
            _btnCancel = new Button();
            _btnValidate = new Button();

            #region Controls
            //suspend the layout
            _frmFolder.SuspendLayout();

            //contain the name of the object group
            _txtbxName.Location = new Point(100, 43);
            _txtbxName.MaxLength = 20;
            _txtbxName.Name = "txtbxName";
            _txtbxName.Size = new Size(138, 20);
            _txtbxName.TabIndex = 0;
            _txtbxName.Text = $"{StrName}";
            _txtbxName.TextAlign = HorizontalAlignment.Right;

            //contain the number of obstacles
            _txtbxNbObstacles.Location = new Point(202, 69);
            _txtbxNbObstacles.Name = "txtbxNbObstacles";
            _txtbxNbObstacles.Size = new Size(36, 20);
            _txtbxNbObstacles.TabIndex = 2;
            _txtbxNbObstacles.Text = $"{ObsgrObstacles.List_obsObstacles.Count}";
            _txtbxNbObstacles.TextAlign = HorizontalAlignment.Right;
            _txtbxNbObstacles.Enabled = false;

            //inform the user the textbox next to it contain the number of obstacles
            _lblNbObstacles.AutoSize = true;
            _lblNbObstacles.Location = new Point(36, 69);
            _lblNbObstacles.Name = "lblNbObstacles";
            _lblNbObstacles.Size = new Size(44, 13);
            _lblNbObstacles.TabIndex = 4;
            _lblNbObstacles.Text = "Number of Obstacles :";

            //inform the user the textbox next to it contain the name
            _lblName.AutoSize = true;
            _lblName.Location = new Point(36, 43);
            _lblName.Name = "lblName";
            _lblName.Size = new Size(41, 13);
            _lblName.TabIndex = 6;
            _lblName.Text = "Name :";

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
            _btnValidate.Tag = IntID;
            _btnValidate.UseVisualStyleBackColor = true;
            _btnValidate.Click += new EventHandler(Validate_Click);

            //form containing the controls
            _frmFolder.AutoScaleDimensions = new SizeF(6F, 13F);
            _frmFolder.AutoScaleMode = AutoScaleMode.Font;
            _frmFolder.AcceptButton = _btnValidate;
            _frmFolder.CancelButton = _btnCancel;
            _frmFolder.ClientSize = new Size(304, 125);
            _frmFolder.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            _frmFolder.Name = "frmFolder";
            _frmFolder.StartPosition = FormStartPosition.CenterParent;
            _frmFolder.Text = "Folder";
            _frmFolder.Tag = IntID;

            //add the controls
            _frmFolder.Controls.Add(_btnValidate);
            _frmFolder.Controls.Add(_btnCancel);
            _frmFolder.Controls.Add(_lblName);
            _frmFolder.Controls.Add(_lblNbObstacles);
            _frmFolder.Controls.Add(_txtbxNbObstacles);
            _frmFolder.Controls.Add(_txtbxName);

            //resume layout
            _frmFolder.ResumeLayout(false);
            _frmFolder.PerformLayout();
            #endregion

            //show the form
            _frmFolder.ShowDialog();
        }

        #region Events
        /// <summary>
        /// Close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, EventArgs e) => _frmFolder.Close();

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

            string strName_ = _txtbxName.Text;       //get the new name

            #region Verify Validity
            //test if the new name is taken
            if (FlgrParent.IsFolderNameTaken($"{strName_}", StrName))
            {
                //inform the game the value is invalid
                blnAreAllValuesValid = false;

                //inform the user the value is invalid
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
            #endregion

            //test if all values are valid
            if (blnAreAllValuesValid)
            {
                string strSaveLog = CreateTextFromObject(this);

                //set the new name
                StrName = strName_;

                string strResultLog = CreateTextFromObject(this);

                Log.AddLog(ObjectType.Folder, EditingType.ChangeProperties, strSaveLog, strResultLog);

                //close the form
                _frmFolder.Close();

                //dispose the form of all data
                _frmFolder.Dispose();
            }
            else
                //show the error message
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