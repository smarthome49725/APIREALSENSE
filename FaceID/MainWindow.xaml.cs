//--------------------------------------------------------------------------------------
// Copyright 2015 Intel Corporation
// All Rights Reserved
//
// Permission is granted to use, copy, distribute and prepare derivative works of this
// software for any purpose and without fee, provided, that the above copyright notice
// and this statement appear in all copies.  Intel makes no representations about the
// suitability of this software for any purpose.  THIS SOFTWARE IS PROVIDED "AS IS."
// INTEL SPECIFICALLY DISCLAIMS ALL WARRANTIES, EXPRESS OR IMPLIED, AND ALL LIABILITY,
// INCLUDING CONSEQUENTIAL AND OTHER INDIRECT DAMAGES, FOR THE USE OF THIS SOFTWARE,
// INCLUDING LIABILITY FOR INFRINGEMENT OF ANY PROPRIETARY RIGHTS, AND INCLUDING THE
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.  Intel does not
// assume any responsibility for any errors which may appear in this software nor any
// responsibility to update it.
//--------------------------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Drawing;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;

namespace FaceID
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Server servidor = new Server();
        public int flag2 = 1;

        public static Thread processingThread;

        public static PXCMSenseManager senseManager;

        public static PXCMFaceConfiguration.RecognitionConfiguration recognitionConfig;
        public static PXCMFaceData faceData;
        private PXCMFaceData.RecognitionData recognitionData;
        private Int32 numFacesDetected;
        public static string userId;
        public static string dbState;
        private const int DatabaseUsers = 10;
        private const string DatabaseName = "UserDB";
        private const string DatabaseFilename = "SH2DB.bin";
        public static bool doRegister;
        public static bool doUnregister;
        private int faceRectangleHeight;
        private int faceRectangleWidth;
        private int faceRectangleX;
        private int faceRectangleY;
        public static String coords;
        public static string flagUserId = null;

        public MainWindow()
        {

            //WebSocket
            Server servidor = new Server();
            Thread servidorThread = new Thread(servidor.StartServer);
            servidorThread.Start();

            InitializeComponent();

            //TRAY ICO
            System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
            ni.Icon = new Icon(@"C:/Users/ADM/Documents/SH2/APIREALSENSE/FaceID/IMG/sh2.ico");
            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            updateUI();

            numFacesDetected = 0;
            userId = string.Empty;
            dbState = string.Empty;
            doRegister = false;
            doUnregister = false;

            // Start SenseManage and configure the face module
            ////ConfigureRealSense();

            // Start the worker thread
            processingThread = new Thread(new ThreadStart(ProcessingThread));
            ////processingThread.Start(); 

        }

        public static void ConfigureRealSense()
        {
            //Start the SenseManager and session               
            senseManager = PXCMSenseManager.CreateInstance();

            // Enable the color stream        
            senseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 640, 480, 30);

            // Enable the face module
            senseManager.EnableFace();
            PXCMFaceModule faceModule = senseManager.QueryFace();

            PXCMFaceConfiguration faceConfig = faceModule.CreateActiveConfiguration();

            // Configure for 3D face tracking (if camera cannot support depth it will revert to 2D tracking)
            faceConfig.SetTrackingMode(PXCMFaceConfiguration.TrackingModeType.FACE_MODE_COLOR_PLUS_DEPTH);

            // Enable facial recognition             
            recognitionConfig = faceConfig.QueryRecognition();
            recognitionConfig.Enable();

            //Create a recognition database             
            PXCMFaceConfiguration.RecognitionConfiguration.RecognitionStorageDesc storageDesc = new PXCMFaceConfiguration.RecognitionConfiguration.RecognitionStorageDesc();
            storageDesc.maxUsers = DatabaseUsers;
            recognitionConfig.CreateStorage(DatabaseName, out storageDesc);
            recognitionConfig.UseStorage(DatabaseName);
            LoadDatabaseFromFile();

            recognitionConfig.SetRegistrationMode(PXCMFaceConfiguration.RecognitionConfiguration.RecognitionRegistrationMode.REGISTRATION_MODE_CONTINUOUS);

            faceConfig.ApplyChanges();

            senseManager.Init();

            faceData = faceModule.CreateOutput();

            senseManager.QueryCaptureManager().QueryDevice().SetMirrorMode(PXCMCapture.Device.MirrorMode.MIRROR_MODE_HORIZONTAL);

            faceConfig.Dispose();
            faceModule.Dispose();
        }

        public static void LoadDatabaseFromFile()
        {
            if (File.Exists(DatabaseFilename))
            {
                Byte[] buffer = File.ReadAllBytes(DatabaseFilename);

                recognitionConfig.SetDatabaseBuffer(buffer);
                dbState = "Loaded";
            }
            else
            {
                dbState = "Not Found";
            }
        }

        public static void SaveDatabaseToFile()
        {
            // Allocate the buffer to save the database
            PXCMFaceData.RecognitionModuleData recognitionModuleData = faceData.QueryRecognitionModule();
            Int32 nBytes = recognitionModuleData.QueryDatabaseSize();
            Byte[] buffer = new Byte[nBytes];

            // Retrieve the database buffer
            recognitionModuleData.QueryDatabaseBuffer(buffer);

            // Save the buffer to a file
            // (NOTE: production software should use file encryption for privacy protection)
            File.WriteAllBytes(DatabaseFilename, buffer);
            dbState = "Saved";
        }

        private void DeleteDatabaseFile()
        {
            if (File.Exists(DatabaseFilename))
            {
                File.Delete(DatabaseFilename);
                dbState = "Deleted";
            }
            else
            {
                dbState = "Not Found";
            }
        }


        private void ProcessingThread()
        {
            try
            {
                while (senseManager.AcquireFrame(true) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    PXCMCapture.Sample sample = senseManager.QuerySample();

                    PXCMImage.ImageData colorData;

                    sample.color.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24, out colorData);

                    Bitmap colorBitmap = colorData.ToBitmap(0, sample.color.info.width, sample.color.info.height);

                    // Get face data
                    if (faceData != null)
                    {
                        faceData.Update();
                        numFacesDetected = faceData.QueryNumberOfDetectedFaces();

                        if (numFacesDetected > 0)
                        {
                            // Get the first face detected (index 0)
                            PXCMFaceData.Face face = faceData.QueryFaceByIndex(0);

                            // Retrieve face location data
                            PXCMFaceData.DetectionData faceDetectionData = face.QueryDetection();
                            if (faceDetectionData != null)
                            {
                                PXCMRectI32 faceRectangle;
                                faceDetectionData.QueryBoundingRect(out faceRectangle);
                                faceRectangleHeight = faceRectangle.h;
                                faceRectangleWidth = faceRectangle.w;
                                faceRectangleX = faceRectangle.x;
                                faceRectangleY = faceRectangle.y;
                            }

                            // Process face recognition data
                            if (face != null)
                            {
                                // Retrieve the recognition data instance
                                recognitionData = face.QueryRecognition();

                                // Set the user ID and process register/unregister logic
                                if (recognitionData.IsRegistered())
                                {
                                    userId = Convert.ToString(recognitionData.QueryUserID());

                                    if (flagUserId != userId)
                                    {
                                        Actions.LoadUser(Convert.ToInt16(userId), 255);
                                        flagUserId = userId;
                                    }

                                    if (doUnregister)
                                    {
                                        recognitionData.UnregisterUser();
                                        SaveDatabaseToFile();
                                        doUnregister = false;
                                    }
                                }
                                else
                                {
                                    if (doRegister)
                                    {
                                        recognitionData.RegisterUser();

                                        // Capture a jpg image of registered user
                                        colorBitmap.Save("image.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                                        doRegister = false;
                                    }
                                    else
                                    {
                                        userId = "Unrecognized";
                                    }
                                }
                            }
                        }
                        else
                        {
                            userId = "No users in view";
                        }
                    }

                    // Release resources
                    colorBitmap.Dispose();
                    sample.color.ReleaseAccess(colorData);
                    sample.color.Dispose();

                    senseManager.ReleaseFrame();

                    coords = faceRectangleX.ToString() + " " + faceRectangleY.ToString() + " " + faceRectangleWidth.ToString() + " " + faceRectangleHeight.ToString();
                    Server.sendMsg(255, "rect", coords, userId);

                }

            }
            catch
            {
                Console.WriteLine("ERRO ProcessingThread");
            }
        }



        private void btnDeleteDatabase_Click(object sender, RoutedEventArgs e)
        {
            DeleteDatabaseFile();
            system_status.Items.Add(DateTime.UtcNow + "     " + "Database Deleted!");
        }

        //Converts image to Byte array
        private byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void ReleaseResources()
        {            
            if (processingThread.IsAlive)
            {
                // Stop the worker thread
                processingThread.Abort();
                // Release resources
                faceData.Dispose();
                senseManager.Dispose();
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ReleaseResources();            
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void sendCod_Click(object sender, RoutedEventArgs e)
        {
            Server.sendMsg(255, cod.Text, msg.Text, "userID");
        }

        private void sendCoordCanvasHTML_Click(object sender, RoutedEventArgs e)
        {
            Server.startTesteCanvasHTML();
            Console.WriteLine("startTesteCanvasHTML!");
            
        }

        private void configIP_Click(object sender, RoutedEventArgs e)
        {
            Update.configureIPandPORT(IP.Text, Convert.ToInt32(PORT.Text));
            system_status.Items.Add(DateTime.UtcNow + "     " + "IP configurado para:" + IP.Text);
            system_status.Items.Add(DateTime.UtcNow + "     " + "PORTA configurada para:" + PORT.Text);
        }

        public void updateUI()
        {
            IPandPORT IPandPort = JsonConvert.DeserializeObject<IPandPORT>(Read.getIPandPort().ToString());
            IP.Text = IPandPort.IP;
            PORT.Text = IPandPort.PORT.ToString(); 

        }

        public void UpdateUITH(String msg)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
               {
                   system_status.Items.Add(DateTime.UtcNow + "     " + msg);
               }));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Simulação da Câmera lendo o usuário...");
             Actions.LoadUser(2, 255);
            //object emails = Read.getAlertEmail();
            //Console.WriteLine(emails);
            //Actions.sendAlertEmail();
            
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();            
        }
    }

}