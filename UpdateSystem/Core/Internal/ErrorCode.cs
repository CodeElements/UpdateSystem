namespace CodeElements.UpdateSystem.Core.Internal
{
    public enum ErrorCode
    {
        Project_ProjectDisabled = 100,
        Project_ProjectNotFound,
        Project_InvalidName,

        LicenseSystem_NotFound = 2000,
        LicenseSystem_Disabled,
        LicenseSystem_Expired,

        LicenseSystem_Licenses_NotFound = 3011,

        LicenseSystem_Activations_InvalidHardwareId = 6000,
        LicenseSystem_Activations_LicenseNotFound,
        LicenseSystem_Activations_LicenseDeactivated,
        LicenseSystem_Activations_LicenseExpired,
        LicenseSystem_Activations_AddressLimitReached,
        LicenseSystem_Activations_InvalidLicenseKeyFormat,
        LicenseSystem_Activations_ActivationLimitReached,

        UpdateSystem_NotFound = 20000,
        UpdateSystem_Disabled,
        UpdateSystem_Expired
    }
}