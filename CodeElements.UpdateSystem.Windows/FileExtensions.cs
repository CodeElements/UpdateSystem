using System;
using System.IO;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     Some useful functions to handle files
    /// </summary>
    internal static class FileExtensions
    {
        private const string NumberPattern = " ({0})";

        /// <summary>
        ///     Get a free temp file name in form of a Guid
        /// </summary>
        /// <returns>A path to a non existing file located in the temp directory</returns>
        public static string GetFreeTempFileName()
        {
            while (true)
            {
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("B"));
                if (!File.Exists(path))
                    return path;
            }
        }

        /// <summary>
        ///     Get a free temp file name in form of a Guid
        /// </summary>
        /// <param name="extension">The extension the file should have. Example: exe (without a dot!)</param>
        public static string GetFreeTempFileName(string extension)
        {
            while (true)
            {
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("B") + "." + extension);
                if (!File.Exists(path))
                    return path;
            }
        }

        /// <summary>
        ///     Return the full path of a unique file within the folder with the extension (the extension must not include a dot)
        /// </summary>
        /// <param name="folder">The folder of the file</param>
        /// <param name="extension">The extension of the file without a dot (e. g. exe, jpg). Null will result in no extension</param>
        /// <returns>Return a non existing file from the folder</returns>
        public static string GetUniqueFileName(string folder, string extension)
        {
            while (true)
            {
                var path = Path.Combine(folder, Guid.NewGuid().ToString("N"));
                if (extension != null)
                    path += "." + extension;

                if (!File.Exists(path))
                    return path;
            }
        }

        /// <summary>
        ///     Return the full path of a unique file within the folder with the extension (the extension must not include a dot)
        /// </summary>
        /// <param name="folder">The folder of the file</param>
        /// <returns>Return a non existing file from the folder</returns>
        public static string GetUniqueFileName(string folder)
        {
            return GetUniqueFileName(folder, null);
        }

        /// <summary>
        ///     Move a directory volume independent
        /// </summary>
        /// <param name="sourceDirectory">The source directory that should be moved</param>
        /// <param name="destinationDirectory">The destination directory</param>
        //https://stackoverflow.com/a/44898021/4166138
        public static void RobustMoveDirectory(string sourceDirectory, string destinationDirectory)
        {
            if (Path.GetPathRoot(sourceDirectory) == Path.GetPathRoot(destinationDirectory))
            {
                Directory.Move(sourceDirectory, destinationDirectory);
            }
            else
            {
                CopyDirectory(sourceDirectory, destinationDirectory);
                Directory.Delete(sourceDirectory, true);
            }
        }

        /// <summary>
        ///     Copy a directory
        /// </summary>
        /// <param name="sourceDirectory">The source directory that should be copied</param>
        /// <param name="destinationDirectory">The destination directory</param>
        //https://stackoverflow.com/a/3822913/4166138
        public static void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            //Now Create all of the directories
            foreach (var dirPath in Directory.EnumerateDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceDirectory, destinationDirectory));

            //Copy all the files & Replaces any files with the same name
            foreach (var newPath in Directory.EnumerateFiles(sourceDirectory, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourceDirectory, destinationDirectory), true);
        }

        /// <summary>
        ///     If the file <see cref="path" /> already exists, add a number at the end to make it non existing
        /// </summary>
        /// <param name="path">The path to a file</param>
        /// <returns>A file which does not exist</returns>
        public static string MakeUnique(string path)
        {
            // Short-cut if already available
            if (!File.Exists(path))
                return path;

            var escapedPath = path.Replace("{", "{{").Replace("}", "}}");
            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(escapedPath.Insert(escapedPath.LastIndexOf(Path.GetExtension(escapedPath)),
                    NumberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(escapedPath + NumberPattern);
        }

        /// <summary>
        ///     Check if the current process has write access to a specfic folder
        /// </summary>
        /// <param name="folderPath">The folder path</param>
        /// <returns>Return true if the current process has access to the path</returns>
        public static bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                var ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static string GetNextFilename(string pattern)
        {
            var tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                var pivot = (max + min) / 2;
                if (File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        /// <summary>
        ///     If the directory <see cref="path" /> already exists, add a number at the end to make it non existing
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakeDirectoryUnique(string path)
        {
            if (!Directory.Exists(path))
                return path;

            return GetNextDirectory(path + NumberPattern);
        }

        private static string GetNextDirectory(string pattern)
        {
            var tmp = string.Format(pattern, 1);
            if (tmp == pattern)
                throw new ArgumentException("The pattern must include an index place-holder", "pattern");

            if (!Directory.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (Directory.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                var pivot = (max + min) / 2;
                if (Directory.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        /// <summary>
        ///     Converts the given path into a format which is comparable. That makes it possible that
        ///     C:\Folder1\Folder2\..\test.txt equals C:\Folder1\test.txt
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>Returns a normalized form of the input <see cref="path" /></returns>
        public static string NormalizePath(this string path)
        {
            string fullPath;
            if (path.Contains(":"))
                fullPath = path;
            else
                try
                {
                    fullPath = Path.GetFullPath(new Uri(path).LocalPath);
                }
                catch (Exception)
                {
                    fullPath = path;
                }

            return fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
        }

        /// <summary>
        ///     Remove all invalid characters from the path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>Return a path without any illegal characters</returns>
        public static string RemoveInvalidCharacters(string path)
        {
            return string.Join(string.Empty, path.Split(Path.GetInvalidFileNameChars()));
        }
    }
}