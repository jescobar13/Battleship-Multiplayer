using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication2.Controls;
using WpfApplication2.Model;

namespace WpfApplication2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameController gameController;
        private Game game;

        private List<CelaControl> celesEnemigues = new List<CelaControl>();
        private List<CelaControl> celesAmigues = new List<CelaControl>();
        private List<string> nomJugadors;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gameController = new GameController();

            afegirJugador();

            if (gameController.numPlayers >= 2 && gameController.pAmic)
            {
                game = new Game(gameController.players);
                game.EstatGamechanged += Game_StateChanged;

                actualitzaPanellAmic();
            }
            else
            {
                if (MessageBox.Show("Error intern. L'aplicacio es tencara.",
                    "Alerta! Error greu:", MessageBoxButton.OK,
                    MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    Close();
                }
            }

        }



        /// <summary>
        /// Event que controla l'estat del Joc
        /// </summary>
        private void Game_StateChanged(object sender, EventArgs e)
        {
            switch (game.Estat)
            {
                case EstatJoc.inGame:
                    if (gameController.getPlayerAmic() != null)
                        gameController.connectaServidor();
                    else
                        MessageBox.Show("Has d'haver introduit un Jugador Amic obligatoriament!");
                    break;

                case EstatJoc.inPause:

                    break;

                case EstatJoc.inStop:

                    break;
            }
        }

        #region Afegir Jugadors Enemics

        /// <summary>
        /// Obre el formulari que permet afegir jugadors.
        /// </summary>
        private void afegirJugador()
        {
            PlayersWindow pw = new PlayersWindow(gameController);
            pw.ShowDialog();
        }

        #endregion

        #region Administracio Enemics
        /// <summary>
        /// Event que es dona quant s'obre el comboBox_Enemic
        /// </summary>
        private void comboBox_enemic_DropDownOpened(object sender, EventArgs e)
        {
            if (nomJugadors == null)
                nomJugadors = new List<string>();

            gameController.setNullPlayerEnemicSelected();

            if (nomJugadors.Count != gameController.players.Count - 1) //Restu 1 per descontar el panell amic.
            {
                comboBox_enemic.ItemsSource = null;
                comboBox_enemic.ItemsSource = nomJugadors;

                foreach (Player p in gameController.players)
                {
                    if (p.tipusJugador == TypePlayer.Enemic)
                        nomJugadors.Add(p.nom);
                }

            }
        }

        /// <summary>
        /// Eveent que es dona quan el element seleccionat en el comboBox_Enemic ha canviat
        /// </summary>
        private void comboBox_enemic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player playerEnemic = gameController.getPlayerEnemic((sender as ComboBox).SelectedItem as string);
            if (playerEnemic != null)
            {
                celesEnemigues.Clear();
                foreach (Cela c in playerEnemic.panell)
                {
                    CelaControl control = new CelaControl();
                    control.CelaDelPanell = c;
                    celesEnemigues.Add(control);
                    control.Click += Cela_Clicked_Enemic;

                    Grid.SetColumn(control, c.Column);
                    Grid.SetRow(control, c.Row);
                    grid_enemic.Children.Add(control);
                }

                if (gameController.connectaClient(playerEnemic))
                    actualitzaPanellEnemic();
            }
        }

        private void actualitzaPanellEnemic()
        {
            gameController.enviaPeticioPanell();
        }

        /// <summary>
        /// Mètode que controla la pulsació de una cela (del panell enemic) per part de l'usuari
        /// </summary>
        private void Cela_Clicked_Enemic(object sender, EventArgs e)
        {
            foreach (CelaControl cc in celesEnemigues)
            {
                if ((sender as CelaControl).Equals(cc))
                    gameController.NouAtac(cc);
            }
        }
        #endregion

        #region Administracio Amics
        /// <summary>
        /// Mètode que controla la pulsació de una cela (del panell amic) per part de l'usuari
        /// </summary>
        private void Cela_Clicked_Amic(object sender, EventArgs e)
        {
            foreach (CelaControl cc in celesAmigues)
            {
                if ((sender as CelaControl).Equals(cc))
                    cc.CelaDelPanell.Estat = EstatCela.Barco;
            }
        }

        /// <summary>
        /// Actualitza el panell d'usuari JugadorAmic
        /// </summary>
        private void actualitzaPanellAmic()
        {
            Player playerAmic = gameController.getPlayerAmic();
            if (playerAmic != null)
            {
                foreach (Cela c in playerAmic.panell)
                {
                    CelaControl control = new CelaControl();
                    c.Tocada = false;
                    control.CelaDelPanell = c;
                    celesAmigues.Add(control);
                    control.Click += Cela_Clicked_Amic;

                    Grid.SetColumn(control, c.Column);
                    Grid.SetRow(control, c.Row);
                    grid_amic.Children.Add(control);

                    label_nomJugadorAmic.Content = playerAmic.nom;
                }
            }
        }
        #endregion

        #region Accions del Menu Principal
        private void menuItem_play_Click(object sender, RoutedEventArgs e)
        {
            game.Start();
            menuItem_play.IsEnabled = false;
            menuItem_pause.IsEnabled = true;
            menuItem_stop.IsEnabled = true;
        }
        private void menuItem_stop_Click(object sender, RoutedEventArgs e)
        {
            menuItem_play.IsEnabled = true;
            menuItem_pause.IsEnabled = false;
            menuItem_stop.IsEnabled = false;
        }
        private void menuItem_pause_Click(object sender, RoutedEventArgs e)
        {
            menuItem_play.IsEnabled = true;
            menuItem_pause.IsEnabled = false;
            menuItem_stop.IsEnabled = true;
        }
        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            afegirJugador();
        }
    }
}
