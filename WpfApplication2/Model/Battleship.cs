using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2.Model
{
    /// <summary>
    /// Tipus de jugador.
    /// </summary>
    public enum TypePlayer
    {
        Amic, Enemic
    }

    public class Player
    {
        public string nom { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public TypePlayer tipusJugador { get; set; }
        public Panell panell { get; set; }

        public Player(string nom, string ip, int port, TypePlayer tipusJugador)
        {
            this.nom = nom;
            this.ip = ip;
            this.port = port;
            this.tipusJugador = tipusJugador;
        }
    }

    /// <summary>
    /// Posibles estats en que el joc es pot trobar
    /// </summary>
    public enum EstatJoc
    {
        inGame,
        inPause,
        inStop
    }

    public class Game
    {
        /// <summary>
        /// Panell de joc
        /// </summary>
        public Game(List<Player> jugadors)
        {
            foreach(Player p in jugadors)
            {
                if(p.tipusJugador == TypePlayer.Amic)
                {
                    p.panell = new Panell();
                }

                if (p.tipusJugador == TypePlayer.Enemic)
                    p.panell = new Panell();
            }
        }

        public void Start()
        {
            estat = EstatJoc.inGame;
            OnEstatGamechanged(new EventArgs());
        }

        private EstatJoc estat;

        /// <summary>
        /// Estableix o retorna el estat del Joc
        /// </summary>
        public EstatJoc Estat
        {
            get { return estat; }
            private set
            {
                if(estat != value)
                {
                    estat = value;
                    OnEstatGamechanged(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Es produeix quan el Joc (Game) canvia d'estat.
        /// </summary>
        public event EventHandler EstatGamechanged;

        /// <summary>
        /// Genera el event EstatGamechanged
        /// </summary>
        protected virtual void OnEstatGamechanged(EventArgs e)
        {
            EventHandler handler = EstatGamechanged;
            if (handler != null)
                handler(this, e);
        }
    }
}
