# Virus-Antivirus Eğitim Projesi


## Proje İçeriği

**VirusCreator:** Arka planda çalışan keylogger simülasyonu. Klavye girişlerini kaydeder ve test dosyaları oluşturur.

**Antivirus:** Otomatik virüs tarama, detaylı rapor ve temizleme yapan uygulama.

## Kurulum

1. Visual Studio 2022 ile `Virus-Antivirus.sln` dosyasını açın
2. Build → Rebuild Solution
3. .NET Framework 4.8 gerekli

## Kullanım

**VirusCreator:**
- Projeyi başlangıç projesi yapın ve F5 ile çalıştırın
- Arka planda otomatik başlar, keylogger klavye girişlerini kaydeder

**Antivirus:**
- Projeyi başlangıç projesi yapın ve F5 ile çalıştırın
- "Virüs Tara" butonuna tıklayın
- Otomatik tarama, temizleme ve detaylı rapor ekranı açılır

## Özellikler

- Otomatik keylogger başlatma/durdurma
- Virüs imzası tabanlı tespit (EDU_VIRUS_SIGNATURE_2024_EDUCATIONAL_ONLY)
- Detaylı rapor: virüs türü, tarih/saat, ele geçirilen bilgiler
- Otomatik temizleme: keylog.txt silinir, diğer dosyalardan imza kaldırılır

## Dosya Yapısı

TestFolder klasöründe keylog.txt ve test dosyaları oluşturulur. Antivirus bu klasörü tarar ve temizler.

**⚠️ Bu uygulamayı yalnızca eğitim amaçlı kullanın.**
