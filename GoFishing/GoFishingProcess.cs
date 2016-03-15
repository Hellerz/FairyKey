using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DmLoader;

namespace GoFishing
{
    public class GoFishingProcess:HookProcessBase
    {
        private readonly CDmSoft _dmSoft;

        public GoFishingProcess(CDmSoft dmSoft, KeyEventArgs start, KeyEventArgs stop)
            : base(start, stop)
        {
            _dmSoft = dmSoft;
            _dmSoft.SetShowErrorMsg(0);
        }


        protected override void Process()
        {
            var screenX = Screen.PrimaryScreen.WorkingArea.Width;
            var screenY = Screen.PrimaryScreen.WorkingArea.Height;
            var fishRodShowDelay = 2000;
            var throwFishRodKey = Keys.OemMinus;
            var addBaitKey = Keys.Oemplus;
            var waterKey = Keys.OemQuestion;
            var installRod = Keys.OemBackslash;
            var waterHits = 10;
            var itemKeyList = new List<Keys> { Keys.OemCloseBrackets, Keys.OemOpenBrackets, Keys.OemSemicolon,Keys.Oemcomma,Keys.OemQuotes,Keys.OemPeriod };
            
            var pickupDelay = 1000;

            var fishRodSimulity = 0.8;
            var fishRodColorInfoHList = new List<double> { 10, 20, 30, 40 };
            var fishRodColorInfoSList = new List<double> { 65, 75, 85, 95 };
            var fishRodColorInfoVList = new List<double> { 16, 22, 28, 34 };

            var leafColorInfoList = GetLeafColorInfoList(fishRodColorInfoHList, fishRodColorInfoSList, fishRodColorInfoVList);

            var baitBounds = new List<int> { screenX / 5, screenY / 5, screenX / 5 * 4, screenY / 4 * 3 };
            var isBaitOffset = new List<int> { 120, 30, 0, 60 };
            var timeOut = 20000;
            var isBaitColorInfo = new List<string> { "FFFBFF", "0.9", "5" };

            var fishRodlen = leafColorInfoList.Count;


            var lastTime = DateTime.Now;


            //进入魔兽世界
            //_dmSoft.Delay(5000);
            //_dmSoft.KeyPress((int)Keys.Enter);
            //_dmSoft.Delay(20000);
            //_dmSoft.KeyPress((int)Keys.Escape);
            //_dmSoft.Delay(1000);


            //WheelUp(40);
            AddRod(installRod);
            while (true)
            {
                if (lastTime.AddMinutes(5) <= DateTime.Now)
                {
                    UserWater(waterKey, waterHits);
                    DeleteItem(itemKeyList);
                    lastTime = DateTime.Now;
                }
                AddBait(addBaitKey);
                while (true)
                {
                    //抛竿
                    ThrowFishRod(throwFishRodKey);
                    //抛竿一段时间才能完全显示位置
                    _dmSoft.Delay(fishRodShowDelay);
                    var tmpBaitPosition = new Point();
                    var stack = new Stack<Point>();
                    for (int fishRodIndex = 0; fishRodIndex < fishRodlen; fishRodIndex++)
                    {
                        var tmpBaitColor = leafColorInfoList[fishRodIndex];
                        tmpBaitPosition = FindBaitPosition(tmpBaitColor, fishRodSimulity, baitBounds);
                        if (tmpBaitPosition.X >= 0 && tmpBaitPosition.Y >= 0)
                        {
                            //移动到鱼饵处
                            _dmSoft.MoveTo(tmpBaitPosition.X, tmpBaitPosition.Y);
                            stack.Push(tmpBaitPosition);
                            if (_dmSoft.GetCursorShape() == "61922884")
                            {
                                break;
                            }
                        }

                    }
                    if (stack.Count > 1)
                    {
                        stack.Pop();
                        tmpBaitPosition = stack.Pop();
                        _dmSoft.MoveTo(tmpBaitPosition.X, tmpBaitPosition.Y);
                    }
                    
                    if (IsBait(isBaitColorInfo, tmpBaitPosition, isBaitOffset, timeOut))
                    {
                        //拉竿
                        PullFishRod();
                        _dmSoft.Delay(pickupDelay);
                        //取鱼
                        PickupFish();
                    }
                }
            }
        }


        private void WheelUp(int times)
        {
            for (int i = 0; i < times; i++)
            {
                _dmSoft.WheelUp(); 
                _dmSoft.Delay(300);
            }
        }

        private void PickupFish()
        {
            _dmSoft.RightClick();
            _dmSoft.Delay(700);
            _dmSoft.RightClick();
            _dmSoft.Delay(700);
        }

        private void PullFishRod()
        {
            _dmSoft.RightClick();
        }


        private void AddBait(Keys addBaitKey)
        {
            _dmSoft.KeyPress((int)addBaitKey);
            _dmSoft.Delay(100);
        }

        private void AddRod(Keys addBaitKey)
        {
            _dmSoft.KeyPress((int)addBaitKey);
            _dmSoft.Delay(1000);
        }

        private void UserWater(Keys key, int hitCount)
        {
            for (int i = 0; i < hitCount; i++)
            {
                _dmSoft.KeyPress((int)key);
                _dmSoft.Delay(100);
            }
        }

        private void DeleteItem(List<Keys> keys)
        {
            keys.ForEach(key =>
            {
                _dmSoft.KeyPress((int)key);
                _dmSoft.Delay(100);
            });
        }
        private void ThrowFishRod(Keys throwFishRodKey)
        {
            _dmSoft.KeyPress((int)throwFishRodKey);
        }

        private Point FindBaitPosition(string color, double fishRodSimulity, List<int> bounds)
        {
            var offsetColor = "1|0|" + color;
            for (int i = 2; i <= 6; i++)
            {
                offsetColor += offsetColor + "," + i + "|0|" + color;
            }
            var colorPos = _dmSoft.FindMultiColorEx(bounds[0], bounds[1], bounds[2], bounds[3], color, offsetColor, fishRodSimulity, 0);
            object x, y;
            _dmSoft.GetResultPos(colorPos, 0, out x, out y);
            return new Point(int.Parse(x.ToString()), int.Parse(y.ToString()));
        }

        private bool IsBait(List<string> isBaitColorInfo, Point baitPoint, List<int> baitOffset, int timeOut)
        {
            var startTime = DateTime.Now.AddMilliseconds(timeOut);
            int l, t, r, b;
            l = baitPoint.X - baitOffset[0];
            t = baitPoint.Y - baitOffset[1];
            r = baitPoint.X + baitOffset[2];
            b = baitPoint.Y + baitOffset[3];
            while (DateTime.Now  < startTime)
            {
                //Debug.WriteLine(string.Format("l:{0},t:{1},r:{2},b:{3}",l,t,r,b));
                var count = _dmSoft.GetColorNum(l, t, r, b, isBaitColorInfo[0], double.Parse(isBaitColorInfo[1]));
                if (count > 0)
                {
                    Debug.WriteLine(string.Format("{0} {1} {2}", count, isBaitColorInfo[0], isBaitColorInfo[1]));
                }
                if (count > int.Parse(isBaitColorInfo[2]))
                {
                    return true;
                }
                _dmSoft.Delay(10);
            }
            return false;
        }

        private List<string> GetLeafColorInfoList(List<double> hList, List<double> sList, List<double> vList)
        {
            var leafColorInfoList = new List<string>();
            foreach (var h in hList)
            {
                foreach (var s in sList)
                {
                    foreach (var v in vList)
                    {
                        var color = HsvToRgb(h, s/100, v/100);
                        
                        leafColorInfoList.Add(color.Name);
                    }
                }
            }
            return leafColorInfoList;
        }

        private Color HsvToRgb(double h, double S, double V)
        {
            while (h < 0) { h += 360; };
            while (h >= 360) { h -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = h / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color
                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    // Green is the dominant color
                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;
                    // Blue is the dominant color
                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;
                    // Red is the dominant color
                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    // The color is not defined, we should throw an error.
                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            return Color.FromArgb(1, Clamp((int)(R * 255.0)), Clamp((int)(G * 255.0)), Clamp((int)(B * 255.0)));
        }

        private int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }
}
