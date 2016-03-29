using System.Threading;

namespace LibraTalk.Windows.Client.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class Promise : IDeferral
    {
        private readonly ManualResetEventSlim evt;

        public Promise(ManualResetEventSlim evt)
        {
            this.evt = evt;
        }

        public void Complete()
        {
            evt.Set();
        }
    }
}