using System;

namespace Chat.Common.Exceptions
{
    public class ServiceBusException : Exception
    {
        public int ErrroCode { get; set; }
        public string ErrorMessage { get; set; }
        public ServiceBusException(int code, string message) : base(message)
        {
            ErrroCode = code;
            ErrorMessage = message;
        }
    }
}
