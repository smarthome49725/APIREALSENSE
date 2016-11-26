using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using FaceID;
using System.Windows;

namespace FaceID
{
    class Actions
    {
        static Codigo codigo;
        
        public static void actions(string cod)
        {  
            /*codigo.userID = 0;
            codigo.rect = false;            
            codigo.nome = "0";
            codigo.tel = "0";
            codigo.nasc = "0";
            codigo.email = "0";
            codigo.password = "0";*/

            Console.WriteLine(cod);
            codigo = JsonConvert.DeserializeObject<Codigo>(cod);

            switch (codigo.cod)
            {
                case "getlogin":
                    getLogin(codigo.login, codigo.password);
                    break;
                case "rect":
                    rect(codigo.level);
                    break;
                case "registerUser":
                    registerUser(codigo.userID, codigo.level, codigo.nome, codigo.tel, codigo.nasc, codigo.email, codigo.password, codigo.registerLevel);
                    break;
                case "unregisterUser":
                    unregisterUser(codigo.userID, codigo.level);
                    break;
                case "geniduser":
                    MainWindow.doRegister = true;
                    Console.WriteLine("doRegister");
                    break;
                case "getuser":
                    LoadUser(codigo.userID, codigo.level, codigo.nome, codigo.tel, codigo.nasc, codigo.email);
                    Console.WriteLine("READ USER-FINAL");
                    break;
                case "updateuser":
                    updateUser(codigo.userID, codigo.level, codigo.nome, codigo.tel, codigo.nasc, codigo.email, codigo.password, codigo.registerLevel);
                    Console.WriteLine("updateuser!");
                    break;
            }
        }

        static void getLogin(String login, String password)
        {            
            int level = Database.GetLogin.getLogin(login, password);            
            if (level != -1)
            {
                Server.sendMsg(255, "200", level.ToString(), "");
            }else
            {
                Server.sendMsg(255, "404", level.ToString(), "");
            }

            
        }

        static void rect(int level)
        {
            if (level == 1)
            {
                Server.conBROW1canWrite = codigo.rect;
                if (codigo.rect)
                {
                    Console.WriteLine("Configure Realsense");
                    MainWindow.ConfigureRealSense();
                    MainWindow.processingThread.Start();
                }
            }
            if (level == 2)
            {
                Server.conBROW2canWrite = codigo.rect;
            }
            if (level == 3)
            {
                Server.conBROW3canWrite = codigo.rect;
            }
            //Server.sendMsg(codigo.level, "Rect level" + codigo.level + ": " + codigo.rect);       
            Console.WriteLine("Rect level" + codigo.level + ": " + codigo.rect);
        }

        static void registerUser(int userId, int level, string nome, string tel, string nasc, string email, string password, int registerLevel)
        {
            Console.WriteLine("registerUser true");
            Create create = new Create();
            create.Adiciona(userId, nome, tel, nasc, email, password, registerLevel);
            MainWindow.SaveDatabaseToFile();

            Actions.LoadUser(userId, level);
        }

        static void unregisterUser(int userId, int level)
        {
            Console.WriteLine("unregisterUser");
            Delete delete = new Delete();
            delete.Deletar(userId);

            MainWindow.doUnregister = true;
            MainWindow.SaveDatabaseToFile();

            Actions.LoadUser(userId, level);
        }

        private static String LoadUser(int userId = 0, int level = 255, string nome = "", string tel = "", string nasc = "", string email = "")
        {
            Console.WriteLine("LoadUser: " + userId);
            Read reader = new Read();
            String userJSON = reader.Reader(userId, nome, tel, nasc, email);

            Server.sendMsg(level, "userData", userJSON, "");
            return userJSON;
        }
        
        public static Task<string> AsyncLoadUser(int userId = 0, string nome = "", string tel = "", string nasc = "", string email = "")
        {
            return Task.Run(() =>
            {
                return LoadUser(userId);
            });
        }

        public static void updateUser(int userId, int level, string nome, string tel, string nasc, string email, string password, int registerLevel)
        {
            Update update = new Update();
            update.Alterar(userId, nome, tel, nasc, email, password, registerLevel);
            Actions.LoadUser(userId, level);
        }
    }
}

class Codigo
{
    public int userID { get; set; }

    public int level { get; set; }
    public string cod { get; set; }

    public bool rect { get; set; }

    public int registerLevel { get; set; }
    public string login { get; set; }
    public string password { get; set; }
    public string nome { get; set; }
    public string tel { get; set; }
    public string nasc { get; set; }
    public string email { get; set; }
}

