/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                         **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 24.06.2021                                                    **
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
** Enums of the editor                                                       **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Type of objects
    /// </summary>
    enum ObjectType
    {
        World,
        Dimension,
        ObstacleType,
        SpecialType,
        Folder,
        Obstacle,
        Special,
        Movement,
        SpecialPlusObstacle
    }

    /// <summary>
    /// Type of changes when editing a world
    /// </summary>
    enum EditingType
    {
        ChangeProperties,
        Remove,
        Add,
        MoveGroup,
        RemoveGroup
    }
}
