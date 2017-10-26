namespace CodeElements.UpdateSystem.UpdateTasks.Base
{
	/// <summary>
	///     The different task types
	/// </summary>
	public enum UpdateTaskType
	{
		/// <summary>
		///     Delete a directory
		/// </summary>
		DeleteDirectory,

		/// <summary>
		///     Delete files from a directory
		/// </summary>
		DeleteFiles,

		/// <summary>
		///     Start a new process
		/// </summary>
		StartProcess,

		/// <summary>
		///     Stop a process
		/// </summary>
		KillProcess,

		/// <summary>
		///     Start or restart a Windows service
		/// </summary>
		StartService,

		/// <summary>
		///     Stop a Windows service
		/// </summary>
		StopService,

		/// <summary>
		///     Execute batch code
		/// </summary>
		ExecuteBatchScript,

		/// <summary>
		///     Execute PowerShell code
		/// </summary>
		ExecutePowerShellScript
	}
}