using Encounter.NightDance.Status;
using R3;
using UnityEngine;

namespace Encounter.NightDance.Core
{
    public class Destination
    {
        private bool isTarget;
        private Vector2Int pos;
        private IFieldObject target;
        public void SetDestination(IFieldObject target)
        {
            this.target = target;
            this.pos = new(-1, -1);
            isTarget = true;
        }
        public void SetDestination(Vector2Int pos)
        {
            this.pos = pos;
            this.target = null;
            isTarget = false;
        }
        public Vector2Int Value => (isTarget && target != null) ? target.Pos : pos;
    }
    /// <summary>
    /// 필드 상의 오브젝트를 나타내는 컴포넌트 클래스
    /// </summary>
    public class Prototype_TileObject : MonoBehaviour, IFieldObject
    {

        private readonly ReactiveProperty<Vector2Int> _onPosChangedSubject = new(new(-1, -1));
        public Observable<Vector2Int> OnPosChanged => _onPosChangedSubject;
        private Vector2Int _pos = new(-1, -1);
        public Vector2Int Pos
        {
            get => _pos;
            set
            {
                _pos = new Vector2Int(
                Mathf.Clamp(value.x, 0, FieldManager.FieldSize.x - 1),
                Mathf.Clamp(value.y, 0, FieldManager.FieldSize.y - 1));
                _onPosChangedSubject.Value = _pos;
            }
        }
        private Vector2 worldPos;
        public Vector2 WorldPos
        {
            get => worldPos;
            set
            {
                worldPos = value;
                transform.position = new Vector3(worldPos.x, 0, worldPos.y);
            }
        }
        public void SetPos(Vector2Int pos)
        {
            Pos = pos;
        }
        public Destination GetCurrentDestination { get; private set; } = new();
        public bool HasDestination => GetCurrentDestination.Value != new Vector2Int(-1, -1);
        public void SetDestination(IFieldObject target)
        {
            GetCurrentDestination.SetDestination(target);
        }
        public void SetDestination(Vector2Int pos)
        {
            GetCurrentDestination.SetDestination(pos);
        }
        public void ClearDestination()
        {
            GetCurrentDestination.SetDestination(new Vector2Int(-1, -1));
        }
    }
}