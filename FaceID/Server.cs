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
        public static NetworkStream streamNJS;
        public static NetworkStream streamBROW1;
        public static NetworkStream streamBROW2;
        public static NetworkStream streamBROW3;
        
        public static Thread conNJSThread = new Thread(Server.conNJS);
        public static Thread conBROW1Thread = new Thread(Server.conBROW1);
        public static Thread conBROW2Thread = new Thread(Server.conBROW2);
        public static Thread conBROW3Thread = new Thread(Server.conBROW3);        
        public static Byte[] bytes;
        public static bool canWrite = false;

        public static TcpClient client;
        public static TcpListener server = null;
        public static string ipAddress;

        /************************
         * START SERVICES
         */
        public void StartServer()
        {
            //MessageBox.Show("socket ok?");
            try
            {
                Int32 port = 8080;

                //IPAddress host = IPAddress.Parse(ipAddress);                
                IPAddress host = IPAddress.Parse("192.168.42.132");
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
                                                
                        Console.WriteLine("[WS]: Conected");

                        if (new Regex("User-Agent").IsMatch(data))
                        {
                            //LEVEL'S                            
                            if (new Regex("level1").IsMatch(data)) //LEVEL 1
                            {
                                //BROWSER1 CONNECTION +++                                
                                if (conBROW1Thread.IsAlive)
                                {
                                    streamBROW1 = null;
                                    streamBROW1 = stream;
                                    stream = null;
                                    Console.WriteLine("conBROW1Thread Number: " + conBROW1Thread.GetHashCode().ToString());                                    
                                }
                                else
                                {
                                    conBROW1Thread.Start();                                    
                                    Console.WriteLine("conBROW1Thread Number: " + conBROW1Thread.GetHashCode().ToString());                                                                     
                                }
                                //BROWSER1 CONNECTION ---
                            }
                            else
                            {
                                if (new Regex("level2").IsMatch(data)) //LEVEL 2
                                {
                                    //BROWSER2 CONNECTION +++
                                    if (conBROW2Thread.IsAlive)
                                    {
                                        streamBROW2 = null;
                                        streamBROW2 = stream;
                                        stream = null;
                                        Console.WriteLine("conBROW2Thread Number: " + conBROW2Thread.GetHashCode().ToString());
                                    }
                                    else
                                    {
                                        conBROW2Thread.Start();
                                        Console.WriteLine("conBROW2Thread Number: " + conBROW2Thread.GetHashCode().ToString());
                                    }
                                    //BROWSER2 CONNECTION ---                                   
                                }
                                else //level3
                                {
                                    //BROWSER3 CONNECTION +++
                                    if (conBROW3Thread.IsAlive)
                                    {
                                        streamBROW3 = null;
                                        streamBROW3 = stream;
                                        stream = null;
                                        Console.WriteLine("conBROW3Thread Number: " + conBROW3Thread.GetHashCode().ToString());
                                    }
                                    else
                                    {
                                        conBROW3Thread.Start();
                                        Console.WriteLine("conBROW3Thread Number: " + conBROW3Thread.GetHashCode().ToString());
                                    }
                                    //BROWSER2 CONNECTION ---
                                }
                            }

                        }
                        else //NODEJS CONNECTION                               
                        {
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
                //stream.Close();
                //streamBROW1.Close();
                //streamNJS.Close();
                //client.Close();
                server.Stop();
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
            int lengthRead;
            Byte[] bytes = Server.bytes;
            streamNJS = stream;
            stream = null;

            while (true)
            {
                lengthRead = streamNJS.Read(bytes, 0, bytes.Length); //BLOCK
                if (lengthRead != 0)
                {                    
                    // translate bytes of request to string            
                    String data = Encoding.UTF8.GetString(bytes);
                    string msg = Converter.decodedStr(bytes, bytes.Length);
                    Console.WriteLine("BROWSER2: " + msg);
                }
                else
                {                   
                    Thread.Sleep(2000);
                    Console.WriteLine("streamNJS OFF");
                }

            }

        }


        /********************
         * CONEXÃO BROWSER1
         * NÃO FOI POSSÍVEL UTILIZAR POO POIS É NCESSÁRIO 
         * TER O CONTROLE DE CADA CONEXÃO, CADA FUNÇÃO (THREAD).
         * EX: NÃO FOI POSSÍVEL UTILIZAR O MÉTODO "conBROW1Thread.IsAlive" 
         * PARA SABER CHECAR THREADS COM MESMO DA EXECUTANDO A MESMA FUNÇÃO.         
         */
        public static void conBROW1()
        {            
            Byte[] bytes = Server.bytes;
            streamBROW1 = stream;
            stream = null;
            canWrite = true;

            while (true)
            {
                ; //BLOCK
                if (streamBROW1.Read(bytes, 0, bytes.Length) != 0)
                {
                    canWrite = true;
                    // translate bytes of request to string            
                    String data = Encoding.UTF8.GetString(bytes);
                    string msg = Converter.decodedStr(bytes, bytes.Length);
                    Console.WriteLine("BROWSER1: " + msg);
                }
                else
                {
                    canWrite = false;
                    Thread.Sleep(2000);
                    Console.WriteLine("streamBROW1 OFF");
                }

            }
        }


        /********************
         * CONEXÃO BROWSER2
         */
        public static void conBROW2()
        {
            int lengthRead;
            Byte[] bytes = Server.bytes;
            streamBROW2 = stream;
            stream = null;

            while (true)
            {
                lengthRead = streamBROW2.Read(bytes, 0, bytes.Length); //BLOCK
                if (lengthRead != 0)
                {
                    // translate bytes of request to string            
                    String data = Encoding.UTF8.GetString(bytes);
                    string msg = Converter.decodedStr(bytes, bytes.Length);
                    Console.WriteLine("BROWSER2: " + msg);
                }
                else
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("streamBROW2 OFF");
                }

            }
        }

        /********************
         * CONEXÃO BROWSER3
         */
        public static void conBROW3()
        {
            int lengthRead;
            Byte[] bytes = Server.bytes;
            streamBROW3 = stream;
            stream = null;

            while (true)
            {
                lengthRead = streamBROW3.Read(bytes, 0, bytes.Length); //BLOCK
                if (lengthRead != 0)
                {
                    // translate bytes of request to string            
                    String data = Encoding.UTF8.GetString(bytes);
                    string msg = Converter.decodedStr(bytes, bytes.Length);
                    Console.WriteLine("BROWSER3: " + msg);
                }
                else
                {
                    Thread.Sleep(2000);
                    Console.WriteLine("streamBROW3 OFF");
                }

            }
        }


        public static void sendMsg(String mess)
        {
            Byte[] msgConverted = Converter.strToByte(mess);            
            if (canWrite)
             {                
                streamBROW1.Write(msgConverted, 0, msgConverted.Length);
                Console.WriteLine("canWrite WS: true");
            }
            else
             {
                 Console.WriteLine("canWrite WS: false");
             }

        }


        /**
         * TESTE ENVIANDO COORDENADAS ARA CANVES HTML BROWSER
         */
        public static void testeCanvasHTML()
        {
            int X = 0;
            int Y = 0;
            int W = 100;
            int H = 100;

            while (X<=301)
            {
                if (X == 300)
                {
                    X = 0;
                    Y = 0;
                }
                X++;
                Y++;
                Server.sendMsg(Y.ToString() + " " + X.ToString() + " " + W.ToString() + " " + H.ToString());
                Thread.Sleep(50);
            }
        }

        public static void startTesteCanvasHTML()
        {         
            Thread testeCanvasHTML_TH = new Thread(Server.testeCanvasHTML);
            testeCanvasHTML_TH.Start();
        }




    }
}
