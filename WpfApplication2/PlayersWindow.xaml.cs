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
using System.Windows.Shapes;
using WpfApplication2.Controls;

namespace WpfApplication2
{
    /// <summary>
    /// Lógica de interacción para PlayersWindow.xaml
    /// </summary>
    public partial class PlayersWindow : Window
    {
        GameController gameController;
        public PlayersWindow(GameController gc)
        {
            gameController = gc;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btn_afegir_jugador_Click(object sender, RoutedEventArgs e)
        {
            if (!(textBox_playerName.Text.Equals("") &&
                textBox_ip.Text.Equals("") &&
                textBox_port.Text.Equals("") &&
                (radioButton_Amic.IsChecked == true || radioButton_Enemic.IsChecked == true)
                ))

            {
                if (MessageBox.Show("Estas segur que vols afegir aquest jugador??",
                    "Question", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes
                    )
                {
                    if (radioButton_Amic.IsChecked == true)
                    {
                        if (gameController.pAmic)
                        {
                            MessageBox.Show("Ja has introduit un jugador amic.", "Informació",
                                MessageBoxButton.OK, MessageBoxImage.Stop);
                        }
                        else
                        {
                            gameController.afegirPlayer(
                                                    textBox_playerName.Text,
                                                    textBox_ip.Text,
                                                    Convert.ToInt32(textBox_port.Text),
                                                    Model.TypePlayer.Amic
                                                    );
                            gameController.numPlayers++;
                            gameController.pAmic = true;
                        }

                    }
                    else if (radioButton_Enemic.IsChecked == true)
                    {
                        gameController.afegirPlayer(
                                                textBox_playerName.Text,
                                                textBox_ip.Text,
                                                Convert.ToInt32(textBox_port.Text),
                                                Model.TypePlayer.Enemic
                        );
                        gameController.numPlayers++;
                    }


                    if (MessageBox.Show("Vols afegir-ne un altre?",
                    "Question", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes
                    )
                    {
                        textBox_playerName.Text = "";
                        textBox_ip.Text = "";
                        textBox_port.Text = "";
                        return;
                    }
                    else
                    {
                        accio_tencar();
                    }
                }
            }
        }

        private void btn_tencar_Click(object sender, RoutedEventArgs e)
        {
            accio_tencar();
        }

        private void accio_tencar()
        {
            if (MessageBox.Show("Estas segur que tencar??",
                    "Question", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes
                    )
            {
                Close();
            }
            else
            {
                textBox_playerName.Text = "";
                textBox_ip.Text = "";
                textBox_port.Text = "";
                return;
            }
        }
    }
}
