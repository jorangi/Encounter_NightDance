using UnityEngine;

[CreateAssetMenu(fileName = "PurificationRule", menuName = "Scriptable Objects/PurificationRule")]
public class PurificationRule : ScriptableObject
{
    public int maxPurification = 100;
    /// <summary>
    /// 정화 계산: 대상 통제 - 정화자 정신
    /// </summary>
    /// <param name="victim_control"></param>
    /// <param name="caster_mine"></param>
    /// <returns></returns>
    public static int PureCalculate(int victim_control, int caster_mental) => Mathf.Max(1, victim_control - caster_mental);
}