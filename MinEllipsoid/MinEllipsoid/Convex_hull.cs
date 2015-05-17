using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Collections;

namespace MinEllipsoid
{
    class Convex_hull
    {
        Points p;
        int[] ep;       //Extreme points: 0 - minX, 1 - minY, 2 - minZ, 3 - maxX, 4 - maxY, 5 - maxZ
        int a, b, c, d; //First 4 points in convex_hull
        int max;
        public Convex_hull(Points temp)
        {
            p = temp;
        }
        public List<Vector3d> Create_convex_hull()
        {
            List<Vector3d> convex_hull = new List<Vector3d>();    //Each 3 points form plain

            generate_EP();
            //Console.Write(".");
            find_max_distance_between_EPs();
            //Console.Write(".");
            find_most_distant_point_from_line_in_EP();
            //Console.Write(".");
            find_most_distant_point_from_plain_in_EP();
            //Console.Write(".");
            //Console.WriteLine("I'm adding 4 triangles into convex_hull");
            add_plain_to(convex_hull, p.points[a], p.points[b], p.points[c]);
            add_plain_to(convex_hull, p.points[b], p.points[c], p.points[d]);
            add_plain_to(convex_hull, p.points[c], p.points[d], p.points[a]);
            add_plain_to(convex_hull, p.points[d], p.points[a], p.points[b]);

            bool[] outside_hull = new bool[p.num_of_points];        //status of points: false - inside (or on) hull, true - outside
            for (int i = 0; i < p.num_of_points; ++i)
                outside_hull[i] = is_point_outside_hull(p.points[i], convex_hull);
            //Console.Write(".");
            double maxDistance = 0;
            for (int i = 0; i < convex_hull.Count - 2; i = i + 3 )
            {
                Console.Write(".");
                maxDistance = 0;
                for (int k=0; k < p.num_of_points; ++k)
                {
                    if (outside_hull[k])
                    {
                        if (point_on_plain(p.points[k], convex_hull, convex_hull[i], convex_hull[i + 1], convex_hull[i + 2]))
                        {
                            if (maxDistance < Point_plain_distance(p.points[k], convex_hull[i], convex_hull[i + 1], convex_hull[i + 2]))
                            {
                                maxDistance = Point_plain_distance(p.points[k], convex_hull[i], convex_hull[i + 1], convex_hull[i + 2]);
                                max = k;
                            }
                        }
                    }
                }
                for (int j = 0; j < convex_hull.Count - 2; j = j + 3 )
                {
                    if (point_on_plain(p.points[max], convex_hull, convex_hull[j], convex_hull[j+1], convex_hull[j+2]))
                    {
                        add_plain_to(convex_hull, p.points[max], convex_hull[j], convex_hull[j + 1]);
                        add_plain_to(convex_hull, convex_hull[j + 2], convex_hull[j + 1], p.points[max]);
                        add_plain_to(convex_hull, convex_hull[j + 2], p.points[max], convex_hull[j]);
                        convex_hull.RemoveRange(j, 3);
                        j = j - 3;
                        if (i >= j && i>0) 
                            i = i - 3;
                        outside_hull[max] = false;
                    }
                }
            }
            return convex_hull;
        }
        public void generate_EP()
        {
            ep = new int[6];                      //Extreme points: 0 - minX, 1 - minY, 2 - minZ, 3 - maxX, 4 - maxY, 5 - maxZ
            for (int i = 0; i < 6; ++i)
                ep[i] = 0;

            for (int i = 0; i < p.num_of_points; ++i)
            {
                //Console.WriteLine("I'm counting extreme points");
                if (p.points[i].X < p.points[ep[0]].X)
                    ep[0] = i;
                if (p.points[i].Y < p.points[ep[1]].Y)
                    ep[1] = i;
                if (p.points[i].Z < p.points[ep[2]].Z)
                    ep[2] = i;
                if (p.points[i].X > p.points[ep[3]].X)
                    ep[3] = i;
                if (p.points[i].Y > p.points[ep[4]].Y)
                    ep[4] = i;
                if (p.points[i].Z > p.points[ep[5]].Z)
                    ep[5] = i;
            }
        }
        public void find_max_distance_between_EPs()
        {
            double maxDistance = 0;
            a = 0; b = 0;                       // The most distant points of EPs
            for (int i = 0; i < 6; ++i)
                for (int j = 0; j < 6; ++j)
                {
                    //Console.WriteLine("I'm searching max distance between two EPs");
                    if (maxDistance < Points_distance(p.points[ep[i]], p.points[ep[j]]))
                    {
                        maxDistance = Points_distance(p.points[ep[i]], p.points[ep[j]]);
                        a = ep[i];
                        b = ep[j];
                    }
                }
        }
        public void find_most_distant_point_from_line_in_EP()
        {
            double maxDistance = 0;
            for (int i = 0; i < 6; ++i)
            {
                //Console.WriteLine("I'm searching the most distant point from line");
                if (ep[i] != a && ep[i] != b)
                    if (maxDistance < Point_line_distance(p.points[ep[i]], p.points[a], p.points[b]))
                    {
                        maxDistance = Point_line_distance(p.points[ep[i]], p.points[a], p.points[b]);
                        c = ep[i];
                    }
            }
        }
        public void find_most_distant_point_from_plain_in_EP()
        {
            double maxDistance = 0;
            d = 0;
            for (int i = 0; i < 6; ++i)
            {
                //Console.WriteLine("I'm searching the most distant point from plain");
                if (ep[i] != a)
                    if (ep[i] != b)
                        if (ep[i] != c)
                            if (maxDistance < Point_plain_distance(p.points[ep[i]], p.points[a], p.points[b], p.points[c]))
                            {
                                maxDistance = Point_plain_distance(p.points[ep[i]], p.points[a], p.points[b], p.points[c]);
                                d = ep[i];
                            }
            }
        }
        public double Points_distance(Vector3d a, Vector3d b)
        {
            //Console.WriteLine("I'm counting distance between two points:"+a.ToString()+"and"+b.ToString());
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
            return result;
        }
        public double Point_line_distance(Vector3d Point, Vector3d M2, Vector3d M3)
        {
            //Console.WriteLine("I'm counting distance between point and line");
            if (Point == M2 || Point == M3) return 0;
            double ax, ay, az;
            ax = M3.X - M2.X;
            ay = M3.Y - M2.Y;
            az = M3.Z - M2.Z;

            double a_mod = Math.Sqrt(ax * ax + ay * ay + az * az);
            double i = ay * (Point.Z - M3.Z) - az * (Point.Y - M3.Y);     //
            double j = az * (Point.X - M3.X) - ax * (Point.Z - M3.Z);     // Coefficients of multiplying of pointing vector and vector between Point and M3
            double k = ax * (Point.Y - M3.Y) - ay * (Point.X - M3.X);     //
            double a_PointM3_mod = Math.Sqrt(i * i + j * j + k * k);
            if (a_mod != 0)
                return (a_PointM3_mod/a_mod);
            return 0;
        }
        public double Point_plain_distance(Vector3d Point, Vector3d M0, Vector3d M1, Vector3d M2)
        {
            //Console.WriteLine("I'm counting distance between point and plain");
            double A = (M1.Y - M0.Y) * (M2.Z - M0.Z) - (M2.Y - M0.Y) * (M1.Z - M0.Z);
            double B = (M1.Z - M0.Z) * (M2.X - M0.X) - (M1.X - M0.X) * (M2.Z - M0.Z);
            double C = (M1.X - M0.X) * (M2.Y - M0.Y) - (M2.X - M0.X) * (M1.Y - M0.Y);
            double D = M0.X * ((M2.Y - M0.Y) * (M1.Z - M0.Z) - (M1.Y - M0.Y) * (M2.Z - M0.Z)) + M0.Y * ((M1.X - M0.X) * (M2.Z - M0.Z) - (M1.Z - M0.Z) * (M2.X - M0.X)) + M0.Z * ((M2.X - M0.X) * (M1.Y - M0.Y) - (M1.X - M0.X) * (M2.Y - M0.Y));

            double plain_mod = Math.Sqrt(A * A + B * B + C * C);
            double plain_point_mod = Math.Abs(A * Point.X + B * Point.Y + C * Point.Z + D);

            if (plain_mod != 0)
                return plain_point_mod / plain_mod;
            return 0;
        }
        public bool is_point_outside_hull(Vector3d Point, List<Vector3d> convex_hull)
        {
            //Console.WriteLine("Is point outside hull?");
            double ox = 0, oy = 0, oz = 0;
            int i = 0;
            for (; i < convex_hull.Count; ++i )
            {
                ox += convex_hull[i].X;
                oy += convex_hull[i].Y;
                oz += convex_hull[i].Z;
            }
            ox = ox / (double)i; oy = oy / (double)i; oz = oz / (double)i;

            for (i = 2; i < convex_hull.Count; i=i+3 )
            {
                //Console.WriteLine("I'm checking if two points are on one side for hull");
                double A = (convex_hull[i-1].Y - convex_hull[i-2].Y) * (convex_hull[i].Z - convex_hull[i-2].Z) - (convex_hull[i].Y - convex_hull[i-2].Y) * (convex_hull[i-1].Z - convex_hull[i-2].Z);
                double B = (convex_hull[i-1].Z - convex_hull[i-2].Z) * (convex_hull[i].X - convex_hull[i-2].X) - (convex_hull[i-1].X - convex_hull[i-2].X) * (convex_hull[i].Z - convex_hull[i-2].Z);
                double C = (convex_hull[i-1].X - convex_hull[i-2].X) * (convex_hull[i].Y - convex_hull[i-2].Y) - (convex_hull[i].X - convex_hull[i-2].X) * (convex_hull[i-1].Y - convex_hull[i-2].Y);
                double D = convex_hull[i-2].X * ((convex_hull[i].Y - convex_hull[i-2].Y) * (convex_hull[i-1].Z - convex_hull[i-2].Z) - (convex_hull[i-1].Y - convex_hull[i-2].Y) * (convex_hull[i].Z - convex_hull[i-2].Z)) + convex_hull[i-2].Y * ((convex_hull[i-1].X - convex_hull[i-2].X) * (convex_hull[i].Z - convex_hull[i-2].Z) - (convex_hull[i-1].Z - convex_hull[i-2].Z) * (convex_hull[i].X - convex_hull[i-2].X)) + convex_hull[i-2].Z * ((convex_hull[i].X - convex_hull[i-2].X) * (convex_hull[i-1].Y - convex_hull[i-2].Y) - (convex_hull[i-1].X - convex_hull[i-2].X) * (convex_hull[i].Y - convex_hull[i-2].Y));

                if (A * ox + B * oy + C * oz + D > 0 && A * Point.X + B * Point.Y + C * Point.Z + D < 0)
                    return true;
                if (A * ox + B * oy + C * oz + D < 0 && A * Point.X + B * Point.Y + C * Point.Z + D > 0)
                    return true;
            }
                return false;
        }
        public bool point_on_plain(Vector3d Point, List<Vector3d> convex_hull, Vector3d M0, Vector3d M1, Vector3d M2)
        {
            //Console.WriteLine("Is point on plain");
            if (Point == M0) return false;
            if (Point == M1) return false;
            if (Point == M2) return false;
            double ox = 0, oy = 0, oz = 0;
            int i = 0;
            for (; i < convex_hull.Count; ++i)
            {
                ox += convex_hull[i].X;
                oy += convex_hull[i].Y;
                oz += convex_hull[i].Z;
            }
            ox = ox / (double)i; oy = oy / (double)i; oz = oz / (double)i;

            double A = (M1.Y - M0.Y) * (M2.Z - M0.Z) - (M2.Y - M0.Y) * (M1.Z - M0.Z);
            double B = (M1.Z - M0.Z) * (M2.X - M0.X) - (M1.X - M0.X) * (M2.Z - M0.Z);
            double C = (M1.X - M0.X) * (M2.Y - M0.Y) - (M2.X - M0.X) * (M1.Y - M0.Y);
            double D = M0.X * ((M2.Y - M0.Y) * (M1.Z - M0.Z) - (M1.Y - M0.Y) * (M2.Z - M0.Z)) + M0.Y * ((M1.X - M0.X) * (M2.Z - M0.Z) - (M1.Z - M0.Z) * (M2.X - M0.X)) + M0.Z * ((M2.X - M0.X) * (M1.Y - M0.Y) - (M1.X - M0.X) * (M2.Y - M0.Y));

            if (A * ox + B * oy + C * oz + D >= 0 && A * Point.X + B * Point.Y + C * Point.Z + D >= 0)
                return false;
            if (A * ox + B * oy + C * oz + D <= 0 && A * Point.X + B * Point.Y + C * Point.Z + D <= 0)
                return false;
            return true;
        }
        public void add_plain_to(List<Vector3d> convex_hull, Vector3d Point1, Vector3d Point2, Vector3d Point3)
        {
            convex_hull.Add(Point1); convex_hull.Add(Point2); convex_hull.Add(Point3);
        }
    }
}
