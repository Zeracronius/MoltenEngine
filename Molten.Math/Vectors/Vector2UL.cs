using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Molten
{
	///<summary>A <see cref = "ulong"/> vector comprised of two components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=8)]
	public partial struct Vector2UL : IFormattable
	{
		///<summary>The X component.</summary>
		public ulong X;

		///<summary>The Y component.</summary>
		public ulong Y;


		///<summary>The size of <see cref="Vector2UL"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2UL));

		///<summary>A Vector2UL with every component set to 1UL.</summary>
		public static readonly Vector2UL One = new Vector2UL(1UL, 1UL);

		/// <summary>The X unit <see cref="Vector2UL"/>.</summary>
		public static readonly Vector2UL UnitX = new Vector2UL(1UL, 0);

		/// <summary>The Y unit <see cref="Vector2UL"/>.</summary>
		public static readonly Vector2UL UnitY = new Vector2UL(0, 1UL);

		/// <summary>Represents a zero'd Vector2UL.</summary>
		public static readonly Vector2UL Zero = new Vector2UL(0, 0);

		 /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get => MathHelperDP.IsOne((X * X) + (Y * Y));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        public bool IsZero
        {
            get => X == 0 && Y == 0;
        }

#region Constructors

		///<summary>Creates a new instance of <see cref = "Vector2UL"/>.</summary>
		public Vector2UL(ulong x, ulong y)
		{
			X = x;
			Y = y;
		}

        ///<summary>Creates a new instance of <see cref = "Vector2UL"/>.</summary>
		public Vector2UL(ulong value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector2UL"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X and Y components of the vector. This must be an array with 2 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public Vector2UL(ulong[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 2)
                throw new ArgumentOutOfRangeException("values", "There must be 2 and only 2 input values for Vector2UL.");

			X = values[0];
			Y = values[1];
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector2UL"/> struct from an unsafe pointer. The pointer should point to an array of two elements.
        /// </summary>
		public unsafe Vector2UL(ulong* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
		}
#endregion

#region Instance Methods
        /// <summary>
        /// Determines whether the specified <see cref="Vector2UL"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector2UL"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector2UL"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Vector2UL other)
        {
            return MathHelperDP.NearEqual(other.X, X) && MathHelperDP.NearEqual(other.Y, Y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector2UL"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector2UL"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector2UL"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2UL other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector2UL"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector2UL"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector2UL"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is Vector2UL))
                return false;

            var strongValue = (Vector2UL)value;
            return Equals(ref strongValue);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Calculates the length of the vector.
        /// </summary>
        /// <returns>The length of the vector.</returns>
        /// <remarks>
        /// <see cref="Vector2F.LengthSquared"/> may be preferred when only the relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public ulong Length()
        {
            return (ulong)Math.Sqrt((X * X) + (Y * Y));
        }

        /// <summary>
        /// Calculates the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public ulong LengthSquared()
        {
            return ((X * X) + (Y * Y));
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        public void Normalize()
        {
            ulong length = Length();
            if (!MathHelperDP.IsZero(length))
            {
                double inverse = 1.0D / length;
			    X = (ulong)(X * inverse);
			    Y = (ulong)(Y * inverse);
            }
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="Vector2UL"/>.
        /// </summary>
        /// <returns>A two-element array containing the components of the vector.</returns>
        public ulong[] ToArray()
        {
            return new ulong[] { X, Y};
        }
		
        /// <summary>
        /// Returns a normalized unit vector of the original vector.
        /// </summary>
        public Vector2UL GetNormalized()
        {
            double length = Length();
            if (!MathHelperDP.IsZero(length))
            {
                double inverse = 1.0D / length;
                return new Vector2UL()
                {
			        X = (ulong)(this.X * inverse),
			        Y = (ulong)(this.Y * inverse),
                };
            }
            else
            {
                return new Vector2UL();
            }
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(ulong min, ulong max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(Vector2UL min, Vector2UL max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1}", X, Y);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", X, Y);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2UL"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider));
        }
#endregion

#region Add operators
        public static void Add(ref Vector2UL left, ref Vector2UL right, out Vector2UL result)
        {
			result.X = (left.X + right.X);
			result.Y = (left.Y + right.Y);
        }

        public static void Add(ref Vector2UL left, ulong right, out Vector2UL result)
        {
			result.X = (left.X + right);
			result.Y = (left.Y + right);
        }

		public static Vector2UL operator +(Vector2UL left, Vector2UL right)
		{
			Add(ref left, ref right, out Vector2UL result);
            return result;
		}

		public static Vector2UL operator +(Vector2UL left, ulong right)
		{
            Add(ref left, right, out Vector2UL result);
            return result;
		}

        public static Vector2UL operator +(ulong left, Vector2UL right)
		{
            Add(ref right, left, out Vector2UL result);
            return result;
		}

		/// <summary>
        /// Assert a <see cref="Vector2UL"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="Vector2UL"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="Vector2UL"/>.</returns>
        public static Vector2UL operator +(Vector2UL value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static void Subtract(ref Vector2UL left, ref Vector2UL right, out Vector2UL result)
        {
			result.X = (left.X - right.X);
			result.Y = (left.Y - right.Y);
        }

        public static void Subtract(ref Vector2UL left, ulong right, out Vector2UL result)
        {
			result.X = (left.X - right);
			result.Y = (left.Y - right);
        }

		public static Vector2UL operator -(Vector2UL left, Vector2UL right)
		{
			Subtract(ref left, ref right, out Vector2UL result);
            return result;
		}

		public static Vector2UL operator -(Vector2UL left, ulong right)
		{
            Subtract(ref left, right, out Vector2UL result);
            return result;
		}

        public static Vector2UL operator -(ulong left, Vector2UL right)
		{
            Subtract(ref right, left, out Vector2UL result);
            return result;
		}

#endregion

#region division operators
		public static void Divide(ref Vector2UL left, ref Vector2UL right, out Vector2UL result)
        {
			result.X = (left.X / right.X);
			result.Y = (left.Y / right.Y);
        }

        public static void Divide(ref Vector2UL left, ulong right, out Vector2UL result)
        {
			result.X = (left.X / right);
			result.Y = (left.Y / right);
        }

		public static Vector2UL operator /(Vector2UL left, Vector2UL right)
		{
			Divide(ref left, ref right, out Vector2UL result);
            return result;
		}

		public static Vector2UL operator /(Vector2UL left, ulong right)
		{
            Divide(ref left, right, out Vector2UL result);
            return result;
		}

        public static Vector2UL operator /(ulong left, Vector2UL right)
		{
            Divide(ref right, left, out Vector2UL result);
            return result;
		}
#endregion

#region Multiply operators
		public static void Multiply(ref Vector2UL left, ref Vector2UL right, out Vector2UL result)
        {
			result.X = (left.X * right.X);
			result.Y = (left.Y * right.Y);
        }

        public static void Multiply(ref Vector2UL left, ulong right, out Vector2UL result)
        {
			result.X = (left.X * right);
			result.Y = (left.Y * right);
        }

		public static Vector2UL operator *(Vector2UL left, Vector2UL right)
		{
			Multiply(ref left, ref right, out Vector2UL result);
            return result;
		}

		public static Vector2UL operator *(Vector2UL left, ulong right)
		{
            Multiply(ref left, right, out Vector2UL result);
            return result;
		}

        public static Vector2UL operator *(ulong left, Vector2UL right)
		{
            Multiply(ref right, left, out Vector2UL result);
            return result;
		}
#endregion

#region Operators - Equality
        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector2UL left, Vector2UL right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector2UL left, Vector2UL right)
        {
            return !left.Equals(ref right);
        }
#endregion

#region Operators - Cast
        ///<summary>Casts a <see cref="Vector2UL"/> to a <see cref="Vector3UL"/>.</summary>
        public static explicit operator Vector3UL(Vector2UL value)
        {
            return new Vector3UL(value.X, value.Y, 0);
        }

        ///<summary>Casts a <see cref="Vector2UL"/> to a <see cref="Vector4UL"/>.</summary>
        public static explicit operator Vector4UL(Vector2UL value)
        {
            return new Vector4UL(value.X, value.Y, 0, 0);
        }

#endregion

#region Static Methods
        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(Vector2UL left, Vector2UL right, Vector2UL epsilon)
        {
            return NearEqual(ref left, ref right, ref epsilon);
        }

        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(ref Vector2UL left, ref Vector2UL right, ref Vector2UL epsilon)
        {
            return MathHelperDP.WithinEpsilon(left.X, right.X, epsilon.X) && MathHelperDP.WithinEpsilon(left.Y, right.Y, epsilon.Y);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector2UL SmoothStep(ref Vector2UL start, ref Vector2UL end, double amount)
        {
            amount = MathHelperDP.SmoothStep(amount);
            return Lerp(ref start, ref end, amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two vectors.</returns>
        public static Vector2UL SmoothStep(Vector2UL start, Vector2UL end, ulong amount)
        {
            return SmoothStep(ref start, ref end, amount);
        }    

        /// <summary>
        /// Orthogonalizes a list of <see cref="Vector2UL"/>.
        /// </summary>
        /// <param name="destination">The list of orthogonalized <see cref="Vector2UL"/>.</param>
        /// <param name="source">The list of vectors to orthogonalize.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all vectors orthogonal to each other. This
        /// means that any given vector in the list will be orthogonal to any other given vector in the
        /// list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthogonalize(Vector2UL[] destination, params Vector2UL[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3
            //q5 = ...

            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Vector2UL newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= (Dot(destination[r], newvector) / Dot(destination[r], destination[r])) * destination[r];

                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Orthonormalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthonormalized vectors.</param>
        /// <param name="source">The list of vectors to orthonormalize.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all vectors orthogonal to each
        /// other and making all vectors of unit length. This means that any given vector will
        /// be orthogonal to any other given vector in the list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthonormalize(Vector2UL[] destination, params Vector2UL[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthogonalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|
            //q5 = ...

            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Vector2UL newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= Dot(destination[r], newvector) * destination[r];

                newvector.Normalize();
                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Vector2UL"/>. <para />
        /// For example, a swizzle input of (1,1) on a <see cref="Vector2UL"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
        /// </summary>
        /// <param name="val">The current vector.</param>
		/// <param name="xIndex">The axis index to use for the new X value.</param>
		/// <param name="yIndex">The axis index to use for the new Y value.</param>
        /// <returns></returns>
        public static unsafe Vector2UL Swizzle(Vector2UL val, int xIndex, int yIndex)
        {
            return new Vector2UL()
            {
			   X = (&val.X)[xIndex],
			   Y = (&val.X)[yIndex],
            };
        }

        /// <returns></returns>
        public static unsafe Vector2UL Swizzle(Vector2UL val, uint xIndex, uint yIndex)
        {
            return new Vector2UL()
            {
			    X = (&val.X)[xIndex],
			    Y = (&val.X)[yIndex],
            };
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector2UL.DistanceSquared(Vector2UL, Vector2UL)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static ulong Distance(ref Vector2UL value1, ref Vector2UL value2)
        {
			ulong x = (value1.X - value2.X);
			ulong y = (value1.Y - value2.Y);

            return (ulong)Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector2UL.DistanceSquared(Vector2UL, Vector2UL)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static ulong Distance(Vector2UL value1, Vector2UL value2)
        {
			ulong x = (value1.X - value2.X);
			ulong y = (value1.Y - value2.Y);

            return (ulong)Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>Checks to see if any value (x, y, z, w) are within 0.0001 of 0.
        /// If so this method truncates that value to zero.</summary>
        /// <param name="power">The power.</param>
        /// <param name="vec">The vector.</param>
        public static Vector2UL Pow(Vector2UL vec, ulong power)
        {
            return new Vector2UL()
            {
				X = (ulong)Math.Pow(vec.X, power),
				Y = (ulong)Math.Pow(vec.Y, power),
            };
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector2UL"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector2UL"/> source vector</param>
        /// <param name="right">Second <see cref="Vector2UL"/> source vector.</param>
        public static ulong Dot(ref Vector2UL left, ref Vector2UL right)
        {
			return ((left.X * right.X) + (left.Y * right.Y));
        }

		/// <summary>
        /// Calculates the dot product of two <see cref="Vector2UL"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector2UL"/> source vector</param>
        /// <param name="right">Second <see cref="Vector2UL"/> source vector.</param>
        public static ulong Dot(Vector2UL left, Vector2UL right)
        {
			return ((left.X * right.X) + (left.Y * right.Y));
        }

		/// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector2UL"/> vector.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector2UL"/> vector.</param>
        /// <param name="value2">Second source position <see cref="Vector2UL"/> vector.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector2UL"/> vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector2UL Hermite(ref Vector2UL value1, ref Vector2UL tangent1, ref Vector2UL value2, ref Vector2UL tangent2, ulong amount)
        {
            double squared = amount * amount;
            double cubed = amount * squared;
            double part1 = ((2.0D * cubed) - (3.0D * squared)) + 1.0D;
            double part2 = (-2.0D * cubed) + (3.0D * squared);
            double part3 = (cubed - (2.0D * squared)) + amount;
            double part4 = cubed - squared;

			return new Vector2UL()
			{
				X = (ulong)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4)),
				Y = (ulong)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4)),
			};
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector2UL"/>.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector2UL"/>.</param>
        /// <param name="value2">Second source position <see cref="Vector2UL"/>.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector2UL"/>.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Vector2UL Hermite(Vector2UL value1, Vector2UL tangent1, Vector2UL value2, Vector2UL tangent2, ulong amount)
        {
            return Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount);
        }

		/// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector2UL"/> containing the 2D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector2UL"/> containing the 2D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector2UL"/> containing the 2D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector2UL Barycentric(ref Vector2UL value1, ref Vector2UL value2, ref Vector2UL value3, ulong amount1, ulong amount2)
        {
			return new Vector2UL(
				((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))), 
				((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)))
			);
        }

        /// <summary>
        /// Performs a linear interpolation between two <see cref="Vector2UL"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref Vector2UL start, ref Vector2UL end, double amount, out Vector2UL result)
        {
			result.X = (ulong)((1D - amount) * start.X + amount * end.X);
			result.Y = (ulong)((1D - amount) * start.Y + amount * end.Y);
        }

		/// <summary>
        /// Performs a linear interpolation between two <see cref="Vector2UL"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector2UL Lerp(ref Vector2UL start, ref Vector2UL end, double amount)
        {
			return new Vector2UL()
			{
				X = (ulong)((1D - amount) * start.X + amount * end.X),
				Y = (ulong)((1D - amount) * start.Y + amount * end.Y),
			};
        }

        /// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector2UL"/>.</param>
        /// <returns>A <see cref="Vector2UL"/> containing the smallest components of the source vectors.</returns>
		public static void Min(ref Vector2UL left, ref Vector2UL right, out Vector2UL result)
		{
				result.X = (left.X < right.X) ? left.X : right.X;
				result.Y = (left.Y < right.Y) ? left.Y : right.Y;
		}

        /// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector2UL"/>.</param>
        /// <returns>A <see cref="Vector2UL"/> containing the smallest components of the source vectors.</returns>
		public static Vector2UL Min(ref Vector2UL left, ref Vector2UL right)
		{
			Min(ref left, ref right, out Vector2UL result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector2UL"/>.</param>
        /// <returns>A <see cref="Vector2UL"/> containing the smallest components of the source vectors.</returns>
		public static Vector2UL Min(Vector2UL left, Vector2UL right)
		{
			return new Vector2UL()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
			};
		}

        /// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector2UL"/>.</param>
        /// <returns>A <see cref="Vector2UL"/> containing the largest components of the source vectors.</returns>
		public static void Max(ref Vector2UL left, ref Vector2UL right, out Vector2UL result)
		{
				result.X = (left.X > right.X) ? left.X : right.X;
				result.Y = (left.Y > right.Y) ? left.Y : right.Y;
		}

        /// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector2UL"/>.</param>
        /// <returns>A <see cref="Vector2UL"/> containing the largest components of the source vectors.</returns>
		public static Vector2UL Max(ref Vector2UL left, ref Vector2UL right)
		{
			Max(ref left, ref right, out Vector2UL result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector2UL"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector2UL"/>.</param>
        /// <returns>A <see cref="Vector2UL"/> containing the largest components of the source vectors.</returns>
		public static Vector2UL Max(Vector2UL left, Vector2UL right)
		{
			return new Vector2UL()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
			};
		}

		/// <summary>
        /// Calculates the squared distance between two <see cref="Vector2UL"/> vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
		public static ulong DistanceSquared(ref Vector2UL value1, ref Vector2UL value2)
        {
            ulong x = value1.X - value2.X;
            ulong y = value1.Y - value2.Y;

            return ((x * x) + (y * y));
        }

        /// <summary>
        /// Calculates the squared distance between two <see cref="Vector2UL"/> vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The squared distance between the two vectors.</returns>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
		public static ulong DistanceSquared(Vector2UL value1, Vector2UL value2)
        {
            ulong x = value1.X - value2.X;
            ulong y = value1.Y - value2.Y;

            return ((x * x) + (y * y));
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector2UL"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector2UL Clamp(Vector2UL value, ulong min, ulong max)
        {
			return new Vector2UL()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
			};
        }

        /// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector2UL"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static void Clamp(ref Vector2UL value, ref Vector2UL min, ref Vector2UL max, out Vector2UL result)
        {
				result.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
				result.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector2UL"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector2UL Clamp(Vector2UL value, Vector2UL min, Vector2UL max)
        {
			return new Vector2UL()
			{
				X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
				Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
			};
        }


        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal. 
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.</remarks>
        public static Vector2UL Reflect(ref Vector2UL vector, ref Vector2UL normal)
        {
            ulong dot = (vector.X * normal.X) + (vector.Y * normal.Y);

            return new Vector2UL()
            {
				X = (ulong)(vector.X - ((2.0D * dot) * normal.X)),
				Y = (ulong)(vector.Y - ((2.0D * dot) * normal.Y)),
            };
        }

        /// <summary>
        /// Converts the <see cref="Vector2UL"/> into a unit vector.
        /// </summary>
        /// <param name="value">The <see cref="Vector2UL"/> to normalize.</param>
        /// <returns>The normalized <see cref="Vector2UL"/>.</returns>
        public static Vector2UL Normalize(Vector2UL value)
        {
            value.Normalize();
            return value;
        }
#endregion

#region Indexers
		/// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X or Y component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component and so on.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 1].</exception>
        
		public ulong this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return X;
					case 1: return Y;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector2UL run from 0 to 1, inclusive.");
			}

			set
			{
				switch(index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector2UL run from 0 to 1, inclusive.");
			}
		}
#endregion
	}
}

