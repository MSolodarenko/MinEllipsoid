using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MinEllipsoid_with_visualisation
{
    class Points_generator
    {
        private int num_of_points;      //how much points do we need to generator
        public Points_generator(int num) { num_of_points = num; }
        public void runGenerator()
        {
            double x;
            StreamWriter sw = new StreamWriter(@"C:\\Users\Solom_000\Documents\GitHub\MinEllipsoid\MinEllipsoid_with_visualisation\MinEllipsoid_with_visualisation\input.txt", false);
            sw.WriteLine(num_of_points);
            Random rand = new Random();
            for (int i = 0; i < num_of_points; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    x = ((((double)rand.Next() % 200)-100)/100);
                    sw.Write(x); sw.Write(" ");
                }
                sw.WriteLine();
            }
            sw.Close();
        }
    }
};