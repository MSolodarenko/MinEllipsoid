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
        public double[,] SM;
        public Ellipsoid Vivien_Ellipsoid(List<Vector3d> planes_hull, List<Vector3d> points_hull, bool enable_log)
        {
            var now = DateTime.Now;

            convex_hull = transfigure_list_of_3points_to_faces(planes_hull);
            if (enable_log)  Console.WriteLine("transfigure_list_of_3points_to_faces  - "+(DateTime.Now-now));
            edges = transfigure_list_of_3points_to_edges(planes_hull);
            if (enable_log) Console.WriteLine("transfigure_list_of_3points_to_edges  - "+(DateTime.Now-now));
            points = points_hull;
            candidates = new List<Paral_planes>();
            Cuboid parker = build_min_parallelepiped();
            if (enable_log) Console.WriteLine("build_min_parallelepiped  - " + (DateTime.Now - now));
            points = Program.Translate(points, parker.A);
            parker = Program.Translate(parker, parker.A);

            double vol = parker.volume();
            if (enable_log) Console.WriteLine("counting volume of cuboid  - " + (DateTime.Now - now));
            
            parker = transfigure_paral_to_cube_1x1x1(parker);
            if (enable_log) Console.WriteLine("transfigure_paral_to_cube_1x1x1  - " + (DateTime.Now - now));
            double radius = find_radius_of_sphere();
            if (enable_log) Console.WriteLine("find_radius_of_sphere  - " + (DateTime.Now - now));
            parker = transfigure_cube_to_paral(parker);
            if (enable_log) Console.WriteLine("transfigure_cube_to_paral  - " + (DateTime.Now - now));
            Ellipsoid result = build_ellipsoid_with_(radius);
            if (enable_log) Console.WriteLine("build_ellipsoid_with_(radius)  - " + (DateTime.Now - now));
            result.volume_of_paral = vol;
            return result;
        }
        public Cuboid build_min_parallelepiped()
        {
            set_candidates_from_faces();
            set_candidates_from_edges();

            Cuboid peter = new Cuboid();

            double vol_min = 10000000;
            int f1 = 0, f2 = 0, f3 = 0;
            for (int i = 0; i < candidates.Count(); ++i)
                for (int j = i; j < candidates.Count(); ++j)
                        for (int k = j; k < candidates.Count(); ++k)
                                {
                                    double t = triple_product_abs(candidates[i].N, candidates[j].N, candidates[k].N);
                                    if (t > 0.000001)
                                    {
                                        double vol = Math.Abs(
                                               (candidates[i].distance * candidates[j].distance * candidates[k].distance) 
                                                / (candidates[i].N.X * (candidates[j].N.Y * candidates[k].N.Z - candidates[j].N.Z * candidates[k].N.Y) + candidates[i].N.Y * (candidates[j].N.Z * candidates[k].N.X - candidates[j].N.X * candidates[k].N.Z) + candidates[i].N.Z * (candidates[j].N.X * candidates[k].N.Y - candidates[j].N.Y * candidates[k].N.X))
                                                );
                                        if (vol < vol_min)
                                        {
                                            vol_min = vol;
                                            f1 = i;
                                            f2 = j;
                                            f3 = k;
                                        }
                                    }
                                }

            peter = new Cuboid(candidates[f1], candidates[f2], candidates[f3]);
            return peter;
        }
        //public Ellipsoid build_ellipsoid_with_(double r)
        //{
        //    double[,] m = (double[,])SM.Clone();
        //    double[,] a = new double[4, 4];

        //    a[0, 0] = m[0, 0] * m[0, 0] + m[1, 0] * m[1, 0] + m[2, 0] * m[2, 0];
        //    a[1, 1] = m[0, 1] * m[0, 1] + m[1, 1] * m[1, 1] + m[2, 1] * m[2, 1];
        //    a[2, 2] = m[0, 2] * m[0, 2] + m[1, 2] * m[1, 2] + m[2, 2] * m[2, 2];

        //    a[1, 0] = a[0, 1] = m[0, 0] * m[0, 1] + m[1, 0] * m[1, 1] + m[2, 0] * m[2, 1];
        //    a[2, 0] = a[0, 2] = m[0, 1] * m[0, 2] + m[1, 1] * m[1, 2] + m[2, 1] * m[2, 2];
        //    a[2, 1] = a[1, 2] = m[0, 0] * m[0, 2] + m[1, 0] * m[1, 2] + m[2, 0] * m[2, 2];

        //    a[0, 3] = a[3, 0] = -0.5 * (m[0, 0] + m[1, 0] + m[2, 0]);
        //    a[1, 3] = a[3, 1] = -0.5 * (m[0, 1] + m[1, 1] + m[2, 1]);
        //    a[2, 3] = a[3, 2] = -0.5 * (m[0, 2] + m[1, 2] + m[2, 2]);

        //    a[3, 3] = 0.75 - (r * r);
        //    //a[3, 3] = - (r * r);

        //    //double[] one = new double[] { 1, 1, 1, 1 };

        //    //double[] result = Gauss.GaussSolve(a, one);
        //    //double[] result = Gauss.Diagonal(a);

        //    //double temp = Math.Sqrt(-result[0]/result[3]);
        //    //Ellipsoid resul = new Ellipsoid(Math.Sqrt(Math.Abs(-result[0] / result[3])), Math.Sqrt(Math.Abs(-result[1] / result[3])), Math.Sqrt(Math.Abs(-result[2] / result[3])));
        //    //Ellipsoid resul = new Ellipsoid(Math.Sqrt(result[3] / result[0]), Math.Sqrt(result[3] / result[1]), Math.Sqrt(result[3] / result[2]));

        //    Ellipsoid resul = quadric_to_Ellipsoid(a);
        //    return resul;
        //}
        public Ellipsoid build_ellipsoid_with_(double r)
        {
            double[,] m = new double[4, 4];
            for (int i = 0; i < SM.GetLength(0); ++i)
                for (int j = 0; j < SM.GetLength(1); ++j)
                    m[i, j] = SM[i, j];
            m[0, 3] = m[3, 0] = m[1, 3] = m[3, 1] = m[2, 3] = m[3, 2] = 0;
            m[3, 3] = 1;
            double[,] a = new double[4, 4];

            a[0, 0] = a[1, 1] = a[2, 2] = 1 / (r * r - 0.75);
            //a[0, 0] = a[1, 1] = a[2, 2] = 1 / (r * r);
            a[1, 0] = a[0, 1] = a[2, 0] = a[0, 2] = a[2, 1] = a[1, 2] = 0;

            a[0, 3] = a[3, 0] = a[1, 3] = a[3, 1] = a[2, 3] = a[3, 2] = -1 / (2 * (r * r - 0.75));
            //a[0, 3] = a[3, 0] = a[1, 3] = a[3, 1] = a[2, 3] = a[3, 2] = 0;
            a[3, 3] = -1;

            double[,] mt = transpon(m);

            a = Gauss.mult(mt, a);
            a = Gauss.mult(a, m);

            //double[] one = new double[] { 1, 1, 1, 1 };

            //double[] result = Gauss.GaussSolve(a, one);
            //double[] result = Gauss.Diagonal(a);

            //Ellipsoid resul = new Ellipsoid(Math.Sqrt(Math.Abs(-result[0] / result[3])), Math.Sqrt(Math.Abs(-result[1] / result[3])), Math.Sqrt(Math.Abs(-result[2] / result[3])));
            //Ellipsoid resul = new Ellipsoid(Math.Sqrt(result[3] / result[0]), Math.Sqrt(result[3] / result[1]), Math.Sqrt(result[3] / result[2]));
            Ellipsoid resul = quadric_to_Ellipsoid(a);

            return resul;
        }
        public Ellipsoid quadric_to_Ellipsoid(double[,] quadr)
        {
            double[,] A = new double[3, 3];
            A[0, 0] = quadr[0, 0]; A[1, 1] = quadr[1, 1]; A[2, 2] = quadr[2, 2];
            A[1, 2] = A[2, 1] = (quadr[1, 2] + quadr[2, 1]) / 2;
            A[0, 1] = A[1, 0] = (quadr[1, 0] + quadr[0, 1]) / 2;
            A[0, 2] = A[2, 0] = (quadr[0, 2] + quadr[2, 0]) / 2;

            double[] a = new double[3];
            a[0] = (quadr[0, 3] + quadr[3, 0]) / 2; a[1] = (quadr[1, 3] + quadr[3, 1]) / 2; a[2] = (quadr[2, 3] + quadr[3, 2]) / 2;

            double a0 = quadr[3, 3];
            double[,] S = Gauss.ElemMatr(3);
            double[] lambda = new double[] { A[0, 0], A[1, 1], A[2, 2] };

            //if (is_not_diagonal(A))
            //{
            //    double[] coef_to_charakter = new double[4];
            //    coef_to_charakter[0] = -1;
            //    coef_to_charakter[1] = A[2, 2] + A[1, 1] + A[0, 0];
            //    coef_to_charakter[2] = A[2, 1] * A[2, 1] + A[0, 1] * A[0, 1] + A[0, 2] * A[0, 2] - A[1, 1] * A[2, 2] - A[0, 0] * (A[1, 1] + A[2, 2]);
            //    coef_to_charakter[3] = A[0, 0] * (A[1, 1] * A[2, 2] - A[2, 1] * A[2, 1]) - A[0, 1] * A[0, 1] * A[2, 2] + 2 * A[0, 1] * A[0, 2] * A[2, 1] - A[1, 1] * A[0, 2] * A[0, 2];
            //    lambda = solve_cubic_equation(coef_to_charakter);

            //    if (!(lambda[0] == lambda[1] && lambda[1] == lambda[2] && lambda[0] == lambda[2]))
            //    {
            //        if (!(lambda[0] == lambda[1] || lambda[1] == lambda[2] || lambda[0] == lambda[2]))
            //        {
            //            for (int i = 0; i < 3; ++i)
            //            {
            //                double[,] matrix = (double[,])A.Clone();
            //                for (int j = 0; j < 3; ++j)
            //                    matrix[j, j] -= lambda[i];
            //                double[] nil = new double[] { 0, 0, 1 };

            //                //double[,] la = Gauss.ForwGauss(matrix);
            //                //double[] l = Gauss.GaussSolve(la, nil);

            //                //double[] l = Gauss.GaussSolve(matrix, nil);

            //                double[] mat1 = new double[] { matrix[0, 0], matrix[0, 1], matrix[0, 2], matrix[1, 0], matrix[1, 1], matrix[1, 2], matrix[2, 0], matrix[2, 1], matrix[2, 2] };
                            

            //                double[,] l1 = new double[3, 1]; l1[0, 0] = l[0]; l1[1, 0] = l[1]; l1[2, 0] = l[2];

            //                double[,] temp = Gauss.mult(matrix, l1);
            //                double temp0 = temp[0, 0] / l1[0, 0];
            //                double temp1 = temp[1, 0] / l1[1, 0];
            //                double temp2 = temp[2, 0] / l1[2, 0];

            //                for (int j = 0; j < 3; ++j)
            //                {
            //                    S[j, i] = l[j] / Math.Sqrt(l[0] * l[0] + l[1] * l[1] + l[2] * l[2]);
            //                }
            //            }
            //        }
            //        else return null;
            //    }
            //}
            //double[] mat1 = new double[] { A[0, 0], A[0, 1], A[0, 2], A[1, 0], A[1, 1], A[1, 2], A[2, 0], A[2, 1], A[2, 2] };
            double[] lambdai = new double[3];
            double[,] sl = new double[3, 3];
            alglib.rmatrixevd(A, 3, 1, out lambda,out lambdai,out sl,out S);

            Vector3d a_v = new Vector3d(a[0], a[1], a[2]);
            
            //double[,] St = transpon(S);
            //a_v = transform_point_from_SM(St,a_v);
            a_v = transform_point_from_SM(S, a_v);

            a[0] = a_v.X; a[1] = a_v.Y; a[2] = a_v.Z;

            double t = 1 + a[0] * a[0] / lambda[0] + a[1] * a[1] / lambda[1] + a[2] * a[2] / lambda[2];
            Ellipsoid result = new Ellipsoid(Math.Sqrt(t / lambda[0]), Math.Sqrt(t / lambda[1]), Math.Sqrt(t / lambda[2]));
            //Ellipsoid result = new Ellipsoid(Math.Sqrt(Math.Abs(t / lambda[0])), Math.Sqrt(Math.Abs(t / lambda[1])), Math.Sqrt(Math.Abs(t / lambda[2])));

            return result;
        }
        public double[] solve_cubic_equation(double[] coef)
        {
            double[] x = new double[3];
            double con = coef[0];
            for (int i = 0; i < 4; ++i)
                coef[i] = coef[i] / con;
            double Q = (coef[1] * coef[1] - 3 * coef[2]) / 9;
            double R = (2 * coef[1] * coef[1] * coef[1] - 9 * coef[1] * coef[2] + 27 * coef[3]) / 54;
            if (R * R < Q * Q * Q)
            {
                double t = Math.Acos(R / Math.Sqrt(Q * Q * Q)) / 3;
                x[0] = -2 * Math.Sqrt(Q) * Math.Cos(t) - coef[1] / 3;
                x[1] = -2 * Math.Sqrt(Q) * Math.Cos(t + (2 * Math.PI / 3)) - coef[1] / 3;
                x[2] = -2 * Math.Sqrt(Q) * Math.Cos(t - (2 * Math.PI / 3)) - coef[1] / 3;
            }
            else return null;
            for (int i = 0; i < x.Length; i++)
            {
                for (int j = 0; j < x.Length - 1; j++)
                {
                    if (x[j] > x[j + 1])
                    {
                        double z = x[j];
                        x[j] = x[j + 1];
                        x[j + 1] = z;
                    }
                }
            }
            return x;
        }
        public bool is_not_diagonal(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); ++i)
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    if (i != j)
                        if (Math.Abs(matrix[i, j]) > 0.0000001)
                            return true;
            return false;
        }
        public double[,] transpon(double[,] m)
        {
            double[,] m1 = (double[,])m.Clone();
            int n = m.GetLength(0);
            double t = 0;
            for (int i = 0; i < n; ++i)
                for (int j = i; j < n; ++j)
                {
                    t = m[j, i];
                    m1[j, i] = m[i, j];
                    m1[i, j] = t;
                }
            return m1;
        }
        public Cuboid transfigure_cube_to_paral(Cuboid parker)
        {
            Vector3d test = transform_point_from_SM(SM, parker.B);
            parker = transform_cuboid_from_SM(SM, parker);
            points = transform_list_vector3d_from_SM(SM, points);
            return parker;
        }
        public double find_radius_of_sphere()
        {
            double rad = 0;
            points = Program.Translate(points, 0.5, 0.5, 0.5);
            rad = find_farthest_point_from_0x0x0();
            points = Program.Translate(points, -0.5, -0.5, -0.5);
            return rad;
        }
        public double find_farthest_point_from_0x0x0()
        {
            //Vector3d result = new Vector3d();
            double maxDistance = 0;
            for (int i = 0; i < points.Count; ++i)
            {
                //Console.WriteLine("I'm searching max distance between two EPs");
                if (maxDistance < Module(points[i]))
                {
                    maxDistance = Module(points[i]);
                    //result = points[i];
                }
            }
            return maxDistance;
        }
        public double Module(Vector3d a)
        {
            Vector3d nul = new Vector3d(0, 0, 0);
            return Points_distance(nul, a);
        }
        public double Points_distance(Vector3d a, Vector3d b)
        {
            //Console.WriteLine("I'm counting distance between two points:" + a.ToString() + "and" + b.ToString());
            double result;
            result = Math.Sqrt(Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2) + Math.Pow((b.Z - a.Z), 2));
            return result;
        }
        public Cuboid transfigure_paral_to_cube_1x1x1(Cuboid parker)
        {
            double[,] m = new double[3, 3];
            m[0, 0] = parker.B.X; m[0,1] = parker.D.X; m[0,2] = parker.A1.X;
            m[1,0] = parker.B.Y; m[1,1] = parker.D.Y; m[1,2] = parker.A1.Y;
            m[2,0] = parker.B.Z; m[2,1] = parker.D.Z; m[2,2] = parker.A1.Z;
            
            SM = Inverse(m);
            //double[,] t = Gauss.mult(m, SM);

            Vector3d test = transform_point_with_SM(SM, parker.B);
            parker = transform_cuboid_with_SM(SM, parker);
            points = transform_list_vector3d_with_SM(SM, points);
            //double[,] t = Inverse(SM);

            return parker;
        }
        public Cuboid transform_cuboid_with_SM(double[,] s, Cuboid parker)
        {
            Cuboid t = new Cuboid(transform_list_vector3d_with_SM(s, parker.toList_Vector3d()));
            return t;
        }
        public List<Vector3d> transform_list_vector3d_with_SM(double[,] s, List<Vector3d> p)
        {
            List<Vector3d> temp = new List<Vector3d>();
            for (int i = 0; i < p.Count(); ++i)
                temp.Add(transform_point_with_SM(s, p[i]));
            return temp;
        }
        public Vector3d transform_point_with_SM(double[,] s, Vector3d p)
        {
            Vector3d t = new Vector3d();
            t.X = p.X * s[0, 0] + p.Y * s[0, 1] + p.Z * s[0, 2];
            t.Y = p.X * s[1, 0] + p.Y * s[1, 1] + p.Z * s[1, 2];
            t.Z = p.X * s[2, 0] + p.Y * s[2, 1] + p.Z * s[2, 2];
            return t;
        }
        public Cuboid transform_cuboid_from_SM(double[,] s, Cuboid parker)
        {
            Cuboid t = new Cuboid(transform_list_vector3d_from_SM(s, parker.toList_Vector3d()));
            return t;
        }
        public List<Vector3d> transform_list_vector3d_from_SM(double[,] s, List<Vector3d> p)
        {
            List<Vector3d> temp = new List<Vector3d>();
            for (int i = 0; i < p.Count(); ++i)
                temp.Add(transform_point_from_SM(s, p[i]));
            return temp;
        }
        public Vector3d transform_point_from_SM(double[,] s, Vector3d p)
        {
            double[] point = new double[] { p.X, p.Y, p.Z };
            double[] res = Gauss.GaussSolve(s, point);
            Vector3d result = new Vector3d(res[0], res[1], res[2]);
            return result;
        }
        public double[,] Inverse(double[,] m)
        {
            double[,] result = new double[3, 3];
            result[0, 0] = m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1];
            result[0, 1] = m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0];
            result[0, 2] = m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0];

            result[1, 0] = m[0, 1] * m[2, 2] - m[0, 2] * m[2, 1];
            result[1, 1] = m[0, 0] * m[2, 2] - m[0, 2] * m[2, 0];
            result[1, 2] = m[0, 0] * m[2, 1] - m[0, 1] * m[2, 0];

            result[2, 0] = m[0, 1] * m[1, 2] - m[0, 2] * m[1, 1];
            result[2, 1] = m[0, 0] * m[1, 2] - m[0, 2] * m[1, 0];
            result[2, 2] = m[0, 0] * m[1, 1] - m[0, 1] * m[1, 0];

            result[0, 1] = -1 * result[0, 1];
            result[1, 0] = -1 * result[1, 0];
            result[1, 2] = -1 * result[1, 2];
            result[2, 1] = -1 * result[2, 1];

            double t = result[0, 1];
            result[0, 1] = result[1, 0];
            result[1, 0] = t;
            t = result[0, 2];
            result[0, 2] = result[2, 0];
            result[2, 0] = t;
            t = result[2, 1];
            result[2, 1] = result[1, 2];
            result[1, 2] = t;

            t = 1 / determinant(m);
            for (int i = 0; i < 3; ++i)
                for (int j = 0; j < 3; ++j)
                    result[i, j] *= t;

            return result;
        }
        public double double_norm_vector(Vector3d a)
        {
            double result = a.X * a.X + a.Y * a.Y + a.Z * a.Z;
            return result;
        }
        public double determinant(double[,] t)
        {
            return t[0, 0] * (t[1, 1] * t[2, 2] - t[1, 2] * t[2, 1]) - t[0, 1] * (t[1, 0] * t[2, 2] - t[1, 2] * t[2, 0]) + t[0, 2] * (t[1, 0] * t[2, 1] - t[1, 1] * t[2, 0]);
        }
        public double triple_product_abs(Vector3d a, Vector3d b, Vector3d c)
        {
            return Math.Abs(a.X * (b.Y * c.Z - b.Z * c.Y) + a.Y * (b.Z * c.X - b.X * c.Z) + a.Z * (b.X * c.Y - b.Y * c.X)); 
        }
        public void set_candidates_from_edges()
        {
            for (int i = 0; i < edges.Count(); ++i)
                for (int j = 0; j < edges.Count(); ++j)
                    if (i != j)
                    {
                        if (!edges[i].is_on_one_plane_with(edges[j]))
                        {
                            Paral_planes malt = new Paral_planes(edges[i], edges[j]);
                            if (paral_planes_contains_points(malt))
                                if (not_in_candidates(malt)) 
                                    candidates.Add(malt);
                        }
                    }
        }
        public bool paral_planes_contains_points(Paral_planes t)
        {
            double A_B = plain_plain_distance(t.A, t.B);
            for (int i = 0; i < points.Count(); ++i)
            {
                //Vector3d p = points[i];
                double p_A = Point_plain_distance(points[i], t.A);
                double p_B = Point_plain_distance(points[i], t.B);
                //if (p_A > A_B)
                if ((p_A - A_B) > 0.0000001)
                    return false;
                //if (p_B > A_B)
                if ((p_B - A_B) > 0.0000001)
                    return false;
            }
            return true;
        }
        public void set_candidates_from_faces()
        {
            for (int i = 0; i < convex_hull.Count(); ++i)
            {
                Vector3d sirius = find_most_distant_point_from_plain(convex_hull[i]);
                Paral_planes james = new Paral_planes(convex_hull[i], sirius);
                if (paral_planes_contains_points(james))
                    if (not_in_candidates(james))
                        candidates.Add(james);
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
