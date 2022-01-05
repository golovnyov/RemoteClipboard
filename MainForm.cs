using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using VH.RemoteClipboard.Extensions;
using VH.RemoteClipboard.Mediator;

namespace VH.RemoteClipboard
{
    public partial class MainForm : Form
    {
        private Stack<string> clipboardValues;

        private readonly ILogger logger;
        private readonly IMediator mediator;
        private readonly System.Timers.Timer timer;

        public MainForm(ILogger<MainForm> logger, IMediator mediator)
        {
            this.logger = logger;
            this.timer = new();
            this.mediator = mediator;

            clipboardValues = new Stack<string>(5);

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

        private void MainForm_Load(object sender, EventArgs e)
        {
            RunWatcherTimer();

            mediator.ClipboardChanged += ClipboardProvider_ClipboardChanged;
        }

        private void ClipboardProvider_ClipboardChanged(object sender, Events.ClipboardChangedEventArgs eventArgs)
        {
            label4.BeginInvoke(new Action(() =>
            {
                label4.Text = DateTime.UtcNow.ToString();

                SetClipboardValue(eventArgs.ClipboardValue.GetText());
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
            label1.BeginInvoke(new Action(() =>
            {
                var clipboardText = Clipboard.GetText();

                if (string.IsNullOrWhiteSpace(clipboardText) || (clipboardValues.TryPeek(out string stackValue) && stackValue == clipboardText))
                {
                    return;
                }

                mediator.NotifyWithText(this, clipboardText);
            }));
        }

        private void SetClipboardValue(string value)
        {
            Clipboard.SetText(value);

            clipboardValues.Push(value);

            this.panel1.Controls.Clear();

            int y_position = 0;

            foreach (var clipboardValue in clipboardValues)
            {
                var labelDynamic = CreateLabel(clipboardValue, y_position);

                this.panel1.Controls.Add(labelDynamic);

                y_position++;
            }
        }


        private Label CreateLabel(string text, int y_position)
        {
            var labelDynamic = new Label()
            {
                Text = PrepareClipboardText(text),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Location = new Point() { X = 5, Y = 50 * y_position },
                Width = 460,
                Height = 40
            };

            this.panel1.Controls.Add(labelDynamic);

            labelDynamic.Click += LabelDynamic_Click;
            labelDynamic.MouseHover += LabelDynamic_MouseHover;
            labelDynamic.MouseLeave += LabelDynamic_MouseLeave;

            return labelDynamic;
        }   

        private void LabelDynamic_MouseLeave(object sender, EventArgs e)
        {
            var eventSender = sender as Label;

            eventSender.BackColor = Color.FromKnownColor(KnownColor.White);
        }

        private void LabelDynamic_MouseHover(object sender, EventArgs e)
        {
            var eventSender = sender as Label;

            eventSender.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        private void LabelDynamic_Click(object sender, EventArgs e)
        {
            var eventSender = sender as Label;

            mediator.NotifyWithText(this, eventSender.Text);
        }

        private static string PrepareClipboardText(string value)
        {
            var trimmedValue = value?.Trim();

            return trimmedValue?.Substring(0, Math.Min(trimmedValue.Length, 25));
        }
    }
}
