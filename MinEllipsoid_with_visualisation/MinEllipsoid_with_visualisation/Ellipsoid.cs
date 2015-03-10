using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MinEllipsoid_with_visualisation
{
    class Ellipsoid
    {
        Points p;           // points cloud
        Points convex_hull; // convex hull // each 3 points form side of convex_hull
        int a_diam, b_diam;
        public Ellipsoid(Points temp, Points temp2)
        {
            p = temp;
            convex_hull = temp2;
        }
        public void build_Ellipsoid()
        {
            find_max_distance_between_points();
            GL.PushMatrix();
            GL.Translate((p.points[a_diam].X + p.points[b_diam].X) / 2, (p.points[a_diam].Y + p.points[b_diam].Y) / 2, (p.points[a_diam].Z + p.points[b_diam].Z) / 2);
            
            GL.PopMatrix();
        }
        public void find_max_distance_between_points()
        {
            double maxDistance = 0;
            a_diam = 0; b_diam = 0;                       // The most distant points of EPs
            for (int i = 0; i < 6; ++i)
                for (int j = 0; j < 6; ++j)
                {
                    Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance < Points_distance(p.points[i], p.points[j]))
                    {
                        maxDistance = Points_distance(p.points[i], p.points[j]);
                        a_diam = i;
                        b_diam = j;
                    }
                }
        }
        public double Points_distance(Vector3d a, Vector3d b)
        {
            Console.WriteLine("I'm counting distance between two points:" + a.ToString() + "and" + b.ToString());
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
            return result;
        }
        public void rotate()
        {
            
        }
    }
}
