using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Virus_Antivirus
{
    public partial class ResultForm : Form
    {
        public ResultForm(List<string> infectedFiles, string scanPath)
        {
            InitializeComponent();
            DisplayResults(infectedFiles, scanPath);
        }

        private void DisplayResults(List<string> infectedFiles, string scanPath)
        {
            txtResults.Clear();
            
            string report = "═══════════════════════════════════════════════════\n";
            report += "           VİRÜS TARAMA RAPORU\n";
            report += "═══════════════════════════════════════════════════\n\n";
            report += $"Tarama Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
            report += $"Taranan Klasör: {scanPath}\n";
            report += $"Bulunan Virüs Sayısı: {infectedFiles.Count}\n";
            report += "\n═══════════════════════════════════════════════════\n\n";

            int index = 1;
            foreach (string file in infectedFiles)
            {
                try
                {
                    string fileName = Path.GetFileName(file);
                    FileInfo fileInfo = new FileInfo(file);
                    
                    report += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                    report += $"VİRÜS #{index}\n";
                    report += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                    report += $"Dosya Adı: {fileName}\n";
                    report += $"Tam Yol: {file}\n";
                    report += $"Oluşturulma: {fileInfo.CreationTime:dd.MM.yyyy HH:mm:ss}\n";
                    report += $"Değiştirilme: {fileInfo.LastWriteTime:dd.MM.yyyy HH:mm:ss}\n";
                    report += $"Boyut: {fileInfo.Length} byte\n";
                    
                        // Virüs türünü belirle
                    if (fileName.Equals("keylog.txt", StringComparison.OrdinalIgnoreCase))
                    {
                        report += $"Virüs Türü: KEYLOGGER\n";
                        report += $"Tehlike Seviyesi: YÜKSEK ⚠\n";
                        report += $"Açıklama: Klavye girişlerini kaydeden zararlı yazılım\n";
                        
                        // Keylog içeriğini oku (dosya silinmiş olabilir, o yüzden try-catch)
                        try
                        {
                            if (File.Exists(file))
                            {
                                string keylogContent = File.ReadAllText(file);
                                int lineCount = keylogContent.Split('\n').Length;
                                int charCount = keylogContent.Length;
                                
                                report += $"\nELE GEÇİRİLEN BİLGİLER:\n";
                                report += $"  • Kaydedilen karakter sayısı: {charCount}\n";
                                report += $"  • Kayıt satır sayısı: {lineCount}\n";
                                
                                // İlk 1000 karakteri göster
                                if (keylogContent.Length > 1000)
                                {
                                    report += $"\nKayıt Önizlemesi (İlk 1000 karakter):\n";
                                    report += $"─────────────────────────────────────────\n";
                                    report += $"{keylogContent.Substring(0, 1000)}\n";
                                    report += $"... (devamı var, toplam {keylogContent.Length} karakter)\n";
                                }
                                else
                                {
                                    report += $"\nKayıt İçeriği:\n";
                                    report += $"─────────────────────────────────────────\n";
                                    report += $"{keylogContent}\n";
                                }
                            }
                            else
                            {
                                report += $"\nELE GEÇİRİLEN BİLGİLER:\n";
                                report += $"  • Dosya zaten temizlendi\n";
                                report += $"  • Keylogger kayıtları silindi\n";
                            }
                        }
                        catch
                        {
                            report += $"  • Dosya okunamadı (zaten temizlenmiş olabilir)\n";
                        }
                    }
                    else
                    {
                        report += $"Virüs Türü: DOSYA ENFEKSİYONU\n";
                        report += $"Tehlike Seviyesi: ORTA\n";
                        report += $"Açıklama: Dosyaya virüs imzası eklenmiş\n";
                        
                        // Dosya içeriğini oku
                        try
                        {
                            string content = File.ReadAllText(file);
                            report += $"\nDosya İçeriği:\n";
                            if (content.Length > 300)
                            {
                                report += $"{content.Substring(0, 300)}...\n";
                            }
                            else
                            {
                                report += $"{content}\n";
                            }
                        }
                        catch
                        {
                            report += $"  • Dosya okunamadı\n";
                        }
                    }
                    
                    report += $"\nTemizleme Durumu: BAŞARILI\n";
                    report += $"Temizleme Zamanı: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
                    report += "\n";
                    
                    index++;
                }
                catch (Exception ex)
                {
                    report += $"Hata: {ex.Message}\n\n";
                }
            }

            report += "═══════════════════════════════════════════════════\n";
            report += $"TOPLAM {infectedFiles.Count} VİRÜS TEMİZLENDİ\n";
            report += $"Temizleme Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n";
            report += "═══════════════════════════════════════════════════\n";

            txtResults.Text = report;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

