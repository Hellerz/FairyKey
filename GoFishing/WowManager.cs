using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using DmLoader;

namespace GoFishing
{
    public class WowManager:HookProcessBase
    {
        private CDmSoft _dmSoft;
        private string _path;
        private string _userName;
        private string _passWord;
        private Process process = new Process();
        private HookProcessBase hook;
        public event Action WowStartuped;
        public WowManager(string path, string userName, string passWord, CDmSoft dmSoft)
            : base(Common.WowStart, Common.WowStop)
        {
            this._path = path;
            this._userName = userName;
            this._passWord = passWord;
            _dmSoft = dmSoft;
            process.StartInfo.FileName = path;
        }

        public WowManager(HookProcessBase hook, CDmSoft dmSoft)
            : base(Common.WowStart, Common.WowStop)
        {
            this.hook = hook;
            _dmSoft = dmSoft;
        }

        public Process FindWow()
        {
            var wows =System.Diagnostics.Process.GetProcessesByName("Wow");
            if(wows.Any())
            {
                return System.Diagnostics.Process.GetProcessesByName("Wow")[0];
            }
            return null;
        }

        public override void Stop()
        {
            base.Stop();
            this.hook.Stop();
        }

        private void KeyPressString(string str, int interval)
        {
            var words = str.Split(' ');
            foreach (var chr in words)
            {
                var key = Common.Parse(chr.ToUpper());
                if (key != Keys.None)
                {
                    _dmSoft.KeyPress((int)key);
                    _dmSoft.Delay(interval);
                }
            }
        }

        protected override void Process()
        {
            _dmSoft.Delay(5000);
            _dmSoft.KeyPress((int)Keys.Enter);
            _dmSoft.Delay(20000);
            _dmSoft.KeyPress((int)Keys.Escape);
            _dmSoft.Delay(1000);
            hook.Start();
        }
    }
}
