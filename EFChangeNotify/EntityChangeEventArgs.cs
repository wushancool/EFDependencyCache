using System;

namespace EFChangeNotify
{
    public class EntityChangeEventArgs<T> : EventArgs
    {
        public bool ContinueListening { get; set; }
    }
}
