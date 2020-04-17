using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PortariaRemota
{
    class Server
    {

        private static Socket csTCP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static List<Socket> _clientsSockets = new List<Socket>();
        private static byte[] gl_RecTCPBuff = new byte[9];
        private static System.Timers.Timer gl_Timer = new System.Timers.Timer();
        private static int counter = 0;

        static void Main(string[] args)
        {
            SetupServer();
            Console.ReadLine();
        }
        private static void SetupServer()
        {
            try
            {
                Console.WriteLine("Iniciando servidor...");

                csTCP.Bind(new IPEndPoint(IPAddress.Any, 100));
                csTCP.Listen(100);
                while(true)
                {
                    csTCP.BeginAccept(new AsyncCallback(AcceptCallBack), null);
                }
            }

            catch (Exception)
            {

                throw;
            }
        }
        private static void AcceptCallBack(IAsyncResult ar)
        {
            Socket _socket = csTCP.EndAccept(ar);
            _clientsSockets.Add(_socket);

            IAsyncResult _result = _socket.BeginReceive(gl_RecTCPBuff, 0, gl_RecTCPBuff.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), _socket);

            counter++;
                Console.Clear();
                Console.WriteLine($"Clientes conectados: {counter}");
        }
        private static void ReceiveCallBack(IAsyncResult ar)
        {
            Socket _socket = (Socket)ar.AsyncState;
            int received = _socket.EndReceive(ar);

            byte[] dataBuf = new byte[received];
            Array.Copy(gl_RecTCPBuff, dataBuf, received);

            string _texto = Encoding.ASCII.GetString(dataBuf);
            //Console.WriteLine("Texto recebido" + _texto);

            string response = string.Empty;

            if (_texto.ToLower() != "get time")
            {
                response = "Requisição inválida";
            }
            else
            {
                response = DateTime.Now.ToLongTimeString();
            }

            byte[] data = Encoding.ASCII.GetBytes(response);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), _socket);
        }
        private static void SendCallBack(IAsyncResult ar)
        {
            Socket _socket = (Socket)ar.AsyncState;
            _socket.EndSend(ar);

        }
    }
}
