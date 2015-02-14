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
            generate_variables();
            double x;
            StreamWriter sw = new StreamWriter(@"C:\\Users\Solom_000\Documents\GitHub\MinEllipsoid\MinEllipsoid_with_visualisation\MinEllipsoid_with_visualisation\input.txt", false);
            sw.WriteLine(num_of_points);
            for (int i=0; i<num_of_points; ++i)
            {
                for (int j=0; j<3; ++j)
                {
                    x = generateNumber();
                    sw.Write(x); sw.Write(" ");
                }
                sw.WriteLine();
            }
            sw.Close();
        }
        double generateNumber()         //method with generates double number between -1 to 1
        {
            if (!if_generated) generate_variables();
            double x1 = (a + x0 + c) % m;
            x0 = (long)x1;
            x1 = ((x1 % 200) - 100) / 100;
            return x1;
        }
        private long m, a, c, x0;
        private bool if_generated = false;
        void generate_variables()       //method generates variables for random generator
        {
            Random rand = new Random();
            m = rand.Next();
            a = rand.Next() % m + 1;
            c = rand.Next() % m + 1;
            x0 = rand.Next() % m + 1;
            if_generated = true;
        }
    }
};