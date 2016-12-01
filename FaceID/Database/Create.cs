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
        //static string strCn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security = True";
        static string strCn = Database.ConnectionString.getConnectionString();

        public static void Adiciona(int userID, string nome, string fone, string nasc, string email, string password, int level, string blacklist)
        {
            string commandText = null;
            if (blacklist == "True")
            {                
                commandText = "INSERT INTO tbusers (userID, Nome, blacklist) VALUES (@userID, @nome, @blacklist)";
            }
            else
            {
                commandText = "INSERT INTO tbusers (userID, Nome, Tel, Nasc, Email, password, level, blacklist) VALUES (@userID, @nome, @fone, @nasc, @email, @password, @level, @blacklist)";
            }

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                if (blacklist == "true")
                {
                    command.Parameters.Add("@userID", SqlDbType.Int);
                    command.Parameters["@userID"].Value = userID;

                    command.Parameters.Add("@nome", SqlDbType.VarChar);
                    command.Parameters["@nome"].Value = nome;

                    command.Parameters.Add("@blacklist", SqlDbType.VarChar);
                    command.Parameters["@blacklist"].Value = blacklist;
                }
                else
                {
                    command.Parameters.Add("@userID", SqlDbType.Int);
                    command.Parameters["@userID"].Value = userID;

                    command.Parameters.Add("@nome", SqlDbType.VarChar);
                    command.Parameters["@nome"].Value = nome;

                    command.Parameters.Add("@fone", SqlDbType.VarChar);
                    command.Parameters["@fone"].Value = fone;

                    command.Parameters.Add("@nasc", SqlDbType.VarChar);
                    command.Parameters["@nasc"].Value = nasc;

                    command.Parameters.Add("@email", SqlDbType.VarChar);
                    command.Parameters["@email"].Value = email;

                    command.Parameters.Add("@password", SqlDbType.VarChar);
                    command.Parameters["@password"].Value = password;

                    command.Parameters.Add("@level", SqlDbType.Int);
                    command.Parameters["@level"].Value = level;

                    command.Parameters.Add("@blacklist", SqlDbType.VarChar);
                    command.Parameters["@blacklist"].Value = blacklist;

                }

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
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
