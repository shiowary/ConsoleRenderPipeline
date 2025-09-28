/*
 
Example code as to how the render pipeline could be used
 
*/


using Microsoft.VisualBasic;
using RenderPipeline;
using System;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Schema;
using Vector3 = RenderPipeline.Vector3;

namespace NAMESPACENAME
{
    public class SOMENAME
    {
    
        public List<(int, int, char, ConsoleColor)> buffer = new List<(int, int, char, ConsoleColor)>(); //list of coordinates which will become the pixels of the next step
        public int width;
        public int height;
        public int totalsize;
        private HelperFunc h = new HelperFunc();
        private HelperFunc.ScreenMath m = new HelperFunc.ScreenMath();
        private Vector3 v = new Vector3();


        //
        public void initReference(RenderClass rc)
        {
            rc.InitRenderReference(this, "ProgrammScreen");
            width = rc.GetScreenSize(0);
            height = rc.GetScreenSize(1);
            totalsize = rc.GetScreenSize(2);
            totalsize = rc.GetScreenSize(2);
        }
        //



        public List<(int, int, char, ConsoleColor)> ProgrammScreen(int clock)
        {
            //ClockScreen();
            DrawRotatingCube(clock);
            //DrawRotatingTriangle(clock);
            drawRotatingPyramid(clock);
            //DrawRoundthings(8); WIP


            //must return buffer in order for the rendering pipeline to know what does what
            return buffer;
        }



        private void ClockScreen()
        {
            buffer.Add((0, 0, '#', ConsoleColor.Magenta)); //middle
            buffer.Add((0, width, '#', ConsoleColor.Magenta)); // right centre
            buffer.Add((height, 0, '#', ConsoleColor.Magenta)); //top centre
            buffer.Add((-height, 0, '#', ConsoleColor.Magenta)); //down centre
            buffer.Add((0, -width, '#', ConsoleColor.Magenta)); //left middle


            //Draws clock arms
            buffer.AddRange(h.daclock(DateTime.Now.Minute, 15, 'M', ConsoleColor.Green));
            buffer.AddRange(h.daclock(DateTime.Now.Hour , 12, 'H', ConsoleColor.Blue));
        }

        //showcase of some implemented functions

        private void DrawRotatingCube(int clock)
        {
            float angle = clock;
            buffer = m.DrawRotatingCube(angle,10f);
        }

        private void DrawRotatingTriangle(int clock)
        {
            float angle = clock;
            buffer = m.DrawRotatingTriangle(angle, 10f,false);
        }

        private void drawRotatingPyramid(int clock)
        {
            float angle = clock;
            buffer.AddRange(m.DrawRotatingPyramid(angle, 8f));
        }

        public void DrawRoundthings(int size)
        {
            //buffer.AddRange(m.DrawSphere(size,'O'));
            buffer.AddRange(m.DrawCircle(size, 'O',ConsoleColor.White ));
        }
    }


}
