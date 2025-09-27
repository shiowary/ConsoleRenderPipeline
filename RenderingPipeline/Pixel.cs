/*
 
How pixels are stored internally
while we use tupel for a large part of the code pixels are then used to render every screen, pixels act as intersection for vector math  
*/


using System;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RenderPipeline
{
    public class Pixel
    {
        public Pixel(Vector3 pos, char px = '+', ConsoleColor col = ConsoleColor.White)
        {
            this.pos = pos;
            this.px = px;
            this.color = col;
        }
        public Vector3 pos = new Vector3();
        public char px;
        public ConsoleColor color;

    public (int,int,char,ConsoleColor) bufferpixel(int x, int y,char px,ConsoleColor col)
        {
            return (x, y, px, col);
        }
    }
}
