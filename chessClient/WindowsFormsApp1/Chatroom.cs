using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    class Chatroom
    {
        private IPAddress broderCastIp = IPAddress.Parse("224.100.0.1");
        private readonly int port = 8001;
        private ListBox listbox;
        private UdpClient udpClient;
        private TextBox textBoxMessage;
        private Button KeyPress;
        private string text = "123";
        public Thread myThread;

        private delegate void SetListBoxItemCallback(string tex);
        SetListBoxItemCallback listBoxCallback;

        public Chatroom(ListBox lb, TextBox tb, Button b)
        {
            listbox = lb;
            textBoxMessage = tb;
            KeyPress = b;
            listBoxCallback = new SetListBoxItemCallback(SetListBoxItem);
            KeyPress.Click += new EventHandler(textBoxMessage_KeyPress);
            myThread = new Thread(new ThreadStart (ReceiveMessage));
            myThread.IsBackground = true;
            myThread.Start();
        }

        private void SetListBoxItem(string text)
        {
            if (listbox.InvokeRequired == true)
            {
                listbox.Invoke(listBoxCallback,text);
            }
            else
            {
                listbox.Items.Add(text);
            }
            Console.WriteLine(text);
        }


        private void ReceiveMessage()
        {
            udpClient = new UdpClient();

            udpClient.Client.SetSocketOption(
                SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, port);

            udpClient.Client.Bind(localEp);

            udpClient.JoinMulticastGroup(broderCastIp);

            udpClient.Ttl = 50;

            IPEndPoint remote = localEp;

            while (true)
            {
                try
                {
                    byte[] bytes = udpClient.Receive(ref remote);
                    string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    SetListBoxItem(string.Format(str));
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message);
                    break;
                }
            }
        }

        private void textBoxMessage_KeyPress(object sender, EventArgs e)
        {
            if (textBoxMessage.Text.Trim().Length > 0)
            {
                SendMessage(broderCastIp, "[" +Global.UserID+ "]說:" + textBoxMessage.Text);
                textBoxMessage.Text = "";
            }
        }

        private void SendMessage(IPAddress ip, string sendString)
        {
            UdpClient myUdpClient = new UdpClient();
            IPEndPoint iep = new IPEndPoint(ip, port);
            byte[] bytes = Encoding.UTF8.GetBytes(sendString);
            try
            {
                myUdpClient.Send(bytes, bytes.Length, iep);
                Console.WriteLine(bytes);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "失敗");
            }
            finally
            {
                myUdpClient.Close();
            }
        }
    }
}
