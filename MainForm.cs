using System;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using VH.RemoteClipboard.Services;

namespace VH.RemoteClipboard
{
    public partial class MainForm : Form
    {
        private string oldClipboardValue;

        private readonly ILogger logger;
        private readonly ILocalClipboardService localClipboardService;
        private readonly IRemoteClipboardService remoteClipboardService;
        private readonly System.Timers.Timer timer;

        public MainForm(ILogger<MainForm> logger, ILocalClipboardService shareClipboardService, IRemoteClipboardService fetchClipboardService)
        {
            this.logger = logger;
            this.localClipboardService = shareClipboardService;
            this.remoteClipboardService = fetchClipboardService;
            this.timer = new();

            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipTitle = "Minimize to Tray App";
            notifyIcon1.BalloonTipText = "You have successfully minimized your form.";

            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500);

                this.Hide();
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            RunWatcherTimer();

            remoteClipboardService.ClipboardChanged += ClipboardProvider_ClipboardChanged;

            await remoteClipboardService.FetchClipboardDataAsync();
        }

        private void ClipboardProvider_ClipboardChanged(object sender, Events.ClipboardChangedEventArgs eventArgs)
        {
            label4.BeginInvoke(new Action(() =>
            {
                label4.Text = DateTime.UtcNow.ToString();

                Clipboard.SetText(eventArgs.Text);

                textBox1.Text = eventArgs.Text.Trim();
            }));
        }

        private void RunWatcherTimer()
        {
            timer.Elapsed += new ElapsedEventHandler(HandleElapsedTimer);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        private void HandleElapsedTimer(object sender, ElapsedEventArgs e)
        {
            label1.BeginInvoke(new Action(async () =>
            {
                var value = Clipboard.GetText();

                if (string.IsNullOrWhiteSpace(value) || oldClipboardValue == value)
                {
                    return;
                }

                oldClipboardValue = value;

                textBox1.Text = value.Trim();

                await localClipboardService.ShareClipboardDataAsync(value);
            }));
        }
    }
}
