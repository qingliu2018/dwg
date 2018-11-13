﻿using System.Collections;
using System.Collections.Generic;
using IxMilia.Dwg.Collections;

namespace IxMilia.Dwg.Objects
{
    public partial class DwgViewPortControlObject : IDictionary<string, DwgViewPort>
    {
        private IDictionary<string, DwgViewPort> _viewPorts = new StringDictionary<DwgViewPort>(ignoreCase: true);

        internal override IEnumerable<DwgObject> ChildItems => _viewPorts.Values;

        internal override void PreWrite()
        {
            foreach (var viewPort in _viewPorts.Values)
            {
                _entityHandles.Add(new DwgHandleReference(DwgHandleReferenceCode.None, viewPort.Handle.HandleOrOffset));
                viewPort.ViewPortControlHandle = new DwgHandleReference(DwgHandleReferenceCode.HardPointer, Handle.HandleOrOffset);
            }
        }

        internal override void PoseParse(BitReader reader, DwgObjectCache objectCache)
        {
            _viewPorts.Clear();
            foreach (var viewPortHandle in _entityHandles)
            {
                if (viewPortHandle.Code != DwgHandleReferenceCode.None)
                {
                    throw new DwgReadException("Incorrect child view port handle code.");
                }

                var viewPort = objectCache.GetObject<DwgViewPort>(reader, viewPortHandle.HandleOrOffset);
                if (viewPort.ViewPortControlHandle.HandleOrOffset != Handle.HandleOrOffset)
                {
                    throw new DwgReadException("Incorrect view port control object parent handle reference.");
                }

                _viewPorts.Add(viewPort.Name, viewPort);
            }
        }

        public void Add(DwgViewPort viewPort) => Add(viewPort.Name, viewPort);

        #region IDictionary<string, DwgViewPort> implementation

        public ICollection<string> Keys => ((IDictionary<string, DwgViewPort>)_viewPorts).Keys;

        public ICollection<DwgViewPort> Values => ((IDictionary<string, DwgViewPort>)_viewPorts).Values;

        public int Count => ((IDictionary<string, DwgViewPort>)_viewPorts).Count;

        public bool IsReadOnly => ((IDictionary<string, DwgViewPort>)_viewPorts).IsReadOnly;

        public DwgViewPort this[string key] { get => ((IDictionary<string, DwgViewPort>)_viewPorts)[key]; set => ((IDictionary<string, DwgViewPort>)_viewPorts)[key] = value; }

        public void Add(string key, DwgViewPort value) => ((IDictionary<string, DwgViewPort>)_viewPorts).Add(key, value);

        public bool ContainsKey(string key) => ((IDictionary<string, DwgViewPort>)_viewPorts).ContainsKey(key);

        public bool Remove(string key) => ((IDictionary<string, DwgViewPort>)_viewPorts).Remove(key);

        public bool TryGetValue(string key, out DwgViewPort value) => ((IDictionary<string, DwgViewPort>)_viewPorts).TryGetValue(key, out value);

        public void Add(KeyValuePair<string, DwgViewPort> item) => ((IDictionary<string, DwgViewPort>)_viewPorts).Add(item);

        public void Clear() => ((IDictionary<string, DwgViewPort>)_viewPorts).Clear();

        public bool Contains(KeyValuePair<string, DwgViewPort> item) => ((IDictionary<string, DwgViewPort>)_viewPorts).Contains(item);

        public void CopyTo(KeyValuePair<string, DwgViewPort>[] array, int arrayIndex) => ((IDictionary<string, DwgViewPort>)_viewPorts).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, DwgViewPort> item) => ((IDictionary<string, DwgViewPort>)_viewPorts).Remove(item);

        public IEnumerator<KeyValuePair<string, DwgViewPort>> GetEnumerator() => ((IDictionary<string, DwgViewPort>)_viewPorts).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<string, DwgViewPort>)_viewPorts).GetEnumerator();

        #endregion

    }
}
