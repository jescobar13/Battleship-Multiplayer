using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using WpfApplication2.Model;

namespace WpfApplication2.Controls
{
    public class SocketClient
    {
        // Data buffer for incoming data.
        byte[] bytes = new byte[1024];

        // Create a TCP/IP  socket.
        Socket sender = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        IPAddress ipAddress;
        IPEndPoint remoteEP;
        GameController gameController;
        Player p;

        public SocketClient(GameController gameController, Player p)
        {
            this.gameController = gameController;
            this.p = p;
        }

        /// <summary>
        /// Connecta el client a un objecte Player
        /// </summary>
        /// <param name="p">Player</param>
        /// <returns>True si esta connectat, False si no s'ha establert la connexio</returns>
        public bool connect()
        {
            try {

                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                ipAddress = IPAddress.Parse(p.ip);
                remoteEP = new IPEndPoint(ipAddress, p.port);

                sender.Connect(remoteEP);

                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return false;
        }

        public bool enviaMissatge(string missatge)
        {
            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes(missatge + "<EOF>");

                // Send the data through the socket.
                int bytesSent = sender.Send(msg);
                Console.WriteLine("Client (envia): {0}", missatge);


                // Receive the response from the remote device.
                int bytesRec = sender.Receive(bytes);
                string missRetornat = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                Console.WriteLine("Client (resposta): {0}", missRetornat);

                gameController.novaPeticioAlClient(p, missRetornat.Substring(0, missRetornat.Length - 5));

                // Release the socket.
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return true;
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }

            return false;
        }
    }
}
