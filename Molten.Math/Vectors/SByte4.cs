using System;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Molten.Math
{
	///<summary>A <see cref = "sbyte"/> vector comprised of four components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public partial struct SByte4
	{
		///<summary>The X component.</summary>
		public sbyte X;

		///<summary>The Y component.</summary>
		public sbyte Y;

		///<summary>The Z component.</summary>
		public sbyte Z;

		///<summary>The W component.</summary>
		public sbyte W;


		///<summary>The size of <see cref="SByte4"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(SByte4));

		public static SByte4 One = new SByte4(1, 1, 1, 1);

		/// <summary>
        /// The X unit <see cref="SByte4"/>.
        /// </summary>
		public static SByte4 UnitX = new SByte4(1, 0, 0, 0);

		/// <summary>
        /// The Y unit <see cref="SByte4"/>.
        /// </summary>
		public static SByte4 UnitY = new SByte4(0, 1, 0, 0);

		/// <summary>
        /// The Z unit <see cref="SByte4"/>.
        /// </summary>
		public static SByte4 UnitZ = new SByte4(0, 0, 1, 0);

		/// <summary>
        /// The W unit <see cref="SByte4"/>.
        /// </summary>
		public static SByte4 UnitW = new SByte4(0, 0, 0, 1);

		public static SByte4 Zero = new SByte4(0, 0, 0, 0);

#region Constructors
		///<summary>Creates a new instance of <see cref = "SByte4"/>.</summary>
		public SByte4(sbyte x, sbyte y, sbyte z, sbyte w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="SByte4"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, Z and W components of the vector. This must be an array with 4 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public SByte4(sbyte[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be 4 and only 4 input values for SByte4.");

			X = values[0];
			Y = values[1];
			Z = values[2];
			W = values[3];
        }

		public unsafe SByte4(sbyte* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
			Z = ptr[2];
			W = ptr[3];
		}
#endregion

#region Common Functions
		/// <summary>
        /// Calculates the squared distance between two <see cref="SByte4"/> vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector</param>
        /// <param name="result">When the method completes, contains the squared distance between the two vectors.</param>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
		public static void DistanceSquared(ref SByte4 value1, ref SByte4 value2, out sbyte result)
        {
            sbyte x = value1.X - value2.X;
            sbyte y = value1.Y - value2.Y;
            sbyte z = value1.Z - value2.Z;
            sbyte w = value1.W - value2.W;

            result = (x * x) + (y * y) + (z * z) + (w * w);
        }

		/// <summary>
        /// Calculates the squared distance between two <see cref="SByte4"/> vectors.
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
		public static sbyte DistanceSquared(ref SByte4 value1, ref SByte4 value2)
        {
            sbyte x = value1.X - value2.X;
            sbyte y = value1.Y - value2.Y;
            sbyte z = value1.Z - value2.Z;
            sbyte w = value1.W - value2.W;

            return (x * x) + (y * y) + (z * z) + (w * w);
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="SByte4"/>.
        /// </summary>
        /// <returns>A four-element array containing the components of the vector.</returns>
        public sbyte[] ToArray()
        {
            return new sbyte[] { X, Y, Z, W};
        }

		/// <summary>
        /// Reverses the direction of the current <see cref="SByte4"/>.
        /// </summary>
        /// <returns>A <see cref="SByte4"/> facing the opposite direction.</returns>
		public SByte4 Negate()
		{
			return new SByte4(-X, -Y, -Z, -W);
		}

		/// <summary>
        /// Performs a linear interpolation between two <see cref="SByte4"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two vectors.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static SByte4 Lerp(ref SByte4 start, ref SByte4 end, float amount)
        {
			return new SByte4()
			{
				X = (sbyte)((1f - amount) * start.X + amount * end.X),
				Y = (sbyte)((1f - amount) * start.Y + amount * end.Y),
				Z = (sbyte)((1f - amount) * start.Z + amount * end.Z),
				W = (sbyte)((1f - amount) * start.W + amount * end.W),
			};
        }

		/// <summary>
        /// Returns a <see cref="SByte4"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="SByte4"/>.</param>
        /// <param name="right">The second source <see cref="SByte4"/>.</param>
        /// <returns>A <see cref="SByte4"/> containing the smallest components of the source vectors.</returns>
		public static SByte4 Min(SByte4 left, SByte4 right)
		{
			return new SByte4()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
				Z = (left.Z < right.Z) ? left.Z : right.Z,
				W = (left.W < right.W) ? left.W : right.W,
			};
		}

		/// <summary>
        /// Returns a <see cref="SByte4"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="SByte4"/>.</param>
        /// <param name="right">The second source <see cref="SByte4"/>.</param>
        /// <returns>A <see cref="SByte4"/> containing the largest components of the source vectors.</returns>
		public static SByte4 Max(SByte4 left, SByte4 right)
		{
			return new SByte4()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
				Z = (left.Z > right.Z) ? left.Z : right.Z,
				W = (left.W > right.W) ? left.W : right.W,
			};
		}

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(sbyte min, sbyte max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
			Z = Z < min ? min : Z > max ? max : Z;
			W = W < min ? min : W > max ? max : W;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(SByte4 min, SByte4 max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
			Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
			W = W < min.W ? min.W : W > max.W ? max.W : W;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture), W.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="SByte4"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider));
        }
#endregion

#region Add operators
		public static SByte4 operator +(SByte4 left, SByte4 right)
		{
			return new SByte4(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
		}

		public static SByte4 operator +(SByte4 left, sbyte right)
		{
			return new SByte4(left.X + right, left.Y + right, left.Z + right, left.W + right);
		}

		/// <summary>
        /// Assert a <see cref="SByte4"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="SByte4"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="SByte4"/>.</returns>
        public static SByte4 operator +(SByte4 value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static SByte4 operator -(SByte4 left, SByte4 right)
		{
			return new SByte4(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
		}

		public static SByte4 operator -(SByte4 left, sbyte right)
		{
			return new SByte4(left.X - right, left.Y - right, left.Z - right, left.W - right);
		}

		/// <summary>
        /// Negate/reverse the direction of a <see cref="SByte4"/>.
        /// </summary>
        /// <param name="value">The <see cref="SByte4"/> to reverse.</param>
        /// <returns>The reversed <see cref="SByte4"/>.</returns>
        public static SByte4 operator -(SByte4 value)
        {
            return new SByte4(-value.X, -value.Y, -value.Z, -value.W);
        }
#endregion

#region division operators
		public static SByte4 operator /(SByte4 left, SByte4 right)
		{
			return new SByte4(left.X / right.X, left.Y / right.Y, left.Z / right.Z, left.W / right.W);
		}

		public static SByte4 operator /(SByte4 left, sbyte right)
		{
			return new SByte4(left.X / right, left.Y / right, left.Z / right, left.W / right);
		}
#endregion

#region Multiply operators
		public static SByte4 operator *(SByte4 left, SByte4 right)
		{
			return new SByte4(left.X * right.X, left.Y * right.Y, left.Z * right.Z, left.W * right.W);
		}

		public static SByte4 operator *(SByte4 left, sbyte right)
		{
			return new SByte4(left.X * right, left.Y * right, left.Z * right, left.W * right);
		}
#endregion

#region Properties

#endregion

#region Static Methods
		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="SByte4"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static SByte4 Clamp(SByte4 value, sbyte min, sbyte max)
        {
			return new SByte4()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
				Z = value.Z < min ? min : value.Z > max ? max : value.Z,
				W = value.W < min ? min : value.W > max ? max : value.W,
			};
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="SByte4"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static SByte4 Clamp(SByte4 value, SByte4 min, SByte4 max)
        {
			return new SByte4()
			{
				X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
				Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
				Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z,
				W = value.W < min.W ? min.W : value.W > max.W ? max.W : value.W,
			};
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
        
		public sbyte this[int index]
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
				throw new ArgumentOutOfRangeException("index", "Indices for SByte4 run from 0 to 3, inclusive.");
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
				throw new ArgumentOutOfRangeException("index", "Indices for SByte4 run from 0 to 3, inclusive.");
			}
		}
#endregion
	}
}

