using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinEllipsoid
{
    class Ellipsoid
    {
        public double a, b, c;
        public double V;
        public Ellipsoid(double x, double y, double z) { a = x; b = y; c = z; }
        public Ellipsoid() { }
        public double Volume()
        {
            V = (4 / 3) * Math.PI * a * b * c;
            return V;
        }
    }
}
