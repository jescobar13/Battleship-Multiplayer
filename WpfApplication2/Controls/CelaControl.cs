using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;
using System.Windows.Media;
using WpfApplication2.Model;

namespace WpfApplication2
{
    public class CelaControl : Button
    {

        public CelaControl()
        {
            Background = Brushes.CornflowerBlue;
        }

        /// <summary>
        /// Objecte Cela del Panell associat en el control
        /// </summary>
        private Cela celaDelPanell;

        public Cela CelaDelPanell
        {
            get { return celaDelPanell; }
            set
            {
                if (celaDelPanell != value)
                {
                    if (celaDelPanell != null)
                        celaDelPanell.EstatCelachanged -= CelaDelPanell_EstatCelaChanged;

                    celaDelPanell = value;
                    celaDelPanell.EstatCelachanged += CelaDelPanell_EstatCelaChanged;
                }
            }
        }

        /// <summary>
        /// Actualitza el color de fons del control quan la cela canvia d'estat
        /// </summary>
        private void CelaDelPanell_EstatCelaChanged(object sender, EventArgs e)
        {
            if (CelaDelPanell.Estat == EstatCela.Tocat)
                Background = Brushes.Brown;
            if (CelaDelPanell.Estat == EstatCela.AiguaNull)
                Background = Brushes.CornflowerBlue;
            if (CelaDelPanell.Estat == EstatCela.Aigua)
                Background = Brushes.Teal;
            if (CelaDelPanell.Estat == EstatCela.Barco)
                Background = Brushes.SpringGreen;
        }
    }
}
