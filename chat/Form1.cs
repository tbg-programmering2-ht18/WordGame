using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace chat
{
    //Thx to "Csharp Tutorials" at https://youtu.be/X16IyNbcAr0

    public partial class Form1 : Form
    {
        private TcpClient client; //Listens for connections from TCP network clients.
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string textToSend;
        Dictionary<string, int> data = new Dictionary<string, int>();
        string[] names_send = { "", "host:  ", "client:  " };
        string[] names_recieve = { "", "client:  ", "host:  " };


        // KNAS?
        public Form1()
        {
            InitializeComponent();
            //The name localhost normally resolves to the IPv4 loopback address 127.0.0.1, and to the IPv6 loopback address ::1
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress adress in localIP)
            {
                //Checks for ipV4
                if (adress.AddressFamily == AddressFamily.InterNetwork)
                {
                    edtServerIP.Text = adress.ToString();
                }
            }
            data.Add("player1", 0);
            data.Add("player2", 0);
            data.Add("maxscore", 0);
            data.Add("winner", 0);
            data.Add("gamestate", 0);
            data.Add("User", 0);
            
        }
        
        private void btnServerStart_Click(object sender, EventArgs e)
        {
            try
            {
                //Check with ex Wireshark 
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(edtServerPort.Text));
                listener.Start();
                this.BackColor = Color.Green;
                this.Update();
                redtHistory. ("Server started" + "\n");
                redtHistory.Update();
                client = listener.AcceptTcpClient(); //Accept a pending connection request 
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
                data["user"] = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnClientStart_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient();

                //Represents a network endpoint as an IP address and a port number.
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(edtClientIP.Text), int.Parse(edtClientPort.Text));

                //Connects the client to a remote TCP host using the specified host name and port number.
                client.Connect(ipEnd);
                if (client.Connected)
                {
                    this.BackColor = Color.Green;
                    this.Update();
                    redtHistory.AppendText("Connected to Server" + "\n");
                    redtHistory.Update();
                    STR = new StreamReader(client.GetStream());
                    STW = new StreamWriter(client.GetStream());
                    STW.AutoFlush = true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;
                    data["user"] = 2;
                    trackBar1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    if (recieve.StartsWith("start"))
                    {
                        string[] list = recieve.Split();
                        start(list[1]);
                    }
                    else
                    {
                        this.redtHistory.Invoke(new MethodInvoker(delegate ()
                        {
                            redtHistory.AppendText(names_recieve[data["user"]] + recieve + "\n");
                        }));
                        if (data["gamestate"] == 1)
                        {

                        }
                    }
                    recieve = "";
           
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(textToSend);
                this.redtHistory.Invoke(new MethodInvoker(delegate ()
                {
                    redtHistory.AppendText(names_send[data["user"]] + textToSend + "\n");
                }));
                if(data["gamestate"] == 1)
                {
                    data["player1"]++;
                    this.lblP1.Invoke(new MethodInvoker(delegate ()
                    {
                        lblP1.Text = data["player1"].ToString();
                    }));
                }
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            backgroundWorker2.CancelAsync();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (edtToSend.Text != "")
            {
                textToSend = edtToSend.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            edtToSend.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int wordint = rnd.Next(0, 13);
            string[] word = { "Albertopolis", "bingle", "clepsydra", "defervescence",
                "divagate", "ergometer", "famulus", "futhark", "glaikit", "higgler", "humdudgeon",
                "martlet", "nainsook", "orrery"};
            textToSend = "start " + word[wordint];
            backgroundWorker2.RunWorkerAsync();
            start(word[wordint]);
        }

        private void start(string word)
        {
            if (data["winner"] == 0)
            {
                redtHistory.AppendText("5 \n");
                redtHistory.Update();
                System.Threading.Thread.Sleep(1000);
                redtHistory.AppendText("4 \n");
                redtHistory.Update();
                System.Threading.Thread.Sleep(1000);
                redtHistory.AppendText("3 \n");
                redtHistory.Update();
                System.Threading.Thread.Sleep(1000);
                redtHistory.AppendText("2 \n");
                redtHistory.Update();
                System.Threading.Thread.Sleep(1000);
                redtHistory.AppendText("1 \n");
                redtHistory.Update();
                System.Threading.Thread.Sleep(1000);
                redtHistory.AppendText("0 \n");
                redtHistory.Update();
                System.Threading.Thread.Sleep(1000);

                //redtHistory.Text = word;
                data["gamestate"] = 1;

            }
            else
            {
                MessageBox.Show("Game is already over");
            }

        }
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            label4.Text = trackBar1.Value.ToString();
            data["maxscore"] = trackBar1.Value;
            redtHistory.AppendText("maxscore set to: " + data["maxscore"].ToString() + "\n");
        }
    }
}
