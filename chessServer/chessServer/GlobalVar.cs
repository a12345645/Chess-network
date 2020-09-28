using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
namespace chessServer
{
    class GlobalVar
    {
        public static string clientip;
        public static ArrayList arr = new ArrayList();
        public static string temp;
       // public static bool isclose;
        public static Thread t1, t2;
        public static DataRef dataRef = new DataRef();
        
        public static int usernumber;
        public static string[] user;
        public static Dictionary<Socket, string> loginUser = new Dictionary<Socket, string>();
        public static Dictionary<string, Socket> loginUserR = new Dictionary<string, Socket>();   //uid查socket
        public static string roomData = "";
        public static List<string> haveRoomUser = new List<string>();
    }

    class GlobalMethod
    {
        public string DataFormat(string before)
        {
            string after;
            int idx = 0;
            after = "";

            while (before[idx] != '*')
            {
                after += before[idx];
                //Console.WriteLine(after[idx]);
                idx++;
            }
            return after;
        }
        public void CutAP(ref string a,ref string p,string data)
        { 
            string[] apdata = new string[2];
            apdata[0] = "";
            apdata[1] = "";
            apdata = data.Split('$');
            a = apdata[0];
            p = apdata[1];
            return;
        }
        
    }
}
