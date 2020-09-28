using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace WindowsFormsApp1
{
    class GameNet
    {
        Socket socket;
        //Thread inThread, outThread;
        NetworkStream stream;
        StreamReader sr;
        StreamWriter sw;
        string ip;
        int sport = 5665;
        int cport = 1011;
        public void SetIP(string ip)
        {
            this.ip = ip;
        }

        public void Listener(Socket s)
        {
            socket = s;
            Thread inThread, outThread;
            stream = new NetworkStream(s);
            sr = new StreamReader(stream);
            sw = new StreamWriter(stream);
            inThread = new Thread(new ThreadStart(Getdata));
            inThread.Start();

            outThread = new Thread(new ThreadStart(Senddata));
            outThread.Start();
            inThread.Join();
            outThread.Join();

        }


        bool getdataend = false;
        public void Getdata()
        {
            while (true)
            {
                
                String line = sr.ReadLine();
                Console.WriteLine("getting msg");
                Global.rtemp = line;
                Console.WriteLine("GET MESSAGE!!" + Global.rtemp);
                ProcessData(line);
                if (getdataend)
                {
                    return;
                }
                    
                Thread.Sleep(50);
            }
        }

        public void Senddata()
        {
            String line = "";
            while (true)
            {

                if (Global.issend == true)
                {
                    Console.WriteLine("sending msg");
                    line = Global.stemp;
                    Thread.Sleep(500);
                    sw.WriteLine(line);
                    sw.Flush();
                    Global.issend = false;
                }
                else if(Global.isend == true)
                {
                    Console.WriteLine("sending end msg");
                    line = "enddddddd";
                    sw.WriteLine(line);
                    sw.Flush();
                    return;
                }
                Thread.Sleep(50);
            }


        }

        public void ServerMain()
        {
            Console.WriteLine("creat server");//建立server提示


            //---------------------------------------取得本機IP位址
            string strHostName = Dns.GetHostName();
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Console.WriteLine("Local IP: " + ipaddress.ToString());
                }
            }
            //----------------------------------------------------------


            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, sport);
            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newsock.Bind(ipep);
            newsock.Listen(10);
            Socket client = newsock.Accept();
            Listener(client);
            newsock.Close();
            Console.WriteLine("close server");//server關閉提示
            
        }

        public void ClientMain()
        {
            Console.WriteLine("creat client");
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), sport);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(ipep);
            Listener(server);
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            Console.WriteLine("close client");//client關閉提示
        }

        private void ProcessData(string s)
        {
            string[] sArray = s.Split(' ');
            if (sArray[0] == "pnum=")
                ControlPieceMoveNet(s);
            else if (sArray[0] == "piecetypechange")
                PieceTypeChange(s);
            else if (sArray[0] == "texture")
                Texture(s);
            else if (sArray[0] == "enddddddd")
                getdataend = true;
            else if (sArray[0] == "color=")
                colorlabel(s);

        }

        private void colorlabel(string s) // 對手名字
        {
            try
            {
                string[] sArray = s.Split(' ');
                int color = int.Parse(sArray[1]);
                if (color == 1)
                    Global.texture.Invoke(Global.game.changewlabel, sArray[2]);
                if (color == -1)
                    Global.texture.Invoke(Global.game.changeblabel, sArray[2]);
            }
            catch
            {}
        }

            private void ControlPieceMoveNet(string s) // 移動旗子
        {
            string[] sArray = s.Split(' ');
            int pnum = int.Parse(sArray[1]);
            if (pnum < 0)
            {
                pnum = -pnum;
                Global.game.black[pnum - 1].piecemoveset(int.Parse(sArray[9]), int.Parse(sArray[11]));
                Global.game.black[pnum - 1].Invoke(Global.game.black[pnum - 1].piecenet);
            }
            else
            {
                Global.game.white[pnum - 1].piecemoveset(int.Parse(sArray[9]), int.Parse(sArray[11]));
                Global.game.white[pnum - 1].Invoke(Global.game.white[pnum - 1].piecenet);
            }
        }

        private void PieceTypeChange(string s)
        {
            string[] sArray = s.Split(' ');
            int pnum = int.Parse(sArray[2]);
            int type = int.Parse(sArray[3]);
            Global.rule.PieceTypeChangeSet(pnum, type);
        }

        private void Texture(string s)
        {
            string[] sArray = s.Split(' ');
            int n = int.Parse(sArray[1]);
            Global.texture.bnum = n;
            Global.texture.Invoke(Global.texture.texturenet);
        }
    }
}
