using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using FaceID;
using System.Windows;
using System.IO;

namespace FaceID
{
    class Actions
    {
        static dynamic codigo;

        public static void actions(string cod)
        {
            Console.WriteLine(cod);

            codigo = JsonConvert.DeserializeObject<dynamic>(cod);

            Console.WriteLine(codigo);

            String code = codigo.cod;

            switch (code)
            {
                case "getlogin":
                    getLogin((string)codigo.login, (string)codigo.password);
                    break;
                case "rect":
                    rect((int)codigo.level);
                    break;
                case "registerUser":                    
                    registerUser((int)codigo.userID, (int)codigo.level, (string)codigo.nome, (string)codigo.tel, (string)codigo.nasc, (string)codigo.email, (string)codigo.password, (int)codigo.registerLevel, (string)codigo.blacklist);
                    break;
                case "registerUserBL":
                    registerUser((int)codigo.userID, (int)codigo.level, (string)codigo.nome, "", "", "", "", 0, (string)codigo.blacklist);
                    break;
                case "unregisterUser":
                    unregisterUser((int)codigo.userID, (int)codigo.level);
                    break;
                case "geniduser":
                    MainWindow.doRegister = true;
                    Console.WriteLine("doRegister");
                    break;
                case "getuser":
                    Console.WriteLine("1");
                    
                    Console.WriteLine((string)codigo.nome);
                    Console.WriteLine((string)codigo.tel);
                    Console.WriteLine((string)codigo.nasc);
                    Console.WriteLine((string)codigo.email);              
                    LoadUser(0, (int)codigo.level, (string)codigo.nome, (string)codigo.tel, (string)codigo.nasc, (string)codigo.email);
                    Console.WriteLine("READ USER-FINAL");
                    break;
                case "updateuser":
                    updateUser((int)codigo.userID, (int)codigo.level, (string)codigo.nome, (string)codigo.tel, (string)codigo.nasc, (string)codigo.email, (string)codigo.password, (int)codigo.registerLevel, (string)codigo.blacklist);
                    Console.WriteLine("updateuser!");
                    break;
                case "updatealertemail":
                    updatealertemail((int)codigo.level, (string)codigo.email1, (string)codigo.email2, (string)codigo.email3);
                    Console.WriteLine("updatealertemail!");
                    break;
                case "getalertemail":
                    getAlertemail((int)codigo.level);
                    break;
                case "getimglogin":
                    getImgLogin((int)codigo.level, (int)codigo.userID);
                    break;
            }
            codigo = null;
        }


        static void getLogin(String login, String password)
        {             
            dynamic userData = Read.getLogin(login, password);
            Console.WriteLine(userData);            
            if (userData == "null")
            {                
                Server.sendMsg(255, "404", userData, "");
            }
            else
            {                
                Server.sendMsg(255, "200", userData, "");
               
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

            Console.WriteLine("Rect level" + codigo.level + ": " + codigo.rect);
        }

        static void registerUser(int userId = 0, int level = 255, string nome = "", string tel = "", string nasc = "", string email = "", string password = "", int registerLevel = 0, string blacklist = "")
        {
            Console.WriteLine("registerUser true");
            Create.Adiciona(userId, nome, tel, nasc, email, password, registerLevel, blacklist);
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

        public static void LoadUser(int userId = 0, int level = 255, string nome = "", string tel = "", string nasc = "", string email = "")
        {
            Task.Run(() =>
            {
                Console.WriteLine("2");
                String userJSON = Read.Reader(userId, level, nome, tel, nasc, email);
                Console.WriteLine("3");
                Server.sendMsg(level, "userData", userJSON, "");
                userJSON = null;
            });
        }

        public static void updateUser(int userId = 0, int level = 0, string nome = "", string tel = "", string nasc = "", string email = "", string password = "", int registerLevel = 0, string blacklist = "false")
        {
            Update.Alterar(userId, nome, tel, nasc, email, password, registerLevel, blacklist);
            Actions.LoadUser(userId, level);
        }

        public static void updatealertemail(int level = 255, string email1 = "", string email2 = "", string email3 = "")
        {
            Console.WriteLine(email1);
            Update.updateAlertEmail(email1, email2, email3);
        }

        public static void getAlertemail(int level = 255)
        {
            object emails = Read.getAlertEmail();
            Server.sendMsg(level, "getalertemail", emails.ToString(), "");
            //Console.WriteLine("Emails Sended For Browser: " + emails);
        }

        public static void getImgLogin(int level = 255, int userID = 0)
        {
            String imgDefault = @"C:\Users\ADM\Documents\SH2\APIREALSENSE\FaceID\IMG\users\user0.png";
            String imgUser = @"C:\Users\ADM\Documents\SH2\APIREALSENSE\FaceID\IMG\users\user" + userID + ".jpg";
            
            if (File.Exists(imgUser))
            {
                Server.sendFile(level, imgUser);
            }
            else
            {
                Server.sendFile(level, imgDefault);
            }
            
        }

        public static void sendAlertEmail()
        {
            Task.Run(() =>
            {
                object emails = Read.getAlertEmail();
                dynamic JSONemails = JsonConvert.DeserializeObject(emails.ToString());

                Console.WriteLine((string)JSONemails.email1);
                Server.sendMail("smtp.gmail.com", 587, true, "smarthome49725@gmail.com", "ninja49725*", "smarthome49725@gmail.com", (String)JSONemails.email1, (String)JSONemails.email2, (String)JSONemails.email3, "SH2", "<h1>SUSPEITO</h1>", true);
            });

        }
    }
}


