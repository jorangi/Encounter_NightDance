using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitDataDTO", menuName = "Scriptable Objects/UnitDataDTO")]
public class UnitDataDTO : ScriptableObject
{
    [Header("id")]
    public string id;
    [Header("기본 데이터")]
    public Dictionary<string, int> base_stats;
    public Dictionary<string, int> growth_stats;
    [Header("마지막 업데이트 시간")]
    public DateTime updated_at;
}
// public class UnitData : ScriptableObject
// {
//     [Header("기본 데이터")]
//     [SerializeField] private int maxLife = 100;
//     public int MaxLife => maxLife;
//     [SerializeField] private int maxMind = 100;
//     public int MaxMind => maxMind;
//     [SerializeField] private int intensity = 0;
//     public int Intensity => intensity;
//     [SerializeField] private int control = 0;
//     public int Control => control;
//     [SerializeField] private int speed = 1;
//     public int Speed => speed;
//     [SerializeField] private int mobility = 5;
//     public int Mobility => mobility;

//     [Header("성향 데이터")]
//     [SerializeField] private int containment = 0;
//     public int Containment => containment;
//     [SerializeField] private int extraction = 0;
//     public int Extraction => extraction;
//     [SerializeField] private int tuning = 0;
//     public int Tuning => tuning;
//     [SerializeField] private int observation = 0;
//     public int Observation => observation;

// }
