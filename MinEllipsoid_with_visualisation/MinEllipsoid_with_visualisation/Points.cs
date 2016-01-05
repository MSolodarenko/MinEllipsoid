using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace MinEllipsoid_with_visualisation
{
    class Points
    {
        public Int32 num_of_points;      //Number of points
        public Vector3d[] points;
        public void ReadFromFile()
        {
            String line;
            string[] unline;

            StreamReader sr = new StreamReader(@"C:\\Users\Solom_000\Documents\GitHub\MinEllipsoid\MinEllipsoid_with_visualisation\MinEllipsoid_with_visualisation\input.txt");

            line = sr.ReadLine();
            num_of_points = Convert.ToInt32(line);

            points = new Vector3d[num_of_points];

            for (int i = 0; i < num_of_points; ++i)
            {
                line = sr.ReadLine();
                unline = line.Split(' ');
                points[i] = new Vector3d(Convert.ToDouble(unline[0]), Convert.ToDouble(unline[1]), Convert.ToDouble(unline[2]));
            }

            sr.Close();
        }
        public Points(Vector3d[] p)
        {
            points = p;
            num_of_points = p.Length;
        }
        public Points(Points t)
        {
            num_of_points = t.num_of_points;
            t.points.CopyTo(points, 0);
        }
        public Points(List<Vector3d> t)
        {
            num_of_points = t.Count;
            t.CopyTo(points);
        }
        public Points() { }
    }
}
