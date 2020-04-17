using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PortariaRemota
{
    public class Client
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static byte[] gl_RecTCPBuff = new byte[9];
        private static System.Timers.Timer gl_Timer = new System.Timers.Timer();
        static void Main(string[] args)
        {
            LoopConnect();
            //Comando para envio de bytes para o socket
            //Send();
            Console.ReadLine();
        }

        private static void Send()
        {
            byte[] command = new byte[] {
                0x00,
                0x0C
            };

            _clientSocket.Blocking = false;
            
            _clientSocket.Send(command, command.Length, 0);

            Thread.Sleep(100);

            int _receive = _clientSocket.Receive(gl_RecTCPBuff);
            //int _receive = 9;

            byte[] data = new byte[_receive];
                //Array.Copy(gl_RecTCPBuff, data, _receive);
                byte[] l_frameHex = new byte[_receive];

                Buffer.BlockCopy(gl_RecTCPBuff, 0, data, 0, _receive);

            _clientSocket.Shutdown(SocketShutdown.Both);

            Console.WriteLine(data);
        }

        private static void LoopConnect()
        {

            int _tentativas = 0;
            _clientSocket.Blocking = false;
            while (!_clientSocket.Connected)
            {
                try
                {
                    _tentativas++;
                    _clientSocket.Connect("localhost", 100);
                    AsyncCallback onconnect = new AsyncCallback(OnConnect);


                }
                catch (SocketException e)
                {
                    Console.Clear();
                    Console.WriteLine("Tentativas de conexão: " + _tentativas.ToString());
                    Console.WriteLine("Erro:  " + e.Message);
                }
            }

            Console.Clear();
            Console.WriteLine("Cliente Conectado.");
        }

        public static void OnConnect(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            if (sock.Connected)
            {
                SetupReceiveCallback(sock);
            }
        }

        public static void SetupReceiveCallback(Socket sock)
        {
            AsyncCallback receiveData = new AsyncCallback(OnReceiveData);
            IAsyncResult _result = sock.BeginReceive(gl_RecTCPBuff, 0, gl_RecTCPBuff.Length,
                SocketFlags.None, receiveData, sock);
        }

        public static void OnReceiveData(IAsyncResult ar)
        {
            Socket sock = (Socket)ar.AsyncState;

            if (!sock.Connected) return;

            int l_nBytesRecTCP = sock.EndReceive(ar);

            if (l_nBytesRecTCP > 0)
            {
                SetupReceiveCallback(sock);

            }
            else
            {
                sock.Shutdown(SocketShutdown.Both);
                sock.Close();
            }
        }

    }
}
