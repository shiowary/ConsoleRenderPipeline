/*
 
 beep
 
*/
// your reference class must be included for now
using NAMESPACENAME;
//

using System;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RenderPipeline
{

    public class RenderClass
    {
        private Screen screen = new Screen();
        private bool running = true;
        private MethodInfo storedMethod;
        private object storedClass;
        public static void Main(string[] args)
        {
            RenderClass main = new RenderClass();
            Console.CursorVisible = false;
            main.running = true;
            main.DrawScreen(10); //screen refresh rate in ms = 3 = 60fps, 3 = 60fps
            Console.ReadKey();
        }

        private void GatherCutonClass(object classInstance, string methodname)
        {
            if (classInstance == null)
            {
                throw new ArgumentNullException(nameof(classInstance));
            }
            Type type = classInstance.GetType();
            MethodInfo method = type.GetMethod(methodname, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (method != null)
            {
                this.storedMethod = method;
            }
            else
            {
               Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Method '{methodname}' not found in class '{type.Name}'.");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void InitRenderReference(object myClass, string MyBufferBuilder)
        {

            if(myClass == null)
            {
                throw new ArgumentNullException(nameof(myClass) , "no class with name found");
            }
            if(MyBufferBuilder == "")
            {
                throw new ArgumentNullException(nameof(MyBufferBuilder) ,"please use a valid function name");
            }

            storedClass = myClass;
            GatherCutonClass(myClass, MyBufferBuilder);
        }



        private List<(int,int,char,ConsoleColor)> InvokeStoredMethod(object target, params object[] parameters)
        {
            if (this.storedMethod == null)
            {
                Console.WriteLine($"ERROR when calling function {this.storedMethod} inside of {storedClass} please check for init errors");
                throw new InvalidOperationException("No method stored, please reinitializse using GatherCutonClass");
                
            }
            return (List<(int,int,char,ConsoleColor)>)storedMethod.Invoke(target, parameters);
        }


        private async Task DrawScreen(int cycleDelay)
        {
            SOMENAME a = new SOMENAME();
            a.initReference(this);
            //
            int i = 0;
            while (running)
            {
                
                screen.buffer.AddRange(InvokeStoredMethod(storedClass,i));
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

        public List<(int,int,char,ConsoleColor)> AddtoBuffer(List<(int,int,char,ConsoleColor)> toadd)
        {
            screen.buffer.AddRange(toadd);
            return new List<(int, int, char, ConsoleColor)>();
        }

        public int GetScreenSize(int axis)
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            if (axis == 0)
            {
                return width;
            }
            if(height == 1)
            {
                return height;
            }
            return height * width;
        }
    }


    public class Screen
    {
        private List<Pixel> pixels = new List<Pixel>();
        private List<Pixel> nextPixels = new List<Pixel>();
        public List<(int,int,char,ConsoleColor)> buffer = new List<(int,int,char,ConsoleColor)> (); //list of coordinates which will become the pixels of the next step

        private HelperFunc h = new HelperFunc();
        private void renderScreen() {
            foreach (Pixel pixel in pixels)
            {
                (int,int,char,ConsoleColor) px = h.TranslateToConsoleScreen(pixel,(int)pixel.pos.x, (int)pixel.pos.y);
                h.ReplaceLineinColor(px.Item1,px.Item2, pixel.px.ToString(),ConsoleColor.Black, pixel.color);
            }
        }
        public void ScreenHandler(int refreshdelay, int iteration)
        {
            //clear current internal pixels
            pixels.Clear();
            pixels = nextPixels;
            //fill the next pixel itteration for next frame
            nextPixels = h.convertToPixels(buffer);
            //clear window fully
            Console.Clear();
            //redraw all pixels within the pixel buffer
            renderScreen();
            //clear the buffer before next frame
            buffer.Clear();
        }

        public List<(int,int,char,ConsoleColor)> DrawInBuffer(int iteration, float other = 0f, string args = "")
        {
            List<(int,int,char,ConsoleColor)> buffer = new List<(int,int,char,ConsoleColor)> ();
            //do some magic to create buffer logic within this

            //for now we do clock

            buffer.AddRange(h.daclock(iteration * 5,15,'A',ConsoleColor.Green));
            buffer.AddRange(h.daclock(-iteration * 2, 7, 'C', ConsoleColor.Blue));

            return buffer;
        }

        private void createLineInBuffer(int x0,int y0,int x1, int y1)
        {
            Vector3 origin = new Vector3(x0, y0);
            Vector3 line = new Vector3(x1, y1);
            pixels.AddRange(h.convertToPixels(h.DrawLine(origin,line)));
        }
        
    }
}
