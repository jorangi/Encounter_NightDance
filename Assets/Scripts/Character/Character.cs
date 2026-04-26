using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

public enum DecisionType
{
    Attacker,
    Supporter,
    Lethargy
}
public enum MoveType
{
    Moveable,
    Taped
}
public class Character : TileObject
{
    #region field
    SpriteRenderer[] HPBars = new SpriteRenderer[3];
    private readonly WaitForSeconds ActStayTime = new(1f);
    public LineRenderer targettingLine;
    private Coroutine targettingArrowing = null;
    public string[] bonusClass;
    public int movement = 5;
    public int SP;
    public string weapon;
    public string accessory;
    public Tile[] AttackRange = new Tile[0]; // AttackRange after Moved Character
    public Tile[] AttackableTiles = new Tile[0]; //The Range of Moveable with attack or after selected skill
    public Tile[] MoveableTiles = new Tile[0];
    public int weight;
    private readonly Vector3 JumpPower = new(0, 0.04f, 0);
    public TileObject exTarget;
    public TileObject mostTarget;
    public Tile mostTile;
    public TileObject highTarget;
    public DecisionType decisionType = DecisionType.Attacker;
    public MoveType moveType = MoveType.Moveable;
    #endregion
    public override void Awake()
    {
        for(int i = 0; i < 3; i++) HPBars[i] = transform.Find("HealthBar").GetChild(i).GetComponent<SpriteRenderer>();
        base.Awake();
        group = new Group();
        POS = Vector2Int.zero;
        SP = Mathf.FloorToInt(oriData.SP);
        targettingLine = GetComponentInChildren<LineRenderer>();
    }
    public override void Init(string id, Vector2Int pos)
    {
        base.Init(id, pos);
        oriData.id = id;
        SetWeapon("Test_Sword");
        GetAttackSkill("Thrash");
        GetAttackSkill("NORMALATTACK_RANGE");
        GetAttackSkill("SlashHalfSpirit");
        GetItem("TestAccessory");
        if(id == "Kain")
        {
            group.groupName = "Player";
            tiles.TurnChar = this;
            movement = 7;
        }
        else
            group.groupName = id;

        realData.Dpn = 100;
        movement = 5;
        
    }
    public void Move(Vector3Int des)
    {
        if(group.groupName != "Player")
        {
            tiles.PathHistory.Clear();
        }
        tiles.FocusChar = this;
        Tile[] ts = tiles.PathHistory.ToArray();
        tiles.TurnAct = Act.Operate;
        if(des.x == POS.x && des.y == POS.y)
        {     
            tiles.TurnAct = Act.Operate;
            SetTempPOS(POS);
            if(group.groupName == "Player")
            {
                GameManager.Inst.tiles.FocusChar = null;
                OperatedAttackRange();
            }
        }
        else
        {
            if(ts == null || ts.Length == 0)
            {
                ts = GameManager.Inst.tiles.PathFind(this, POS, (Vector2Int)des, false);
            }
            StartCoroutine(MoveUnit(ts));
        }
    }
    public virtual IEnumerator MoveUnit(Tile[] tilePos)
    {
        if(GameManager.Inst.ControlType == ControlType.WithMouse)
            GameManager.Inst.inputManager.MouseFocusSprite.enabled = false;
        else
            GameManager.Inst.inputManager.FocusSprite.enabled = false;

        tiles.TurnAct = Act.Acting;
        Transform unitSprite = spriteRenderer.transform;
        foreach(SpriteRenderer hpbar in HPBars) hpbar.enabled = false;

        if(tilePos.Length == 0)
        {
            foreach(SpriteRenderer hpbar in HPBars) hpbar.enabled = true;
            tiles.TurnAct = Act.Operate;
            if(group.groupName == "Player")
            {
                GameManager.Inst.tiles.FocusChar = null;
                OperatedAttackRange();
            }
            yield break;
        }
        for(int i = 0; i < tilePos.Length; i++)
        {
            Tile t = tilePos[i];
            Vector3 des = tiles.TilePos(t.POS);
            des.z *= 0.51f;
            bool XorY = Mathf.Abs(transform.localPosition.x - des.x) > 0.05f;
            while (Mathf.Abs(transform.localPosition.x - des.x) > 0.05f || Mathf.Abs(transform.localPosition.y - des.y) > 0.05f)
            {
                if (XorY ? Mathf.Abs(transform.localPosition.x - des.x) > 0.5f : Mathf.Abs(transform.localPosition.y - des.y) > 0.5f)
                {
                    // ShadowSprite.localScale -= sizeMo;
                    unitSprite.localPosition += JumpPower;
                }
                else
                {
                    // ShadowSprite.localScale += sizeMo;
                    unitSprite.localPosition -= JumpPower;
                }
   
                switch(Mathf.RoundToInt(tiles.transform.eulerAngles.z)/90)
                {
                    case 0:
                        spriteRenderer.flipX = !XorY ? spriteRenderer.flipX : transform.localPosition.x - des.x > 0;
                        break;
                    case 1:
                        spriteRenderer.flipX = XorY ? spriteRenderer.flipX : transform.localPosition.y - des.y < 0;
                        break;
                    case 2:
                        spriteRenderer.flipX = !XorY ? spriteRenderer.flipX : transform.localPosition.x - des.x < 0;
                        break;
                    case 3:
                        spriteRenderer.flipX = XorY ? spriteRenderer.flipX : transform.localPosition.y - des.y > 0;
                        break;
                }

                transform.localPosition += transform.localPosition.x - des.x == 0 ? - 
                new Vector3(0, Mathf.Sign(transform.localPosition.y - des.y) * Time.fixedDeltaTime * GameManager.Inst.UnitMoveSpeed, 
                des.z * Time.fixedDeltaTime * GameManager.Inst.UnitMoveSpeed) : 
                new Vector3(Mathf.Sign(des.x - transform.localPosition.x) * Time.fixedDeltaTime * GameManager.Inst.UnitMoveSpeed, 0, 
                des.z * Time.fixedDeltaTime * GameManager.Inst.UnitMoveSpeed);

                yield return null;
            }
            transform.localPosition = new Vector3(des.x, des.y, des.z);
            unitSprite.localPosition = new Vector3(0, 0, 0);
        }
        foreach(SpriteRenderer hpbar in HPBars) hpbar.enabled = true;
        tiles.TurnAct = Act.Operate;
        SetTempPOS((Vector2Int)tilePos[^1].POS);
        if(group.groupName == "Player")
        {
            GameManager.Inst.tiles.FocusChar = null;
            OperatedAttackRange();
        }
    }
    public void OperatedAttackRange()
    {
        if(group.groupName != "Player")
            return;
    
        foreach(Tile t in MoveableTiles)
        {
            tiles.SetTileState(t, TileState.BlankTile);
        }
        foreach(Tile t in AttackableTiles)
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
                r[i] = Convert.ToInt32(((string[])DataManager.Datas[attackSkill]["range"])[i]);
            }
            if (r[0] < min)
                min = r[0];
            if (r[0] > max)
                max = r[1];
            int rB = Convert.ToInt32(DataManager.Datas[weapon]["rangeBonus"]);
            Tile[] _t = tiles.CalcAttackableTile(this, TempPOS, r[0], r[1] + rB);
            foreach(Tile __t in _t)
            {
                if(__t.tileObject != null && __t.tileObject is not Character && 
                __t.tileObject.objectData.Special.ContainsKey("attackable") && __t.tileObject.objectData.Special["attackable"] == "false") continue;
                
                AttackableTileList.Add(__t);
                tiles.SetTileState(__t, TileState.RedTile);
                if(__t.tileObject != null && 
                (__t.tileObject is Character || __t.tileObject.objectData.Special.ContainsKey("attackable") && __t.tileObject.objectData.Special["attackable"] == "true")
                && JudgeEnemy(__t.tileObject))
                {
                    tiles.uiCon.Actions.transform.Find("Attack").gameObject.SetActive(true);
                    tiles.uiCon.canUseAtkSkill.Add(attackSkill);
                }
            }
        }
        AttackableTileList.ToArray();
        AttackRange = AttackableTileList.Distinct().ToArray();
        tiles.uiCon.Visibled = VisibledActionUI.Action;
        tiles.uiCon.ActionsOn();
    }
    public void ActOnTile(Tile t)
    {
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
            tiles.uiCon.AttackSkillChange(string.IsNullOrEmpty(tiles.uiCon.SetSkillId) ? "" : tiles.uiCon.SetSkillId);
        }
        if(tile == null || tile.tileObject == null) 
        {
            GameManager.Inst.uiCon.Preview.TargetPreview.Data = null;
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
                if(Array.IndexOf(AttackableTiles, tile) > -1)
                {
                    CursorManager.CursorChange(CursorMode.Attack);
                    tiles.Checker.Add(tile);
                    foreach(Tile _t in tiles.SelectedChecker)
                        _t.obj.transform.Find("Checker").gameObject.SetActive(false);
                    foreach(Tile _t in tiles.Checker)
                        _t.obj.transform.Find("Checker").gameObject.SetActive(true);

                    GameManager.Inst.uiCon.SkillPreview.SetActive(true);
                    GameManager.Inst.uiCon.Preview.CasterPreview.Data = this;
                    GameManager.Inst.uiCon.Preview.TargetPreview.Data = (Character)tile.tileObject;
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
        GameManager.Inst.uiCon.SetSkillId = "";
        tiles.AllUnitRecalcRange();
        TurnStart();
    }
    public void ShowTargettingArrow()
    {
        if(targettingArrowing != null)
        {
            targettingLine.gameObject.SetActive(false);
            StopCoroutine(targettingArrowing);
        }
        targettingArrowing = StartCoroutine(TargettingArrow());
    }
    public void FindTarget()
    {
        if(group.groupName == "Player")
            return;
        List<TileObject> cand = new();
        foreach(Tile t in AttackableTiles)
        {
            if(t.TempTileObject is Character character && t.TempTileObject != null && this != t.TempTileObject && Array.IndexOf(group.allyGroup, t.TempTileObject.group.groupName) == -1)
            {
                if(BestPosition(t.TempTileObject, out string _) == null)
                    continue;
                cand.Add(character);
            }
        }
        TileObject TargetUnit = TargetPriority(cand);
        exTarget = TargetUnit;
    }
    public TileObject FindNearTarget()
    {
        if(group.groupName == "Player")
            return null;
        int distance = int.MaxValue;
        TileObject target = null;
        foreach(Character unit in tiles.Units)
        {
            if(unit != this && Array.IndexOf(group.allyGroup, unit.group.groupName) == -1)
            {
                int tempDis = Mathf.Abs(TempPOS.x - unit.TempPOS.x) + Mathf.Abs(TempPOS.y - unit.TempPOS.y);
                if(distance > tempDis)
                {
                    distance = tempDis;
                    target = unit;
                }
            }
        }
        return target;
    }
    public virtual void TurnCancle()
    {
        foreach(Tile _t in AttackRange)
            tiles.SetTileState(_t, TileState.BlankTile);
        transform.localPosition = tiles.TilePos(POS);
        TempPOS = POS;
        MoveableTiles = tiles.MoveableRange(this, POS, movement, false, out AttackableTiles);
        tiles.tileDatas[POS.y, POS.x].TempTileObject = this;
        tiles.TurnAct = Act.Ready;
        AttackRange = new Tile[0];
        
        foreach(Character unit in GameManager.Inst.tiles.Units) unit.FindTarget();

        if(group.groupName == "Player")
        {
            if(GameManager.Inst.ControlType == ControlType.WithMouse)
                GameManager.Inst.inputManager.MouseFocusSprite.enabled = true;
            else
                GameManager.Inst.inputManager.FocusSprite.enabled = true;
            
            foreach(Tile _t in AttackableTiles)
                tiles.SetTileState(_t, TileState.RedTile);
            foreach(Tile _t in MoveableTiles)
                tiles.SetTileState(_t, TileState.BlueTile);
        }
    }
    public override void TurnStart()
    {
        base.TurnStart();
        GameManager.Inst.uiCon.Preview.CasterPreview.Data = this;
    }
    public void ActStay()
    {
        tiles.uiCon.Visibled = VisibledActionUI.None;
        tiles.uiCon.ActionsOn();
        Stay();
        tiles.ActEnd();
    }
    public override void ActEnd()
    {
        base.ActEnd();
    }
    public void SetWeapon(string id)
    {
        weapon = id.ToUpper();
        if (oriData.attackSkills.Contains("NORMALATTACK_MELEE"))
            oriData.attackSkills.Remove("NORMALATTACK_MELEE");
        if (oriData.attackSkills.Contains("NORMALATTACK_SPEAR"))
            oriData.attackSkills.Remove("NORMALATTACK_SPEAR");
        if (oriData.attackSkills.Contains("NORMALATTACK_RANGE"))
            oriData.attackSkills.Remove("NORMALATTACK_RANGE");

        if (DataManager.Datas[weapon]["weaponType"].ToString().ToUpper() == "SWORD"|| DataManager.Datas[weapon]["weaponType"].ToString().ToUpper() == "BLUNT")
            GetAttackSkill("NORMALATTACK_MELEE");
        else if (DataManager.Datas[weapon]["weaponType"].ToString().ToUpper() == "SPEAR")
            GetAttackSkill("NORMALATTACK_SPEAR");
        else if (DataManager.Datas[weapon]["weaponType"].ToString().ToUpper() == "BOW")
            GetAttackSkill("NORMALATTACK_RANGE");
    }
    public void GetItem(string id)
    {
        id = id.ToUpper();
        bool multipleItem = true;

        if(multipleItem)
        {
            int index = realData.inventory.IndexOf(id);
            if(index > -1)
            {
                realData.inventory_ea[index] ++;
            }
            else
            {
                realData.inventory.Add(id);
                realData.inventory_ea.Add(1);
            }
        }
        else
        {
            realData.inventory.Add(id);
            realData.inventory_ea.Add(1);
        }
    }
    public void GetItem(string id, int slotNum)
    {

    }
    public void GetAttackSkill(string id)
    {
        if (!realData.attackSkills.Contains(id.ToUpper()))
            realData.attackSkills.Add(id.ToUpper());
    }
    public void GetSkills(string id)
    {
        realData.Skills.Add(id.ToUpper());
    }
    public bool JudgeEnemy(TileObject target)
    {
        if(target == null)
            return false;
        if(target is Character && Array.IndexOf(group.allyGroup, target.group.groupName) == -1)
            return true;
        else
        {
            if(target.objectData.Special.ContainsKey("Type"))
            {
                string[] types = target.objectData.Special["Type"].Split(',');
                foreach(string type in types)
                {
                    if(type == "plant" && Array.IndexOf((object[])DataManager.Datas[weapon]["tag"], "fire") > -1) return true;
                    else if(type == "tree"&& Array.IndexOf((object[])DataManager.Datas[weapon]["tag"], "fire") > -1 || Array.IndexOf((object[])DataManager.Datas[weapon]["tag"], "axe") > -1) return true;
                }
            }
        }
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
                preview.DmgType.color = Convert.ToChar(DataManager.Datas[id]["dmgType"]) == 'p' ? Color.cyan : Color.black;
                preview.Desc.text = NameDesc[1];

                GameManager.Inst.uiCon.SetCasterPreviewUnit();
                GameManager.Inst.uiCon.SetTargetPreviewUnit();

                GameManager.Inst.tiles.SelectedChecker.Clear();
                int[] r = new int[2];
                for(int i = 0; i < ((string[])DataManager.Datas[id]["range"]).Length; i++)
                    r[i] = Convert.ToInt32(((string[])DataManager.Datas[id]["range"])[i]);
                int rB = Convert.ToInt32(DataManager.Datas[GameManager.Inst.tiles.TurnChar.weapon]["rangeBonus"]);
                Tile[] t = GameManager.Inst.tiles.CalcAttackableTile(this, GameManager.Inst.tiles.TurnChar.TempPOS, id);
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
        tiles.WhoFocused = this;
        if(tiles.TurnAct != Act.Operate && tiles.TurnAct != Act.Ready)
            return;
        tiles.uiCon.SetExpectTarget(this, realData.attackSkills[0], exTarget);
        if(tiles.TurnAct == Act.Ready)
        {
            foreach(Tile t in tiles.TurnChar.AttackableTiles)
            {
                tiles.SetTileState(t, TileState.BlankTile);
            }
            foreach(Tile t in tiles.TurnChar.MoveableTiles)
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
    public void FocusOff()
    {
        tiles.WhoFocused = null;
        tiles.uiCon.SetExpectTarget(this, realData.attackSkills[0], null);
        if(tiles.TurnAct != Act.Ready && tiles.TurnAct != Act.Operate)
            return;

        foreach(Tile t in AttackableTiles)
        {
            tiles.SetTileState(t, TileState.BlankTile);
        }
        foreach(Tile t in MoveableTiles)
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
    public IEnumerator TargettingArrow()
    {
        if(this == tiles.TurnChar)
            yield break;
        while(true)
        {
            if(exTarget != null)
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
            }
            yield return null;
        }
    }
    public Tile FindVTileToAttack(Tile tile, int maxRange, int minRange)
    {
        //최적 공격 타일 탐색(반격을 안받는다 등의 여러 요인에 영향을 받아 최적의 공격 장소를 탐색하는 메소드)

        //int prio = int.MinValue;
        Vector3Int tPos = tile.POS;
        tiles.MoveableRange(this, (Vector2Int)tPos, 0, false, out Tile[] atkTiles);

        foreach(Tile t in atkTiles)
        {
            if(Array.IndexOf(MoveableTiles, t) > -1)
            {
                return t;
            }
        }

        for(int i = Mathf.Max(0, tPos.x - maxRange); i <= Mathf.Min(tPos.x + maxRange, tiles.size.x); i++)
        {
            for(int j = Mathf.Max(0, tPos.y - maxRange); j <= Mathf.Min(tPos.y + maxRange, tiles.size.y); j++)
            {
                int distance = Mathf.Abs(i - tPos.x) + Mathf.Abs(j - tPos.y);
                if(distance >= minRange && distance <= maxRange)
                {
                    Tile t = tiles.tileDatas[j, i];
                    if(t.blocked || t.tileObject != null)
                    {
                        continue;
                    }
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
    public IEnumerator DecisionAct()
    {
        if(mostTile != null)
        {
            //해당 타일로 이동을 우선시 함
        }
        if(decisionType == DecisionType.Attacker)
        {
            if(mostTarget != null && Array.IndexOf(group.allyGroup, mostTarget.group.groupName) == -1)
            {
                Tile mostTargetsTile = tiles.tileDatas[mostTarget.POS.y, mostTarget.POS.x];
                Tile bestPos = BestPosition(mostTarget, out string skillId);
                Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);

                while(tiles.TurnAct == Act.Acting)
                {
                    yield return null;
                }
                yield return ActStayTime;
                if(Array.IndexOf(AttackableTiles, mostTargetsTile) > -1)
                {       
                    BattleClass battleClass = new(this, (Character)mostTarget, skillId);
                    tiles.BattleRun(battleClass);
                }
                yield break;
            }
            else if(highTarget != null && Array.IndexOf(group.allyGroup, highTarget.group.groupName) == -1)
            {
                Tile highTile = tiles.tileDatas[highTarget.POS.y, highTarget.POS.x];
                if(System.Array.IndexOf(AttackableTiles, highTile) > -1)
                {
                    Tile bestPos = BestPosition(highTarget, out string skillId);
                    Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                    
                    Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                    
                    while(tiles.TurnAct == Act.Acting)
                    {
                        yield return null;
                    }
                    yield return ActStayTime;
                    BattleClass battleClass = new(this, (Character)highTarget, skillId);
                    tiles.BattleRun(battleClass);
                }
                else
                {
                    Tile bestPos = BestPosition(highTarget, out string highSkillId);
                    Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                    int distance = path.Length;
                    int _dis = int.MinValue;
                    TileObject lowTarget = null;
                    foreach(Tile t in AttackableTiles)
                    {
                        if(t.tileObject != null && System.Array.IndexOf(group.allyGroup, t.tileObject.group) == -1)
                        {
                            Tile highPos = BestPosition(t.tileObject, out string skillId);
                            Tile[] hPath = tiles.PathFind(this, POS, (Vector2Int)highPos.POS, false);
                            int __dis = Mathf.Abs(hPath[^1].POS.x - path[^1].POS.x) + Mathf.Abs(hPath[^1].POS.y - path[^1].POS.y);
                            if(__dis >= distance && _dis > __dis)
                            {
                                _dis = __dis;
                                highSkillId = skillId;
                                path = hPath;
                                lowTarget = t.tileObject;
                            }
                        }
                    }
                    Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                    
                    while(tiles.TurnAct == Act.Acting)
                    {
                        yield return null;
                    }
                    yield return ActStayTime;
                    if(highTarget != null || lowTarget != null)
                    {
                        BattleClass battleClass = new(this, highTarget != null ? (Character)highTarget : (Character)lowTarget, highSkillId);
                        tiles.BattleRun(battleClass);
                    }
                }
            }
            else
            {
                FindTarget();
                TileObject target = exTarget;
                if(target == null)
                {
                    target = FindNearTarget();
                    Tile[] path = tiles.PathFindWithoutObject(this, TempPOS, target.TempPOS, false);
                    Move(path != null && moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                    while(tiles.TurnAct == Act.Acting)
                    {
                        yield return null;
                    }
                    tiles.ActStay();
                }
                else
                {
                    Tile bestPos =  BestPosition(target, out string skill);
                    BattleClass battleClass = new(this, (Character)target, skill);
                    Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                    Move(path != null && moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                    while(tiles.TurnAct == Act.Acting)
                    {
                        yield return null;
                    }
                    tiles.BattleRun(battleClass);
                }
            }
        }
        else if(decisionType == DecisionType.Supporter)
        {
            if(mostTarget != null && Array.IndexOf(group.allyGroup, mostTarget.group.groupName) > -1)
            {
                BattleClass battleClass = null;
                Tile bestPos = BestPosition(mostTarget, out string skillId);
                Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                if(Array.IndexOf(group.allyGroup, mostTarget.group.groupName) > -1)
                {
                    if(mostTarget.realData.CurHP <= mostTarget.realData.HP * 0.75)
                        battleClass = new(this, (Character)mostTarget, skillId);
                }
                Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                while(tiles.TurnAct == Act.Acting)
                {
                    yield return null;
                }
                yield return ActStayTime;
                tiles.BattleRun(battleClass);
                
                yield break;
            }
            else if(highTarget != null && System.Array.IndexOf(group.allyGroup, highTarget.group.groupName) > -1)
            {
                BattleClass battleClass = null;
                Tile highTile = tiles.tileDatas[highTarget.POS.y, highTarget.POS.x];
                Tile[] path = null;

                if(System.Array.IndexOf(AttackableTiles, highTile) > -1)
                {
                    Tile bestPos = null;
                    string skillId = "";
                    if(highTarget.realData.CurHP <= highTarget.realData.HP * 0.4f)
                    {
                        bestPos = BestPosition(highTarget, out skillId);
                        path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                        battleClass = new(this, (Character)highTarget, skillId);
                        Move(bestPos.POS);
                    }
                    else
                    {
                        List<Character> allies = new();
                        List<Character> enemies = new();

                        foreach(Tile t in AttackableTiles)
                        {
                            if(t.tileObject != null && t.tileObject is Character character)
                            {
                                if(System.Array.IndexOf(group.allyGroup, t.tileObject.group.groupName) > -1)
                                    allies.Add(character);
                                else
                                    enemies.Add(character);
                            }
                        }
                        bool continueing = false;
                        
                        foreach(Character ch in enemies)
                        {
                            bestPos = BestPosition(ch, DecisionType.Attacker, out skillId);
                            battleClass = new(this, ch, skillId);
                            if(battleClass.totalDmg >= ch.realData.CurHP)
                            {
                                path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                                continueing = true;
                                break;
                            }
                        }
                        if(!continueing)
                        {
                            foreach(Character ch in allies)
                            {
                                if(ch.realData.CurHP <= ch.realData.HP * 0.75f)
                                {
                                    bestPos = BestPosition(ch, out skillId);
                                    path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                                    battleClass = new(this, ch, skillId);
                                    continueing = true;
                                    break;
                                }
                            }
                            if(!continueing)
                            {
                                if(false)
                                {
                                    //우선 대상의 버프 여하에 따라 버프 적용
                                }
                                else
                                {
                                    foreach(Character ch in allies)
                                    {
                                        if(false)
                                        {
                                            //범위 대상의 버프 여하에 따라 버프 적용
                                        }
                                    }
                                    if(!continueing)
                                    {
                                        foreach(Tile t in AttackableTiles)
                                        {
                                            if(System.Array.IndexOf(group.allyGroup, t.tileObject.group.groupName) == -1)
                                            {
                                                Tile _t = BestPosition(exTarget, DecisionType.Attacker, out skillId);
                                                battleClass = new(this, (Character)exTarget, skillId);
                                                path = tiles.PathFind(this, POS, (Vector2Int)_t.POS, false);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                    while(tiles.TurnAct == Act.Acting)
                    {
                        yield return null;
                    }
                    yield return ActStayTime;
                    tiles.BattleRun(battleClass);
                }
                else
                {
                    Tile bestPos = BestPosition(highTarget, out string highSkillId);
                    path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                    int distance = path.Length;
                    int _dis = int.MinValue;
                    TileObject lowTarget = null;
                    List<Character> allies = new();
                    List<Character> enemies = new();

                    foreach(Tile t in AttackableTiles)
                    {
                        if(t.tileObject != null && t.tileObject is Character character)
                        {
                            if(System.Array.IndexOf(group.allyGroup, t.tileObject.group) == -1) enemies.Add(character);
                            else allies.Add(character);
                        }
                    }
                    
                    bool continueing = false;
                    foreach(Character ch in enemies)
                    {
                        bestPos = BestPosition(highTarget, out string killingSkill);
                        battleClass = new(this, ch, killingSkill);
                        if(battleClass.totalDmg >= ch.realData.CurHP)
                        {
                            path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                            continueing = true;
                            break;
                        }
                    }
                    
                    if(!continueing)
                    {
                        foreach(Character character in allies)
                        {
                            if(character.realData.CurHP >= character.realData.HP * 0.75f) continue;

                            Tile highPos = BestPosition(character, out string skillId);
                            Tile[] hPath = tiles.PathFind(this, POS, (Vector2Int)highPos.POS, false);
                            int __dis = Mathf.Abs(hPath[^1].POS.x - path[^1].POS.x) + Mathf.Abs(hPath[^1].POS.y - path[^1].POS.y);
                            if(__dis >= distance && _dis > __dis)
                            {
                                _dis = __dis;
                                highSkillId = skillId;
                                path = hPath;
                                lowTarget = character;
                                continueing = true;
                            }
                        }
                        Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                    
                        while(tiles.TurnAct == Act.Acting)
                        {
                            yield return null;
                        }
                        yield return ActStayTime;
                        if(highTarget != null || lowTarget != null)
                        {
                            battleClass = new(this, highTarget != null ? (Character)highTarget : (Character)lowTarget, highSkillId);
                            tiles.BattleRun(battleClass);
                        }
                    }
                }
            }
            else
            {
                Tile _t = null;
                foreach(Tile t in AttackableTiles)
                {
                    if(System.Array.IndexOf(group.allyGroup, t.tileObject.group.groupName) == -1)
                    {
                        _t = BestPosition(exTarget, out string skillId);
                    }
                }
                if(_t == null)
                {
                    int dmg = int.MinValue;
                    Character target = null;
                    Tile bestPos = null;
                    string bestSkill = "";
                    foreach(Character character in tiles.Units)
                        if(System.Array.IndexOf(group.allyGroup, character.group.groupName) == -1)
                        {
                            foreach(string skillId in realData.attackSkills)
                            {
                                BattleClass battleClass = new(this, character, skillId);
                                if(battleClass.totalDmg > dmg)
                                {
                                    dmg = battleClass.totalDmg;
                                    target = character;
                                    bestPos = BestPosition(mostTarget, out bestSkill);
                                }
                            }
                        }
                    Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                    Move(moveType == MoveType.Moveable ? path[Mathf.Min(movement, path.Length - 1)].POS : tiles.tileDatas[POS.y, POS.x].POS);
                }
                else
                {
                    Tile bestPos = BestPosition(_t.tileObject, out string skillId);
                    Tile[] path = tiles.PathFind(this, POS, (Vector2Int)bestPos.POS, false);
                    Move(_t.POS);

                    while(tiles.TurnAct == Act.Acting)
                    {
                        yield return null;
                    }
                    yield return ActStayTime;
                    BattleClass battleClass = new(this, (Character)_t.tileObject, skillId);
                    tiles.BattleRun(battleClass);
                }
            }
        }
        else
        {
            Move(tiles.tileDatas[POS.y, POS.x].POS);
        }
    }
    public Tile BestPosition(TileObject tileObject, out string skillId)
    {
        string skill = realData.attackSkills[0];
        Tile targetTile = tiles.tileDatas[tileObject.TempPOS.y, tileObject.TempPOS.x];
        Tile bestTile = null;

        bool firstRun = true;
        if(decisionType == DecisionType.Attacker || decisionType == DecisionType.Supporter)
        {
            int dmg = 0;
            foreach (string attackSkill in realData.attackSkills)
            {
                BattleClass battleClass = new(this, (Character)tileObject, attackSkill);
                if(!firstRun && decisionType == DecisionType.Attacker && Mathf.Abs(battleClass.totalDmg) < 0 ||
                   (decisionType == DecisionType.Supporter && Mathf.Abs(battleClass.totalDmg) > 0))
                    continue;

                firstRun = false;
                int[] r = new int[2];
                for(int i = 0 ; i < 2; i++)
                {
                    r[i] = Convert.ToInt32(((string[])DataManager.Datas[attackSkill]["range"])[i]);
                }

                int rB = Convert.ToInt32(DataManager.Datas[weapon]["rangeBonus"]);

                if(Mathf.Abs(battleClass.totalDmg) >= Mathf.Abs(dmg))
                {
                    bestTile = FindVTileToAttack(targetTile, r[1] + rB, r[0]);
                    dmg = battleClass.totalDmg;
                    skill = attackSkill;
                }
            }
        }
        else
        {
            int MaxRange = int.MinValue;
            for (int i = Mathf.Max(0, targetTile.POS.x - 1); i <= Mathf.Min(tiles.size.x, targetTile.POS.x + 1); i++)
            {
                for(int j = Mathf.Max(0, targetTile.POS.y - 1); j <= Mathf.Min(tiles.size.y, targetTile.POS.y + 1); j++)
                {
                    if(!tiles.tileDatas[j, i].blocked && tiles.tileDatas[j, i].tileObject == null)
                    {
                        int distance = Mathf.Abs(i - POS.x) + Mathf.Abs(j - POS.y);
                        if(distance >= MaxRange)
                        {
                            bestTile = tiles.tileDatas[j, i];
                        }
                    }
                }
            }
        }
        skillId = skill;
        return bestTile;
    }
    public Tile BestPosition(TileObject tileObject, DecisionType type, out string skillId)
    {
        string skill = "";
        Tile targetTile = tiles.tileDatas[tileObject.TempPOS.y, tileObject.TempPOS.x];
        Tile bestTile = null;

        if(type == DecisionType.Attacker)
        {
            int dmg = 0;
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (string attackSkill in realData.attackSkills)
            {
                BattleClass battleClass = new(this, (Character)tileObject, attackSkill);
                if(Mathf.Abs(battleClass.totalDmg) < 0)
                    continue;

                int[] r = new int[2];
                for(int i = 0 ; i < 2; i++)
                {
                    r[i] = System.Convert.ToInt32(((string[])DataManager.Datas[attackSkill]["range"])[i]);
                }
                if (r[0] < min)
                    min = r[0];
                if (r[0] > max)
                    max = r[1];
                
                int rB = Convert.ToInt32(DataManager.Datas[weapon]["rangeBonus"]);

                if(Mathf.Abs(battleClass.totalDmg) > Mathf.Abs(dmg))
                {
                    bestTile = FindVTileToAttack(targetTile, max + rB, min);
                    dmg = battleClass.totalDmg;
                    skill = attackSkill;
                }
            }
        }
        else if(type == DecisionType.Supporter)
        {
            int dmg = 0;
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (string attackSkill in realData.attackSkills)
            {
                BattleClass battleClass = new(this, (Character)tileObject, attackSkill);
                if(Mathf.Abs(battleClass.totalDmg) > 0)
                    continue;

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
                
                if(Mathf.Abs(battleClass.totalDmg) > Mathf.Abs(dmg))
                {
                    bestTile = FindVTileToAttack(targetTile, max + rB, min);
                    dmg = battleClass.totalDmg;
                    skill = attackSkill;
                }
            }
        }
        else
        {
            int MaxRange = int.MinValue;
            for (int i = Mathf.Max(0, targetTile.POS.x - 1); i <= Mathf.Min(tiles.size.x, targetTile.POS.x + 1); i++)
            {
                for(int j = Mathf.Max(0, targetTile.POS.y - 1); j <= Mathf.Min(tiles.size.y, targetTile.POS.y + 1); j++)
                {
                    int diff = Mathf.Abs(i - targetTile.POS.x) + Mathf.Abs(j - targetTile.POS.y);
                    if(!tiles.tileDatas[j, i].blocked && tiles.tileDatas[j, i].tileObject == null)
                    {
                        int distance = Mathf.Abs(i - POS.x) + Mathf.Abs(j - POS.y);
                        if(distance >= MaxRange)
                        {
                            bestTile = tiles.tileDatas[j, i];
                        }
                    }
                }
            }
        }
        skillId = skill;
        return bestTile;
    }
    public TileObject TargetPriority(List<TileObject> cand)
    {
        cand.Distinct();
        int prio = int.MinValue;
        int MaxDmg = int.MinValue;
        TileObject MostDmgUnit = null;
        TileObject TargetUnit = null;

        foreach(TileObject obj in cand)
        {
            MostDmgUnit = MostDmgUnit != null ? MostDmgUnit : obj;
            TargetUnit = TargetUnit != null ? TargetUnit : obj;
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
        return TargetUnit;
    }
}