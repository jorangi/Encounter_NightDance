using System;
using System.Collections.Generic;
using Encounter.NightDance.Core.Features;
using R3;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    public interface IUnitCore
    {
        /// <summary>
        /// 유닛의 위치, 대부분의 경우 TileObject클래스에서 구현됨
        /// </summary>
        public Vector2Int Pos { get; set; }
        public Observable<Vector2Int> OnPosChanged { get; }
        /// <summary>
        /// 유닛이 갖는 특성을 제너릭으로 반환하는 메서드
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetFeature<T>() where T : class, IUnitFeature;
        ///<summary>
        /// 유닛에게 특성을 추가하는 메서드
        /// </summary>        
        public void AddFeature<T>(T feature) where T : class, IUnitFeature;
        /// <summary>
        /// 유닛의 특성을 제거하는 메서드
        /// </summary>
        public void RemoveFeature<T>() where T : class, IUnitFeature;
        /// <summary>
        /// 유닛의 특성을 전부 제거하는 메서드
        /// </summary>
        public void ClearFeature();
        /// <summary>
        /// 대상이 지닌 모든 특성을 반환하는 메서드(일단은 구현)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetFeatures<T>() where T : class, IUnitFeature;
    }
}