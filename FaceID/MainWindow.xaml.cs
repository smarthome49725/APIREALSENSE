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
using System.Drawing; //Utilizado para suportar operações com Bitmap
using System.Windows.Controls;
using System.IO; //Utilizado para I/O de arquivos etc.






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
        //Cria uma variável do tipo 'PXCMSenseManager' para interface com algoritmos pré-definidos ex: Reconhecimento facial         
        public static PXCMSenseManager senseManager;
        //Cria variável para configurar o algoritmo de reconhecimento faical
        public static PXCMFaceConfiguration.RecognitionConfiguration recognitionConfig;
        public static PXCMFaceData faceData;
        private PXCMFaceData.RecognitionData recognitionData;
        private Int32 numFacesDetected;//Guarda o número de faces detectadas
        public static string userId;
        public static string dbState;
        private const int DatabaseUsers = 10; //Numero máximo de usuários suportado pelo banco de dados        
        private const string DatabaseName = "UserDB"; //Nome do banco de dados
        private const string DatabaseFilename = "SH2DB.bin"; //Nome do arquivo do banco de dados
        public static bool doRegister;
        public static bool doUnregister;
        private int faceRectangleHeight;
        private int faceRectangleWidth;
        private int faceRectangleX;
        private int faceRectangleY;
        public static String coords;
        public static string flagUserId = null;             





        //------------------------------------------------------MainWindow--------------------------------------------------------------------------------------------------





        public MainWindow()
        {
            //WebSocket
            Server servidor = new Server();            
            Thread servidorThread = new Thread(servidor.StartServer);            
            servidorThread.Start();           
                        

            //----------------------------------------------

            InitializeComponent();

            rectFaceMarker.Visibility = Visibility.Hidden;
            chkShowFaceMarker.IsChecked = true;
            numFacesDetected = 0;
            userId = string.Empty;
            dbState = string.Empty;
            doRegister = false;
            doUnregister = false;

            // Start SenseManage and configure the face module
            //////////////ConfigureRealSense();

            // Start the worker thread
            processingThread = new Thread(new ThreadStart(ProcessingThread)); //Cria uma thread para executar os processos de reconhecimeto facial
            //////////////processingThread.Start(); //Inicia a thread que realiza o processo de reconhecimento facial

           
        }

        





        //---------------------------------------------------ConfigureRealSense-----------------------------------------------------------------------------------------------------




        public static void ConfigureRealSense() //Configura o pipeline para processamento
        {
            /* Start the SenseManager and session  
             * Cria a instancia de PXCMSenseManager em senseManager para 
             * interface com algoritmos pré-definidos ex: Reconhecimento facial
             */
            senseManager = PXCMSenseManager.CreateInstance(); //private PXCMSenseManager senseManager; 

            /* Enable the color stream
             * Habilita a transmissão de cor
             */
            ///////senseManager.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 640, 480, 30);

            /* Enable the face module
             * Ativa o Rastreamento de face no pipeline
             */
            senseManager.EnableFace();
            PXCMFaceModule faceModule = senseManager.QueryFace(); //Retorna a instância do módulo da face (FaceModule)

            /* O CreateActiveConfiguration função retorna uma nova instância do FaceConfiguration exemplo, 
             * que o aplicativo usa para configurar o módulo de rastreamento de face.
             * O pedido deve liberar a instância após o uso.              
             */
            PXCMFaceConfiguration faceConfig = faceModule.CreateActiveConfiguration();

            // Configure for 3D face tracking (if camera cannot support depth it will revert to 2D tracking)
            faceConfig.SetTrackingMode(PXCMFaceConfiguration.TrackingModeType.FACE_MODE_COLOR_PLUS_DEPTH);

            /* Enable facial recognition
             * Cria uma interface de configuração do algoritmo de reconhecimento facial             
             * Neste algoritmo já tem as opções de CRUD, só é preciso configura-las              
             */
            recognitionConfig = faceConfig.QueryRecognition(); //private PXCMFaceConfiguration.RecognitionConfiguration recognitionConfig;
            recognitionConfig.Enable(); //faceModule.CreateActiveConfiguration().QueryRecognition().Enable();           

            /* Create a recognition database
             * A estrutura 'RecognitionStorageDesc' descreve os parâmetros de configuração de banco de dados de reconhecimento.
             */
            PXCMFaceConfiguration.RecognitionConfiguration.RecognitionStorageDesc storageDesc = new PXCMFaceConfiguration.RecognitionConfiguration.RecognitionStorageDesc();
            storageDesc.maxUsers = DatabaseUsers; //Numero máximo de usuários suportado pelo banco de dados
            recognitionConfig.CreateStorage(DatabaseName, out storageDesc); //Cria o banco de dados passando a descrição configurada em 'storageDesc'            
            recognitionConfig.UseStorage(DatabaseName); //Coloca o banco de dados em uso

            /* Verifica se o arquivo existe e se existir, executa 'SetDatabaseBuffer'
             * para substitui o banco de dados de reconhecimento
             */
            LoadDatabaseFromFile();

            /* REGISTRATION_MODE_CONTINUOUS registrar os usuários automaticamente. 
             * REGISTRATION_MODE_ON_DEMAND  registar os usuários mediante solicitação.             
             */
            recognitionConfig.SetRegistrationMode(PXCMFaceConfiguration.RecognitionConfiguration.RecognitionRegistrationMode.REGISTRATION_MODE_CONTINUOUS);


            faceConfig.ApplyChanges(); // Aplica as configurações feitas no Modulo de Reconhecimento facial

            /* A função 'Init' inicializa o pipeline 
             * (Isso configura o pipeline de processamento que recebe e processa os dados do hardware)
             */
            senseManager.Init();

            /* A função 'CreateOutput' retorna uma nova instância do 'FaceData' exemplo, que o aplicativo usa para armazenar e recuperar
             *  os dados de saída do módulo de rastreamento de face.
             *  O pedido deve liberar a instância após o uso.
             */
            faceData = faceModule.CreateOutput();

            /* A função 'SetMirrorMode' define a orientação das imagens da câmera.
             */
            senseManager.QueryCaptureManager().QueryDevice().SetMirrorMode(PXCMCapture.Device.MirrorMode.MIRROR_MODE_HORIZONTAL);

            // Libera os recursos
            faceConfig.Dispose();
            faceModule.Dispose();
        }



        //--------------------------------------------------------LoadDatabaseFromFile------------------------------------------------------------------------------------------------
             
        public static void LoadDatabaseFromFile()
        {
            if (File.Exists(DatabaseFilename)) //Verifica se existe o arquivo do banco de dados no diretório corrente
            {
                //Faz uma leitura de todos os bytes contido no arquivo do banco de dados e deposita em buffer

                Byte[] buffer = File.ReadAllBytes(DatabaseFilename);

                /* A função 'SetDatabaseBuffer' substitui o banco de dados de reconhecimento com o banco de dados de origem especificado.
                 * O tamanho do banco de dados de origem deve corresponder ao que o QueryDatabaseSize função retorna.
                 */
                recognitionConfig.SetDatabaseBuffer(buffer); //Seta os dados para o processamento e continuidade do reconhecimento facial
                dbState = "Loaded"; //Seta na UI esta informação
            }
            else
            {
                dbState = "Not Found"; //Seta na UI esta informação
            }
        }

        //---------------------------------------------------------SaveDatabaseToFile-----------------------------------------------------------------------------------------------





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




        //-------------------------------------------------DeleteDatabaseFile-------------------------------------------------------------------------------------------------------




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




        //---------------------------------------------------------ProcessingThread-----------------------------------------------------------------------------------------------





        private void ProcessingThread()
        {

            /* Start AcquireFrame/ReleaseFrame loop
             * (ADS) DESCREVER O LOOP: verificar qual o retorno da função 'AcquireFrame(true)' e o retorno de 'pxcmStatus.PXCM_STATUS_NO_ERRO'            
             * The frame processing is paused between the 'AcquireFrame' function and the next 'ReleaseFrame' function.
             * 'AcquireFrame(true)' Pausa o processamento de frame, lê o frame atual e salva em algum local que é acessado pela função 'QuerySample()'.
             *  Mais abaixo o processamento de frame é liberado com a função 'ReleaseFrame()'
             */
            try
            {
                while (senseManager.AcquireFrame(true) >= pxcmStatus.PXCM_STATUS_NO_ERROR)
                {
                    /* Acquire the color image data
                     * Consulta a sample (imagem salva com a chamada da função 'AcquireFrame(true)')
                     * e Atribui para a variável sample(amostra) (Dados na forma Bruta)
                     */
                    PXCMCapture.Sample sample = senseManager.QuerySample();

                    /* Cria uma variável que é uma estrutura apropriada para receber a um tipo de imagem. No caso a imagem bruta recebida
                     * pela função 'AcquireFrame()' e convertida pela função 'color.AcquireAccess()'
                     */
                    PXCMImage.ImageData colorData;
                    /* Converte a imagem(dados brutos) retornada para Sample e salva na estrutura de 
                     * imagem ImageData por meio o ultimo parâmetro da função 'color.AcquireAccess(out colorData)' 
                     */
                    sample.color.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB24, out colorData);

                    /* Converte para Bitmap
                     */
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

                                //int faceRectangleX2 = (faceRectangleX - 510) * -1;






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
                                        Actions.AsyncLoadUser(Convert.ToInt16(userId));
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

                    // Display the color stream and other UI elements
                    UpdateUI(colorBitmap);

                    // Release resources
                    colorBitmap.Dispose();
                    sample.color.ReleaseAccess(colorData);
                    sample.color.Dispose();

                    /* Release the frame
                     * 'ReleaseFrame' libera o bloqueio sobre o quadro atual. O processamento de frame continua.
                     */
                    senseManager.ReleaseFrame();

                    coords = faceRectangleX.ToString() + " " + faceRectangleY.ToString() + " " + faceRectangleWidth.ToString() + " " + faceRectangleHeight.ToString();
                    Server.sendMsg(255, "rect", coords, userId);

                }

            }
            catch
            {
                Console.WriteLine("ERRORRRRRRRRRRRRRRRRRRRRRR");
            }
        }


        //Converte imagem em Byte array
        private byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }


        //------------------------------------------------------UpdateUI--------------------------------------------------------------------------------------------------




        private void UpdateUI(Bitmap bitmap)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate ()
            {
                // Display  the color image
                if (bitmap != null)
                {
                    imgColorStream.Source = ConvertBitmap.BitmapToBitmapSource(bitmap);
                    //Byte[] bitmapByte = ImageToByte(bitmap);
                    //servidor.sendFile(bitmapByte);


                }

                // Update UI elements
                lblNumFacesDetected.Content = String.Format("Faces Detected: {0}", numFacesDetected);
                lblUserId.Content = String.Format("User ID: {0}", userId);
                lblDatabaseState.Content = String.Format("Database: {0}", dbState); //Infoma o status do carregamento do banco de dados ('Loaded' ou 'Not Found')

                // Change picture border color depending on if user is in camera view
                if (numFacesDetected > 0)
                {
                    bdrPictureBorder.BorderBrush = System.Windows.Media.Brushes.LightGreen;
                }
                else
                {
                    bdrPictureBorder.BorderBrush = System.Windows.Media.Brushes.Red;
                }


                // Show or hide face marker
                if ((numFacesDetected > 0) && (chkShowFaceMarker.IsChecked == true))
                {
                    // Show face marker
                    if (this.flag2 == 1)
                    {
                        ////////////Server.sendMsg("detected");
                        this.flag2 = 0;
                    }

                    rectFaceMarker.Height = faceRectangleHeight;
                    rectFaceMarker.Width = faceRectangleWidth;
                    Canvas.SetLeft(rectFaceMarker, faceRectangleX);
                    Canvas.SetTop(rectFaceMarker, faceRectangleY);
                    rectFaceMarker.Visibility = Visibility.Visible;

                    // Show floating ID label
                    lblFloatingId.Content = String.Format("User ID: {0}", userId);
                    Canvas.SetLeft(lblFloatingId, faceRectangleX);
                    Canvas.SetTop(lblFloatingId, faceRectangleY - 20);
                    lblFloatingId.Visibility = Visibility.Visible;
                }
                else
                {
                    if (this.flag2 == 0)
                    {
                        //////////////////Server.sendMsg("notDetected");
                        this.flag2 = 1;
                    }
                    // Hide the face marker and floating ID label
                    rectFaceMarker.Visibility = Visibility.Hidden;
                    lblFloatingId.Visibility = Visibility.Hidden;
                }
            }));

            // Release resources
            bitmap.Dispose();
        }




        //-----------------------------------------------------ReleaseResources---------------------------------------------------------------------------------------------------



        private void ReleaseResources()
        {
            // Stop the worker thread
            processingThread.Abort();

            // Release resources
            faceData.Dispose();
            senseManager.Dispose();
        }




        //-----------------------------------------------------btnRegister_Click---------------------------------------------------------------------------------------------------




        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            doRegister = true;
        }


        //------------------------------------------------------btnUnregister_Click--------------------------------------------------------------------------------------------------



        private void btnUnregister_Click(object sender, RoutedEventArgs e)
        {
            doUnregister = true;
        }


        //------------------------------------------------------btnSaveDatabase_Click--------------------------------------------------------------------------------------------------



        private void btnSaveDatabase_Click(object sender, RoutedEventArgs e)
        {
            SaveDatabaseToFile();
        }

        public static void saveDatabaseToFile()
        {
            SaveDatabaseToFile();
        }


        //------------------------------------------------------btnDeleteDatabase_Click--------------------------------------------------------------------------------------------------




        private void btnDeleteDatabase_Click(object sender, RoutedEventArgs e)
        {
            DeleteDatabaseFile();
        }


        //------------------------------------------------------btnExit_Click--------------------------------------------------------------------------------------------------


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ReleaseResources();
            this.Close();
        }


        //------------------------------------------------------Window_Closing--------------------------------------------------------------------------------------------------

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ReleaseResources();
        }

        private void IP_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void configIP_Click(object sender, RoutedEventArgs e)
        {
            Server.ipAddress = IP.Text;             
            MessageBox.Show("ipAddress Configurado com: " + Server.ipAddress);
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
    } //public partial class MainWindow : Window
}//namespace FaceID

