using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("기본 데이터")]
    [SerializeField] private int maxLife = 100;
    public int MaxLife => maxLife;
    [SerializeField] private int maxMind = 100;
    public int MaxMind => maxMind;
    [SerializeField] private int intensity = 0;
    public int Intensity => intensity;
    [SerializeField] private int control = 0;
    public int Control => control;
    [SerializeField] private int speed = 1;
    public int Speed => speed;
    [SerializeField] private int mobility = 5;
    public int Mobility => mobility;

    [Header("성향 데이터")]
    [SerializeField] private int containment = 0;
    public int Containment => containment;
    [SerializeField] private int extraction = 0;
    public int Extraction => extraction;
    [SerializeField] private int tuning = 0;
    public int Tuning => tuning;
    [SerializeField] private int observation = 0;
    public int Observation => observation;

}
