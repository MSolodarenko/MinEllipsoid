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
        static int Main(string[] args)
        {
            int N = 3;
            int n = 10;

            for (int i = 1; i <= N; ++i)
            {
                Points p = generate_points(i, n);
                List<Vector3d> planes_hull = create_convex_hulls_plane_list(p);
                List<Vector3d> points_hull = create_convex_hulls_point_list(planes_hull);

            }
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
    }
}
