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
** This page serves to initialize the editor                                 **
**                                                                           **
**                                                                           **
**                                                                           **
**                                                                           **
******************************************************************************/

using System.Windows.Forms;

namespace TooDeePlatformer.Editor
{
    /// <summary>
    /// Class containing the editor form
    /// </summary>
    public partial class frmEditor : Form
    {
        /// <summary>
        /// Constructor of the form
        /// </summary>
        public frmEditor()
        {
            //initialize the form
            InitializeComponent();

            //initialize the editor
            Main.Launch(this);
        }
    }
}