using UnityEngine;

[CreateAssetMenu(fileName = "PurificationRule", menuName = "Scriptable Objects/PurificationRule")]
public class PurificationRule : ScriptableObject
{
    public float maxPurification = 100.0f;
    public float PureCalculate(float victim_control, float caster_mine) => Mathf.Max(1, victim_control - caster_mine);
}