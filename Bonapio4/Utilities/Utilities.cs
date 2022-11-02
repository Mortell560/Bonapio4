using System;
using System.Drawing;

namespace Bonapio4.Utilities
{
    public class Utilities
    {
        /// <summary>
        /// Generates a random color bc why not
        /// </summary>
        /// <returns></returns>
        public static Color RandomColor()
        {
            Random rnd = new Random();
            Byte[] b = new Byte[3];
            rnd.NextBytes(b);

            Color color = new Color();

            color = Color.FromArgb(b[0], b[1], b[2]);

            return color;
        }

        public static double RandomDouble(float min, float max)
        {
            Random rnd = new Random();
            return rnd.NextDouble() * (max - min) + min;
        }

    }
}
