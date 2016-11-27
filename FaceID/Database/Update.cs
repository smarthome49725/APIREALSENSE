using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID
{
    class Update
    {
        static object IPandPORT;
        
        static string strCn = ConnectionString.getConnectionString();

        public static void Alterar(int userID = 0, string nome = "", string fone = "", string nasc = "", string email = "", string password = "", int level = 0, string blacklist = "false")
        {         
            string commandText = "UPDATE tbusers SET nome=@nome, tel=@fone, nasc=@nasc, email=@email, password=@password, level=@level, blacklist=@blacklist WHERE userID=@userID";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

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

        
        public static void configureIPandPORT(string IP, int PORT)
        {
            string commandText = "UPDATE config SET IP=@IP, PORT=@PORT WHERE ID=1";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@IP", SqlDbType.VarChar);
                command.Parameters["@IP"].Value = IP;

                command.Parameters.Add("@PORT", SqlDbType.Int);
                command.Parameters["@PORT"].Value = PORT;

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


        public static void updateAlertEmail(string email1, string email2, string email3)
        {
            string commandText = "UPDATE contact_email SET email1=@email1, email2=@email2, email3=@email3 WHERE ID=1";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@email1", SqlDbType.VarChar);
                command.Parameters["@email1"].Value = email1;

                command.Parameters.Add("@email2", SqlDbType.VarChar);
                command.Parameters["@email2"].Value = email2;

                command.Parameters.Add("@email3", SqlDbType.VarChar);
                command.Parameters["@email3"].Value = email3;

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
