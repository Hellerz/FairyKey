using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DmLoader;
using Timer = System.Timers.Timer;

namespace GoFishing
{
    public partial class DashBoard : Form
    {
        private CDmSoft _dmSoft = new CDmSoft();
        private static DateTime executeTime;
        private Timer timer;
        public DashBoard()
        {
            InitializeComponent();
            var gofish = new GoFishingProcess(_dmSoft, Common.FishStart, Common.FishStop);
            textBox1.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            timer = new Timer(10000);//实例化Timer类，设置间隔时间为10000毫秒；   
            timer.Elapsed += (sender, args) =>
            {
                if (!executeTime.Equals(DateTime.MinValue)&& executeTime - args.SignalTime < new TimeSpan(0, 0, 0, 10))
                {
                    executeTime = DateTime.MinValue;
                   
                    Execute();
                }
            };//到达时间的时候执行事件；   
            timer.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；   
            timer.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；  
            timer.Start();
            //var str = _dmSoft.GetAveRGB(0, 0, Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
           
           // var wowManager = new WowManager(@"C:\Game\wow\黑暗之门登录器.exe", "h e a d l a n d", "D1 D2 D7 D1 D9 D0 D6 D1 D2 D0", _dmSoft);
            
            //493c20 493d1f
        }

        private void Execute()
        {
            var gofish = new GoFishingProcess(_dmSoft, Common.None,Common.None);
            gofish.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime date;
            if (DateTime.TryParse(textBox1.Text,out date))
            {
                executeTime = date;
                MessageBox.Show(string.Format("设置成功！{0}", date.ToString("u")));
            }
            else
            {
                MessageBox.Show(string.Format("时间格式不正确！"));
            }
        }
    }
}
