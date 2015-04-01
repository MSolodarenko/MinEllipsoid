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
        public Points p;           // points cloud
        public List<Vector3d> convex_hull; // list which contain points to form convex hull
        public Vector3d a_diam, b_diam;
        public Ellipsoid(Points Point_Cloud, List<Vector3d> Convex_hull)
        {
            p = Point_Cloud;
            convex_hull = Convex_hull.Distinct().ToList();
            a_diam = new Vector3d();
            b_diam = new Vector3d();
        }
        public void build_Ellipsoid()
        {
            find_max_distance_between_points();

            //GL.Translate((p.points[a_diam].X + p.points[b_diam].X) / 2, (p.points[a_diam].Y + p.points[b_diam].Y) / 2, (p.points[a_diam].Z + p.points[b_diam].Z) / 2);
            Translate((a_diam.X + b_diam.X) / 2, (a_diam.Y + b_diam.Y) / 2, (a_diam.Z + b_diam.Z) / 2);
            //GL.Rotate(angle(),axis());
            Rotate(angle(), axis());

        }
        public void Translate(double x, double y, double z)
        {
            List<Vector3d> temp = new List<Vector3d>();
            while (convex_hull.Count != 0)
            {
                Vector3d t = convex_hull[0];
                t.X -= x;
                t.Y -= y;
                t.Z -= z;
                temp.Add(t);
                convex_hull.RemoveAt(0);
            }
            convex_hull = temp;
        }
        public void Rotate(double angle, Vector3d axis)
        {
            double m11 = 0, m12 = 0, m13 = 0, m21 = 0, m22 = 0, m23 = 0, m31 = 0, m32 = 0, m33 = 0;
            m11 = Math.Cos(angle) + (1 - Math.Cos(angle)) * axis.X * axis.X;
            m12 = (1 - Math.Cos(angle)) * axis.X * axis.Y - axis.Z * Math.Sin(angle);
            m13 = (1 - Math.Cos(angle)) * axis.X * axis.Z + Math.Sin(angle) * axis.Y;
            m21 = (1 - Math.Cos(angle)) * axis.Y * axis.X + Math.Sin(angle) * axis.Z;
            m22 = Math.Cos(angle) + (1 - Math.Cos(angle)) * axis.Y * axis.Y;
            m23 = (1 - Math.Cos(angle)) * axis.Y * axis.Z - Math.Sin(angle) * axis.X;
            m31 = (1 - Math.Cos(angle)) * axis.Z * axis.X - Math.Sin(angle) * axis.Y;
            m32 = (1 - Math.Cos(angle)) * axis.Z * axis.Y + Math.Sin(angle) * axis.X;
            m33 = Math.Cos(angle) + (1 - Math.Cos(angle)) * axis.Z * axis.Z;

            List<Vector3d> temp = new List<Vector3d>();
            Vector3d t = new Vector3d();
            while (convex_hull.Count != 0)
            {
                Vector3d tmp = convex_hull[0];
                t.X = tmp.X * m11 + tmp.Y * m12 + tmp.Z * m13;
                t.Y = tmp.X * m21 + tmp.Y * m22 + tmp.Z * m23;
                t.Z = tmp.X * m31 + tmp.Y * m32 + tmp.Z * m33;
                temp.Add(t);
                convex_hull.RemoveAt(0);
            }
            convex_hull = temp;
        }
        public double angle()
        {
            Vector3d a = a_diam;
            Vector3d b = new Vector3d(1, 0, 0);
            double ab = a.X * b.X + a.Y * b.Y + a.Z * b.Z;
            double mod_a = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
            double mod_b = Math.Sqrt(b.X * b.X + b.Y * b.Y + b.Z * b.Z);
            return Math.Acos(ab/(mod_a*mod_b));
        }
        public Vector3d axis()
        {
            Vector3d M0 = a_diam;
            Vector3d M1 = new Vector3d(0, 0, 0);
            Vector3d M2 = new Vector3d(1, 0, 0);
            double A = (M1.Y - M0.Y) * (M2.Z - M0.Z) - (M2.Y - M0.Y) * (M1.Z - M0.Z);
            double B = (M1.Z - M0.Z) * (M2.X - M0.X) - (M1.X - M0.X) * (M2.Z - M0.Z);
            double C = (M1.X - M0.X) * (M2.Y - M0.Y) - (M2.X - M0.X) * (M1.Y - M0.Y);
            Vector3d result = new Vector3d(A, B, C);
            return result;
        }
        public void find_max_distance_between_points()
        {
            double maxDistance = 0;
            for (int i = 0; i < convex_hull.Count; ++i)
                for (int j = 0; j < convex_hull.Count; ++j)
                {
                    //Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance < Points_distance(convex_hull[i], convex_hull[j]))
                    {
                        maxDistance = Points_distance(convex_hull[i], convex_hull[j]);
                        a_diam = convex_hull[i];
                        b_diam = convex_hull[j];
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
        
    }
}
