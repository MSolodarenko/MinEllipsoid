using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Collections;
using System.Collections.Generic;

namespace MinEllipsoid_with_visualisation
{
    class MyApplication
    {
        [STAThread]
        public static void Main()
        {
            //Points_generator gen = new Points_generator(15);
            //gen.runGenerator();
            Points p = new Points();
            p.ReadFromFile();
            Console.WriteLine("I start creating convex_hull");
            Convex_hull p_list = new Convex_hull(p);
            //List<Vector3d> point_list = p_list.Create_convex_hull();
            List<List<Vector3d>> point_list = p_list.Create_convex_hull();
            //Ellipsoid ell = new Ellipsoid(p, point_list);

            int iter = 0;

            using (var game = new GameWindow(700,700))
            {
                game.Load += (sender, e) =>
                {
                    // setup settings, load textures, sounds
                    game.VSync = VSyncMode.On;
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    // add game logic, input handling
                    if (game.Keyboard[Key.Escape])
                    {
                        game.Exit();
                    }
                    if (game.Keyboard[Key.Left])
                    {
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.Rotate(1, 0, 1, 0);
                    }
                    if (game.Keyboard[Key.Right])
                    {
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.Rotate(1, 0, -1, 0);
                    }
                    if (game.Keyboard[Key.Up])
                    {
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.Rotate(1, 1, 0, 0);
                    }
                    if (game.Keyboard[Key.Down])
                    {
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.Rotate(1, -1, 0, 0);
                    }
                    if (game.Keyboard[Key.Q])
                    {
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.Scale(1.01, 1.01, 1.01);
                    }
                    if (game.Keyboard[Key.Z])
                    {
                        GL.MatrixMode(MatrixMode.Modelview);
                        GL.Scale(0.99, 0.99, 0.99);
                    }
                    if (game.Keyboard[Key.M])
                    {
                        if (iter < point_list.Count-1)
                            iter++;
                        else iter = point_list.Count - 1;
                    }
                    if (game.Keyboard[Key.N])
                    {
                        if (iter > 0)
                            iter--;
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                    GL.Enable(EnableCap.DepthTest);
                    //GL.ClearColor(Color.SkyBlue);

                    GL.MatrixMode(MatrixMode.Projection);
                    
                    GL.LoadIdentity();
                    GL.Ortho(-5.0, 5.0, -5.0, 5.0, -7.0, 7.0);
                    GL.Scale(5, 5, 5);
                    //ell.build_Ellipsoid();
                    Decarts_Lines();

                    DrawAlgo(point_list, iter);

                    //Draw_Plains(point_list);

                    //Draw_Borders_of_Plains(point_list);
                    
                    DrawPoints(p, Color.Green);
                    
                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }    
        }
        public static void DrawAlgo(List<List<Vector3d>> res, int iter)
        {
            if (iter < res.Count)
            {
                Draw_Plains(res[iter]);

                Draw_Borders_of_Plains(res[iter]);
            }
        }
        public static void DrawPoints(Points p, Color XColor)     
        {
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < p.num_of_points; ++i)
            {
                //GL.Color3(XColor);
                switch (i%10)
                {
                    case 0:
                        GL.Color3(Color.Red);
                        break;
                    case 1:
                        GL.Color3(Color.Green);
                        break;
                    case 2:
                        GL.Color3(Color.Blue);
                        break;
                    case 3:
                        GL.Color3(Color.Violet);
                        break;
                    case 4:
                        GL.Color3(Color.Orange);
                        break;
                    case 5:
                        GL.Color3(Color.Yellow);
                        break;
                    case 6:
                        GL.Color3(Color.White);
                        break;
                    case 7:
                        GL.Color3(Color.Gray);
                        break;
                    case 8:
                        GL.Color3(Color.Aqua);
                        break;
                    case 9:
                        GL.Color3(Color.Brown);
                        break;
                    default:
                        GL.Color3(XColor);
                        break;
                }
                Circle(p.points[i]);
            }
            GL.End();
        }
        public void Ellips(double a, double b)          //not finished
        {
            GL.Begin(PrimitiveType.Points);
            for (double angle = 1.0f; angle < 361.0f; angle += 0.2)
            {
                double x2 = a * Math.Cos(angle);
                double y2 = b * Math.Sin(angle);
                double z2 = 0;
                GL.Vertex3(x2, y2, z2);
            }
            GL.End();
        }
        public static void Circle(Vector3d a)      
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(a.X, a.Y, a.Z);
            double radius = 0.01;
            for (double angle = 1.0f; angle < 361.0f; angle += 0.2)
            {
                double x2 = a.X + Math.Sin(angle) * radius;
                double y2 = a.Y + Math.Cos(angle) * radius;
                double z2 = a.Z;
                GL.Vertex3(x2, y2, z2);
            }
            GL.End();
        }
        public static void Decarts_Lines()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(100, 0, 0);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 100, 0);
            GL.Vertex3(0, 0, 0);
            GL.Color3(Color.Blue);
            GL.Vertex3(0, 0, 100);
            GL.Vertex3(0, 0, 0);
            GL.End();
        }
        public static void Color_Chooser(Vector3d a)
        {
            //if (a.Z <= -0.5)
            //    GL.Color3(Color.Black);
            //else if (a.Z > -0.5 && a.Z <= 0)
            //    GL.Color3(Color.Purple);
            //else if (a.Z > 0 && a.Z < 0.5)
            //    GL.Color3(Color.Violet);
            //else GL.Color3(Color.Pink);
            if (a.Z <= -0.8)
                GL.Color3(Color.Black);
            else if (a.Z > -0.8 && a.Z <= -0.6)
                GL.Color3(Color.DarkMagenta);
            else if (a.Z > -0.6 && a.Z <= -0.4)
                GL.Color3(Color.Purple);
            else if (a.Z > -0.4 && a.Z <= -0.2)
                GL.Color3(Color.DarkViolet);
            else if (a.Z > -0.2 && a.Z <= 0)
                GL.Color3(Color.Blue);
            else if (a.Z > 0 && a.Z <= 0.2)
                GL.Color3(Color.DodgerBlue);
            else if (a.Z > 0.2 && a.Z <= 0.4)
                GL.Color3(Color.DeepSkyBlue);
            else if (a.Z > 0.4 && a.Z <= 0.6)
                GL.Color3(Color.MediumSpringGreen);
            else if (a.Z > 0.6 && a.Z <= 0.8)
                GL.Color3(Color.LightCyan);
            else 
                GL.Color3(Color.White);
        }
        public static void Draw_Plains(List<Vector3d> point_list)
        {
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < point_list.Count; ++i)
            {
                Color_Chooser(point_list[i]);
                GL.Vertex3(point_list[i]);
            }
            GL.End();
        }
        public static void Draw_Borders_of_Plains(List<Vector3d> point_list)
        {
            for (int i = 0; i < point_list.Count - 2; i = i + 3)
            {
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(Color.Black);
                GL.Vertex3(point_list[i]);
                GL.Vertex3(point_list[i + 1]);
                GL.Vertex3(point_list[i + 2]);
                GL.End();
            }
        }
    }
}