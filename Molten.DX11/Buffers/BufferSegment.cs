﻿using Molten.Collections;
using Silk.NET.Direct3D11;
using Silk.NET.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    internal unsafe class BufferSegment : PipelineShaderObject, IPoolable, ICloneable
    {
        /// <summary>The size of the segment in bytes. This is <see cref="ElementCount"/> multiplied by <see cref="Stride"/>.</summary>
        internal uint ByteCount;

        /// <summary>The number of elements that the segment can hold.</summary>
        internal uint ElementCount;

        /// <summary>The previous segment (if any) within the mapped buffer.</summary>
        internal BufferSegment Previous;

        /// <summary>The next segment (if any) within the mapped buffer.</summary>
        internal BufferSegment Next;

        /// <summary>
        /// The <see cref="GraphicsBuffer"/> that contains the current <see cref="BufferSegment"/>
        /// </summary>
        internal GraphicsBuffer Buffer;

        /// <summary>
        /// The byte offset within the <see cref="Buffer"/> <see cref="GraphicsBuffer"/>.
        /// </summary>
        internal uint ByteOffset;

        /// <summary>
        /// The size of each element within the buffer, in bytes.
        /// </summary>
        internal uint Stride;

        internal VertexFormat VertexFormat;

        internal Format DataFormat;

        /// <summary>If true, the segment is not used.</summary>
        internal bool IsFree;

        internal BufferSegment(DeviceDX11 device) : base(device) { }

        internal void SetVertexFormat<T>() where T: struct, IVertexType
        {
            VertexFormat = Buffer.Device.VertexBuilder.GetFormat<T>();
        }

        internal void SetVertexFormat(Type vertexType)
        {
            VertexFormat = Buffer.Device.VertexBuilder.GetFormat(vertexType);
        }

        internal void SetIndexFormat(IndexBufferFormat format)
        {
            switch (format)
            {
                case IndexBufferFormat.Unsigned32Bit:
                    DataFormat = Format.FormatR32Uint;
                    break;

                case IndexBufferFormat.Unsigned16Bit:
                    DataFormat = Format.FormatR16Uint;
                    break;
            }
        }

        /// <summary>Copies an array of elements into the buffer.</summary>
        /// <param name="data">The elements to set </param>
        internal void SetData<T>(PipeDX11 pipe, T[] data) where T : struct => SetData<T>(pipe, data, 0, (uint)data.Length);

        /// <summary>Copies element data into the buffer.</summary>
        /// <param name="data">The source of elements to copy into the buffer.</param>
        /// <param name="offset">The ID of the first element in the buffer at which to copy the source data into.</param>
        /// <param name="count">The number of elements to copy from the source array.</param>
        internal void SetData<T>(PipeDX11 pipe, T[] data, uint count) where T : struct => SetData<T>(pipe, data, 0, count);

        /// <summary>Copies element data into the buffer.</summary>
        /// <param name="data">The source of elements to copy into the buffer.</param>
        /// <param name="startIndex">The index in the data array at which to start copying from.</param>
        /// <param name="count">The number of elements to copy from the source array, beginning at the start index.</param>
        /// <param name="elementOffset">The number of elements from the beginning of the <see cref="BufferSegment"/> to offset the destination of the provided data.
        /// The number of bytes the data is offset is based on the <see cref="Stride"/> value of the buffer segment.</param>
        /// <param name="completionCallback">The callback to invoke when the set-data operation has been completed.</param>
        internal void SetData<T>(PipeDX11 pipe, T[] data, uint startIndex, uint count, uint elementOffset = 0, StagingBuffer staging = null, Action completionCallback = null) where T : struct
        {
            uint tStride = (uint)Marshal.SizeOf(typeof(T));
            uint dataSize = tStride * count;
            uint writeOffset = elementOffset * tStride;
            uint finalBufferPos = dataSize + writeOffset + ByteOffset;
            uint segmentBounds = ByteOffset + ByteCount;

            // Ensure the buffer can fit the provided data.
            if (finalBufferPos > segmentBounds)
                throw new OverflowException($"Provided data's final byte position {finalBufferPos} would exceed the segment's bounds (byte {segmentBounds})");

            BufferSetOperation<T> op = new BufferSetOperation<T>()
            {
                Data = new T[count],
                DataStride = tStride,
                ByteOffset = ByteOffset + writeOffset,
                StartIndex = startIndex,
                Count = count,
                DestinationSegment = this,
                CompletionCallback = completionCallback,
                Staging = staging,
            };

            // Copy data and queue operation.
            Array.Copy(data, startIndex, op.Data, 0, count);
            Buffer.QueueOperation(op);
        }

        /// <summary>Immediately sets the data on the buffer.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pipe">The pipe.</param>
        /// <param name="data">The data.</param>
        /// <param name="startIndex">The element index within the provided data array to start copying from.</param>
        /// <param name="count">The number of elements to transfer from the provided data array.</param>
        /// <param name="byteOffset">The number of bytes to offset the copied data within the buffer segment.</param>
        internal void SetDataImmediate<T>(PipeDX11 pipe, T[] data, uint startIndex, uint count, uint elementOffset = 0, StagingBuffer staging = null) where T : struct
        {
            uint tStride = (uint)Marshal.SizeOf<T>();
            uint dataSize = tStride * count;
            uint writeOffset = elementOffset * tStride;
            uint finalBytePos = dataSize + writeOffset + ByteOffset;
            uint segmentBounds = ByteOffset + ByteCount;

            // Ensure the buffer can fit the provided data.
            if (finalBytePos > segmentBounds)
                throw new OverflowException($"Provided data's final byte position {finalBytePos} would exceed the segment's bounds (byte {segmentBounds})");

            Buffer.Set<T>(pipe, data, startIndex, count, tStride, ByteOffset + writeOffset, staging);
        }

        internal void Map(PipeDX11 pipe, Action<GraphicsBuffer, ResourceStream> callback, GraphicsBuffer staging = null)
        {
            Buffer.GetStream(pipe, ByteOffset, Stride * ElementCount, (buffer, stream) =>
            {
                if (Buffer.Mode == BufferMode.DynamicRing)
                    ByteOffset = (uint)stream.Position;

                callback(buffer, stream);
            }, staging); 
        }

        internal void GetData<T>(PipeDX11 pipe, 
            T[] destination, 
            uint startIndex, 
            uint count, 
            uint elementOffset = 0, 
            Action<T[]> completionCallback = null) where T : struct
        {
            BufferGetOperation<T> op = new BufferGetOperation<T>()
            {
                ByteOffset = ByteOffset,
                DestinationArray = destination,
                DestinationIndex = startIndex,
                Count = count,
                DataStride = (uint)Marshal.SizeOf<T>(),
                CompletionCallback = completionCallback,
                SourceSegment = this,
            };

            Buffer.QueueOperation(op);
        }

        internal void CopyTo(PipeDX11 pipe, uint sourceByteOffset, BufferSegment destination, uint destByteOffset, uint count, bool isImmediate = false, Action completionCallback = null)
        {
            uint bytesToCopy = Stride * count;
            uint totalOffset = ByteOffset + sourceByteOffset;
            uint lastByte = totalOffset + bytesToCopy;

            uint destLastByte = destination.ByteOffset + destination.ByteCount;
            if (lastByte > destLastByte)
                throw new OverflowException("specified copy region would exceed the bounds of the destination segment.");

            ResourceRegion sourceRegion = new ResourceRegion()
            {
                Left = totalOffset,
                Right = lastByte,
                Back = 1,
                Bottom = 1,
            };

            if (isImmediate)
            {
                Buffer.CopyTo(pipe, destination.Buffer, sourceRegion, destination.ByteOffset + destByteOffset);
            }
            else
            {
                BufferCopyOperation op = new BufferCopyOperation()
                {
                    CompletionCallback = completionCallback,
                    SourceBuffer = Buffer,
                    DestinationBuffer = destination.Buffer,
                    DataStride = Stride,
                    DestinationByteOffset = destination.ByteOffset + destByteOffset,
                    SourceRegion = sourceRegion,
                };

                Buffer.QueueOperation(op);
            }
        }

        internal override void Refresh(PipeDX11 pipe, PipelineBindSlot<DeviceDX11, PipeDX11> slot)
        {
            Buffer.Refresh(pipe, slot);
        }

        /// <summary>Releases the buffer space reserved by the segment.</summary>
        internal void Release()
        {
            Buffer.Deallocate(this);
        }

        /// <summary>Clears segment's internal data.</summary>
        public void ClearForPool()
        {
            Previous = null;
            Next = null;
            VertexFormat = null;
            SetIndexFormat(IndexBufferFormat.Unsigned32Bit);

            UAV->Release();
            UAV = null;

            SRV->Release();
            SRV = null;
        }

        /// <summary>Sets the next segment to the one specified and also sets it's previous to the current segment.</summary>
        /// <param name="next">The next segment.</param>
        internal void LinkNext(BufferSegment next)
        {
            Next = next;
            if(next != null)
                next.Previous = this;
        }

        /// <summary>Sets the previous segment to the one specified and also sets it's next to the current segment.</summary>
        /// <param name="previous">The previous segment.</param>
        internal void LinkPrevious(BufferSegment previous)
        {
            Previous = previous;
            if(previous != null)
                previous.Next = this;
        }

        /// <summary>Take's bytes off the previous segment and adds them to the current.</summary>
        /// <param name="bytesToTake">The bytes to take.</param>
        internal void TakeFromPrevious(uint bytesToTake)
        {
            ByteOffset -= bytesToTake;
            ByteCount += bytesToTake;
            Previous.ByteCount -= bytesToTake;

            // If all of the previous was consumed, recycle it.
            if(Previous.ByteCount == 0)
            {
                LinkPrevious(Previous.Previous);
                Device.RecycleBufferSegment(Previous);
            }
        }

        /// <summary>Decreases the size of the current segment by the specified amount of bytes, from the front, then adds  it to the next segment's back.</summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="bytesToTake">The bytes to take.</param>
        internal void TakeFromNext(uint bytesToTake)
        {
            ByteCount += bytesToTake;
            Next.ByteOffset += bytesToTake;
            Next.ByteCount -= bytesToTake;

            // If all of the previous was consumed, recycle it.
            if (Next.ByteCount == 0)
            {
                LinkNext(Next.Next);
                Device.RecycleBufferSegment(Next);
            }
        }

        internal BufferSegment SplitFromBack(uint bytesToTake)
        {
            BufferSegment seg = Device.GetBufferSegment();
            seg.LinkNext(this);
            seg.LinkPrevious(Previous);

            seg.ByteCount = bytesToTake;
            seg.ByteOffset = ByteOffset;
            seg.Buffer = Buffer;

            ByteOffset += bytesToTake;
            ByteCount -= bytesToTake;

            return seg;
        }

        internal BufferSegment SplitFromFront(uint bytesToTake)
        {
            BufferSegment seg = Device.GetBufferSegment();
            seg.LinkNext(Next);
            seg.LinkPrevious(this);

            ByteCount -= bytesToTake;
            seg.ByteCount = bytesToTake;
            seg.ByteOffset = ByteOffset + ByteCount;
            seg.Buffer = Buffer;
            return seg;
        }

        public object Clone()
        {
            BufferSegment clone = Device.GetBufferSegment();
            CloneTo(clone);
            return clone;
        }

        public void CloneTo(BufferSegment dest)
        {
            dest.ByteCount = ByteCount;
            dest.ByteOffset = ByteOffset;
            dest.ElementCount = ElementCount;
            dest.DataFormat = DataFormat;
            dest.IsFree = IsFree;
            dest.Next = Next;
            dest.Buffer = Buffer;
            dest.Previous = Previous;
            dest.SRV = SRV;
            dest.Stride = Stride;
            dest.UAV = UAV;
            dest.VertexFormat = VertexFormat;
        }
    }
}
