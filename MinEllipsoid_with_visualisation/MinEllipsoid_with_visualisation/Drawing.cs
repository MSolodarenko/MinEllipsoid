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
            //Points_generator gen = new Points_generator(10);
            //gen.runGenerator();
            Points p = new Points();
            p.ReadFromFile();
            Console.WriteLine("I start creating convex_hull");
            Convex_hull p_list = new Convex_hull(p);
            List<Vector3d> point_list = p_list.Create_convex_hull();
            
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
                };

                game.RenderFrame += (sender, e) =>
                {
                    // render graphics
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    GL.LoadIdentity();
                    GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

                    Decarts_Lines();
                    DrawPoints(p);

                    GL.Begin(PrimitiveType.Triangles);
                    for (int i = 0; i < point_list.Count; ++i )
                    {
                        if (point_list[i].Z < -0.5)
                            GL.Color3(Color.Black);
                        else if (point_list[i].Z > -0.5 && point_list[i].Z < 0)
                            GL.Color3(Color.Maroon);
                        else if (point_list[i].Z > 0 && point_list[i].Z < 0.5)
                            GL.Color3(Color.Crimson);
                        else GL.Color3(Color.Red);
                        GL.Vertex3(point_list[i]);
                    }
                    GL.End();

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }    
        }
        public static void DrawPoints(Points p)     
        {
            for (int i = 0; i < p.num_of_points; ++i)
            {
                GL.Color3(Color.Red);
                Circle(p.points[i]);
            }
        }
        public static void Circle(Vector3d a)      
        {
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Vertex3(a);
            double radius = 0.005;
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
            GL.Vertex2(-1, 0);
            GL.Vertex2(1, 0);
            GL.Vertex2(0, 1);
            GL.Vertex2(0, -1);
            GL.End();
        }
    }
}