using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MinEllipsoid
{
    class Program
    {
        static int Main()
        {
            int N = 10;
            int n = 10;

            for (int i = 1; i <= N; ++i)
            {
                Points p = generate_points(i, n);
                Console.Write("*");
                List<Vector3d> planes_hull = create_convex_hulls_plane_list(p);
                Console.Write("*");
                List<Vector3d> points_hull = create_convex_hulls_point_list(planes_hull);
                Console.WriteLine("*");

                //PetRub ell1 = new PetRub();
                //Ellipsoid PR = ell1.PetRub_Ellipsoid(points_hull);
                //Console.WriteLine(i.ToString()+")   VolumePR = "+ PR.Volume());

                Vivien ell2 = new Vivien();
                Ellipsoid Viv = ell2.Vivien_Ellipsoid(planes_hull, points_hull);
                Console.WriteLine(i.ToString() + ")   VolumeViv =" + Viv.Volume());
            }
            Console.ReadKey();
            return 0;
        }

        public static Points generate_points(int N, int n)
        {
            Points_generator g = new Points_generator(n);
            g.runGenerator(N);
            Points p = new Points();
            p.ReadFromFile(N);
            return p;
        }
        public static List<Vector3d> create_convex_hulls_plane_list(Points p)
        {
            Convex_hull p_list = new Convex_hull(p);
            List<Vector3d> plane_list = p_list.Create_convex_hull();
            return plane_list;
        }
        public static List<Vector3d> create_convex_hulls_point_list(List<Vector3d> plane_list)
        {
            List<Vector3d> point_list = plane_list.Distinct().ToList();
            return point_list;
        }
        public static List<Vector3d> Translate(List<Vector3d> convex_hull, double x, double y, double z)
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
            return temp;
        }
        public static List<Vector3d> Rotate(List<Vector3d> convex_hull, Vector3d a) 
        {
            List<Vector3d> temp = new List<Vector3d>();
            if (a.X < 0)
            {
                while (convex_hull.Count != 0)
                {
                    Vector3d tmp = convex_hull[0];
                    Vector3d t = new Vector3d();
                    t.X = -tmp.X;
                    t.Y = tmp.Y;
                    t.Z = tmp.Z;
                    temp.Add(t);
                    convex_hull.RemoveAt(0);
                }
                a.X = -1 * a.X;
                return Rotate(temp,a);
            }
            if (a.Y < 0)
            {
                while (convex_hull.Count != 0)
                {
                    Vector3d tmp = convex_hull[0];
                    Vector3d t = new Vector3d();
                    t.X = tmp.X;
                    t.Y = -tmp.Y;
                    t.Z = tmp.Z;
                    temp.Add(t);
                    convex_hull.RemoveAt(0);
                }
                a.Y = -1 * a.Y;
                return Rotate(temp, a);
            }
            if (a.Z < 0)
            {
                while (convex_hull.Count != 0)
                {
                    Vector3d tmp = convex_hull[0];
                    Vector3d t = new Vector3d();
                    t.X = tmp.X;
                    t.Y = tmp.Y;
                    t.Z = -tmp.Z;
                    temp.Add(t);
                    convex_hull.RemoveAt(0);
                }
                a.Z = -1 * a.Z;
                return Rotate(temp, a);
            }
            double cos_xy = a.X / (Math.Sqrt(a.X * a.X + a.Y * a.Y));
            double sin_xy = Math.Sin(Math.Acos(cos_xy));
            while (convex_hull.Count != 0)
            {
                Vector3d tmp = convex_hull[0];
                Vector3d t = new Vector3d();
                t.X = tmp.X*cos_xy + tmp.Y*sin_xy;
                t.Y = -tmp.X*sin_xy + tmp.Y*cos_xy;
                t.Z = tmp.Z;
                temp.Add(t);
                convex_hull.RemoveAt(0);
            }
            Vector3d te = a;
            a.X = te.X * cos_xy + te.Y * sin_xy;
            a.Y = -te.X * sin_xy + te.Y * cos_xy;
            double cos_xz = a.X / (Math.Sqrt(a.X * a.X + a.Z * a.Z));
            double sin_xz = Math.Sin(Math.Acos(cos_xz));
            while (temp.Count != 0)
            {
                Vector3d tmp = temp[0];
                Vector3d t = new Vector3d();
                t.X = tmp.X * cos_xz + tmp.Z * sin_xz;
                t.Y = tmp.Y;
                t.Z = -tmp.X * sin_xz + tmp.Z * cos_xz;
                convex_hull.Add(t);
                temp.RemoveAt(0);
            }
            return convex_hull;
        }
    }
}
