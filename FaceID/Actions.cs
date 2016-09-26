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