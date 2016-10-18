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
        public void Adiciona(string nome, string fone, string nasc, string email)
        {
            string sCreateDatabase = "CREATE DATABASE SHDB";
            string sCreateTable = "CREATE TABLE tbusers (UserID INTEGER PRIMARY KEY IDENTITY," +
                "Nome VARCHAR(50) NOT NULL , Telefone VARCHAR(20), " +
                "Email VARCHAR(50),Nascimento VARCHAR(30),LampPorta VARCHAR(3),LampCorredor VARCHAR(3)," +
                "LampCozinha VARCHAR(3),LampBanheiro VARCHAR(3), LampQuarto VARCHAR(3)," +
                "TempAr INTEGER, StatusTV VARCHAR(3), StatusCortina VARCHAR(3))";
            string sInsertRow = "INSERT INTO tbusers (Nome,Telefone,Email,Nascimento,LampPorta,"+
            "LampCorredor,LampBanheiro,LampQuarto,TempAr,StatusTV,StatusCortina)"
                + "VALUES('@Nome','@Fone', '@Email', '@Nasc', 'On', '', 'On', 'On', 'On', 27, 'On', 'On');";

            SqlConnection mycon = new SqlConnection();
            mycon.ConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Mostratec\Documents\SH2\APIREALSENSE\FaceID\DataBase\SHDB.mdf;Integrated Security=True";

            SqlCommand mycomm = new SqlCommand();
            mycomm.CommandType = CommandType.Text;
            mycomm.CommandText = sCreateDatabase;
            mycomm.Connection = mycon;


            try
            {
                //    Open the connection
                mycon.Open();
                //    Execute CreateDatabase query
                mycomm.ExecuteNonQuery();
            }
            catch
            {
                //    Catch any errors and show the error message
                Console.WriteLine("Banco de dados existente");
            }
            finally
            {
                mycon.Close();
            }

            mycon.ConnectionString =
              "workstation id=;initial catalog=shdb; integrated security=true";
            try
            {
                //    Open the connection
                mycon.Open();
                //    Execute CreateTable query
                mycomm.CommandText = sCreateTable;
                mycomm.ExecuteNonQuery();
                //    Execute InsertFirstRow query
                if (String.IsNullOrEmpty(nome))
                {
                    mycomm.Parameters.AddWithValue("@Nome", DBNull.Value);
                }
                else
                {
                    mycomm.Parameters.AddWithValue("@Nome", nome);
                }
                if (String.IsNullOrEmpty(fone)) 
                {
                    mycomm.Parameters.AddWithValue("@Fone", DBNull.Value);
                } else 
                {
                    mycomm.Parameters.AddWithValue("@Fone", fone);
                }
                if (String.IsNullOrEmpty(email))
                {
                    mycomm.Parameters.AddWithValue("@Email", DBNull.Value);
                }
                else
                {
                    mycomm.Parameters.AddWithValue("@Email", fone);
                }
                if (String.IsNullOrEmpty(nasc))
                {
                    mycomm.Parameters.AddWithValue("@Nasc", DBNull.Value);
                }
                else
                {
                    mycomm.Parameters.AddWithValue("@Email", email);
                }
                mycomm.CommandText = sInsertRow;
                mycomm.ExecuteNonQuery();
            }
            catch
            {
                //    Catch any errors and show the error message
                Console.WriteLine(" Tabela existente no banco de dados ");
                try
                {
                    mycomm.Parameters.AddWithValue("@Nome", nome);
                    mycomm.Parameters.AddWithValue("@Fone", fone);
                    mycomm.Parameters.AddWithValue("@Email", email);
                    mycomm.Parameters.AddWithValue("@Nasc", nasc);

                    mycomm.CommandText = sInsertRow;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            finally
            {
                mycon.Close();
            }
        }
    }
}
