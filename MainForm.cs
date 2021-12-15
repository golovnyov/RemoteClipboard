using System;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using VH.RemoteClipboard.Services;

namespace VH.RemoteClipboard
{
    public partial class MainForm : Form
    {
        private string[] clipboardValues;

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

            clipboardValues = new string[5];

            InitializeComponent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;

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

                SetClipboardValue(eventArgs.Text);

                Clipboard.SetText(eventArgs.Text);
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

                if (string.IsNullOrWhiteSpace(value) || clipboardValues[0] == value)
                {
                    return;
                }

                SetClipboardValue(value);

                await localClipboardService.ShareClipboardDataAsync(value);
            }));
        }

        private void SetClipboardValue(string value)
        {
            clipboardValues = new[] { value }.Concat(clipboardValues.Take(4)).ToArray();

            lbl_cpb_main_value.Text = PrepareClipboardText(clipboardValues[0]);

            this.panel1.Controls.Clear();

            if (clipboardValues.Count(x => !string.IsNullOrWhiteSpace(x)) == 1)
            {
                return;
            }

            for (int i = 1; i < clipboardValues.Count(x => !string.IsNullOrWhiteSpace(x)); i++)
            {
                this.panel1.Controls.Add(new Label() { Text = PrepareClipboardText(clipboardValues[i]), Location = new Point() { X = 5, Y = 30 * (i - 1) }, AutoSize = true });

                var copyButton = new Button() { Text = "Copy", Location = new Point() { X = 395, Y = 30 * (i - 1) } };

                int cls = i;

                copyButton.Click += (s, ea) =>
                {
                    Clipboard.SetText(clipboardValues[cls]);
                };

                this.panel1.Controls.Add(copyButton);
            }
        }

        private string PrepareClipboardText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var trimmedValue = value.Trim();

            return trimmedValue.Substring(0, Math.Min(trimmedValue.Length, 25));
        }
    }
}
