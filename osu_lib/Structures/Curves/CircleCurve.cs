using System;

namespace Structures.Curves
{
    //Approximates a curve of three points using a circle calculated from those
    //three points.
    //Main algorithm courtesy of "Equation of a Circle from 3 Points
    //(2 dimensions)" from http://paulbourke.net/geometry/circlesphere/
    public class CircleCurve
    {
        double radius;
        Point center;

        double startangle, endangle;

        //Tells which direction the curve goes
        bool clockwise;

        double arclength;

        public CircleCurve(Point p1, Point p2, Point p3, double length)
        {
            arclength = length;

            //Calculate the center and radius
            this.calculateCenterandRadius(p1, p2, p3);
            //Calculate the first and last angles of the curve
            this.calculateAngles(p1, p2, p3, length);
        }

        public Point Center
        {
            get { return center; }
        }

        public double Radius
        {
            get { return radius; }
        }

        //Gets a point on the approximated curve on the circle
        //Goes from 0 to 1, where 0 is the starting point and 1 is the ending point
        public Point GetPoint(double t)
        {
            //TODO: Make exception more useful
            if(t < 0 || t > 1)
                throw new ArgumentOutOfRangeException();

            double angle;
            double anglediff;
            if(clockwise)
            {
                if(endangle - startangle > 0)
                    anglediff = endangle - startangle;
                else
                    anglediff = endangle + ((2 * Math.PI) - startangle);

                angle = Dewlib.RestrictRange(t * anglediff + startangle, 0, 2 * Math.PI);
            }
            else
            {
                if(startangle - endangle > 0)
                    anglediff = startangle - endangle;
                else
                    anglediff = startangle + ((2 * Math.PI) - endangle);

                angle = Dewlib.RestrictRange(-t * anglediff + startangle, 0, 2 * Math.PI);
            }

            Point accessed = new Point();
            accessed.x = center.x + radius * Math.Cos(angle);
            accessed.y = center.y + radius * Math.Sin(angle);

            return accessed;
        }

        //Gets a point on the curve that is an arclength away from the start
        public Point GetPointAlong(double length)
        {
            //Calculate how much along the curve the given length is
            double percent = length / arclength;

            if(percent > 1)
                throw new ArgumentException("Error: given length is beyond the length of the curve\n" +
                                            "length: " +  length + "\n" +
                                            "arclength: " + arclength);

            return GetPoint(percent);
        }

        //Helper method that's only called in the constructor to calculate center
        //and radius
        private void calculateCenterandRadius(Point p1, Point p2, Point p3)
        {
            if(p1.x == p2.x)
            {
                Point temppoint = p2;
                p2 = p3;
                p3 = temppoint;
            }
            else if(p2.x == p3.x)
            {
                Point temppoint = p1;
                p1 = p2;
                p2 = temppoint;
            }

            double ma = (p2.y - p1.y) / (p2.x - p1.x);
            double mb = (p3.y - p2.y) / (p3.x - p2.x);

            center = new Point();

            center.x = (ma * mb * (p1.y - p3.y) + mb * (p1.x + p2.x) - ma * (p2.x + p3.x)) /
                            (2 * (mb - ma));

            if(ma == 0)
                center.y = (-1 / mb) * (center.x - ((p2.x + p3.x) / 2)) + ((p2.y + p3.y) / 2);
            else
                center.y = (-1 / ma) * (center.x - ((p1.x + p2.x) / 2)) + ((p1.y + p2.y) / 2);

            radius = Math.Sqrt(Math.Pow(p1.x - center.x, 2) + Math.Pow(p1.y - center.y, 2));
        }

        //Helper method that calculates the angles of the first and last points
        //of the curve
        private void calculateAngles(Point p1, Point p2, Point p3, double length)
        {
            this.startangle = Dewlib.RestrictRange(Math.Atan2(p1.y - center.y, p1.x - center.x), 0, 2*Math.PI);
            double midangle = Dewlib.RestrictRange(Math.Atan2(p2.y - center.y, p2.x - center.x), 0, 2*Math.PI);
            //NOT the last point of this curve
            //Only used to calculate the direction of the curve
            double lastangle = Dewlib.RestrictRange(Math.Atan2(p3.y - center.y, p3.x - center.x), 0, 2*Math.PI);

            if(startangle >= midangle && midangle >= lastangle)
                clockwise = false;
            else if(startangle >= lastangle && lastangle >= midangle)
                clockwise = true;
            else if(midangle >= startangle && startangle >= lastangle)
                clockwise = true;
            else if(midangle >= lastangle && lastangle >= startangle)
                clockwise = false;
            else if(lastangle >= startangle && startangle >= midangle)
                clockwise = false;
            else if(lastangle >= midangle && midangle >= startangle)
                clockwise = true;

            //Use the arclength to calculate the final angle since the last control point
            //of the slider is NOT the last point of the curve
            //This is an angle differential since the formula assumes a start from an angle of 0
            double anglediff = length / radius;
            if(clockwise)
                this.endangle = Dewlib.RestrictRange(startangle + anglediff, 0, 2*Math.PI);
            else
                this.endangle = Dewlib.RestrictRange(startangle - anglediff, 0, 2*Math.PI);
        }
    }
}
