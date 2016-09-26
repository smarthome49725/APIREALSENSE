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

        static string strCn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DBAgenda.mdf;Integrated Security = True";
        SqlConnection conexao = new SqlConnection(strCn);

        private void Deletar()
        {
            string deleta = "delete from tbcontatos where nome= " + nome;
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
