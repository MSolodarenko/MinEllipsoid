using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

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

                    GL.Begin(PrimitiveType.Lines);
                    GL.Color3(Color.Green);
                    GL.Vertex2(-1, 0);
                    GL.Vertex2(1, 0);
                    GL.Vertex2(0, 1);
                    GL.Vertex2(0, -1);
                    GL.End();

                    //GL.Begin(PrimitiveType.Points);

                    for (int i = 0; i < p.num_of_points; ++i)
                    {
                        //GL.Color3(Color.Red);
                        //GL.Vertex3(p.points[i]);
                        
                        GL.Color3(1.0, 1.0, 0.6);

                        GL.Begin(PrimitiveType.TriangleFan);
                        GL.Vertex3(p.points[i]);
                        double radius = 0.01;
                        for (double angle = 1.0f; angle < 361.0f; angle += 0.2)
                        {
                            double x2 = p.points[i].X + Math.Sin(angle) * radius;
                            double y2 = p.points[i].Y + Math.Cos(angle) * radius;
                            GL.Vertex2(x2, y2);
                        }

                        GL.End();
                    }
                    
                    //GL.End();

                    game.SwapBuffers();
                };

                // Run the game at 60 updates per second
                game.Run(60.0);
            }
             
        }
    }
}