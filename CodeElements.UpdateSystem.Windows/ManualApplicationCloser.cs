using System;

namespace CodeElements.UpdateSystem.Windows
{
    /// <summary>
    ///     A manual application closer that exposes an event <see cref="CloseApplication" /> that will be invoked when the
    ///     application should be closed
    /// </summary>
    public class ManualApplicationCloser : IApplicationCloser
    {
        void IApplicationCloser.ExitApplication()
        {
            CloseApplication?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     This event will be fired in case that the application should close. Subscribe to this event and execute the
        ///     required operations.
        /// </summary>
        public event EventHandler CloseApplication;
    }
}