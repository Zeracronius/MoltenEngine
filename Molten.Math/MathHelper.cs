﻿// MIT, 2018 - James Yarwood - Molten Engine

// Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// -----------------------------------------------------------------------------
// Original code from SlimMath project. http://code.google.com/p/slimmath/
// Greetings to SlimDX Group. Original code published with the following license:
// -----------------------------------------------------------------------------

/*
* Copyright (c) 2007-2011 SlimDX Group
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;

namespace Molten
{
    /// <summary>
    /// Single-precision math helper class.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

        /// <summary>
        /// A value specifying the approximation of π which is 180 degrees.
        /// </summary>
        public const float Pi = (float)Math.PI;

        /// <summary>
        /// A value specifying the approximation of 2π which is 360 degrees.
        /// </summary>
        public const float TwoPi = (float)(2 * Math.PI);

        /// <summary>
        /// A value specifying the approximation of π/2 which is 90 degrees.
        /// </summary>
        public const float PiOverTwo = (float)(Math.PI / 2);

        /// <summary>
        /// A value specifying the approximation of π/4 which is 45 degrees.
        /// </summary>
        public const float PiOverFour = (float)(Math.PI / 4);

        /// <summary>
        /// Multiply by this value to convert from degrees to radians.
        /// </summary>
        public const float DegToRad = Pi / 180.0f;

        /// <summary>
        /// Multiply by this value to convert from radians to degrees.
        /// </summary>
        public const float RadToDeg = 180.0f / Pi;

        /// <summary>
        /// Multiply by this value to convert from gradians to radians.
        /// </summary>
        public const float GradToRad = Pi / 200.0f;

        /// <summary>
        /// Multiply by this value to convert from gradians to degrees.
        /// </summary>
        public const float GradToDeg = 9.0f / 10.0f;
        /// <summary>
        /// Multiply by this value to convert from radians to gradians.
        /// </summary>
        public const float RadToGrad = 200.0f / Pi;

        /// <summary>
        /// Large tolerance value. Defaults to 1e-5f.
        /// </summary>
        public static float BigEpsilon = 1E-5f;

        /// <summary>
        /// Tolerance value. Defaults to 1e-7f.
        /// </summary>
        public static float Epsilon = 1E-7f;

        /// <summary>
        /// Represents an invalid Vector3.
        /// </summary>
        public static readonly Vector3F NoVector = new Vector3F(-float.MaxValue, -float.MaxValue, -float.MaxValue);

        /// <summary>
        /// Reference for a vector with dimensions (0,0,1).
        /// </summary>
        public static Vector3F BackVector = Vector3F.BackwardLH;

        /// <summary>
        /// Reference for a vector with dimensions (0,-1,0).
        /// </summary>
        public static Vector3F DownVector = Vector3F.Down;

        /// <summary>
        /// Reference for a vector with dimensions (0,0,-1).
        /// </summary>
        public static Vector3F ForwardVector = Vector3F.ForwardLH;

        /// <summary>
        /// Refers to the identity quaternion.
        /// </summary>
        public static QuaternionF IdentityOrientation = QuaternionF.Identity;

        /// <summary>
        /// Reference for a vector with dimensions (-1,0,0).
        /// </summary>
        public static Vector3F LeftVector = Vector3F.Left;

        /// <summary>
        /// Reference for a vector with dimensions (1,0,0).
        /// </summary>
        public static Vector3F RightVector = Vector3F.Right;

        /// <summary>
        /// Reference for a vector with dimensions (0,1,0).
        /// </summary>
        public static Vector3F UpVector = Vector3F.Up;

        /// <summary>
        /// Matrix containing zeroes for every element.
        /// </summary>
        public static Matrix4F ZeroMatrix = new Matrix4F(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        /// <summary>
        /// Reference for a vector with dimensions (0,0,0).
        /// </summary>
        public static Vector3F ZeroVector = Vector3F.Zero;

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
        public unsafe static bool NearEqual(float a, float b)
        {
            // Check if the numbers are really close -- needed
            // when comparing numbers near zero.
            if (IsZero(a - b))
                return true;

            // Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            // Different signs means they do not match.
            if ((aInt < 0) != (bInt < 0))
                return false;

            // Find the difference in ULPs.
            int ulp = Math.Abs(aInt - bInt);

            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
            const int maxUlp = 4;
            return (ulp <= maxUlp);
        }

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
        public static bool IsZero(float a)
        {
            return Math.Abs(a) < ZeroTolerance;
        }

        /// <summary>
        /// Determines whether the specified value is close to one (1.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to one (1.0f); otherwise, <c>false</c>.</returns>
        public static bool IsOne(float a)
        {
            return IsZero(a - 1.0f);
        }

        /// <summary>
        /// Checks if a - b are almost equals within a float epsilon.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <param name="epsilon">Epsilon value</param>
        /// <returns><c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise</returns>
        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            float num = a - b;
            return ((-epsilon <= num) && (num <= epsilon));
        }

        /// <summary>
        /// Converts revolutions to degrees.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RevolutionsToDegrees(float revolution)
        {
            return revolution * 360.0f;
        }

        /// <summary>
        /// Converts revolutions to radians.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RevolutionsToRadians(float revolution)
        {
            return revolution * TwoPi;
        }

        /// <summary>
        /// Converts revolutions to gradians.
        /// </summary>
        /// <param name="revolution">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RevolutionsToGradians(float revolution)
        {
            return revolution * 400.0f;
        }

        /// <summary>
        /// Converts degrees to revolutions.
        /// </summary>
        /// <param name="degree">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float DegreesToRevolutions(float degree)
        {
            return degree / 360.0f;
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degree">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float DegreesToRadians(float degree)
        {
            return degree * DegToRad;
        }

        /// <summary>
        /// Converts radians to revolutions.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RadiansToRevolutions(float radian)
        {
            return radian / TwoPi;
        }

        /// <summary>
        /// Converts radians to gradians.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RadiansToGradians(float radian)
        {
            return radian * RadToGrad;
        }

        /// <summary>
        /// Converts gradians to revolutions.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float GradiansToRevolutions(float gradian)
        {
            return gradian / 400.0f;
        }

        /// <summary>
        /// Converts gradians to degrees.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float GradiansToDegrees(float gradian)
        {
            return gradian * GradToDeg;
        }

        /// <summary>
        /// Converts gradians to radians.
        /// </summary>
        /// <param name="gradian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float GradiansToRadians(float gradian)
        {
            return gradian * GradToRad;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radian">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static float RadiansToDegrees(float radian)
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
        public static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Clamps the specified integer value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// Clamps the specified unsigned integer value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static uint Clamp(uint value, uint min, uint max)
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
        public static float Lerp(float from, float to, float amount)
        {
            return (1 - amount) * from + amount * to;
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
        public static byte Lerp(byte from, byte to, float amount)
        {
            return (byte)Lerp((float)from, (float)to, amount);
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static float SmoothStep(float amount)
        {
            return (amount <= 0) ? 0
                : (amount >= 1) ? 1
                : amount * amount * (3 - (2 * amount));
        }


        /// <summary>
        /// Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static float SmootherStep(float amount)
        {
            return (amount <= 0) ? 0
                : (amount >= 1) ? 1
                : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        /// <summary>
        /// Calculates the modulo of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="modulo">The modulo.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static float Mod(float value, float modulo)
        {
            if (modulo == 0.0f)
                return value;

            return value % modulo;
        }

        /// <summary>
        /// Calculates the modulo 2*PI of the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the modulo applied to value</returns>
        public static float Mod2PI(float value)
        {
            return Mod(value, TwoPi);
        }

        /// <summary>
        /// Wraps the specified value into a range [min, max]
        /// </summary>
        /// <param name="value">The value to wrap.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>Result of the wrapping.</returns>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        public static int Wrap(int value, int min, int max)
        {
            if (min > max)
                throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");

            // Code from http://stackoverflow.com/a/707426/1356325
            int range_size = max - min + 1;

            if (value < min)
                value += range_size * ((min - value) / range_size + 1);

            return min + (value - min) % range_size;
        }

        /// <summary>
        /// Wraps the specified value into a range [min, max[
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>Result of the wrapping.</returns>
        /// <exception cref="ArgumentException">Is thrown when <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
        public static float Wrap(float value, float min, float max)
        {
            if (NearEqual(min, max))
                return min;

            double mind = min;
            double maxd = max;
            double valued = value;

            if (mind > maxd)
                throw new ArgumentException(string.Format("min {0} should be less than or equal to max {1}", min, max), "min");

            var range_size = maxd - mind;
            return (float)(mind + (valued - mind) - (range_size * Math.Floor((valued - mind) / range_size)));
        }

        /// <summary>
        /// Reduces the angle into a range from -Pi to Pi.
        /// </summary>
        /// <param name="angle">Angle to wrap.</param>
        /// <returns>Wrapped angle.</returns>
        public static float WrapAngle(float angle)
        {
            angle = (float)Math.IEEERemainder(angle, TwoPi);
            if (angle < -Pi)
            {
                angle += TwoPi;
                return angle;
            }
            if (angle >= Pi)
            {
                angle -= TwoPi;
            }
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
        public static float Gauss(float amplitude, float x, float y, float centerX, float centerY, float sigmaX, float sigmaY)
        {
            double cx = x - centerX;
            double cy = y - centerY;

            double componentX = (cx * cx) / (2 * sigmaX * sigmaX);
            double componentY = (cy * cy) / (2 * sigmaY * sigmaY);

            return (float)(amplitude * Math.Exp(-(componentX + componentY)));
        }

        /// <summary>Rounds down to the nearest X value.</summary>
        /// <param name="value">The value.</param>
        /// <param name="x">The x (nearest to round to).</param>
        /// <returns></returns>
        public static float RoundToNearest(float value, float x)
        {
            float diff = value % x;
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
        public static void GetVelocityOfPoint(ref Vector3F point, ref Vector3F center, ref Vector3F linearVelocity, ref Vector3F angularVelocity, out Vector3F velocity)
        {
            Vector3F offset;
            Vector3F.Subtract(ref point, ref center, out offset);
            Vector3F.Cross(ref angularVelocity, ref offset, out velocity);
            Vector3F.Add(ref velocity, ref linearVelocity, out velocity);
        }

        /// <summary>
        /// Computes the velocity of a point as if it were attached to an object with the given center and velocity.
        /// </summary>
        /// <param name="point">Point to compute the velocity of.</param>
        /// <param name="center">Center of the object to which the point is attached.</param>
        /// <param name="linearVelocity">Linear velocity of the object.</param>
        /// <param name="angularVelocity">Angular velocity of the object.</param>
        /// <returns>Velocity of the point.</returns>
        public static Vector3F GetVelocityOfPoint(Vector3F point, Vector3F center, Vector3F linearVelocity, Vector3F angularVelocity)
        {
            Vector3F toReturn;
            GetVelocityOfPoint(ref point, ref center, ref linearVelocity, ref angularVelocity, out toReturn);
            return toReturn;
        }


        /// <summary>Returns true if the specified value is to the power of two.</summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(ulong val)
        {
            return (val != 0) && ((val & (val - 1)) == 0);
        }

        /// <summary>Returns true if the specified value is to the power of two.</summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(long val)
        {
            return (val != 0) && ((val & (val - 1)) == 0);
        }

        /// <summary>Returns true if the specified value is to the power of two.</summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(uint val)
        {
            return (val != 0) && ((val & (val - 1)) == 0);
        }

        /// <summary>Returns true if the specified value is to the power of two.</summary>
        /// <param name="val">The value.</param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(int val)
        {
            return (val != 0) && ((val & (val - 1)) == 0);
        }

        /// <summary>
        /// Returns 1 for positive values, -1 for negative values, and 0 for zero.
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns></returns>
        public static int Sign(float val)
        {
            return (val == 0) ? 0 : (val > 0) ? 1 : -1;
        }

        /// <summary>
        /// Returns 1 for positive values, -1 for negative values, and 0 for zero.
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns></returns>
        public static int Sign(int val)
        {
            return (val == 0) ? 0 : (val > 0) ? 1 : -1;
        }

        /// <summary>
        /// Returns 1 for positive values, -1 for negative values, and 0 for zero.
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns></returns>
        public static int Sign(long val)
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
        public static void BarycentricCoordinates(ref Vector3F p, ref Vector3F a, ref Vector3F b, ref Vector3F c, out float aWeight, out float bWeight, out float cWeight)
        {
            Vector3F ab, ac;
            Vector3F.Subtract(ref b, ref a, out ab);
            Vector3F.Subtract(ref c, ref a, out ac);
            Vector3F triangleNormal;
            Vector3F.Cross(ref ab, ref ac, out triangleNormal);
            float x = triangleNormal.X < 0 ? -triangleNormal.X : triangleNormal.X;
            float y = triangleNormal.Y < 0 ? -triangleNormal.Y : triangleNormal.Y;
            float z = triangleNormal.Z < 0 ? -triangleNormal.Z : triangleNormal.Z;

            float numeratorU, numeratorV, denominator;
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

                float distance1 = Vector3F.DistanceSquared(ref p, ref a);
                float distance2 = Vector3F.DistanceSquared(ref p, ref b);
                float distance3 = Vector3F.DistanceSquared(ref p, ref c);
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

        /// Forumla to calculate signed area
        /// Positive if CCW
        /// Negative if CW
        /// 0 if collinear
        /// A[P1,P2,P3]  =  (x1*y2 - y1*x2) + (x2*y3 - y2*x3) + (x3*y1 - y3*x1)
        ///              =  (x1-x3)*(y2-y3) - (y1-y3)*(x2-x3)
        public static Winding GetWinding(Vector2F pa, Vector2F pb, Vector2F pc)
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

        public static int GetWindingSign(Vector2F pa, Vector2F pb, Vector2F pc)
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

        /// <summary>
        /// Returns the largest value out of all of the provided values.
        /// </summary>
        /// <param name="first">The first value to be compared.</param>
        /// <param name="otherValues">The other values to be compared.</param>
        /// <returns></returns>
        public static int Max(int first, params int[] otherValues)
        {
            if (otherValues.Length == 0)
                throw new Exception("At least one other value must be provided.");

            int max = first;
            for (int i = 0; i < otherValues.Length; i++)
                max = Math.Max(max, otherValues[i]);

            return max;
        }

        /// <summary>
        /// Returns the smallest value out of all of the provided values.
        /// </summary>
        /// <param name="first">The first value to be compared.</param>
        /// <param name="otherValues">The other values to be compared.</param>
        /// <returns></returns>
        public static int Min(int first, params int[] otherValues)
        {
            if (otherValues.Length == 0)
                throw new Exception("At least one other value must be provided.");

            int min = first;
            for (int i = 0; i < otherValues.Length; i++)
                min = Math.Min(min, otherValues[i]);

            return min;
        }
    }
}
