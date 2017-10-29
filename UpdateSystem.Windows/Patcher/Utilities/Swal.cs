using System;

namespace CodeElements.UpdateSystem.Windows.Patcher.Utilities
{
    internal static class Swal
    {
        public static void low(Action action, Logger logger = null)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                logger?.Error("Exception swallowed: " + e);
            }
        }
    }
}