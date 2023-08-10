using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : TileObject
{
    public LineRenderer targettingLine;
    private Coroutine targettingArrowing = null;
    public string[] bonusClass;
    public string[] Inventory;
    public int movement = 5;
    public int SP;
    public string weapon;
    public Tile[] AttackRange = new Tile[0]; //The Range of Moveable with attack
    public Tile[] AttackableTiles = new Tile[0]; // AttackRange of Moved Character
    public Tile[] MoveableTiles = new Tile[0];
    public int weight;
    private bool isfocus;
    public bool IsFocus
    {
        get => isfocus;
        set
        {
            isfocus = value;
            FocusOn();
        }
    }
    public TileObject exTarget;
    public override void Awake()
    {
        base.Awake();
        GameManager.Inst.UnitMoveSpeed = 10.0f;
        group = new Group();
        POS = Vector2Int.zero;
        SP = Mathf.FloorToInt(oriData.SP);
        targettingLine = GetComponentInChildren<LineRenderer>();
    }
    public override void Init(string id)
    {
        base.Init(id);
        oriData.id = id;
        SetWeapon("Test_Sword");
        GetAttackSkill("Thrash");
        GetAttackSkill("NormalAttack_Range");
        GetAttackSkill("SlashHalfSpirit");
        if(id == "Kain")
        {
            group.groupName = "Player";
            tiles.TurnChar = this;
            movement = 7;
        }
        else
            group.groupName = id;
        
    }
    public virtual void Move(Vector3Int des)
    {
        tiles.FocusChar = this;
        Tile[] ts = tiles.PathHistory.ToArray();
        tiles.TurnAct = Act.Operate;
        if(des.x == POS.x && des.y == POS.y)
        {     
            GameManager.Inst.tiles.FocusChar = null;
            tiles.TurnAct = Act.Operate;
            SetPOS(POS);
            Debug.Log($"{id}를 배치함");
            OperatedAttackRange();
        }
        else
        {
            StartCoroutine(MoveUnit(ts));
        }
    }
    public virtual IEnumerator MoveUnit(Tile[] tilePos)
    {
        Debug.Log("움직이기 시작");
        if(tilePos == null || tilePos.Length == 0)
        yield break;

        if(GameManager.Inst.ControlType == controlType.WithMouse)
            GameManager.Inst.inputManager.MouseFocusSprite.enabled = false;
        else
            GameManager.Inst.inputManager.FocusSprite.enabled = false;

        tiles.TurnAct = Act.Acting;
        for(int i = 0; i < tilePos.Length; i++)
        {
            Tile t = tilePos[i];
            Vector3 des = tiles.TilePos(t.POS);
            des.z *= 0.51f;
            while (Mathf.Abs(transform.localPosition.x - des.x) > 0.1f || Mathf.Abs(transform.localPosition.y - des.y) > 0.1f)
            {
                transform.localPosition += transform.localPosition.x - des.x == 0 ? - 
                new Vector3(0, Mathf.Sign(transform.localPosition.y - des.y) * Time.deltaTime * GameManager.Inst.UnitMoveSpeed, 
                des.z * Time.deltaTime * GameManager.Inst.UnitMoveSpeed) : 

                new Vector3(Mathf.Sign(des.x - transform.localPosition.x) * Time.deltaTime * GameManager.Inst.UnitMoveSpeed, 0, 
                des.z * Time.deltaTime * GameManager.Inst.UnitMoveSpeed);
                yield return null;
            }
            transform.localPosition = new Vector3(des.x, des.y, des.z);
        }
        GameManager.Inst.tiles.FocusChar = null;
        tiles.TurnAct = Act.Operate;
        SetPOS((Vector2Int)tilePos[^1].POS);
        OperatedAttackRange();
    }
    public void OperatedAttackRange()
    {
        if(group.groupName == "Player")
        {
            Debug.Log("행동 선택");
            List<Tile> totalTile = new();
            totalTile.AddRange(MoveableTiles);
            totalTile.AddRange(AttackableTiles);
            totalTile.Distinct();

            foreach(Tile t in totalTile)
            {
                tiles.SetTileState(t, TileState.BlankTile);
            }
            tiles.uiCon.Actions.transform.Find("Attack").gameObject.SetActive(false);
            int min = int.MaxValue;
            int max = int.MinValue;
            tiles.uiCon.canUseAtkSkill.Clear();
            List<Tile> AttackableTileList = new();
            foreach (string attackSkill in oriData.attackSkills)
            {
                int[] r = new int[2];
                for(int i = 0 ; i < 2; i++)
                {
                    r[i] = System.Convert.ToInt32(((string[])DataManager.Datas[attackSkill]["range"])[i]);
                }
                if (r[0] < min)
                    min = r[0];
                if (r[0] > max)
                    max = r[1];

                int rB = System.Convert.ToInt32(DataManager.Datas[weapon]["rangeBonus"]);
                Tile[] _t = tiles.CalcAttackableTile(this, tempPOS, r[0], r[1] + rB);
                foreach(Tile __t in _t)
                {
                    AttackableTileList.Add(__t);
                    tiles.SetTileState(__t, TileState.RedTile);
                    if(__t.tileObject != null && JudgeEnemy(__t.tileObject))
                    {
                        tiles.uiCon.Actions.transform.Find("Attack").gameObject.SetActive(true);
                        tiles.uiCon.canUseAtkSkill.Add(attackSkill);
                    }
                }
            }
            AttackableTileList.ToArray();
            AttackRange = AttackableTileList.ToArray();
            tiles.uiCon.ActionsOn();
        }
    }
    public void ActOnTile(Tile t)
    {
        Debug.Log("액팅");
        if((Vector2Int)t.POS == POS)
        {
            Move(t.POS);
            return;    
        }
        if(t == null) return;
        if(t.tileObject != null)
        {
            if(System.Array.IndexOf(group.allyGroup, t.tileObject.group) == -1 && System.Array.IndexOf(AttackableTiles, t) > -1)
            {
                foreach(string a in oriData.attackSkills)
                {
                    if(DataManager.Datas[a]["dmgType"].ToString() != "h")
                    {
                        //
                    }
                }
                //Attack it
            }
            else
            {
                //Heal, if you have
            }
        }
        else if(System.Array.IndexOf(MoveableTiles, t) > -1)
        {
            Move(t.POS);
        }
    }
    public void SetTarget(Tile tile)
    {
        foreach(Tile _t in tiles.SelectedChecker)
            _t.obj.transform.Find("Checker").gameObject.SetActive(false);
        foreach(Tile _t in tiles.Checker)
            _t.obj.transform.Find("Checker").gameObject.SetActive(false);
        if(tiles.TurnAct == Act.SelectSkill)
        {
            tiles.Checker.Clear();
            foreach(Tile _t in AttackableTiles)
            {
                _t.tileObject?.SetHPBar(_t.tileObject.realData.CurHP);
            }
            tiles.uiCon.AttackSkillChange(string.IsNullOrEmpty(tiles.uiCon.setSkillId) ? "" : tiles.uiCon.setSkillId);
        }
        if(tile == null || tile.tileObject == null) 
        {
            tiles.uiCon.SkillPreview.SetActive(false);
            return;
        }
        
        bool judgeEnemy = JudgeEnemy(tile.tileObject);
        int SelectedCheckerCount = tiles.SelectedChecker.Count;

        foreach(Tile _t in tiles.SelectedChecker)
            _t.obj.transform.Find("Checker").gameObject.SetActive(true);
        if(!judgeEnemy && SelectedCheckerCount == 0)
        {
            tiles.uiCon.SkillPreview.SetActive(false);
        }
        if(judgeEnemy && SelectedCheckerCount == 0)
        {
            if(tiles.TurnAct == Act.SelectSkill)
            {
                if(System.Array.IndexOf(AttackableTiles, tile) > -1)
                {
                    CursorManager.CursorChange(CursorMode.Attack);
                    tiles.Checker.Add(tile);
                    foreach(Tile _t in tiles.SelectedChecker)
                        _t.obj.transform.Find("Checker").gameObject.SetActive(false);
                    foreach(Tile _t in tiles.Checker)
                        _t.obj.transform.Find("Checker").gameObject.SetActive(true);

                    GameManager.Inst.uiCon.SkillPreview.SetActive(true);
                    GameManager.Inst.uiCon.Preview.CasterPreview.data = this;
                    GameManager.Inst.uiCon.Preview.TargetPreview.data = (Character)tile.tileObject;
                    tiles.uiCon.SetCasterPreviewUnit();
                    tiles.uiCon.SetTargetPreviewUnit();
                }
                else
                {
                    GameManager.Inst.uiCon.SkillPreview.SetActive(false);
                    CursorManager.CursorChange(CursorMode.DisabledAttack);
                }
            }
        }
    }
    public void WholeTurnStart()
    {
        MoveableTiles = tiles.MoveableRange(this, POS, movement, false, out AttackableTiles);
        if(group.groupName != "Player") FindTarget();
        if(targettingArrowing != null)
        {
            targettingLine.gameObject.SetActive(false);
            StopCoroutine(targettingArrowing);
        }
        targettingArrowing = StartCoroutine(TargettingArrow());
    }
    public void FindTarget()
    {
        List<TileObject> cand = new();
        foreach(Tile t in AttackableTiles)
        {
            if(t.tempTileObject != null && System.Array.IndexOf(group.allyGroup, t.tempTileObject.group.groupName) == -1)
            {
                cand.Add((Character)t.tempTileObject);
            }
        }
        int prio = int.MinValue;
        
        int MaxDmg = int.MinValue;
        TileObject MostDmgUnit = null;
        TileObject TargetUnit = null;

        foreach(TileObject obj in cand)
        {
            MostDmgUnit ??= obj;
            TargetUnit ??= obj;
            int p = 0;


            if (obj is Character c)
            {
                BattleClass bC = new(this, c, realData.attackSkills[0]);

                if (bC.totalDmg >= c.realData.CurHP) p += 5;
                if (bC.totalDmg == 0) p -= 4;

                int postCountDmg = 0;
                foreach (BattleData bD in bC.battleDatas)
                {
                    if (bD.InRe)
                        postCountDmg += bD.dmg;
                    else
                        break;
                }
                if (postCountDmg < c.realData.CurHP && bC.totalCountDmg >= realData.CurHP) p -= 2;
                if (!bC.countable) p++;
                if (bC.avgAcc < 50) p--;
                if (bC.avgAcc < 30) p--;
                if (bC.avgAcc < 10) p--;
                if (bC.avgAcc == 0) p -= 4;

                if (p == prio)
                {
                    MaxDmg = bC.totalDmg > MaxDmg ? bC.totalDmg : MaxDmg;
                    MostDmgUnit = bC.totalDmg > MaxDmg ? c : MostDmgUnit;
                    prio = bC.totalDmg > MaxDmg ? p : prio;
                }
                else if (p > prio)
                {
                    prio = p;
                    TargetUnit = obj;
                }
            }
        }
        exTarget = TargetUnit;
    }
    public virtual void TurnCancle()
    {
        tiles.tileDatas[tempPOS.y, tempPOS.x].tempTileObject = null;
        tiles.tileDatas[tempPOS.y, tempPOS.x].tileObject = null;
        transform.localPosition = tiles.TilePos(POS);
        tempPOS = POS;
        tiles.TurnAct = Act.Ready;
        AttackRange = new Tile[0];
        if(group.groupName == "Player")
        {
            if(GameManager.Inst.ControlType == controlType.WithMouse)
                GameManager.Inst.inputManager.MouseFocusSprite.enabled = true;
            else
                GameManager.Inst.inputManager.FocusSprite.enabled = true;
            
            foreach(Tile _t in AttackableTiles)
                tiles.SetTileState(_t, TileState.RedTile);
            foreach(Tile _t in MoveableTiles)
                tiles.SetTileState(_t, TileState.BlueTile);

            // tiles.MoveableRange(POS, movement, !string.IsNullOrEmpty(ori_data.HalfSpirit) ? (bool)DataManager.Datas[ori_data.HalfSpirit]["fly"] : false, out tiles.AttackableResult);
        }
    }
    public override void TurnStart()
    {
        base.TurnStart();
        GameManager.Inst.uiCon.Preview.CasterPreview.data = this;
        if(group.groupName == "Player")
        {
            foreach(Tile _t in AttackableTiles)
                tiles.SetTileState(_t, TileState.RedTile);
            foreach(Tile _t in MoveableTiles)
                tiles.SetTileState(_t, TileState.BlueTile);
                
            // Tile[] t = tiles.MoveableRange(POS, movement, !string.IsNullOrEmpty(ori_data.HalfSpirit) ? (bool)DataManager.Datas[ori_data.HalfSpirit]["fly"] : false, out tiles.AttackableResult);
        }
    }
    public void ActStay()
    {
        Stay();
        TurnEnd();
    }
    public override void TurnEnd()
    {
        base.TurnEnd();
    }
    public void SetWeapon(string id)
    {
        weapon = id;
        if (oriData.attackSkills.Contains("NormalAttack_Melee"))
            oriData.attackSkills.Remove("NormalAttack_Melee");
        if (oriData.attackSkills.Contains("NormalAttack_Spear"))
            oriData.attackSkills.Remove("NormalAttack_Spear");
        if (oriData.attackSkills.Contains("NormalAttack_Range"))
            oriData.attackSkills.Remove("NormalAttack_Range");

        if (DataManager.Datas[weapon]["weaponType"].ToString() == "sword"|| DataManager.Datas[weapon]["weaponType"].ToString() == "blunt")
            GetAttackSkill("NormalAttack_Melee");
        else if (DataManager.Datas[weapon]["weaponType"].ToString() == "spear")
            GetAttackSkill("NormalAttack_Spear");
        else if (DataManager.Datas[weapon]["weaponType"].ToString() == "bow")
            GetAttackSkill("NormalAttack_Range");
    }
    public void GetAttackSkill(string id)
    {
        if (!oriData.attackSkills.Contains(id))
            oriData.attackSkills.Add(id);
    }
    public void GetSkills(string id)
    {
        oriData.Skills.Add(id);
    }
    public bool JudgeEnemy(TileObject tileObject)
    {
        if(tileObject == null)
            return false;
        if(System.Array.IndexOf(group.allyGroup, tileObject.group) == -1)
            return true;
        return false;
    }
    public virtual void SelectSkill(string id)
    {
        if(group.groupName == "Player")
        {  
            if(string.IsNullOrEmpty(id))
            {
                if(GameManager.Inst.uiCon.selectSkillBackupTiles == null && GameManager.Inst.uiCon.backupTiles != null)
                    foreach (Tile _t in GameManager.Inst.uiCon.backupTiles)
                        GameManager.Inst.tiles.SetTileState(_t, TileState.RedTile);
                else
                {
                    if(GameManager.Inst.uiCon.backupTiles != null)
                        foreach(Tile _t in GameManager.Inst.uiCon.backupTiles)
                            GameManager.Inst.tiles.SetTileState(_t, TileState.BlankTile);
                    if(GameManager.Inst.uiCon.selectSkillBackupTiles != null)
                        foreach (Tile _t in GameManager.Inst.uiCon.selectSkillBackupTiles)
                            GameManager.Inst.tiles.SetTileState(_t, TileState.RedTile);
                }
            }
            else
            {    
                foreach(Tile _t in GameManager.Inst.tiles.SelectedChecker)
                {
                    GameManager.Inst.tiles.tileObject[_t.POS.y, _t.POS.x].transform.Find("Checker").gameObject.SetActive(false);
                }
                foreach(Tile _t in GameManager.Inst.tiles.TurnChar.AttackRange)
                    GameManager.Inst.tiles.SetTileState(_t, TileState.BlankTile);

                Preview preview = GameManager.Inst.uiCon.Preview;
                preview.attackSkill = id;
                string[] NameDesc = DataManager.DescConvert(id);
                preview.SP.text = $"SP : {GameManager.Inst.tiles.TurnChar.SP} (-" + DataManager.Datas[id]["cost"] + ")";
                preview.Name.text = NameDesc[0];
                preview.Cost.text = $"SP : " + DataManager.Datas[id]["cost"];
                preview.Power.text = 
                    DataManager.Datas[id].ContainsKey("dmg") || System.Convert.ToInt32(DataManager.Datas[id]["dmg"]) == 0? 
                    DataManager.Datas[id].ContainsKey("attackCount")? 
                    DataManager.Datas[id]["dmg"].ToString() + " × " + DataManager.Datas[id]["attackCount"].ToString() :
                    DataManager.Datas[id]["dmg"].ToString() : "-";
                preview.DmgType.color = System.Convert.ToChar(DataManager.Datas[id]["dmgType"]) == 'p' ? Color.cyan : Color.black;
                preview.Desc.text = NameDesc[1];

                GameManager.Inst.uiCon.SetCasterPreviewUnit();
                GameManager.Inst.uiCon.SetTargetPreviewUnit();

                GameManager.Inst.tiles.SelectedChecker.Clear();
                int[] r = new int[2];
                for(int i = 0; i < ((string[])DataManager.Datas[id]["range"]).Length; i++)
                    r[i] = System.Convert.ToInt32(((string[])DataManager.Datas[id]["range"])[i]);
                int rB = System.Convert.ToInt32(DataManager.Datas[GameManager.Inst.tiles.TurnChar.weapon]["rangeBonus"]);
                Tile[] t = GameManager.Inst.tiles.CalcAttackableTile(this, GameManager.Inst.tiles.TurnChar.tempPOS, r[0], r[1] + rB);

                GameManager.Inst.tiles.TurnChar.AttackableTiles = t;

                foreach(Tile _t in t)
                    GameManager.Inst.tiles.SetTileState(_t, TileState.RedTile);
                CursorManager.CursorChange(CursorMode.Attack);
                GameManager.Inst.uiCon.AttackSkillChange(id);
                GameManager.Inst.tiles.TurnAct = Act.SelectSkill;
            }
        }
    }
    public void FocusOn()
    {
        if(tiles.TurnChar == this) return;

        if(IsFocus)
        {
            tiles.WhoFocused = this;
            if(tiles.TurnAct != Act.Operate && tiles.TurnAct != Act.Ready)
                return;
            tiles.uiCon.SetExpectTarget(this, realData.attackSkills[0], exTarget);
            if(tiles.TurnAct == Act.Ready)
            {
                List<Tile> total = new();
                total.AddRange(tiles.TurnChar.AttackableTiles);
                total.AddRange(tiles.TurnChar.MoveableTiles);
                total.Distinct();

                foreach(Tile t in total)
                {
                    tiles.SetTileState(t, TileState.BlankTile);
                }
                foreach(Tile t in AttackableTiles)
                {
                    tiles.SetTileState(t, TileState.RedTile);
                }
                foreach(Tile t in MoveableTiles)
                {
                    tiles.SetTileState(t, TileState.BlueTile);
                }
            }
        }
        else
        {
            tiles.WhoFocused = null;
            tiles.uiCon.SetExpectTarget(this, realData.attackSkills[0], null);
            if(tiles.TurnAct != Act.Ready && tiles.TurnAct != Act.Operate)
                return;
            List<Tile> total = new();
            total.AddRange(AttackableTiles);
            total.AddRange(MoveableTiles);
            total.Distinct();
            foreach(Tile t in total)
            {
                tiles.SetTileState(t, TileState.BlankTile);
            }

            if(tiles.TurnChar.AttackRange.Length > 0)
            {
                foreach(Tile t in tiles.TurnChar.AttackRange)
                {
                    tiles.SetTileState(t, TileState.RedTile);
                }
            }
            else
            {
                foreach(Tile t in tiles.TurnChar.AttackableTiles)
                {
                    tiles.SetTileState(t, TileState.RedTile);
                }
                foreach(Tile t in tiles.TurnChar.MoveableTiles)
                {
                    tiles.SetTileState(t, TileState.BlueTile);
                }
            }
        }
    }
    public IEnumerator TargettingArrow()
    {
        if(exTarget == null || this == tiles.TurnChar)
            yield break;
        while(true)
        {
            if(tiles.WhoFocused == null || tiles.WhoFocused == this)
            {
                targettingLine.gameObject.SetActive(true);
                Vector3 startPos = transform.position;
                Vector3 p = exTarget.transform.position - transform.position;

                float inclinVal = 0.01f;
                int centerIndex = Mathf.RoundToInt((targettingLine.positionCount - 1) * 0.5f);
                int c = targettingLine.positionCount;

                for(int i = 0; i < targettingLine.positionCount; i++)
                {
                    float r = (float)i / (c - 1);
                    targettingLine.SetPosition(i, 
                    new Vector3(startPos.x + p.x * r, 
                                Mathf.Pow(centerIndex, 2) * inclinVal +  startPos.y + p.y * r - 
                                (i > targettingLine.positionCount * 0.5f ? 
                                Mathf.Pow((c - 1 - i) - centerIndex, 2) : Mathf.Pow(centerIndex - i, 2))* inclinVal, 
                                startPos.z + p.z * r));
                }
            }
            else targettingLine.gameObject.SetActive(false);
            yield return null;
        }
    }
    public Tile FindVantageTile()
    {
        return null;
    }
    public Tile FindVTileToAttack(Tile tile, int maxRange, int minRange)
    {
        //int prio = int.MinValue;
        Vector3Int tPos = tile.POS;
        for(int i = Mathf.Max(0, tPos.x - maxRange); i <= Mathf.Min(tPos.x + maxRange, tiles.size.x); i++)
        {
            for(int j = Mathf.Max(0, tPos.y - maxRange); j <= Mathf.Min(tPos.y + maxRange, tiles.size.y); j++)
            {
                int distance = Mathf.Abs(i - tPos.x) + Mathf.Abs(j - tPos.y);
                if(distance >= minRange && distance <= maxRange)
                {
                    Tile t = tiles.tileDatas[j, i];
                    Tile[] route = tiles.PathFind(this, POS, (Vector2Int)t.POS, false);
                    if(route.Length <= movement + maxRange)
                    {
                        return t;
                        // if(distance == maxRange)
                        // {
                        //     return t;
                        // }
                    }
                }
            }    
        }
        return null;
    }
}