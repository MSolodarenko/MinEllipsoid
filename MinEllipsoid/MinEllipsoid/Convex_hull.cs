using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Collections;
using MIConvexHull;

namespace MinEllipsoid
{
    class Convex_hull
    {
        Points p;
        public Convex_hull(Points temp)
        {
            p = temp;
        }
        public List<Vector3d> Create_convex_hull()
        {
            double[][] vertices = new double[p.num_of_points][];

            for (int i = 0; i < p.num_of_points; ++i)
            {
                vertices[i] = new double[3];
                vertices[i][0] = p.points[i].X;
                vertices[i][1] = p.points[i].Y;
                vertices[i][2] = p.points[i].Z;
            }
            var convexHull = ConvexHull.Create(vertices);
            var convexHullVertices = convexHull.Points.ToList();
            var convexHullFaces = convexHull.Faces.ToList();
            List<DefaultVertex> point_list = new List<DefaultVertex>();
            for (int i = 0; i < convexHullFaces.Count; ++i)
            {
                point_list.Add(convexHullFaces[i].Vertices[0]);
                point_list.Add(convexHullFaces[i].Vertices[1]);
                point_list.Add(convexHullFaces[i].Vertices[2]);
            }
            Vector3d[] res = new Vector3d[point_list.Count];
            for (int i = 0; i < point_list.Count; ++i)
            {
                res[i] = Vertcie_to_Vector3d(point_list[i]);
            }
            return res.ToList();
        }
        public Vector3d Vertcie_to_Vector3d(DefaultVertex t)
        {
            return (new Vector3d(t.Position[0], t.Position[1], t.Position[2]));
        }
    }

}

