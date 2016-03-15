using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DmLoader;

namespace GoFishing
{
    public abstract class HookProcessBase
    {
        private bool _isStart;
        protected Thread HookThread = null;

        protected HookProcessBase(KeyEventArgs start, KeyEventArgs stop)
        {
            var hook = new UserActivityHook(false, true);
            hook.KeyDown += (o, args) =>
            {
                if (IsInputKeys(args, start))
                {
                    if (_isStart)
                    {
                        Stop();
                        _isStart = false;
                    }
                }
                if (IsInputKeys(args, stop))
                {
                    if (!_isStart)
                    {
                        Start();
                        _isStart = true;
                    }
                }
            };
        }

        private bool IsInputKeys(KeyEventArgs key1, KeyEventArgs key2)
        {
            return key1.Alt == key2.Alt &&
                   key1.Control == key2.Control &&
                   key1.KeyCode == key2.KeyCode;
        }

        public virtual void Start()
        {
            HookThread = new Thread(Process);
            HookThread.IsBackground = true;
            HookThread.Start();
        }

        public virtual void Stop()
        {
            if (HookThread != null)
            {
                HookThread.Abort();
                HookThread = null;
            }
        }

        protected abstract void Process();
    }
}
