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
using System.Threading;

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

            switch ((string)codigo.cod)
            {
                case "getlogin":
                    getLogin((string)codigo.login, (string)codigo.password);
                    break;
                case "rect":
                    rect((int)codigo.level, (bool)codigo.rect);
                    break;
                case "registerUser":
                    registerUser(codigo);
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
                    LoadUser(0, (int)codigo.level, false, (string)codigo.nome, (string)codigo.tel, (string)codigo.nasc, (string)codigo.email);
                    Console.WriteLine("READ USER-FINAL");
                    break;
                case "updateuser":
                    updateUser(codigo);
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


        static void rect(int level, bool ONO_FF)
        {
            if (level == 1)
            {
                Server.conBROW1canWrite = ONO_FF;
                Console.WriteLine("ProcessingThread.IsAlive?: " + !MainWindow.processingThread.IsAlive);
                if (ONO_FF && !MainWindow.processingThread.IsAlive)
                {
                    MainWindow.ConfigureRealSense();
                    MainWindow.processingThread.Start();
                }
            }
            if (level == 2)
            {
                Server.conBROW2canWrite = ONO_FF;
            }
            if (level == 3)
            {
                Server.conBROW3canWrite = ONO_FF;
            }

            Console.WriteLine("Rect level" + level + ": " + ONO_FF);
        }

        static void registerUser(dynamic userData)
        {
            Task.Run(() =>
            {
                Console.WriteLine("registerUser true");
                Create.Adiciona(userData);
                MainWindow.SaveDatabaseToFile();
                Actions.LoadUser(userData.userId, userData.level);
            });
        }

        static void unregisterUser(int userId, int level)
        {
            Task.Run(() =>
            {
                Console.WriteLine("unregisterUser");
                Delete delete = new Delete();
                delete.Deletar(userId);

                MainWindow.doUnregister = true;
                MainWindow.SaveDatabaseToFile();

                Actions.LoadUser(userId, level);
            });
        }

        public static void LoadUser(int userId = 0, int level = 255, bool isCam = false, string nome = "", string tel = "", string nasc = "", string email = "")
        {
            Task.Run(() =>
            {
                Console.WriteLine("2");
                String userJSON = Read.Reader(userId, level, isCam, nome, tel, nasc, email);
                Console.WriteLine("3");
                Server.sendMsg(level, "userData", userJSON, "");
                userJSON = null;
            });
        }

        public static void updateUser(dynamic userData)
        {
            Task.Run(() =>
            {
                Update.Alterar(userData);
                Actions.LoadUser(userData.userId, userData.level);
            });
        }

        public static void updatealertemail(int level = 255, string email1 = "", string email2 = "", string email3 = "")
        {
            Update.updateAlertEmail(email1, email2, email3);
        }

        public static void getAlertemail(int level = 255)
        {
            Task.Run(() =>
            {
                object emails = Read.getAlertEmail();
                Server.sendMsg(level, "getalertemail", emails.ToString(), "");
                //Console.WriteLine("Emails Sended For Browser: " + emails);
            });
        }

        public static void getImgLogin(int level = 255, int userID = 0)
        {
            Task.Run(() =>
            {
                String imgDefault = @"..\..\IMG\users\user0.png";
                String imgUser = @"..\..\IMG\users\user" + userID + ".jpg";

                if (File.Exists(imgUser))
                {
                    Server.sendFile(level, imgUser);
                }
                else
                {
                    Server.sendFile(level, imgDefault);
                }
            });

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


