/******************************************************************************
** PROGRAMME TooDeePlatformer Editor                                  **
**                                                                           **
** Lieu      : ETML - section informatique                                   **
** Auteur    : Devaud Aurélien                                               **
** Date      : 11.06.2021                                                    **
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
** This file is used to describe the position of an object                   **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System.Drawing;

namespace TooDeePlatformer.Editor
{
    #region Position
    /// <summary>
    /// Class used to describe the position of an object
    /// </summary>
    public class Position
    {
        //declare the variables
        public Location LocLocation { get; set; }       //Point location
        public Distance DisDistance { get; set; }       //Size

        #region Constructor
        public Position()
        {
            LocLocation = new Location();
            DisDistance = new Distance();
        }
        #endregion

        #region Convertions
        /// <summary>
        /// Invert the Y position
        /// </summary>
        /// <param name="intDimensionHeight"></param>
        /// <returns></returns>
        public Position InvertY(int intDimensionHeight)
        {
            Position posPosition_ = new Position();
            posPosition_.DisDistance.dblWidth = DisDistance.dblWidth;
            posPosition_.DisDistance.dblHeight = DisDistance.dblHeight;
            posPosition_.LocLocation.dblX = LocLocation.dblX;
            posPosition_.LocLocation.dblY = intDimensionHeight - LocLocation.dblY - DisDistance.dblHeight;

            return posPosition_;
        }

        /// <summary>
        /// Return a Rectangle object
        /// </summary>
        /// <returns></returns>
        public Rectangle Rectangle()
        {
            return new Rectangle(
                (int)(LocLocation.dblX),
                (int)(LocLocation.dblY),
                (int)(DisDistance.dblWidth),
                (int)(DisDistance.dblHeight)
            );
        }

        /// <summary>
        /// Create a position from a point and a size
        /// </summary>
        /// <param name="Point"></param>
        /// <param name="Size"></param>
        /// <param name="intDimensionHeight"></param>
        public void FromPointLocation(Point Point, Size Size, int intDimensionHeight)
        {
            DisDistance.dblWidth = Size.Width / Main.intUNITSIZE;
            DisDistance.dblHeight = Size.Height / Main.intUNITSIZE;
            LocLocation.dblX = Point.X / Main.intUNITSIZE;
            LocLocation.dblY = Point.Y / Main.intUNITSIZE;
            LocLocation.dblY = InvertY(intDimensionHeight).LocLocation.dblY;
        }

        /// <summary>
        /// Return a size
        /// </summary>
        /// <returns></returns>
        public Size ToSize() => new Size((int)(DisDistance.dblWidth * Main.intUNITSIZE), (int)(DisDistance.dblHeight * Main.intUNITSIZE));

        /// <summary>
        /// Return a point
        /// </summary>
        /// <param name="intDimensionHeight"></param>
        /// <returns></returns>
        public Point ToPoint(int intDimensionHeight)
        {
            Position Position_ = new Position();
            Position_.LocLocation = LocLocation;
            Position_.DisDistance = DisDistance;

            return new Point((int)(Position_.LocLocation.dblX * Main.intUNITSIZE), (int)(Position_.InvertY(intDimensionHeight).LocLocation.dblY * Main.intUNITSIZE));
        }
        #endregion

        #region Convert Locations
        /// <summary>
        /// Convert 2 points into a position
        /// </summary>
        /// <param name="intX"></param>
        /// <param name="intY"></param>
        /// <param name="intX2"></param>
        /// <param name="intY2"></param>
        /// <returns></returns>
        public static Position Convert2pLocationToPosition(int intX, int intY, int intX2, int intY2)
        {
            //detect if the second X point is in front of the first
            if (intX2 >= intX)
            {
                intX = intX - intX % Main.intUNITSIZE;
                if (intX2 % Main.intUNITSIZE != 0)
                    intX2 = intX2 + Main.intUNITSIZE - intX2 % Main.intUNITSIZE;
            }
            else
            {
                int intTemp = intX2;
                if (intX % Main.intUNITSIZE == 0)
                    intX2 = intX;
                else
                    intX2 = intX + Main.intUNITSIZE - intX % Main.intUNITSIZE;
                intX = intTemp - intTemp % Main.intUNITSIZE;
            }

            //detect if the second Y point is under the first position
            if (intY2 >= intY)
            {
                intY = intY - intY % Main.intUNITSIZE;
                if (intY2 % Main.intUNITSIZE != 0)
                    intY2 = intY2 + Main.intUNITSIZE - intY2 % Main.intUNITSIZE;
            }
            else
            {
                int intTemp = intY2;
                if (intY % Main.intUNITSIZE == 0)
                    intY2 = intY;
                else
                    intY2 = intY + Main.intUNITSIZE - intY % Main.intUNITSIZE;
                intY = intTemp - intTemp % Main.intUNITSIZE;
            }

            //transform the coordinates into the games coordinates format
            Position Position_ = new Position();
            Position_.LocLocation.dblX = intX / Main.intUNITSIZE;
            Position_.LocLocation.dblY = intY / Main.intUNITSIZE;
            Position_.DisDistance.dblWidth = (intX2 - intX) / Main.intUNITSIZE;
            Position_.DisDistance.dblHeight = (intY2 - intY) / Main.intUNITSIZE;

            return Position_;
        }
        #endregion
    }
    #endregion

    #region Location
    /// <summary>
    /// Used to describe a point
    /// </summary>
    public class Location
    {
        //declare the variables
        public double dblX { get; set; }        //X location
        public double dblY { get; set; }        //Y location

        public Location()
        {
            dblX = 0;
            dblY = 0;
        }
    }
    #endregion

    #region Distance
    /// <summary>
    /// Used to describe a size
    /// </summary>
    public class Distance
    {
        public double dblWidth { get; set; }        //X distance to the point
        public double dblHeight { get; set; }       //Y distance to the point

        public Distance()
        {
            dblWidth = 0;
            dblHeight = 0;
        }
    }
    #endregion
}