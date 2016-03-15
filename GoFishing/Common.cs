using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GoFishing
{
    public class Common
    {
        private static Dictionary<string, Keys> keysMapping;

        static Common()
        {
            keysMapping = new Dictionary<string, Keys>();
            var keysType = typeof (Keys);
            var keys = Enum.GetValues(typeof (Keys));
            foreach (var key in keys)
            {
                var k = (Keys)key;
                var name = Enum.GetName(keysType,k);
                if (!keysMapping.ContainsKey(name))
                {
                    keysMapping.Add(name, k);
                }
            }
        }


        public static KeyEventArgs FishStart = new KeyEventArgs(Keys.F11);
        public static KeyEventArgs FishStop = new KeyEventArgs(Keys.F11);
        public static KeyEventArgs WowStart = new KeyEventArgs(Keys.F12);
        public static KeyEventArgs WowStop = new KeyEventArgs(Keys.F12);
        public static KeyEventArgs None = new KeyEventArgs(Keys.None);

        public static Keys Parse(string str)
        {
            if (keysMapping.ContainsKey(str))
            {
                return keysMapping[str];
            }
            return Keys.None;
        }
    }
}
