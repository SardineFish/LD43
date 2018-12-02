using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MultiPlayer
{
    public static class Utilities
    {
        public static double[] Vector2List(Vector2 v)
        {
            return new double[] { v.x, v.y };
        }

        public static Vector2 List2Vector(double[] v)
        {
            return new Vector2((float)v[0], (float)v[1]);
        }
    }
}
