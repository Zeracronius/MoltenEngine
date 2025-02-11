﻿namespace Molten.Graphics
{
    /// <summary>A mesh which uses an index buffer to reduce the amount of vertex data, by referring to vertices by their ID in the index buffer, 
    /// essentially allowing the same vertex to be referenced multiple times.</summary>
    /// <typeparam name="T">The type of vertex data that the mesh is to expect.</typeparam>
    public interface IIndexedMesh<T> : IMesh<T> where T : unmanaged, IVertexType
    {
        /// <summary>Copies the provided index data to the current mesh.</summary>
        /// <typeparam name="I">The type of data to set.</typeparam>
        /// <param name="data">The data to be copied.</param>
        void SetIndices<I>(I[] data) where I : unmanaged;

        /// <summary>Copies the provided index data to the current mesh.</summary>
        /// <typeparam name="I">The type of data to set.</typeparam>
        /// <param name="count">The number of elements in the dat array to copy.</param>
        /// <param name="data">The data to be copied.</param>
        void SetIndices<I>(I[] data, uint count) where I : unmanaged;

        /// <summary>Copies the provided index data to the current mesh.</summary>
        /// <typeparam name="I">The type of data to set.</typeparam>
        /// <param name="count">The number of elements in the dat array to copy.</param>
        /// <param name="data">The data to be copied.</param>
        /// <param name="startIndex">The element within the data array to start copying from.</param>
        void SetIndices<I>(I[] data, uint startIndex, uint count) where I : unmanaged;

        /// <summary>Gets the maximum number of indices the mesh can contain.</summary>
        uint MaxIndices { get; }

        /// <summary>Gets the number of indices currently stored in the mesh.</summary>
        uint IndexCount { get; }
    }
}
