/*
 
 beep
 
*/


using System;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RenderPipeline
{

    public class RenderClass
    {
        public Screen screen = new Screen();
        private bool running = true;
        public static void Main(string[] args)
        {
            //sRenderClass main = new RenderClass();
            //main.DrawScreen();
            RenderClass main = new RenderClass();
            main.DrawScreen(100);
            Console.ReadKey();


        }
        private async Task DrawScreen(int cycleDelay)
        {
            int i = 0;
            while (running)
            {
                screen.ScreenHandler(cycleDelay,i);
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Escape)
                        running = false;
                }

                await Task.Delay(cycleDelay);
                i++;
            }
        }
    }

    public class Pixel
    {
        public Pixel(Vector3 pos, char px = '+', ConsoleColor col = ConsoleColor.White) {
            this.pos = pos;
            this.px = px;
            this.color = col;
        }
        public Vector3 pos = new Vector3();
        public char px;
        public ConsoleColor color;
    }

    public class Screen
    {
        private List<Pixel> pixels = new List<Pixel>();
        private List<Pixel> nextPixels = new List<Pixel>();
        private List<(int,int)> buffer = new List<(int,int)> (); //list of coordinates which will become the pixels of the next step

        private HelperFunc h = new HelperFunc();
        public void renderScreen() {
            foreach (Pixel pixel in pixels)
            {
                (int,int) px = h.TranslateToConsoleScreen((int)pixel.pos.x, (int)pixel.pos.y);
                h.ReplaceLineinColor(px.Item1,px.Item2, pixel.px.ToString(),ConsoleColor.Black, pixel.color);
            }
        }

        public void createLineInBuffer(int x0,int y0,int x1, int y1)
        {
            Vector3 origin = new Vector3(x0, y0);
            Vector3 line = new Vector3(x1, y1);
            pixels.AddRange(h.convertToPixels(h.DrawLine(origin,line)));
        }


        public void ScreenHandler(int refreshdelay, int iteration)
        {
            //clear current internal pixels
            pixels.Clear();
            pixels = nextPixels;

            // we currently fill the buffer "manually"
            buffer.AddRange(DrawInBuffer(iteration));
            //

            //fill the next pixel itteration for next frame
            nextPixels = h.convertToPixels(buffer);
            //clear window fully
            Console.Clear();
            //redraw all pixels within the pixel buffer
            renderScreen();
            //clear the buffer
            buffer.Clear();
        }

        public List<(int,int)> DrawInBuffer(int iteration, float other = 0f, string args = "")
        {
            List<(int,int)> buffer = new List<(int,int)> ();

            //do some magic to create buffer logic within this

            //for now we do clock
            buffer.AddRange(h.daclock(iteration * 5));

            return buffer;
        }

        
    }


    public class HelperFunc
    {
        public void ReplaceLine(int left, int top, string txt) {
            Console.SetCursorPosition(left, top);
            Console.Write(txt);
        }
        public Pixel convertToPixel((int, int) pixel, char px = '+', ConsoleColor col = ConsoleColor.White)
        {
            Vector3 tempVector = new Vector3(pixel.Item1, pixel.Item2);
            Pixel temp = new Pixel(tempVector, px, col); //generate a default pixel for now
            return temp;
        }

        public List<Pixel> convertToPixels(List<(int,int)> px2, List<(int,int,int)> px3 = null)
        {
            List<Pixel> newpixels = new List<Pixel>();
            foreach ((int,int) pixelcoord in px2)
            {
                Pixel temp = convertToPixel(pixelcoord);
                newpixels.Add(temp);
            }
            return newpixels;
        }

        //Bresenham’s algorithm to draw a line from (0,0) to the vector3 point
        //this part is stolen from stack overflow
        public List<(int, int)> DrawLine(Vector3 origin, Vector3 line)
        {
            List<(int, int)> points = new List<(int, int)>();
            int x0 = (int)origin.x;
            int y0 = (int)origin.y;
            int x1 = (int)line.x;
            int y1 = (int)line.y;
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            while (true)
            {
                points.Add((x0, y0));
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
            return points;
        }

        public void ReplaceLineinColor(int left, int top,string txt, ConsoleColor backcol = ConsoleColor.Black, ConsoleColor forecol = ConsoleColor.White)
        {
            ConsoleColor baseforcol = ConsoleColor.White;
            ConsoleColor basebackcol = ConsoleColor.Black;
            Console.ForegroundColor = forecol;
            Console.BackgroundColor = backcol;
            ReplaceLine(left, top, txt);
            Console.ForegroundColor=baseforcol;
            Console.BackgroundColor=basebackcol;
        }

        public int calculatebuffer()
        {
            return 0;
        }

        public (int,int) FindAcualCentre() //its the center of the current console window, this is an absolute number
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;
            return (centerX, centerY);
        }

        //Translates the coordinate given into a coordinate that the console understands aka (index from top, index from left)
        public (int, int) TranslateToConsoleScreen(int x, int y, int z = 0)
        {
            var (centerX, centerY) = FindAcualCentre();
            // X grows right, console X grows right
            int screenX = centerX + x;
            // Y grows up but console Y grows down
            int screenY = centerY - y;
            // Clamp to console bounds
            screenX = Math.Clamp(screenX, 0, Console.WindowWidth - 1);
            screenY = Math.Clamp(screenY, 0, Console.WindowHeight - 1);
            return (screenX, screenY);
        }



        public List<(int,int)> daclock(int iteration)
        {
            float angle = -(iteration % 360) * (float)(Math.PI / 180.0);
            int length = 10;
            int endX = (int)(length * Math.Cos(angle));
            int endY = (int)(length * Math.Sin(angle));
            return DrawLine(new Vector3(0, 0), new Vector3(endX, endY));
        }



        public void writeLineSlow(string Line, int delay)
        {
            foreach (var item in Line)
            {
                Console.Write(item);
                Thread.Sleep(delay);
            }
            Console.Write("\n");
        }


        public class ScreenMath
        {
            private HelperFunc helper = new HelperFunc();
            private List<(int,int)> accumolator = new List<(int,int)> ();



            //modified code and ideas from https://stackoverflow.com/questions/11075505/get-all-points-within-a-triangle
            public List<(int,int)>GetTriangle(Vector3 a, Vector3 b, Vector3 c)
            {
                List<(int,int)> tempbuffer = new List<(int,int)> ();
                tempbuffer.AddRange(helper.DrawLine(a, b));
                tempbuffer.AddRange(helper.DrawLine(a, c));
                tempbuffer.AddRange(helper.DrawLine(b, c));
                return tempbuffer;
            }

            public List<(int,int)> FillTriangle(Vector3 a, Vector3 b, Vector3 c)
            {
                var v1 = (x: (int)a.x, y: (int)a.y);
                var v2 = (x: (int)b.x, y: (int)b.y);
                var v3 = (x: (int)c.x, y: (int)c.y);
                List<(int,int)>points = new List<(int,int)> ();
                //sort the tiangle points by size to magic

                var verts = new List<(int x, int y)> { v1, v2, v3 }
                .OrderBy(v => v.y).ToList();
                v1 = verts[0]; v2 = verts[1]; v3 = verts[2];
                //having them in order allows us to make assumptions

                for (int y = v1.y; y <= v3.y; y++)
                {
                    bool secondHalf = y > v2.y || v2.y == v1.y;
                    int segmentY = secondHalf ? v3.y - v2.y : v2.y - v1.y;
                    int segmentOffset = secondHalf ? y - v2.y : y - v1.y;

                    float alpha = (float)(y - v1.y) / (v3.y - v1.y);
                    float beta = (float)segmentOffset / segmentY;

                    int ax = v1.x + (int)((v3.x - v1.x) * alpha);
                    int bx = secondHalf
                        ? v2.x + (int)((v3.x - v2.x) * beta)
                        : v1.x + (int)((v2.x - v1.x) * beta);

                    if (ax > bx) (ax, bx) = (bx, ax);

                    for (int x = ax; x <= bx; x++)
                        points.Add((x, y));
                }

                return points;

            }

            private int InterpX((int x, int y) a, (int x, int y) b, int y)
            {
                if (a.y == b.y) return a.x;
                return a.x + (b.x - a.x) * (y - a.y) / (b.y - a.y);
            }

            //Modified to fit my needs with T:Vector3 
            public  Vector3 GetTriangleCenter<T>(T p0, T p1, T p2)
            where T : Vector3
            {
                return new Vector3(p0.x + p1.x + p2.x / 3, p0.y + p1.y + p2.y / 3);
            }
            public static float TriangleArea(float a, float b, float c)
            {
                double s = (a + b + c) / 2.0f;
                return (float)Math.Sqrt(s * (s - (double)a) * (s - (double)b) * (s - (double)c));
            }
            public static bool CheckIfValidTriangle<T>(T v1, T v2, T v3, out float a, out float b, out float c)
            where T : Vector3
            {
                a = Vector2.Distance(new Vector2(v1.x, v1.y), new Vector2(v2.x, v2.y));
                b = Vector2.Distance(new Vector2(v2.x, v2.y), new Vector2(v3.x, v3.y));
                c = Vector2.Distance(new Vector2(v3.x, v3.y), new Vector2(v1.x, v1.y));

                if (a + b <= c || a + c <= b || b + c <= a)
                    return false;
                return true;
            }
        }
    
    }
}
