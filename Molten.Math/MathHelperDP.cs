﻿using System;

namespace Molten
{
    /// <summary>
    /// Double-precision math helper class
    /// </summary>
    public static class MathHelperDP
    {
        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const double ZeroTolerance = 1e-6D; // Value a 8x higher than 1.19209290E-07F

        /// <summary>
        /// A value specifying the approximation of π which is 180 degrees.
        /// </summary>
        public const double Pi = Math.PI;

        /// <summary>
        /// A value specifying the approximation of 2π which is 360 degrees.
        /// </summary>
        public const double TwoPi = (2D * Math.PI);

        /// <summary>
        /// A value specifying the approximation of π/2 which is 90 degrees.
        /// </summary>
        public const double PiOverTwo = (Math.PI / 2);

        /// <summary>
        /// A value specifying the approximation of π/4 which is 45 degrees.
        /// </summary>
        public const double PiOverFour = (Math.PI / 4);

        /// <summary>
        /// Multiply by this value to convert from degrees to radians.
        /// </summary>
        public const double DegToRad = Pi / 180.0D;

        /// <summary>
        /// Multiply by this value to convert from radians to degrees.
        /// </summary>
        public const double RadToDeg = 180.0D / Pi;

        /// <summary>
        /// Multiply by this value to convert from gradians to radians.
        /// </summary>
        public const double GradToRad = Pi / 200.0D;

        /// <summary>
        /// Multiply by this value to convert from gradians to degrees.
        /// </summary>
        public const double GradToDeg = 9.0D / 10.0D;
        /// <summary>
        /// Multiply by this value to convert from radians to gradians.
        /// </summary>
        public const double RadToGrad = 200.0D / Pi;

        /// <summary>
        /// Large tolerance value. Defaults to 1e-5D.
        /// </summary>
        public static double BigEpsilon = 1E-5D;

        /// <summary>
        /// Tolerance value. Defaults to 1e-7D.
        /// </summary>
        public static double Epsilon = 1E-7D;

        /// <summary>
        /// Represents an invalid Vector3.
        /// </summary>
        public static readonly Vector3D NoVector = new Vector3D(-double.MaxValue, -double.MaxValue, -double.MaxValue);

        /// <summary>
        /// Reference for a vector with dimensions (0,0,1).
        /// </summary>
        public static Vector3D BackVector = Vector3D.BackwardLH;

        /// <summary>
        /// Reference for a vector with dimensions (0,-1,0).
        /// </summary>
        public static Vector3D DownVector = Vector3D.Down;

        /// <summary>
        /// Reference for a vector with dimensions (0,0,-1).
        /// </summary>
        public static Vector3D ForwardVector = Vector3D.ForwardLH;

        /// <summary>
        /// Refers to the identity quaternion.
        /// </summary>
        public static QuaternionF IdentityOrientation = QuaternionF.Identity;

        /// <summary>
        /// Reference for a vector with dimensions (-1,0,0).
        /// </summary>
        public static Vector3D LeftVector = Vector3D.Left;

        /// <summary>
        /// Reference for a vector with dimensions (1,0,0).
        /// </summary>
        public static Vector3D RightVector = Vector3D.Right;

        /// <summary>
        /// Reference for a vector with dimensions (0,1,0).
        /// </summary>
        public static Vector3D UpVector = Vector3D.Up;

        /// <summary>
        /// Matrix containing zeroes for every element.
        /// </summary>
        public static Matrix4D ZeroMatrix = new Matrix4D(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Reference for a vector with dimensions (0,0,0).
        /// </summary>
        public static Vector3D ZeroVector = Vector3D.Zero;

        /// <summary>
        /// Refers to the rigid identity transformation.
        /// </summary>
        public static RigidTransform RigidIdentity = RigidTransform.Identity;
        
        /// <summary>
        /// Checks if a and b are almost equals, taking into account the magnitude of floating point numbers (unlike <see cref="WithinEpsilon"/> method). See Remarks.
        /// See remarks.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <returns><c>true</c> if a almost equal to b, <c>false</c> otherwise</returns>
        /// <remarks>
        /// The code is using the technique described by Bruce Dawson in 
        /// <a href="http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/">Comparing Floating point numbers 2012 edition</a>. 
        /// </remarks>
        public unsafe static bool NearEqual(double a, double b)
        {
            // Check if the numbers are really close -- needed
            // when comparing numbers near zero.
            if (IsZero(a - b))
                return true;

            // Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            long aInt = *(long*)&a;
            long bInt = *(long*)&b;

            // Different signs means they do not match.
            if ((aInt < 0L) != (bInt < 0L))
                return false;

            // Find the difference in ULPs.
            long ulp = Math.Abs(aInt - bInt);

            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
            const long maxUlp = 4;
            return (ulp <= maxUlp);
        }

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0D).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0D); otherwise, <c>false</c>.</returns>
        public static bool IsZero(double a)
        {
            return Math.Abs(a) < ZeroTolerance;
        }

        /// <summary>
        /// Determines whether the specified value is close to one (1.0D).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to one (1.0D); otherwise, <c>false</c>.</returns>
        public static bool IsOne(double a)
        {
            return IsZero(a - 1.0D);
        }

        /// <summary>
        /// Checks if a - b are almost equals within a float epsilon.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <param name="epsilon">Epsilon value</param>
        /// <returns><c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise</returns>
        public static bool WithinEpsilon(double a, double b, double epsilon)
        {
            double num = a - b;
            return ((-epsilon <= num) && (num <= epsilon));
        }

        /// <summary>
        /// Converts revolutions to degrees.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double RevolutionsToDegrees(double revolution)
        {
            return revolution * 360.0D;
        }

        /// <summary>
        /// Converts revolutions to radians.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double RevolutionsToRadians(double revolution)
        {
            return revolution * TwoPi;
        }

        /// <summary>
        /// Converts revolutions to gradians.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double RevolutionsToGradians(double revolution)
        {
            return revolution * 400.0D;
        }

        /// <summary>
        /// Converts degrees to revolutions.
        /// </summary>
        /// <param name="degree">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double DegreesToRevolutions(double degree)
        {
            return degree / 360.0D;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degree">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double DegreesToRadians(double degree)
        {
            return degree * DegToRad;
        }

        /// <summary>
        /// Converts radians to revolutions.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double RadiansToRevolutions(double radian)
        {
            return radian / TwoPi;
        }

        /// <summary>
        /// Converts radians to gradians.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double RadiansToGradians(double radian)
        {
            return radian * RadToGrad;
        }

        /// <summary>
        /// Converts gradians to revolutions.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double GradiansToRevolutions(double gradian)
        {
            return gradian / 400.0D;
        }

        /// <summary>
        /// Converts gradians to degrees.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double GradiansToDegrees(double gradian)
        {
            return gradian * GradToDeg;
        }

        /// <summary>
        /// Converts gradians to radians.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double GradiansToRadians(double gradian)
        {
            return gradian * GradToRad;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static double RadiansToDegrees(double radian)
        {
            return radian * RadToDeg;
        }

        /// <summary>
        /// Clamps the specified float value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <remarks>
        /// See http://www.encyclopediaofmath.org/index.php/Linear_interpolation and
        /// http://fgiesen.wordpress.com/2012/08/15/linear-interpolation-past-present-and-future/
        /// </remarks>
        /// <param name="from">Value to interpolate from.</param>
        /// <param name="to">Value to interpolate to.</param>
        /// <param name="amount">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        public static double Lerp(double from, double to, double amount)
        {
            return (1D - amount) * from + amount * to;
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static double SmoothStep(double amount)
        {
            return (amount <= 0D) ? 0D
                : (amount >= 1D) ? 1D
                : amount * amount * (3D - (2D * amount));
        }


        /// <summary>
        /// Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static double SmootherStep(double amount)
        {
            return (amount <= 0D) ? 0D
                : (amount >= 1D) ? 1D
                : amount * amount * amount * (amount * ((amount * 6D) - 15D) + 10D);
        }

        /// <summary>
        /// Calculates the modulo of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulo">The modulo.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static double Mod(double value, double modulo)
        {
            if (modulo == 0.0D)
                return value;

            return value % modulo;
        }

        /// <summary>
        /// Calculates the modulo 2*PI of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static double Mod2PI(double value)
        {
            return Mod(value, TwoPi);
        }

        /// <summary>
        /// Wraps the specified value into a range [min, max[
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>Result of the wrapping.</returns>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        public static double Wrap(double value, double min, double max)
        {
            if (NearEqual(min, max))
                return min;

            double mind = min;
            double maxd = max;
            double valued = value;

            if (mind > maxd)
                throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");

            double range_size = maxd - mind;
            return mind + (valued - mind) - (range_size * Math.Floor((valued - mind) / range_size));
        }

        /// <summary>
        /// Reduces the angle into a range from -Pi to Pi.
        /// </summary>
        /// <param name="angle">Angle to wrap.</param>
        /// <returns>Wrapped angle.</returns>
        public static double WrapAngle(double angle)
        {
            angle = Math.IEEERemainder(angle, TwoPi);
            if (angle < -Pi)
            {
                angle += TwoPi;
                return angle;
            }

            if (angle >= Pi)
                angle -= TwoPi;

            return angle;
        }

        /// <summary>
        /// Gauss function.
        /// http://en.wikipedia.org/wiki/Gaussian_function#Two-dimensional_Gaussian_function
        /// </summary>
        /// <param name="amplitude">Curve amplitude.</param>
        /// <param name="x">Position X.</param>
        /// <param name="y">Position Y</param>
        /// <param name="centerX">Center X.</param>
        /// <param name="centerY">Center Y.</param>
        /// <param name="sigmaX">Curve sigma X.</param>
        /// <param name="sigmaY">Curve sigma Y.</param>
        /// <returns>The result of Gaussian function.</returns>
        public static double Gauss(double amplitude, double x, double y, double centerX, double centerY, double sigmaX, double sigmaY)
        {
            double cx = x - centerX;
            double cy = y - centerY;

            double componentX = (cx * cx) / (2 * sigmaX * sigmaX);
            double componentY = (cy * cy) / (2 * sigmaY * sigmaY);

            return amplitude * Math.Exp(-(componentX + componentY));
        }

        /// <summary>Rounds down to the nearest X value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="x">The x (nearest to round to).</param>
        /// <returns></returns>
        public static double RoundToNearest(double value, double x)
        {
            double diff = value % x;
            return value - diff;
        }

        /// <summary>
        /// Computes the velocity of a point as if it were attached to an object with the given center and velocity.
        /// </summary>
        /// <param name="point">Point to compute the velocity of.</param>
        /// <param name="center">Center of the object to which the point is attached.</param>
        /// <param name="linearVelocity">Linear velocity of the object.</param>
        /// <param name="angularVelocity">Angular velocity of the object.</param>
        /// <param name="velocity">Velocity of the point.</param>
        public static void GetVelocityOfPoint(ref Vector3D point, ref Vector3D center, ref Vector3D linearVelocity, ref Vector3D angularVelocity, out Vector3D velocity)
        {
            Vector3D offset;
            Vector3D.Subtract(ref point, ref center, out offset);
            Vector3D.Cross(ref angularVelocity, ref offset, out velocity);
            Vector3D.Add(ref velocity, ref linearVelocity, out velocity);
        }

        /// <summary>
        /// Computes the velocity of a point as if it were attached to an object with the given center and velocity.
        /// </summary>
        /// <param name="point">Point to compute the velocity of.</param>
        /// <param name="center">Center of the object to which the point is attached.</param>
        /// <param name="linearVelocity">Linear velocity of the object.</param>
        /// <param name="angularVelocity">Angular velocity of the object.</param>
        /// <returns>Velocity of the point.</returns>
        public static Vector3D GetVelocityOfPoint(Vector3D point, Vector3D center, Vector3D linearVelocity, Vector3D angularVelocity)
        {
            Vector3D toReturn;
            GetVelocityOfPoint(ref point, ref center, ref linearVelocity, ref angularVelocity, out toReturn);
            return toReturn;
        }

        /// <summary>
        /// Returns 1 for positive values, -1 for negative values, and 0 for zero.
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns></returns>
        public static int Sign(double val)
        {
            return (val == 0) ? 0 : (val > 0) ? 1 : -1;
        }

        /// <summary>
        /// Gets the barycentric coordinates of the point with respect to a triangle's vertices.
        /// </summary>
        /// <param name="p">Point to compute the barycentric coordinates of.</param>
        /// <param name="a">First vertex in the triangle.</param>
        /// <param name="b">Second vertex in the triangle.</param>
        /// <param name="c">Third vertex in the triangle.</param>
        /// <param name="aWeight">Weight of the first vertex.</param>
        /// <param name="bWeight">Weight of the second vertex.</param>
        /// <param name="cWeight">Weight of the third vertex.</param>
        public static void BarycentricCoordinates(ref Vector3D p, ref Vector3D a, ref Vector3D b, ref Vector3D c, out double aWeight, out double bWeight, out double cWeight)
        {
            Vector3D ab, ac;
            Vector3D.Subtract(ref b, ref a, out ab);
            Vector3D.Subtract(ref c, ref a, out ac);
            Vector3D triangleNormal;
            Vector3D.Cross(ref ab, ref ac, out triangleNormal);
            double x = triangleNormal.X < 0 ? -triangleNormal.X : triangleNormal.X;
            double y = triangleNormal.Y < 0 ? -triangleNormal.Y : triangleNormal.Y;
            double z = triangleNormal.Z < 0 ? -triangleNormal.Z : triangleNormal.Z;

            double numeratorU, numeratorV, denominator;
            if (x >= y && x >= z)
            {
                //The projection of the triangle on the YZ plane is the largest.
                numeratorU = (p.Y - b.Y) * (b.Z - c.Z) - (b.Y - c.Y) * (p.Z - b.Z); //PBC
                numeratorV = (p.Y - c.Y) * (c.Z - a.Z) - (c.Y - a.Y) * (p.Z - c.Z); //PCA
                denominator = triangleNormal.X;
            }
            else if (y >= z)
            {
                //The projection of the triangle on the XZ plane is the largest.
                numeratorU = (p.X - b.X) * (b.Z - c.Z) - (b.X - c.X) * (p.Z - b.Z); //PBC
                numeratorV = (p.X - c.X) * (c.Z - a.Z) - (c.X - a.X) * (p.Z - c.Z); //PCA
                denominator = -triangleNormal.Y;
            }
            else
            {
                //The projection of the triangle on the XY plane is the largest.
                numeratorU = (p.X - b.X) * (b.Y - c.Y) - (b.X - c.X) * (p.Y - b.Y); //PBC
                numeratorV = (p.X - c.X) * (c.Y - a.Y) - (c.X - a.X) * (p.Y - c.Y); //PCA
                denominator = triangleNormal.Z;
            }

            if (denominator < -1e-9 || denominator > 1e-9)
            {
                denominator = 1 / denominator;
                aWeight = numeratorU * denominator;
                bWeight = numeratorV * denominator;
                cWeight = 1 - aWeight - bWeight;
            }
            else
            {
                //It seems to be a degenerate triangle.
                //In that case, pick one of the closest vertices.
                //MOST of the time, this will happen when the vertices
                //are all very close together (all three points form a single point).
                //Sometimes, though, it could be that it's more of a line.
                //If it's a little inefficient, don't worry- this is a corner case anyway.

                double distance1 = Vector3D.DistanceSquared(ref p, ref a);
                double distance2 = Vector3D.DistanceSquared(ref p, ref b);
                double distance3 = Vector3D.DistanceSquared(ref p, ref c);
                if (distance1 < distance2 && distance1 < distance3)
                {
                    aWeight = 1;
                    bWeight = 0;
                    cWeight = 0;
                }
                else if (distance2 < distance3)
                {
                    aWeight = 0;
                    bWeight = 1;
                    cWeight = 0;
                }
                else
                {
                    aWeight = 0;
                    bWeight = 0;
                    cWeight = 1;
                }
            }
        }

        /// <summary>
        /// Forumla to calculate signed area. Positive if CCW. Negative if CW. 0 if collinear
        /// 
        /// </summary>
        /// <remarks>A[P1,P2,P3]  =  (x1*y2 - y1*x2) + (x2*y3 - y2*x3) + (x3*y1 - y3*x1)
        ///              =  (x1-x3)*(y2-y3) - (y1-y3)*(x2-x3)
        /// </remarks>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <param name="pc"></param>
        /// <returns></returns>
        public static Winding GetWinding(Vector2D pa, Vector2D pb, Vector2D pc)
        {
            double detleft = (pa.X - pc.X) * (pb.Y - pc.Y);
            double detright = (pa.Y - pc.Y) * (pb.X - pc.X);
            double val = detleft - detright;

            if (val > -TriUtil.EPSILON && val < TriUtil.EPSILON)
                return Winding.Collinear;
            else if (val > 0)
                return Winding.CounterClockwise;

            return Winding.Clockwise;
        }

        public static int GetWindingSign(Vector2D pa, Vector2D pb, Vector2D pc)
        {
            double detleft = (pa.X - pc.X) * (pb.Y - pc.Y);
            double detright = (pa.Y - pc.Y) * (pb.X - pc.X);
            double val = detleft - detright;

            if (val > -TriUtil.EPSILON && val < TriUtil.EPSILON)
                return 0;
            else if (val > 0)
                return -1;

            return 1;
        }
    }
}
