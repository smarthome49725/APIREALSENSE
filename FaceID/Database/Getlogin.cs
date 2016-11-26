using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace FaceID.Database
{
    class GetLogin
    {
        static object IPandPORT;
        //VS2012
        //static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security=True";
        //vs2015
        //static string strCn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security = True";
        static string strCn = ConnectionString.getConnectionString();
        static int level= -1 ;

        public static int getLogin(string login, string password)
        {
            string commandText = "SELECT * FROM tbusers WHERE email=@login and password=@password";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@login", SqlDbType.VarChar);
                command.Parameters["@login"].Value = login;

                command.Parameters.Add("@password", SqlDbType.VarChar);
                command.Parameters["@password"].Value = password;
                                
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        level = Convert.ToInt16(reader["level"]);
                    }else
                    {
                        level = -1;
                    }
            
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                connection.Close();
                connection.Dispose();
            }
            return level;
        }

    }
}

