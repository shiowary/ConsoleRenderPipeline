/*

*/


using System;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RenderPipeline
{
    public class Input
    {

        private List<ConsoleKeyInfo> pressedKeys;
        private List<ConsoleKeyInfo> releasedKeys;

        //https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-basic-6/aa243025(v=vs.60)?redirectedfrom=MSDN
        public int[] ArrowInputNormalized() //1 up, 2 down, 3 left , 4 right
        {
            int[] dir = new int[] { 0, 0 };
            ConsoleKeyInfo key = Console.ReadKey(true);
            /*
             vbKeyLeft	37	LEFT ARROW key
             vbKeyUp	38	UP ARROW key
             vbKeyRight	39	RIGHT ARROW key
             vbKeyDown	40	DOWN ARROW key
            vbKeyReturn	13	ENTER key
             */
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow://left
                    dir[0] -= 1;
                    break;
                case ConsoleKey.UpArrow: //up
                    dir[1] -= 1;
                    break;
                case ConsoleKey.RightArrow://right
                    dir[0] += 1;
                    break;
                case ConsoleKey.DownArrow: //down
                    dir[1] += 1;
                    break;
                default:
                    return dir;
            }
            return dir;
        }
        public bool IsButtonPressed(ConsoleKeyInfo key)
        {
            return pressedKeys.Contains(key);
        }

        public bool IsButtonReleased(ConsoleKeyInfo key) {
            return releasedKeys.Contains(key);
        }


        public bool ismouseclicked(int btn)
        {
            return false;
        }
        public bool isMouseKeyPressed(int btn)
        {
            return false;
        }




    }
}
