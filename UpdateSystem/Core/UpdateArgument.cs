namespace CodeElements.UpdateSystem.Core
{
	public class UpdateArgument
	{
		public UpdateArgument(string argument, ExecutionOption executionOption)
		{
			Argument = argument;
			ExecutionOption = executionOption;
		}

		public string Argument { get; set; }
		public ExecutionOption ExecutionOption { get; set; }
	}

	public enum ExecutionOption
	{
		Succeeded,
		Failed,
		Always
	}
}