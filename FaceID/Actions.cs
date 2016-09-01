using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FaceID
{
    class Actions
    {
        public static void actions(string cod)
        {
            log(cod);
            if ((new Regex("^{Save").IsMatch(cod)))
            {
                Console.WriteLine("JSON");
            }       
        }

        public static void log(string json)
        {
            //Console.WriteLine(json);

            //string json = "{ \"ID\":\"0\",\"Nome\":\"MeuNome\",\"Email\":\"123@123.com.br\",\"Nascimento\":\"110595.0\"}";
            Usuario deserializedUser = JsonConvert.DeserializeObject<Usuario>(json); /*Transforma a partir do JSON*/
            Console.WriteLine(deserializedUser.nome);
            Console.WriteLine(deserializedUser.tel);
            Console.WriteLine(deserializedUser.age);
            Console.WriteLine("----------------");
        }
    }
}

class Usuario
{
    //public Int32 ID { get; set; } //campo ID
    public string nome { get; set; } // campo nome
    public string tel { get; set; } // campo nascimento
    public decimal age { get; set; } // campo email
}