namespace CodeElements.UpdateSystem.Core.Internal
{
    internal static class ErrorTypes
    {
        public const string ValidationError = "InvalidArgumentException";
        public const string ProjectError = "ProjectException";
        public const string LicenseServiceError = "LicenseServiceException";
        public const string UpdateSystemError = "UpdateSystemException";
        public const string AuthenticationError = "AuthenticationException";
        public const string NotFoundError = "NotFoundException";
    }
}