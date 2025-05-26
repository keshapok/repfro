using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bot
{
    class CharachterControl
    {
        public static void TryToAttackMob()
        {
            Click();
            RandomDelaySleep(100);
            PreventFromRunningFarAway();
        }

        static void Click()
        {
            Input.mouse_event(Input.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            Input.mouse_event(Input.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        static void PreventFromRunningFarAway() // By pressing "S" button, char go back
        {
            for (int i = 0; i<3; i++) { 
                Input.keybd_event(Convert.ToByte(Keys.S), 0, Input.KEYBOARDEVENTF_KEYDOWN, 0);
                Input.keybd_event(Convert.ToByte(Keys.S), 0, Input.KEYBOARDEVENTF_KEYUP, 0);
                SendKeys.SendWait("s");
            }
        }

        public static void AttackMobAndWait(int delay)
        {
            SendKeys.SendWait("1"); // Press "1". "1" mean attack mob
            RandomDelaySleep(delay);
        }

        public static void GetLoot()
        {
            for (int j = 0; j < 50; j++)
            {
                Input.keybd_event(Convert.ToByte(Keys.X), 0, Input.KEYBOARDEVENTF_KEYDOWN, 0);
                Input.keybd_event(Convert.ToByte(Keys.X), 0, Input.KEYBOARDEVENTF_KEYUP, 0);
                RandomDelaySleep(100);
            }
        }

        public static void PressKeyBoardButton(byte key)
        {
            Input.keybd_event(key, 0, Input.KEYBOARDEVENTF_KEYDOWN, 0);
            Input.keybd_event(key, 0, Input.KEYBOARDEVENTF_KEYUP, 0);
        }
        
        public static void PressKeyBoardButton(Keys key)
        {
            Input.keybd_event(Convert.ToByte(key), 0, Input.KEYBOARDEVENTF_KEYDOWN, 0);
            Input.keybd_event(Convert.ToByte(key), 0, Input.KEYBOARDEVENTF_KEYUP, 0);
        }

        static void RandomDelaySleep(float delayInMilliseconds) // min delay = 5 mSec
        {
            if (delayInMilliseconds < 5)
            {
                delayInMilliseconds = 5;
            }
            else
            {
                
                Thread.Sleep(Convert.ToInt32(delayInMilliseconds));
                return;
            }
            
            float dispersion = 20; // +-20%
            float percentsFromDelay = delayInMilliseconds / 100 * dispersion;

            var rand = new Random();
            int randomDelay = rand.Next(-Convert.ToInt32(delayInMilliseconds) / Convert.ToInt32(percentsFromDelay),
                Convert.ToInt32(delayInMilliseconds) / Convert.ToInt32(percentsFromDelay));

            Thread.Sleep(Convert.ToInt32(delayInMilliseconds) + Convert.ToInt32(randomDelay));
        }

        static void PressRandomKey() // Function was made to prevent clicker detection. Not in use.
        {
            string[] keys = { "-", "=", "d", "q", "e", "x" };
            var rand = new Random();
            int mistakePercent = 7;
            if (rand.Next(0, 100) <= mistakePercent)
            {
                RandomDelaySleep(300);
                SendKeys.Send(keys[rand.Next(0, keys.Length - 1)]);
            }
        }


    }
}
