using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Virus_Antivirus
{
    public partial class Form1 : Form
    {
        private const string VIRUS_SIGNATURE = "EDU_VIRUS_SIGNATURE_2024_EDUCATIONAL_ONLY";
        private List<string> infectedFiles = new List<string>();

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Keylogger'ı durdur
            StopKeylogger();
            
            // Otomatik tarama başlat
            System.Threading.Thread.Sleep(300);
            btnScan_Click(null, null);
        }

        private string FindTestFolder()
        {
            try
            {
                // Tüm olası konumları kontrol et
                List<string> searchPaths = new List<string>();
                
                // 1. Kendi klasöründe
                searchPaths.Add(Path.Combine(Application.StartupPath, "TestFolder"));
                
                // 2. Parent klasörde
                var parent = Directory.GetParent(Application.StartupPath);
                if (parent != null)
                {
                    searchPaths.Add(Path.Combine(parent.FullName, "TestFolder"));
                    
                    // 3. VirusCreator'ın bin\Debug klasöründe
                    var grandParent = parent.Parent;
                    if (grandParent != null)
                    {
                        searchPaths.Add(Path.Combine(grandParent.FullName, "VirusCreator", "bin", "Debug", "TestFolder"));
                        searchPaths.Add(Path.Combine(grandParent.FullName, "VirusCreator", "bin", "Release", "TestFolder"));
                        
                        // 4. Solution root'ta
                        var solutionRoot = grandParent.Parent;
                        if (solutionRoot != null)
                        {
                            searchPaths.Add(Path.Combine(solutionRoot.FullName, "TestFolder"));
                        }
                    }
                }
                
                // 5. keylog.txt dosyasını bul ve klasörünü al
                try
                {
                    string[] allFiles = Directory.GetFiles(
                        Path.Combine(Application.StartupPath, "..", "..", ".."),
                        "keylog.txt",
                        SearchOption.AllDirectories);
                    
                    if (allFiles.Length > 0)
                    {
                        string keylogPath = allFiles[0];
                        string keylogDir = Path.GetDirectoryName(keylogPath);
                        searchPaths.Add(keylogDir);
                    }
                }
                catch { }
                
                // Tüm yolları kontrol et
                foreach (string path in searchPaths)
                {
                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                    {
                        return path;
                    }
                }
            }
            catch { }

            return null;
        }

        private void StopKeylogger()
        {
            try
            {
                string testFolder = FindTestFolder();
                if (testFolder != null)
                {
                    string stopFile = Path.Combine(testFolder, "stop_keylogger.txt");
                    File.WriteAllText(stopFile, "STOP");
                }
            }
            catch { }
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            try
            {
                infectedFiles.Clear();
                
                // TestFolder'ı bul
                string scanPath = FindTestFolder();
                
                if (scanPath == null || !Directory.Exists(scanPath))
                {
                    MessageBox.Show(
                        "TestFolder bulunamadı!\n\n" +
                        "Önce VirusCreator uygulamasını çalıştırın.",
                        "Klasör Bulunamadı",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Klasörü tara
                ScanDirectory(scanPath);

                if (infectedFiles.Count > 0)
                {
                    // Önce temizle, sonra rapor göster
                    List<string> filesBeforeClean = new List<string>(infectedFiles);
                    PerformClean(scanPath, filesBeforeClean);
                }
                else
                {
                    MessageBox.Show(
                        "Virüs bulunamadı. Sistem temiz!",
                        "Temiz",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Hata: {ex.Message}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ScanDirectory(string directory)
        {
            try
            {
                // Tüm txt dosyalarını tara
                string[] files = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
                
                foreach (string file in files)
                {
                    try
                    {
                        string content = File.ReadAllText(file);
                        if (content.Contains(VIRUS_SIGNATURE))
                        {
                            infectedFiles.Add(file);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Tarama hatası: {ex.Message}");
            }
        }

        private void PerformClean(string scanPath, List<string> filesToReport)
        {
            try
            {
                // ÖNCE rapor için dosyaları kopyala (temizlenmeden önce)
                List<string> filesForReport = new List<string>(filesToReport);
                
                int cleanedCount = 0;

                foreach (string file in filesToReport)
                {
                    try
                    {
                        string fileName = Path.GetFileName(file);
                        
                        // Keylogger dosyası ise tamamen sil
                        if (fileName.Equals("keylog.txt", StringComparison.OrdinalIgnoreCase))
                        {
                            if (File.Exists(file))
                            {
                                File.Delete(file);
                                cleanedCount++;
                            }
                        }
                        else
                        {
                            // Diğer dosyalardan virüs imzasını kaldır
                            if (File.Exists(file))
                            {
                                string content = File.ReadAllText(file);
                                if (content.Contains(VIRUS_SIGNATURE))
                                {
                                    content = content.Replace(VIRUS_SIGNATURE, "");
                                    content = content.TrimEnd();
                                    File.WriteAllText(file, content);
                                    cleanedCount++;
                                }
                            }
                        }
                    }
                    catch { }
                }

                // Detaylı rapor ekranını göster (temizlenmeden önceki dosya listesi ile)
                if (filesForReport.Count > 0)
                {
                    ResultForm resultForm = new ResultForm(filesForReport, scanPath ?? "Bilinmiyor");
                    resultForm.ShowDialog();
                }
            }
            catch { }
        }
    }
}
