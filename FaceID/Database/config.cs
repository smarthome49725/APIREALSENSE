using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace FaceID.Database
{
    class config
    {
        static object IPandPORT;
        //VS2012
        //static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security=True";
        //vs2015
        //static string strCn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security = True";
        static string strCn = ConnectionString.getConnectionString();

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


        public static object getIPandPort(int ID = 1)
        {            
            //string objIPandPORT;
            string commandText = "SELECT * FROM config WHERE ID=1";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        IPandPORT = new
                        {
                            IP = reader["IP"].ToString(),
                            PORT = reader["PORT"].ToString(),
                        };                                              
                    }
                   
                    IPandPORT = JsonConvert.SerializeObject(IPandPORT);
                     
                    // Call Close when done reading.
                    reader.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                connection.Close();
                connection.Dispose();
            }            
            return IPandPORT;
        }
    }
}

