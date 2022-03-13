using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Molten
{
	///<summary>A <see cref = "byte"/> vector comprised of three components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public partial struct Byte3 : IFormattable
	{
		///<summary>The X component.</summary>
		public byte X;

		///<summary>The Y component.</summary>
		public byte Y;

		///<summary>The Z component.</summary>
		public byte Z;


		///<summary>The size of <see cref="Byte3"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Byte3));

		///<summary>A Byte3 with every component set to 1.</summary>
		public static readonly Byte3 One = new Byte3(1, 1, 1);

		/// <summary>The X unit <see cref="Byte3"/>.</summary>
		public static readonly Byte3 UnitX = new Byte3(1, 0, 0);

		/// <summary>The Y unit <see cref="Byte3"/>.</summary>
		public static readonly Byte3 UnitY = new Byte3(0, 1, 0);

		/// <summary>The Z unit <see cref="Byte3"/>.</summary>
		public static readonly Byte3 UnitZ = new Byte3(0, 0, 1);

		/// <summary>Represents a zero'd Byte3.</summary>
		public static readonly Byte3 Zero = new Byte3(0, 0, 0);

		 /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get => MathHelper.IsOne((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        public bool IsZero
        {
            get => X == 0 && Y == 0 && Z == 0;
        }

#region Constructors
        ///<summary>Creates a new instance of <see cref = "Byte3"/>, using a <see cref="Byte2"/> to populate the first two components.</summary>
		public Byte3(Byte2 vector, byte z)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
		}

		///<summary>Creates a new instance of <see cref = "Byte3"/>.</summary>
		public Byte3(byte x, byte y, byte z)
		{
			X = x;
			Y = y;
			Z = z;
		}

        ///<summary>Creates a new instance of <see cref = "Byte3"/>.</summary>
		public Byte3(byte value)
		{
			X = value;
			Y = value;
			Z = value;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="Byte3"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y and Z components of the vector. This must be an array with 3 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public Byte3(byte[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 3)
                throw new ArgumentOutOfRangeException("values", "There must be 3 and only 3 input values for Byte3.");

			X = values[0];
			Y = values[1];
			Z = values[2];
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Byte3"/> struct from an unsafe pointer. The pointer should point to an array of three elements.
        /// </summary>
		public unsafe Byte3(byte* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
			Z = ptr[2];
		}
#endregion

#region Instance Methods
        /// <summary>
        /// Determines whether the specified <see cref="Byte3"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Byte3"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Byte3"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Byte3 other)
        {
            return MathHelper.NearEqual(other.X, X) && MathHelper.NearEqual(other.Y, Y) && MathHelper.NearEqual(other.Z, Z);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Byte3"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Byte3"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Byte3"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Byte3 other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Byte3"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Byte3"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Byte3"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is Byte3))
                return false;

            var strongValue = (Byte3)value;
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
        public byte Length()
        {
            return (byte)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Calculates the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public byte LengthSquared()
        {
            return (byte)((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        public void Normalize()
        {
            byte length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
			    X = (byte)(X * inverse);
			    Y = (byte)(Y * inverse);
			    Z = (byte)(Z * inverse);
            }
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="Byte3"/>.
        /// </summary>
        /// <returns>A three-element array containing the components of the vector.</returns>
        public byte[] ToArray()
        {
            return new byte[] { X, Y, Z};
        }
		
        /// <summary>
        /// Returns a normalized unit vector of the original vector.
        /// </summary>
        public Byte3 GetNormalized()
        {
            float length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
                return new Byte3()
                {
			        X = (byte)(this.X * inverse),
			        Y = (byte)(this.Y * inverse),
			        Z = (byte)(this.Z * inverse),
                };
            }
            else
            {
                return new Byte3();
            }
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(byte min, byte max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
			Z = Z < min ? min : Z > max ? max : Z;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(Byte3 min, Byte3 max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
			Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Byte3"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider));
        }
#endregion

#region Add operators
        public static void Add(ref Byte3 left, ref Byte3 right, out Byte3 result)
        {
			result.X = (byte)(left.X + right.X);
			result.Y = (byte)(left.Y + right.Y);
			result.Z = (byte)(left.Z + right.Z);
        }

        public static void Add(ref Byte3 left, byte right, out Byte3 result)
        {
			result.X = (byte)(left.X + right);
			result.Y = (byte)(left.Y + right);
			result.Z = (byte)(left.Z + right);
        }

		public static Byte3 operator +(Byte3 left, Byte3 right)
		{
			Add(ref left, ref right, out Byte3 result);
            return result;
		}

		public static Byte3 operator +(Byte3 left, byte right)
		{
            Add(ref left, right, out Byte3 result);
            return result;
		}

        public static Byte3 operator +(byte left, Byte3 right)
		{
            Add(ref right, left, out Byte3 result);
            return result;
		}

		/// <summary>
        /// Assert a <see cref="Byte3"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="Byte3"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="Byte3"/>.</returns>
        public static Byte3 operator +(Byte3 value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static void Subtract(ref Byte3 left, ref Byte3 right, out Byte3 result)
        {
			result.X = (byte)(left.X - right.X);
			result.Y = (byte)(left.Y - right.Y);
			result.Z = (byte)(left.Z - right.Z);
        }

        public static void Subtract(ref Byte3 left, byte right, out Byte3 result)
        {
			result.X = (byte)(left.X - right);
			result.Y = (byte)(left.Y - right);
			result.Z = (byte)(left.Z - right);
        }

		public static Byte3 operator -(Byte3 left, Byte3 right)
		{
			Subtract(ref left, ref right, out Byte3 result);
            return result;
		}

		public static Byte3 operator -(Byte3 left, byte right)
		{
            Subtract(ref left, right, out Byte3 result);
            return result;
		}

        public static Byte3 operator -(byte left, Byte3 right)
		{
            Subtract(ref right, left, out Byte3 result);
            return result;
		}

#endregion

#region division operators
		public static void Divide(ref Byte3 left, ref Byte3 right, out Byte3 result)
        {
			result.X = (byte)(left.X / right.X);
			result.Y = (byte)(left.Y / right.Y);
			result.Z = (byte)(left.Z / right.Z);
        }

        public static void Divide(ref Byte3 left, byte right, out Byte3 result)
        {
			result.X = (byte)(left.X / right);
			result.Y = (byte)(left.Y / right);
			result.Z = (byte)(left.Z / right);
        }

		public static Byte3 operator /(Byte3 left, Byte3 right)
		{
			Divide(ref left, ref right, out Byte3 result);
            return result;
		}

		public static Byte3 operator /(Byte3 left, byte right)
		{
            Divide(ref left, right, out Byte3 result);
            return result;
		}

        public static Byte3 operator /(byte left, Byte3 right)
		{
            Divide(ref right, left, out Byte3 result);
            return result;
		}
#endregion

#region Multiply operators
		public static void Multiply(ref Byte3 left, ref Byte3 right, out Byte3 result)
        {
			result.X = (byte)(left.X * right.X);
			result.Y = (byte)(left.Y * right.Y);
			result.Z = (byte)(left.Z * right.Z);
        }

        public static void Multiply(ref Byte3 left, byte right, out Byte3 result)
        {
			result.X = (byte)(left.X * right);
			result.Y = (byte)(left.Y * right);
			result.Z = (byte)(left.Z * right);
        }

		public static Byte3 operator *(Byte3 left, Byte3 right)
		{
			Multiply(ref left, ref right, out Byte3 result);
            return result;
		}

		public static Byte3 operator *(Byte3 left, byte right)
		{
            Multiply(ref left, right, out Byte3 result);
            return result;
		}

        public static Byte3 operator *(byte left, Byte3 right)
		{
            Multiply(ref right, left, out Byte3 result);
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
        public static bool operator ==(Byte3 left, Byte3 right)
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
        public static bool operator !=(Byte3 left, Byte3 right)
        {
            return !left.Equals(ref right);
        }
#endregion

#region Operators - Cast
        ///<summary>Casts a <see cref="Byte3"/> to a <see cref="Byte2"/>.</summary>
        public static explicit operator Byte2(Byte3 value)
        {
            return new Byte2(value.X, value.Y);
        }

        ///<summary>Casts a <see cref="Byte3"/> to a <see cref="Byte4"/>.</summary>
        public static explicit operator Byte4(Byte3 value)
        {
            return new Byte4(value.X, value.Y, value.Z, 0);
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
        public static bool NearEqual(Byte3 left, Byte3 right, Byte3 epsilon)
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
        public static bool NearEqual(ref Byte3 left, ref Byte3 right, ref Byte3 epsilon)
        {
            return MathHelper.WithinEpsilon(left.X, right.X, epsilon.X) && MathHelper.WithinEpsilon(left.Y, right.Y, epsilon.Y) && MathHelper.WithinEpsilon(left.Z, right.Z, epsilon.Z);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Byte3 SmoothStep(ref Byte3 start, ref Byte3 end, float amount)
        {
            amount = MathHelper.SmoothStep(amount);
            return Lerp(ref start, ref end, amount);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two vectors.</returns>
        public static Byte3 SmoothStep(Byte3 start, Byte3 end, byte amount)
        {
            return SmoothStep(ref start, ref end, amount);
        }    

        /// <summary>
        /// Orthogonalizes a list of <see cref="Byte3"/>.
        /// </summary>
        /// <param name="destination">The list of orthogonalized <see cref="Byte3"/>.</param>
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
        public static void Orthogonalize(Byte3[] destination, params Byte3[] source)
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
                Byte3 newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= (byte)(Dot(destination[r], newvector) / Dot(destination[r], destination[r])) * destination[r];

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
        public static void Orthonormalize(Byte3[] destination, params Byte3[] source)
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
                Byte3 newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= Dot(destination[r], newvector) * destination[r];

                newvector.Normalize();
                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Byte3"/>. <para />
        /// For example, a swizzle input of (1,1) on a <see cref="Byte3"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
        /// </summary>
        /// <param name="val">The current vector.</param>
		/// <param name="xIndex">The axis index to use for the new X value.</param>
		/// <param name="yIndex">The axis index to use for the new Y value.</param>
		/// <param name="zIndex">The axis index to use for the new Z value.</param>
        /// <returns></returns>
        public static unsafe Byte3 Swizzle(Byte3 val, int xIndex, int yIndex, int zIndex)
        {
            return new Byte3()
            {
			   X = (&val.X)[xIndex],
			   Y = (&val.X)[yIndex],
			   Z = (&val.X)[zIndex],
            };
        }

        /// <returns></returns>
        public static unsafe Byte3 Swizzle(Byte3 val, uint xIndex, uint yIndex, uint zIndex)
        {
            return new Byte3()
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
        /// <see cref="Byte3.DistanceSquared(Byte3, Byte3)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static byte Distance(ref Byte3 value1, ref Byte3 value2)
        {
			byte x = (byte)(value1.X - value2.X);
			byte y = (byte)(value1.Y - value2.Y);
			byte z = (byte)(value1.Z - value2.Z);

            return (byte)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Byte3.DistanceSquared(Byte3, Byte3)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static byte Distance(Byte3 value1, Byte3 value2)
        {
			byte x = (byte)(value1.X - value2.X);
			byte y = (byte)(value1.Y - value2.Y);
			byte z = (byte)(value1.Z - value2.Z);

            return (byte)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>Checks to see if any value (x, y, z, w) are within 0.0001 of 0.
        /// If so this method truncates that value to zero.</summary>
        /// <param name="power">The power.</param>
        /// <param name="vec">The vector.</param>
        public static Byte3 Pow(Byte3 vec, byte power)
        {
            return new Byte3()
            {
				X = (byte)Math.Pow(vec.X, power),
				Y = (byte)Math.Pow(vec.Y, power),
				Z = (byte)Math.Pow(vec.Z, power),
            };
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Byte3"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Byte3"/> source vector</param>
        /// <param name="right">Second <see cref="Byte3"/> source vector.</param>
        public static byte Dot(ref Byte3 left, ref Byte3 right)
        {
			return (byte)(((byte)left.X * right.X) + ((byte)left.Y * right.Y) + ((byte)left.Z * right.Z));
        }

		/// <summary>
        /// Calculates the dot product of two <see cref="Byte3"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Byte3"/> source vector</param>
        /// <param name="right">Second <see cref="Byte3"/> source vector.</param>
        public static byte Dot(Byte3 left, Byte3 right)
        {
			return (byte)((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z));
        }

		/// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Byte3"/> vector.</param>
        /// <param name="tangent1">First source tangent <see cref="Byte3"/> vector.</param>
        /// <param name="value2">Second source position <see cref="Byte3"/> vector.</param>
        /// <param name="tangent2">Second source tangent <see cref="Byte3"/> vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Byte3 Hermite(ref Byte3 value1, ref Byte3 tangent1, ref Byte3 value2, ref Byte3 tangent2, byte amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0F * cubed) - (3.0F * squared)) + 1.0F;
            float part2 = (-2.0F * cubed) + (3.0F * squared);
            float part3 = (cubed - (2.0F * squared)) + amount;
            float part4 = cubed - squared;

			return new Byte3()
			{
				X = (byte)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4)),
				Y = (byte)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4)),
				Z = (byte)((((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4)),
			};
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Byte3"/>.</param>
        /// <param name="tangent1">First source tangent <see cref="Byte3"/>.</param>
        /// <param name="value2">Second source position <see cref="Byte3"/>.</param>
        /// <param name="tangent2">Second source tangent <see cref="Byte3"/>.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Byte3 Hermite(Byte3 value1, Byte3 tangent1, Byte3 value2, Byte3 tangent2, byte amount)
        {
            return Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount);
        }

		/// <summary>
        /// Returns a <see cref="Byte3"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Byte3"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Byte3"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Byte3"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Byte3 Barycentric(ref Byte3 value1, ref Byte3 value2, ref Byte3 value3, byte amount1, byte amount2)
        {
			return new Byte3(
				(byte)((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))), 
				(byte)((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y))), 
				(byte)((value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)))
			);
        }

        /// <summary>
        /// Performs a linear interpolation between two <see cref="Byte3"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref Byte3 start, ref Byte3 end, float amount, out Byte3 result)
        {
			result.X = (byte)((1F - amount) * start.X + amount * end.X);
			result.Y = (byte)((1F - amount) * start.Y + amount * end.Y);
			result.Z = (byte)((1F - amount) * start.Z + amount * end.Z);
        }

		/// <summary>
        /// Performs a linear interpolation between two <see cref="Byte3"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Byte3 Lerp(ref Byte3 start, ref Byte3 end, float amount)
        {
			return new Byte3()
			{
				X = (byte)((1F - amount) * start.X + amount * end.X),
				Y = (byte)((1F - amount) * start.Y + amount * end.Y),
				Z = (byte)((1F - amount) * start.Z + amount * end.Z),
			};
        }

        /// <summary>
        /// Returns a <see cref="Byte3"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Byte3"/>.</param>
        /// <param name="right">The second source <see cref="Byte3"/>.</param>
        /// <returns>A <see cref="Byte3"/> containing the smallest components of the source vectors.</returns>
		public static void Min(ref Byte3 left, ref Byte3 right, out Byte3 result)
		{
				result.X = (left.X < right.X) ? left.X : right.X;
				result.Y = (left.Y < right.Y) ? left.Y : right.Y;
				result.Z = (left.Z < right.Z) ? left.Z : right.Z;
		}

        /// <summary>
        /// Returns a <see cref="Byte3"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Byte3"/>.</param>
        /// <param name="right">The second source <see cref="Byte3"/>.</param>
        /// <returns>A <see cref="Byte3"/> containing the smallest components of the source vectors.</returns>
		public static Byte3 Min(ref Byte3 left, ref Byte3 right)
		{
			Min(ref left, ref right, out Byte3 result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Byte3"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Byte3"/>.</param>
        /// <param name="right">The second source <see cref="Byte3"/>.</param>
        /// <returns>A <see cref="Byte3"/> containing the smallest components of the source vectors.</returns>
		public static Byte3 Min(Byte3 left, Byte3 right)
		{
			return new Byte3()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
				Z = (left.Z < right.Z) ? left.Z : right.Z,
			};
		}

        /// <summary>
        /// Returns a <see cref="Byte3"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Byte3"/>.</param>
        /// <param name="right">The second source <see cref="Byte3"/>.</param>
        /// <returns>A <see cref="Byte3"/> containing the largest components of the source vectors.</returns>
		public static void Max(ref Byte3 left, ref Byte3 right, out Byte3 result)
		{
				result.X = (left.X > right.X) ? left.X : right.X;
				result.Y = (left.Y > right.Y) ? left.Y : right.Y;
				result.Z = (left.Z > right.Z) ? left.Z : right.Z;
		}

        /// <summary>
        /// Returns a <see cref="Byte3"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Byte3"/>.</param>
        /// <param name="right">The second source <see cref="Byte3"/>.</param>
        /// <returns>A <see cref="Byte3"/> containing the largest components of the source vectors.</returns>
		public static Byte3 Max(ref Byte3 left, ref Byte3 right)
		{
			Max(ref left, ref right, out Byte3 result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Byte3"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Byte3"/>.</param>
        /// <param name="right">The second source <see cref="Byte3"/>.</param>
        /// <returns>A <see cref="Byte3"/> containing the largest components of the source vectors.</returns>
		public static Byte3 Max(Byte3 left, Byte3 right)
		{
			return new Byte3()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
				Z = (left.Z > right.Z) ? left.Z : right.Z,
			};
		}

		/// <summary>
        /// Calculates the squared distance between two <see cref="Byte3"/> vectors.
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
		public static byte DistanceSquared(ref Byte3 value1, ref Byte3 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            return (byte)((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        /// Calculates the squared distance between two <see cref="Byte3"/> vectors.
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
		public static byte DistanceSquared(Byte3 value1, Byte3 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            return (byte)((x * x) + (y * y) + (z * z));
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Byte3"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Byte3 Clamp(Byte3 value, byte min, byte max)
        {
			return new Byte3()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
				Z = value.Z < min ? min : value.Z > max ? max : value.Z,
			};
        }

        /// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Byte3"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static void Clamp(ref Byte3 value, ref Byte3 min, ref Byte3 max, out Byte3 result)
        {
				result.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
				result.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
				result.Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Byte3"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Byte3 Clamp(Byte3 value, Byte3 min, Byte3 max)
        {
			return new Byte3()
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
        public static Byte3 Reflect(ref Byte3 vector, ref Byte3 normal)
        {
            int dot = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z);

            return new Byte3()
            {
				X = (byte)(vector.X - ((2.0F * dot) * normal.X)),
				Y = (byte)(vector.Y - ((2.0F * dot) * normal.Y)),
				Z = (byte)(vector.Z - ((2.0F * dot) * normal.Z)),
            };
        }

        /// <summary>
        /// Converts the <see cref="Byte3"/> into a unit vector.
        /// </summary>
        /// <param name="value">The <see cref="Byte3"/> to normalize.</param>
        /// <returns>The normalized <see cref="Byte3"/>.</returns>
        public static Byte3 Normalize(Byte3 value)
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
        
		public byte this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Byte3 run from 0 to 2, inclusive.");
			}

			set
			{
				switch(index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Byte3 run from 0 to 2, inclusive.");
			}
		}
#endregion
	}
}

