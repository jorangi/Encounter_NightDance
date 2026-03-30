using System.Collections.Generic;
using System.ComponentModel;
using Encounter.NightDance.Status;
using UnityEngine;

namespace Encounter.NightDance.Core.Datas
{
    public abstract class EntityData: ScriptableObject
    {
        [SerializeField]private string _mappingId;
        /// <summary>
        /// 백엔드/DB/Addressables용 매핑 아이디, wp_sword_001 이런식으로 string 기반
        /// </summary>
        public string MappingId => _mappingId;
        [SerializeField, ReadOnly(true)]private uint _id;
        /// <summary>
        /// 런타임용 uint 타입 아이디, 매핑 아이디로부터 고정된 해시값 사용, 런타임에서 string보다는 int가 빠름
        /// </summary>
        public uint Id => _id;
        public string Name;
        public string Description;
        /// <summary>
        /// 매핑 아이디로부터 해시값을 생성하여 아이디로 사용, 인스펙터의 값이 변경될 때마다 호출되는 라이프 사이클 함수를 사용
        /// </summary>
        protected virtual void OnValidate()
        {
            if(Application.isPlaying || string.IsNullOrEmpty(_mappingId)) return;
            uint newId = Murmur3Hash.GetHash(_mappingId);
            if(_id != newId)
            {
                _id = newId;
            }
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}