﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;

namespace MinEllipsoid
{
    class Points
    {
        public Int32 num_of_points;      //Number of points
        public Vector3d[] points;
        public void ReadFromFile(int N)
        {
            String line;
            string[] unline;

            StreamReader sr = new StreamReader(@"input" + N.ToString() + ".txt");

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
    }
}
