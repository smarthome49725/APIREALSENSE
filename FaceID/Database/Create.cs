using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID
{
    class Create
    {

        //VS2012
        //static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security=True";
        //vs2015
        static string strCn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\SHDB.mdf;Integrated Security = True";       
               
        public void Adiciona(string nome, string fone, string nasc, string email)
        {            
            string commandText = "INSERT INTO tbusers (Nome, Tel, Nasc, Email) VALUES (@nome, @fone, @nasc, @email)";                   

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@nome", SqlDbType.VarChar);
                command.Parameters["@nome"].Value = nome;

                command.Parameters.Add("@fone", SqlDbType.VarChar);
                command.Parameters["@fone"].Value = fone;

                command.Parameters.Add("@nasc", SqlDbType.VarChar);
                command.Parameters["@nasc"].Value = nasc;

                command.Parameters.Add("@email", SqlDbType.VarChar);
                command.Parameters["@email"].Value = email;               

                try
                {
                    connection.Open();
                    Int32 rowsAffected = command.ExecuteNonQuery();
                    Console.WriteLine("RowsAffected: {0}", rowsAffected);                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
