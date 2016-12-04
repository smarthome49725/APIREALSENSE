using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows;

namespace FaceID
{
    class Read
    {
        static object IPandPORT;
        static object emails;
        static int level = -1;

        static string strCn = Database.ConnectionString.getConnectionString();

        //static String userJSON;
        static List<string> lista = new List<string>();

        public static dynamic Reader(int userID = 0, int level = 255, bool isCam = false, string nome = "?", string fone = "?", string nasc = "?", string email = "?")
        {
            dynamic userData = null;
            string commandText = null;

            if (userID != 0)
            {
                commandText = "SELECT * FROM tbusers WHERE userID=@userID";
            }
            else
            {
                nome = nome == "" ? "?" : nome;
                fone = fone == "" ? "?" : fone;
                nasc = nasc == "" ? "?" : nasc;
                email = email == "" ? "?" : email;

                if (nome == "?" && fone == "?" && nasc == "?" && email == "?")
                {
                    nome = ""; fone = ""; nasc = ""; email = "";
                }


                commandText = "SELECT * FROM tbusers WHERE (" +
                        "Nome LIKE '%" + nome + "%')" +
                    " OR (tel LIKE '%" + fone + "%')" +
                    " OR (nasc LIKE '%" + nasc + "%')" +
                   " OR (email LIKE '%" + email + "%')";
            }


            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                //User detected 1by camera
                if (userID != 0)
                {
                    command.Parameters.Add("@userID", SqlDbType.Int);
                    command.Parameters["@userID"].Value = userID;
                }

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();


                    while (reader.Read())
                    {
                        userData = new
                        {
                            userID = reader["userID"].ToString(),
                            nome = reader["nome"].ToString(),
                            tel = reader["tel"].ToString(),
                            nasc = reader["nasc"].ToString(),
                            email = reader["email"].ToString(),
                            level = reader["level"].ToString(),
                            blacklist = reader["blacklist"].ToString(),

                            lightBathroom = reader["lightBathroom"].ToString(),
                            lightKitchen = reader["lightKitchen"].ToString(),
                            lightBedroom = reader["lightBedroom"].ToString(),
                            lightRoom1 = reader["lightRoom1"].ToString(),
                            lightRoom2 = reader["lightRoom2"].ToString(),
                            curtain = reader["curtain"].ToString(),
                            TV = reader["TV"].ToString(),
                            TV_Increase = reader["TV_Increase"].ToString(),
                            air_conditioning = reader["air_conditioning"].ToString()
                        };


                        //User detected by camera
                        if (isCam == true && userData.blacklist == "True")
                        {
                            Console.WriteLine("SUSPEITO DETECTED!");
                            Actions.sendAlertEmail();
                        }
                        else if (isCam == true)
                        {   
                            Server.sendMsg(0, "PORT", JsonConvert.SerializeObject(userData), (string)userData.userID);
                        }

                        Console.WriteLine(userData.ToString());
                        lista.Add(JsonConvert.SerializeObject(userData));

                    }

                    userData = JsonConvert.SerializeObject(lista);

                    lista.Clear();

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
            return userData;
        }


        public static object getAlertEmail()
        {
            string commandText = "SELECT * FROM contact_email WHERE ID=1";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        emails = new
                        {
                            email1 = reader["email1"].ToString(),
                            email2 = reader["email2"].ToString(),
                            email3 = reader["email3"].ToString()
                        };

                    }

                    emails = JsonConvert.SerializeObject(emails);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                connection.Close();
                connection.Dispose();
            }
            return emails;
        }



        public static dynamic getLogin(string login, string password)
        {
            dynamic userData = null;
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
                    while (reader.Read())
                    {
                        userData = new
                        {
                            userID = reader["userID"].ToString(),
                            nome = reader["nome"].ToString(),
                            tel = reader["tel"].ToString(),
                            nasc = reader["nasc"].ToString(),
                            email = reader["email"].ToString(),
                            level = reader["level"].ToString(),
                            blacklist = reader["blacklist"].ToString()
                        };
                    }
                    userData = JsonConvert.SerializeObject(userData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                connection.Close();
                connection.Dispose();
            }

            return userData;
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
