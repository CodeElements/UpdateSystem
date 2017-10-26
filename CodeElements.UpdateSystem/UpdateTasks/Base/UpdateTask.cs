using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#if ZEROFORMATTABLE
using ZeroFormatter;
#endif

namespace CodeElements.UpdateSystem.UpdateTasks.Base
{
    /// <summary>
    ///     Defines the base class of every update task
    /// </summary>
#if ZEROFORMATTABLE
	[ZeroFormattable]
	[Union(typeof(DeleteDirectoryTask), typeof(DeleteFilesTask), typeof(ExecuteBatchScriptTask),
		typeof(ExecutePowerShellScriptTask), typeof(KillProcessTask), typeof(StartProcessTask), typeof(StartServiceTask),
		typeof(StopServiceTask))]
#endif
    [JsonConverter(typeof(UpdateTaskConverter))]
    public abstract class UpdateTask
    {
#if ZEROFORMATTABLE
		protected const int StartIndex = 5;
#endif

        /// <summary>
        ///     All existing update task types matches to the classes
        /// </summary>
        public static Dictionary<UpdateTaskType, Type> UpdateTaskTypes { get; } = new Dictionary<UpdateTaskType, Type>
        {
            {UpdateTaskType.DeleteDirectory, typeof(DeleteDirectoryTask)},
            {UpdateTaskType.DeleteFiles, typeof(DeleteFilesTask)},
            {UpdateTaskType.ExecuteBatchScript, typeof(ExecuteBatchScriptTask)},
            {UpdateTaskType.ExecutePowerShellScript, typeof(ExecutePowerShellScriptTask)},
            {UpdateTaskType.KillProcess, typeof(KillProcessTask)},
            {UpdateTaskType.StartProcess, typeof(StartProcessTask)},
            {UpdateTaskType.StartService, typeof(StartServiceTask)},
            {UpdateTaskType.StopService, typeof(StopServiceTask)}
        };

        /// <summary>
        ///     The type of the task
        /// </summary>
#if ZEROFORMATTABLE
		[UnionKey]
#endif
        public abstract UpdateTaskType TaskType { get; }

        /// <summary>
        ///     Define is the task should be executed if the update package is skipped
        /// </summary>
#if ZEROFORMATTABLE
		[Index(0)]
#endif
        [JsonProperty(PropertyName = "onVersionJumpRequired")]
        public virtual bool OnVersionJumpRequired { get; set; }

        /// <summary>
        ///     The order number which defines when the task should be executed. The tasks will be executed ordered by this
        ///     property (ascending). All tasks with order numbers below 0 will be executed before the files are copied and all
        ///     tasks with order numbers greater than 0 will be executed after the files were copied. The value 0 is invalid for
        ///     this property
        /// </summary>
#if ZEROFORMATTABLE
		[Index(1)]
#endif
        [JsonProperty(PropertyName = "executionOrderNumber")]
        public virtual int ExecutionOrderNumber { get; set; }

        /// <summary>
        ///     Defines if the update process should fail if the task does not execute correctly (e. g. an exception occurres). If
        ///     this property is set to <value>false</value>, the update will continue.
        /// </summary>
#if ZEROFORMATTABLE
		[Index(2)]
#endif
        [JsonProperty(PropertyName = "importantForUpdateProcess")]
        public virtual bool IsImportantForUpdateProcess { get; set; }

        /// <summary>
        ///     The encoded platform data that defines where the task should be executed
        /// </summary>
#if ZEROFORMATTABLE
		[Index(3)]
#endif
        [JsonProperty(PropertyName = "platforms")]
        public virtual int Platforms { get; set; }

        /// <summary>
        ///     The signature used to validate that this task was created by the author
        /// </summary>
#if ZEROFORMATTABLE
		[Index(4)]
#endif
        [JsonProperty(PropertyName = "signature")]
        public virtual byte[] Signature { get; set; }
    }
}