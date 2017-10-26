using CodeElements.UpdateSystem.Windows.Patcher.Reversion;

namespace CodeElements.UpdateSystem.Windows.Patcher
{
    internal interface IRevertable
    {
        RevertableType Type { get; }
        void Revert();
    }
}