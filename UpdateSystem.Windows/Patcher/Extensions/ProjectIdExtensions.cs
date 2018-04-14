using System;
using System.IO;

namespace CodeElements.UpdateSystem.Windows.Patcher.Extensions
{
    internal static class ProjectIdExtensions
    {
        public static string GetTempDirectory(this Guid projectId)
        {
            return Path.Combine(Path.GetTempPath(), $"CodeElements.UpdateSystem.{projectId:D}");
        }
    }
}