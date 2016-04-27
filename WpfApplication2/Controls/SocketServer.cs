using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WpfApplication2.Controls
{
    public class SocketServer
    {
        private GameController gameController;

        public SocketServer(GameController gameController)
        {
            this.gameController = gameController;
        }

        public static bool Connected = false;

        IPAddress ipAddress;
        IPEndPoint localEndPoint;

        byte[] bytes = new Byte[1024];
        Socket handler;
        public string data = null;

        //Creem l'objecte TCP/IP socket
        Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

        //Timer recepció
        static Timer _timer;

        public void conecta()
        {
            ipAddress = IPAddress.Parse("172.24.7.210");
            //ipAddress = IPAddress.Parse("192.168.1.39");

            localEndPoint = new IPEndPoint(ipAddress, 4100);
            listener.Bind(localEndPoint);
            listener.Listen(10);
            Console.Write("Server Connectat");
            Connected = true;
            startTimer();
        }

        private void startTimer()
        {
            _timer = new Timer(500);

            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            handler = null;
            try
            {
                Console.WriteLine("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.
                handler = listener.Accept();
                data = null;
                ReceiveFromBalrog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveFromBalrog()
        {
            bytes = new byte[1024];
            int bytesRec = handler.Receive(bytes);
            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);

            if (data.Length > 0)
            {
                Console.WriteLine("Servidor (rebut): ... " + data);
                string missatgeARetornar = gameController.novaPeticioAlServidor(
                    data.Substring(0, data.Length - 5));

                enviaMissatge(missatgeARetornar);
            }

            listener.Disconnect(true);
        }

        private void enviaMissatge(string miss)
        {

            byte[] msg = Encoding.ASCII.GetBytes(miss);
            handler.Send(msg);
            Console.WriteLine("Server (envia): " + miss);
        }

        public void Close()
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}

