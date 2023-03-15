using SuperSimpleTcp;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using STL.CHATROOM.Domain;
using System.Net.Http;
using System.Xml.Linq;
using System.Text.Json;
using static STL.CHATROOM.Domain.ENUM_LIST;

namespace TCP_Server
{
    public partial class Server : Form
    {
        
        public Server()
        {              
            InitializeComponent();          
        }
        SimpleTcpServer server;
        List<OnlineUser> ClientLIst=new List<OnlineUser>();
        private void Form1_Load(object sender, EventArgs e)
        {
            txtIP.Text = System.Configuration.ConfigurationSettings.AppSettings["ServerIp"];
            btnSend.Enabled = false;
            server = new SimpleTcpServer(txtIP.Text);
            server.Events.ClientConnected += Events_ClientConnected;
            server.Events.ClientDisconnected += Events_ClientDisconnected;
            server.Events.DataReceived += Events_DataReceived;

        }
        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {                
                string message = Encoding.UTF8.GetString(e.Data);

                //byte[] b2 = Convert.FromBase64String(message);
                //string aa = Encoding.BigEndianUnicode.GetString(b2);

                ClsMessage msgObj = JsonSerializer.Deserialize<ClsMessage>(message);
                if (msgObj.ACTION == ACTION.NAME)
                {
                    var _client= ClientLIst.Where(x => x.CLIENTIP == e.IpPort).FirstOrDefault();
                    if (ClientLIst.Where(x=>x.NAME==msgObj.USERNAME).Count()>0)
                    {
                        server.DisconnectClient(e.IpPort);
                       
                        ClientLIst.Remove(_client);
                        return;
                    }
                    ClientLIst.Remove(_client);
                    ClientLIst.Add(new OnlineUser(e.IpPort, msgObj.USERNAME));
                    SendClientLIst();
                }
                else if (msgObj.ACTION == ACTION.MESSAGE || msgObj.ACTION == ACTION.BUZZ)
                {
                    sentMsg(message, msgObj.TO);
                }
               
            });

        }

        private void Events_ClientDisconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {               
                txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                var _client=ClientLIst.Where(x => x.CLIENTIP == e.IpPort).FirstOrDefault();
                ClientLIst.Remove(_client);
                SendClientLIst();                
            });
        }
        private void SendClientLIst()
        {

            foreach (var item in ClientLIst)
            {

                ClsMessage _msg = new();
                _msg.FROM = "";
                _msg.TO = item.CLIENTIP;
                _msg.BODY = string.Empty;
                _msg.USERNAME = string.Empty;
                _msg.Users=ClientLIst;
                _msg.ACTION = ACTION.USER_LIST;

                var jsonString = JsonSerializer.Serialize(_msg);
                sentMsg(jsonString, item.CLIENTIP);
            
            }
        }

        private void Events_ClientConnected(object sender, ConnectionEventArgs e)
        {
            
            this.Invoke((MethodInvoker)delegate
            { 
                ClientLIst.Add(new OnlineUser( e.IpPort,string.Empty));
                txtInfo.Text += $"{e.IpPort} connected.{Environment.NewLine}";  
                lblTotalCount.Text = ClientLIst.Count.ToString();                

            });
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            server.Start();
            txtInfo.Text += $"Starting...{Environment.NewLine}";
            btnStart.Enabled = false;            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string clientIpName = listClientIP.SelectedItem.ToString();
            sentMsg(txtMessage.Text, clientIpName);
            txtInfo.Text += $"{txtMessage.Text}{Environment.NewLine}";
            txtMessage.Text = string.Empty;
        }
        private void sentMsg(string msg, string clientIpName)
        {
            string clientIp = clientIpName.Split("-")[0];
            if (server.IsListening)
            {
                if (!string.IsNullOrEmpty(msg) && clientIp != null)
                {
                    server.Send(clientIp, msg);                   
                   
                }
            }
        }
        private void listClientIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSend.Enabled = true;
        }
        
    }
}