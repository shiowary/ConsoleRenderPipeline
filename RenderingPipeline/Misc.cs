/*
 
 beep
 
*/


using System;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RenderPipeline
{
    public class HelperFunc
    {
        public void ReplaceLine(int left, int top, string txt)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(txt);
        }
        public Pixel convertToPixel((int, int, char, ConsoleColor) pixel, char px = '+', ConsoleColor col = ConsoleColor.White)
        {
            Vector3 tempVector = new Vector3(pixel.Item1, pixel.Item2);
            Pixel temp = new Pixel(tempVector, pixel.Item3, pixel.Item4); //generate a default pixel for now
            return temp;
        }

        public List<Pixel> convertToPixels(List<(int, int, char, ConsoleColor)> px2, List<(int, int, int)> px3 = null)
        {
            List<Pixel> newpixels = new List<Pixel>();
            foreach ((int, int, char, ConsoleColor) pixelcoord in px2)
            {
                Pixel temp = convertToPixel(pixelcoord);
                newpixels.Add(temp);
            }
            return newpixels;
        }

        //Bresenham’s algorithm to draw a line from (0,0) to the vector3 point
        //this part is stolen from stack overflow
        public List<(int, int, char, ConsoleColor)> DrawLine(Vector3 origin, Vector3 line, char px = '#', ConsoleColor col = ConsoleColor.White)
        {
            List<(int, int, char, ConsoleColor)> points = new List<(int, int, char, ConsoleColor)>();
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
                points.Add((x0, y0, px, col));
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

        public void ReplaceLineinColor(int left, int top, string txt, ConsoleColor backcol = ConsoleColor.Black, ConsoleColor forecol = ConsoleColor.White)
        {
            ConsoleColor baseforcol = ConsoleColor.White;
            ConsoleColor basebackcol = ConsoleColor.Black;
            Console.ForegroundColor = forecol;
            Console.BackgroundColor = backcol;
            ReplaceLine(left, top, txt);
            Console.ForegroundColor = baseforcol;
            Console.BackgroundColor = basebackcol;
        }

        public int calculatebuffer()
        {
            return 0;
        }

        public (int, int) FindAcualCentre() //its the center of the current console window, this is an absolute number
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int centerX = width / 2;
            int centerY = height / 2;
            return (centerX, centerY);
        }

        //Translates the coordinate given into a coordinate that the console understands aka (index from top, index from left)
        public (int, int, char, ConsoleColor) TranslateToConsoleScreen(Pixel px, int x, int y, int z = 0)
        {
            var (centerX, centerY) = FindAcualCentre();
            // X grows right, console X grows right
            int screenX = centerX + x;
            // Y grows up but console Y grows down
            int screenY = centerY - y;
            // Clamp to console bounds
            screenX = Math.Clamp(screenX, 0, Console.WindowWidth - 1);
            screenY = Math.Clamp(screenY, 0, Console.WindowHeight - 1);
            return (screenX, screenY, px.px, px.color);
        }



        public List<(int, int, char, ConsoleColor)> daclock(int iteration, int len, char px, ConsoleColor col)
        {
            float angle = -(iteration % 360) * (float)(Math.PI / 180.0);
            int length = len;
            int endX = (int)(length * Math.Cos(angle));
            int endY = (int)(length * Math.Sin(angle));
            return DrawLine(new Vector3(0, 0), new Vector3(endX, endY), px, col);
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
            private List<(int, int, char, ConsoleColor)> accumolator = new List<(int, int, char, ConsoleColor)>();
            private Vector3 v = new Vector3();


            //modified code and ideas from https://stackoverflow.com/questions/11075505/get-all-points-within-a-triangle
            public List<(int, int, char, ConsoleColor)> GetTriangle(Vector3 a, Vector3 b, Vector3 c)
            {
                List<(int, int, char, ConsoleColor)> tempbuffer = new List<(int, int, char, ConsoleColor)>();
                tempbuffer.AddRange(helper.DrawLine(a, b));
                tempbuffer.AddRange(helper.DrawLine(a, c));
                tempbuffer.AddRange(helper.DrawLine(b, c));
                return tempbuffer;
            }

            public List<(int, int, char, ConsoleColor)> FillTriangle(Vector3 a, Vector3 b, Vector3 c)
            {
                var v1 = (x: (int)a.x, y: (int)a.y);
                var v2 = (x: (int)b.x, y: (int)b.y);
                var v3 = (x: (int)c.x, y: (int)c.y);
                List<(int, int, char, ConsoleColor)> points = new List<(int, int, char, ConsoleColor)>();
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
                        points.Add((x, y, '#', ConsoleColor.White));
                }

                return points;

            }

            private int InterpX((int x, int y) a, (int x, int y) b, int y)
            {
                if (a.y == b.y) return a.x;
                return a.x + (b.x - a.x) * (y - a.y) / (b.y - a.y);
            }

            public Vector3 GetTriangleCenter(Vector3 p0, Vector3 p1, Vector3 p2)
            {
                return new Vector3(p0.x + p1.x + p2.x / 3, p0.y + p1.y + p2.y / 3);
            }
            public static float TriangleArea(float a, float b, float c)
            {
                double s = (a + b + c) / 2.0f;
                return (float)Math.Sqrt(s * (s - (double)a) * (s - (double)b) * (s - (double)c));
            }
            public static bool CheckIfValidTriangle(Vector3 v1, Vector3 v2, Vector3 v3, out float a, out float b, out float c)
            {
                a = Vector2.Distance(new Vector2(v1.x, v1.y), new Vector2(v2.x, v2.y));
                b = Vector2.Distance(new Vector2(v2.x, v2.y), new Vector2(v3.x, v3.y));
                c = Vector2.Distance(new Vector2(v3.x, v3.y), new Vector2(v1.x, v1.y));

                if (a + b <= c || a + c <= b || b + c <= a)
                    return false;
                return true;
            }



            //Geometric projection and 3D rendering for Cube and Triangle

            public Vector3 Project(Vector3 point, float fov, float viewerDistance)
            {
                float factor = fov / (viewerDistance + point.z);
                float x = point.x * factor;
                float y = point.y * factor;
                return new Vector3(x, y, point.z);
            }



            public List<(int, int, char, ConsoleColor)> DrawRotatingCube(float angle, float size = 5f)
            {
                List<(int, int, char, ConsoleColor)> buffer = new List<(int, int, char, ConsoleColor)>();

                var vertices = new Vector3[]
                {
                    new Vector3(-size, -size, -size),
                    new Vector3( size, -size, -size),
                    new Vector3( size,  size, -size),
                    new Vector3(-size,  size, -size),
                    new Vector3(-size, -size,  size),
                    new Vector3( size, -size,  size),
                    new Vector3( size,  size,  size),
                    new Vector3(-size,  size,  size),
                };

                var rotatedVertices = new Vector3[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    rotatedVertices[i] = v.RotateY(vertices[i], angle);
                    rotatedVertices[i] = v.RotateX(rotatedVertices[i], angle * 1.3f);
                    rotatedVertices[i] = v.RotateZ(rotatedVertices[i], angle * 0.7f);
                }

                var screenVertices = new Vector3[vertices.Length];
                for (int i = 0; i < rotatedVertices.Length; i++)
                {
                    Vector3 translated = rotatedVertices[i] + new Vector3(0, 0, 15f);
                    screenVertices[i] = Project(translated, 25f, 10f);
                }

                // Define cube edges
                var edges = new (int, int)[]
                {
                (0, 1), (1, 2), (2, 3), (3, 0),
                (4, 5), (5, 6), (6, 7), (7, 4),
                (0, 4), (1, 5), (2, 6), (3, 7)
                };

                foreach (var (start, end) in edges)
                {
                    Vector3 pointA = screenVertices[start];
                    Vector3 pointB = screenVertices[end];
                    var linePoints = helper.DrawLine(
                        new Vector3((int)pointA.x, (int)pointA.y, 0),
                        new Vector3((int)pointB.x, (int)pointB.y, 0),
                        '#',
                        ConsoleColor.Cyan
                    );
                    buffer.AddRange(linePoints);
                }

                return buffer;
            }

            public List<(int, int, char, ConsoleColor)> DrawRotatingTriangle(float angle, float size = 8, bool filled = false)
            {
                var v1 = new Vector3(-size, -size, -size);
                var v2 = new Vector3(size, -size, 0);
                var v3 = new Vector3(0, size, size);

                v1 = v.RotateZ(v.RotateY(v.RotateX(v1, angle), angle * 0.8f), angle * 1.2f);
                v2 = v.RotateZ(v.RotateY(v.RotateX(v2, angle), angle * 0.8f), angle * 1.2f);
                v3 = v.RotateZ(v.RotateY(v.RotateX(v3, angle), angle * 0.8f), angle * 1.2f);

                v1 = Project(v1 + new Vector3(0, 0, 15f), 25f, 10f);
                v2 = Project(v2 + new Vector3(0, 0, 15f), 25f, 10f);
                v3 = Project(v3 + new Vector3(0, 0, 15f), 25f, 10f);

                if (filled == false)
                {
                    //DrawTriangle not filled
                    return GetTriangle(v1, v2, v3); ;
                }
                else
                {
                    //Draw filled triangle
                    return FillTriangle(v1, v2, v3);
                }
            }

            public List<(int, int, char, ConsoleColor)> DrawRotatingPyramid(float angle, float size = 8)
            {
                var v1 = new Vector3(-size, -size, -size);
                var v2 = new Vector3(size, -size, -size);
                var v3 = new Vector3(0, size, -size);
                var v4 = new Vector3(0, 0, size);

                v1 = v.RotateZ(v.RotateY(v.RotateX(v1, angle), angle * 0.8f), angle * 1.2f);
                v2 = v.RotateZ(v.RotateY(v.RotateX(v2, angle), angle * 0.8f), angle * 1.2f);
                v3 = v.RotateZ(v.RotateY(v.RotateX(v3, angle), angle * 0.8f), angle * 1.2f);
                v4 = v.RotateZ(v.RotateY(v.RotateX(v4, angle), angle * 0.8f), angle * 1.2f);

                float distance = 25f;
                v1 = Project(v1 + new Vector3(0, 0, distance), 30f, 15f);
                v2 = Project(v2 + new Vector3(0, 0, distance), 30f, 15f);
                v3 = Project(v3 + new Vector3(0, 0, distance), 30f, 15f);
                v4 = Project(v4 + new Vector3(0, 0, distance), 30f, 15f);

                var buffer = new List<(int, int, char, ConsoleColor)>();

                buffer.AddRange(helper.DrawLine(v1, v2, '+', ConsoleColor.Cyan)); // Base
                buffer.AddRange(helper.DrawLine(v2, v3, '+', ConsoleColor.Cyan)); // Base
                buffer.AddRange(helper.DrawLine(v3, v1, '+', ConsoleColor.Cyan)); // Base
                buffer.AddRange(helper.DrawLine(v1, v4, '#', ConsoleColor.Yellow)); // Side
                buffer.AddRange(helper.DrawLine(v2, v4, '#', ConsoleColor.Yellow)); // Side
                buffer.AddRange(helper.DrawLine(v3, v4, '#', ConsoleColor.Yellow)); // Side

                return buffer;
            }


            public List<(int, int, char, ConsoleColor)> DrawCircle(int radius, char px, ConsoleColor col)
            {
                var points = new List<(int, int, char, ConsoleColor)>();

                for (int deg = 0; deg < 360; deg++)
                {
                    float angle = deg * (float)(Math.PI / 180.0);
                    int x = (int)(radius * Math.Cos(angle));
                    int y = (int)(radius * Math.Sin(angle));
                    points.Add((x, y, px, col));
                }

                return points;
            }


            //Requires proper shading still
            public List<(int, int, char, ConsoleColor)> DrawSphere(int radius, char px)
            {
                var points = new List<(int, int, char, ConsoleColor)>();

                for (int thetaDeg = 0; thetaDeg < 360; thetaDeg += 5)
                {
                    for (int phiDeg = 0; phiDeg <= 180; phiDeg += 5)
                    {
                        float theta = thetaDeg * (float)(Math.PI / 180.0);
                        float phi = phiDeg * (float)(Math.PI / 180.0);
                        float x = radius * (float)Math.Cos(theta) * (float)Math.Sin(phi);
                        float y = radius * (float)Math.Sin(theta) * (float)Math.Sin(phi);
                        float z = radius * (float)Math.Cos(phi);
                        Vector3 projected = Project(new Vector3(x, y, z), 30f, 15f);
                        points.Add(((int)projected.x, (int)projected.y, px, ConsoleColor.Yellow));
                    }
                }
                return points;
            }


        }
    }
}
