using System;
using System.Collections.Generic;
using UnityEngine;
namespace Encounter.NightDance.Core
{
    public class MinHeap<T> where T : IComparable<T>
    {
        private const int ROOTINDEX = 1;
        List<T> heap = new();
        public int Count => heap.Count - 1;
        public MinHeap()
        {
            heap.Add(default(T));
        }
        public void Push(T item)
        {
            heap.Add(item);
            UpHeap(heap.Count - 1);
        }
        public T Pop()
        {
            if(Count <= 0) throw new Exception("힙이 비어있습니다. Pop을 할 수 없습니다.");
            T root = heap[ROOTINDEX];
            heap[ROOTINDEX] = heap[Count];
            heap.RemoveAt(Count);
            if(Count > 0)
            {
                DownHeap(ROOTINDEX);
            }
            return root;
        }
        /// <summary>
        /// 선택한 노드보다 부모가 큰 경우 교환, 반복(삽입+정렬)
        /// </summary>
        /// <param name="index"></param>
        private void UpHeap(int index)
        {
            int parentIndex = index/2;
            while(parentIndex > 0 && heap[parentIndex].CompareTo(heap[index]) > 0)
            {
                (heap[parentIndex], heap[index]) = (heap[index], heap[parentIndex]);
                index = parentIndex;
                parentIndex = index/2;
            }
        }
        private void DownHeap(int index)
        {
            while(true)
            {
                int leftIndex = index * 2;
                int rightIndex = index * 2 + 1;
                int smallest = index;
                if(leftIndex < heap.Count && heap[leftIndex].CompareTo(heap[smallest]) < 0) smallest = leftIndex;
                if(rightIndex < heap.Count && heap[rightIndex].CompareTo(heap[smallest]) < 0) smallest = rightIndex;
                if(smallest == index) break;
                (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                index = smallest;
            }
        }
    }
}