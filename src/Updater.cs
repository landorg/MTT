using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace MTTApp
{
    internal static class Updater
    {
        private const string ApiUrl = "https://api.github.com/repos/landorg/MTT/releases/latest";

        internal static string VersionString =>
            Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        private static Version CurrentVersion =>
            Assembly.GetExecutingAssembly().GetName().Version;

        internal static void CheckAsync(Form owner)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                try
                {
                    using (var wc = new WebClient())
                    {
                        wc.Headers.Add("User-Agent", "MTT-Updater");
                        string json = wc.DownloadString(ApiUrl);
                        var rel = JObject.Parse(json);
                        string tag = (string)rel["tag_name"];
                        if (tag == null) return;
                        if (!Version.TryParse(tag.TrimStart('v'), out Version latest)) return;
                        if (latest <= CurrentVersion) return;
                        string downloadUrl = null;
                        foreach (var asset in rel["assets"])
                            if ((string)asset["name"] == "MTT.exe")
                            { downloadUrl = (string)asset["browser_download_url"]; break; }
                        if (downloadUrl != null)
                            e.Result = new object[] { latest, downloadUrl };
                    }
                }
                catch { }
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result == null) return;
                var res = (object[])e.Result;
                var latest = (Version)res[0];
                string url = (string)res[1];
                var ans = MessageBox.Show(
                    $"Version {latest.ToString(3)} verfügbar.\nJetzt aktualisieren?",
                    "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ans == DialogResult.Yes)
                    ApplyUpdate(url);
            };
            worker.RunWorkerAsync();
        }

        private static void ApplyUpdate(string downloadUrl)
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string dir = Path.GetDirectoryName(exePath);
            string updatePath = Path.Combine(dir, "MTT_update.exe");
            string script = Path.Combine(Path.GetTempPath(), "mtt_update.bat");
            try
            {
                using (var wc = new WebClient())
                    wc.DownloadFile(downloadUrl, updatePath);

                File.WriteAllText(script,
                    "@echo off\r\n" +
                    "timeout /t 2 /nobreak >nul\r\n" +
                    $"copy /Y \"{updatePath}\" \"{exePath}\"\r\n" +
                    $"start \"\" \"{exePath}\"\r\n" +
                    $"del \"{updatePath}\"\r\n" +
                    "del \"%~f0\"\r\n");

                Process.Start(new ProcessStartInfo("cmd.exe", $"/c \"{script}\"")
                    { WindowStyle = ProcessWindowStyle.Hidden });
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update fehlgeschlagen: {ex.Message}", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
