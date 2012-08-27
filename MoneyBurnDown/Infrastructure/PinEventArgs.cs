using System;

namespace MoneyBurnDown.Infrastructure
{
    public class PinEventArgs : EventArgs
    {
        public bool IsPinned { get; set; }
    }
}