using System.Collections.Generic;
using System.Linq;
using CodeElements.UpdateSystem.Files;
using CodeElements.UpdateSystem.Files.Operations;
using CodeElements.UpdateSystem.UpdateTasks.Base;
using Newtonsoft.Json;
#if ELEMENTSCORE
using CodeElements.Core;

#endif

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     Instructions for the updater that to patch the current installation
    /// </summary>
    public class UpdateInstructions
    {
        internal UpdateInstructions(List<SignedFileInformation> files)
        {
            TargetFiles = files.Cast<FileInformation>().ToList();
            FileSignatures = files.GroupBy(x => x.Hash).ToDictionary(x => x.Key, x => x.First().Signature);
        }

        public UpdateInstructions()
        {
        }

        /// <summary>
        ///     The file operations that must be done. This property is only set when an update package with the exact version was
        ///     found
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<IFileOperation> FileOperations
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

        /// <summary>
        ///     The target files. This property is only set when an update package with the exact version was not found. The
        ///     updater must decide by himself what has to be done
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<FileInformation> TargetFiles
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

        /// <summary>
        ///     The update tasks. The update tasks are in chronological order, beginning with the lowest
        ///     <see cref="UpdateTask.ExecutionOrderNumber" /> going to the highest. This property may be null
        /// </summary>
        [JsonProperty]
        public List<UpdateTask> Tasks
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }

        /// <summary>
        ///     The file signatures of the files that may be downloaded for the update process. If <see cref="TargetFiles" /> is
        ///     set, the signatures of all files will be in the dictionary; if <see cref="FileOperations" /> is set, only the
        ///     signatures of the files that implement <see cref="INeedDownload" /> will be available.
        /// </summary>
        [JsonProperty]
        public IDictionary<Hash, byte[]> FileSignatures
        {
            get;
#if !ELEMENTSCORE
            private
#endif
            set;
        }
    }
}