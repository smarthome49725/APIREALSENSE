using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.Text.RegularExpressions;
using System.Security.Cryptography; //SH1
using System.Net.WebSockets;
using System.Drawing;
using System.Drawing.Imaging;


namespace FaceID
{
    class Server
    {
        public static NetworkStream stream;
        //public static NetworkStream streamBROW1;
        // BETA public static NetworkStream streamBROW2;
        public static NetworkStream streamNJS;
        public static Thread conNJSThread = new Thread(Server.conNJS);
        public static Thread conBROWThread1 = new Thread(Server.conBROW1);
        // BETA public static Thread conBROWThread2 = new Thread(Server.conBROW2);
        public static Byte[] bytes;

        public static TcpClient client;
        public static TcpListener server = null;
        public static string ipAddress;

        /************************
         * START SERVICES
         */
        public void StartServer()
        {
            MessageBox.Show("socket ok?");
            try
            {
                Int32 port = 8080;
                
                IPAddress host = IPAddress.Parse(ipAddress);                
                server = new TcpListener(host, port);

                // Start listening for client requests.                
                server.Start();

                /*****************************************************
                 * LOOP PARA CRIAR CONEXÕES NODEJS E BROWSER
                 */
                while (true)
                {
                    Console.WriteLine("AGUARDANDO SOCKET...");

                    client = server.AcceptTcpClient();
                    stream = client.GetStream();

                    while (!stream.DataAvailable) ;
                    bytes = new Byte[client.Available];
                    stream.Read(bytes, 0, bytes.Length);
                    String data = Encoding.UTF8.GetString(bytes);

                    if ((new Regex("^GET").IsMatch(data)))
                    {
                        Console.WriteLine("[WS]: " + data);

                        string statusResponse;
                        Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                        + "Connection: Upgrade" + Environment.NewLine
                        + "Upgrade: websocket" + Environment.NewLine
                        /* + "Sec-WebSocket-Protocol: websession" + Environment.NewLine*/
                        + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                            SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(
                                    new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                )
                            )
                        ) + Environment.NewLine
                        + Environment.NewLine);

                        stream.Write(response, 0, response.Length);
                        statusResponse = System.Text.Encoding.ASCII.GetString(response, 0, response.Length);

                        Console.WriteLine("[WS]: " + statusResponse);

                        if (new Regex("User-Agent").IsMatch(data))
                        {                            
                            //BROWSER1 CONNECTION
                            if (conBROWThread1.IsAlive)
                            {
                                /*streamBROW1 = null;
                                streamBROW1 = stream;
                                stream = null;*/
                                Thread conBROWThread1 = new Thread(Server.conBROW1);
                                conBROWThread1.Start();
                                Console.WriteLine("conBROWThread Number: " + conBROWThread1.GetHashCode().ToString());
                            }
                            else
                            {
                                Thread conBROWThread1 = new Thread(Server.conBROW1);
                                conBROWThread1.Start();
                                Console.WriteLine("conBROWThread Number: " + conBROWThread1.GetHashCode().ToString());
                            }

                           /* BETA
                            * //BROWSER2 CONNECTION
                            if (conBROWThread2.IsAlive)
                            {
                                streamBROW2 = null;
                                streamBROW2 = stream;
                                stream = null;
                                Console.WriteLine("conBROWThread Number: " + conBROWThread2.GetHashCode().ToString());
                            }
                            else
                            {
                                conBROWThread2.Start();
                                Console.WriteLine("conBROWThread Number: " + conBROWThread2.GetHashCode().ToString());
                            }*/
                        }
                        else
                        {
                            //NODEJS CONNECTION                               
                            if (conNJSThread.IsAlive)
                            {
                                streamNJS = null;
                                streamNJS = stream;
                                stream = null;
                                Console.WriteLine("conNJSThread Number: " + conNJSThread.GetHashCode().ToString());
                            }
                            else
                            {
                                conNJSThread.Start();
                                Console.WriteLine("conNJSThread Number: " + conNJSThread.GetHashCode().ToString());

                            }

                        }
                    }

                }
                // Close everything.
                stream.Close();
                /*streamBROW1.Close();*/
                streamNJS.Close();
                client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            // informa e pausa o console para dar tempo de ler a exceção
            Console.WriteLine("\nPressione enter para continuar...");
            Console.Read();
        }


        /*****************
         * CONEXÃO NODEJS                               
         */
        public static void conNJS()
        {
            Byte[] bytes = Server.bytes;
            streamNJS = stream;
            //stream = null;

            while (true)
            {
                while (!streamNJS.DataAvailable) ; //true Se houver dados disponíveis no stream a ser lido; Caso contrário, false.                             

                streamNJS.Read(bytes, 0, bytes.Length);                

                // translate bytes of request to string            
                String data = Encoding.UTF8.GetString(bytes);

                string msg = Converter.decodedStr(bytes, bytes.Length);

                Console.WriteLine("NODEJS: " + msg);
            }

        }


        /********************
         * CONEXÃO BROWSER1
         */
        public static void conBROW1()
        {
            Byte[] bytes = Server.bytes;
            NetworkStream streamBROW1;
            streamBROW1 = stream;
            stream = null;

            while (true)
            {
                while (!streamBROW1.DataAvailable) ; //true Se houver dados disponíveis no stream a ser lido; Caso contrário, false.               

                streamBROW1.Read(bytes, 0, bytes.Length);

                // translate bytes of request to string            
                String data = Encoding.UTF8.GetString(bytes);

                string msg = Converter.decodedStr(bytes, bytes.Length);
                //Funções de interrupção e flag' 
                //Actions.actions(msg);
                Console.WriteLine("BROWSER: " + msg);
            }
        }


        /********************
         * BETA CONEXÃO BROWSER2
         *
        public static void conBROW2()
        {
            Byte[] bytes = Server.bytes;
            streamBROW2 = stream;
            stream = null;

            while (true)
            {
                while (!streamBROW2.DataAvailable) ; //true Se houver dados disponíveis no stream a ser lido; Caso contrário, false.               

                streamBROW2.Read(bytes, 0, bytes.Length);

                // translate bytes of request to string            
                String data = Encoding.UTF8.GetString(bytes);

                string msg = Converter.decodedStr(bytes, bytes.Length);
                //Funções de interrupção e flag' 
                //Actions.actions(msg);
                Console.WriteLine("BROWSER: " + msg);
            }
        }
        */



        public static void sendMsg(String mess)
        {
            Byte[] msgConverted = Converter.strToByte(mess);
            //stream.WriteAsync(msgConverted, 0, msgConverted.Length);
            stream.Write(msgConverted, 0, msgConverted.Length);
        }

    }
}


