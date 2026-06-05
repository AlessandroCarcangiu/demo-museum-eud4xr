using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SerializableDictionaryEntry> entries = new List<SerializableDictionaryEntry>();

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        entries.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            entries.Add(new SerializableDictionaryEntry(pair.Key, pair.Value));
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        this.Clear();

        foreach (SerializableDictionaryEntry entry in entries)
        {
            this.Add(entry.Key, entry.Value);
        }
    }

    [Serializable]
    private struct SerializableDictionaryEntry
    {
        [SerializeField]
        private TKey key;

        public TKey Key => key;

        [SerializeField]
        private TValue value;

        public TValue Value => value;

        public SerializableDictionaryEntry(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}