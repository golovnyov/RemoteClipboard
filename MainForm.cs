using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VH.RemoteClipboard.Mediator;
using VH.RemoteClipboard.Models;

namespace VH.RemoteClipboard
{
    public partial class MainForm : Form
    {
        IntPtr _ClipboardViewerNext;

        private Stack<string> clipboardValues;

        private readonly IMediator mediator;

        private DateTime? dateTimeWM_DRAWCLIPBOARDRaising;

        public MainForm(IMediator mediator)
        {
            this.mediator = mediator;

            clipboardValues = new Stack<string>(5);

            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            switch ((Win32.Msgs)m.Msg)
            {
                // The WM_DRAWCLIPBOARD message is sent to the first window
                // in the clipboard viewer chain when the content of the
                // clipboard changes. This enables a clipboard viewer
                // window to display the new content of the clipboard.
                case Win32.Msgs.WM_DRAWCLIPBOARD:
                    if (dateTimeWM_DRAWCLIPBOARDRaising.HasValue && DateTime.Now.Subtract(dateTimeWM_DRAWCLIPBOARDRaising.Value).TotalSeconds < 3)
                    {
                        return;
                    }

                    ProcessClipboardData();

                    //
                    // Each window that receives the WM_DRAWCLIPBOARD message
                    // must call the SendMessage function to pass the message
                    // on to the next window in the clipboard viewer chain.
                    //
                    Win32.User32.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);

                    dateTimeWM_DRAWCLIPBOARDRaising = DateTime.Now;
                    break;

                case Win32.Msgs.WM_CHANGECBCHAIN:
                    // When a clipboard viewer window receives the WM_CHANGECBCHAIN message,
                    // it should call the SendMessage function to pass the message to the
                    // next window in the chain, unless the next window is the window
                    // being removed. In this case, the clipboard viewer should save
                    // the handle specified by the lParam parameter as the next window in the chain.

                    // wParam is the Handle to the window being removed from
                    // the clipboard viewer chain
                    // lParam is the Handle to the next window in the chain
                    // following the window being removed.
                    if (m.WParam == _ClipboardViewerNext)
                    {
                        // If wParam is the next clipboard viewer then it
                        // is being removed so update pointer to the next
                        // window in the clipboard chain

                        _ClipboardViewerNext = m.LParam;
                    }
                    else
                    {
                        Win32.User32.SendMessage(_ClipboardViewerNext, m.Msg, m.WParam, m.LParam);
                    }
                    break;

                default:
                    //
                    // Let the form process the messages that we are
                    // not interested in
                    //
                    base.WndProc(ref m);
                    break;
            }
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
            mediator.RemoteClipboardChanged += ClipboardProvider_ClipboardChanged;

            RegisterClipboardViewer();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterClipboardViewer();
        }

        private void ClipboardProvider_ClipboardChanged(object sender, Events.ClipboardChangedEventArgs eventArgs)
        {
            if (eventArgs is null)
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            label4.BeginInvoke(new Action(() =>
            {
                label4.Text = DateTime.UtcNow.ToString();

                Clipboard.SetText(eventArgs.ClipboardValue.GetText());
            }));
        }

        private void RefreshUi(string value)
        {
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
                Text = text,
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

            Clipboard.SetText(eventSender.Text);
        }

        /// <summary>
        /// Show the clipboard contents in the window 
        /// and show the notification balloon if a link is found
        /// </summary>
        private void ProcessClipboardData()
        {
            string clipboardText = Clipboard.GetText();

            if (string.IsNullOrWhiteSpace(clipboardText) || (clipboardValues.TryPeek(out string stackValue) && string.Equals(stackValue, clipboardText, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            RefreshUi(clipboardText);

            var cpbValue = new ClipboardValue();

            cpbValue.SetText(clipboardText);

            mediator.NotifyLocalClipboardChanged(this, cpbValue);
        }

        /// <summary>
        /// Register this form as a Clipboard Viewer application
        /// </summary>
        private void RegisterClipboardViewer()
        {
            _ClipboardViewerNext = Win32.User32.SetClipboardViewer(this.Handle);
        }

        /// <summary>
        /// Remove this form from the Clipboard Viewer list
        /// </summary>
        private void UnregisterClipboardViewer()
        {
            Win32.User32.ChangeClipboardChain(this.Handle, _ClipboardViewerNext);
        }
    }
}
