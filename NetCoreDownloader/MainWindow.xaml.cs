using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace NetCoreDownloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this._is32Bit = FxChecker.IsX86;
            this._isArm64 = FxChecker.IsArm64;
            this.InitializeComponent();
            if (this._isArm64)
            {
                this.x64Text.Text = "ARM64 (64-bit)";
                this.btnDownloadX64.Content = "Download Runtime - ARM64";
            }
            this.grpProgress.Visibility = Visibility.Hidden;
            if (this._is32Bit)
            {
                foreach (object obj in this.grdDownloadButtons.Children)
                {
                    UIElement uielement = (UIElement)obj;
                    if (uielement is TextBlock || uielement is Label)
                    {
                        uielement.Visibility = Visibility.Collapsed;
                    }
                }
            }
            this.PopulateDownloadSource();
            this.PopulateStatus();
        }

        // Token: 0x06000033 RID: 51 RVA: 0x00002C64 File Offset: 0x00000E64
        private string GetLINQPadPath(bool x86, bool cmd)
        {
            string str = cmd ? "LPRun7" : "LINQPad7";
            string text = (x86 || this._is32Bit) ? "x86" : (this._isArm64 ? "arm64" : "x64");
            string text2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str + "-" + text + ".exe");
            if (File.Exists(text2))
            {
                return text2;
            }
            text2 = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str + ".exe");
            if (File.Exists(text2) && MainWindow.GetArchitectureFromExe(text2) == text)
            {
                return text2;
            }
            return "executable";
        }

        // Token: 0x06000034 RID: 52 RVA: 0x00002D0C File Offset: 0x00000F0C
        public static string GetArchitectureFromExe(string path)
        {
            byte[] array = new byte[4096];
            using (FileStream fileStream = File.OpenRead(path))
            {
                if (fileStream.Length < 4096L)
                {
                    return null;
                }
                fileStream.Read(array, 0, 4096);
            }
            int num = BitConverter.ToInt32(array, 60);
            if (num < 0 || num + 4 > 4090)
            {
                return null;
            }
            ushort num2 = BitConverter.ToUInt16(array, num + 4);
            if (num2 == 34404)
            {
                return "x64";
            }
            if (num2 == 43620)
            {
                return "arm64";
            }
            if (num2 != 332)
            {
                return null;
            }
            return "x86";
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00002DBC File Offset: 0x00000FBC
        private void PopulateStatus()
        {
            //MainWindow.< PopulateStatus > d__9 < PopulateStatus > d__;

            //< PopulateStatus > d__.<> t__builder = AsyncVoidMethodBuilder.Create();

            //< PopulateStatus > d__.<> 4__this = this;

            //< PopulateStatus > d__.<> 1__state = -1;

            //< PopulateStatus > d__.<> t__builder.Start < MainWindow.< PopulateStatus > d__9 > (ref < PopulateStatus > d__);
        }

        // Token: 0x06000036 RID: 54 RVA: 0x00002DF4 File Offset: 0x00000FF4
        private void PopulateStatusBox(TextBox box, SemanticVersion version)
        {
            if (version == null)
            {
                box.Text = "";
                return;
            }
            try
            {
                if (version.CompareTo(FxChecker.MinVersion) < 0)
                {
                    box.Text = "Update required";
                }
                else if (version.CompareTo(FxChecker.MinRecommendedVersion) < 0)
                {
                    box.Text = "Newer version available";
                }
                else
                {
                    box.Text = "You're up to date!";
                }
            }
            catch
            {
            }
        }

        // Token: 0x06000037 RID: 55 RVA: 0x00002E68 File Offset: 0x00001068
        private void PopulateDownloadSource()
        {
            //MainWindow.< PopulateDownloadSource > d__11 < PopulateDownloadSource > d__;

            //< PopulateDownloadSource > d__.<> t__builder = AsyncVoidMethodBuilder.Create();

            //< PopulateDownloadSource > d__.<> 4__this = this;

            //< PopulateDownloadSource > d__.<> 1__state = -1;

            //< PopulateDownloadSource > d__.<> t__builder.Start < MainWindow.< PopulateDownloadSource > d__11 > (ref < PopulateDownloadSource > d__);
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00002E9F File Offset: 0x0000109F
        private void DownloadX64(object sender, RoutedEventArgs e)
        {
            this.Download(this._x64Uri);
        }

        // Token: 0x06000039 RID: 57 RVA: 0x00002EAD File Offset: 0x000010AD
        private void DownloadX86(object sender, RoutedEventArgs e)
        {
            this.Download(this._x86Uri);
        }

        // Token: 0x0600003A RID: 58 RVA: 0x00002EBB File Offset: 0x000010BB
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this._cancelSource.Cancel();
        }

        // Token: 0x0600003B RID: 59 RVA: 0x00002EC8 File Offset: 0x000010C8
        private void LlRefresh_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.< LlRefresh_Click > d__15 < LlRefresh_Click > d__;

            //< LlRefresh_Click > d__.<> t__builder = AsyncVoidMethodBuilder.Create();

            //< LlRefresh_Click > d__.<> 4__this = this;

            //< LlRefresh_Click > d__.<> 1__state = -1;

            //< LlRefresh_Click > d__.<> t__builder.Start < MainWindow.< LlRefresh_Click > d__15 > (ref < LlRefresh_Click > d__);
        }

        // Token: 0x0600003C RID: 60 RVA: 0x00002EFF File Offset: 0x000010FF
        private void BtnLaunchLINQPadX64_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchLINQPad(false);
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00002F08 File Offset: 0x00001108
        private void BtnLaunchLINQPadX86_Click(object sender, RoutedEventArgs e)
        {
            this.LaunchLINQPad(true);
        }

        // Token: 0x0600003E RID: 62 RVA: 0x00002F11 File Offset: 0x00001111
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("The X86 runtime lets you run LINQPad7-x86.exe, which launches LINQPad as a 32-bit process.\r\n\r\nThis is required when you need to reference 32-bit native DLLs.");
        }

        // Token: 0x0600003F RID: 63 RVA: 0x00002F20 File Offset: 0x00001120
        private void LaunchLINQPad(bool x86)
        {
            string linqpadPath = this.GetLINQPadPath(x86, false);
            if (!File.Exists(linqpadPath))
            {
                MessageBox.Show("Cannot locate " + linqpadPath + ".");
                return;
            }
            string linqpadPath2 = this.GetLINQPadPath(x86, true);
            if (!File.Exists(linqpadPath2))
            {
                MessageBox.Show("Cannot locate " + linqpadPath2 + ".");
                return;
            }
            using (Process process = Process.Start(new ProcessStartInfo(linqpadPath2)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }))
            {
                StringBuilder errors = new StringBuilder();
                process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs errorArgs)
                {
                    errors.AppendLine(errorArgs.Data);
                };
                process.BeginErrorReadLine();
                int num = 0;
                while (process.StandardOutput.ReadLine() != null)
                {
                    num++;
                }
                int exitCode = process.ExitCode;
                if (exitCode != 0 && exitCode != 1)
                {
                    string text = "Unable to start LINQPad.";
                    if (errors.Length > 0)
                    {
                        text = text + "\r\n\r\n" + errors.ToString();
                    }
                    MessageBox.Show(text, "LINQPad", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;
                }
            }
            Process.Start(new ProcessStartInfo(linqpadPath)
            {
                UseShellExecute = true
            }).Dispose();
        }

        private void Download(string uri)
        {
            //MainWindow.< Download > d__20 < Download > d__;

            //< Download > d__.<> t__builder = AsyncVoidMethodBuilder.Create();

            //< Download > d__.<> 4__this = this;

            //< Download > d__.uri = uri;

            //< Download > d__.<> 1__state = -1;

            //< Download > d__.<> t__builder.Start < MainWindow.< Download > d__20 > (ref < Download > d__);
        }


        private bool _is32Bit;

        private bool _isArm64;

        private string _x64Uri;

        private string _x86Uri;

        private CancellationTokenSource _cancelSource;

        // Token: 0x04000023 RID: 35
        internal TextBox txtDownloadSource;

        // Token: 0x04000025 RID: 37
        internal Button btnDownloadX64;

        // Token: 0x04000026 RID: 38
        internal Button btnDownloadX86;

        // Token: 0x04000027 RID: 39
        //internal GroupBox grpProgress = new GroupBox();

        // Token: 0x04000028 RID: 40
        internal TextBlock lblAction;

        // Token: 0x04000029 RID: 41
        internal Button btnCancel;

        // Token: 0x0400002A RID: 42
        internal ProgressBar progressBar;

        // Token: 0x0400002B RID: 43
        internal TextBlock txtProgress;

        // Token: 0x0400002C RID: 44
        internal GroupBox grpLaunch;

        // Token: 0x0400002D RID: 45
        internal Button btnLaunchLINQPadX64;

        // Token: 0x0400002E RID: 46
        internal Button btnLaunchLINQPadX86;
    }
}
