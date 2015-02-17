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
        public void Create_convex_hull(Points p)
        {
            int min = 0;
            for (int i=0; i<p.num_of_points; ++i)
            {
                if (p.points[i].X < p.points[min].X || p.points[i].Y < p.points[min].Y || p.points[i].Z < p.points[min].Z)
                    min = i;
            }

            double[,] distance = new double[p.num_of_points,p.num_of_points];
            for (int i = 0; i < p.num_of_points; ++i)
                for (int j = 0; j < p.num_of_points; ++j)
                    distance[i, j] = Count_distance(p.points[i], p.points[j]);

        }
        public double Count_distance(Vector3d a, Vector3d b)
        {
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
            return result;
        }
    }
}
