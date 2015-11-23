namespace Gunde.Core.Output
{
    public interface IResultListener
    {
        void OutputLine(string message, params object[] formatParams);
    }
}