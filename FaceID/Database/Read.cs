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

        static string strCn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DBAgenda.mdf;Integrated Security = True";
        SqlConnection conexao = new SqlConnection(strCn);
        SqlDataReader DR;

        private void Reader(string @string)
        {
            string pesquisa = "select * from tbcontatos where name = " + @string;
            SqlCommand cmd = new SqlCommand(pesquisa, conexao);
            try
            {
                // Abrindo a conexão com o banco
                conexao.Open();
                DR = cmd.ExecuteReader();
                // Se houver um registro correspondente ao Id
                if (DR.Read())
                {
                    var id = DR.GetValue(0).ToString();
                    var pNome = DR.GetValue(1).ToString();
                    var pFone = DR.GetValue(2).ToString();
                    var pMail = DR.GetValue(3).ToString();
                }
                else
                {
                    Console.WriteLine("Registro não encontrado");
                }
                DR.Close();
                cmd.Dispose();
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
