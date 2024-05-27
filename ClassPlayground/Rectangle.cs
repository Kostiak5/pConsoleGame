using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassPlayground
{
    internal class Rectangle
    {

        public double width, height;

        public Rectangle(int width, int height) //kdyz definuju jenom tyhle tri hodnoty
        {
            this.width = width;
            this.height = height;
        }

        public double CalculateArea()
        {
            return width * height;
        }
        public double CalculateAspectRatio()
        {
            return height / width;
        }

        public bool ContainsPoint(int x, int y, double width, double height)
        {
            if (x >= 0 && x <= width && y >= 0 && y <= height) { return true; }
            else { return false; }
        }
    }
}
