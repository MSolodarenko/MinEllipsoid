using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MinEllipsoid
{
    class Ellipsoid
    {
        public double a, b, c;
        public double V;
        public double volume_of_paral;
        public Ellipsoid(double x, double y, double z) { a = x; b = y; c = z; V = Volume(); }
        public Ellipsoid() { }
        public double Volume()
        {
            V = (Math.PI * a * b * c) * 4 / 3;
            return V;
        }
    }
    class Cuboid
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
        public Cuboid(Paral_planes f1, Paral_planes f2, Paral_planes f3)
        {
            Face ABCD = f1.A; Face A1B1C1D1 = f1.B;
            Face ABB1A1 = f2.A; Face DCC1D1 = f2.B;
            Face AA1D1D = f3.A; Face BB1C1C = f3.B;

            A = point_from_3_faces(ABCD, ABB1A1, AA1D1D);
            B = point_from_3_faces(ABCD, ABB1A1, BB1C1C);
            C = point_from_3_faces(ABCD, DCC1D1, BB1C1C);
            D = point_from_3_faces(ABCD, AA1D1D, DCC1D1);
            A1 = point_from_3_faces(A1B1C1D1, ABB1A1, AA1D1D);
            B1 = point_from_3_faces(A1B1C1D1, ABB1A1, BB1C1C);
            C1 = point_from_3_faces(A1B1C1D1, DCC1D1, BB1C1C);
            D1 = point_from_3_faces(A1B1C1D1, AA1D1D, DCC1D1);
        }
        public Cuboid(List<Vector3d> t)
        {
            A = t[0]; B = t[1]; C = t[2]; D = t[3];
            A1 = t[4]; B1 = t[5]; C1 = t[6]; D1 = t[7];
        }
        public Vector3d point_from_3_faces(Face a, Face b, Face c)
        {
            double[,] m = new double[3,4];
            m[0, 0] = a.A; m[0, 1] = a.B; m[0, 2] = a.C; m[0, 3] = -1 * a.D;
            m[1, 0] = b.A; m[1, 1] = b.B; m[1, 2] = b.C; m[1, 3] = -1 * b.D;
            m[2, 0] = c.A; m[2, 1] = c.B; m[2, 2] = c.C; m[2, 3] = -1 * c.D;
            double[] resul = Gauss.GaussSolve(m);
            Vector3d result = new Vector3d(resul[0], resul[1], resul[2]);
            return result;
        }
        public List<Vector3d> toList_Vector3d()
        {
            List<Vector3d> result = new List<Vector3d>();
            result.Add(A); result.Add(B); result.Add(C); result.Add(D);
            result.Add(A1); result.Add(B1); result.Add(C1); result.Add(D1);
            return result;
        }
        public double volume()
        {
            double result;
            Vector3d a, b, c;
            //AB, AD, AA1
            a = new Vector3d(B.X - A.X, B.Y - A.Y, B.Z - A.Z);
            b = new Vector3d(D.X - A.X, D.Y - A.Y, D.Z - A.Z);
            c = new Vector3d(A1.X - A.X, A1.Y - A.Y, A1.Z - A.Z);
            result = Math.Abs(a.X * (b.Y * c.Z - b.Z * c.Y) + a.Y * (b.Z * c.X - b.X * c.Z) + a.Z * (b.X * c.Y - b.Y * c.X));
            return result;
        }
    }
    class Face
    {
        public double A, B, C, D;
        public Face() { }
        public Face(double a, double b, double c, double d) { A = a; B = b; C = c; D = d; }
        public Face(Vector3d M0, Vector3d M1, Vector3d M2)
        {
            A = (M1.Y - M0.Y) * (M2.Z - M0.Z) - (M2.Y - M0.Y) * (M1.Z - M0.Z);
            B = (M1.Z - M0.Z) * (M2.X - M0.X) - (M1.X - M0.X) * (M2.Z - M0.Z);
            C = (M1.X - M0.X) * (M2.Y - M0.Y) - (M2.X - M0.X) * (M1.Y - M0.Y);
            D = M0.X * ((M2.Y - M0.Y) * (M1.Z - M0.Z) - (M1.Y - M0.Y) * (M2.Z - M0.Z)) + M0.Y * ((M1.X - M0.X) * (M2.Z - M0.Z) - (M1.Z - M0.Z) * (M2.X - M0.X)) + M0.Z * ((M2.X - M0.X) * (M1.Y - M0.Y) - (M1.X - M0.X) * (M2.Y - M0.Y));
        }
        public Face(Vector3d normal, Vector3d point)
        {
            A = normal.X; B = normal.Y; C = normal.Z;
            D = -1 * (normal.X * point.X + normal.Y * point.Y + normal.Z * point.Z);
        }
        //public Face(Edge d1, Edge d2)
        //{
        //    Vector3d m1 = d1.A;
        //    Vector3d p1 = new Vector3d(d1.B.X - d1.A.X, d1.B.Y - d1.A.Y, d1.B.Z - d1.A.Z);
        //    Vector3d p2 = new Vector3d(d2.B.X - d2.A.X, d2.B.Y - d2.A.Y, d2.B.Z - d2.A.Z);
        //    A = p1.Y * p2.Z - p2.Y * p1.Z;
        //    B = p1.Z * p2.X - p1.X * p2.Z;
        //    C = p1.X * p2.Y - p2.X * p1.Y;
        //    D = -m1.X * (p1.Y * p2.Z - p2.Y * p1.Z) - m1.Y * (p1.Z * p2.X - p1.X * p2.Z) - m1.Z * (p1.X * p2.Y - p2.X * p1.Y);
        //}
        //public Face(Face d, Vector3d p)
        //{
        //    A = d.A; B = d.B; C = d.C;
        //    D = -A * p.X - B * p.Y - C * p.Z;
        //}
        public double formula(Vector3d t)
        {
            return A * t.X + B * t.Y + C * t.Z + D;
        }
        public bool equals(Face shane)
        {
            if (shane.A != 0)
            {
                double k = this.A / shane.A;
                if (Math.Abs(this.B - k * shane.B) < 0.00000001)
                    if (Math.Abs(this.C - k * shane.C) < 0.00000001)
                        if (Math.Abs(this.D - k * shane.D) < 0.00000001)
                            return true;
            }
            if (this.A != 0)
            {
                double k = shane.A / this.A;
                if (Math.Abs(shane.B - k * this.B) < 0.00000001)
                    if (Math.Abs(shane.C - k * this.C) < 0.00000001)
                        if (Math.Abs(shane.D - k * this.D) < 0.00000001)
                            return true;
            }
            if (shane.B != 0)
            {
                double k = this.B / shane.B;
                    if (Math.Abs(this.C - k * shane.C) < 0.00000001)
                        if (Math.Abs(this.D - k * shane.D) < 0.00000001)
                            return true;
            }
            if (this.B != 0)
            {
                double k = shane.B / this.B;
                    if (Math.Abs(shane.C - k * this.C) < 0.00000001)
                        if (Math.Abs(shane.D - k * this.D) < 0.00000001)
                            return true;
            }
            if (shane.C != 0)
            {
                double k = this.C / shane.C;
                    if (Math.Abs(this.D - k * shane.D) < 0.00000001)
                        return true;
            }
            if (this.C != 0)
            {
                double k = shane.C / this.C;
                    if (Math.Abs(shane.D - k * this.D) < 0.00000001)
                        return true;
            }
            if (Math.Abs(this.D - shane.D) < 0.00000001)
                return true;
            return false;
        }

    }
    class Paral_planes
    {
        public Face A, B;
        public Vector3d N;
        public Paral_planes() { }
        public Paral_planes(Face a, Face b, Vector3d n) { A = a; B = b; N = n; }
        public Paral_planes(Face a, Face b, double nx, double ny, double nz)
        {
            Vector3d dobby = new Vector3d(nx, ny, nz);
            A = a; B = b; N = dobby;
        }
        public Paral_planes(Edge a, Edge b)
        {
            double i = 0, j = 0, k = 0;
            //double m11 = i, m12 = j, m13 = k;
            double m21 = a.B.X - a.A.X, m22 = a.B.Y - a.A.Y, m23 = a.B.Z - a.A.Z;
            double m31 = b.B.X - b.A.X, m32 = b.B.Y - b.A.Y, m33 = b.B.Z - b.A.Z;
            i = m22 * m33 - m23 * m32;
            j = m23 * m31 - m21 * m33;
            k = m21 * m32 - m22 * m31;
            N = new Vector3d(i, j, k);
            A = new Face(N, a.A);
            B = new Face(N, b.A);
        }
        public bool equals(Paral_planes jack)
        {
            if (this.A.equals(jack.A))
                if (this.B.equals(jack.B))
                    return true;
             if (this.A.equals(jack.B))
                if (this.B.equals(jack.A))
                    return true;
            return false;
        }
    }
    class Edge
    {
        public Vector3d A, B;
        public Edge() { }
        public Edge(Vector3d a, Vector3d b) { A = a; B = b; }
        //public bool is_on_one_plane_with(Edge alex)
        //{
        //    double m11 = this.A.X - alex.A.X, m12 = this.A.Y - alex.A.Y, m13 = this.A.Z - alex.A.Z;
        //    double m21 = this.B.X - this.A.X, m22 = this.B.Y - this.A.Y, m23 = this.B.Z - this.A.Z;
        //    double m31 = alex.B.X - alex.A.X, m32 = alex.B.Y - alex.A.Y, m33 = alex.B.Z - alex.A.Z;
        //    double div = m11 * m22 * m33 + m12 * m23 * m31 + m13 * m21 * m32 - m13 * m22 * m31 - m23 * m32 * m11 - m33 * m12 * m21;
        //    if (Math.Abs(div) < 0.00000001)
        //        return true;
        //    return false;
        //}
        public bool is_on_one_plane_with(Edge alex)
        {
            double m11 = this.B.X - this.A.X, m12 = alex.B.X - alex.A.X, m13 = alex.A.X - this.A.X;
            double m21 = this.B.Y - this.A.Y, m22 = alex.B.Y - alex.A.Y, m23 = alex.A.Y - this.A.Y;
            double m31 = this.B.Z - this.A.Z, m32 = alex.B.Z - alex.A.Z, m33 = alex.A.Z - this.A.Z;
            double div = m11 * m22 * m33 + m12 * m23 * m31 + m13 * m21 * m32 - m13 * m22 * m31 - m23 * m32 * m11 - m33 * m12 * m21;
            if (Math.Abs(div) < 0.00000001)
                return true;
            return false;
        }
        public bool equals(Edge mary)
        {
            if (this.A == mary.A)
                if (this.B == mary.B)
                    return true;
            if (this.A == mary.B)
                if (this.B == mary.A)
                    return true;
            return false;
        }
    }
}
