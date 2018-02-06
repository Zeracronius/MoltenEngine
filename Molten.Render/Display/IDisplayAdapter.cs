﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public interface IDisplayAdapter
    {
        /// <summary>Occurs when an <see cref="IDisplayOutput"/> is connected to the current <see cref="IDisplayAdapter"/>.</summary>
        event DisplayOutputChanged OnOutputAdded;

        /// <summary>Occurs when an <see cref="IDisplayOutput"/> is disconnected from the current <see cref="IDisplayAdapter"/>.</summary>
        event DisplayOutputChanged OnOutputRemoved;

        /// <summary>Gets all <see cref="IDisplayOutput"/> devices attached to the current <see cref="IDisplayAdapter"/>.</summary>
        /// <param name="outputList">The output list.</param>
        void GetAttachedOutputs(List<IDisplayOutput> outputList);

        void GetActiveOutputs(List<IDisplayOutput> outputList);

        IDisplayOutput GetOutput(int id);

        void AddActiveOutput(IDisplayOutput output);

        void RemoveActiveOutput(IDisplayOutput output);

        void RemoveAllActiveOutputs();

        /// <summary>Gets the name of the adapter.</summary>
        string Name { get; }

        /// <summary>Gets the amount of dedicated video memory, in megabytes.</summary>
        double DedicatedVideoMemory { get; }

        /// <summary>Gets the amount of system memory dedicated to the adapter.</summary>
        double DedicatedSystemMemory { get; }

        /// <summary>Gets the amount of system memory that is being shared with the adapter.</summary>
        double SharedSystemMemory { get; }

        /// <summary>Gets the listing ID of the <see cref="IDisplayAdapter"/>.</summary>
        int ID { get; }

        /// <summary>The PCI ID of the hardware vendor.</summary>
        int VendorID { get; }

        /// <summary>The PCI ID of the hardware adapter.</summary>
        int DeviceID { get; }
        /// <summary>Gets a unique value that identifies the adapter hardware.</summary>
        long UniqueID { get; }

        /// <summary>Gets the PCI ID of the revision number of the adapter.</summary>
        int Revision { get; }

        /// <summary>Gets the PCI ID of the sub-system.</summary>
        int SubsystemID { get; }

        /// <summary>Gets the number of <see cref="IDisplayOutput"/> connected to the current <see cref="IDisplayAdapter"/>.</summary>
        int OutputCount { get; }

        /// <summary>Gets the <see cref="IDisplayManager"/> that spawned the adapter.</summary>
        IDisplayManager Manager { get; }
    }
}