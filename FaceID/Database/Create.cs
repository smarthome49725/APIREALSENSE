using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID.Database
{
    class Create
    {
        static string strCn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\DataBase.mdf;Integrated Security = True";
        SqlConnection conexao = new SqlConnection(strCn);

        public void Adiciona(string @User="oi", string @fone="123", string @nasc="123456")
        {
            string adiciona = "INSERT INTO [tbuser] (Usuario,Tel,Nasc) VALUES (" + @User + "," + @fone + "," + @nasc+");";

            try
            {
                SqlCommand cmd = new SqlCommand(adiciona, conexao);

                conexao.Open();
                int resultado;
                resultado = cmd.ExecuteNonQuery();

                if (resultado == 1)
                {
                    Console.WriteLine("Registro adicionado com sucesso");
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
