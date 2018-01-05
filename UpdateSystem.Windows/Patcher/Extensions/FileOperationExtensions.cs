using System;
using System.Collections.Generic;
using CodeElements.UpdateSystem.Files;
using CodeElements.UpdateSystem.Files.Operations;

namespace CodeElements.UpdateSystem.Windows.Patcher.Extensions
{
    internal static class FileOperationExtensions
    {
        public static List<FileInformation> GetTargetFiles(this IFileOperation fileOperation)
        {
            if (fileOperation is INeedDownload needDownload)
                return new List<FileInformation> {needDownload.Target};
            if (fileOperation is DeleteFileOperation deleteFileOperation)
                return new List<FileInformation> {deleteFileOperation.Target};
            if (fileOperation is MoveFileOperation moveFileOperation)
                return moveFileOperation.Targets;

            throw new ArgumentException(nameof(fileOperation));
        }
    }
}