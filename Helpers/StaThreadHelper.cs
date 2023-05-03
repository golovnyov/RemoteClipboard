using System;
using System.Threading;

namespace VH.RemoteClipboard.Helpers
{
    internal class StaThreadHelper
    {
        internal static void RunAsSTAThread(Action goForIt)
        {
            using AutoResetEvent @event = new(false);
            
            Thread thread = new(() => { goForIt(); @event.Set(); });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            @event.WaitOne();
        }
    }
}
