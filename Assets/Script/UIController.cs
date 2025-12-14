using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum VisibledActionUI
{
    None,
    Action,
    AttackSkills,
    Item
}
public class Preview
{
    public UnitPreview CasterPreview = new();
    public UnitPreview TargetPreview = new();
    public string attackSkill;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI SP;
    public TextMeshProUGUI Cost;
    public Image DmgType;
    public TextMeshProUGUI Power;
    public TextMeshProUGUI Desc;
}
public class UnitPreview
{
    private Character _data;
    public Character Data
    {
        get =>  _data;
        set
        {
            if(_data != value)
            {
                _data = value;
                UIController ui = GameManager.Inst.uiCon;
                if(!string.IsNullOrEmpty(ui.SetSkillId))
                {
                    if(value == null)
                    {
                        ui.RollbackPrev();
                        ui.InRangeEnemiesPrev();
                    }
                    else
                    {
                        ui.RollbackPrev();
                        ui.OnlyTargetPrev();
                    }
                }
            }
        }
    }
    public Transform Unit;
    public TextMeshProUGUI UnitName;
    public TextMeshProUGUI HP;
    public TextMeshProUGUI FullHP;
    public Image HealthBar;
    public Image PrevHealthBar;
    public RectTransform HPBox;
}
public class UIController : MonoBehaviour
{
    #region UnitDataUI
    public RectTransform UnitData;
    public Image UnitImage, Group;
    public TextMeshProUGUI Level, Name, EXP, Class;
    public RectTransform _HP, _SP, _ATK, _RYN, _SPD, _DEX, _DEF, _RES, _DPN, _RPN, _STR, EXPBar;
    public TextMeshProUGUI HP, SP, ATK, RYN, SPD, DEX, DEF, RES, DPN, RPN, STR, HalfSpirit;
    public RectTransform WEAPON, ACCESSORY, INVENTORY, SKILLS;
    public RectTransform SkillPrefab;
    #endregion

    #region uiDatas
    private Character settedUnitFieldPrieview;
    public Character SettedUnitFieldPrieview
    {
        get => settedUnitFieldPrieview;
        set
        {
            if(value != settedUnitFieldPrieview)
            {
                SetUnitFieldPreview(value);
            }
            settedUnitFieldPrieview = value;
        }
    }
    public RectTransform UnitFieldPreview;
    private Tiles tiles;
    private string tempSelectedUI = "";
    private string selectedUI = "";
    public RectTransform tempToShowTooltipSelectedUI, ToShowTooltipSelectedUI;
    public string TempSelectedUI
    {
        get => tempSelectedUI;
        set
        {
            if(selectedUI == "")
            {
                SetDescToolTip(value);
                ShowDescToolTip(tempToShowTooltipSelectedUI);
            }
            tempSelectedUI = value;
        }
    }
    public string SelectedUI
    {
        get => selectedUI;
        set
        {
            if(selectedUI == value)
            {
                selectedUI = "";
                return;
            }
            if(!string.IsNullOrEmpty(value))
            {
                SetDescToolTip(value);
                ShowDescToolTip(ToShowTooltipSelectedUI);
            }
            selectedUI = value;
        }
    }
    public RectTransform TooltipList, MainDescBox, SubDescBox;
    public Sprite[] DamageType;
    public Image HoldRing;
    private readonly int Season = 2;
    public RectTransform Minimap;
    public List<TurnQueueItem> TurnQueues = new();
    // private bool RandomCardVisibled = true;
    private bool TurnQueueVisibled = true;
    public float UISpeed = 20f;
    public GameObject Inflict, Recieve;
    public GameObject Actions, AttackSkills, Items;
    public GameObject SkillPreview;
    public Preview Preview;
    private Coroutine ActionsSliding;
    private Coroutine AttackSkillsSliding;
    private Coroutine ItemsSliding;
    private Coroutine TurnQueueSliding;
    // private Coroutine RandomCardsSliding;
    public Transform AttackSkillList;
    public Transform BattlePreviewBoard;
    public List<GameObject> IgnoreClick = new();
    public List<string> canUseAtkSkill = new();
    public Tile[] backupTiles = new Tile[0];
    public Tile[] selectSkillBackupTiles;
    // public Transform RandomCards;
    public RectTransform TurnQueue;
    public Transform QueueList;
    public GameObject TQItem;
    public Tile[] targetsTiles = new Tile[0];
    private string setskillid;
    public string SetSkillId
    {
        get => setskillid;
        set
        {
            if(setskillid != value)
            {
                setskillid = value;
                RollbackPrev();
                if(!string.IsNullOrEmpty(value))
                {
                    if(Preview.TargetPreview.Data == null)
                    {
                        InRangeEnemiesPrev();
                    }
                    else
                    {
                        OnlyTargetPrev();
                    }
                }
            }
        }
    }
    public BattleClass battleClass;
    public GameObject ExpectTarget;
    private VisibledActionUI visibled = VisibledActionUI.None;
    public VisibledActionUI Visibled
    {
        get => visibled;
        set
        {
            visibled = value;
            switch(value)
            {
                case VisibledActionUI.None:
                    break;
                case VisibledActionUI.Action:
                    ActionsOn();
                    break;
                case VisibledActionUI.Item:
                    ItemsOn();
                    break;
                case VisibledActionUI.AttackSkills:
                    AttackOn();
                    break;
            }
        }
    }
    #endregion

    #region method
    private void Awake()
    {
        tiles = GameManager.Inst.tiles;
        ActionsSliding = null;
        AttackSkillsSliding = null;
        ItemsSliding = null;
        AttackSkills.transform.localScale = Vector3.zero;
        Items.transform.localScale = Vector3.zero;
        InitPreview();
    }
    private void InitPreview()
    {
        Preview = new()
        {
            CasterPreview = new UnitPreview
            {
                Unit = SkillPreview.transform.Find("BattlePrev").Find("Caster")
            },
            TargetPreview = new UnitPreview
            {
                Unit = SkillPreview.transform.Find("BattlePrev").Find("Target")
            }
        };
        Preview.CasterPreview.UnitName = Preview.CasterPreview.Unit.Find("Portrait").GetComponentInChildren<TextMeshProUGUI>();
        Preview.CasterPreview.HP = Preview.CasterPreview.Unit.Find("Health").GetComponentInChildren<TextMeshProUGUI>();
        Preview.CasterPreview.FullHP = Preview.CasterPreview.Unit.Find("Health").Find("FullHP").GetComponentInChildren<TextMeshProUGUI>();
        Preview.CasterPreview.HPBox = Preview.CasterPreview.HP.transform.parent.parent.GetComponent<RectTransform>();
        Preview.CasterPreview.HealthBar = Preview.CasterPreview.Unit.Find("Health").Find("HealthBar").GetComponent<Image>();
        Preview.CasterPreview.PrevHealthBar = Preview.CasterPreview.Unit.Find("Health").Find("PrevHealthBar").GetComponent<Image>();

        Preview.TargetPreview.UnitName = Preview.TargetPreview.Unit.Find("Portrait").GetComponentInChildren<TextMeshProUGUI>();
        Preview.TargetPreview.HP = Preview.TargetPreview.Unit.Find("Health").GetComponentInChildren<TextMeshProUGUI>();
        Preview.TargetPreview.FullHP = Preview.TargetPreview.Unit.Find("Health").Find("FullHP").GetComponentInChildren<TextMeshProUGUI>();
        Preview.TargetPreview.HPBox = Preview.TargetPreview.HP.transform.parent.parent.GetComponent<RectTransform>();
        Preview.TargetPreview.HealthBar = Preview.TargetPreview.Unit.Find("Health").Find("HealthBar").GetComponent<Image>();
        Preview.TargetPreview.PrevHealthBar = Preview.TargetPreview.Unit.Find("Health").Find("PrevHealthBar").GetComponent<Image>();

        Preview.Name = SkillPreview.transform.Find("Name").GetComponentInChildren<TextMeshProUGUI>();
        Preview.SP = SkillPreview.transform.parent.Find("Scroll View").Find("SPBox").GetComponentInChildren<TextMeshProUGUI>();
        Preview.Cost = AttackSkills.transform.Find("SkillDesc").Find("DmgBox").Find("Cost").GetComponent<TextMeshProUGUI>();
        Preview.DmgType = AttackSkills.transform.Find("SkillDesc").Find("DmgBox").Find("DmgType").GetComponent<Image>();
        Preview.Power = AttackSkills.transform.Find("SkillDesc").Find("DmgBox").Find("DmgType").GetComponentInChildren<TextMeshProUGUI>();
        Preview.Desc = AttackSkills.transform.Find("SkillDesc").Find("DescBox").GetComponentInChildren<TextMeshProUGUI>();
    }
    private void Start() 
    {
        UILangSet();
    }
    public void UILangSet()
    {
        TextMeshProUGUI[] Texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach(TextMeshProUGUI t in Texts)
        {
            if(DataManager.textData.ContainsKey(t.name.Replace("Text_", "")))
            {
                t.text = DataManager.textData[t.name.Replace("Text_", "")][GameManager.langCode];
            }
        }
    }
    public void SetUnitFieldPreview(Character unit)
    {
        if(unit == null)
        {
            UnitFieldPreview.gameObject.SetActive(false);
            return;
        }
        UnitFieldPreview.gameObject.SetActive(true);
        string unitId = unit.id;
        UnitFieldPreview.Find("illustration").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Images/Character/{unitId}/{unitId.ToLower()}_normal");
        UnitFieldPreview.GetComponentInChildren<TextMeshProUGUI>().text = $"{Mathf.FloorToInt(unit.realData.CurHP)}";
        UnitFieldPreview.Find("HealthBar").Find("HealthBar").GetComponent<Image>().fillAmount = unit.realData.CurHP / unit.realData.HP;
    }
    //Action UI
    public void SetVisibled(string visibledUI)
    {
        Visibled = visibledUI switch
        {
            "action" => VisibledActionUI.Action,
            "items" => VisibledActionUI.Item,
            "attackskills" => VisibledActionUI.AttackSkills,
            _ => VisibledActionUI.None,
        };
    }
    public void ActionsOn()
    {
        ActionsSliding ??= StartCoroutine(ActionsVisible());
        foreach(Tile t in tiles.SelectedChecker)
            t.checker.gameObject.SetActive(false);
        tiles.SelectedChecker.Clear();
    }
    public IEnumerator ActionsVisible()
    {
        foreach (Transform child in AttackSkillList)
        {
            child.gameObject.SetActive(false);
        }

        bool t = false;
        foreach (string attackSkill in canUseAtkSkill)
        {
            if(!t)
            {
                t = true;
                Preview.SP.text = $"SP : {tiles.TurnChar.SP} (-" + DataManager.Datas[attackSkill]["cost"] + ")";
                Preview.Name.text = DataManager.textData[attackSkill][GameManager.langCode];
                Preview.Cost.text = $"SP : " + DataManager.Datas[attackSkill]["cost"];
                Preview.DmgType.color = System.Convert.ToChar(DataManager.Datas[attackSkill]["dmgType"]) == 'p' ? Color.cyan : Color.black;
                Preview.Desc.text = DataManager.textData[$"DESC_{attackSkill}"][GameManager.langCode];
                Preview.Power.text = 
                    DataManager.Datas[attackSkill].ContainsKey("dmg") || System.Convert.ToInt32(DataManager.Datas[attackSkill]["dmg"]) == 0? 
                    DataManager.ToInt(DataManager.Datas[attackSkill]["attackCount"]) > 1? 
                    DataManager.Datas[attackSkill]["dmg"].ToString() + " x " + DataManager.Datas[attackSkill]["attackCount"].ToString() :
                    DataManager.Datas[attackSkill]["dmg"].ToString() : "-";
            }

            AttackSkillList.transform.Find(attackSkill).gameObject.SetActive(true);
        }

        if(Visibled == VisibledActionUI.Action)
        {
            IgnoreClick.Add(Actions);
            for(int i = 0; i < Actions.transform.childCount; i++)
            {
                RectTransform childBox = Actions.transform.GetChild(i).GetComponent<RectTransform>();
                StartCoroutine(ActionButtonUpDown(childBox, false));

                while((childBox.anchoredPosition.y < -70 && i < Actions.transform.childCount - 1) || (childBox.anchoredPosition.y != -20 && i == Actions.transform.childCount - 1))
                {
                    yield return null;
                }
            }
        }
        else
        {
            IgnoreClick.Remove(Actions);
            for (int i = 0; i < Actions.transform.childCount; i++)
            {
                RectTransform childBox = Actions.transform.GetChild(i).GetComponent<RectTransform>();
                StartCoroutine(ActionButtonUpDown(childBox, true));
                while ((childBox.anchoredPosition.y > -70 && i < Actions.transform.childCount - 1) || (childBox.anchoredPosition.y != -160 && i == Actions.transform.childCount - 1))
                {
                    yield return null;
                }
            }
        }
        ActionsSliding = null;
    }
    public void ItemsOn()
    {
        ItemsSliding ??= StartCoroutine(ItemsVisible());
    }
    public IEnumerator ItemsVisible()
    {
        RectTransform rect = Items.GetComponent<RectTransform>();
        SkillPreview.SetActive(false);
        if (Visibled == VisibledActionUI.Item)
        {
            ActionsOn();
            IgnoreClick.Add(Items);
            while(rect.localScale.x < 0.98f)
            {
                rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(4, 4, 4), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.localScale = Vector3.one;
            tiles.TurnAct = Act.Item;
        }
        else
        {
            IgnoreClick.Remove(Items);
            while (rect.localScale.x > 0.02f)
            {
                rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(-3, -3, -3), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.localScale = Vector3.zero;
        }
        ItemsSliding = null;
    }
    public void AttackOn()
    {
        AttackSkillsSliding ??= StartCoroutine(AttackListVisible());
    }
    public IEnumerator AttackListVisible()
    {
        RectTransform rect = AttackSkills.GetComponent<RectTransform>();
        SkillPreview.SetActive(false);
        if (Visibled == VisibledActionUI.AttackSkills)
        {
            ActionsOn();
            IgnoreClick.Add(AttackSkills);
            while(rect.localScale.x < 0.98f)
            {
                rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(4, 4, 4), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.localScale = Vector3.one;
            tiles.TurnAct = Act.Attack;
        }
        else
        {
            IgnoreClick.Remove(AttackSkills);
            while (rect.localScale.x > 0.02f)
            {
                rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(-3, -3, -3), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.localScale = Vector3.zero;
        }
        AttackSkillsSliding = null;
    }
    public IEnumerator ActionButtonUpDown(RectTransform rect, bool down)
    {
        yield return null;
        if(down)
        {
            while(rect.anchoredPosition.y > -160)
            {
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, new Vector2(rect.anchoredPosition.x, -300), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -160);
        }
        else
        {
            while(rect.anchoredPosition.y < -20)
            {
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, new Vector2(rect.anchoredPosition.x, 120), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -20);
        }
    }
    //Battle UI
    public void SetCasterPreviewUnit()
    {
        Character caster = Preview.CasterPreview.Data;
        GameObject cObj = Instantiate(Resources.Load<GameObject>($"Prefab/Standing_Character/{caster.id}"), Preview.CasterPreview.Unit.Find("Portrait"));
        Destroy(Preview.CasterPreview.Unit.Find("Portrait").Find("Base").gameObject);
        Preview.CasterPreview.UnitName.text = DataManager.textData[caster.id.ToUpper()][GameManager.langCode];
        cObj.name = "Base";
        cObj.transform.SetSiblingIndex(1);
        SetPreview(Preview.attackSkill);
    }
    public void SetTargetPreviewUnit()
    {
        if(Preview.TargetPreview.Data == null)
            return;
        Character target = Preview.TargetPreview.Data;
        GameObject tObj = Instantiate(Resources.Load<GameObject>($"Prefab/Standing_Character/{target.id}"), Preview.TargetPreview.Unit.Find("Portrait"));
        Destroy(Preview.TargetPreview.Unit.Find("Portrait").Find("Base").gameObject);
        Preview.TargetPreview.UnitName.text = DataManager.textData[target.id.ToUpper()][GameManager.langCode];
        tObj.name = "Base";
        tObj.transform.SetSiblingIndex(1);
        SetPreview(Preview.attackSkill);
    }
    public void SetPreview(string skillId)
    {
        Character caster = Preview.CasterPreview.Data;
        Character target = Preview.TargetPreview.Data;
        CharacterData casterData = Preview.CasterPreview.Data.realData;
        CharacterData targetData = Preview.TargetPreview.Data != null ? Preview.TargetPreview.Data.realData : null;
        Dictionary<string, object> skillData = DataManager.Datas[skillId];

        foreach(Transform child in BattlePreviewBoard)
        {
            Destroy(child.gameObject);
        }

        if(targetData == null)
        {
            Preview.TargetPreview.Unit.gameObject.SetActive(false);
        }
        else
        {
            Preview.TargetPreview.Unit.gameObject.SetActive(true);
            if(skillData.ContainsKey("dmg"))
            {
                battleClass = new(caster, target, skillId);

                for(int i = 0; i < battleClass.battleDatas.Count; i++)
                {
                    InReArrow(battleClass.battleDatas[i].InRe, battleClass.battleDatas[i].dmg, battleClass.battleDatas[i].acc, battleClass.battleDatas[i].atkCount);
                }

                int totalDmg = battleClass.totalDmg;
                int totalCountDmg = battleClass.totalCountDmg;

                int casterRemainHP = Mathf.FloorToInt(Mathf.Clamp(Mathf.FloorToInt(casterData.CurHP) - totalCountDmg, 0, (int)casterData.HP));
                int targetRemainHP = Mathf.FloorToInt(Mathf.Clamp(Mathf.FloorToInt(targetData.CurHP) - totalDmg, 0, (int)targetData.HP));

                Preview.CasterPreview.FullHP.text = $"{Mathf.FloorToInt(casterData.HP)}";
                Preview.TargetPreview.FullHP.text = $"{Mathf.FloorToInt(targetData.HP)}";

                Preview.CasterPreview.HP.text = $"{casterRemainHP}";
                Preview.TargetPreview.HP.text = $"{targetRemainHP}";

                Preview.CasterPreview.PrevHealthBar.fillAmount = casterRemainHP / casterData.HP;
                Preview.TargetPreview.PrevHealthBar.fillAmount = targetRemainHP / targetData.HP;
                
                Preview.CasterPreview.HPBox.anchoredPosition = 
                new Vector2(Preview.CasterPreview.PrevHealthBar.rectTransform.sizeDelta.x * (1 - (casterRemainHP / casterData.HP)), -3.5f);
                Preview.TargetPreview.HPBox.anchoredPosition = 
                new Vector2(-Preview.TargetPreview.PrevHealthBar.rectTransform.sizeDelta.x * (1 - (targetRemainHP / targetData.HP)), -3.5f);

                if(Preview.CasterPreview.PrevHealthBar.fillAmount > 0.25f)
                    Preview.CasterPreview.PrevHealthBar.color = Color.white;
                else
                    Preview.CasterPreview.PrevHealthBar.color = Color.red;
                if(Preview.TargetPreview.PrevHealthBar.fillAmount > 0.25f)
                    Preview.TargetPreview.PrevHealthBar.color = Color.white;
                else
                    Preview.TargetPreview.PrevHealthBar.color = Color.red;
            }
        }
    }
    public RectTransform InReArrow(bool InRe, int val, int acc, int atkCount)
    {
        GameObject obj = InRe ? Instantiate(Recieve, BattlePreviewBoard) : 
                                Instantiate(Inflict, BattlePreviewBoard);
        obj.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = (atkCount <= 1) ? $"{val}" : $"{val} × {atkCount}";
        obj.transform.Find("Percentage").GetComponent<TextMeshProUGUI>().text = $"{acc}%";
        return obj.GetComponent<RectTransform>();
    }
    public void SkillShift(bool LeftOrRight)
    {
        Character caster = Preview.CasterPreview.Data;
        Character target = Preview.TargetPreview.Data;
        int distance = Mathf.Abs(caster.TempPOS.x - target.POS.x) + Mathf.Abs(caster.TempPOS.y - target.POS.y);
        List<string> attackableSkills = new();
        for(int i = 0 ; i < caster.realData.attackSkills.Count; i++)
        {
            int[] r = DataManager.ToIntArr(DataManager.Datas[caster.realData.attackSkills[i]]["range"]);
            if(r[0] >= distance
            && r[1] + DataManager.ToInt(DataManager.Datas[caster.weapon]["rangeBonus"]) <= distance)
            {
                attackableSkills.Add(caster.realData.attackSkills[i]);
            }
        }
        int index = attackableSkills.IndexOf(Preview.attackSkill);
        AttackSkillChange(attackableSkills[LeftOrRight ? 
        index == attackableSkills.Count - 1 ? 0 : index + 1 
        :index == 0 ? attackableSkills.Count - 1 : index - 1]);
    }
    public void AttackSkillChange(string id)
    {
        if(string.IsNullOrEmpty(id))
        {
            RollbackPrev();
            return;
        }
        SetSkillId = id;
        Dictionary<string, object> skillData = DataManager.Datas[SetSkillId];
        Character caster = Preview.CasterPreview.Data;
        targetsTiles = tiles.CalcAttackableTile(caster, caster.TempPOS, id);
        tiles.TurnChar.AttackableTiles = targetsTiles;
        
        if(string.IsNullOrEmpty(id))
            return;
        foreach(Tile tile in tiles.Checker)
        {
            tile.checker.gameObject.SetActive(false);
        }
        tiles.Checker.Clear();
        Preview.attackSkill = SetSkillId;

        SetCasterPreviewUnit();
        SetTargetPreviewUnit();

        string setSkill = SetSkillId;
        string[] NameDesc = DataManager.DescConvert(setSkill);
        Preview.SP.text = $"SP : {tiles.TurnChar.SP} (-" + skillData["cost"] + ")";
        Preview.Name.text = NameDesc[0];
        Preview.Cost.text = $"SP : " + skillData["cost"];
        Preview.Power.text = 
                skillData.ContainsKey("dmg") || Convert.ToInt32(skillData["dmg"]) == 0? 
                DataManager.ToInt(skillData["attackCount"]) > 1? 
                skillData["dmg"].ToString() + " × " + skillData["attackCount"].ToString() :
                skillData["dmg"].ToString() : "-";
        Preview.DmgType.sprite = Convert.ToChar(skillData["dmgType"]) == 'p' ? DamageType[0] : DamageType[1];
        Preview.Desc.text = NameDesc[1];
    }
    public void RollbackPrev()
    {
        if(string.IsNullOrEmpty(SetSkillId))
        return;
        Dictionary<string, object> skillData = DataManager.Datas[SetSkillId];
        Character caster = Preview.CasterPreview.Data;
        int[] r = DataManager.ToIntArr(skillData["range"]);
        targetsTiles = tiles.CalcAttackableTile(caster, caster.TempPOS, r[0], r[1] + DataManager.ToInt(DataManager.Datas[caster.weapon]["rangeBonus"]));
        tiles.TurnChar.AttackableTiles = targetsTiles;
        CharacterData casterData = caster.realData;
        caster.SetPrevHPBar((float)casterData.CurHP);
        foreach(Character unit in tiles.Units) unit.SetPrevHPBar((float)unit.realData.CurHP);
    }
    public void InRangeEnemiesPrev()
    {
        Character caster = Preview.CasterPreview.Data;
        targetsTiles = tiles.CalcAttackableTile(caster, caster.TempPOS, SetSkillId);
        tiles.TurnChar.AttackableTiles = targetsTiles;
        CharacterData casterData = caster.realData;
        caster.SetPrevHPBar((float)casterData.CurHP);
        foreach(Tile t in targetsTiles)
            if(t.tileObject != null)
            {
                BattleClass battleClass = new(caster, (Character)t.tileObject, SetSkillId);            
                ((Character)t.tileObject).SetPrevHPBar(Mathf.FloorToInt(t.tileObject.realData.CurHP) - battleClass.totalDmg);
            }
    }
    public void OnlyTargetPrev()
    {
        Character caster = Preview.CasterPreview.Data;
        Character target = Preview.TargetPreview.Data;
        tiles.TurnChar.AttackableTiles = targetsTiles;
        CharacterData casterData = caster.realData;
        CharacterData targetData = target.realData;
        if(Array.IndexOf(targetsTiles, tiles.tileDatas[target.POS.y, target.POS.x]) > -1)
        {
            BattleClass battleClass = new(caster, target, SetSkillId);  
            caster.SetPrevHPBar((float)casterData.CurHP - battleClass.totalCountDmg);          
            target.SetPrevHPBar(Mathf.FloorToInt(targetData.CurHP) - battleClass.totalDmg);
        }
        else
        {
            SkillPreview.SetActive(false);
        }
    }
    public void CheckerClear()
    {
        foreach(Tile t in tiles.Checker)
        {
            t.checker.gameObject.SetActive(false);
        }
        foreach(Tile t in tiles.SelectedChecker)
        {
            t.checker.gameObject.SetActive(false);
        }
        tiles.SelectedChecker.Clear();
        tiles.Checker.Clear();
    }
    // public void CardsSwitching()
    // {
    //     RandomCardsSliding ??= StartCoroutine(CdSwitching());
    // }
    // public IEnumerator CdSwitching()
    // {
    //     RectTransform bg = RandomCards.Find("Background").GetComponent<RectTransform>();
    //     if(RandomCardVisibled)
    //     {
    //         while(bg.anchoredPosition.y > 0)
    //         {
    //             bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(0, -20), Time.deltaTime * UISpeed);
    //             yield return null;
    //         }
    //         bg.anchoredPosition = new Vector2(0, 0);
    //     }
    //     else
    //     {
    //         while(bg.anchoredPosition.y < 200)
    //         {
    //             bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(0, 220), Time.deltaTime * UISpeed);
    //             yield return null;
    //         }
    //         bg.anchoredPosition = new Vector2(0, 200);
    //     }
    //     RandomCardVisibled = !RandomCardVisibled;
    //     TextMeshProUGUI tmpUGUI = RandomCards.Find("Background").Find("CardsSwitch").GetComponentInChildren<TextMeshProUGUI>();
    //     tmpUGUI.text = tmpUGUI.text == "∧" ? "∨" : "∧";
    //     RandomCardsSliding = null;
    // }
    public void TurnQueueSwitching()
    {
        TurnQueueSliding ??= StartCoroutine(TqSwitching());
    }
    public IEnumerator TqSwitching()
    {
        RectTransform bg = TurnQueue.Find("Background").GetComponent<RectTransform>();
        if(TurnQueueVisibled)
        {
            while(bg.anchoredPosition.y > 0)
            {
                bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(0, -20), Time.deltaTime * UISpeed);
                yield return null;
            }
            bg.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            while(bg.anchoredPosition.y < 128)
            {
                bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(0, 148), Time.deltaTime * UISpeed);
                yield return null;
            }
            bg.anchoredPosition = new Vector2(0, 128);
        }
        TurnQueueVisibled = !TurnQueueVisibled;
        TextMeshProUGUI tmpUGUI = TurnQueue.Find("Background").Find("TurnQueueSwitch").GetComponentInChildren<TextMeshProUGUI>();
        tmpUGUI.text = tmpUGUI.text == "〉" ? "〈" : "〉";
        TurnQueueSliding = null;
    }
    public void SyncTurnQueue()
    {
        for(int i = TurnQueues.Count - 1; i > 0; i--)
        {
            TurnQueues.RemoveAt(i);
            Destroy(QueueList.GetChild(i).gameObject);
        }
        for(int i = 0; i < tiles.TurnQueue.Count; i++)
        {
            GameObject t = Instantiate(TQItem, QueueList);
            t.name = tiles.TurnQueue[i].id;
            TurnQueueItem item = t.GetComponent<TurnQueueItem>();
            item.unitP.Init(tiles.TurnQueue[i]);
            TurnQueues.Add(item);
        }
    }
    public void CloseAttack()
    {
        AttackSkillChange(null);
        foreach(Tile t in targetsTiles)
            if(t.tileObject != null)
            {
                Character _target = t.tileObject as Character;
                _target.SetPrevHPBar(t.tileObject.realData.CurHP);
            }
        CursorManager.CursorChange(CursorMode.Normal);
        tiles.TurnAct = Act.Operate;
        foreach(Tile t in tiles.TurnChar.AttackRange)
        {
            tiles.SetTileState(t, TileState.RedTile);
        }
    }
    public void CloseSkills()
    {
    }
    public void CloseItems()
    {
    }
    public void MapExpand()
    {
        switch(Minimap.rect.height)
        {
            case 0 : 
                Minimap.sizeDelta = new Vector2(512, 512);
                break;
            case 512 : 
                Minimap.sizeDelta = new Vector2(1024, 1024);
                break;
            case 1024 : 
                Minimap.sizeDelta = new Vector2(512, 0);
                break;
        }
    }   
    public void UnitDataUISet(Character character)
    {
        UnitDataUISet(character.realData);
    }
    public void UnitDataUISet(CharacterData data)
    {
        UnitImage.sprite = Resources.Load<Sprite>($"Images/Character/{data.id}/{data.id.ToLower()}_normal");

        #region group
        Group.sprite = Resources.Load<Sprite>($"Images/Character/Background/Symbol/{data.group}");
        #endregion
        #region detail
        Name.text = string.Empty;
        Class.text = string.Empty;
        Level.text = string.Empty;
        Level.text = $"<color=#D7D7D7><size=45>LV.</size></color>{data.Lv}";
        
        string desc = DataManager.textData.ContainsKey($"DESC_{data.id}") ? DataManager.LangText("DESC_" + data.id) : "";
        if(desc.IndexOf($"<s{Season}>") > - 1)
            desc = desc.Split(new string[]{$"<s{Season}>", $"</s{Season}>"}, StringSplitOptions.None)[1]; 
        Name.text += $"{DataManager.LangText(data.id)} <color=#A6A6A6><size=28>{desc}</size></color>".Replace("\r", "");
        EXP.text = $"{data.EXP} / {data.MaxExp}";
        Class.text += $"{DataManager.LangText(data.characterClass)} <color=#A6A6A6><size=16>({data.ClassEXP}%)</size></color>".Replace("\r", "");
        Class.name = data.characterClass;
        #endregion
        #region status
        HP.text = $"{Mathf.FloorToInt(data.HP)}";
        SP.text = $"{Mathf.FloorToInt(data.SP)}";
        ATK.text = $"{Mathf.FloorToInt(data.Atk)}";
        RYN.text = $"{Mathf.FloorToInt(data.Ryn)}";
        SPD.text = $"{Mathf.FloorToInt(data.Spd)}";
        DEX.text = $"{Mathf.FloorToInt(data.Dex)}";
        DEF.text = $"{Mathf.FloorToInt(data.Def)}";
        RES.text = $"{Mathf.FloorToInt(data.Res)}";
        DPN.text = $"{Mathf.FloorToInt(data.Dpn)}";
        RPN.text = $"{Mathf.FloorToInt(data.Rpn)}";
        STR.text = $"{Mathf.FloorToInt(data.Str)}";
        HalfSpirit.text = string.IsNullOrEmpty(data.HalfSpirit) ? "-" : $"{DataManager.LangText(data.HalfSpirit)}";
        HalfSpirit.name = string.IsNullOrEmpty(data.HalfSpirit) ? "-" : $"{data.HalfSpirit}";
        #endregion
        #region items
        WEAPON.GetComponentInChildren<TextMeshProUGUI>().text = 
        string.IsNullOrEmpty(data.Char.weapon) ? "-" : $"{DataManager.LangText(data.Char.weapon)}";
        WEAPON.name = string.IsNullOrEmpty(data.Char.weapon) ? "-" : data.Char.weapon;
        ACCESSORY.GetComponentInChildren<TextMeshProUGUI>().text = 
        string.IsNullOrEmpty(data.Char.accessory) ? "-" : $"{DataManager.LangText(data.Char.accessory)}";
        ACCESSORY.name = string.IsNullOrEmpty(data.Char.accessory) ? "-" : data.Char.accessory;


        int MaxSlot;
        if (data.Lv < 5) MaxSlot = 1; else if(data.Lv < 15) MaxSlot = 2; else MaxSlot = 3;
        for(int i = 1; i <= 3; i++)
        {
            INVENTORY.GetChild(i).gameObject.SetActive(i <= MaxSlot);
            INVENTORY.GetChild(i).Find("Name").GetComponent<TextMeshProUGUI>().text = data.inventory.Count >= i ? $"{DataManager.LangText(data.inventory[i - 1])}" : "-";
            INVENTORY.GetChild(i).name = data.inventory.Count >= i ? data.inventory[i - 1] : "BlankSlot";
            INVENTORY.GetChild(i).Find("EA").GetComponent<TextMeshProUGUI>().text = 
            data.inventory.Count >= i && DataManager.ToInt(DataManager.Datas[data.inventory[i - 1]]["maxEA"]) > 0 ? $"{data.inventory_ea[i - 1]}" : "";
        }
        #endregion 
        #region skills
        for(int i = SKILLS.childCount - 1; i >= 0; i--) Destroy(SKILLS.GetChild(i).gameObject);
        Dictionary<string, int> skills = new();
        foreach(string s in data.attackSkills)
        {
            if(s.IndexOf("NormalAttack") > -1)
                continue;
            if(skills.ContainsKey(s))
                skills[s]++;
            else
                skills.Add(s, 1);
        } 
        foreach(string s in data.Skills) if(skills.ContainsKey(s)) skills[s]++; else skills.Add(s, 1);        
        if(skills.Count == 0)
        {
            GameObject obj = Instantiate(SkillPrefab.gameObject, SKILLS);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = "-";
        }
        else
        {
            foreach(KeyValuePair<string, int> skill in skills)
            {
                GameObject obj = Instantiate(SkillPrefab.gameObject, SKILLS);
                obj.name = skill.Key;
                obj.GetComponentInChildren<TextMeshProUGUI>().text = skill.Value == 1 ? $"{DataManager.LangText(skill.Key)}" : $"{DataManager.LangText(skill.Key)} Lv.{skill.Value}";
            }
        }
        #endregion
    }
    public void UnitDataVisible(bool visible)
    {
        SetDescToolTip("");
        UnitData.gameObject.SetActive(visible);
        GameManager.Inst.InputBlock = visible;
        if(visible)
        {
            CursorManager.CursorChange(CursorMode.Normal);
            ExpectTarget.SetActive(false);
        }
        else if(tiles.TurnAct == Act.SelectSkill)
            CursorManager.CursorChange(CursorMode.Attack);
    }
    public void BattleRun()
    {
        Visibled = VisibledActionUI.None;
        tiles.BattleRun(battleClass);
        AttackOn();
    }
    public void SetExpectTarget(Character caster, string skillId, TileObject _target)
    {
        if(_target == null || _target is not Character || caster.group.groupName == "Player")
        {
            ExpectTarget.SetActive(false);
            return;
        }
        
        Character target = (Character)_target;
        ExpectTarget.SetActive(true);
        RectTransform rect = ExpectTarget.GetComponentInChildren<RectTransform>();
        ExpectTarget.GetComponentInChildren<UnitPortrait>().Init(target);
        foreach(Transform tr in ExpectTarget.transform.Find("ArrowDisplay"))
        {
            Destroy(tr.gameObject);
        }
        int[] r = DataManager.ToIntArr(DataManager.Datas[skillId]["range"]);
        int rB = DataManager.ToInt(DataManager.Datas[caster.weapon]["rangeBonus"]);
        Tile assumedTile = caster.FindVTileToAttack(tiles.tileDatas[target.TempPOS.y, target.TempPOS.x], r[0] + rB, r[0]);
        BattleClass battleClass = new(caster, target, skillId, (Vector2Int)assumedTile.POS);
        
        rect.anchoredPosition = new Vector2(-60f, 60f);
        foreach(BattleData bD in battleClass.battleDatas)
        {
            RectTransform obj = InReArrow(!bD.InRe, bD.dmg, bD.acc, bD.atkCount);
            obj.SetParent(ExpectTarget.transform.Find("ArrowDisplay"));
            obj.localScale = new Vector3(0.85f, 0.85f, 1);
            rect.anchoredPosition += new Vector2(0, 20f);
        }

        ExpectTarget.transform.Find("PrevHealth").GetComponent<Image>().fillAmount = 
        (float)(Mathf.FloorToInt(target.realData.CurHP) - battleClass.totalDmg) / Mathf.FloorToInt(target.realData.HP);
    }
    public void ShowDescToolTip(RectTransform rect)
    {
        TooltipList.position = rect.position;
        TooltipList.anchoredPosition += new Vector2(
        TooltipList.anchoredPosition.x + rect.rect.width > 1920.0f - TooltipList.rect.width ? 
        -TooltipList.rect.width : rect.rect.width, 
        TooltipList.anchoredPosition.y + rect.rect.height/2 < TooltipList.rect.height - 1080.0f ? 
        TooltipList.rect.height - 1080.0f - TooltipList.anchoredPosition.y : rect.rect.height/2);
    }
    public void SetDescToolTip(string id)
    {
        SubDescBox.gameObject.SetActive(false);
        if(string.IsNullOrEmpty(id) || !DataManager.textData.ContainsKey(id))
        {
            TooltipList.gameObject.SetActive(false);
            return;
        }
        TooltipList.gameObject.SetActive(true);
        string[] con = DataManager.DescConvert(id);
        MainDescBox.Find("Name").GetComponent<TextMeshProUGUI>().text = $"▼ {con[0]}";
        MainDescBox.Find("Desc").GetComponent<TextMeshProUGUI>().text = $"{con[1]}";
        StartCoroutine(ForceUpdateUI(TooltipList));
    }
    public void ShowOrHideTooltipDesc(RectTransform desc)
    {
        TextMeshProUGUI n = desc.transform.parent.Find("Name").GetComponent<TextMeshProUGUI>();
        n.text = n.text.IndexOf('▼') > -1 ? n.text.Replace('▼', '▲') : n.text.Replace('▲', '▼');
        desc.gameObject.SetActive(!desc.gameObject.activeSelf);
        StartCoroutine(ForceUpdateUI(TooltipList));
    }
    public IEnumerator ForceUpdateUI(RectTransform tr)
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(tr);
    }
    #endregion
}