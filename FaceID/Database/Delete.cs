using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID.Database
{
    class Delete
    {
        string nome = "Json.Nome";
        string fone = "Json.Fone";
        string nasc = "Json.Date";
        //VS2012
        static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\SHDB.mdf;Integrated Security=True";
        //vs2015
        //static string strCn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\SHDB.mdf;Integrated Security = True";
        SqlConnection conexao = new SqlConnection(strCn);

        private void Deletar()
        {
            string deleta = "delete * from tbuser where nome= " + nome;

            SqlCommand cmd = new SqlCommand(deleta, conexao);

            try
            {
                // Abrindo a conexão com o banco
                conexao.Open();
                // Criando uma variável para adicionar e armazenar o resultado
                int resultado;
                var op = Console.ReadLine();
                if (op.ToLower() == "s")
                {
                    resultado = cmd.ExecuteNonQuery();
                    if (resultado == 1)
                    {
                        Console.WriteLine("Registro removido com sucesso");
                    }
                    cmd.Dispose();
                }
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
