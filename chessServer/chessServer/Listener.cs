using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;



namespace chessServer
{
    
    class Listener
    {
        public static bool isclose = false;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private static Socket handler = null;
        private static ArrayList CliSockets = new ArrayList();
        private static Object thisLock = new Object();
        private static Thread worker;
        private static GlobalMethod globalMethod = new GlobalMethod();
        public static void StartListening()
        {
            byte[] bytes = new Byte[1024];
            IPAddress ipAddress = IPAddress.Parse("0.0.0.0");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 12345);

            Socket listener = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);  //建立server listener
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                
                Console.WriteLine("Waiting Client");

                GlobalVar.t2 = new Thread(new ThreadStart(WorkerThread));
                GlobalVar.t2.IsBackground = true;
                GlobalVar.t2.Start();
                while (true)
                {
                    Socket sClient = listener.Accept();
                    IPEndPoint point = sClient.RemoteEndPoint as IPEndPoint;
                    Console.WriteLine("New Client join, IP:" + point.Address.ToString());
                    CliSockets.Add(sClient);
                    GlobalVar.arr.Clear();
                    for(int a=0;a< CliSockets.Count;a++)
                    {
                        GlobalVar.arr.Add(CliSockets[a]);
                    }
                }


            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        public static void WorkerThread()
        {
            Socket socket1 = null;
            ArrayList readList = new ArrayList();
            while (true)
            {

                lock (thisLock)                //以免同時動到CliSockets
                {
                    readList.Clear();
                    for (int i = 0; i < CliSockets.Count; i++)
                    {
                        readList.Add(CliSockets[i]);                   //放入現有的cliet的socket
                    }
                }
                if (readList.Count <= 0)
                {
                    Thread.Sleep(100);
                    continue;
                }
                try
                {
                    Socket.Select(readList, null, null, 500);
                    for (int i = 0; i < readList.Count; i++)
                    {
                        socket1 = (Socket)readList[i];
                        IPEndPoint point = socket1.RemoteEndPoint as IPEndPoint;
                        string ip = point.Address.ToString();
                        string resb = "";
                        byte[] buffer = new byte[1024];

                        int recLen = socket1.Receive(buffer);
                        string temp = Encoding.Default.GetString(buffer);              //暫存接收到的資訊
                        string request = "";
                        string data, account = "", password = "";
                        if (recLen > 0)
                        {
                            request = globalMethod.DataFormat(temp);                    //將TEMP資料格式化

                            Console.WriteLine("Client" + ip + " request:" + request);
                            if (request == "Login")//----------------------------------------------------------------------------LOGIN
                            {
                                Console.WriteLine("Login");

                                resb = "get Login request*";
                                socket1.Send(Encoding.Default.GetBytes(resb));


                                recLen = socket1.Receive(buffer);
                                if (recLen > 0)
                                {
                                    temp = Encoding.Default.GetString(buffer);
                                    data = globalMethod.DataFormat(temp);
                                    Console.WriteLine(data);

                                    globalMethod.CutAP(ref account, ref password, data);

                                    Console.WriteLine("Uid = " + account);
                                    Console.WriteLine("Pwd = " + password);
                                    
                                    
                                    if (GlobalVar.dataRef.LoginCheck(account, password))
                                    {
                                        Console.WriteLine("LoGINNNNNNNNNNNNNNN");
                                        GlobalVar.usernumber += 1;

                                        if(!GlobalVar.loginUser.ContainsKey(socket1) && !GlobalVar.loginUserR.ContainsKey(account))
                                        {
                                            GlobalVar.loginUser.Add(socket1, account);
                                            GlobalVar.loginUserR.Add(account, socket1);
                                            resb = "LLogin*";
                                            socket1.Send(Encoding.Default.GetBytes(resb));
                                        }
                                        else
                                        {
                                            resb = "Logined*";
                                            socket1.Send(Encoding.Default.GetBytes(resb));
                                        }
                                        
                                    }
                                    else
                                    {
                                        resb = "error*";
                                        socket1.Send(Encoding.Default.GetBytes(resb));
                                    }
                                        
                                    
                                }

                            }//---------------------------------------------------------------------------------------------------

                            if(request == "register")//----------------------------------------------------------------註冊
                            {

                                Console.WriteLine("register");


                                recLen = socket1.Receive(buffer);
                                if (recLen > 0)
                                {

                                    
                                    temp = Encoding.Default.GetString(buffer);
                                    data = globalMethod.DataFormat(temp);
                                    Console.WriteLine(data);

                                    if(data != "break")
                                    {
                                        globalMethod.CutAP(ref account, ref password, data);

                                        Console.WriteLine("Uid = " + account);
                                        Console.WriteLine("Pwd = " + password);

                                        GlobalVar.dataRef.NewUser(account, password);
                                    }
                                    

                                    
                                    

                                }
                            }//-----------------------------------------------------------------------------

                            if(request == "Newroom")//----------------------------------開房間
                            {
                                Console.WriteLine("Newroom");
                                GlobalVar.roomData += GlobalVar.loginUser[socket1];
                                GlobalVar.roomData += ',';
                                GlobalVar.haveRoomUser.Add(GlobalVar.loginUser[socket1]);
                                
                            }

                            if(request == "roomData")//-------回傳房間
                            {

                                Console.WriteLine("roomData");
                                resb = GlobalVar.roomData + "*";
                                socket1.Send(Encoding.Default.GetBytes(resb));
                            }

                            if (request == "disconnect")
                            {
                                Console.WriteLine("disconnect");
                                string re = "";
                                if (GlobalVar.loginUser.ContainsKey(socket1))
                                {
                                    re = GlobalVar.loginUser[socket1] + ',';
                                    GlobalVar.roomData = GlobalVar.roomData.Replace(re, String.Empty);
                                    Console.WriteLine(GlobalVar.roomData);
                                }
                            }

                            if(request == "CloseRoom")
                            {
                                if (GlobalVar.haveRoomUser.Contains(GlobalVar.loginUser[socket1]))
                                {
                                    resb = "elsemsg" + "*";
                                    socket1.Send(Encoding.Default.GetBytes(resb));
                                    GlobalVar.haveRoomUser.Remove(GlobalVar.loginUser[socket1]);
                                }
                                Console.WriteLine("CloseRoom");
                                string re = "";
                                if (GlobalVar.loginUser.ContainsKey(socket1))
                                {
                                    re = GlobalVar.loginUser[socket1] + ',';
                                    GlobalVar.roomData = GlobalVar.roomData.Replace(re, String.Empty);
                                    Console.WriteLine(GlobalVar.roomData);
                                }
                            }

                            if(request == "JoinRoom")//-------------------------------加入防間
                            {
                                Console.WriteLine("JoinRoom");
                                recLen = socket1.Receive(buffer);            //取得要加入的房間
                                if (recLen > 0)
                                {


                                    temp = Encoding.Default.GetString(buffer);    
                                    data = globalMethod.DataFormat(temp);
                                    Console.WriteLine(data);

                                    resb = "GO*";
                                    Socket joinsocket = GlobalVar.loginUserR[data];
                                    joinsocket.Send(Encoding.Default.GetBytes(resb));


                                    IPEndPoint joinpoint = joinsocket.RemoteEndPoint as IPEndPoint;
                                    string joinip = joinpoint.Address.ToString() + "*";

                                    Thread.Sleep(500);
                                    socket1.Send(Encoding.Default.GetBytes(joinip));



                                }
                            }
                            
                        }
                        else
                        {
                            Console.WriteLine("有socket斷線");                 
                            for (int j = 0; j < CliSockets.Count; j++)
                            {
                                Socket s = (Socket)CliSockets[j];
                                if (s == socket1)
                                {
                                    CliSockets.RemoveAt(j);
                                    GlobalVar.arr.RemoveAt(j);
                                    if(GlobalVar.loginUser.ContainsKey(socket1))
                                        GlobalVar.loginUserR.Remove(GlobalVar.loginUser[socket1]);
                                    GlobalVar.loginUser.Remove(socket1);
                                    
                                }
                                   
                            }
                            socket1.Shutdown(SocketShutdown.Both);
                            socket1.Close();
                            break;
                        }
                        //socket1.Send(buffer, recLen, SocketFlags.None);

                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine("{0} Error code: {1}.", e.Message, e.ErrorCode);

                    for (int j = 0; j < CliSockets.Count; j++)
                    {
                        Socket s = (Socket)CliSockets[j];
                        if (s == socket1)
                        {
                            CliSockets.RemoveAt(j);
                            GlobalVar.arr.RemoveAt(j);
                        }
                           

                    }
                    socket1.Shutdown(SocketShutdown.Both);
                    socket1.Close();
                }
            }

        }


    }

    
}
