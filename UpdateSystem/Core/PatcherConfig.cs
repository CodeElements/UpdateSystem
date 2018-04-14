using System;
using System.Collections.Generic;
using CodeElements.UpdateSystem.Files.Operations;
using CodeElements.UpdateSystem.UpdateTasks.Base;

namespace CodeElements.UpdateSystem.Core
{
    public class PatcherConfig
    {
        public Guid ProjectId { get; set; }
        public List<IFileOperation> FileOperations { get; set; }
        public Dictionary<Hash, string> AvailableFiles { get; set; }
        public List<UpdateTask> UpdateTasks { get; set; }
    }
}