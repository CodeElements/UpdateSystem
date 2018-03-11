namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     An argument submitted to your application by the patcher
    /// </summary>
    public class UpdateArgument
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="UpdateArgument" />
        /// </summary>
        /// <param name="argument">Command-line argument to pass</param>
        /// <param name="executionOption">Configure when the argument should be passed</param>
        public UpdateArgument(string argument, ExecutionOption executionOption)
        {
            Argument = argument;
            ExecutionOption = executionOption;
        }

        /// <summary>
        ///     Command-line argument to pass
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        ///     Configure when the argument should be passed
        /// </summary>
        public ExecutionOption ExecutionOption { get; set; }
    }

    /// <summary>
    ///     Different options for passing an argument
    /// </summary>
    public enum ExecutionOption
    {
        /// <summary>
        ///     Pass the argument when the update process succeeded
        /// </summary>
        Succeeded,

        /// <summary>
        ///     Pass the argument when the update process failed
        /// </summary>
        Failed,

        /// <summary>
        ///     Always pass the argument
        /// </summary>
        Always
    }
}