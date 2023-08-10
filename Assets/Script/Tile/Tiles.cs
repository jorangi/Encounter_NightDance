using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;

public enum TileState
{
    RedTile,
    BlueTile,
    BlankTile,
    OrangeTile
}
public enum Act
{
    Ready,
    Operate,
    Attack,
    SelectSkill,
    Skill,
    SetTarget,
    Item,
    Acting
}
public class BattleClass
{
    public Character caster, target;
    public CharacterData casterData, targetData;
    public string id;
    public Dictionary<string, object> skillData;
    public bool countable;
    public int attackCount, counterAttackCount;
    public int totalDmg, totalCountDmg;
    public int avgAcc;
    public Vector2Int AssumedPOS = Vector2Int.zero;
    public List<BattleData> battleDatas = new();
    public BattleClass(Character caster, Character target, string id)
    {
        this.caster = caster;
        this.target = target;
        this.id = id;
        this.AssumedPOS = caster.tempPOS;
        Setup();
    }
    public BattleClass(Character caster, Character target, string id, Vector2Int AssumedPOS)
    {
        this.caster = caster;
        this.target = target;
        this.id = id;
        this.AssumedPOS = AssumedPOS;
        Setup();    
    }
    public void Setup()
    {
        casterData = caster.realData;
        targetData = target.realData;
        skillData = DataManager.Datas[id];

        int distance = Mathf.Abs(AssumedPOS.x - target.POS.x) + Mathf.Abs(AssumedPOS.y - target.POS.y);

        countable = countable || ((DataManager.ToIntArr(DataManager.Datas[target.realData.attackSkills[0]]["range"])[0]) <= distance) &&
        ((DataManager.ToIntArr(DataManager.Datas[target.realData.attackSkills[0]]["range"])[1] + DataManager.ToInt(DataManager.Datas[target.weapon]["rangeBonus"])) >= distance);
        int counterCount = 0;

        if(id.IndexOf("NormalAttack") > -1)
        {
            attackCount++;
        
            int SpdDiff = (int)((casterData.Spd * Mathf.Clamp(casterData.Str / caster.weight, 0.0f, 1.0f)) - 
                                (targetData.Spd * Mathf.Clamp(targetData.Str / target.weight, 0.0f, 1.0f)));

            if(SpdDiff >= 5) attackCount++;
            if(SpdDiff >= 10) attackCount++;
            if(SpdDiff >= 20) attackCount++;
            if(SpdDiff >= 40) attackCount++;
            if(SpdDiff >= 80) attackCount++;

            BattleData d = new()
            {
                acc = (int)Mathf.Clamp(
                    (caster.realData.Dex + DataManager.ToFloat(DataManager.Datas[caster.weapon]["acc"]) + DataManager.ToFloat(DataManager.Datas[id]["acc"]))
                    - (target.realData.Dex + target.realData.Spd)
                    , 0.0f, 100.0f)
            };
            avgAcc += d.acc;
            d.dmg = GameManager.Inst.tiles.DamageCalc(id, casterData, targetData, skillData["dmgType"].ToString());
            d.InRe = false;
            battleDatas.Add(d);
            totalDmg += d.dmg;
        }
        else if(DataManager.ToInt(skillData["attackCount"]) > -1)
        {
            if(DataManager.ToInt(skillData["attackCount"]) == 0) attackCount++;
            else attackCount = DataManager.ToInt(skillData["attackCount"]);

            BattleData d = new()
            {
                dmg = GameManager.Inst.tiles.DamageCalc(id, casterData, targetData, skillData["dmgType"].ToString()),
                acc = (int)Mathf.Clamp(
                (caster.realData.Dex + DataManager.ToFloat(DataManager.Datas[caster.weapon]["acc"]) + DataManager.ToFloat(DataManager.Datas[id]["acc"]))
                - (target.realData.Dex + target.realData.Spd)
                , 0.0f, 100.0f)
            };
            avgAcc += d.acc;
            d.atkCount = attackCount;
            d.InRe = false;
            battleDatas.Add(d);

                totalDmg += d.dmg;
            for(int i = 0; i < attackCount; i++)
            {
            }
        }
        if(countable)
        {
            BattleData d = new();
            string counterId = targetData.attackSkills[0];
            counterCount ++;

            int SpdDiff = (int)((targetData.Spd * Mathf.Clamp(targetData.Str / target.weight, 0.0f, 1.0f)) - 
                                (casterData.Spd * Mathf.Clamp(casterData.Str / caster.weight, 0.0f, 1.0f)));

            if(SpdDiff >= 5) counterCount++;
            if(SpdDiff >= 10) counterCount++;
            if(SpdDiff >= 20) counterCount++;
            if(SpdDiff >= 40) counterCount++;
            if(SpdDiff >= 80) counterCount++;

            d.acc = (int)Mathf.Clamp(
            (target.realData.Dex + DataManager.ToFloat(DataManager.Datas[caster.weapon]["acc"]))
            - (caster.realData.Dex + caster.realData.Spd)
            , 0.0f, 100.0f);
            d.atkCount = counterCount; 
            counterAttackCount = counterCount;

            d.InRe = true;
            d.dmg = GameManager.Inst.tiles.DamageCalc(counterId, targetData, casterData, DataManager.Datas[counterId]["dmgType"].ToString());
            totalCountDmg += d.dmg;

            battleDatas.Add(d);
        }
        if(id.IndexOf("NormalAttack") > -1)
        {
            if(attackCount > 1)
            {
                BattleData d = new()
                {
                    acc = (int)Mathf.Clamp(
                        (caster.realData.Dex + DataManager.ToFloat(DataManager.Datas[caster.weapon]["acc"]) + DataManager.ToFloat(DataManager.Datas[id]["acc"]))
                        - (target.realData.Dex + target.realData.Spd)
                        , 0.0f, 100.0f),
                    dmg = GameManager.Inst.tiles.DamageCalc(id, casterData, targetData, skillData["dmgType"].ToString()),
                    InRe = false,
                    atkCount = attackCount - 1
                };
                battleDatas.Add(d);
                totalDmg += d.dmg;
                
                avgAcc += d.acc;
            }
                
        }
        avgAcc /= attackCount;
    }
}
public class BattleData
{
    public bool InRe; // Inflict : True, Recieve : False
    public int dmg;
    public int acc;
    public int atkCount;
}
public class Tiles : MonoBehaviour
{
    public List<BattleClass> battleClasses = new();
    public int RandCount = 0;
    public int[] RandomValue = new int[9999];
    public List<Character> Units;
    public List<Character> TurnQueue;
    public Material[] charMat;
    public Ray ray;
    public RaycastHit hit;
    public DataManager DataManager;
    public TileObject FocusChar = null;
    public UIController uiCon;
    public Character TurnChar;
    public Transform Focus;
    public Transform MouseFocus;
    public Vector2Int FocusPos
    {
        get => focusPos;
        set
        {
            if(focusPos != value)
            {
                Character character = (Character)tileDatas[focusPos.y, focusPos.x].tileObject;
                if(character != null)
                    character.IsFocus = false;
                
                Character _character = (Character)tileDatas[value.y, value.x].tileObject;
                if(_character != null)
                    _character.IsFocus = true;
                if(TurnChar.group.groupName == "Player" && TurnAct == Act.SelectSkill)
                {
                    TurnChar.SetPrevHPBar((float)TurnChar.realData.CurHP);
                    
                    if(uiCon.targetsTiles.Length > 0)
                    {
                        foreach(Tile tile in uiCon.targetsTiles)
                        {
                            if(tile.tileObject != null)
                            {
                                Debug.Log((float)tile.tileObject?.realData.CurHP);
                                tile.tileObject?.SetPrevHPBar((float)tile.tileObject?.realData.CurHP);
                            }
                        }
                    }

                    TurnChar.SetTarget(tileDatas[value.y, value.x]);
                }
                PathFindWithCursor(TurnChar.POS, value, false);
            }

            focusPos = value;
        }
    }
    private Vector2Int focusPos;
    public float rot;
    public float sensitive = 5f;
    public Camera cam;
    public Tile[,] tileDatas;
    public GameObject[,] tileObject;
    public Vector2Int size = Vector2Int.zero;
    [SerializeField]
    private GameObject tempTile;
    public Act TurnAct = Act.Ready;
    public List<Tile> Checker = new();
    public List<Tile> SelectedChecker = new();
    public Transform objectParent;
    public List<Tile> PathHistory = new();
    public bool IsEnemiesRange = false;
    public Character WhoFocused = null;

    public void Awake()
    {
        cam = Camera.main;
        rot = -10;
        Init();
    }
    public void Start()
    {
        TurnStart();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            uiCon.CardsSwitching();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            uiCon.TurnQueueSwitching();
        }
        if (FocusChar != null)
        {
            Focus.transform.localPosition = new Vector3(FocusChar.transform.localPosition.x, FocusChar.transform.localPosition.y, Focus.transform.localPosition.z);
        }
        for(int i = 0 ; i < PathHistory.Count; i++)
        {
            if(i == PathHistory.Count - 1)
                break;
            Debug.DrawLine(TilePos(PathHistory[i].POS), TilePos(PathHistory[i+1].POS));
        }
    }
    private void Init()
    {
        for(int i = 0; i < RandomValue.Length; i++)
        {
            RandomValue[i] = Random.Range(0, 101);
        }
        BattleFieldData fD = new();
        CharacterData kain = DataManager.UnitFirstSetup("Kain");
        CharacterData kana = DataManager.UnitFirstSetup("Kana");
        CharacterData herwin = DataManager.UnitFirstSetup("Herwin");
        CharacterData nia = DataManager.UnitFirstSetup("Nia");
        fD.charDatas = new CharacterData[4]{kain, kana, herwin, nia};
        fD.charIds = new string[4]{"Kain", "Kana", "Herwin", "Nia"};
        fD.charPossess = new Vector2Int[4]{new Vector2Int(0, 0), new Vector2Int(5, 0), new Vector2Int(6, 1), new Vector2Int(7, 1)};
        fD.charGroups = new string[4]{"Player", "Enemy", "Enemy", "Enemy"};
        fD.size = new Vector2Int(23, 21);

        DataManager.battleFieldData.Add("TestField", fD);

        SetBattleFieldData("TestField");
        uiCon.UnitDataUISet(DataManager.charSync["Kain"]);
    }
    public void CreateField(Vector2Int size)
    {
        this.size = size;
        tileDatas = new Tile[size.y, size.x];
        tileObject = new GameObject[size.y, size.x];

        Vector2 boxSize = new(size.x - 1, size.y - 1);
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                GameObject tile = Instantiate(tempTile);
                tileObject[i, j] = tile;
                tileDatas[i, j] = new Tile()
                {
                    POS = new Vector3Int(j, i, 0),
                    obj = tile,
                    tileSprite = tile.GetComponent<SpriteRenderer>(),
                    rangeSprite = tile.transform.Find("Range").GetComponent<SpriteRenderer>(),
                    stateSprite = tile.transform.Find("TileState").GetComponent<SpriteRenderer>(),
                    selectedUnitSprite = tile.transform.Find("SelectedUnit").GetComponent<SpriteRenderer>()
                };                
                tile.transform.position = new Vector3(-boxSize.x / 2 + j, boxSize.y / 2 - i, 0.51f - (0.51f * tileDatas[i, j].POS.z));

                tile.name = $"{j}|{i}";
                tile.transform.SetParent(transform);
            }
        }
        rot = 10;
        transform.eulerAngles = new(rot, 0, 0);
    }
    public void SetBattleFieldData(string id)
    {
        BattleFieldData fieldData = DataManager.battleFieldData[id];
        CreateField(fieldData.size);
        for(int i = 0; i < fieldData.charDatas.Count(); i++)
        {
            SetUnit(fieldData.charIds[i], fieldData.charDatas[i], fieldData.charGroups[i], fieldData.charPossess[i]);
        }
        for(int i = 1; i < TurnQueue.Count; i++)
        {
            for(int j = i; j < TurnQueue.Count; j++)
            {
                if(TurnQueue[i].oriData.Spd < TurnQueue[j].oriData.Spd ||
                 (TurnQueue[i].oriData.Spd == TurnQueue[j].oriData.Spd && 
                 TurnQueue[i].group.groupName != "Player" && TurnQueue[j].group.groupName == "Player" ))
                {
                    (TurnQueue[i],TurnQueue[j]) = (TurnQueue[j], TurnQueue[i]);
                }
            }
        }
    }
    public void SetUnit(string id, CharacterData data, string group, Vector2Int POS)
    {
        int index = 0;
        foreach(Transform tr in objectParent)
            if(tr.name.IndexOf(id) > -1) index++;
        GameObject unit = Instantiate(Resources.Load<GameObject>($"Prefab/Battle_Object/{id}"), objectParent);
        unit.name = id + index;
        TileObject obj = unit.GetComponent<TileObject>();
        data.UnitBonus.Add("HalfSpirit");
        obj.oriData = data;
        obj.oriData.Char = (Character)obj;
        obj.realData = obj.oriData;
        obj.realData.CurHP = obj.oriData.HP;
        obj.Init(id); 
        obj.group.groupName = group == "Player" ? group : obj.id;
        obj.group.allyGroup = new string[1]{group};
        obj.SetPOS(POS);
        obj.POS = obj.tempPOS;

        DataManager.charSync.Add(id, obj.realData);
        if (obj.TryGetComponent<Character>(out Character c))
        {
            c.oriData = data;
            Units.Add(c);
            if(!TurnQueue.Contains(c) && (id == "Kain" || id == "_Kana"))
            {
                AddTurn(0, c);
                return;
            }
            AddTurn(c);
        }
    }
    public void AddTurn(int index, Character charD)
    {
        TurnQueue.Insert(index, charD);
    }
    public void AddTurn(Character charD)
    {
        TurnQueue.Add(charD);
    }
    public void RemoveTurn(CharacterData charD)
    {

    }
    public Tile[] PathFind(Character character, Vector2Int start, Vector2Int dest, bool Flyer)
    {
        if (!Flyer && tileDatas[dest.y, dest.x].blocked && tileDatas[dest.y, dest.x].tileObject != null)
            return null;
        foreach (Tile t in tileDatas)
        {
            t.ResetPath();
        }
        List<Tile> openList = new();
        List<Tile> closeList = new();
        openList.Add(tileDatas[start.y, start.x]);
        for (int i = Mathf.Max(0, start.y - 1); i <= Mathf.Min(size.y - 1, start.y + 1); i++)
        {
            for (int j = Mathf.Max(0, start.x - 1); j <= Mathf.Min(size.x - 1, start.x + 1); j++)
            {
                bool isAlly = tileDatas[i,j].tileObject == null || System.Array.IndexOf(character.group.allyGroup, tileDatas[i, j].tileObject.group.groupName) > -1;
                if ((Flyer || (!tileDatas[i, j].blocked && isAlly)) &&
                 isAlly && start != new Vector2Int(j, i) &&
                  Mathf.Abs(i - start.y) + Mathf.Abs(j - start.x) == 1)
                {
                    openList.Add(tileDatas[i, j]);
                    tileDatas[i, j].parentNode = tileDatas[start.y, start.x];
                    tileDatas[i, j].G = 10;
                    tileDatas[i, j].H = Mathf.Abs(dest.x - j) * 10 + Mathf.Abs(dest.y - i) * 10;
                }
            }
        }
        closeList.Add(tileDatas[start.y, start.x]);
        openList.Remove(tileDatas[start.y, start.x]);
        if(openList.Count == 0)
            return null;
        int MinF = int.MaxValue;
        Tile selTile = null;
        foreach (Tile tile in openList)
        {
            if (tile.F < MinF && tile.G % 10 == 0)
            {
                selTile = tile;
                MinF = tile.F;
            }
        }
        if ((Vector2Int)selTile.POS == dest)
        {
            Tile[] result = { selTile };
            return result;
        }
        int qwe = 3000;
        while (openList.Count > 0 && qwe > 0)
        {
            qwe--;
            closeList.Add(selTile);
            openList.Remove(selTile);

            Tile tempTile = null;
            for (int i = Mathf.Max(0, selTile.POS.y - 1); i <= Mathf.Min(size.y - 1, selTile.POS.y + 1); i++)
            {
                for (int j = Mathf.Max(0, selTile.POS.x - 1); j <= Mathf.Min(size.x - 1, selTile.POS.x + 1); j++)
                {
                    if (Mathf.Abs(i - selTile.POS.y) + Mathf.Abs(j - selTile.POS.x) == 1)
                    {
                        if (dest == new Vector2Int(j, i) && tileDatas[i, j].tileObject == null)
                        {
                            tileDatas[i, j].parentNode = selTile;
                            selTile = tileDatas[i, j];
                            List<Tile> nav = new();
                            int asd = 10000;
                            while (asd > 0 && (Vector2Int)selTile.POS != new Vector2Int(start.x, start.y))
                            {
                                nav.Add(selTile);
                                asd--;
                                selTile = selTile.parentNode;
                            }
                            nav.Reverse();
                            return nav.ToArray();
                        }
                        bool isAlly = tileDatas[i,j].tileObject == null || System.Array.IndexOf(character.group.allyGroup, tileDatas[i, j].tileObject.group.groupName) > -1;
                        if ((Flyer || (!tileDatas[i, j].blocked && isAlly)) &&
                         isAlly && start != new Vector2Int(j, i) &&
                          !closeList.Contains(tileDatas[i, j]) && tileDatas[i, j].G > 0)
                        {
                            if(tileDatas[i,j].tileObject != null)
                            {
                                //Debug.Log("! : " + tileDatas[i, j].tileObject.name);
                            }
                            if (!openList.Contains(tileDatas[i, j]))
                            {
                                openList.Add(tileDatas[i, j]);
                            }
                            int oriF = tileDatas[i, j].F;
                            int oriG = tileDatas[i, j].G;
                            int oriH = tileDatas[i, j].H;
                            Vector2Int oriParentV2 = new(-1, -1);
                            if (tileDatas[i, j].parentNode != null)
                                oriParentV2 = (Vector2Int)tileDatas[i, j].parentNode.POS;

                            tileDatas[i, j].parentNode = selTile;
                            tileDatas[i, j].H = Mathf.Abs(dest.x - j) * 10 + Mathf.Abs(dest.y - i) * 10;
                            tileDatas[i, j].G = selTile.G + 10;

                            if (oriParentV2 != new Vector2Int(-1, -1) && oriF < tileDatas[i, j].F)
                            {
                                tileDatas[i, j].parentNode = tileDatas[oriParentV2.y, oriParentV2.x];
                                tileDatas[i, j].G = oriG;
                                tileDatas[i, j].H = oriH;
                            }
                            if (tempTile == null || tileDatas[i, j].F <= tempTile.F)
                            {
                                tempTile = tileDatas[i, j];
                            }
                        }
                    }
                }
            }
            selTile = tempTile;
            int tileF = int.MaxValue;
            if (selTile != null)
                tileF = selTile.F;
            foreach (Tile tile in openList)
            {
                if (tile.F <= tileF)
                {
                    selTile = tile;
                    tileF = tile.F;
                }
            }
        }
        return null;
    }
    public Tile[] PathFindWithCursor(Vector2Int start, Vector2Int dest, bool Flyer)
    {
        if(TurnAct != Act.Ready || start == dest)
            return null;
        if(PathHistory.Count == 0)
        {
            Tile[] pH = PathFind(TurnChar, start, dest, Flyer);
            if(pH == null) return null;
            PathHistory = pH.ToList();
        }
        else
        {
            Tile[] LastPath = PathFind(TurnChar, (Vector2Int)PathHistory[^1].POS, dest, Flyer);

            if(LastPath == null) return null;

            for(int i = 0; i < LastPath.Length; i++)
                PathHistory.Add(LastPath[i]);
            Tile[] CharToFocus = PathFind(TurnChar, TurnChar.POS, dest, Flyer);
            List<Tile> ComparePathHistory = PathHistory;
            List<Tile> TempComparePathHistory = new();

            while(ComparePathHistory.Count > CharToFocus.Length || (Vector2Int)ComparePathHistory[^1].POS != dest)
            {
                TempComparePathHistory.Clear();
                ComparePathHistory.RemoveAt(ComparePathHistory.Count - 1);
                for(int i = 0; i < ComparePathHistory.Count; i++)
                {
                    TempComparePathHistory.Add(ComparePathHistory[i]);
                }
                if(ComparePathHistory.Count == 0)
                {
                    return null;
                }
                LastPath = PathFind(TurnChar, (Vector2Int)ComparePathHistory[^1].POS, dest, Flyer);
                for(int i = 0; i < LastPath.Length; i++)
                {
                    ComparePathHistory.Add(LastPath[i]);
                }
                if(ComparePathHistory.Count > CharToFocus.Length || (Vector2Int)ComparePathHistory[^1].POS != dest)
                {
                    ComparePathHistory.Clear();
                    for(int i = 0; i < TempComparePathHistory.Count; i++)
                    {
                        ComparePathHistory.Add(TempComparePathHistory[i]);
                    }
                }
            }
            PathHistory = ComparePathHistory;
        }
        return PathHistory.ToArray();
    }
    public Tile[] MoveableRange(Character character, Vector2Int start, int movement, bool Flyer, out Tile[] atkTiles)
    {
        int min = -1;
        int max = 0;
        foreach (string attackSkill in character.oriData.attackSkills)
        {
            int[] r = new int[2];
            
            for(int i = 0 ; i < 2; i++)
                r[i] = DataManager.ToIntArr(DataManager.Datas[attackSkill]["range"])[i];
            
            if (r[0] < min || min == -1)
                min = r[0];
            if (r[0] > max)
                max = r[1];

            max += DataManager.ToInt(DataManager.Datas[TurnChar.weapon]["rangeBonus"]);
        }
        if(min == -1)
            min = 0;
        List<Tile> result = new();
        List<Tile> Aresult = new();

        for (int i = Mathf.Max(start.x - (movement + max), 0); i <= Mathf.Min(start.x + movement + max, size.x - 1); i++)
        {
            for (int j = Mathf.Max(start.y - (movement + max), 0); j <= Mathf.Min(start.y + movement + max, size.y - 1); j++)
            {
                if (Mathf.Abs(i - start.x) + Mathf.Abs(j - start.y) > movement)
                {
                    continue;
                }
                Tile[] moveableTiles = PathFind(character, start, new Vector2Int(i, j), Flyer);
                if (moveableTiles == null)
                    continue;
                for (int k = 0; k < Mathf.Min(movement, moveableTiles.Length); k++)
                {
                    Tile[] aTs = CalcAttackableTile(character, (Vector2Int)moveableTiles[k].POS, min, max);
                    foreach(Tile aT in aTs)
                    {
                        Aresult.Add(aT);
                    }
                    result.Add(moveableTiles[k]);
                }
            }
        }
        atkTiles = Aresult.Distinct().ToArray();
        result.Add(tileDatas[start.y, start.x]);
        result.Distinct();
        return result.ToArray();
    }
    
    // public Tile[] MoveableRange(Vector2Int start, int movement, bool Flyer, out Tile[] atkTiles)
    // {
    //     int min = -1;
    //     int max = 0;
    //     foreach (string attackSkill in TurnChar.ori_data.attackSkills)
    //     {
    //         int[] r = new int[2];
            
    //         for(int i = 0 ; i < 2; i++)
    //             r[i] = DataManager.ToIntArr(DataManager.Datas[attackSkill]["range"])[i];
            
    //         if (r[0] < min || min == -1)
    //             min = r[0];
    //         if (r[0] > max)
    //             max = r[1];

    //         max += DataManager.ToInt(DataManager.Datas[TurnChar.weapon]["rangeBonus"]);
    //     }
    //     if(min == -1)
    //         min = 0;
    //     List<Tile> result = new();
    //     List<Tile> Aresult = new();

    //     for (int i = Mathf.Max(start.x - (movement + max), 0); i <= Mathf.Min(start.x + (movement + max), size.x - 1); i++)
    //     {
    //         for (int j = Mathf.Max(start.y - (movement + max), 0); j <= Mathf.Min(start.y + (movement + max), size.y - 1); j++)
    //         {
    //             if (Mathf.Abs(i - start.x) + Mathf.Abs(j - start.y) > movement)
    //                 continue;
    //             Tile[] moveableTiles = PathFind(start, new Vector2Int(i, j), Flyer);
    //             if (moveableTiles == null)
    //                 continue;
    //             for (int k = 0; k < Mathf.Min(movement, moveableTiles.Length); k++)
    //             {
    //                 result.Add(moveableTiles[k]);
    //                 Tile[] aTs = CalcAttackableTile(moveableTiles[k].POS, min, max);
    //                 foreach(Tile aT in aTs)
    //                     Aresult.Add(aT);
    //             }
    //         }
    //     }
    //     atkTiles = Aresult.Distinct().ToArray();
    //     MoveableResult = result.Distinct().ToArray();
    //     if(TurnChar.group.groupName == "Player")
    //     {
    //         foreach(Tile t in AttackableResult)
    //             if(t.tileObject != TurnChar)
    //                 SetTileState(t.POS, TileState.RedTile);
    //         foreach(Tile t in MoveableResult)
    //             SetTileState(t.POS, TileState.BlueTile);
    //     }
    //     return MoveableResult;
    // }
    public void TileSet(Vector2Int pos, bool moveable, string id)
    {
        GameObject t = tileObject[pos.y, pos.x];
        Tile d = tileDatas[pos.y, pos.x];
        d.blocked = !moveable;
    }
    public void SetTileState(Tile tile, TileState state)
    {
        if (state == TileState.RedTile)
            tile.stateSprite.color = Color.red;
        else if (state == TileState.BlankTile)
            tile.stateSprite.color = new Color(0, 0, 0, 0);
        else if (state == TileState.BlueTile)
            tile.stateSprite.color = Color.blue;
        else if (state == TileState.OrangeTile)
            tile.stateSprite.color = new Color(0.988f, 0.372f, 0.105f);
    }
    public Vector3 TilePos(Vector3Int pos)
    {
        Vector2 boxSize = new(size.x - 1, size.y - 1);
        return new Vector3(-boxSize.x / 2 + pos.x, boxSize.y / 2 - pos.y, - 0.51f * pos.z);
    }
    public Vector3 TilePos(Vector2Int pos)
    {
        Vector2 boxSize = new(size.x - 1, size.y - 1);
        if(tileDatas[pos.y, pos.x] != null)
            return new Vector3(-boxSize.x / 2 + pos.x, boxSize.y / 2 - pos.y, tileDatas[pos.y, pos.x].POS.z);
        return new Vector3(-boxSize.x / 2 + pos.x, boxSize.y / 2 - pos.y, 0);
    }
    public int DamageCalc(string id, CharacterData caster, CharacterData target, string dmgType)
    {
        int power = DataManager.ToInt(DataManager.Datas[id]["dmg"]);
        float UnitBonusDamage = 1.0f;
        string[] arr;
        if ((DataManager.Datas[id]["unitBonus"]) is Newtonsoft.Json.Linq.JArray)
            arr = ((JArray)DataManager.Datas[id]["unitBonus"]).ToObject<string[]>();
        else
            arr = ((string[])(DataManager.Datas[id]["unitBonus"]));
        if (arr.Length > 0)
        {
            foreach(string unitBonus in arr)
                UnitBonusDamage *= target.UnitBonus.IndexOf(unitBonus) > -1 ? UnitBonusDamage == 1.0f ? 2.0f : 1.5f : 1.0f;
        }
        return dmgType switch
        {
            "p" => Mathf.Max(0, Mathf.FloorToInt((caster.Atk - (target.Def * (1 - caster.Dpn * 0.01f))) * power * 0.01f * UnitBonusDamage)),
            "r" => Mathf.Max(0, Mathf.FloorToInt((caster.Ryn - (target.Res * (1 - caster.Rpn * 0.01f))) * power * 0.01f * UnitBonusDamage)),
            "h" => Mathf.Min(0, -Mathf.FloorToInt(caster.Ryn * power * 0.01f)),
            _ => 0,
        };
    }
    public void TurnStart()
    {
        foreach(TileObject obj in Units)
        {
            ((Character)obj).WholeTurnStart();
        }

        PathHistory.Clear();
        TurnChar.TurnStart();
        Focus.transform.localPosition = new Vector3(TurnChar.transform.localPosition.x, TurnChar.transform.localPosition.y, TurnChar.transform.localPosition.z - 0.05f);
        uiCon.SyncTurnQueue();
        foreach(TileObject obj in Units)
        {
            SpriteRenderer sp = obj.transform.Find("Mark").GetComponent<SpriteRenderer>();
            if(TurnChar.group == obj.group)
                sp.color = Color.white;
            else if(System.Array.IndexOf(TurnChar.group.allyGroup, obj.group) > -1)
                sp.color = Color.green;
            else if(System.Array.IndexOf(TurnChar.group.allyGroup, obj.group) == -1)
                sp.color = Color.red;
            
        }
    }
    public void TurnCancle()
    {
        uiCon.selectSkillBackupTiles = null;
        uiCon.backupTiles = new Tile[0];
        TurnChar.TurnCancle();
        uiCon.ActionsOn();
    }
    public void TurnEnd()
    {
        foreach (Tile t in TurnChar.AttackableTiles)
            SetTileState(t, TileState.BlankTile);
    }
    public void ActStay()
    {
        TurnChar.ActStay();
    }
    public TileObject[] NearUnit(TileObject target, int min_range, int max_range)
    {
        List<TileObject> result = new();
        for (int i = target.POS.y - max_range; i < target.POS.y + max_range; i++)
        {
            for (int j = target.POS.x - max_range; j < target.POS.x + max_range; j++)
            {
                if(tileDatas[i, j].tileObject != null && Mathf.Abs(i - target.POS.y) + Mathf.Abs(j - target.POS.x) >= min_range && Mathf.Abs(i - target.POS.y) + Mathf.Abs(j - target.POS.x) <= max_range )
                {
                    result.Add(tileDatas[i, j].tileObject);
                }
            }
        }
        return result.ToArray();
    }
    public Tile[] CalcAttackableTile(Character character, Vector2Int pos, int min, int max)
    {
        List<Tile> t = new();
        for (int i = Mathf.Max(0, pos.y - max); i <= Mathf.Min(pos.y + max, size.y - 1); i++)
        {
            for (int j = Mathf.Max(0, pos.x - max); j <= Mathf.Min(pos.x + max, size.x - 1); j++)
            {
                if (Mathf.Abs(i - pos.y) + Mathf.Abs(j - pos.x) <= max && Mathf.Abs(i - pos.y) + Mathf.Abs(j - pos.x) >= min)
                {
                    Tile tile = tileDatas[i, j];
                    if(tile.tileObject != character)
                    {
                        t.Add(tile);
                    }
                    else
                    {
                        if(character.POS != character.tempPOS) t.Add(tile);
                        else continue;
                    }
                }
            }
        }
        return t.ToArray();
    }
    public void BattleRun(BattleClass battleClass)
    {
        battleClass.caster.ActStart();
        battleClass.target.ActStart();
        if(battleClass.battleDatas.Count > 0)
        {
            battleClass.caster.BattleStart();
            battleClass.target.BattleStart();
        }
        else
            RandCount ++;

        for(int i = 0; i < battleClass.battleDatas.Count; i++)
        {
            if(battleClass.battleDatas[i].InRe && battleClass.battleDatas[i].acc >= 0)
            {
                battleClass.casterData.CurHP -= battleClass.battleDatas[i].dmg;
                battleClass.target.Attack();
                battleClass.caster.Hit();
            }
            else if(!battleClass.battleDatas[i].InRe&& battleClass.battleDatas[i].acc >= 0)
            {
                battleClass.targetData.CurHP -= battleClass.battleDatas[i].dmg;
                battleClass.caster.Attack();
                battleClass.target.Hit();
            }
            RandCount ++;
        }
        
        if(battleClass.battleDatas.Count > 0)
        {
            battleClass.caster.BattleEnd();
            battleClass.target.BattleEnd();
        } 
        battleClass.caster.ActEnd();
        battleClass.target.ActEnd();
    }
    public Tile[] UnitRange(Character unit, out Tile[] atkT)
    {
        Tile[] t = MoveableRange(unit, unit.POS, unit.movement, false, out atkT);
        return t;
    }
    public void ShowEnemiesRange()
    {
        foreach(Character unit in Units)
        {
            if(System.Array.IndexOf(TurnChar.group.allyGroup, unit.group.groupName) > -1) continue;

            Tile[] t = UnitRange(unit, out Tile[] atkT);

            foreach (Tile _t in atkT)
            {
                _t.rangeSprite.color = IsEnemiesRange ? new Color(0, 0, 0, 0) : new Color(0.513f, 0.305f, 0.533f, 1f);
            }
        }
        IsEnemiesRange = !IsEnemiesRange;
    }
    public void ShowEnemyRange(Character unit)
    {
        _ = UnitRange(unit, out Tile[] atkT);

        Tile __t = tileDatas[unit.POS.y, unit.POS.x];
        if(__t.CheckRange.Contains(unit))
        {
            __t.CheckRange.Remove(unit);
            if(__t.CheckRange.Count == 0)
                __t.selectedUnitSprite.color = new Color(0, 0, 0, 0);    
        }
        else
        {
            __t.CheckRange.Add(unit);
            __t.selectedUnitSprite.color = new Color(0.913f, 0.352f, 0.352f, 1f); 
        }

        foreach(Tile _t in atkT)
        {
            if(_t.CheckRange.Contains(unit))
            {
                _t.CheckRange.Remove(unit);
                if(_t.CheckRange.Count == 0)
                    _t.selectedUnitSprite.color = new Color(0, 0, 0, 0);    
            }
            else
            {
                _t.CheckRange.Add(unit);
                _t.selectedUnitSprite.color = new Color(0.913f, 0.352f, 0.352f, 1f); 
            }
        }
    }
}