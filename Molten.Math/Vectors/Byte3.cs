using System.Runtime.InteropServices;

namespace Molten.Math
{
	///<summary>A <see cref = "byte"/> vector comprised of 3 components.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial struct Byte3
	{
		///<summary>The X component.</summary>
		public byte X;

		///<summary>The Y component.</summary>
		public byte Y;

		///<summary>The Z component.</summary>
		public byte Z;

		///<summary>Creates a new instance of <see cref = "Byte3"/></summary>
		public Byte3(byte x, byte y, byte z)
		{
			X = x;
			Y = y;
			Z = z;
		}

#region operators
		public static Byte3 operator +(Byte3 left, Byte3 right)
		{
			return new Byte3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		public static Byte3 operator -(Byte3 left, Byte3 right)
		{
			return new Byte3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		public static Byte3 operator /(Byte3 left, Byte3 right)
		{
			return new Byte3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
		}

		public static Byte3 operator *(Byte3 left, Byte3 right)
		{
			return new Byte3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
		}
#endregion
	}
}
