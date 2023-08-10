using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using TMPro;
using System.Text.RegularExpressions;
using System.Reflection;

public class DataManager : MonoBehaviour
{
    public TextAsset TextData;
    public static Dictionary<string, Dictionary<string, object>> Datas = new();
    public static Dictionary<string, List<string>> textData = new();
    public Dictionary<string, BattleFieldData> battleFieldData = new();
    public Dictionary<string, CharacterData> charSync = new(); 
    public TextAsset t_AttackSkills;
    public TextAsset t_Weapons;
    public TextAsset t_Skills;
    public TextAsset t_HalfSpirits;
    public TextAsset t_Items;
    public TextAsset t_Class;
    public TextAsset t_CharBase;
    public GameObject AttackSkillPrefab;

    private void Awake()
    {
        foreach(string str in TextData.text.Split('\n'))
        {
            string[] t = str.Split('\t');
            if (t[0] == string.Empty)
                continue;
            List<string> langs = new();
            bool i = false;
            foreach(string str2 in t)
            {
                if (!i)
                {
                    i = true;
                    continue;
                }
                langs.Add(str2);
            }
            textData.Add(t[0], langs);
        }
        JObject attackSkillParse = JObject.Parse(t_AttackSkills.text);
        foreach (JToken attackSkill in (JArray)attackSkillParse["AttackSkills"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in attackSkill)
            {
                bool a = false;
                if(jp.Value.Count() > 0)
                {
                    List<string> _d = new List<string>();
                    foreach(JToken j in jp.Value)
                    {
                        _d.Add(j.ToString());
                    }
                    d.Add(jp.Name, _d.ToArray());
                    a = true;
                    continue;
                }
                if(a)
                    continue;
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "attackSkill");
            Datas.Add(attackSkill["id"].ToString(), d);
        }

        JObject weaponParse = JObject.Parse(t_Weapons.text);
        foreach (JToken weapon in (JArray)weaponParse["Weapon"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in weapon)
            {
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "weaponParse");
            Datas.Add(weapon["id"].ToString(), d);
        }

        JObject skillsParse = JObject.Parse(t_Skills.text);
        foreach (JToken skill in (JArray)skillsParse["Skill"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in skill)
            {
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "skill");
            Datas.Add(skill["id"].ToString(), d);
        }

        JObject halfSpiritParse = JObject.Parse(t_HalfSpirits.text);
        foreach (JToken halfSpirit in (JArray)halfSpiritParse["HalfSpirit"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in halfSpirit)
            {
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "halfSpirit");
            Datas.Add(halfSpirit["id"].ToString(), d);
        }

        JObject itemsParse = JObject.Parse(t_Items.text);
        foreach (JToken items in (JArray)itemsParse["Items"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in items)
            {
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "items");
            Datas.Add(items["id"].ToString(), d);
        }

        JObject classParse = JObject.Parse(t_Class.text);
        foreach (JToken _class in (JArray)classParse["Class"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in _class)
            {
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "class");
            Datas.Add(_class["id"].ToString(), d);
        }

        JObject charBaseParse = JObject.Parse(t_CharBase.text);
        foreach (JToken _charBase in (JArray)charBaseParse["CharacterDatas"])
        {
            Dictionary<string, object> d = new();
            foreach(JProperty jp in _charBase)
            {
                d.Add(jp.Name, jp.Value);
            }
            d.Add("dataType", "charBase");
            Datas.Add(_charBase["id"].ToString(), d);
        }

        UIDataSetup();
    }
    public void UIDataSetup()
    {
        GameObject skList = GameManager.Inst.uiCon.AttackSkillList.gameObject;
        foreach (KeyValuePair<string, Dictionary<string, object>> aSkill in Datas)
        {
            if(aSkill.Value["dataType"].ToString() == "attackSkill")
            {
                GameObject skItem = Instantiate(AttackSkillPrefab, skList.transform);
                skItem.name = aSkill.Value["id"].ToString();
                skItem.GetComponent<SkillListItem>().id = aSkill.Value["id"].ToString();
                skItem.GetComponentInChildren<TextMeshProUGUI>().text = DescConvert(aSkill.Value["id"].ToString())[0];
            }
        }
    }
    public CharacterData UnitFirstSetup(string id)
    {
        CharacterData c = new();

        string cl = Datas[id]["Class"].ToString();
        c.id = id;
        c.Lv = ToInt(Datas[id]["Lv"]);
        c.Lv = 51;
        c.HP = ToFloat(Datas[id]["HP"]) + c.Lv * (ToFloat(Datas[id]["HPUp"]) + ToFloat(Datas[cl]["HPUp"]));
        c.SP = ToFloat(Datas[id]["SP"]) + c.Lv * (ToFloat(Datas[id]["SPUp"]) + ToFloat(Datas[cl]["SPUp"]));
        c.Str = ToFloat(Datas[id]["Str"]) + c.Lv * (ToFloat(Datas[id]["StrUp"]) + ToFloat(Datas[cl]["StrUp"]));
        c.Atk = ToFloat(Datas[id]["Atk"]) + c.Lv * (ToFloat(Datas[id]["AtkUp"]) + ToFloat(Datas[cl]["AtkUp"]));
        c.Ryn = ToFloat(Datas[id]["Ryn"]) + c.Lv * (ToFloat(Datas[id]["RynUp"]) + ToFloat(Datas[cl]["RynUp"]));
        c.Spd = ToFloat(Datas[id]["Spd"]) + c.Lv * (ToFloat(Datas[id]["SpdUp"]) + ToFloat(Datas[cl]["SpdUp"]));
        c.Dex = ToFloat(Datas[id]["Dex"]) + c.Lv * (ToFloat(Datas[id]["DexUp"]) + ToFloat(Datas[cl]["DexUp"]));
        c.Def = ToFloat(Datas[id]["Def"]) + c.Lv * (ToFloat(Datas[id]["DefUp"]) + ToFloat(Datas[cl]["DefUp"]));
        c.Res = ToFloat(Datas[id]["Res"]) + c.Lv * (ToFloat(Datas[id]["ResUp"]) + ToFloat(Datas[cl]["ResUp"]));
        c.Dpn = 100;
        c.characterClass = cl;
        c.ClassEXP = ToInt(Datas[cl]["exp"]);
        c.MaxExp = c.Lv * 5;
        return c;
    }
    public static T[] ConvertJToken<T>(JToken value)
    {
        List<T> values = new();
        int i = 0;
        foreach (JToken val in value)
        {
            values.Add((T)System.Convert.ChangeType(val, typeof(T)));
            i++;
        }
        return values.ToArray();
    }
    public static string[] DescConvert(string Id)
    {
        string name = "";
        string desc = "";
        name = textData[Id][GameManager.langCode];

        if(!textData.ContainsKey($"desc_{Id}"))
         return new string[] {name, desc};

        desc = textData[$"desc_{Id}"][GameManager.langCode];
        Regex regex = new Regex(@"<|>");
        string[] descs = desc.Split(new char[]{'<', '>'});
        desc = string.Empty;
        for(int i = 0; i < descs.Length; i++)
        {
            if(i%2 == 1)
            {
                descs[i] = ((string[])Datas[Id]["val"])[System.Convert.ToInt32(Regex.Replace(descs[i], @"[^0-9]", ""))];
                if(descs[i].IndexOf("**") > -1)
                {
                    descs[i] = textData[descs[i].Replace("**", "")][GameManager.langCode];
                }
                else if(descs[i].IndexOf('*') > - 1)
                {
                    descs[i] = descs[i].Replace("*", "");
                    if(descs[i].IndexOf('[') > -1)
                    {
                        descs[i] = ((object[])Datas[Id][descs[i].Split('[')[0]])[System.Convert.ToInt32(Regex.Replace(descs[i], @"[^0-9]", ""))].ToString();
                    }
                    else
                    {
                        descs[i] = Datas[Id][descs[i]].ToString();
                    }
                }
            }
            desc += descs[i];
        }
        desc = desc.Replace("/n", "\n");
        return new string[] {name, desc};
    }
    public static int[] ToIntArr(object d)
    {
        string[] a = (string[])d;
        int[] r = new int[a.Length];
        for(int i = 0; i < r.Length; i++)
        {
            r[i] = System.Convert.ToInt32(a[i]);
        }
        return r;
    }
    public static float[] ToFloatArr(object d)
    {
        string[] a = (string[])d;
        float[] r = new float[a.Length];
        for(int i = 0; i < r.Length; i++)
        {
            r[i] = (float)System.Convert.ToDouble(a[i]);
        }
        return r;
    }
    public static bool[] ToBoolArr(object d)
    {
        string[] a = (string[])d;
        bool[] r = new bool[a.Length];
        for(int i = 0; i < r.Length; i++)
        {
            switch(a[i])
            {
                case "false" :
                    r[i] = false;
                    break;
                case "true" :
                    r[i] = true;
                    break;
            }
        }
        return r;
    }
    public static int ToInt(object d)
    {
        return System.Convert.ToInt32(d.ToString());
    }
    public static float ToFloat(object d)
    {
        return (float)System.Convert.ToDouble(d.ToString());
    }
    public static bool ToBool(object d)
    {
        return d.ToString() == "true" ? true : false;
    }
}