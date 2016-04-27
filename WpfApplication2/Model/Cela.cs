using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Model
{
    public enum EstatCela
    {
        AiguaNull, Tocat, Aigua, Barco
    }

    public class Cela
    {
        const string letters = "ABCDEFGHIJ";

        /// <summary>
        /// Panell al que partany la cela
        /// </summary>
        public Panell Panell { get; set; }

        /// <summary>
        /// Fila (Index basat en 0)
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Columna (Index basat en 0)
        /// </summary>
        public int Column { get; set; }

        public bool Tocada { get; set; }

        private EstatCela estat;

        /// <summary>
        /// Inicialitza una nova instancia de <see cref="Cela"/>
        /// </summary>
        /// <param name="panell">Referencia al panell al que pertany</param>
        /// <param name="row">Fila (Index basat en 0)</param>
        /// <param name="column">Columne (Index basat en 0)</param>
        public Cela(Panell panell, int row, int column)
        {
            Panell = panell;
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Estableix o retorna el estat de la cela
        /// </summary>
        public EstatCela Estat
        {
            get { return estat; }
            set
            {
                estat = value;
                OnEstatCelachanged(new EventArgs());
            }
        }

        public string donemCodiEstat()
        {
            string estatCela = null;

            if (Estat == EstatCela.Aigua)
                estatCela = "0";
            if (Estat == EstatCela.Tocat)
                estatCela = "1";
            if (Estat == EstatCela.AiguaNull)
                estatCela = "2";
            if (Estat == EstatCela.Barco)
                estatCela = "3";

            return estatCela;
        }

        public string getCelaWithStateToString()
        {
            return letters[Row % letters.Length].ToString() + ":" + (Column + 1).ToString() + "," + donemCodiEstat() + ";";
        }

        public string getCelaToString()
        {
            return letters[Row % letters.Length].ToString() + ":" + (Column + 1).ToString();
        }

        public void canviaEstat(EstatCela estat)
        {
            Estat = estat;
        }


        /// <summary>
        /// Es produeix quan la cela canvia d'estat.
        /// </summary>
        public event EventHandler EstatCelachanged;

        /// <summary>
        /// Genera el event EstatCelachanged
        /// </summary>
        protected virtual void OnEstatCelachanged(EventArgs e)
        {
            EventHandler handler = EstatCelachanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
