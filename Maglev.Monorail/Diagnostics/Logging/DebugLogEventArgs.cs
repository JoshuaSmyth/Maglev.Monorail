using System;

namespace Maglev.Monorail.Diagnostics.Logging
{
    public class DebugLogEventArgs : EventArgs
    {
        public string MyEventString { get; set; }

        public DebugLogEventArgs(string myString)
        {
            this.MyEventString = myString;
        }
    }
}
