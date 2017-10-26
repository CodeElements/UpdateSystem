using System;
using System.Security.Cryptography;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Defines a class that provides information to download an update
    /// </summary>
    public interface IDownloadable
    {
        /// <summary>
        ///     The instructions of the updater
        /// </summary>
        UpdateInstructions Instructions { get; }

        /// <summary>
        ///     The guid of the update project
        /// </summary>
        Guid ProjectGuid { get; }

        /// <summary>
        ///     The public key of the update project, required to validate the signatures of files and tasks
        /// </summary>
        RSAParameters PublicKey { get; }
    }
}