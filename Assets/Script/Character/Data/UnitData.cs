using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]

public class UnitData : ScriptableObject
{
    [Header("유닛 ID")]
    [SerializeField] private string id;
    public string Id => id;
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
    [Header("성장 데이터")]
    [SerializeField] private float growthLife = 0.0f;
    public float GrowthLife => growthLife;
    [SerializeField] private float growthMind = 0.0f;
    public float GrowthMind => growthMind;
    [SerializeField] private float growthIntensity = 0.0f;
    public float GrowthIntensity => growthIntensity;
    [SerializeField] private float growthControl = 0.0f;
    public float GrowthControl => growthControl;
    [SerializeField] private float growthSpeed = 0.0f;
    public float GrowthSpeed => growthSpeed;

    [Header("성향 데이터")]
    [SerializeField] private int containment = 0;
    public int Containment => containment;
    [SerializeField] private int extraction = 0;
    public int Extraction => extraction;
    [SerializeField] private int tuning = 0;
    public int Tuning => tuning;
    [SerializeField] private int observation = 0;
    public int Observation => observation;

    /// <summary>
    /// 서버 동기화 - 데이터 초기화
    /// </summary>
    /// <param name="id">유닛 ID</param>
    /// <param name="stats">유닛 기본 스탯</param>
    /// <param name="growth_stats">유닛 성장 스탯</param>
    public void Initialize(in string id, IReadOnlyDictionary<string, int> stats, IReadOnlyDictionary<string, int> growth_stats)
    {
        //유닛 id
        this.id = id;

        //기본 스탯
        this.maxLife = stats["life"];
        this.maxMind = stats["mind"];
        this.intensity = stats["intensity"];
        this.control = stats["control"];
        this.speed = stats["speed"];
        this.mobility = stats["mobility"];

        //성장 스탯
        this.growthLife = growth_stats["life"];
        this.growthMind = growth_stats["mind"];
        this.growthIntensity = growth_stats["intensity"];
        this.growthControl = growth_stats["control"];
        this.growthSpeed = growth_stats["speed"];
    }
}
