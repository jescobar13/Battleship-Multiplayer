using System.Collections.Generic;
using System.Text.RegularExpressions;
using WpfApplication2.Model;
using System.Windows;

namespace WpfApplication2.Controls
{
    public class GameController
    {
        SocketServer serverSocket;
        SocketClient clientSocket;

        #region Gestio de jugadors
        /// <summary>
        /// Llista de jugadors disponibles
        /// </summary>
        public List<Player> players { get; set; }
        /// <summary>
        /// Numero de jugadors totals registrats
        /// </summary>
        public int numPlayers { get; set; }
        /// <summary>
        /// True en cas d'haver-hi un jugador amic
        /// False s'hi no s'ha introduit un jugador amic
        /// </summary>
        public bool pAmic { get; set; }

        /// <summary>
        /// Afegeix un jugador al Controlador del Joc
        /// </summary>
        /// <param name="nom">Nom del jugador</param>
        /// <param name="ip">IP del Jugador</param>
        /// <param name="port">Port del Jugador</param>
        /// <param name="tipusJugador">Tipus de jugador Amic/Enemic</param>
        public void afegirPlayer(string nom, string ip, int port, TypePlayer tipusJugador)
        {
            if (players == null)
                players = new List<Player>();

            players.Add(new Player(nom, ip, port, tipusJugador));
            if (tipusJugador == TypePlayer.Amic) pAmic = true;
            numPlayers++;
        }

        /// <summary>
        /// Retorna l'objecte Jugador Amic
        /// </summary>
        /// <returns>Objecte Player</returns>
        public Player getPlayerAmic()
        {
            //Player playerAmic = null;

            //foreach (Player p in players)
            //{
            //    if (p.tipusJugador == TypePlayer.Amic)
            //        playerAmic = p;
            //}

            //return playerAmic;
            Player jugadorAmic = players.Find(x => x.tipusJugador == TypePlayer.Amic);
            return jugadorAmic;
        }

        /// <summary>
        /// Retorna el objecte jugador enemic segons el seu nom
        /// </summary>
        /// <param name="nomJugador">Nom del jugador Enemic</param>
        /// <returns>Objecte Jugador</returns>
        internal Player getPlayerEnemic(string nomJugador)
        {
            Player playerEnemic = null;

            foreach (Player p in players)
            {
                if (p.nom.Equals(nomJugador))
                    playerEnemic = p;
            }

            return playerEnemic;
        }
        #endregion

        #region Accions del Client

        Player playerEnemicSelected;

        public void setNullPlayerEnemicSelected()
        {
            playerEnemicSelected = null;
        }

        /// <summary>
        /// Mètode que ens serveix per crear una connexió i connectar el client segons un Jugador (Player).
        /// </summary>
        /// <param name="player">Jugador al que desitjem connectar-nos</param>
        /// <returns></returns>
        public bool connectaClient(Player player)
        {
            playerEnemicSelected = player;

            clientSocket = null;
            clientSocket = new SocketClient(this, player);


            if (clientSocket.connect())
            {
                return true;
            }
            return false;
        }

        public bool enviaPeticioPanell()
        {
            clientSocket.enviaMissatge("P|");
            return true;
        }

        public bool NouAtac(CelaControl btn)
        {
            if (connectaClient(playerEnemicSelected))
            {
                clientSocket.enviaMissatge("A|" + btn.CelaDelPanell.getCelaToString());
                return true;
            }
            return false;
        }

        /// <summary>
        /// Controla / Gestiona els missatges que entren en el client.
        /// </summary>
        /// <param name="missRetornat"></param>
        internal void novaPeticioAlClient(Player player, string missRetornat)
        {
            string[] missatge = missRetornat.Split(new char[] { '|' });

            if (missatge[0].Equals("P"))
            {
                tractarPanellEnemic(player, missatge[1]);
            }
            else if (missatge[0].Equals("A"))
            {
                tractarAtacLlancat(player, missatge[1]);
            }
        }
        /// <summary>
        /// Fa el tractament del panell enemic.
        /// </summary>
        /// <param name="panell">Estat panell Enemic</param>
        private void tractarPanellEnemic(Player player, string panell)
        {
            string[] sCela = panell.Split(new char[] { ';' });
            string[] yxe;

            for (int i = 0; i < sCela.Length; i++)
            {
                yxe = sCela[i].Split(new char[] { ',' });

                foreach (Cela c in player.panell)
                {
                    if (c.getCelaToString().Equals(yxe[0]))
                    {
                        switch (yxe[1])
                        {
                            case "0":
                                c.Estat = EstatCela.Aigua;
                                break;
                            case "1":
                                c.Estat = EstatCela.Tocat;
                                break;
                            case "2":
                                c.Estat = EstatCela.AiguaNull;
                                break;
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Fa el tractament del atac llançat.
        /// </summary>
        /// <param name="resultat"></param>
        private void tractarAtacLlancat(Player player, string resultat)
        {
            //Entrada --> A:1,0
            string[] celaIestat = Regex.Split(resultat, ",");
            foreach (Cela c in player.panell)
            {
                if (c.getCelaToString().Equals(celaIestat[0]))
                {
                    switch (celaIestat[1])
                    {
                        case "0":
                            c.Estat = EstatCela.Aigua;
                            break;
                        case "1":
                            c.Estat = EstatCela.Tocat;
                            break;
                        case "2":
                            c.Estat = EstatCela.AiguaNull;
                            break;
                    }
                    break;
                }
            }
        }

        #endregion

        #region Accions del servidor

        /// <summary>
        /// Mètode que ens serveix per crear una connexió i connectar el servidor.
        /// </summary>
        /// <returns></returns>
        public bool connectaServidor()
        {
            serverSocket = null;
            serverSocket = new SocketServer(this);
            try
            {
                serverSocket.conecta();
            }
            catch
            {
                return false;
            }

            return true;
        }

        #region Respostes/Gestio del servidor
        /// <summary>
        /// Controla / Gestiona els missatges que entren en el servidor.
        /// </summary>
        /// <param name="missRetornat">Missatge que ha arrivat</param>
        /// <returns>Resposta a la peticio</returns>
        internal string novaPeticioAlServidor(string missRetornat)
        {
            string[] missatge = missRetornat.Split(new char[] { '|' });
            string resposta = null;

            if (missatge[0].Equals("P"))
            {
                resposta = construeixEstatPanellAmic();
            }
            else if (missatge[0].Equals("A"))
            {
                resposta = construeixRespostaAtac(missatge[1]);
            }
            return resposta + "<EOF>";
        }

        /// <summary>
        /// Construeix la cadena de resposta a la peticio del estat del panell
        /// </summary>
        /// <returns>Cadena ja Construida ex: "P|A:1,0;E:4,3;</returns>
        private string construeixEstatPanellAmic()
        {
            string resposta = "P|";

            foreach (Cela c in getPlayerAmic().panell)
            {
                if (c.Estat == EstatCela.Barco)
                {
                    //resposta += c.getCelaToString() + ",0;";
                }
                else if(c.Estat == EstatCela.AiguaNull)
                {
                    //resposta += c.getCelaToString() + ",2;";
                }
                else
                {
                    resposta += c.getCelaWithStateToString();
                }
            }
            return resposta;
        }

        public delegate void SetEstatDeleg(EstatCela estat);

        /// <summary>
        /// Construeix la cadena de resposta a l'atac.
        /// En el cas de que el panell amic 
        /// </summary>
        /// <param name="atac">Cela Atacada ex: "A:3"</param>
        /// <returns>Resposta a l'atac ex: A|0</returns>
        private string construeixRespostaAtac(string atac)
        {
            string resposta = "A|";
            foreach (Cela c in getPlayerAmic().panell)
            {
                if (c.getCelaToString().Equals(atac))
                {
                    if (c.donemCodiEstat().Equals("3"))
                    {
                        resposta += c.getCelaToString() + ",1";
                        Application.Current.Dispatcher.Invoke(new SetEstatDeleg(c.canviaEstat), EstatCela.Tocat);
                        break;
                    }
                    else
                    {
                        resposta += c.getCelaToString() + ",0";
                        Application.Current.Dispatcher.Invoke(new SetEstatDeleg(c.canviaEstat), EstatCela.Aigua);

                    }
                }
            }

            return resposta;
        }
        #endregion
        #endregion
    }
}
