using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.IO;
using MIConvexHull;

namespace MinEllipsoid
{
    class Program
    {
        static int Main()
        {
            int N = 10;
            int number_of_points = 50;

            bool enable_log = false;

            StreamWriter sw = new StreamWriter(@"output.txt");

            Ellipsoid[] PR = new Ellipsoid[N+1];
            Ellipsoid[] Vi = new Ellipsoid[N+1];
            sw.WriteLine("    VolumePR VolumeVi Paral_PR Paral_Vi ");
            for (int i = 1; i <= N; ++i)
            {
                Points p = generate_points(i, number_of_points);

                Console.Write("*");
                List<Vector3d> planes_hull = create_convex_hulls_plane_list(p);
                Console.WriteLine("*");
                List<Vector3d> points_hull = create_convex_hulls_point_list(planes_hull);

                PetRub ell1 = new PetRub();
                PR[i] = ell1.PetRub_Ellipsoid(points_hull);
                Console.Write("*");
                points_hull = create_convex_hulls_point_list(planes_hull);

                Vivien ell2 = new Vivien();
                Vi[i] = ell2.Vivien_Ellipsoid(planes_hull, points_hull, enable_log);

                Console.WriteLine("");
                Console.WriteLine(i.ToString() + ")   VolumePR = " + PR[i].Volume());
                Console.WriteLine(i.ToString() + ")   VolumeVi = " + Vi[i].Volume());
                Console.WriteLine("");
                Console.WriteLine("Volume of paral PR = " + PR[i].volume_of_paral);
                Console.WriteLine("Volume of paral Vi= " + Vi[i].volume_of_paral);
                Console.WriteLine("");

                sw.WriteLine(i.ToString() + ")   VolumePR = " + PR[i].Volume());
                sw.WriteLine(i.ToString() + ")   VolumeVi = " + Vi[i].Volume());
                sw.WriteLine("Volume of paral PR = " + PR[i].volume_of_paral);
                sw.WriteLine("Volume of paral Vi = " + Vi[i].volume_of_paral);
                sw.WriteLine("");
            }
            sw.Close();

            output_results_file(PR, Vi, N, number_of_points);

            Console.ReadKey();
            return 0;
        }
        public static void output_results_file(Ellipsoid[] PR, Ellipsoid[] VI, int n, int numb_of_points)
        {
            StreamWriter sw = new StreamWriter(@"results.txt");
            double a = 0, b = 0;
            for (int i = 1; i <= n; ++i )
            {
                double t = VI[i].Volume() / PR[i].Volume();
                double t1 = VI[i].volume_of_paral / PR[i].volume_of_paral;
                a += t;
                b += t1;
            }
            a /= n;
            b /= n;
            a = Math.Round(a, 4);
            b = Math.Round(b, 4);
            sw.WriteLine("Number of tests = " + n.ToString());
            sw.WriteLine("Number of points = " + numb_of_points.ToString());
            sw.WriteLine("Average ratio between volumes of ellipsoids (Vi/PR) = " + a.ToString());
            sw.WriteLine("Average ratio between volumes of parallelepipeds (Vi/PR) = " + b.ToString());
            sw.WriteLine("");
            sw.Close();
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
            //List<Vector3d> plane_list = p_list.Create_convex_hull();
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
        public static List<Vector3d> Translate(List<Vector3d> convex_hull, Vector3d point)
        {
            return Translate(convex_hull, point.X, point.Y, point.Z);
        }
        public static Cuboid Translate(Cuboid paral, double x, double y, double z)
        {
            Cuboid t = new Cuboid(Translate(paral.toList_Vector3d(), x, y, z));
            return t;
        }
        public static Cuboid Translate(Cuboid paral, Vector3d point)
        {
            return Translate(paral, point.X, point.Y, point.Z);
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
