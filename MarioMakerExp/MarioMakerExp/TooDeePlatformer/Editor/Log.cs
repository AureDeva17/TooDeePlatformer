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
** Log all the actions made by the user the change the world                 **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Log all the actions made by the user the change the world
    /// </summary>
    class Log
    {
        //declare the static properties
        private const int intMAXLOGS = 30;                          //max number of logs
        private static List<Log> List_lgLogs { get; set; }          //contains all the logs
        private static int IntCurrentLog { get; set; }              //contains the current state of the editing

        #region Static Constructor
        /// <summary>
        /// Set the default values of the static variables and properties
        /// </summary>
        static Log()
        {
            List_lgLogs = new List<Log>();
            IntCurrentLog = -1;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Add a new log to the list of logs
        /// </summary>
        /// <param name="obtType"></param>
        /// <param name="edtEdit"></param>
        /// <param name="strSave"></param>
        public static void AddLog(ObjectType obtType, EditingType edtEdit, string strSave, string strResult)
        {
            if (List_lgLogs.Count > 0)
                for (int i = IntCurrentLog + 1; i < List_lgLogs.Count; i++)
                    List_lgLogs.RemoveAt(IntCurrentLog + 1);

            List_lgLogs.Add(new Log(obtType, edtEdit, strSave, strResult));
            IntCurrentLog++;

            if (List_lgLogs.Count >= intMAXLOGS)
            {
                List_lgLogs.RemoveAt(0);
                IntCurrentLog--;
            }
        }

        /// <summary>
        /// Add a new log to the list of logs with images
        /// </summary>
        /// <param name="obtType"></param>
        /// <param name="edtEdit"></param>
        /// <param name="objChanged"></param>
        /// <param name="strSave"></param>
        /// <param name="strResult"></param>
        /// <param name="imgSave"></param>
        /// <param name="imgResult"></param>
        public static void AddLog(ObjectType obtType, EditingType edtEdit, string strSave, string strResult, Image imgSave, Image imgResult)
        {
            if (List_lgLogs.Count > 0)
                for (int i = IntCurrentLog + 1; i < List_lgLogs.Count; i++)
                    List_lgLogs.RemoveAt(IntCurrentLog + 1);

            if (imgSave != null && imgResult != null)
                List_lgLogs.Add(new Log(obtType, edtEdit, strSave, strResult, new Bitmap(imgSave), new Bitmap(imgResult)));
            else if (imgSave == null && imgResult != null)
                List_lgLogs.Add(new Log(obtType, edtEdit, strSave, strResult, null, new Bitmap(imgResult)));
            else if (imgSave != null && imgResult == null)
                List_lgLogs.Add(new Log(obtType, edtEdit, strSave, strResult, new Bitmap(imgSave), null));
            else if (imgSave == null && imgResult == null)
                List_lgLogs.Add(new Log(obtType, edtEdit, strSave, strResult, null, null));

            IntCurrentLog++;

            if (List_lgLogs.Count >= intMAXLOGS)
            {
                List_lgLogs.RemoveAt(0);
                IntCurrentLog--;
            }
        }

        /// <summary>
        /// Remove all logs
        /// </summary>
        public static void RemoveAll()
        {
            List_lgLogs.Clear();

            IntCurrentLog = -1;
        }

        /// <summary>
        /// Go back one action
        /// </summary>
        public static void Cancel()
        {
            if (IntCurrentLog > -1)
            {
                switch (List_lgLogs[IntCurrentLog].EdtEdit)
                {
                    case EditingType.ChangeProperties:
                        Restore(List_lgLogs[IntCurrentLog], true);
                        break;

                    case EditingType.Add:
                        Remove(List_lgLogs[IntCurrentLog], true);
                        break;

                    case EditingType.Remove:
                        Add(List_lgLogs[IntCurrentLog], true);
                        break;

                    case EditingType.MoveGroup:
                        MoveBackGroup(List_lgLogs[IntCurrentLog], true);
                        break;

                    case EditingType.RemoveGroup:
                        AddGroup(List_lgLogs[IntCurrentLog], true);
                        break;
                }

                try
                {
                    Main.WrldWorld.DimgrDimensions.List_dimDimensions[Main.WrldWorld.DimgrDimensions.IntSelectedDimension].UnSelectAll();
                }
                catch
                {
                    MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Main.WrldWorld.UpdateTreeNodes();
                IntCurrentLog--;
            }
        }

        /// <summary>
        /// Go back one cancel action
        /// </summary>
        public static void UnCancel()
        {
            if (IntCurrentLog + 1 < List_lgLogs.Count)
            {
                IntCurrentLog++;
                switch (List_lgLogs[IntCurrentLog].EdtEdit)
                {
                    case EditingType.ChangeProperties:
                        Restore(List_lgLogs[IntCurrentLog], false);
                        break;

                    case EditingType.Add:
                        Add(List_lgLogs[IntCurrentLog], false);
                        break;

                    case EditingType.Remove:
                        Remove(List_lgLogs[IntCurrentLog], false);
                        break;

                    case EditingType.MoveGroup:
                        MoveBackGroup(List_lgLogs[IntCurrentLog], false);
                        break;

                    case EditingType.RemoveGroup:
                        RemoveGroup(List_lgLogs[IntCurrentLog], false);
                        break;
                }

                try
                {
                    Main.WrldWorld.DimgrDimensions.List_dimDimensions[Main.WrldWorld.DimgrDimensions.IntSelectedDimension].UnSelectAll();
                }
                catch
                {
                    MessageBox.Show("No dimension found !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Main.WrldWorld.UpdateTreeNodes();
            }
        }

        /// <summary>
        /// Restore the object
        /// </summary>
        /// <param name="lgLog"></param>
        private static void Restore(Log lgLog, bool blnGoingBack)
        {
            switch (lgLog.ObtType)
            {
                case ObjectType.World:
                    World wrldObject;
                    World wrldTarget;

                    if (blnGoingBack)
                    {
                        wrldObject = World.CreateObjectFromText(lgLog.StrSave, true);
                        wrldTarget = World.GetObjectByID(World.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        wrldObject = World.CreateObjectFromText(lgLog.StrResult, true);
                        wrldTarget = World.GetObjectByID(World.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    wrldTarget.StrName = wrldObject.StrName;
                    wrldTarget.IntDefaultPlayerDimensionID = wrldObject.IntDefaultPlayerDimensionID;
                    wrldTarget.IntDefaultPlayerX = wrldObject.IntDefaultPlayerX;
                    wrldTarget.IntDefaultPlayerY = wrldObject.IntDefaultPlayerY;
                    break;

                case ObjectType.Obstacle:
                    Obstacle obsObject;
                    Obstacle obsTarget;

                    if (blnGoingBack)
                    {
                        obsObject = Obstacle.CreateObjectFromText(lgLog.StrSave, true);
                        obsTarget = Obstacle.GetObjectByID(Obstacle.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        obsObject = Obstacle.CreateObjectFromText(lgLog.StrResult, true);
                        obsTarget = Obstacle.GetObjectByID(Obstacle.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    obsTarget.IntFolderID = obsObject.IntFolderID;
                    obsTarget.StrName = obsObject.StrName;
                    obsTarget.IntTypeID = obsObject.IntTypeID;
                    obsTarget.IntWidth = obsObject.IntWidth;
                    obsTarget.IntHeight = obsObject.IntHeight;
                    obsTarget.IntX = obsObject.IntX;
                    obsTarget.IntY = obsObject.IntY;
                    break;

                case ObjectType.Special:
                    Special spcObject;
                    Special spcTarget;

                    if (blnGoingBack)
                    {
                        spcObject = Special.CreateObjectFromText(lgLog.StrSave, true);
                        spcTarget = Special.GetObjectByID(Special.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        spcObject = Special.CreateObjectFromText(lgLog.StrResult, true);
                        spcTarget = Special.GetObjectByID(Special.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    spcTarget.IntFolderID = spcObject.IntFolderID;
                    spcTarget.StrName = spcObject.StrName;
                    spcTarget.IntSpecialTypeID = spcObject.IntSpecialTypeID;
                    spcTarget.IntX = spcObject.IntX;
                    spcTarget.IntY = spcObject.IntY;
                    spcTarget.IntToDimensionID = spcObject.IntToDimensionID;
                    spcTarget.IntToX = spcObject.IntToX;
                    spcTarget.IntToY = spcObject.IntToY;
                    spcTarget.IntActivateFolderID = spcObject.IntActivateFolderID;
                    spcTarget.IntNumberLaps = spcObject.IntNumberLaps;
                    break;

                case ObjectType.Movement:
                    Movement mvObject;
                    Movement mvTarget;

                    if (blnGoingBack)
                    {
                        mvObject = Movement.CreateObjectFromText(lgLog.StrSave, true);
                        mvTarget = Movement.GetObjectByID(Movement.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        mvObject = Movement.CreateObjectFromText(lgLog.StrResult, true);
                        mvTarget = Movement.GetObjectByID(Movement.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    mvTarget.IntFolderID = mvObject.IntFolderID;
                    mvTarget.StrName = mvObject.StrName;
                    mvTarget.DisDistance = mvObject.DisDistance;
                    mvTarget.IntTickGiven = mvObject.IntTickGiven;
                    mvTarget.IntTickStop = mvObject.IntTickStop;
                    break;

                case ObjectType.ObstacleType:
                    ObstacleType obstypObject;
                    ObstacleType obstypTarget;

                    if (blnGoingBack)
                    {
                        obstypObject = ObstacleType.CreateObjectFromText(lgLog.StrSave, true);
                        obstypTarget = ObstacleType.GetObjectByID(ObstacleType.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        obstypObject = ObstacleType.CreateObjectFromText(lgLog.StrResult, true);
                        obstypTarget = ObstacleType.GetObjectByID(ObstacleType.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    if (blnGoingBack)
                        obstypTarget.ImgTexture = lgLog.ImgSave;
                    else
                        obstypTarget.ImgTexture = lgLog.ImgResult;

                    obstypTarget.IntWorldID = obstypObject.IntWorldID;
                    obstypTarget.StrName = obstypObject.StrName;
                    obstypTarget.IntSolidness = obstypObject.IntSolidness;
                    obstypTarget.IntBounciness = obstypObject.IntBounciness;
                    obstypTarget.BlnDoBounce = obstypObject.BlnDoBounce;
                    break;

                case ObjectType.SpecialType:
                    SpecialType spctypObject;
                    SpecialType spctypTarget;

                    if (blnGoingBack)
                    {
                        spctypObject = SpecialType.CreateObjectFromText(lgLog.StrSave, true);
                        spctypTarget = SpecialType.GetObjectByID(SpecialType.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        spctypObject = SpecialType.CreateObjectFromText(lgLog.StrResult, true);
                        spctypTarget = SpecialType.GetObjectByID(SpecialType.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    if (blnGoingBack)
                        spctypTarget.ImgTexture = lgLog.ImgSave;
                    else
                        spctypTarget.ImgTexture = lgLog.ImgResult;


                    spctypTarget.IntWorldID = spctypObject.IntWorldID;
                    spctypTarget.StrName = spctypObject.StrName;
                    spctypTarget.IntSolidness = spctypObject.IntSolidness;
                    spctypTarget.IntBounciness = spctypObject.IntBounciness;
                    spctypTarget.BlnDoBounce = spctypObject.BlnDoBounce;
                    spctypTarget.IntWidth = spctypObject.IntWidth;
                    spctypTarget.IntHeight = spctypObject.IntHeight;
                    spctypTarget.BlnKillPlayer = spctypObject.BlnKillPlayer;
                    spctypTarget.BlnTravelTo = spctypObject.BlnTravelTo;
                    spctypTarget.BlnStartGroupMove = spctypObject.BlnStartGroupMove;
                    spctypTarget.BlnWinGame = spctypObject.BlnWinGame;
                    spctypTarget.BlnOnlyOnEnter = spctypObject.BlnOnlyOnEnter;
                    break;

                case ObjectType.Folder:
                    Folder flObject;
                    Folder flTarget;

                    if (blnGoingBack)
                    {
                        flObject = Folder.CreateObjectFromText(lgLog.StrSave, true);
                        flTarget = Folder.GetObjectByID(Folder.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        flObject = Folder.CreateObjectFromText(lgLog.StrResult, true);
                        flTarget = Folder.GetObjectByID(Folder.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    flTarget.IntDimensionID = flObject.IntDimensionID;
                    flTarget.StrName = flObject.StrName;
                    flTarget.BlnLoop = flObject.BlnLoop;
                    flTarget.BlnComeBack = flObject.BlnComeBack;
                    flTarget.BlnOnlyWhenSeen = flObject.BlnOnlyWhenSeen;
                    break;

                case ObjectType.Dimension:
                    Dimension dimObject;
                    Dimension dimTarget;

                    if (blnGoingBack)
                    {
                        dimObject = Dimension.CreateObjectFromText(lgLog.StrSave, true);
                        dimTarget = Dimension.GetObjectByID(Dimension.CreateObjectFromText(lgLog.StrResult, true).IntID);
                    }
                    else
                    {
                        dimObject = Dimension.CreateObjectFromText(lgLog.StrResult, true);
                        dimTarget = Dimension.GetObjectByID(Dimension.CreateObjectFromText(lgLog.StrSave, true).IntID);
                    }

                    if (blnGoingBack)
                        dimTarget.ImgBackgroundImage = lgLog.ImgSave;
                    else
                        dimTarget.ImgBackgroundImage = lgLog.ImgResult;

                    dimTarget.IntWorldID = dimObject.IntWorldID;
                    dimTarget.StrName = dimObject.StrName;
                    dimTarget.IntWidth = dimObject.IntWidth;
                    dimTarget.IntHeight = dimObject.IntHeight;
                    break;
            }
        }

        /// <summary>
        /// Add back an object
        /// </summary>
        /// <param name="lgLog"></param>
        private static void Add(Log lgLog, bool blnGoingBack)
        {
            string strText;

            if (blnGoingBack)
                strText = lgLog.StrSave;
            else
                strText = lgLog.StrResult;

            switch (lgLog.ObtType)
            {
                case ObjectType.Obstacle:
                    Obstacle.List_obsObstacles.Add(Obstacle.CreateObjectFromText(strText, false));
                    break;

                case ObjectType.Special:
                    Special.List_spcSpecials.Add(Special.CreateObjectFromText(strText, false));
                    break;

                case ObjectType.Movement:
                    Movement.List_frmvMovements.Add(Movement.CreateObjectFromText(strText, false));
                    break;

                case ObjectType.ObstacleType:
                    ObstacleType.List_obstypTypes.Add(ObstacleType.CreateObjectFromText(strText, false));
                    break;

                case ObjectType.SpecialType:
                    SpecialType.List_spctypTypes.Add(SpecialType.CreateObjectFromText(strText, false));
                    break;

                case ObjectType.Folder:
                    Folder.List_flFolders.Add(Folder.CreateObjectFromText(strText, false));
                    break;

                case ObjectType.Dimension:
                    Dimension.List_dimDimensions.Add(Dimension.CreateObjectFromText(strText, false));
                    break;
            }
        }

        /// <summary>
        /// Remove back the object
        /// </summary>
        /// <param name="lgLog"></param>
        private static void Remove(Log lgLog, bool blnGoingBack)
        {
            string strText;

            if (blnGoingBack)
                strText = lgLog.StrResult;
            else
                strText = lgLog.StrSave;

                switch (lgLog.ObtType)
                {
                    case ObjectType.Obstacle:
                        Obstacle objObject = Obstacle.GetObjectByID(Obstacle.CreateObjectFromText(strText, true).IntID);
                        objObject.ObsgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Remove(Obstacle.GetObjectByID(objObject.IntID).GetPictureBox());
                        Obstacle.List_obsObstacles.RemoveAt(Obstacle.GetIndexFromID(objObject.IntID));
                        break;

                    case ObjectType.Special:
                        Special spcObject = Special.GetObjectByID(Special.CreateObjectFromText(strText, true).IntID);
                        spcObject.SpcgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Remove(spcObject.GetPictureBox());
                        Special.List_spcSpecials.RemoveAt(Special.GetIndexFromID(spcObject.IntID));
                        break;

                    case ObjectType.Movement:
                        Movement mvObject = Movement.GetObjectByID(Movement.CreateObjectFromText(strText, true).IntID);
                        Movement.List_frmvMovements.RemoveAt(Movement.GetIndexFromID(mvObject.IntID));
                        break;

                    case ObjectType.ObstacleType:
                        ObstacleType obstypObject = ObstacleType.GetObjectByID(ObstacleType.CreateObjectFromText(strText, true).IntID);
                        ObstacleType.List_obstypTypes.RemoveAt(ObstacleType.GetIndexFromID(obstypObject.IntID));
                        break;

                    case ObjectType.SpecialType:
                        SpecialType spctypObject = SpecialType.GetObjectByID(SpecialType.CreateObjectFromText(strText, true).IntID);
                        SpecialType.List_spctypTypes.RemoveAt(SpecialType.GetIndexFromID(spctypObject.IntID));
                        break;

                    case ObjectType.Folder:
                        Folder flObject = Folder.GetObjectByID(Folder.CreateObjectFromText(strText, true).IntID);
                        Folder.List_flFolders.RemoveAt(Folder.GetIndexFromID(flObject.IntID));
                        break;

                    case ObjectType.Dimension:
                        Dimension dimObject = Dimension.GetObjectByID(Dimension.CreateObjectFromText(strText, true).IntID);
                        dimObject.TbpDimension.Controls.Remove(dimObject.PnlDimension);
                        dimObject.DimgrParent.TbcDimensions.Controls.Remove(dimObject.TbpDimension);
                        Dimension.List_dimDimensions.RemoveAt(Dimension.GetIndexFromID(dimObject.IntID));
                        break;
                }
        }

        /// <summary>
        /// Move back the group to it's former place
        /// </summary>
        /// <param name="lgLog"></param>
        private static void MoveBackGroup(Log lgLog, bool blnGoingBack)
        {
            List<string> list_strLines = new List<string>();

            list_strLines.AddRange(lgLog.StrSave.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            List<string> list_strObstacles = new List<string>();
            List<string> list_strSpecials = new List<string>();

            for (int i = 0; i < list_strLines.Count; i++)
            {
                if (list_strLines[i].Split(Main.chrSEPARATOR).Length < 9)
                    list_strObstacles.Add(list_strLines[i]);
                else
                    list_strSpecials.Add(list_strLines[i]);
            }

            int intX = Convert.ToInt32(lgLog.StrResult.Split(Main.chrSEPARATOR)[0]);
            int intY = Convert.ToInt32(lgLog.StrResult.Split(Main.chrSEPARATOR)[1]);

            if (blnGoingBack)
            {
                intX *= -1;
                intY *= -1;
            }

            for (int i = 0; i < list_strObstacles.Count; i++)
                Obstacle.GetObjectByID(Obstacle.CreateObjectFromText(list_strObstacles[i], true).IntID).AddToDefaultLocation(intX, intY);

            for (int i = 0; i < list_strSpecials.Count; i++)
                Special.GetObjectByID(Special.CreateObjectFromText(list_strSpecials[i], true).IntID).AddToDefaultLocation(intX, intY);
        }

        /// <summary>
        /// Remove a group of objects
        /// </summary>
        /// <param name="lgLog"></param>
        /// <param name="blnGoingBack"></param>
        private static void RemoveGroup(Log lgLog, bool blnGoingBack)
        {
            string strText;

            if (blnGoingBack)
                strText = lgLog.StrResult;
            else
                strText = lgLog.StrSave;

            List<string> list_strLines = new List<string>();

            list_strLines.AddRange(strText.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            List<string> list_strObstacles = new List<string>();
            List<string> list_strSpecials = new List<string>();

            for (int i = 0; i < list_strLines.Count; i++)
            {
                if (list_strLines[i].Split(Main.chrSEPARATOR).Length < 9)
                    list_strObstacles.Add(list_strLines[i]);
                else
                    list_strSpecials.Add(list_strLines[i]);
            }

            if (!blnGoingBack)
            {
                for (int i = 0; i < list_strObstacles.Count; i++)
                {
                    Obstacle objObject = Obstacle.GetObjectByID(Obstacle.CreateObjectFromText(list_strObstacles[i], true).IntID);
                    objObject.ObsgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Remove(objObject.GetPictureBox());
                    Obstacle.List_obsObstacles.RemoveAt(Obstacle.GetIndexFromID(objObject.IntID));
                }

                for (int i = 0; i < list_strSpecials.Count; i++)
                {
                    Special spcObject = Special.GetObjectByID(Special.CreateObjectFromText(list_strSpecials[i], true).IntID);
                    spcObject.SpcgrParent.FlParent.FlgrParent.DimParent.PnlDimension.Controls.Remove(spcObject.GetPictureBox());
                    Special.List_spcSpecials.RemoveAt(Special.GetIndexFromID(spcObject.IntID));
                }
            }
        }

        /// <summary>
        /// Adding a group of objects
        /// </summary>
        /// <param name="lgLog"></param>
        /// <param name="blnGoingBack"></param>
        private static void AddGroup(Log lgLog, bool blnGoingBack)
        {
            string strText;

            if (blnGoingBack)
                strText = lgLog.StrSave;
            else
                strText = lgLog.StrResult;

            List<string> list_strLines = new List<string>();

            list_strLines.AddRange(strText.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            List<string> list_strObstacles = new List<string>();
            List<string> list_strSpecials = new List<string>();

            for (int i = 0; i < list_strLines.Count; i++)
            {
                if (list_strLines[i].Split(Main.chrSEPARATOR).Length < 9)
                    list_strObstacles.Add(list_strLines[i]);
                else
                    list_strSpecials.Add(list_strLines[i]);
            }

            if (blnGoingBack)
            {
                for (int i = 0; i < list_strObstacles.Count; i++)
                    Obstacle.List_obsObstacles.Add(Obstacle.CreateObjectFromText(list_strObstacles[i], false));

                for (int i = 0; i < list_strSpecials.Count; i++)
                    Special.List_spcSpecials.Add(Special.CreateObjectFromText(list_strSpecials[i], false));
            }
        }
        #endregion

        //declare all the properties
        public ObjectType ObtType { get; set; }                     //type of the object changed
        public EditingType EdtEdit { get; set; }                    //type of the edit
        public string StrSave { get; set; }                         //savegard of the object
        public string StrResult { get; set; }                       //result of the change (only used in case of a change and not for a removing or adding)
        public Image ImgSave { get; set; }                          //save the image
        public Image ImgResult { get; set; }                        //result of the image

        #region Constructor
        /// <summary>
        /// Construct the new log
        /// </summary>
        /// <param name="obtType"></param>
        /// <param name="edtEdit"></param>
        public Log(ObjectType obtType, EditingType edtEdit, string strSave, string strResult)
        {
            ObtType = obtType;
            EdtEdit = edtEdit;
            StrSave = strSave;
            StrResult = strResult;
        }

        /// <summary>
        /// Constructor of the log with an image
        /// </summary>
        /// <param name="obtType"></param>
        /// <param name="edtEdit"></param>
        /// <param name="objChanged"></param>
        /// <param name="strSave"></param>
        /// <param name="strResult"></param>
        /// <param name="imgSave"></param>
        /// <param name="imgResult"></param>
        public Log(ObjectType obtType, EditingType edtEdit, string strSave, string strResult, Image imgSave, Image imgResult)
        {
            ObtType = obtType;
            EdtEdit = edtEdit;
            StrSave = strSave;
            StrResult = strResult;
            ImgSave = imgSave;
            ImgResult = imgResult;
        }
        #endregion
    }
}