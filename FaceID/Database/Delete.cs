using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceID
{
    class Delete
    {

        //VS2012
        //static string strCn = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security=True";
        //vs2015
        static string strCn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\Database\SHDB.mdf;Integrated Security = True";

        public void Deletar(int userID)
        {
            string commandText = "DELETE * FROM tbusers WHERE userID=@userID";

            using (SqlConnection connection = new SqlConnection(strCn))
            {
                SqlCommand command = new SqlCommand(commandText, connection);

                command.Parameters.Add("@userID", SqlDbType.Int);
                command.Parameters["@userID"].Value = userID;

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
