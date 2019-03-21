using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molten.Graphics
{
    public abstract class GraphicsStateBank<T, E> : IDisposable
        where T : PipelineObject<DeviceDX11, PipeDX11>
        where E: struct, IConvertible
    {
        protected Dictionary<E, T> _presets;
        List<T> _states;

        internal GraphicsStateBank()
        {
            _presets = new Dictionary<E, T>();
            _states = new List<T>();
        }

        public void Dispose()
        {
            foreach (T state in _states)
                state.Dispose();
        }

        /// <summary>
        /// Attempts to add the provided state to the bank. If an identical state is already stored, the provided one is disposed and the existing one returned. <para/>
        /// The provided state will not be disposed if it is one which is already stored in the bank.
        /// </summary>
        /// <param name="state">The state to add.</param>
        /// <returns></returns>
        internal T AddOrRetrieveExisting(T state)
        {
            foreach (T existing in _states)
            {
                if (existing.Equals(state))
                {
                    if(state != existing)
                        state.Dispose();

                    return existing;
                }
            }            

            _states.Add(state);
            return state;
        }

        internal T GetPreset(E preset)
        {
            if(_presets.TryGetValue(preset, out T state))
            {
                return state;
            }
            else
            {
                state = CreatePreset(preset);
                _presets.Add(preset, state);
            }

            return state;
        }

        protected abstract T CreatePreset(E preset);
    }
}
