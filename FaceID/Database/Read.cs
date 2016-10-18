using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID.Database
{
    class Read
    {
        string nome = "Json.Nome";
        string fone = "Json.Fone";
        string nasc = "Json.Date";

        //VS2012
        static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\SHDB.mdf;Integrated Security=True";
        //vs2015
        //static string strCn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SHDB.mdf;Integrated Security = True";
        SqlConnection conexao = new SqlConnection(strCn);
        SqlDataReader DR;

        private void Reader(string @string)
        {
            string pesquisa = "SELECT * FROM tbusers WHERE Usuario = '"+@string+"' or LIKE '%"+@string.Substring(0, 3)+"%'";
            SqlCommand cmd = new SqlCommand(pesquisa, conexao);
            try
            {
                // Abrindo a conexão com o banco
                conexao.Open();
                DR = cmd.ExecuteReader();
                // Se houver um registro correspondente ao Id
                List<string> lista = new List<string>();
                while (DR.Read())
                {
                    lista.Add(DR["Usuario"].ToString());
                }
                Console.WriteLine("{0}", lista);
            }
            catch (Exception ex)
            {
                //exiba qual é o erro
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conexao.Close();
            }
        }
    }
}
