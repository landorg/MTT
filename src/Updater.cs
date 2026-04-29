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

        private static Version CurrentVersion
        {
            get
            {
                var v = Assembly.GetExecutingAssembly().GetName().Version;
                return new Version(v.Major, v.Minor, v.Build);
            }
        }

        internal static void CheckAsync(MTT owner)
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
                        if (tag == null) { e.Result = "no tag_name in response"; return; }
                        if (!Version.TryParse(tag.TrimStart('v'), out Version latest))
                        { e.Result = $"could not parse version from tag '{tag}'"; return; }
                        if (latest <= CurrentVersion)
                        { e.Result = $"up to date (latest={tag}, current={VersionString})"; return; }
                        string downloadUrl = null;
                        foreach (var asset in rel["assets"])
                            if ((string)asset["name"] == "MTT.exe")
                            { downloadUrl = (string)asset["browser_download_url"]; break; }
                        if (downloadUrl == null)
                        { e.Result = $"update {tag} found but no MTT.exe asset attached"; return; }
                        e.Result = new object[] { latest, downloadUrl };
                    }
                }
                catch (Exception ex) { e.Result = $"check failed: {ex.Message}"; }
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                if (e.Result is string msg)
                {
                    owner.logToBox($"[update] {msg}");
                    return;
                }
                if (e.Result == null) return;
                var res = (object[])e.Result;
                var latest = (Version)res[0];
                string url = (string)res[1];
                owner.logToBox($"[update] version {latest.ToString(3)} available");
                var ans = MessageBox.Show(
                    $"Version {latest.ToString(3)} verfügbar.\nJetzt aktualisieren?",
                    "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (ans == DialogResult.Yes)
                    ApplyUpdate(url, owner);
            };
            worker.RunWorkerAsync();
        }

        private static void ApplyUpdate(string downloadUrl, MTT owner)
        {
            string exePath = Process.GetCurrentProcess().MainModule.FileName;
            string dir = Path.GetDirectoryName(exePath);
            string updatePath = Path.Combine(dir, "MTT_update.exe");
            string script = Path.Combine(Path.GetTempPath(), "mtt_update.bat");
            try
            {
                owner.logToBox($"[update] downloading to {updatePath}");
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
                owner.logToBox($"[update] apply failed: {ex.Message}");
                MessageBox.Show($"Update fehlgeschlagen: {ex.Message}", "Fehler",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
