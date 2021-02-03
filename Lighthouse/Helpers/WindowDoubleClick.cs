using System;
using System.Windows.Threading;

namespace Lighthouse.Helpers
{
    public class WindowDoubleClick
    {
        private bool clicked;

        public WindowDoubleClick()
        {
            var dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(700) };
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e) => clicked = false;

        public bool OnClickClick()
         {
             if (clicked) return true;

             clicked = true;
             return false;
         }
    }
}