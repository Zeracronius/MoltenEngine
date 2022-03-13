using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Molten
{
	///<summary>A <see cref = "ushort"/> vector comprised of four components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=2)]
	public partial struct Vector4US : IFormattable
	{
		///<summary>The X component.</summary>
		public ushort X;

		///<summary>The Y component.</summary>
		public ushort Y;

		///<summary>The Z component.</summary>
		public ushort Z;

		///<summary>The W component.</summary>
		public ushort W;


		///<summary>The size of <see cref="Vector4US"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector4US));

		///<summary>A Vector4US with every component set to (ushort)1.</summary>
		public static readonly Vector4US One = new Vector4US((ushort)1, (ushort)1, (ushort)1, (ushort)1);

		/// <summary>The X unit <see cref="Vector4US"/>.</summary>
		public static readonly Vector4US UnitX = new Vector4US((ushort)1, 0, 0, 0);

		/// <summary>The Y unit <see cref="Vector4US"/>.</summary>
		public static readonly Vector4US UnitY = new Vector4US(0, (ushort)1, 0, 0);

		/// <summary>The Z unit <see cref="Vector4US"/>.</summary>
		public static readonly Vector4US UnitZ = new Vector4US(0, 0, (ushort)1, 0);

		/// <summary>The W unit <see cref="Vector4US"/>.</summary>
		public static readonly Vector4US UnitW = new Vector4US(0, 0, 0, (ushort)1);

		/// <summary>Represents a zero'd Vector4US.</summary>
		public static readonly Vector4US Zero = new Vector4US(0, 0, 0, 0);

		 /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get => MathHelper.IsOne((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        public bool IsZero
        {
            get => X == 0 && Y == 0 && Z == 0 && W == 0;
        }

#region Constructors
        ///<summary>Creates a new instance of <see cref = "Vector4US"/>, using a <see cref="Vector2US"/> to populate the first two components.</summary>
		public Vector4US(Vector2US vector, ushort z, ushort w)
		{
			X = vector.X;
			Y = vector.Y;
			Z = z;
			W = w;
		}
        ///<summary>Creates a new instance of <see cref = "Vector4US"/>, using a <see cref="Vector3US"/> to populate the first three components.</summary>
		public Vector4US(Vector3US vector, ushort w)
		{
			X = vector.X;
			Y = vector.Y;
			Z = vector.Z;
			W = w;
		}

		///<summary>Creates a new instance of <see cref = "Vector4US"/>.</summary>
		public Vector4US(ushort x, ushort y, ushort z, ushort w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

        ///<summary>Creates a new instance of <see cref = "Vector4US"/>.</summary>
		public Vector4US(ushort value)
		{
			X = value;
			Y = value;
			Z = value;
			W = value;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector4US"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, Z and W components of the vector. This must be an array with 4 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public Vector4US(ushort[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be 4 and only 4 input values for Vector4US.");

			X = values[0];
			Y = values[1];
			Z = values[2];
			W = values[3];
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector4US"/> struct from an unsafe pointer. The pointer should point to an array of four elements.
        /// </summary>
		public unsafe Vector4US(ushort* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
			Z = ptr[2];
			W = ptr[3];
		}
#endregion

#region Instance Methods
        /// <summary>
        /// Determines whether the specified <see cref="Vector4US"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector4US"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector4US"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Vector4US other)
        {
            return MathHelper.NearEqual(other.X, X) && MathHelper.NearEqual(other.Y, Y) && MathHelper.NearEqual(other.Z, Z) && MathHelper.NearEqual(other.W, W);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector4US"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector4US"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector4US"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector4US other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector4US"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector4US"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector4US"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is Vector4US))
                return false;

            var strongValue = (Vector4US)value;
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
                hashCode = (hashCode * 397) ^ W.GetHashCode();
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
        public ushort Length()
        {
            return (ushort)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        /// <summary>
        /// Calculates the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public ushort LengthSquared()
        {
            return (ushort)((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        public void Normalize()
        {
            ushort length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
			    X = (ushort)(X * inverse);
			    Y = (ushort)(Y * inverse);
			    Z = (ushort)(Z * inverse);
			    W = (ushort)(W * inverse);
            }
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="Vector4US"/>.
        /// </summary>
        /// <returns>A four-element array containing the components of the vector.</returns>
        public ushort[] ToArray()
        {
            return new ushort[] { X, Y, Z, W};
        }
		
        /// <summary>
        /// Returns a normalized unit vector of the original vector.
        /// </summary>
        public Vector4US GetNormalized()
        {
            float length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
                return new Vector4US()
                {
			        X = (ushort)(this.X * inverse),
			        Y = (ushort)(this.Y * inverse),
			        Z = (ushort)(this.Z * inverse),
			        W = (ushort)(this.W * inverse),
                };
            }
            else
            {
                return new Vector4US();
            }
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(ushort min, ushort max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
			Z = Z < min ? min : Z > max ? max : Z;
			W = W < min ? min : W > max ? max : W;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(Vector4US min, Vector4US max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
			Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
			W = W < min.W ? min.W : W > max.W ? max.W : W;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture), W.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector4US"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider));
        }
#endregion

#region Add operators
        public static void Add(ref Vector4US left, ref Vector4US right, out Vector4US result)
        {
			result.X = (ushort)(left.X + right.X);
			result.Y = (ushort)(left.Y + right.Y);
			result.Z = (ushort)(left.Z + right.Z);
			result.W = (ushort)(left.W + right.W);
        }

        public static void Add(ref Vector4US left, ushort right, out Vector4US result)
        {
			result.X = (ushort)(left.X + right);
			result.Y = (ushort)(left.Y + right);
			result.Z = (ushort)(left.Z + right);
			result.W = (ushort)(left.W + right);
        }

		public static Vector4US operator +(Vector4US left, Vector4US right)
		{
			Add(ref left, ref right, out Vector4US result);
            return result;
		}

		public static Vector4US operator +(Vector4US left, ushort right)
		{
            Add(ref left, right, out Vector4US result);
            return result;
		}

        public static Vector4US operator +(ushort left, Vector4US right)
		{
            Add(ref right, left, out Vector4US result);
            return result;
		}

		/// <summary>
        /// Assert a <see cref="Vector4US"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="Vector4US"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="Vector4US"/>.</returns>
        public static Vector4US operator +(Vector4US value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static void Subtract(ref Vector4US left, ref Vector4US right, out Vector4US result)
        {
			result.X = (ushort)(left.X - right.X);
			result.Y = (ushort)(left.Y - right.Y);
			result.Z = (ushort)(left.Z - right.Z);
			result.W = (ushort)(left.W - right.W);
        }

        public static void Subtract(ref Vector4US left, ushort right, out Vector4US result)
        {
			result.X = (ushort)(left.X - right);
			result.Y = (ushort)(left.Y - right);
			result.Z = (ushort)(left.Z - right);
			result.W = (ushort)(left.W - right);
        }

		public static Vector4US operator -(Vector4US left, Vector4US right)
		{
			Subtract(ref left, ref right, out Vector4US result);
            return result;
		}

		public static Vector4US operator -(Vector4US left, ushort right)
		{
            Subtract(ref left, right, out Vector4US result);
            return result;
		}

        public static Vector4US operator -(ushort left, Vector4US right)
		{
            Subtract(ref right, left, out Vector4US result);
            return result;
		}

#endregion

#region division operators
		public static void Divide(ref Vector4US left, ref Vector4US right, out Vector4US result)
        {
			result.X = (ushort)(left.X / right.X);
			result.Y = (ushort)(left.Y / right.Y);
			result.Z = (ushort)(left.Z / right.Z);
			result.W = (ushort)(left.W / right.W);
        }

        public static void Divide(ref Vector4US left, ushort right, out Vector4US result)
        {
			result.X = (ushort)(left.X / right);
			result.Y = (ushort)(left.Y / right);
			result.Z = (ushort)(left.Z / right);
			result.W = (ushort)(left.W / right);
        }

		public static Vector4US operator /(Vector4US left, Vector4US right)
		{
			Divide(ref left, ref right, out Vector4US result);
            return result;
		}

		public static Vector4US operator /(Vector4US left, ushort right)
		{
            Divide(ref left, right, out Vector4US result);
            return result;
		}

        public static Vector4US operator /(ushort left, Vector4US right)
		{
            Divide(ref right, left, out Vector4US result);
            return result;
		}
#endregion

#region Multiply operators
		public static void Multiply(ref Vector4US left, ref Vector4US right, out Vector4US result)
        {
			result.X = (ushort)(left.X * right.X);
			result.Y = (ushort)(left.Y * right.Y);
			result.Z = (ushort)(left.Z * right.Z);
			result.W = (ushort)(left.W * right.W);
        }

        public static void Multiply(ref Vector4US left, ushort right, out Vector4US result)
        {
			result.X = (ushort)(left.X * right);
			result.Y = (ushort)(left.Y * right);
			result.Z = (ushort)(left.Z * right);
			result.W = (ushort)(left.W * right);
        }

		public static Vector4US operator *(Vector4US left, Vector4US right)
		{
			Multiply(ref left, ref right, out Vector4US result);
            return result;
		}

		public static Vector4US operator *(Vector4US left, ushort right)
		{
            Multiply(ref left, right, out Vector4US result);
            return result;
		}

        public static Vector4US operator *(ushort left, Vector4US right)
		{
            Multiply(ref right, left, out Vector4US result);
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
        public static bool operator ==(Vector4US left, Vector4US right)
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
        public static bool operator !=(Vector4US left, Vector4US right)
        {
            return !left.Equals(ref right);
        }
#endregion

#region Operators - Cast
        ///<summary>Casts a <see cref="Vector4US"/> to a <see cref="Vector2US"/>.</summary>
        public static explicit operator Vector2US(Vector4US value)
        {
            return new Vector2US(value.X, value.Y);
        }

        ///<summary>Casts a <see cref="Vector4US"/> to a <see cref="Vector3US"/>.</summary>
        public static explicit operator Vector3US(Vector4US value)
        {
            return new Vector3US(value.X, value.Y, value.Z);
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
        public static bool NearEqual(Vector4US left, Vector4US right, Vector4US epsilon)
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
        public static bool NearEqual(ref Vector4US left, ref Vector4US right, ref Vector4US epsilon)
        {
            return MathHelper.WithinEpsilon(left.X, right.X, epsilon.X) && MathHelper.WithinEpsilon(left.Y, right.Y, epsilon.Y) && MathHelper.WithinEpsilon(left.Z, right.Z, epsilon.Z) && MathHelper.WithinEpsilon(left.W, right.W, epsilon.W);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector4US SmoothStep(ref Vector4US start, ref Vector4US end, float amount)
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
        public static Vector4US SmoothStep(Vector4US start, Vector4US end, ushort amount)
        {
            return SmoothStep(ref start, ref end, amount);
        }    

        /// <summary>
        /// Orthogonalizes a list of <see cref="Vector4US"/>.
        /// </summary>
        /// <param name="destination">The list of orthogonalized <see cref="Vector4US"/>.</param>
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
        public static void Orthogonalize(Vector4US[] destination, params Vector4US[] source)
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
                Vector4US newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= (ushort)(Dot(destination[r], newvector) / Dot(destination[r], destination[r])) * destination[r];

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
        public static void Orthonormalize(Vector4US[] destination, params Vector4US[] source)
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
                Vector4US newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= Dot(destination[r], newvector) * destination[r];

                newvector.Normalize();
                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Vector4US"/>. <para />
        /// For example, a swizzle input of (1,1) on a <see cref="Vector4US"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
        /// </summary>
        /// <param name="val">The current vector.</param>
		/// <param name="xIndex">The axis index to use for the new X value.</param>
		/// <param name="yIndex">The axis index to use for the new Y value.</param>
		/// <param name="zIndex">The axis index to use for the new Z value.</param>
		/// <param name="wIndex">The axis index to use for the new W value.</param>
        /// <returns></returns>
        public static unsafe Vector4US Swizzle(Vector4US val, int xIndex, int yIndex, int zIndex, int wIndex)
        {
            return new Vector4US()
            {
			   X = (&val.X)[xIndex],
			   Y = (&val.X)[yIndex],
			   Z = (&val.X)[zIndex],
			   W = (&val.X)[wIndex],
            };
        }

        /// <returns></returns>
        public static unsafe Vector4US Swizzle(Vector4US val, uint xIndex, uint yIndex, uint zIndex, uint wIndex)
        {
            return new Vector4US()
            {
			    X = (&val.X)[xIndex],
			    Y = (&val.X)[yIndex],
			    Z = (&val.X)[zIndex],
			    W = (&val.X)[wIndex],
            };
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector4US.DistanceSquared(Vector4US, Vector4US)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static ushort Distance(ref Vector4US value1, ref Vector4US value2)
        {
			ushort x = (ushort)(value1.X - value2.X);
			ushort y = (ushort)(value1.Y - value2.Y);
			ushort z = (ushort)(value1.Z - value2.Z);
			ushort w = (ushort)(value1.W - value2.W);

            return (ushort)Math.Sqrt((x * x) + (y * y) + (z * z) + (w * w));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector4US.DistanceSquared(Vector4US, Vector4US)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static ushort Distance(Vector4US value1, Vector4US value2)
        {
			ushort x = (ushort)(value1.X - value2.X);
			ushort y = (ushort)(value1.Y - value2.Y);
			ushort z = (ushort)(value1.Z - value2.Z);
			ushort w = (ushort)(value1.W - value2.W);

            return (ushort)Math.Sqrt((x * x) + (y * y) + (z * z) + (w * w));
        }

        /// <summary>Checks to see if any value (x, y, z, w) are within 0.0001 of 0.
        /// If so this method truncates that value to zero.</summary>
        /// <param name="power">The power.</param>
        /// <param name="vec">The vector.</param>
        public static Vector4US Pow(Vector4US vec, ushort power)
        {
            return new Vector4US()
            {
				X = (ushort)Math.Pow(vec.X, power),
				Y = (ushort)Math.Pow(vec.Y, power),
				Z = (ushort)Math.Pow(vec.Z, power),
				W = (ushort)Math.Pow(vec.W, power),
            };
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector4US"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector4US"/> source vector</param>
        /// <param name="right">Second <see cref="Vector4US"/> source vector.</param>
        public static ushort Dot(ref Vector4US left, ref Vector4US right)
        {
			return (ushort)(((ushort)left.X * right.X) + ((ushort)left.Y * right.Y) + ((ushort)left.Z * right.Z) + ((ushort)left.W * right.W));
        }

		/// <summary>
        /// Calculates the dot product of two <see cref="Vector4US"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector4US"/> source vector</param>
        /// <param name="right">Second <see cref="Vector4US"/> source vector.</param>
        public static ushort Dot(Vector4US left, Vector4US right)
        {
			return (ushort)((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W));
        }

		/// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector4US"/> vector.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector4US"/> vector.</param>
        /// <param name="value2">Second source position <see cref="Vector4US"/> vector.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector4US"/> vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector4US Hermite(ref Vector4US value1, ref Vector4US tangent1, ref Vector4US value2, ref Vector4US tangent2, ushort amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0F * cubed) - (3.0F * squared)) + 1.0F;
            float part2 = (-2.0F * cubed) + (3.0F * squared);
            float part3 = (cubed - (2.0F * squared)) + amount;
            float part4 = cubed - squared;

			return new Vector4US()
			{
				X = (ushort)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4)),
				Y = (ushort)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4)),
				Z = (ushort)((((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4)),
				W = (ushort)((((value1.W * part1) + (value2.W * part2)) + (tangent1.W * part3)) + (tangent2.W * part4)),
			};
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector4US"/>.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector4US"/>.</param>
        /// <param name="value2">Second source position <see cref="Vector4US"/>.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector4US"/>.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Vector4US Hermite(Vector4US value1, Vector4US tangent1, Vector4US value2, Vector4US tangent2, ushort amount)
        {
            return Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount);
        }

		/// <summary>
        /// Returns a <see cref="Vector4US"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector4US"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector4US"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector4US"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector4US Barycentric(ref Vector4US value1, ref Vector4US value2, ref Vector4US value3, ushort amount1, ushort amount2)
        {
			return new Vector4US(
				(ushort)((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))), 
				(ushort)((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y))), 
				(ushort)((value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z))), 
				(ushort)((value1.W + (amount1 * (value2.W - value1.W))) + (amount2 * (value3.W - value1.W)))
			);
        }

        /// <summary>
        /// Performs a linear interpolation between two <see cref="Vector4US"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref Vector4US start, ref Vector4US end, float amount, out Vector4US result)
        {
			result.X = (ushort)((1F - amount) * start.X + amount * end.X);
			result.Y = (ushort)((1F - amount) * start.Y + amount * end.Y);
			result.Z = (ushort)((1F - amount) * start.Z + amount * end.Z);
			result.W = (ushort)((1F - amount) * start.W + amount * end.W);
        }

		/// <summary>
        /// Performs a linear interpolation between two <see cref="Vector4US"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector4US Lerp(ref Vector4US start, ref Vector4US end, float amount)
        {
			return new Vector4US()
			{
				X = (ushort)((1F - amount) * start.X + amount * end.X),
				Y = (ushort)((1F - amount) * start.Y + amount * end.Y),
				Z = (ushort)((1F - amount) * start.Z + amount * end.Z),
				W = (ushort)((1F - amount) * start.W + amount * end.W),
			};
        }

        /// <summary>
        /// Returns a <see cref="Vector4US"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector4US"/>.</param>
        /// <param name="right">The second source <see cref="Vector4US"/>.</param>
        /// <returns>A <see cref="Vector4US"/> containing the smallest components of the source vectors.</returns>
		public static void Min(ref Vector4US left, ref Vector4US right, out Vector4US result)
		{
				result.X = (left.X < right.X) ? left.X : right.X;
				result.Y = (left.Y < right.Y) ? left.Y : right.Y;
				result.Z = (left.Z < right.Z) ? left.Z : right.Z;
				result.W = (left.W < right.W) ? left.W : right.W;
		}

        /// <summary>
        /// Returns a <see cref="Vector4US"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector4US"/>.</param>
        /// <param name="right">The second source <see cref="Vector4US"/>.</param>
        /// <returns>A <see cref="Vector4US"/> containing the smallest components of the source vectors.</returns>
		public static Vector4US Min(ref Vector4US left, ref Vector4US right)
		{
			Min(ref left, ref right, out Vector4US result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector4US"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector4US"/>.</param>
        /// <param name="right">The second source <see cref="Vector4US"/>.</param>
        /// <returns>A <see cref="Vector4US"/> containing the smallest components of the source vectors.</returns>
		public static Vector4US Min(Vector4US left, Vector4US right)
		{
			return new Vector4US()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
				Z = (left.Z < right.Z) ? left.Z : right.Z,
				W = (left.W < right.W) ? left.W : right.W,
			};
		}

        /// <summary>
        /// Returns a <see cref="Vector4US"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector4US"/>.</param>
        /// <param name="right">The second source <see cref="Vector4US"/>.</param>
        /// <returns>A <see cref="Vector4US"/> containing the largest components of the source vectors.</returns>
		public static void Max(ref Vector4US left, ref Vector4US right, out Vector4US result)
		{
				result.X = (left.X > right.X) ? left.X : right.X;
				result.Y = (left.Y > right.Y) ? left.Y : right.Y;
				result.Z = (left.Z > right.Z) ? left.Z : right.Z;
				result.W = (left.W > right.W) ? left.W : right.W;
		}

        /// <summary>
        /// Returns a <see cref="Vector4US"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector4US"/>.</param>
        /// <param name="right">The second source <see cref="Vector4US"/>.</param>
        /// <returns>A <see cref="Vector4US"/> containing the largest components of the source vectors.</returns>
		public static Vector4US Max(ref Vector4US left, ref Vector4US right)
		{
			Max(ref left, ref right, out Vector4US result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector4US"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector4US"/>.</param>
        /// <param name="right">The second source <see cref="Vector4US"/>.</param>
        /// <returns>A <see cref="Vector4US"/> containing the largest components of the source vectors.</returns>
		public static Vector4US Max(Vector4US left, Vector4US right)
		{
			return new Vector4US()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
				Z = (left.Z > right.Z) ? left.Z : right.Z,
				W = (left.W > right.W) ? left.W : right.W,
			};
		}

		/// <summary>
        /// Calculates the squared distance between two <see cref="Vector4US"/> vectors.
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
		public static ushort DistanceSquared(ref Vector4US value1, ref Vector4US value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;
            int w = value1.W - value2.W;

            return (ushort)((x * x) + (y * y) + (z * z) + (w * w));
        }

        /// <summary>
        /// Calculates the squared distance between two <see cref="Vector4US"/> vectors.
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
		public static ushort DistanceSquared(Vector4US value1, Vector4US value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;
            int w = value1.W - value2.W;

            return (ushort)((x * x) + (y * y) + (z * z) + (w * w));
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector4US"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector4US Clamp(Vector4US value, ushort min, ushort max)
        {
			return new Vector4US()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
				Z = value.Z < min ? min : value.Z > max ? max : value.Z,
				W = value.W < min ? min : value.W > max ? max : value.W,
			};
        }

        /// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector4US"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static void Clamp(ref Vector4US value, ref Vector4US min, ref Vector4US max, out Vector4US result)
        {
				result.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
				result.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
				result.Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z;
				result.W = value.W < min.W ? min.W : value.W > max.W ? max.W : value.W;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector4US"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector4US Clamp(Vector4US value, Vector4US min, Vector4US max)
        {
			return new Vector4US()
			{
				X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
				Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
				Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z,
				W = value.W < min.W ? min.W : value.W > max.W ? max.W : value.W,
			};
        }


        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal. 
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.</remarks>
        public static Vector4US Reflect(ref Vector4US vector, ref Vector4US normal)
        {
            int dot = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z) + (vector.W * normal.W);

            return new Vector4US()
            {
				X = (ushort)(vector.X - ((2.0F * dot) * normal.X)),
				Y = (ushort)(vector.Y - ((2.0F * dot) * normal.Y)),
				Z = (ushort)(vector.Z - ((2.0F * dot) * normal.Z)),
				W = (ushort)(vector.W - ((2.0F * dot) * normal.W)),
            };
        }

        /// <summary>
        /// Converts the <see cref="Vector4US"/> into a unit vector.
        /// </summary>
        /// <param name="value">The <see cref="Vector4US"/> to normalize.</param>
        /// <returns>The normalized <see cref="Vector4US"/>.</returns>
        public static Vector4US Normalize(Vector4US value)
        {
            value.Normalize();
            return value;
        }
#endregion

#region Indexers
		/// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z or W component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component and so on.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        
		public ushort this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
					case 3: return W;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector4US run from 0 to 3, inclusive.");
			}

			set
			{
				switch(index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
					case 3: W = value; break;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector4US run from 0 to 3, inclusive.");
			}
		}
#endregion
	}
}

