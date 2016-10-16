using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;




namespace FaceID
{
    class Converter
    {
        //Send String
        public static Byte[] strToByte(String mess)
        {
            byte[] rawData = System.Text.ASCIIEncoding.UTF8.GetBytes(mess);

            int frameCount = 0;
            byte[] frame = new byte[10];

            frame[0] = (byte)129;

            if (rawData.Length <= 125)
            {
                frame[1] = (byte)rawData.Length;
                frameCount = 2;
            }
            else if (rawData.Length >= 126 && rawData.Length <= 65535)
            {
                frame[1] = (byte)126;
                long len = rawData.Length;
                frame[2] = (byte)((len >> 8) & (byte)255);
                frame[3] = (byte)(len & (byte)255);
                frameCount = 4;
            }
            else
            {
                frame[1] = (byte)127;
                long len = rawData.Length;
                frame[2] = (byte)((len >> 56) & (byte)255);
                frame[3] = (byte)((len >> 48) & (byte)255);
                frame[4] = (byte)((len >> 40) & (byte)255);
                frame[5] = (byte)((len >> 32) & (byte)255);
                frame[6] = (byte)((len >> 24) & (byte)255);
                frame[7] = (byte)((len >> 16) & (byte)255);
                frame[8] = (byte)((len >> 8) & (byte)255);
                frame[9] = (byte)(len & (byte)255);
                frameCount = 10;
            }

            int bLength = frameCount + rawData.Length;

            byte[] reply = new byte[bLength];

            int bLim = 0;
            for (int i = 0; i < frameCount; i++)
            {
                reply[bLim] = frame[i];
                bLim++;
            }

            for (int i = 0; i < rawData.Length; i++)
            {
                reply[bLim] = rawData[i];
                bLim++;
            }

            return reply;
            //stream.WriteAsync(reply, 0, reply.Length);


        }


        //ToBase 64
        public static string imgToBase64(string imgPath)
        {
            using (Image image = Image.FromFile(imgPath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;

                }
            }

        }

        //Decodifica a msg
        public static string decodedStr(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }

            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }

            return System.Text.ASCIIEncoding.UTF8.GetString(buffer, dataIndex, dataLength);

        }


        //private static Byte[] EncodeMessageToSend(String message)
        public static Byte[] encodeData(Byte[] data)
        {
            Byte[] response;
            //Byte[] bytesRaw = Encoding.UTF8.GetBytes(message);
            //Byte[] bytesRaw = System.IO.File.ReadAllBytes(path);
            Byte[] bytesRaw = data;


            Byte[] frame = new Byte[10];


            long indexStartRawData = -1;
            long length = bytesRaw.Length;

            frame[0] = (Byte)130;
            if (length <= 125)
            {
                frame[1] = (Byte)length;
                indexStartRawData = 2;
            }
            else if (length >= 126 && length <= 65535)
            {
                frame[1] = (Byte)126;
                frame[2] = (Byte)((length >> 8) & 255);
                frame[3] = (Byte)(length & 255);
                indexStartRawData = 4;
            }
            else
            {
                frame[1] = (Byte)127;
                frame[2] = (Byte)((length >> 56) & 255);
                frame[3] = (Byte)((length >> 48) & 255);
                frame[4] = (Byte)((length >> 40) & 255);
                frame[5] = (Byte)((length >> 32) & 255);
                frame[6] = (Byte)((length >> 24) & 255);
                frame[7] = (Byte)((length >> 16) & 255);
                frame[8] = (Byte)((length >> 8) & 255);
                frame[9] = (Byte)(length & 255);

                indexStartRawData = 10;
            }

            response = new Byte[indexStartRawData + length];

            Int64 i, reponseIdx = 0;

            //Add the frame bytes to the reponse
            for (i = 0; i < indexStartRawData; i++)
            {
                response[reponseIdx] = frame[i];
                reponseIdx++;
            }

            //Add the data bytes to the response
            for (i = 0; i < length; i++)
            {
                response[reponseIdx] = bytesRaw[i];
                reponseIdx++;
            }

            return response;
            //stream.WriteAsync(response, 0, response.Length);
            //stream.Write(response, 0, response.Length);
        }


        /* public void streamPhoto()
        {

            //Teste de frame --------------------------------------------------------------------
            int i = 0;
            string bufferPath = "frames2\\";
            string path;
            string ext = ".png";

            while (true)
            {
                i++;
                path = bufferPath + i.ToString() + ext;
                if (i == 6)
                {
                    i = 1;
                }
                Console.WriteLine(File.Exists(path).ToString());
                //MessageBox.Show(path);
                if (File.Exists(path))
                {
                    Console.WriteLine(i);
                    byte[] imgFormated = sendFile(path);
                    this.stream.WriteAsync(imgFormated, 0, imgFormated.Length);
                    this.stream.Flush();
                    Thread.Sleep(100);

                }
            }
        }*/





    }
}
