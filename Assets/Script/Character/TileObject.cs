using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CharacterData
{
    public Character Char;
    public string group = "EltCamp";
    public string id;
    public int Lv;
    private float curHP;
    public float CurHP
    {
        get => curHP;
        set
        {
            value = Mathf.Clamp(value, 0, HP);
            if(Char != null && value != curHP)
            {
                Char.SetHPBar(value);
            }
            curHP = value;
        }
    }
    private float hp;
    public float HP
    {
        get => hp;
        set
        {
            hp = value;
        }
    }
    private float sp;
    public float SP
    {
        get => sp;
        set
        {
            sp = value;
        }
    }
    public int Mov;
    private float str;
    public float Str
    {
        get => str;
        set
        {
            str = value;
        }
    }
    private float atk;
    public float Atk
    {
        get => atk;
        set
        {
            atk = value;
        }
    }
    private float ryn;
    public float Ryn
    {
        get => ryn;
        set
        {
            ryn = value;
        }
    }
    private float dex;
    public float Dex
    {
        get => dex;
        set
        {
            dex = value;
        }
    }
    private float spd;
    public float Spd
    {
        get => spd;
        set
        {
            spd = value;
        }
    }
    private float def;
    public float Def
    {
        get => def;
        set
        {
            def = value;
        }
    }
    private float res;
    public float Res
    {
        get => res;
        set
        {
            res = value;
        }
    }
    private float dpn;
    public float Dpn
    {
        get => dpn;
        set
        {
            dpn = value;
        }
    }
    private float rpn;
    public float Rpn
    {
        get => rpn;
        set
        {
            rpn = value;
        }
    }
    
    public string HalfSpirit = string.Empty;
    private int exp;
    public int EXP
    {
        get => exp;
        set
        {
            exp = value;
        }
    }
    public int MaxExp;
    public List<string> attackSkills = new();
    public List<string> Skills = new();
    public string characterClass;
    public List<string> UnitBonus = new();
    public int ClassEXP;
}
public class Group
{
    public string groupName;
    public string[] allyGroup = new string[0];
}
public class TileObject : MonoBehaviour
{
    #region tileObjectData
    public CharacterData oriData = new();
    public CharacterData realData = new();
    public SpriteRenderer spriteRenderer;
    public string id;
    public SpriteRenderer HealthBar;
    public SpriteRenderer PrevHealthBar;
    public Tiles tiles;
    public Group group = new();
    public Vector2Int POS;
    public Vector2Int tempPOS;
    public virtual void Awake()
    {
        tiles = GameManager.Inst.tiles;
        HealthBar = transform.Find("HealthBar").Find("HealthBar").GetComponent<SpriteRenderer>();
        PrevHealthBar = transform.Find("HealthBar").Find("PrevHealthBar").GetComponent<SpriteRenderer>();
    }
    public virtual void Update()
    {
        switch(Mathf.RoundToInt(tiles.transform.eulerAngles.z)/90)
        {
            case 0:
                transform.Find("Unit").localPosition = new Vector3(0, -0.3f, 0.2f);
                transform.Find("Unit").localEulerAngles = new Vector3(Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, 0, 0);
                break;
            case 1:
                transform.Find("Unit").localPosition = new Vector3(-0.3f, 0, 0.2f);
                transform.Find("Unit").localEulerAngles = new Vector3(0, -Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, -90);
                break;
            case 2:
                transform.Find("Unit").localPosition = new Vector3(0, 0.3f, 0.2f);
                transform.Find("Unit").localEulerAngles = new Vector3(Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, 0, -180);
                break;
            case 3:
                transform.Find("Unit").localPosition = new Vector3(0.3f, 0, 0.2f);
                transform.Find("Unit").localEulerAngles = new Vector3(0, -Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, -270);
                break;
        }
    }
    public virtual void Init(string id)
    {
        this.id = id;
        oriData.id = id;
        SetRealData();
    }
    public void SetPOS(Vector2Int pos)
    {
        tempPOS = new Vector2Int(pos.x, pos.y);
        tiles.tileDatas[tempPOS.y, tempPOS.x].tempTileObject = this;
        Vector2 des = tiles.TilePos(pos);
        transform.localPosition = new Vector3(des.x, des.y, - tiles.tileDatas[pos.y, pos.x].POS.z * 0.51f);
        tiles.tileDatas[pos.y, pos.x].tileObject = this;
        foreach(Character character in tiles.Units)
        {
            if(character.group.groupName != "Player") character.FindTarget();
        }
    }
    public virtual void GetBuff(string id, int turn)
    {

    }
    public void Outlining()
    {
        spriteRenderer.material = tiles.charMat[1];
    }
    public void RemoveOutlining()
    {
        spriteRenderer.material = tiles.charMat[0];
    }
    public virtual void SetHPBar(float hp)
    {
        float ratio = (float)Mathf.FloorToInt(hp) / Mathf.FloorToInt(realData.HP);
        HealthBar.transform.localScale = new Vector2(ratio, 1);
        PrevHealthBar.transform.localScale = new Vector2(ratio, 1);
        if(ratio > 0.25f)
        {
            HealthBar.color = Color.white;
            PrevHealthBar.color = Color.white;
        }
        else
        {
            HealthBar.color = Color.red;
            PrevHealthBar.color = Color.red;
        }
    }
    public virtual void SetPrevHPBar(float hp)
    {
        float ratio = (float)Mathf.FloorToInt(hp) / Mathf.FloorToInt(realData.HP);
        PrevHealthBar.transform.localScale = new Vector2(ratio, 1);
        if(ratio > 0.25f)
        {
            PrevHealthBar.color = Color.white;
        }
        else
        {
            PrevHealthBar.color = Color.red;
        }
    }
    public void SetRealData()
    {
        realData.HP = (int)(oriData.HP);
        realData.SP = (int)(oriData.SP);
        realData.Atk = (int)(oriData.Atk);
        realData.Ryn = (int)(oriData.Ryn);
        realData.Spd = (int)(oriData.Spd);
        realData.Dex = (int)(oriData.Dex);
        realData.Def = (int)(oriData.Def);
        realData.Res = (int)(oriData.Res);
        realData.Dpn = (int)(oriData.Dpn);
        realData.Rpn = (int)(oriData.Rpn);
        realData.Str = (int)(oriData.Str);
    }
    #endregion
    #region BattleState
    public delegate void MethodEff();
    public List<MethodEff> stageStart = new ();
    public List<MethodEff> stageClear = new ();
    public List<MethodEff> turnStart = new ();
    public List<MethodEff> turnEnd = new ();
    public List<MethodEff> stay = new ();
    public List<MethodEff> actStart = new ();
    public List<MethodEff> actEnd = new ();
    public List<MethodEff> battleStart = new ();
    public List<MethodEff> battleEnd = new ();
    public List<MethodEff> attack = new ();
    public List<MethodEff> hit = new ();
    public List<MethodEff> inflictElement = new ();
    public List<MethodEff> recieveElement = new ();
    public List<MethodEff> inflictCC = new ();
    public List<MethodEff> recieveCC = new ();

    public void StageStart()
    {
        foreach(MethodEff m in stageStart)
            m();
    }
    public void StageClear()
    {
        foreach(MethodEff m in stageClear)
            m();
    }
    public virtual void Stay()
    {
        foreach(MethodEff m in stay)
            m();
    }
    public virtual void ActStart()
    {
        foreach(MethodEff m in actStart)
            m();
    }
    public virtual void ActEnd()
    {
        foreach(MethodEff m in actEnd)
            m();
    }
    public virtual void TurnStart()
    {
        foreach(MethodEff m in turnStart)
            m();
    }
    public virtual void TurnEnd()
    {
        POS = tempPOS;
        GameManager.Inst.tiles.FocusChar = null;
        foreach(MethodEff m in turnEnd)
            m();

        
    }
    public void BattleStart()
    {
        //전투 시작
        foreach(MethodEff m in battleStart)
            m();
    }
    public void BattleEnd()
    {
        //전투 종료
        foreach(MethodEff m in battleEnd)
            m();
    }
    public void Attack()
    {
        //매 공격
        foreach(MethodEff m in attack)
            m();
    }
    public void Hit()
    {
        //매 피격
        foreach(MethodEff m in hit)
            m();
    }
    public void InflictElement()
    {
        //속성을 부여
        foreach(MethodEff m in inflictElement)
            m();
    }
    public void RecieveElement()
    {
        //속성을 받음
        foreach(MethodEff m in recieveElement)
            m();
    }
    public void InflictCC()
    {
        //CC를 부여
        foreach(MethodEff m in inflictCC)
            m();
    }
    public void RecieveCC()
    {
        //CC를 받음
        foreach(MethodEff m in recieveCC)
            m();
    }
    #endregion
}
