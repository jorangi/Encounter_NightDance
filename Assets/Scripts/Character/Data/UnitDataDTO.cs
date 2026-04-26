using UnityEngine;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class UnitDataDTO
{
    [JsonProperty("id")]
    [Header("id")]
    public string id;
    [Header("기본 데이터")]
    [JsonProperty("base_stat")]
    public Dictionary<string, int> base_stats;
    [JsonProperty("growth_stat")]
    public Dictionary<string, int> growth_stats;
    [Header("마지막 업데이트 시간")]
    [JsonProperty("updated_at")]
    public DateTime updated_at;
}