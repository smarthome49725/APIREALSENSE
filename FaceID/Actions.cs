using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using FaceID.Database;

namespace FaceID
{
    class Actions
    {
        public static void actions(string cod)
        {

            Console.WriteLine(cod);

            if (new Regex("^registerUser").IsMatch(cod))
            {
                Console.WriteLine("registerUser true");
                string @string = cod.Replace("registerUser", "");
                var json = log(@string);

                Create create = new Create();
                create.Adiciona(/*json.nome, json.tel, json.age*/);
            }

            if (new Regex("^unregisterUser").IsMatch(cod))
            {
                Console.WriteLine("unregisterUser");
                string s = cod.Replace("unregisterUser", "");
                Console.WriteLine(s);
            }


            /*if (new Regex("^LEVEL1:onrectON").IsMatch(cod))
            {                
                Console.WriteLine(cod);
            }*/

            switch (cod)
            {
                case "LEVEL1:rectON":
                    Server.conBROW1canWrite = true;
                    Console.WriteLine("switch LEVEL1:onrectON");
                    break;
                case "LEVEL2:rectON":
                    Server.conBROW2canWrite = true;
                    Console.WriteLine("switch LEVEL2:onrectON");
                    break;
                case "LEVEL3:rectON":
                    Server.conBROW3canWrite = true;
                    Console.WriteLine("switch LEVEL3:onrectON");
                    break;
                case "LEVEL1:rectOFF":
                    Server.conBROW1canWrite = false;
                    Console.WriteLine("switch LEVEL1:onrectOFF");
                    break;
                case "LEVEL2:rectOFF":
                    Server.conBROW2canWrite = false;
                    Console.WriteLine("switch LEVEL2:onrectOFF");
                    break;
                case "LEVEL3:rectOFF":
                    Server.conBROW3canWrite = false;
                    Console.WriteLine("switch LEVEL3:onrectOFF");
                    break;
            }



        }

        public static Usuario log(string json)
        {
            //Console.WriteLine(json);

            //string json = "{ \"ID\":\"0\",\"Nome\":\"MeuNome\",\"Email\":\"123@123.com.br\",\"Nascimento\":\"110595.0\"}";
            Usuario deserializedUser = JsonConvert.DeserializeObject<Usuario>(json); /*Transforma a partir do JSON*/

            /*Console.WriteLine(deserializedUser.nome);
            Console.WriteLine(deserializedUser.tel);
            Console.WriteLine(deserializedUser.age);
            
            Console.WriteLine("----------------");*/
            return deserializedUser;
        }
    }
}

class Usuario
{
    //public Int32 ID { get; set; } //campo ID
    public string nome { get; set; } // campo nome
    public string tel { get; set; } // campo nascimento
    public string age { get; set; } // campo email
}