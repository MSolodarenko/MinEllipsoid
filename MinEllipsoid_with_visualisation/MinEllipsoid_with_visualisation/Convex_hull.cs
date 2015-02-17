using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MinEllipsoid_with_visualisation
{
    class Convex_hull
    {
        Points p;
        public Convex_hull(Points temp)
        {
            p = temp;
        }
        public void Create_convex_hull()
        {
            int[] ep = new int[6];                      //Extreme points: 0 - minX, 1 - minY, 2 - minZ, 3 - maxX, 4 - maxY, 5 - maxZ
            for (int i = 0; i < 6; ++i)
                ep[i] = 0;

            for (int i = 0; i < p.num_of_points; ++i)
            {
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

            double maxDistance = 0;
            int a = 0, b = 0;                       // The most distant points of EPs
            for (int i = 0; i < 6; ++i)
                for (int j = 0; j < 6; ++j)
                {
                    if (maxDistance < Points_distance(p.points[ep[i]], p.points[ep[j]]))
                    {
                        maxDistance = Points_distance(p.points[ep[i]], p.points[ep[j]]);
                        a = ep[i];
                        b = ep[j];
                    }
                }

            maxDistance = 0;
            int c = 0;
            for (int i = 0; i<6; ++i)
            {
                if (i != a && i != b)
                    if (maxDistance < Point_line_distance(p.points[ep[i]],p.points[a],p.points[b]))
                    {
                        maxDistance = Point_line_distance(p.points[ep[i]], p.points[a], p.points[b]);
                        c = ep[i];
                    }
            }

            maxDistance = 0;
            int d = 0;
            for (int i = 0; i < 6; ++i)
            {
                if (i != a && i != b && i != c)
                    if (maxDistance < Point_plain_distance(p.points[ep[i]], p.points[a], p.points[b], p.points[c]))
                    {
                        maxDistance = Point_plain_distance(p.points[ep[i]], p.points[a], p.points[b], p.points[c]);
                        d = ep[i];
                    }
            }

        }
        public double Points_distance(Vector3d a, Vector3d b)
        {
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
            return result;
        }
        public double Point_line_distance(Vector3d Point, Vector3d M2, Vector3d M3)
        {
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
    }
}
