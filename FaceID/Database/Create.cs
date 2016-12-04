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
        static string strCn = Database.ConnectionString.getConnectionString();

        public static void Adiciona(dynamic userData)
        {
            string commandText = null;
            if ((string)userData.blacklist == "True")
            {                
                commandText = "INSERT INTO tbusers (userID, Nome, blacklist) VALUES (@userID, @nome, @blacklist)";
            }
            else
            {
                commandText = "INSERT INTO tbusers (userID, Nome, Tel, Nasc, Email, password, level, blacklist, lightBathroom, lightKitchen, lightBedroom, lightRoom1, lightRoom2, TV, curtain, air_conditioning)"+
                    "VALUES (@userID, @nome, @tel, @nasc, @email, @password, @level, @blacklist, @lightBathroom, @lightKitchen, @lightBedroom, @lightRoom1, @lightRoom2, @TV, @curtain, @air_conditioning)";                                  
            }

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                if ((string)userData.blacklist == "True")
                {
                    command.Parameters.Add("@userID", SqlDbType.VarChar);
                    command.Parameters["@userID"].Value = (string)userData.userID;

                    command.Parameters.Add("@nome", SqlDbType.VarChar);
                    command.Parameters["@nome"].Value = (string)userData.nome;

                    command.Parameters.Add("@blacklist", SqlDbType.VarChar);
                    command.Parameters["@blacklist"].Value = (string)userData.blacklist;
                }
                else
                {
                    command.Parameters.Add("@userID", SqlDbType.VarChar);
                    command.Parameters["@userID"].Value = (string)userData.userID;

                    command.Parameters.Add("@nome", SqlDbType.VarChar);
                    command.Parameters["@nome"].Value = (string)userData.nome;

                    command.Parameters.Add("@tel", SqlDbType.VarChar);
                    command.Parameters["@tel"].Value = (string)userData.tel;

                    command.Parameters.Add("@nasc", SqlDbType.VarChar);
                    command.Parameters["@nasc"].Value = (string)userData.nasc;

                    command.Parameters.Add("@email", SqlDbType.VarChar);
                    command.Parameters["@email"].Value = (string)userData.email;

                    command.Parameters.Add("@password", SqlDbType.VarChar);
                    command.Parameters["@password"].Value = (string)userData.password;

                    command.Parameters.Add("@level", SqlDbType.VarChar);
                    command.Parameters["@level"].Value = (string)userData.level;

                    command.Parameters.Add("@blacklist", SqlDbType.VarChar);
                    command.Parameters["@blacklist"].Value = (string)userData.blacklist;
     

                    command.Parameters.Add("@lightBathroom", SqlDbType.VarChar);
                    command.Parameters["@lightBathroom"].Value = (string)userData.lightBathroom;

                    command.Parameters.Add("@lightKitchen", SqlDbType.VarChar);
                    command.Parameters["@lightKitchen"].Value = (string)userData.lightKitchen;

                    command.Parameters.Add("@lightBedroom", SqlDbType.VarChar);
                    command.Parameters["@lightBedroom"].Value = (string)userData.lightBedroom;

                    command.Parameters.Add("@lightRoom1", SqlDbType.VarChar);
                    command.Parameters["@lightRoom1"].Value = (string)userData.lightRoom1;

                    command.Parameters.Add("@lightRoom2", SqlDbType.VarChar);
                    command.Parameters["@lightRoom2"].Value = (string)userData.lightRoom2;

                    command.Parameters.Add("@TV", SqlDbType.VarChar);
                    command.Parameters["@TV"].Value = (string)userData.TV;

                    command.Parameters.Add("@curtain", SqlDbType.VarChar);
                    command.Parameters["@curtain"].Value = (string)userData.curtain;

                    command.Parameters.Add("@air_conditioning", SqlDbType.VarChar);
                    command.Parameters["@air_conditioning"].Value = (string)userData.air_conditioning;

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
