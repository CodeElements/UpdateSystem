using System;
using CodeElements.UpdateSystem.Core.Internal;

namespace CodeElements.UpdateSystem
{
    /// <summary>
    ///     The exception that is thrown when the update system server returns an error response
    /// </summary>
    public class UpdateSystemRequestException : Exception
    {
        internal UpdateSystemRequestException(RestError error) : base(error.Message)
        {
            ErrorCode = (ErrorCode) error.Code;
            ErrorType = error.Type;
        }

        /// <summary>
        ///     The error code
        /// </summary>
        public ErrorCode ErrorCode { get; }

        /// <summary>
        ///     The type of the error
        /// </summary>
        public string ErrorType { get; set; }
    }
}