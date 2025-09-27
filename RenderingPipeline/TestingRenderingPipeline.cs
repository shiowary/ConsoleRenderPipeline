/*
 
 beep
 
*/


using Microsoft.VisualBasic;
using RenderPipeline;
using System;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Example
{
    public class newEscapeRoom
    {
    
        public List<(int, int, char, ConsoleColor)> buffer = new List<(int, int, char, ConsoleColor)>(); //list of coordinates which will become the pixels of the next step
        public int width;
        public int height;
        public int totalsize;
        private HelperFunc h = new HelperFunc();
        private HelperFunc.ScreenMath m = new HelperFunc.ScreenMath();


        public void initReference(RenderClass rc)
        {
            rc.InitRenderReference(this, "ProgrammScreen");
            width = rc.GetScreenSize(0);
            height = rc.GetScreenSize(1);
            totalsize = rc.GetScreenSize(2);
            totalsize = rc.GetScreenSize(2);
        }




        public List<(int, int, char, ConsoleColor)> ProgrammScreen()
        {
            ClockScreen();
            //
            //this function will be called to draw screen
            //

            return buffer;
        }




        public void addbuffertoscreen()
        {
            
        }
        public void ClockScreen()
        {
            buffer.Add((0, 0, '#', ConsoleColor.Magenta));
            buffer.Add((0, width, '#', ConsoleColor.Magenta));
            buffer.Add((height, 0, '#', ConsoleColor.Magenta));
            buffer.Add((-height, 0, '#', ConsoleColor.Magenta));
            buffer.Add((0, -width, '#', ConsoleColor.Magenta));

            buffer.AddRange(h.daclock(DateTime.Now.Minute, 15, 'M', ConsoleColor.Green));
            buffer.AddRange(h.daclock(DateTime.Now.Hour , 12, 'H', ConsoleColor.Blue));
        }


    }


}
