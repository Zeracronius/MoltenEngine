using System.Runtime.InteropServices;

namespace Molten.Math
{
	///<summary>A <see cref = "long"/> vector comprised of 2 components.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial struct Vector2L
	{
		///<summary>The X component.</summary>
		public long X;

		///<summary>The Y component.</summary>
		public long Y;

		///<summary>Creates a new instance of <see cref = "Vector2L"/></summary>
		public Vector2L(long x, long y)
		{
			X = x;
			Y = y;
		}

#region operators
		public static Vector2L operator +(Vector2L left, Vector2L right)
		{
			return new Vector2L(left.X + right.X, left.Y + right.Y);
		}

		public static Vector2L operator -(Vector2L left, Vector2L right)
		{
			return new Vector2L(left.X - right.X, left.Y - right.Y);
		}

		public static Vector2L operator /(Vector2L left, Vector2L right)
		{
			return new Vector2L(left.X / right.X, left.Y / right.Y);
		}

		public static Vector2L operator *(Vector2L left, Vector2L right)
		{
			return new Vector2L(left.X * right.X, left.Y * right.Y);
		}
#endregion
	}
}
