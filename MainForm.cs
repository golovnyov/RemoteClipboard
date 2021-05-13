using System;
using System.Linq;
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
        private readonly IShareClipboardService shareClipboardService;
        private readonly IFetchClipboardService fetchClipboardService;
        private readonly IClipboardProvider clipboardProvider;
        private readonly System.Timers.Timer timer;

        public MainForm(
            ILogger<MainForm> logger,
            IClipboardProvider clipboardProvider,
            IShareClipboardService shareClipboardService,
            IFetchClipboardService fetchClipboardService)
        {
            this.logger = logger;
            this.clipboardProvider = clipboardProvider;
            this.shareClipboardService = shareClipboardService;
            this.fetchClipboardService = fetchClipboardService;
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

            clipboardProvider.ClipboardChanged += ClipboardProvider_ClipboardChanged;

            await fetchClipboardService.FetchClipboardDataAsync();
        }

        private void ClipboardProvider_ClipboardChanged(object sender, Events.ClipboardChangedEventArgs eventArgs)
        {
            label4.BeginInvoke(new Action(() => { label4.Text = DateTime.UtcNow.ToString(); }));
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var value = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(value) || oldClipboardValue == value)
            {
                return;
            }

            oldClipboardValue = value;

            if (value.Length > 25)
            {
                label2.Text = $"{value.Substring(0, Math.Min(value.Length, 25))}...";
            }
            else
            {
                label2.Text = value;
            }

            await shareClipboardService.ShareClipboardDataAsync(value);
        }

        private void RunWatcherTimer()
        {
            timer.Elapsed += new ElapsedEventHandler(HandleElapsedTimer);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        private void HandleElapsedTimer(object sender, ElapsedEventArgs e)
        {
            button1.BeginInvoke(new Action(() => { button1.PerformClick(); }));
        }
    }
}
