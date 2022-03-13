using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Molten
{
	///<summary>A <see cref = "ulong"/> vector comprised of three components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=8)]
	public partial struct Vector3UL : IFormattable
	{
		///<summary>The X component.</summary>
		public ulong X;

		///<summary>The Y component.</summary>
		public ulong Y;

		///<summary>The Z component.</summary>
		public ulong Z;


		///<summary>The size of <see cref="Vector3UL"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3UL));

		///<summary>A Vector3UL with every component set to 1UL.</summary>
		public static readonly Vector3UL One = new Vector3UL(1UL, 1UL, 1UL);

		/// <summary>The X unit <see cref="Vector3UL"/>.</summary>
		public static readonly Vector3UL UnitX = new Vector3UL(1UL, 0, 0);

		/// <summary>The Y unit <see cref="Vector3UL"/>.</summary>
		public static readonly Vector3UL UnitY = new Vector3UL(0, 1UL, 0);

		/// <summary>The Z unit <see cref="Vector3UL"/>.</summary>
		public static readonly Vector3UL UnitZ = new Vector3UL(0, 0, 1UL);

		/// <summary>Represents a zero'd Vector3UL.</summary>
		public static readonly Vector3UL Zero = new Vector3UL(0, 0, 0);

		 /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get => MathHelperDP.IsOne((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        public bool IsZero
        {
            get => X == 0 && Y == 0 && Z == 0;
        }

#region Constructors
        ///<summary>Creates a new instance of <see cref = "Vector3UL"/>, using a <see cref="Vector2UL"/> to populate the first two components.</summary>
		public Vector3UL(Vector2UL vector, ulong z)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
		}

		///<summary>Creates a new instance of <see cref = "Vector3UL"/>.</summary>
		public Vector3UL(ulong x, ulong y, ulong z)
		{
			X = x;
			Y = y;
			Z = z;
		}

        ///<summary>Creates a new instance of <see cref = "Vector3UL"/>.</summary>
		public Vector3UL(ulong value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector3UL"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y and Z components of the vector. This must be an array with 3 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public Vector3UL(ulong[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 3)
                throw new ArgumentOutOfRangeException("values", "There must be 3 and only 3 input values for Vector3UL.");

			X = values[0];
			Y = values[1];
			Z = values[2];
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector3UL"/> struct from an unsafe pointer. The pointer should point to an array of three elements.
        /// </summary>
		public unsafe Vector3UL(ulong* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
			Z = ptr[2];
		}
#endregion

#region Instance Methods
        /// <summary>
        /// Determines whether the specified <see cref="Vector3UL"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector3UL"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector3UL"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Vector3UL other)
        {
            return MathHelperDP.NearEqual(other.X, X) && MathHelperDP.NearEqual(other.Y, Y) && MathHelperDP.NearEqual(other.Z, Z);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3UL"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector3UL"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector3UL"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3UL other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3UL"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector3UL"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector3UL"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is Vector3UL))
                return false;

            var strongValue = (Vector3UL)value;
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
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
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
            return (ulong)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
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
            return ((X * X) + (Y * Y) + (Z * Z));
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
			    Z = (ulong)(Z * inverse);
            }
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="Vector3UL"/>.
        /// </summary>
        /// <returns>A three-element array containing the components of the vector.</returns>
        public ulong[] ToArray()
        {
            return new ulong[] { X, Y, Z};
        }
		
        /// <summary>
        /// Returns a normalized unit vector of the original vector.
        /// </summary>
        public Vector3UL GetNormalized()
        {
            double length = Length();
            if (!MathHelperDP.IsZero(length))
            {
                double inverse = 1.0D / length;
                return new Vector3UL()
                {
			        X = (ulong)(this.X * inverse),
			        Y = (ulong)(this.Y * inverse),
			        Z = (ulong)(this.Z * inverse),
                };
            }
            else
            {
                return new Vector3UL();
            }
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(ulong min, ulong max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
			Z = Z < min ? min : Z > max ? max : Z;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(Vector3UL min, Vector3UL max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
			Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3UL"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider));
        }
#endregion

#region Add operators
        public static void Add(ref Vector3UL left, ref Vector3UL right, out Vector3UL result)
        {
			result.X = (left.X + right.X);
			result.Y = (left.Y + right.Y);
			result.Z = (left.Z + right.Z);
        }

        public static void Add(ref Vector3UL left, ulong right, out Vector3UL result)
        {
			result.X = (left.X + right);
			result.Y = (left.Y + right);
			result.Z = (left.Z + right);
        }

		public static Vector3UL operator +(Vector3UL left, Vector3UL right)
		{
			Add(ref left, ref right, out Vector3UL result);
            return result;
		}

		public static Vector3UL operator +(Vector3UL left, ulong right)
		{
            Add(ref left, right, out Vector3UL result);
            return result;
		}

        public static Vector3UL operator +(ulong left, Vector3UL right)
		{
            Add(ref right, left, out Vector3UL result);
            return result;
		}

		/// <summary>
        /// Assert a <see cref="Vector3UL"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="Vector3UL"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="Vector3UL"/>.</returns>
        public static Vector3UL operator +(Vector3UL value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static void Subtract(ref Vector3UL left, ref Vector3UL right, out Vector3UL result)
        {
			result.X = (left.X - right.X);
			result.Y = (left.Y - right.Y);
			result.Z = (left.Z - right.Z);
        }

        public static void Subtract(ref Vector3UL left, ulong right, out Vector3UL result)
        {
			result.X = (left.X - right);
			result.Y = (left.Y - right);
			result.Z = (left.Z - right);
        }

		public static Vector3UL operator -(Vector3UL left, Vector3UL right)
		{
			Subtract(ref left, ref right, out Vector3UL result);
            return result;
		}

		public static Vector3UL operator -(Vector3UL left, ulong right)
		{
            Subtract(ref left, right, out Vector3UL result);
            return result;
		}

        public static Vector3UL operator -(ulong left, Vector3UL right)
		{
            Subtract(ref right, left, out Vector3UL result);
            return result;
		}

#endregion

#region division operators
		public static void Divide(ref Vector3UL left, ref Vector3UL right, out Vector3UL result)
        {
			result.X = (left.X / right.X);
			result.Y = (left.Y / right.Y);
			result.Z = (left.Z / right.Z);
        }

        public static void Divide(ref Vector3UL left, ulong right, out Vector3UL result)
        {
			result.X = (left.X / right);
			result.Y = (left.Y / right);
			result.Z = (left.Z / right);
        }

		public static Vector3UL operator /(Vector3UL left, Vector3UL right)
		{
			Divide(ref left, ref right, out Vector3UL result);
            return result;
		}

		public static Vector3UL operator /(Vector3UL left, ulong right)
		{
            Divide(ref left, right, out Vector3UL result);
            return result;
		}

        public static Vector3UL operator /(ulong left, Vector3UL right)
		{
            Divide(ref right, left, out Vector3UL result);
            return result;
		}
#endregion

#region Multiply operators
		public static void Multiply(ref Vector3UL left, ref Vector3UL right, out Vector3UL result)
        {
			result.X = (left.X * right.X);
			result.Y = (left.Y * right.Y);
			result.Z = (left.Z * right.Z);
        }

        public static void Multiply(ref Vector3UL left, ulong right, out Vector3UL result)
        {
			result.X = (left.X * right);
			result.Y = (left.Y * right);
			result.Z = (left.Z * right);
        }

		public static Vector3UL operator *(Vector3UL left, Vector3UL right)
		{
			Multiply(ref left, ref right, out Vector3UL result);
            return result;
		}

		public static Vector3UL operator *(Vector3UL left, ulong right)
		{
            Multiply(ref left, right, out Vector3UL result);
            return result;
		}

        public static Vector3UL operator *(ulong left, Vector3UL right)
		{
            Multiply(ref right, left, out Vector3UL result);
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
        public static bool operator ==(Vector3UL left, Vector3UL right)
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
        public static bool operator !=(Vector3UL left, Vector3UL right)
        {
            return !left.Equals(ref right);
        }
#endregion

#region Operators - Cast
        ///<summary>Casts a <see cref="Vector3UL"/> to a <see cref="Vector2UL"/>.</summary>
        public static explicit operator Vector2UL(Vector3UL value)
        {
            return new Vector2UL(value.X, value.Y);
        }

        ///<summary>Casts a <see cref="Vector3UL"/> to a <see cref="Vector4UL"/>.</summary>
        public static explicit operator Vector4UL(Vector3UL value)
        {
            return new Vector4UL(value.X, value.Y, value.Z, 0);
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
        public static bool NearEqual(Vector3UL left, Vector3UL right, Vector3UL epsilon)
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
        public static bool NearEqual(ref Vector3UL left, ref Vector3UL right, ref Vector3UL epsilon)
        {
            return MathHelperDP.WithinEpsilon(left.X, right.X, epsilon.X) && MathHelperDP.WithinEpsilon(left.Y, right.Y, epsilon.Y) && MathHelperDP.WithinEpsilon(left.Z, right.Z, epsilon.Z);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector3UL SmoothStep(ref Vector3UL start, ref Vector3UL end, double amount)
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
        public static Vector3UL SmoothStep(Vector3UL start, Vector3UL end, ulong amount)
        {
            return SmoothStep(ref start, ref end, amount);
        }    

        /// <summary>
        /// Orthogonalizes a list of <see cref="Vector3UL"/>.
        /// </summary>
        /// <param name="destination">The list of orthogonalized <see cref="Vector3UL"/>.</param>
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
        public static void Orthogonalize(Vector3UL[] destination, params Vector3UL[] source)
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
                Vector3UL newvector = source[i];

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
        public static void Orthonormalize(Vector3UL[] destination, params Vector3UL[] source)
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
                Vector3UL newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= Dot(destination[r], newvector) * destination[r];

                newvector.Normalize();
                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Vector3UL"/>. <para />
        /// For example, a swizzle input of (1,1) on a <see cref="Vector3UL"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
        /// </summary>
        /// <param name="val">The current vector.</param>
		/// <param name="xIndex">The axis index to use for the new X value.</param>
		/// <param name="yIndex">The axis index to use for the new Y value.</param>
		/// <param name="zIndex">The axis index to use for the new Z value.</param>
        /// <returns></returns>
        public static unsafe Vector3UL Swizzle(Vector3UL val, int xIndex, int yIndex, int zIndex)
        {
            return new Vector3UL()
            {
			   X = (&val.X)[xIndex],
			   Y = (&val.X)[yIndex],
			   Z = (&val.X)[zIndex],
            };
        }

        /// <returns></returns>
        public static unsafe Vector3UL Swizzle(Vector3UL val, uint xIndex, uint yIndex, uint zIndex)
        {
            return new Vector3UL()
            {
			    X = (&val.X)[xIndex],
			    Y = (&val.X)[yIndex],
			    Z = (&val.X)[zIndex],
            };
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector3UL.DistanceSquared(Vector3UL, Vector3UL)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static ulong Distance(ref Vector3UL value1, ref Vector3UL value2)
        {
			ulong x = (value1.X - value2.X);
			ulong y = (value1.Y - value2.Y);
			ulong z = (value1.Z - value2.Z);

            return (ulong)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector3UL.DistanceSquared(Vector3UL, Vector3UL)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static ulong Distance(Vector3UL value1, Vector3UL value2)
        {
			ulong x = (value1.X - value2.X);
			ulong y = (value1.Y - value2.Y);
			ulong z = (value1.Z - value2.Z);

            return (ulong)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>Checks to see if any value (x, y, z, w) are within 0.0001 of 0.
        /// If so this method truncates that value to zero.</summary>
        /// <param name="power">The power.</param>
        /// <param name="vec">The vector.</param>
        public static Vector3UL Pow(Vector3UL vec, ulong power)
        {
            return new Vector3UL()
            {
				X = (ulong)Math.Pow(vec.X, power),
				Y = (ulong)Math.Pow(vec.Y, power),
				Z = (ulong)Math.Pow(vec.Z, power),
            };
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector3UL"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector3UL"/> source vector</param>
        /// <param name="right">Second <see cref="Vector3UL"/> source vector.</param>
        public static ulong Dot(ref Vector3UL left, ref Vector3UL right)
        {
			return ((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z));
        }

		/// <summary>
        /// Calculates the dot product of two <see cref="Vector3UL"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector3UL"/> source vector</param>
        /// <param name="right">Second <see cref="Vector3UL"/> source vector.</param>
        public static ulong Dot(Vector3UL left, Vector3UL right)
        {
			return ((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z));
        }

		/// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector3UL"/> vector.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector3UL"/> vector.</param>
        /// <param name="value2">Second source position <see cref="Vector3UL"/> vector.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector3UL"/> vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector3UL Hermite(ref Vector3UL value1, ref Vector3UL tangent1, ref Vector3UL value2, ref Vector3UL tangent2, ulong amount)
        {
            double squared = amount * amount;
            double cubed = amount * squared;
            double part1 = ((2.0D * cubed) - (3.0D * squared)) + 1.0D;
            double part2 = (-2.0D * cubed) + (3.0D * squared);
            double part3 = (cubed - (2.0D * squared)) + amount;
            double part4 = cubed - squared;

			return new Vector3UL()
			{
				X = (ulong)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4)),
				Y = (ulong)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4)),
				Z = (ulong)((((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4)),
			};
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector3UL"/>.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector3UL"/>.</param>
        /// <param name="value2">Second source position <see cref="Vector3UL"/>.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector3UL"/>.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Vector3UL Hermite(Vector3UL value1, Vector3UL tangent1, Vector3UL value2, Vector3UL tangent2, ulong amount)
        {
            return Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount);
        }

		/// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector3UL"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector3UL"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector3UL"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector3UL Barycentric(ref Vector3UL value1, ref Vector3UL value2, ref Vector3UL value3, ulong amount1, ulong amount2)
        {
			return new Vector3UL(
				((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))), 
				((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y))), 
				((value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)))
			);
        }

        /// <summary>
        /// Performs a linear interpolation between two <see cref="Vector3UL"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref Vector3UL start, ref Vector3UL end, double amount, out Vector3UL result)
        {
			result.X = (ulong)((1D - amount) * start.X + amount * end.X);
			result.Y = (ulong)((1D - amount) * start.Y + amount * end.Y);
			result.Z = (ulong)((1D - amount) * start.Z + amount * end.Z);
        }

		/// <summary>
        /// Performs a linear interpolation between two <see cref="Vector3UL"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector3UL Lerp(ref Vector3UL start, ref Vector3UL end, double amount)
        {
			return new Vector3UL()
			{
				X = (ulong)((1D - amount) * start.X + amount * end.X),
				Y = (ulong)((1D - amount) * start.Y + amount * end.Y),
				Z = (ulong)((1D - amount) * start.Z + amount * end.Z),
			};
        }

        /// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector3UL"/>.</param>
        /// <returns>A <see cref="Vector3UL"/> containing the smallest components of the source vectors.</returns>
		public static void Min(ref Vector3UL left, ref Vector3UL right, out Vector3UL result)
		{
				result.X = (left.X < right.X) ? left.X : right.X;
				result.Y = (left.Y < right.Y) ? left.Y : right.Y;
				result.Z = (left.Z < right.Z) ? left.Z : right.Z;
		}

        /// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector3UL"/>.</param>
        /// <returns>A <see cref="Vector3UL"/> containing the smallest components of the source vectors.</returns>
		public static Vector3UL Min(ref Vector3UL left, ref Vector3UL right)
		{
			Min(ref left, ref right, out Vector3UL result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector3UL"/>.</param>
        /// <returns>A <see cref="Vector3UL"/> containing the smallest components of the source vectors.</returns>
		public static Vector3UL Min(Vector3UL left, Vector3UL right)
		{
			return new Vector3UL()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
				Z = (left.Z < right.Z) ? left.Z : right.Z,
			};
		}

        /// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector3UL"/>.</param>
        /// <returns>A <see cref="Vector3UL"/> containing the largest components of the source vectors.</returns>
		public static void Max(ref Vector3UL left, ref Vector3UL right, out Vector3UL result)
		{
				result.X = (left.X > right.X) ? left.X : right.X;
				result.Y = (left.Y > right.Y) ? left.Y : right.Y;
				result.Z = (left.Z > right.Z) ? left.Z : right.Z;
		}

        /// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector3UL"/>.</param>
        /// <returns>A <see cref="Vector3UL"/> containing the largest components of the source vectors.</returns>
		public static Vector3UL Max(ref Vector3UL left, ref Vector3UL right)
		{
			Max(ref left, ref right, out Vector3UL result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector3UL"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3UL"/>.</param>
        /// <param name="right">The second source <see cref="Vector3UL"/>.</param>
        /// <returns>A <see cref="Vector3UL"/> containing the largest components of the source vectors.</returns>
		public static Vector3UL Max(Vector3UL left, Vector3UL right)
		{
			return new Vector3UL()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
				Z = (left.Z > right.Z) ? left.Z : right.Z,
			};
		}

		/// <summary>
        /// Calculates the squared distance between two <see cref="Vector3UL"/> vectors.
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
		public static ulong DistanceSquared(ref Vector3UL value1, ref Vector3UL value2)
        {
            ulong x = value1.X - value2.X;
            ulong y = value1.Y - value2.Y;
            ulong z = value1.Z - value2.Z;

            return ((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the squared distance between two <see cref="Vector3UL"/> vectors.
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
		public static ulong DistanceSquared(Vector3UL value1, Vector3UL value2)
        {
            ulong x = value1.X - value2.X;
            ulong y = value1.Y - value2.Y;
            ulong z = value1.Z - value2.Z;

            return ((x * x) + (y * y) + (z * z));
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector3UL"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector3UL Clamp(Vector3UL value, ulong min, ulong max)
        {
			return new Vector3UL()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
				Z = value.Z < min ? min : value.Z > max ? max : value.Z,
			};
        }

        /// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector3UL"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static void Clamp(ref Vector3UL value, ref Vector3UL min, ref Vector3UL max, out Vector3UL result)
        {
				result.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
				result.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
				result.Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector3UL"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector3UL Clamp(Vector3UL value, Vector3UL min, Vector3UL max)
        {
			return new Vector3UL()
			{
				X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
				Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
				Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z,
			};
        }


        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal. 
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.</remarks>
        public static Vector3UL Reflect(ref Vector3UL vector, ref Vector3UL normal)
        {
            ulong dot = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z);

            return new Vector3UL()
            {
				X = (ulong)(vector.X - ((2.0D * dot) * normal.X)),
				Y = (ulong)(vector.Y - ((2.0D * dot) * normal.Y)),
				Z = (ulong)(vector.Z - ((2.0D * dot) * normal.Z)),
            };
        }

        /// <summary>
        /// Converts the <see cref="Vector3UL"/> into a unit vector.
        /// </summary>
        /// <param name="value">The <see cref="Vector3UL"/> to normalize.</param>
        /// <returns>The normalized <see cref="Vector3UL"/>.</returns>
        public static Vector3UL Normalize(Vector3UL value)
        {
            value.Normalize();
            return value;
        }
#endregion

#region Indexers
		/// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y or Z component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component and so on.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
        
		public ulong this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector3UL run from 0 to 2, inclusive.");
			}

			set
			{
				switch(index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector3UL run from 0 to 2, inclusive.");
			}
		}
#endregion
	}
}

