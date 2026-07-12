using System;
using System.Collections.Generic;

namespace Encounter.NightDance.Core
{
    public class MultikeyMap<TKey1, TKey2, TValue>
    {
        private readonly Dictionary<TKey1, TValue> _key1ToValue = new();
        private readonly Dictionary<TKey2, TValue> _key2ToValue = new();
        private readonly Dictionary<TValue, (TKey1 Key1, TKey2 Key2)> _valueToKeys = new();

        public void Add(TValue value, TKey1 key1, TKey2 key2)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (key1 == null) throw new ArgumentNullException(nameof(key1));
            if (key2 == null) throw new ArgumentNullException(nameof(key2));

            if (_key1ToValue.ContainsKey(key1))
            {
                throw new ArgumentException($"Already registered key1: {key1}");
            }
            if (_key2ToValue.ContainsKey(key2))
            {
                throw new ArgumentException($"Already registered key2: {key2}");
            }
            if (_valueToKeys.ContainsKey(value))
            {
                throw new ArgumentException($"Already registered value: {value}");
            }

            _key1ToValue[key1] = value;
            _key2ToValue[key2] = value;
            _valueToKeys[value] = (key1, key2);
        }

        public bool TryGetValue(TKey1 key, out TValue value)
        {
            return _key1ToValue.TryGetValue(key, out value);
        }

        public bool TryGetValue(TKey2 key, out TValue value)
        {
            return _key2ToValue.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey1 key)
        {
            return _key1ToValue.ContainsKey(key);
        }

        public bool ContainsKey(TKey2 key)
        {
            return _key2ToValue.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _valueToKeys.ContainsKey(value);
        }

        public bool Remove(TKey1 key1)
        {
            if (_key1ToValue.TryGetValue(key1, out TValue value))
            {
                if (_valueToKeys.TryGetValue(value, out var keys))
                {
                    _key2ToValue.Remove(keys.Key2);
                    _valueToKeys.Remove(value);
                }
                _key1ToValue.Remove(key1);
                return true;
            }
            return false;
        }

        public bool Remove(TKey2 key2)
        {
            if (_key2ToValue.TryGetValue(key2, out TValue value))
            {
                if (_valueToKeys.TryGetValue(value, out var keys))
                {
                    _key1ToValue.Remove(keys.Key1);
                    _valueToKeys.Remove(value);
                }
                _key2ToValue.Remove(key2);
                return true;
            }
            return false;
        }

        public bool RemoveValue(TValue value)
        {
            if (_valueToKeys.TryGetValue(value, out var keys))
            {
                _key1ToValue.Remove(keys.Key1);
                _key2ToValue.Remove(keys.Key2);
                _valueToKeys.Remove(value);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _key1ToValue.Clear();
            _key2ToValue.Clear();
            _valueToKeys.Clear();
        }

        public int Count => _valueToKeys.Count;

        public IEnumerable<TKey1> Keys1 => _key1ToValue.Keys;
        public IEnumerable<TKey2> Keys2 => _key2ToValue.Keys;
        public IEnumerable<TValue> Values => _valueToKeys.Keys;
    }

    /* Legacy MultikeyMap Implementation
    public class MultikeyMap<TValue>
    {
        /// <summary>
        /// 모든 타입의 key를 허용하기 위해 Key를 object로 생성한 Dictionary
        /// </summary>
        private readonly Dictionary<object, TValue> _keyToValue = new();
        /// <summary>
        /// Value가 어떤 타입의 Key들을 갖고 있는지 추적하기 위해 object Hashset을 값으로 갖는 Dictionary
        /// </summary>
        private readonly Dictionary<TValue, HashSet<object>> _valueToKeys = new();
        public void Add(TValue value, params object[] keys)
        {
            if (keys == null)
            {
                throw new ArgumentException("MultiKeyMap은 최소 1개 이상의 key가 필요합니다.");
            }
            if(!_valueToKeys.TryGetValue(value, out var registeredKeys))
            {
                registeredKeys = new HashSet<object>();
                _valueToKeys[value] = registeredKeys;
            }
            foreach (var key in keys)
            {
                if (key == null) continue;
                if (_keyToValue.ContainsKey(key))
                {
                    throw new Exception($"이미 등록된 키입니다: {key}");
                }
                _keyToValue[key] = value;
                registeredKeys.Add(key);
            }
        }
        public bool TryGetValue(object key, out TValue value) => _keyToValue.TryGetValue(key, out value);
        public bool Remove(object key)
        {
            if(_keyToValue.TryGetValue(key, out TValue value))
            {
                if(_valueToKeys.TryGetValue(value, out var keys))
                {
                    keys.Remove(key);
                    if(keys.Count == 0)
                    {
                        _valueToKeys.Remove(value);
                    }
                }
                _keyToValue.Remove(key);
                return true;
            }
            return false;
        }
        public void Clear()
        {
            _keyToValue.Clear();
            _valueToKeys.Clear();
        }
        public int Count => _valueToKeys.Count;
        public bool ContainsKey(object key) => _keyToValue.ContainsKey(key);
        public bool ContainsValue(TValue value) => _valueToKeys.ContainsKey(value);
        public int CountOfKey(TValue value)
        {
            if(_valueToKeys.TryGetValue(value, out var keys))
            {
               return keys.Count; 
            }
            throw new Exception("해당 값이 존재하지 않습니다.");
        }
        public IEnumerable<object> Keys => _keyToValue.Keys;
        public IEnumerable<TValue> Values =>_keyToValue.Values;
        public IEnumerable<TValue> GetValues(object key)
        {
            if(_keyToValue.TryGetValue(key, out var value))
            {
                yield return value;
            }
        }
        public IEnumerable<TValue> GetValues(params object[] keys)
        {
            if(keys == null) return GetAllValues();
            var values = new List<TValue>();
            foreach(var key in keys)
            {
                if(_keyToValue.TryGetValue(key, out TValue value))
                {
                    values.Add(value);
                }
            }
            return values;
        }
        public IEnumerable<TValue> GetAllValues() => _valueToKeys.Keys;
        public TValue this[object key]
        {
            get
            {
                if(_keyToValue.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                throw new Exception($"해당 키({key})를 가진 값이 없습니다.");
            }
            set
            {
                if(key == null) return;
                if(_keyToValue.TryGetValue(key, out TValue oldValue))
                {
                    if(EqualityComparer<TValue>.Default.Equals(oldValue, value)) return;
                    if(_valueToKeys.TryGetValue(oldValue, out var oldKeys))
                    {
                        oldKeys.Remove(key);
                        if(oldKeys.Count == 0) _valueToKeys.Remove(oldValue);
                    }
                }
                _keyToValue[key] = value;
                if(!_valueToKeys.TryGetValue(value, out var registeredKeys))
                {
                    registeredKeys = new HashSet<object>();
                    _valueToKeys[value] = registeredKeys;
                }
                registeredKeys.Add(key);
            }
        }
    }
    */
}