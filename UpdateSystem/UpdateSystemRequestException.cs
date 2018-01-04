using System;
using CodeElements.UpdateSystem.Core.Internal;

namespace CodeElements.UpdateSystem
{
    public class UpdateSystemRequestException : Exception
    {
        internal UpdateSystemRequestException(RestError error) : base(error.Message)
        {
            ErrorCode = (ErrorCode) error.Code;
            ErrorType = error.Type;
        }

        public ErrorCode ErrorCode { get; }
        public string ErrorType { get; set; }
    }
}