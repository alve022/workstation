using STL.CHATROOM.Domain;
using SuperSimpleTcp;
using System.Media;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static STL.CHATROOM.Domain.ENUM_LIST;
using System.Drawing.Drawing2D;

namespace TCPClientApp
{
    public partial class Client : Form
    {       
        public Client()
        {
            InitializeComponent();
        }
        SimpleTcpClient tcpclient;
        List<OnlineUser> ClientLIst = new List<OnlineUser>();
        private void Form1_Load(object sender, EventArgs e)
        {  
            txtIP.Text = System.Configuration.ConfigurationSettings.AppSettings["ClientIp"];

            tcpclient = new SimpleTcpClient(txtIP.Text);
            tcpclient.Events.Connected += Events_Connected;
            tcpclient.Events.DataReceived += Events_DataReceived;
            tcpclient.Events.Disconnected += Events_Disconnected;
            
            btnSend.Enabled = false;
            btnBuzz.Enabled = false;             
           
            lblVersion.Hide();
            lblNewVersion.Hide();
            txtMessage.Enabled = false;

            //==================ListBox Design======================================
            listClientIP.ItemHeight = 30;
            listClientIP.IntegralHeight = false;
            listClientIP.Height = 334;
            listClientIP.DrawMode=System.Windows.Forms.DrawMode.OwnerDrawFixed;

            //==================ListBox Design======================================    
            //============================Copy Batch File to Appdata Folder============================
            string fileName = "bat.bat";
            string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            string sourcePath = programFiles + "\\STL\\STLCHATROOM";
            string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            System.IO.File.Copy(sourceFile, destFile, true);
            //============================Copy Batch File to Appdata Folder============================
            this.ActiveControl = txtName;
            checkUpdate();            
        }
        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server disconnected.{Environment.NewLine}";
                MessageBox.Show("User Name Exist Try Another !!!");
                btnConnect.Enabled = true;
                txtMessage.Enabled = false;

            });
        }

        private void Events_DataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate {
                string message = Encoding.UTF8.GetString(e.Data);
                ClsMessage msgObj = JsonSerializer.Deserialize<ClsMessage>(message);
                if (msgObj.ACTION == ACTION.MESSAGE || msgObj.ACTION == ACTION.BUZZ)
                {
                    
                    txtInfo.Text += $"{msgObj.USERNAME} : {msgObj.BODY}{Environment.NewLine}";
                    if (msgObj.ACTION == ACTION.BUZZ) {
                        buzzShake();
                    }
                    notifyIcon.BalloonTipText = "You have a Message from " + msgObj.USERNAME;
                    notifyIcon.ShowBalloonTip(2000);

                }
                else if (msgObj.ACTION == ACTION.USER_LIST)
                {
                    ClientLIst = msgObj.Users;
                    populateUserList();
                }
                listClientIP.SelectedItems.Add(msgObj.USERNAME);

            });
            
        }

        private void populateUserList()
        {
            listClientIP.Items.Clear();
            foreach (var item in ClientLIst)
            {
                if (!item.NAME.Contains(txtName.Text))
                {
                    listClientIP.Items.Add(item.NAME);
                }
            }
            
        }

        private void Events_Connected(object sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"You are connected.{Environment.NewLine}";
                               
            });

        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (tcpclient.IsConnected)
            {
                if (!string.IsNullOrEmpty(txtMessage.Text) && listClientIP.SelectedItem != "")
                {
                    ClsMessage _msg = new();
                    _msg.FROM = tcpclient.LocalEndpoint.ToString();
                    _msg.TO = ClientLIst.Where(x=>x.NAME== listClientIP.SelectedItem.ToString()).FirstOrDefault().CLIENTIP;
                    _msg.BODY = txtMessage.Text;
                    _msg.USERNAME = txtName.Text;
                    _msg.ACTION = ACTION.MESSAGE;
                    var jsonString = JsonSerializer.Serialize(_msg);
                    SendData(jsonString);                    
                    txtInfo.Text += $"Me : {txtMessage.Text}{Environment.NewLine}";                    
                    txtMessage.Text = string.Empty;
                }
            }
        }

        private void SendData(string _msg)
        {
            tcpclient.Send(_msg);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {

                    if (txtName.Text != null && txtName.Text != "")
                    {
                        tcpclient.Connect();
                        btnConnect.Enabled = false;
                        txtMessage.Enabled = true;
                        ClsMessage _msg = new();
                        _msg.FROM = tcpclient.LocalEndpoint.ToString();
                        _msg.TO = string.Empty;
                        _msg.BODY = string.Empty;
                        _msg.USERNAME = txtName.Text;
                        _msg.ACTION = ACTION.NAME;
                        var jsonString = JsonSerializer.Serialize(_msg);
                        SendData(jsonString);                                           
                    }
                    else
                    {
                        MessageBox.Show("Please Enter Your Name");
                    }
            
            }
            catch (Exception ex)
            {              
                MessageBox.Show("Server is not Started !!!!");
            }
        }

        private void listClientIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSend.Enabled = true;
            btnBuzz.Enabled = true;            
        }
        private void Client_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;                
            }
        }
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {           
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIcon.Visible = false;
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (listClientIP.SelectedItem != null && listClientIP.SelectedItem != "")
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSend_Click(sender, e);
                }
            }
            else
            {
                MessageBox.Show("Please Select User");
            }
        }

        private void btnBuzz_Click(object sender, EventArgs e)
        {
            if (tcpclient.IsConnected)
            {
                if (listClientIP.SelectedItem != null && listClientIP.SelectedItem != "")
                {
                    tcpclient.Connect();
                    btnConnect.Enabled = false;
                    ClsMessage _msg = new();
                    _msg.FROM = tcpclient.LocalEndpoint.ToString();
                    _msg.TO = ClientLIst.Where(x => x.NAME == listClientIP.SelectedItem.ToString()).FirstOrDefault().CLIENTIP;
                    _msg.BODY = "Buzzzzzzz";
                    _msg.USERNAME = txtName.Text;
                    _msg.ACTION = ACTION.BUZZ;
                    var jsonString = JsonSerializer.Serialize(_msg);
                    SendData(jsonString);                    
                    txtInfo.Text += $"Me: {_msg.BODY}{Environment.NewLine}";
                }
            }

        }

        private void buzzShake()
        {
            this.WindowState = FormWindowState.Minimized;
            if (this.WindowState != FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon.Visible = false;                
            }

            for (int i = 0; i < 5; i++)
            {
                this.Left += 10;
                System.Threading.Thread.Sleep(75);
                this.Left -= 10;
                System.Threading.Thread.Sleep(75);

                string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string soundlocation = Path.Combine(executableLocation, "messenger_buzz.wav");                
                SoundPlayer buzzSound = new SoundPlayer(soundlocation);
                buzzSound.Play();                
            }
        }

        private void checkUpdate()
        {           
            string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");            
            string sourcePath = programFiles + "\\STL\\STLCHATROOM";

            string curentVersion= System.Configuration.ConfigurationSettings.AppSettings["CurrentVersion"];
            var urlVersion = "http://localhost/update/newversion.txt";
            var newVersion = (new WebClient().DownloadString(urlVersion));

            newVersion = newVersion.Replace(".", "");
            curentVersion = curentVersion.Replace(".", "");

            this.Invoke(new Action(() => {

                if (Convert.ToInt32(newVersion) > Convert.ToInt32(curentVersion))
                   {
                        lblVersion.Text = curentVersion;
                        lblNewVersion.Text = (new WebClient().DownloadString(urlVersion));
                        btnUpdate.Enabled = true;
                        lblVersion.Show();
                        lblNewVersion.Show();
                        btnConnect.Hide();
                        btnSend.Hide();
                        btnBuzz.Hide();                        

                    }
                else
                    {
                        btnUpdate.Hide();
                        lblVersion.Hide();
                        lblNewVersion.Hide();
                        btnConnect.Show();
                        btnSend.Show ();
                        btnBuzz.Show();
                    }
                }));
           
        }
        private void initScript()
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string path = appDataFolder + @"\bat.bat";            
            System.Diagnostics.Process p = new System.Diagnostics.Process();            
            p.StartInfo.FileName = path;            
            p.StartInfo.Arguments = "";            
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.Verb = "runas";           
            p.Start();            
            Environment.Exit(1);

        }
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            path = Path.Combine(path, "Downloads");
            string dlocation = path + "\\update.msi";

            WebClient web = new WebClient();
            web.DownloadFileCompleted += Web_DownloadFileCompleted;
            web.DownloadFileAsync(new Uri("http://localhost/update/update.msi"), dlocation);           
        }

        private void Web_DownloadFileCompleted(object? sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            initScript();
        }
        Color b_color;
        Color h_color = Color.Green;
        private void listClientIP_DrawItem(object sender, DrawItemEventArgs e)
        {
            b_color = e.BackColor;
            Color clr = Color.FromArgb(0, b_color);
            Brush bb = new LinearGradientBrush(e.Bounds, b_color, clr, 1200);

            if (e.Index >= 0)
            {
                SolidBrush sb = new SolidBrush(((e.State & DrawItemState.Selected) == DrawItemState.Selected) ? h_color : b_color);
                e.Graphics.FillRectangle(sb, e.Bounds);
                e.Graphics.FillRectangle(bb, e.Bounds);
                string txt = listClientIP.Items[e.Index].ToString();
                SolidBrush tb = new SolidBrush(e.ForeColor);
                e.Graphics.DrawString(txt,e.Font,tb,listClientIP.GetItemRectangle(e.Index).Location);
            }
        }

       
    }
    
}