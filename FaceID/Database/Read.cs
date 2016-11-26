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

        //VS2012
        //static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security=True";
        //vs2015
        //static string strCn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security = True;";
        static string strCn = ConnectionString.getConnectionString();


        object userData;
        String userJSON;
        List<string> lista = new List<string>();

        public String Reader(int userID = 0, string nome = "?", string fone = "?", string nasc = "?", string email = "?")
        {            
            string commandText;

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

                commandText = "SELECT * FROM tbusers WHERE (" +
                        "Nome LIKE '%" + nome + "%')" +
                    " OR (tel LIKE '%" + fone + "%')" +
                    " OR (nasc LIKE '%" + nasc + "%')" +
                   " OR (email LIKE '%" + email + "%')";
            }


            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

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
                            level = reader["level"].ToString()
                        };                       

                        Console.WriteLine(userData.ToString());
                        lista.Add(JsonConvert.SerializeObject(userData));                        
                           
                       

                    }                 
                    
                    userJSON = JsonConvert.SerializeObject(lista);

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
            return userJSON;
        }
    }

}
