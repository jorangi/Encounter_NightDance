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
    [SerializeField] private int growthLife = 0;
    public int GrowthLife => growthLife;
    [SerializeField] private int growthMind = 0;
    public int GrowthMind => growthMind;
    [SerializeField] private int growthIntensity = 0;
    public int GrowthIntensity => growthIntensity;
    [SerializeField] private int growthControl = 0;
    public int GrowthControl => growthControl;
    [SerializeField] private int growthSpeed = 0;
    public int GrowthSpeed => growthSpeed;

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
        bool exist_G_Life = growth_stats.TryGetValue("life", out int growthLife);
        this.growthLife = exist_G_Life ? growthLife : 0;
        bool exist_G_Mind = growth_stats.TryGetValue("mind", out int growthMind);
        this.growthMind = exist_G_Mind ? growthMind : 0;
        bool exist_G_Intensity = growth_stats.TryGetValue("intensity", out int growthIntensity);
        this.growthIntensity = exist_G_Intensity ? growthIntensity : 0;
        bool exist_G_Control = growth_stats.TryGetValue("control", out int growthControl);
        this.growthControl = exist_G_Control ? growthControl : 0;
        bool exist_G_Speed = growth_stats.TryGetValue("speed", out int growthSpeed);
        this.growthSpeed = exist_G_Speed ? growthSpeed : 0;
        //기동은 성장X
    }
}
