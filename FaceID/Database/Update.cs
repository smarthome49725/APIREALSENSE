using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID.Database
{
    class Update
    {
        string nome = "Json.Nome";
        string fone = "Json.Fone";
        string nasc = "Json.Date";
        string mail = "Json.Mail";

        static string strCn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DBAgenda.mdf;Integrated Security = True";
        SqlConnection conexao = new SqlConnection(strCn);

        private void Altera()
        {

            string altera = "update tbcontatos set Nome= '" + nome + "', Fone= '" + fone + "', Email= '" + mail + "where nome=" + nome;

            SqlCommand cmd = new SqlCommand(altera, conexao);
            try
            {
                conexao.Open();
                int resultado;
                resultado = cmd.ExecuteNonQuery();

                if (resultado == 1)
                {
                    Console.WriteLine("Registro alterado com sucesso");
                }
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
