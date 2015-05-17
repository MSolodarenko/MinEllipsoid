using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MinEllipsoid
{
    class PetRub
    {
        public Vector3d A;
        public Vector3d B;
        public int a_num;
        public int b_num;
        public List<Vector3d> points;
        public double kx, ky, kz;
        public class Cuboid
        {
            public Vector3d A, B, C, D;
            public Vector3d A1, B1, C1, D1;
            public Cuboid()
            {
                A = new Vector3d();
                B = new Vector3d();
                C = new Vector3d();
                D = new Vector3d();
                A1 = new Vector3d();
                B1 = new Vector3d();
                C1 = new Vector3d();
                D1 = new Vector3d();
            }
        }
        public Ellipsoid PetRub_Ellipsoid(List<Vector3d> point_list)
        {
            points = point_list;
            find_most_distant_points();
            translate_and_rotate_space_on_diam();
            Cuboid harry = build_min_cuboid();
            harry = center_cuboid(harry);
            transfigure_cuboid_to_cube_1x1x1(harry);
            Ellipsoid potter = build_sphere();
            potter = transfigure_sphere_to_ellipsoid(potter);
            bool all_ok = check(potter);
            return potter;
        }
        public bool check(Ellipsoid alastor)
        {
            for (int i = 0; i < points.Count(); ++i )
            {
                double t =formula(alastor, points[i]);
                if (t > 0.000001)
                    return false;
            }
            return true;
        }
        public double formula(Ellipsoid severus, Vector3d snape)
        {
            return Math.Pow(snape.X / severus.a, 2) + Math.Pow(snape.Y / severus.b, 2) + Math.Pow(snape.Z / severus.c, 2) - 1;
        }
        public Ellipsoid transfigure_sphere_to_ellipsoid(Ellipsoid minerva)
        {
            minerva.a = minerva.a * kx;
            minerva.b = minerva.b * ky;
            minerva.c = minerva.c * kz;

            List<Vector3d> temp = new List<Vector3d>();
            while (points.Count != 0)
            {
                Vector3d t = points[0];
                t.X = t.X*kx;
                t.Y = t.Y*ky;
                t.Z = t.Z*kz;
                temp.Add(t);
                points.RemoveAt(0);
            }
            points = temp;

            return minerva;
        }
        public Ellipsoid build_sphere()
        {
            Vector3d bellatrix = find_farthest_point_from_0x0x0();
            Ellipsoid riddle = new Ellipsoid(Module(bellatrix), Module(bellatrix), Module(bellatrix));
            return riddle;
        }
        public void transfigure_cuboid_to_cube_1x1x1(Cuboid hermione)
        {
            kx = hermione.D.X;
            ky = hermione.D.Y;
            kz = hermione.D.Z;

            List<Vector3d> temp = new List<Vector3d>();
            while (points.Count != 0)
            {
                Vector3d t = points[0];
                t.X = t.X/kx;
                t.Y = t.Y/ky;
                t.Z = t.Z/kz;
                temp.Add(t);
                points.RemoveAt(0);
            }
            points = temp;
        }
        public Cuboid center_cuboid(Cuboid albus)
        {
            double x = (albus.A.X + albus.C1.X) / 2;
            double y = (albus.A.Y + albus.C1.Y) / 2;
            double z = (albus.A.Z + albus.C1.Z) / 2;
            points = Program.Translate(points, x, y, z);
            List<Vector3d> temp = new List<Vector3d>();
            temp.Add(albus.A); temp.Add(albus.A1); temp.Add(albus.B); temp.Add(albus.B1);
            temp.Add(albus.C); temp.Add(albus.C1); temp.Add(albus.D); temp.Add(albus.D1);
            temp = Program.Translate(temp, x, y, z);
            albus.A = temp[0]; albus.A1 = temp[1]; albus.B = temp[2]; albus.B1 = temp[3];
            albus.C = temp[4]; albus.C1 = temp[5]; albus.D = temp[6]; albus.D1 = temp[7];
            return albus;
        }
        public Cuboid build_min_cuboid()
        {
            Cuboid tom = new Cuboid();

            rotate_side_space();

            Vector3d a = find_left_point();
            Vector3d b = find_up_point();
            Vector3d c = find_right_point();
            Vector3d d = find_down_point();

            tom.A.X = B.X; tom.A.Y = a.Y; tom.A.Z = d.Z;
            tom.B.X = B.X; tom.B.Y = a.Y; tom.B.Z = b.Z;
            tom.C.X = B.X; tom.C.Y = c.Y; tom.C.Z = b.Z;
            tom.D.X = B.X; tom.D.Y = c.Y; tom.D.Z = d.Z;
            tom.A1 = tom.A; tom.A1.X = A.X;
            tom.B1 = tom.B; tom.B1.X = A.X;
            tom.C1 = tom.C; tom.C1.X = A.X;
            tom.D1 = tom.D; tom.D1.X = A.X;

            return tom;
        }
        public Vector3d find_left_point()
        {
            Vector3d t = new Vector3d();
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
                {
                    //Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance > points[i].Y)
                    {
                        maxDistance = points[i].Y;
                        t = points[i];
                    }
                }
            return t;
        }
        public Vector3d find_right_point()
        {
            Vector3d t = new Vector3d();
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                //Console.WriteLine("I'm searching max distance between two EPs");
                if (maxDistance < points[i].Y)
                {
                    maxDistance = points[i].Y;
                    t = points[i];
                }
            }
            return t;
        }
        public Vector3d find_up_point()
        {
            Vector3d t = new Vector3d();
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                //Console.WriteLine("I'm searching max distance between two EPs");
                if (maxDistance > points[i].Z)
                {
                    maxDistance = points[i].Z;
                    t = points[i];
                }
            }
            return t;
        }
        public Vector3d find_down_point()
        {
            Vector3d t = new Vector3d();
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                //Console.WriteLine("I'm searching max distance between two EPs");
                if (maxDistance < points[i].Z)
                {
                    maxDistance = points[i].Z;
                    t = points[i];
                }
            }
            return t;
        }
        public void rotate_side_space()
        {
            List<Vector2d> p = new List<Vector2d>();
            for (int i = 0; i < points.Count(); ++i)
            {
                Vector2d t = new Vector2d();
                t.X = points[i].Y;
                t.Y = points[i].Z;
                p.Add(t);
            }
            Vector2d[] ab = find_most_distant_points(p);
            
            Vector2d C = new Vector2d();
            C.X = ab[0].X - ab[1].X;
            C.Y = ab[0].Y - ab[1].Y;

            p = Rotate2d(p,C);

            List<Vector3d> temp = new List<Vector3d>();
            while (points.Count != 0)
            {
                Vector3d t = points[0];
                t.Y = p[0].X;
                t.Z = p[0].Y;
                temp.Add(t);
                points.RemoveAt(0);
                p.RemoveAt(0);
            }
            points = temp;
            A = points[a_num];
            B = points[b_num];
        }
        public List<Vector2d> Rotate2d(List<Vector2d> p, Vector2d a)
        {
            List<Vector2d> temp = new List<Vector2d>();
            if (a.X < 0)
            {
                while (p.Count != 0)
                {
                    Vector2d tmp = p[0];
                    Vector2d t = new Vector2d();
                    t.X = -tmp.X;
                    t.Y = tmp.Y;
                    temp.Add(t);
                    p.RemoveAt(0);
                }
                a.X = -1 * a.X;
                return Rotate2d(temp, a);
            }
            if (a.Y < 0)
            {
                while (p.Count != 0)
                {
                    Vector2d tmp = p[0];
                    Vector2d t = new Vector2d();
                    t.X = tmp.X;
                    t.Y = -tmp.Y;
                    temp.Add(t);
                    p.RemoveAt(0);
                }
                a.Y = -1 * a.Y;
                return Rotate2d(temp, a);
            }
            double cos_xy = a.X / (Math.Sqrt(a.X * a.X + a.Y * a.Y));
            double sin_xy = Math.Sin(Math.Acos(cos_xy));
            while (p.Count != 0)
            {
                Vector2d tmp = p[0];
                Vector2d t = new Vector2d();
                t.X = tmp.X * cos_xy + tmp.Y * sin_xy;
                t.Y = -tmp.X * sin_xy + tmp.Y * cos_xy;
                temp.Add(t);
                p.RemoveAt(0);
            }
            return temp;
        }
        public void translate_and_rotate_space_on_diam()
        {
            double x = (A.X+B.X)/2;
            double y = (A.Y+B.Y)/2;
            double z = (A.Z+B.Z)/2;
            points = Program.Translate(points, x, y, z);
            B = points[b_num];
            points = Program.Rotate(points, B);
            A = points[a_num];
            B = points[b_num];
        }
        public void find_most_distant_points()
        {
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
                for (int j = 0; j < points.Count; ++j)
                {
                    //Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance < Points_distance(points[i], points[j]))
                    {
                        maxDistance = Points_distance(points[i], points[j]);
                        A = points[i];
                        a_num = i;
                        B = points[j];
                        b_num = j;
                    }
                }
        }
        public Vector3d find_farthest_point_from_0x0x0()
        {
            Vector3d result = new Vector3d();
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
                {
                    //Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance < Module(points[i]))
                    {
                        maxDistance = Module(points[i]);
                        result = points[i];
                    }
                }
            return result;
        }
        public Vector2d[] find_most_distant_points(List<Vector2d> p)
        {
            Vector2d A = new Vector2d();
            Vector2d B = new Vector2d();
            double maxDistance = 0;
            for (int i = 0; i < p.Count; ++i)
                for (int j = 0; j < p.Count; ++j)
                {
                    //Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance < Points_distance(p[i], p[j]))
                    {
                        maxDistance = Points_distance(p[i], p[j]);
                        A = p[i];
                        B = p[j];
                    }
                }
            Vector2d[] result = new Vector2d[2];
            result[0] = A;
            result[1] = B;
            return result;
        }
        public double Module(Vector3d a)
        {
            Vector3d nul = new Vector3d(0, 0, 0);
            return Points_distance(a, nul);
        }
        public double Points_distance(Vector3d a, Vector3d b)
        {
            //Console.WriteLine("I'm counting distance between two points:" + a.ToString() + "and" + b.ToString());
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
            return result;
        }
        public double Points_distance(Vector2d a, Vector2d b)
        {
            //Console.WriteLine("I'm counting distance between two points:" + a.ToString() + "and" + b.ToString());
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2));
            return result;
        }
    }
}
