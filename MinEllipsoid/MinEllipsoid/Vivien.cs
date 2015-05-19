using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MinEllipsoid
{
    class Vivien
    {
        public List<Vector3d> points;
        public List<Face> convex_hull;
        public List<Edge> edges;
        public List<Paral_planes> candidates;
        public Ellipsoid Vivien_Ellipsoid(List<Vector3d> planes_hull, List<Vector3d> points_hull)
        {
            convex_hull = transfigure_list_of_3points_to_faces(planes_hull);
            edges = transfigure_list_of_3points_to_edges(planes_hull);
            points = points_hull;
            candidates = new List<Paral_planes>();
            Cuboid parker = build_min_parallelepiped();

            return null;
        }
        public Cuboid build_min_parallelepiped()
        {
            set_candidates_from_faces();
            set_candidates_from_edges();

            double vol_min = 10000000;
            int f1=0, f2=0, f3=0;
            for (int i = 0; i < candidates.Count(); ++i)
                for (int j = 0; j < candidates.Count(); ++j)
                    for (int k = 0; k < candidates.Count(); ++k )
                        if (i!=j || i!=k || j!=k)
                            if (triple_product_abs(candidates[i].N,candidates[j].N,candidates[k].N) > 0.000001)
                            {
                                double vol = Math.Abs( Math.Pow(norm_vector(candidates[i].N)*norm_vector(candidates[j].N)*norm_vector(candidates[k].N),2) / triple_product_abs(candidates[i].N,candidates[j].N,candidates[k].N));
                                if (vol < vol_min)
                                {
                                    vol_min = vol;
                                    f1 = i;
                                    f2 = j;
                                    f3 = k;
                                }
                            }
            Cuboid peter = new Cuboid(candidates[f1], candidates[f2], candidates[f3]);
            return peter;
        }
        public double norm_vector(Vector3d a)
        {
            double result = Math.Sqrt(a.X * a.X + a.Y * a.Y + a.Z * a.Z);
            return result;
        }
        public double triple_product_abs(Vector3d a, Vector3d b, Vector3d c)
        {
            double result = Math.Abs(a.X * (b.Y * c.Z - b.Z * c.Y) + a.Y * (b.Z * c.X - b.X * c.Z) + a.Z * (b.X * c.Y - b.Y * c.X)); 
            return result;
        }
        public void set_candidates_from_edges()
        {
            for (int i = 0; i < edges.Count(); ++i)
                for (int j = 0; j < edges.Count(); ++j)
                    if (i != j)
                    {
                        if (!edges[i].is_on_one_plane_with(edges[j]))
                        {
                            Face f1 = new Face(edges[i], edges[j]);
                            Face f2 = new Face(f1, edges[j].A);
                            Vector3d normal = new Vector3d(f1.A, f1.B, f1.C);
                            Paral_planes malt = new Paral_planes(f1,f2,normal);
                            if (paral_planes_contains_points(malt))
                                if (not_in_candidates(malt)) 
                                    candidates.Add(malt);
                        }
                    }
        }
        public bool paral_planes_contains_points(Paral_planes t)
        {
            for (int i = 0; i < points.Count(); ++i)
            {
                if (Point_plain_distance(points[i], t.A) > plain_plain_distance(t.A, t.B))
                    return false;
                if (Point_plain_distance(points[i], t.B) > plain_plain_distance(t.A, t.B))
                    return false;
            }
            return true;
        }
        public void set_candidates_from_faces()
        {
            for (int i = 0; i < convex_hull.Count(); ++i)
            {
                Vector3d sirius = find_most_distant_point_from_plain(convex_hull[i]);
                Vector3d normal = new Vector3d(convex_hull[i].A, convex_hull[i].B, convex_hull[i].C);
                Face lupin = new Face(normal, sirius);
                Paral_planes james = new Paral_planes(convex_hull[i], lupin, normal);
                if (not_in_candidates(james))
                {
                    candidates.Add(james);
                }
            }
        }
        public bool not_in_candidates(Paral_planes lily)
        {
            for (int i = 0; i < candidates.Count(); ++i )
            {
                if (candidates[i].equals(lily))
                    return false;
            }
            return true;
        }
        public Vector3d find_most_distant_point_from_plain(Face marty)
        {
            double maxDistance = 0;
            Vector3d doc = new Vector3d();
            for (int i = 0; i < points.Count(); ++i)
            {
                if (maxDistance < Point_plain_distance(points[i], marty))
                {
                    maxDistance = Point_plain_distance(points[i], marty);
                    doc = points[i];
                }
            }
            return doc;
        }
        public double Point_plain_distance(Vector3d Point, Face Trevor)
        {
            double plain_mod = Math.Sqrt(Trevor.A * Trevor.A + Trevor.B * Trevor.B + Trevor.C * Trevor.C);
            double plain_point_mod = Math.Abs(Trevor.A * Point.X + Trevor.B * Point.Y + Trevor.C * Point.Z + Trevor.D);

            if (plain_mod != 0)
                return plain_point_mod / plain_mod;
            return 0;
        }
        public double plain_plain_distance(Face Trevor, Face Michael)
        {
            double plain_mod = Math.Sqrt(Trevor.A * Trevor.A + Trevor.B * Trevor.B + Trevor.C * Trevor.C);
            double plain_point_mod = Math.Abs(Trevor.D-Michael.D);

            if (plain_mod != 0)
                return plain_point_mod / plain_mod;
            return 0;
        }
        public List<Edge> transfigure_list_of_3points_to_edges(List<Vector3d> max)
        {
            List<Edge> michael = new List<Edge>();
            for (int i = 0; i < max.Count(); i = i + 3)
            {
                Edge mel1 = new Edge(max[i], max[i + 1]);
                Edge mel2 = new Edge(max[i + 1], max[i + 2]);
                Edge mel3 = new Edge(max[i + 2], max[i]);
                //Face lucius = new Face(draco[i], draco[i + 1], draco[i + 2]);
                //malfoy.Add(lucius);
                if (not_in_edge(mel1,michael))
                    michael.Add(mel1);
                if (not_in_edge(mel2, michael))
                    michael.Add(mel2);
                if (not_in_edge(mel3, michael))
                    michael.Add(mel3);
            }
            return michael;
        }
        public List<Face> transfigure_list_of_3points_to_faces(List<Vector3d> draco)
        {
            List<Face> malfoy = new List<Face>();
            for (int i = 0; i < draco.Count(); i = i + 3)
            {
                Face lucius = new Face(draco[i], draco[i + 1], draco[i + 2]);
                if (not_in_face(lucius,malfoy)) 
                    malfoy.Add(lucius);
            }
            return malfoy;
        }
        public bool not_in_face(Face lily, List<Face> face)
        {
            for (int i = 0; i < face.Count(); ++i)
            {
                if (face[i].equals(lily))
                    return false;
            }
            return true;
        }
        public bool not_in_edge(Edge lily, List<Edge> edge)
        {
            for (int i = 0; i < edge.Count(); ++i)
            {
                if (edge[i].equals(lily))
                    return false;
            }
            return true;
        }
    }
}
