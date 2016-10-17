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
        String data;
        public static Byte[] bytes;
        public static bool conBROW1canWrite = false;
        public static bool conBROW2canWrite = false;
        public static bool conBROW3canWrite = false;
        public static bool conNJScanWrite = false;

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
                    Console.WriteLine("AGUARDANDO SOCKET...\n");

                    client = server.AcceptTcpClient();
                    stream = client.GetStream();

                    while (!stream.DataAvailable) ;
                    
                    bytes = new Byte[client.Available];
                    stream.Read(bytes, 0, bytes.Length);
                    data = Encoding.UTF8.GetString(bytes);                    
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
                        
                            //LEVEL'S                            
                            if (new Regex("level1").IsMatch(data)) //LEVEL 1
                            {
                                //BROWSER1 CONNECTION +++                                
                                if (conBROW1Thread.IsAlive)
                                {
                                    streamBROW1.Dispose();
                                    streamBROW1 = stream;
                                    stream = null;

                                    Console.WriteLine("conBROW1Thread Number: " + conBROW1Thread.GetHashCode().ToString());
                                    conBROW1canWrite = true;
                                }
                                else
                                {
                                    conBROW1Thread.Start();
                                    Console.WriteLine("conBROW1Thread Number: " + conBROW1Thread.GetHashCode().ToString() + "\n");
                                    conBROW1canWrite = true;
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
                                        streamBROW2.Dispose();
                                        streamBROW2 = stream;
                                        stream = null;
                                        Console.WriteLine("conBROW2Thread Number: " + conBROW2Thread.GetHashCode().ToString());
                                        conBROW2canWrite = true;
                                    }
                                    else
                                    {
                                        conBROW2Thread.Start();
                                        Console.WriteLine("conBROW2Thread Number: " + conBROW2Thread.GetHashCode().ToString());
                                        conBROW2canWrite = true;
                                    }
                                    //BROWSER2 CONNECTION ---                                   
                                }
                                else //level3
                                {
                                    //BROWSER3 CONNECTION +++
                                    if (conBROW3Thread.IsAlive)
                                    {
                                        streamBROW3.Dispose();
                                        streamBROW3 = stream;
                                        stream = null;
                                        Console.WriteLine("conBROW3Thread Number: " + conBROW3Thread.GetHashCode().ToString());
                                        conBROW3canWrite = true;
                                    }
                                    else
                                    {
                                        conBROW3Thread.Start();
                                        Console.WriteLine("conBROW3Thread Number: " + conBROW3Thread.GetHashCode().ToString());
                                        conBROW3canWrite = true;
                                    }
                                    //BROWSER2 CONNECTION ---
                                }
                            }                      
                    }
                    else //net.socket != ^GET                              
                    {
                        if (conNJSThread.IsAlive)
                        {
                            streamNJS.Dispose();
                            streamNJS = stream;
                            stream = null;
                            Console.WriteLine("conNJSThread Number: " + conNJSThread.GetHashCode().ToString());
                            conNJScanWrite = true;
                        }
                        else
                        {
                            conNJSThread.Start();
                            Console.WriteLine("conNJSThread Number: " + conNJSThread.GetHashCode().ToString());
                            conNJScanWrite = true;
                        }

                    }//net.socket  



                }
            }
            catch// (System.IO.IOException e)
            {
                //Console.WriteLine("System.IO.IOException: {0}", e);
            }
            /* finally
             {
                 //server.Stop();
             }*/

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
            String data;
            String msg;
            streamNJS = stream;
            stream = null;

            while (true)
            {
                try
                {
                    if (streamNJS.Read(bytes, 0, bytes.Length) != 0)
                    {
                        // translate bytes of request to string            
                        data = Encoding.UTF8.GetString(bytes);
                        msg = Converter.decodedStr(bytes, bytes.Length);
                        Actions.actions(msg);
                    }
                    else
                    {
                        if (conNJScanWrite)
                        {
                            conNJScanWrite = false;
                            //Console.WriteLine("conBROW1canWrite WS: false");
                            //Console.WriteLine("streamBROW1 OFF");
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch
                {
                    Thread.Sleep(1000);
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
            String data;
            String msg;
            streamBROW1 = stream;
            stream = null;                       

            while (true)
            {
                try
                {
                    if (streamBROW1.Read(bytes, 0, bytes.Length) != 0)
                    {
                        // translate bytes of request to string            
                        data = Encoding.UTF8.GetString(bytes);
                        msg = Converter.decodedStr(bytes, bytes.Length);
                        Actions.actions(msg);
                    }
                    else
                    {
                        if (conBROW1canWrite)
                        {
                            conBROW1canWrite = false;
                            //Console.WriteLine("conBROW1canWrite WS: false");
                            //Console.WriteLine("streamBROW1 OFF");
                        }
                        Thread.Sleep(3000);
                    }
                }
                catch
                {
                    Thread.Sleep(3000);
                }
            }

        }


        /********************
         * CONEXÃO BROWSER2
         */
        public static void conBROW2()
        {
            Byte[] bytes = Server.bytes;
            String data;
            String msg;
            streamBROW2 = stream;
            stream = null;                     

            while (true)
            {
                try
                {
                    if (streamBROW2.Read(bytes, 0, bytes.Length) != 0)
                    {
                        // translate bytes of request to string            
                        data = Encoding.UTF8.GetString(bytes);
                        msg = Converter.decodedStr(bytes, bytes.Length);
                        Actions.actions(msg);
                    }
                    else
                    {
                        if (conBROW2canWrite)
                        {
                            conBROW2canWrite = false;
                            //Console.WriteLine("conBROW2canWrite WS: false");
                            //Console.WriteLine("streamBROW2 OFF");
                        }
                        Thread.Sleep(3000);
                    }
                }
                catch
                {
                    Thread.Sleep(3000);
                }
            }

        }


        /********************
        * CONEXÃO BROWSER3
        */
        public static void conBROW3()
        {
            Byte[] bytes = Server.bytes;
            String data;
            String msg;
            streamBROW3 = stream;
            stream = null;                       

            while (true)
            {
                try
                {
                    if (streamBROW3.Read(bytes, 0, bytes.Length) != 0)
                    {
                        // translate bytes of request to string            
                        data = Encoding.UTF8.GetString(bytes);
                        msg = Converter.decodedStr(bytes, bytes.Length);
                        Actions.actions(msg);
                    }
                    else
                    {
                        if (conBROW3canWrite)
                        {
                            conBROW3canWrite = false;
                            //Console.WriteLine("conBROW3canWrite WS: false");
                            //Console.WriteLine("streamBROW3 OFF");
                        }
                        Thread.Sleep(3000);
                    }
                }
                catch
                {
                    Thread.Sleep(3000);
                }
            }

        }


        /******************************************************
         * SEND MSG FOR BROWSER:
         * level == 0: send only for Client NODEJS 
         * level == 1: send only for user level 1
         * level == 2: send only for user level 2
         * level == 3: send only for user level 3
         * level == 255: send for broadcasting (1 and 2 and 3)
         */
        public static void sendMsg(int level, String mess)
        {
            Byte[] msgConverted = Converter.strToByte(mess);

            //TRY SEND MSG NODEJS+++
            if (level == 0 && conNJScanWrite)
            {                
                try
                {
                    Byte[] rawData = System.Text.ASCIIEncoding.UTF8.GetBytes(mess);
                    streamNJS.Write(rawData, 0, rawData.Length);
                    
                }
                catch
                {
                    conNJScanWrite = false;
                    Console.WriteLine("Exception sendMsg NODEJS 000");
                }                
            }
            //TRY SEND MSG NODEJS---

            //TRY SEND MSG BROWSER1+++
            if (level == 1 || (level == 255 && conBROW1canWrite))
            {                
                try
                {
                    streamBROW1.Write(msgConverted, 0, msgConverted.Length);
                }
                catch
                {
                    conBROW1canWrite = false;
                    Console.WriteLine("Exception sendMsg BROWSER 111");                    
                }                
            }
            //TRY SEND MSG BROWSER1---

            //TRY SEND MSG BROWSER2+++
            if (level == 2 || (level == 255 && conBROW2canWrite))
            {
                
                try
                {
                    streamBROW2.Write(msgConverted, 0, msgConverted.Length);
                }
                catch
                {
                    conBROW2canWrite = false;
                    Console.WriteLine("Exception sendMsg BROWSER 222");
                }                
            }
            //TRY SEND MSG BROWSER2---

            //TRY SEND MSG BROWSER3+++
            if (level == 3 || (level == 255 && conBROW3canWrite))
            {                
                try
                {
                    streamBROW3.Write(msgConverted, 0, msgConverted.Length);
                }
                catch
                {
                    conBROW3canWrite = false;
                    Console.WriteLine("Exception sendMsg BROWSER 333");
                }                
            }
            //TRY SEND MSG BROWSER3---

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
            String coords;

            while (X <= 301)
            {
                if (X == 300)
                {
                    X = 0;
                    Y = 0;
                }
                X++;
                Y++;
                coords = Y.ToString() + " " + X.ToString() + " " + W.ToString() + " " + H.ToString();

                Server.sendMsg(255, coords);
                Thread.Sleep(100);
            }
        }

        /**
        * TESTE ENVIANDO DADOS PARA O BROWSER
        */
        public static void testeDataBrowser()
        {
            while (true)
            {
                String msg = "TESTE ENVIANDO DADOS";
                Server.sendMsg(0, msg);
                Console.WriteLine("TESTE ENVIANDO DADOS PARA O BROWSER");
                Thread.Sleep(100);
            }
        }

        public static void startTesteCanvasHTML()
        {
            Thread testeCanvasHTML_TH = new Thread(Server.testeCanvasHTML);
            testeCanvasHTML_TH.Start();

            /*Thread testeDataBrowser_TH = new Thread(Server.testeDataBrowser);
            testeDataBrowser_TH.Start();*/
        }




    }
}
