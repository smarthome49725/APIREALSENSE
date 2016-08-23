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
        string msg;

        public void StartServer()
        {
            TcpListener server = null;

            // Set the TcpListener on port 80.
            Int32 port = 80;
            IPAddress host = IPAddress.Parse("192.168.1.123");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(host, port);

            // Start listening for client requests.
            // Começa a escultar as requisições de clientes
            server.Start();

            int cont = 0; //contar as mensagens

            //enter to an infinite cycle to be able to handle every change in stream
            while (true)
            {
                //Console.WriteLine("\n\n*-----------------Aguardadndo conexão--------------*\n");

                // Realizar uma chamada de bloqueio para aceitar solicitações. 
                // Você também poderia utilizador server.AcceptSocket () aqui.
                TcpClient client = server.AcceptTcpClient();
                stream = client.GetStream();

                cont++; //contar as msg's
                while (true)
                {
                    while (!stream.DataAvailable) ; //true Se houver dados disponíveis no stream a ser lido; Caso contrário, false.
                    
                    //(TcpClient.Available) Obtém a quantidade de dados que tenham sido recebidos a partir da rede e está disponível para ser lido.
                    // Abaixo é utilizado para definir o tamanho do array de bytes
                    Byte[] bytes = new Byte[client.Available];

                    stream.Read(bytes, 0, bytes.Length);

                    // translate bytes of request to string            
                    String data = Encoding.UTF8.GetString(bytes);
                    //Console.WriteLine("\n*------------MSG: " + cont.ToString() + " " + data.GetType() + "------------------*" + Environment.NewLine);

                    //Se a msg não inicia com GET, envia para ser decodificada com msg normal
                    if (!(new Regex("^GET").IsMatch(data)))
                    {
                        msg = Converter.decodedStr(bytes, bytes.Length);
                        //Funções de interrupção e flag' 
                        Actions.actions(msg); 
                    }

                    
                    
                    /* Verifica se a mensagem inicia com a string "GET", se iniciar, subtende-se que a mesma é um cabeçalho HTTP
                     * solicitando um handshack
                     */
                    if (new Regex("^GET").IsMatch(data))
                    {
                        string statusResponse;
                        Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                        + "Connection: Upgrade" + Environment.NewLine
                        + "Upgrade: websocket" + Environment.NewLine
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

                        Console.WriteLine("\n\n+++-----------RESPONSE-----------+++\n\n" + statusResponse);


                    }                        

                }
            }
        }   
        
        public static void sendMsg(String mess)
        {
            Byte[] msgConverted = Converter.strToByte(mess);
            stream.WriteAsync(msgConverted, 0, msgConverted.Length);
        }               
       
    }
}