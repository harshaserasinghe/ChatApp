using System;

namespace Chat.Common.Exceptions
{
    public class AgentException : Exception
    {
        public int ErrroCode { get; set; }
        public string ErrorMessage { get; set; }
        public AgentException(int code, string message) : base(message)
        {
            ErrroCode = code;
            ErrorMessage = message;
        }
    }
}
