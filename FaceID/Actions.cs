using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using FaceID;

namespace FaceID
{
    class Actions
    {
        static Codigo codigo;

        public static void actions(string cod)
        {

            Console.WriteLine(cod);
            codigo = JsonConvert.DeserializeObject<Codigo>(cod);

            switch (codigo.cod)
            {
                case "rect":                    
                    rect();
                    break;
                case "registerUser":
                    registerUser();
                    break;
                case "unregisterUser":
                    unregisterUser();
                    break;
                case "geniduser":
                    MainWindow.doRegister = true;
                    Console.WriteLine("doRegister");
                    break;
                case "rmiduser":
                    MainWindow.doUnregister = true;
                    Console.WriteLine("doUnregister");                
                    break;
                case "getuser":                    
                    LoadUser(codigo.userID, codigo.nome, codigo.tel, codigo.nasc, codigo.email);
                    Console.WriteLine("READ USER-FINAL");                    
                    break;
                case "updateuser":
                    updateUser(codigo.userID, codigo.nome, codigo.tel, codigo.nasc, codigo.email);
                    Console.WriteLine("updateuser!");
                    break;
            }
        }

        static void rect()
        {
            if (codigo.level == 1)
            {
                Server.conBROW1canWrite = codigo.rect;
                if(codigo.rect)
                {
                    Console.WriteLine("Configure Realsense");
                    MainWindow.ConfigureRealSense();
                    MainWindow.processingThread.Start();               
                }
            }
            if (codigo.level == 2)
            {
                Server.conBROW2canWrite = codigo.rect;
            }
            if (codigo.level == 3)
            {
                Server.conBROW3canWrite = codigo.rect;
            }
            //Server.sendMsg(codigo.level, "Rect level" + codigo.level + ": " + codigo.rect);       
            Console.WriteLine("Rect level" + codigo.level + ": " + codigo.rect);
        }

        static void registerUser()
        {
            Console.WriteLine("registerUser true");
            //MainWindow.SaveDatabaseToFile();
            Create create = new Create();
            create.Adiciona(codigo.userID, codigo.nome, codigo.tel, codigo.nasc, codigo.email);

            Actions.LoadUser(codigo.userID);            
        }

        static void unregisterUser()
        {
            Console.WriteLine("unregisterUser");
            Delete delete = new Delete();
            delete.Deletar(codigo.userID);
        }

        public static void LoadUser(int userId=0, string nome="", string tel="", string nasc="", string email="")
        {
            Console.WriteLine("LoadUser: " + userId);
            Read reader = new Read();
            String userJSON = reader.Reader(userId, nome, tel, nasc, email);
            //Console.WriteLine(codigo.userID);            

            Server.sendMsg(255, "userData", userJSON, "");
        }

        public static void updateUser(int userId, string nome, string tel, string nasc, string email)
        {
            Update update = new Update();
            update.Alterar(userId, nome, tel, nasc, email);
        }
    }
}

class Codigo
{
    public int userID { get; set; }

    public int level { get; set; }
    public string cod { get; set; }

    public bool rect { get; set; }

    public string nome { get; set; }
    public string tel { get; set; }
    public string nasc { get; set; }
    public string email { get; set; }
}