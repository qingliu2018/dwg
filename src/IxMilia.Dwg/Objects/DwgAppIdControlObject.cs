﻿using System.Collections;
using System.Collections.Generic;
using IxMilia.Dwg.Collections;

namespace IxMilia.Dwg.Objects
{
    public partial class DwgAppIdControlObject : IDictionary<string, DwgAppId>
    {
        private IDictionary<string, DwgAppId> _appIds = new StringDictionary<DwgAppId>(ignoreCase: true);

        internal override IEnumerable<DwgObject> ChildItems => _appIds.Values;

        internal override void PreWrite()
        {
            foreach (var appId in _appIds.Values)
            {
                _entityHandles.Add(new DwgHandleReference(DwgHandleReferenceCode.None, appId.Handle.HandleOrOffset));
                appId.AppIdControlHandle = new DwgHandleReference(DwgHandleReferenceCode.HardPointer, Handle.HandleOrOffset);
            }
        }

        internal override void PoseParse(BitReader reader, DwgObjectCache objectCache)
        {
            _appIds.Clear();
            foreach (var appIdHandle in _entityHandles)
            {
                if (appIdHandle.Code != DwgHandleReferenceCode.None)
                {
                    throw new DwgReadException("Incorrect child app id handle code.");
                }

                var appId = objectCache.GetObject<DwgAppId>(reader, appIdHandle.HandleOrOffset);
                if (appId.AppIdControlHandle.HandleOrOffset != Handle.HandleOrOffset)
                {
                    throw new DwgReadException("Incorrect app id control object parent handle reference.");
                }

                _appIds.Add(appId.Name, appId);
            }
        }

        public void Add(DwgAppId appId) => Add(appId.Name, appId);

        #region IDictionary<string, DwgAppId> implementation

        public ICollection<string> Keys => ((IDictionary<string, DwgAppId>)_appIds).Keys;

        public ICollection<DwgAppId> Values => ((IDictionary<string, DwgAppId>)_appIds).Values;

        public int Count => ((IDictionary<string, DwgAppId>)_appIds).Count;

        public bool IsReadOnly => ((IDictionary<string, DwgAppId>)_appIds).IsReadOnly;

        public DwgAppId this[string key] { get => ((IDictionary<string, DwgAppId>)_appIds)[key]; set => ((IDictionary<string, DwgAppId>)_appIds)[key] = value; }

        public void Add(string key, DwgAppId value) => ((IDictionary<string, DwgAppId>)_appIds).Add(key, value);

        public bool ContainsKey(string key) => ((IDictionary<string, DwgAppId>)_appIds).ContainsKey(key);

        public bool Remove(string key) => ((IDictionary<string, DwgAppId>)_appIds).Remove(key);

        public bool TryGetValue(string key, out DwgAppId value) => ((IDictionary<string, DwgAppId>)_appIds).TryGetValue(key, out value);

        public void Add(KeyValuePair<string, DwgAppId> item) => ((IDictionary<string, DwgAppId>)_appIds).Add(item);

        public void Clear() => ((IDictionary<string, DwgAppId>)_appIds).Clear();

        public bool Contains(KeyValuePair<string, DwgAppId> item) => ((IDictionary<string, DwgAppId>)_appIds).Contains(item);

        public void CopyTo(KeyValuePair<string, DwgAppId>[] array, int arrayIndex) => ((IDictionary<string, DwgAppId>)_appIds).CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<string, DwgAppId> item) => ((IDictionary<string, DwgAppId>)_appIds).Remove(item);

        public IEnumerator<KeyValuePair<string, DwgAppId>> GetEnumerator() => ((IDictionary<string, DwgAppId>)_appIds).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<string, DwgAppId>)_appIds).GetEnumerator();

        #endregion

    }
}
