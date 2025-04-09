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
** This class is a group of special types, it manages the special types      **
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
    /// This class is a group of special types, it manages the special types
    /// </summary>
    class SpecialTypeGroup
    {
        //declare the events
        public event dlgPersoEvent evtValuesChanged;                    //triggered when visual values are changed

        //declare the fields
        private string _strName;                                        //contain the name of the group
        private readonly World _wrldParent;                             //contain the parent

        #region Properties
        /// <summary>
        /// Contain all the special objects of the group
        /// </summary>
        public List<SpecialType> List_spctypTypes
        {
            get
            {
                List<SpecialType> list_objects_ = new List<SpecialType>();

                for (int i = 0; i < SpecialType.List_spctypTypes.Count; i++)
                    if (SpecialType.List_spctypTypes[i].IntWorldID == WrldParent.IntID)
                        list_objects_.Add(SpecialType.List_spctypTypes[i]);

                return list_objects_;
            }
        }

        /// <summary>
        /// Indexer of the list of types
        /// </summary>
        /// <param name="intID"></param>
        /// <returns></returns>
        public SpecialType this[int intID]
        {
            get
            {
                for (int i = 0; i < List_spctypTypes.Count; i++)
                    if (List_spctypTypes[i].IntID == intID)
                        return List_spctypTypes[i];

                return null;
            }

            set
            {
                for (int i = 0; i < List_spctypTypes.Count; i++)
                    if (List_spctypTypes[i].IntID == intID)
                        List_spctypTypes[i] = value;
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
        public SpecialTypeGroup(string strName_, World wrldParent_)
        {
            this._wrldParent = wrldParent_;
            StrName = strName_;

            AddType("Door", 1, 2, false, true, false, false, true, 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\TravelDoor1x2.png"), ImageLayout.Tile);
            AddType("Flag", 1, 8, false, false, false, true, true, 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\Flag1x8.png"), ImageLayout.Tile);

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
            for (int i = 0; i < List_spctypTypes.Count; i++)
                if (List_spctypTypes[i].IntID == intID)
                    return i;

            return -1;
        }

        /// <summary>
        /// Return the ID of the object targeted by an ID
        /// </summary>
        /// <param name="intIndex"></param>
        /// <returns></returns>
        public int GetIDFromIndex(int intIndex) => List_spctypTypes[intIndex].IntID;
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
            for (int i = 0; i < List_spctypTypes.Count; i++)
                //test if the name is taken
                if (List_spctypTypes[i].StrName == strNewName && strOldName != strNewName)
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
        public void AddType(string strName, int intWidth, int intHeight, bool blnKill, bool blnTravel, bool blnActFolder, bool blnWin, bool blnEnter, int intSolidness, int intBounciness, bool blnDoBounce, Image imgTexture, ImageLayout imglytLayout)
        {
            //declare the counter 
            int intI = 1;                           //add a number each time the name was taken
            string strNewName = strName;

            if (strName == null)
                //try a new name
                do
                    //set the new attempted name
                    strNewName = $"Type {intI++}";
                //if the name is taken try again
                while (IsTypeNameTaken(strNewName, null));

            //add the new dimension using the automaticly generated name with the given size
            SpecialType.List_spctypTypes.Add(new SpecialType(strNewName, intWidth, intHeight, blnKill, blnTravel, blnActFolder, blnWin,blnEnter, intSolidness, intBounciness, blnDoBounce, imgTexture, imglytLayout, WrldParent.IntID));

            Log.AddLog(ObjectType.SpecialType, EditingType.Add, null, SpecialType.CreateTextFromObject(SpecialType.List_spctypTypes[SpecialType.List_spctypTypes.Count - 1]), null, SpecialType.List_spctypTypes[SpecialType.List_spctypTypes.Count - 1].ImgTexture);

            evtValuesChanged?.Invoke(-1, this, GetType());
        }

        /// <summary>
        /// Remove the selected dimension
        /// </summary>
        /// <param name="intIndex"></param>
        public void Remove(int intIndex)
        {
            string strSaveLog = SpecialType.CreateTextFromObject(SpecialType.List_spctypTypes[intIndex]);
            string strText = "";
            bool blnDoLog = false;
            Image imgSave = null;

            if (SpecialType.List_spctypTypes[intIndex].ImgTexture != null)
                imgSave = new Bitmap(SpecialType.List_spctypTypes[intIndex].ImgTexture);

            if (SpecialType.List_spctypTypes.Count > 1)
            {
                string strSaveLogTyp = SpecialType.CreateTextFromObject(SpecialType.List_spctypTypes[intIndex]);

                for (int i = 0; i < Special.List_spcSpecials.Count; i++)
                    if (Special.List_spcSpecials[i].IntSpecialTypeID == List_spctypTypes[intIndex].IntID)
                    {
                        strText += Special.CreateTextFromObject(Special.List_spcSpecials[i]) + "\n";
                        blnDoLog = true;
                        Special.List_spcSpecials[i].SpcgrParent.Remove(i);
                        i--;
                    }

                if (blnDoLog)
                    Log.AddLog(ObjectType.SpecialPlusObstacle, EditingType.RemoveGroup, strText, null);

                SpecialType.List_spctypTypes.RemoveAt(intIndex);

                WrldParent.IntSpecialTypeID = List_spctypTypes[0].IntID;

                Log.AddLog(ObjectType.SpecialType, EditingType.Remove, strSaveLog, null);

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
            for (int i = 0; i < SpecialType.List_spctypTypes.Count; i++)
                if (SpecialType.List_spctypTypes[i].IntWorldID == WrldParent.IntID)
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

            for (int i = 0; i < List_spctypTypes.Count; i++)
                list_trnTypes.Add(List_spctypTypes[i].GetTreeNode());

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
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();                 //declare the contextbox control
            List<ToolStripMenuItem> list_tsmiMain = new List<ToolStripMenuItem>();      //declare the list of tools

            //add the new tool used to add a dimension
            list_tsmiMain.Add(new ToolStripMenuItem("Add Special Type"));
            list_tsmiMain[0].Click += new EventHandler(AddSpecialType_Click);
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
        private void AddSpecialType_Click(object sender, EventArgs e)
        {
            AddType(null, 1, 1, false, false, false, false, false, 100, 100, true, Image.FromFile(Main.StrImagesPath + @"\" + "Brick1x1.png"), ImageLayout.Tile);

            //open a properties form used to modify the properties of the last added dimension
            List_spctypTypes[List_spctypTypes.Count - 1].OpenPropertiesForm();
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