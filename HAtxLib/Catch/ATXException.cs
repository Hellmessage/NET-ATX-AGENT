using System;

namespace HAtxLib.Catch {
    public class ATXException : Exception {
        public ATXException(string message) : base(message) { }
    }
}
