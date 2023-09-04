using HAtxLib;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace UIDemo {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private async void Form1_Load(object sender, EventArgs e) {
            await Task.Run(() => {
                HAtx atx = new HAtx("98897a394f41334c4c");
                Console.WriteLine(atx.AtxAgentUrl);
                UrlBox.Text = atx.AtxAgentWs + "/minicap";
                WebSocket client = new WebSocket(UrlBox.Text);
                client.OnOpen += Client_OnOpen;
                client.OnMessage += Client_OnMessage;
                client.Connect();
            });
        }

        private void Client_OnMessage(object sender, MessageEventArgs e) {
            if (e.RawData.Length < 200) {
                return;
            }
            using (MemoryStream stream = new MemoryStream(e.RawData)) {
                pictureBox1.Image = Image.FromStream(stream);
            }
        }

        private void Client_OnOpen(object sender, EventArgs e) {
            Console.WriteLine("data");
        }

        private void GoToButton_Click(object sender, EventArgs e) {

        }
    }
}
