using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSender
{
    public partial class MainForm : Form
    {
        const int PORT = 1723;

        public MainForm()
        {
            InitializeComponent();
            textBox2.Click += textBox2_Click;
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        string[] files;
        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            files = (string[])e.Data.GetData(DataFormats.FileDrop);

            button1_Click(null, null);
        }

        void resetControls()
        {
            hostBox.Enabled = textBox2.Enabled = button1.Enabled = true;
            button1.Text = "Send";
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Continuous;
        }

        void textBox2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = ofd.FileName;
            }
        }



        
        string curfilename = "";
        private async void button1_Click(object sender, EventArgs e)
        {
            IPAddress address;
            FileInfo file;
            int chunksize = 1000000;
            chunksCount = 0;
            sended = 0;
            hostBox.Enabled = textBox2.Enabled = button1.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            
            // Parsing
            button1.Text = "Preparing...";
           
            if (!IPAddress.TryParse(hostBox.Text, out address))
            {
                MessageBox.Show("Error with IP Address");
                resetControls();
                return;
            }
            try
            {
                var r = await Task.Run(() =>
                {
                    ParallelOptions po2 = new ParallelOptions(); po2.MaxDegreeOfParallelism = 2;
                    Parallel.For(0, files.Length , po2, (i2, loopstate2) =>
                    {
                        string curfile = files[i2];
                        Console.WriteLine(curfile);
                        Dictionary<int, Remote.SharedType.Chunk> chunks = new Dictionary<int, Remote.SharedType.Chunk>();  

                        long len;
                        int chunkcount;
                        file = new FileInfo(curfile);
                        using (FileStream fs = File.Open(curfile, FileMode.Open, FileAccess.Read))
                        using (BufferedStream bs = new BufferedStream(fs, chunksize))
                        {
                            len = fs.Length;
                            if (len < chunksize)
                            {
                                chunkcount = 1;
                                chunksCount += 1;
                            }
                            else
                            {
                                double chkcnt = len / chunksize;
                                chkcnt = Math.Ceiling(chkcnt);
                                chunkcount = (int)chkcnt + 1;
                                chunksCount += chunkcount;
                            }
                            byte[] buffer = new byte[chunksize];
                            int bytesRead;
                            int num = 0;

                            //read chunks
                            try
                            {
                                while ((bytesRead = bs.Read(buffer, 0, chunksize)) != 0) //reading only 50mb chunks at a time
                                {
                                    //var stream = new BinaryReader(new MemoryStream(buffer));
                                    var chk = new Remote.SharedType.Chunk();
                                    chk.startposition = num * chunksize;
                                    chk.len = bytesRead;
                                    chk.data = new byte[bytesRead];
                                    chk.filename = Path.GetFileName(curfile);
                                    Buffer.BlockCopy(buffer, 0, chk.data, 0, bytesRead);
                                    chk.num = num;
                                    if (chk.data == null)
                                    {
                                        MessageBox.Show("ERROR DATA IS NULL");
                                    }
                                    chunks.Add(num, chk);
                                    num++;
                                    RepProgress(num, chunkcount);
                                    GC.Collect();
                                }
                                ParallelOptions po = new ParallelOptions(); po.MaxDegreeOfParallelism = 10;
                                Parallel.For(0, chunks.Count, po, (i, loopstate) =>
                                {

                                ret1: try
                                    {
                                        Remote.Client client = new Remote.Client(hostBox.Text, int.Parse(portBox.Text));
                                        var res = client.RemoteObject.WriteChunk(chunks[i]);
                                        if (res == "OK")
                                        {
                                            sended += 1;
                                            chunks[i].data = null;
                                        }
                                        else
                                        {
                                            Console.WriteLine(res);
                                            goto ret1;
                                        }
                                    }
                                    catch (Exception eee)
                                    {
                                        Console.WriteLine(eee.Message + eee.StackTrace + eee.Source);
                                        goto ret1;
                                    }
                                });
                                chunksCount -= chunkcount;
                                sended -= chunkcount;
                            }
                            catch (Exception ee)
                            {
                            }
                        }
                    });
                    return "OK";
                });
            }
            catch
            {
                MessageBox.Show("Error opening file");
                resetControls();
                return;
            }

            
            // Connecting
            button1.Text = "Connecting...";

            

            // Sending
            button1.Text = "Sending...";
            progressBar1.Style = ProgressBarStyle.Continuous;
            
            //MessageBox.Show("Sending complete!");
            resetControls();
        }
        int chunksCount = 0;
        int sended = 0;

        private void RepProgress(int num, int chunkcount)
        {
           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            portBox.Text = File.ReadAllText("Port.inf");
            hostBox.Text = File.ReadAllText("Host.inf");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = sended.ToString() + "\\" + chunksCount.ToString();
            textBox2.Text = curfilename;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.WriteAllText("Port.inf", portBox.Text.Trim());
            File.WriteAllText("Host.inf", hostBox.Text.Trim());
        }
    }
}
