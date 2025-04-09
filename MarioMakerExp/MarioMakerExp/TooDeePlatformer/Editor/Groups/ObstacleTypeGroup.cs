/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 17.06.2021                                                    **
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
** This class is a group of obstacles type, it manages the obstacles types   **
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
    /// This class is a group of obstacles type, it manages the obstacles types
    /// </summary>
    class ObstacleTypeGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                     //triggered when visual values are changed

        //declare the fields
        private string _strName;                                        //contain the name of the group
        private readonly World _wrldParent;                             //contain the parent

        #region Properties
        /// <summary>
        /// Contain all the obstacle types of the group
        /// </summary>
        public List<ObstacleType> List_obstypTypes
        {
            get
            {
                List<ObstacleType> list_objects_ = new List<ObstacleType>();

                for (int i = 0; i < ObstacleType.List_obstypTypes.Count; i++)
                    if (ObstacleType.List_obstypTypes[i].IntWorldID == WrldParent.IntID)
                        list_objects_.Add(ObstacleType.List_obstypTypes[i]);

                return list_objects_;
            }
        }

        /// <summary>
        /// Indexer of the list of types
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public ObstacleType this[int intID]
        {
            get
            {
                for (int i = 0; i < List_obstypTypes.Count; i++)
                    if (List_obstypTypes[i].IntID == intID)
                        return List_obstypTypes[i];

                return null;
            }

            set
            {
                for (int i = 0; i < List_obstypTypes.Count; i++)
                    if (List_obstypTypes[i].IntID == intID)
                        List_obstypTypes[i] = value;
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
        public ObstacleTypeGroup(string strName_, World wrldParent_)
        {
            StrName = strName_;
            this._wrldParent = wrldParent_;

            AddType("Brick", 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\Brick1x1.png"), ImageLayout.Tile);
            AddType("Grass", 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\Grass1x1.png"), ImageLayout.Tile);
            AddType("Dirt", 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\Dirt1x1.png"), ImageLayout.Tile);

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
        public int GetIDFromIndex(int intIndex) => List_obstypTypes[intIndex].IntID;
        #endregion

        #region TestName
        /// <summary>
        /// Test if the name is taken
        /// </summary>
        /// <param name="strNewName"></param>
        /// <param name="strOldName"></param>
        /// <returns></returns>
        public bool IsTypeNameTaken(string strNewName, string strOldName)
        {
            //search in the moves
            for (int i = 0; i < List_obstypTypes.Count; i++)
                //test if the name is taken
                if (List_obstypTypes[i].StrName == strNewName && strOldName != strNewName)
                    return true;

            return false;
        }
        #endregion

        #region Adding and removing
        /// <summary>
        /// Add a dimension
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        public void AddType(string strName, int intSolidness, int intBounciness, bool blnDoBounce, Image imgTexture, ImageLayout imglytLayout)
        {
            //declare the counter 
            int intI = 1;                           //add a number each time the name was taken
            string strNewName;

            if (strName == null)
            {
                //try a new name
                do
                    strNewName = $"Type {intI++}";
                //if the name is taken try again
                while (IsTypeNameTaken(strNewName, null));
            }
            else
                strNewName = strName;

            //add the new dimension using the automaticly generated name with the given size
            ObstacleType.List_obstypTypes.Add(new ObstacleType(strNewName, intSolidness, intBounciness, blnDoBounce, imgTexture, imglytLayout, WrldParent.IntID));

            Log.AddLog(ObjectType.ObstacleType, EditingType.Add, null, ObstacleType.CreateTextFromObject(ObstacleType.List_obstypTypes[ObstacleType.List_obstypTypes.Count - 1]), null, ObstacleType.List_obstypTypes[ObstacleType.List_obstypTypes.Count - 1].ImgTexture);

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove the selected dimension
        /// </summary>
        /// <param name="intIndex"></param>
        public void Remove(int intIndex)
        {
            string strSaveLog = ObstacleType.CreateTextFromObject(ObstacleType.List_obstypTypes[intIndex]);
            string strText = "";
            bool blnDoLog = false;
            Image imgSave = null;

            if (ObstacleType.List_obstypTypes[intIndex].ImgTexture != null)
                imgSave = new Bitmap(ObstacleType.List_obstypTypes[intIndex].ImgTexture);

            if (ObstacleType.List_obstypTypes.Count > 1)
            {
                for (int i = 0; i < Obstacle.List_obsObstacles.Count; i++)
                    if (Obstacle.List_obsObstacles[i].IntTypeID == ObstacleType.List_obstypTypes[intIndex].IntID)
                    {
                        strText += Obstacle.CreateTextFromObject(Obstacle.List_obsObstacles[i]) + "\n";
                        blnDoLog = true;
                        Obstacle.List_obsObstacles[i].ObsgrParent.Remove(Obstacle.List_obsObstacles[i].ObsgrParent.GetIndexFromID(Obstacle.List_obsObstacles[i].IntID));
                        i--;
                    }

                if (blnDoLog)
                    Log.AddLog(ObjectType.SpecialPlusObstacle, EditingType.RemoveGroup, strText, null);

                //remove the dimension of the list
                ObstacleType.List_obstypTypes.RemoveAt(intIndex);

                WrldParent.IntObstacleTypeID = List_obstypTypes[0].IntID;

                Log.AddLog(ObjectType.ObstacleType, EditingType.Remove, strSaveLog, null);

                evtValuesChanged?.Invoke(-1, this, GetType());
            }
            else
                MessageBox.Show("You must keep at least one type !");
        }

        /// <summary>
        /// Remove all objects
        /// </summary>
        public void RemoveAll()
        {
            for (int i = 0; i < ObstacleType.List_obstypTypes.Count; i++)
                if (ObstacleType.List_obstypTypes[i].IntWorldID == WrldParent.IntID)
                    Remove(i--);
        }
        #endregion

        #region TreeNode
        /// <summary>
        /// Get the world node
        /// </summary>
        public TreeNode GetTreeNode()
        {
            //declare the list of world node
            List<TreeNode> list_trnTypes = new List<TreeNode>();

            for (int i = 0; i < List_obstypTypes.Count; i++)
                list_trnTypes.Add(List_obstypTypes[i].GetTreeNode());

            //retrun the world node
            return new TreeNode(StrName, list_trnTypes.ToArray()) { ContextMenuStrip = GetContextMenu() };
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
            list_tsmiMain.Add(new ToolStripMenuItem("Add Obstacle Type"));
            list_tsmiMain[0].Click += new EventHandler(AddObstacleType_Click);
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
        /// Method used the add a type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddObstacleType_Click(object sender, EventArgs e)
        {
            AddType(null, 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\" + "Brick1x1.png"), ImageLayout.Tile);

            //open a properties form used to modify the properties of the last added dimension
            List_obstypTypes[List_obstypTypes.Count - 1].OpenPropertiesForm();
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
        #endregion
    }
}