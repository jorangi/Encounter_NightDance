using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [SerializeField] private int maxHp = 100;
    private int curHp;
    public int MaxHp => maxHp;
    public int CurHp => curHp;
}
