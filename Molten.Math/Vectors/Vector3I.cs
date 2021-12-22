using System.Runtime.InteropServices;

namespace Molten.Math
{
	///<summary>A <see cref = "int"/> vector comprised of 3 components.</summary>
	[StructLayout(LayoutKind.Sequential)]
	public partial struct Vector3I
	{
		///<summary>The X component.</summary>
		public int X;

		///<summary>The Y component.</summary>
		public int Y;

		///<summary>The Z component.</summary>
		public int Z;

		///<summary>Creates a new instance of <see cref = "Vector3I"/></summary>
		public Vector3I(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

#region operators
		public static Vector3I operator +(Vector3I left, Vector3I right)
		{
			return new Vector3I(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		public static Vector3I operator -(Vector3I left, Vector3I right)
		{
			return new Vector3I(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		public static Vector3I operator /(Vector3I left, Vector3I right)
		{
			return new Vector3I(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
		}

		public static Vector3I operator *(Vector3I left, Vector3I right)
		{
			return new Vector3I(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
		}
#endregion
	}
}
