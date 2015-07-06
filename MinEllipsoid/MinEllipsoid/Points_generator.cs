using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MinEllipsoid
{
    class Points_generator
    {
        private int num_of_points;      //how much points do we need to generator
        public Points_generator(int num) { num_of_points = num; }
        public void runGenerator(int N)
        {
            double x;
            StreamWriter sw = new StreamWriter(@"input"+N.ToString()+".txt", false);
            sw.WriteLine(num_of_points);
            Random rand = new Random();
            for (int i = 0; i < num_of_points; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    x = ((((double)rand.Next() % 2000)-1000)/1000);
                    sw.Write(x); sw.Write(" ");
                }
                sw.WriteLine();
            }
            sw.Close();
        }
    }
};