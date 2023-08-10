using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

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
    public Character data;
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

    #endregion
    #region temp
    public Sprite[] DamageType;
    public Image HoldRing;
    private int season = 2;
    public RectTransform Minimap;
    public List<TurnQueueItem> TurnQueues = new();
    private bool ActionsVisibled = false;
    private bool SkillsVisibled = false;
    private bool RandomCardVisibled = true;
    private bool TurnQueueVisibled = true;
    public float UISpeed = 20f;
    public GameObject Inflict, Recieve;
    public GameObject Actions;
    public GameObject AttackSkills;
    public GameObject SkillPreview;
    public Preview Preview;
    private Coroutine ActionsSliding;
    private Coroutine AttackSkillsSliding;
    private Coroutine TurnQueueSliding;
    private Coroutine RandomCardsSliding;
    public Transform AttackSkillList;
    public Transform BattlePreviewBoard;
    public List<GameObject> IgnoreClick = new();
    public List<string> canUseAtkSkill = new();
    public Tile[] backupTiles = new Tile[0];
    public Tile[] selectSkillBackupTiles;
    public Transform RandomCards;
    public Transform TurnQueue;
    public Transform QueueList;
    public GameObject TQItem;
    public Tile[] targetsTiles = new Tile[0];
    public string setSkillId;
    public BattleClass battleClass;
    public GameObject ExpectTarget;

    private void Awake()
    {
        ActionsSliding = null;
        AttackSkillsSliding = null;
        AttackSkills.transform.localScale = Vector3.zero;
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
    public void ActionsOn()
    {
        ActionsSliding ??= StartCoroutine(ActionsVisible());
        foreach(Tile t in GameManager.Inst.tiles.SelectedChecker)
            GameManager.Inst.tiles.tileObject[t.POS.y, t.POS.x].transform.Find("Checker").gameObject.SetActive(false);
        GameManager.Inst.tiles.SelectedChecker.Clear();
    }
    public IEnumerator ActionsVisible()
    {
        ActionsVisibled = !ActionsVisibled;

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
                Preview.SP.text = $"SP : {GameManager.Inst.tiles.TurnChar.SP} (-" + DataManager.Datas[attackSkill]["cost"] + ")";
                Preview.Name.text = DataManager.textData[attackSkill][GameManager.langCode];
                Preview.Cost.text = $"SP : " + DataManager.Datas[attackSkill]["cost"];
                Preview.DmgType.color = System.Convert.ToChar(DataManager.Datas[attackSkill]["dmgType"]) == 'p' ? Color.cyan : Color.black;
                Preview.Desc.text = DataManager.textData[$"desc_{attackSkill}"][GameManager.langCode];
                Preview.Power.text = 
                    DataManager.Datas[attackSkill].ContainsKey("dmg") || System.Convert.ToInt32(DataManager.Datas[attackSkill]["dmg"]) == 0? 
                    DataManager.ToInt(DataManager.Datas[attackSkill]["attackCount"]) > 1? 
                    DataManager.Datas[attackSkill]["dmg"].ToString() + " x " + DataManager.Datas[attackSkill]["attackCount"].ToString() :
                    DataManager.Datas[attackSkill]["dmg"].ToString() : "-";
            }

            AttackSkillList.transform.Find(attackSkill).gameObject.SetActive(true);
        }

        if(ActionsVisibled)
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
    public IEnumerator ActionButtonUpDown(RectTransform rect, bool down)
    {
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
    public void AttackOn()
    {
        AttackSkillsSliding ??= StartCoroutine(AttackListVisible());
    }
    public IEnumerator AttackListVisible()
    {
        SkillsVisibled = !SkillsVisibled;
        RectTransform rect = AttackSkills.GetComponent<RectTransform>();
        SkillPreview.SetActive(false);
        if (SkillsVisibled)
        {
            ActionsOn();
            IgnoreClick.Add(AttackSkills);
            while(rect.localScale.x < 0.98f)
            {
                rect.localScale = Vector3.Lerp(rect.localScale, new Vector3(4, 4, 4), Time.deltaTime * UISpeed);
                yield return null;
            }
            rect.localScale = Vector3.one;
            GameManager.Inst.tiles.TurnAct = Act.Attack;
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
    public void SetCasterPreviewUnit()
    {
        Character caster = Preview.CasterPreview.data;
        GameObject cObj = Instantiate(Resources.Load<GameObject>($"Prefab/Standing_Character/{caster.id}"), Preview.CasterPreview.Unit.Find("Portrait"));
        Destroy(Preview.CasterPreview.Unit.Find("Portrait").Find("Base").gameObject);
        Preview.CasterPreview.UnitName.text = DataManager.textData[caster.id][GameManager.langCode];
        cObj.name = "Base";
        cObj.transform.SetSiblingIndex(1);
        SetPreview(Preview.attackSkill);
    }
    public void SetTargetPreviewUnit()
    {
        if(Preview.TargetPreview.data == null)
            return;
        Character target = Preview.TargetPreview.data;
        GameObject tObj = Instantiate(Resources.Load<GameObject>($"Prefab/Standing_Character/{target.id}"), Preview.TargetPreview.Unit.Find("Portrait"));
        Destroy(Preview.TargetPreview.Unit.Find("Portrait").Find("Base").gameObject);
        Preview.TargetPreview.UnitName.text = DataManager.textData[target.id][GameManager.langCode];
        tObj.name = "Base";
        tObj.transform.SetSiblingIndex(1);
        SetPreview(Preview.attackSkill);
    }
    public void SetPreview(string skillId)
    {
        Character caster = Preview.CasterPreview.data;
        Character target = Preview.TargetPreview.data;
        CharacterData casterData = Preview.CasterPreview.data.realData;
        CharacterData targetData = Preview.TargetPreview.data?.realData;
        Dictionary<string, object> skillData = DataManager.Datas[skillId];
        
        caster.SetPrevHPBar((float)casterData.CurHP);
        foreach(Tile tile in targetsTiles)
        {
            if(tile.tileObject != null)
            {
                tile.tileObject?.SetPrevHPBar((float)tile.tileObject?.realData.CurHP);
            }
        }

        foreach(Transform child in BattlePreviewBoard)
        {
            Destroy(child.gameObject);
        }

        if(targetData == null)
        {
            Preview.TargetPreview.Unit.gameObject.SetActive(false);
            foreach(Tile t in targetsTiles)
                if(t.tileObject != null)
                {
                    BattleClass battleClass = new(caster, (Character)t.tileObject, setSkillId);            
                    ((Character)t.tileObject).SetPrevHPBar(Mathf.FloorToInt(t.tileObject.realData.CurHP) - battleClass.totalDmg);
                }
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

                Preview.CasterPreview.FullHP.text = $"{casterData.HP}";
                Preview.TargetPreview.FullHP.text = $"{targetData.HP}";

                Preview.CasterPreview.HP.text = $"{casterRemainHP}";
                Preview.TargetPreview.HP.text = $"{targetRemainHP}";

                Preview.CasterPreview.PrevHealthBar.fillAmount = casterRemainHP / casterData.HP;
                caster.SetPrevHPBar(casterRemainHP);
                Preview.TargetPreview.PrevHealthBar.fillAmount = targetRemainHP / targetData.HP;
                target.SetPrevHPBar(targetRemainHP);
                
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
        Character caster = Preview.CasterPreview.data;
        Character target = Preview.TargetPreview.data;
        int distance = Mathf.Abs(caster.tempPOS.x - target.POS.x) + Mathf.Abs(caster.tempPOS.y - target.POS.y);
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
        if(string.IsNullOrEmpty(setSkillId) && string.IsNullOrEmpty(id)) return;
        setSkillId = string.IsNullOrEmpty(id) ? setSkillId : id;
        Dictionary<string, object> skillData = DataManager.Datas[setSkillId];
        Character caster = Preview.CasterPreview.data;
        int[] r = DataManager.ToIntArr(skillData["range"]);
        targetsTiles = GameManager.Inst.tiles.CalcAttackableTile(caster, caster.tempPOS, r[0], r[1] + DataManager.ToInt(DataManager.Datas[caster.weapon]["rangeBonus"]));
        GameManager.Inst.tiles.TurnChar.AttackableTiles = targetsTiles;
        CharacterData casterData = caster.realData;

        
        caster.SetHPBar(casterData.CurHP);
        foreach(Tile t in targetsTiles)
            if(t.tileObject != null)
            {
                BattleClass battleClass = new(caster, (Character)t.tileObject, setSkillId);

                float hpRatio = Mathf.Clamp((float)(Mathf.FloorToInt(t.tileObject.realData.CurHP) - battleClass.totalDmg) / Mathf.FloorToInt(t.tileObject.realData.HP), 0.0f, 1.0f);
                SpriteRenderer hpBar = t.tileObject.PrevHealthBar;
                hpBar.transform.localScale = new Vector2(hpRatio, 1.0f);
                if(hpRatio > 0.25f)
                    hpBar.color = Color.white;
                else
                    hpBar.color = Color.red;
            }
        
        if(string.IsNullOrEmpty(id))
            return;
        foreach(Tile tile in GameManager.Inst.tiles.Checker)
        {
            tile.obj.transform.Find("Checker").gameObject.SetActive(false);
        }
        GameManager.Inst.tiles.Checker.Clear();
        Preview.attackSkill = setSkillId;

        SetCasterPreviewUnit();
        SetTargetPreviewUnit();

        string setSkill = setSkillId;
        string[] NameDesc = DataManager.DescConvert(setSkill);
        Preview.SP.text = $"SP : {GameManager.Inst.tiles.TurnChar.SP} (-" + skillData["cost"] + ")";
        Preview.Name.text = NameDesc[0];
        Preview.Cost.text = $"SP : " + skillData["cost"];
        Preview.Power.text = 
                skillData.ContainsKey("dmg") || System.Convert.ToInt32(skillData["dmg"]) == 0? 
                DataManager.ToInt(skillData["attackCount"]) > 1? 
                skillData["dmg"].ToString() + " × " + skillData["attackCount"].ToString() :
                skillData["dmg"].ToString() : "-";
        Preview.DmgType.sprite = System.Convert.ToChar(skillData["dmgType"]) == 'p' ? DamageType[0] : DamageType[1];
        Preview.Desc.text = NameDesc[1];
    }
    public void CheckerClear()
    {
        GameManager.Inst.tiles.Checker.Clear();
    }
    public void CardsSwitching()
    {
        RandomCardsSliding ??= StartCoroutine(CdSwitching());
    }
    public IEnumerator CdSwitching()
    {
        RectTransform bg = RandomCards.Find("Background").GetComponent<RectTransform>();
        if(RandomCardVisibled)
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
            while(bg.anchoredPosition.y < 200)
            {
                bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(0, 220), Time.deltaTime * UISpeed);
                yield return null;
            }
            bg.anchoredPosition = new Vector2(0, 200);
        }
        RandomCardVisibled = !RandomCardVisibled;
        TextMeshProUGUI tmpUGUI = RandomCards.Find("Background").Find("CardsSwitch").GetComponentInChildren<TextMeshProUGUI>();
        tmpUGUI.text = tmpUGUI.text == "∧" ? "∨" : "∧";
        RandomCardsSliding = null;
    }
    public void TurnQueueSwitching()
    {
        TurnQueueSliding ??= StartCoroutine(TqSwitching());
    }
    public IEnumerator TqSwitching()
    {
        RectTransform bg = TurnQueue.Find("Background").GetComponent<RectTransform>();
        if(TurnQueueVisibled)
        {
            while(bg.anchoredPosition.x < 0)
            {
                bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(20, 0), Time.deltaTime * UISpeed);
                yield return null;
            }
            bg.anchoredPosition = new Vector2(0, 0);
        }
        else
        {
            while(bg.anchoredPosition.x > -128)
            {
                bg.anchoredPosition = Vector2.Lerp(bg.anchoredPosition, new Vector2(-148, 0), Time.deltaTime * UISpeed);
                yield return null;
            }
            bg.anchoredPosition = new Vector2(-128, 0);
        }
        TurnQueueVisibled = !TurnQueueVisibled;
        TextMeshProUGUI tmpUGUI = TurnQueue.Find("Background").Find("TurnQueueSwitch").GetComponentInChildren<TextMeshProUGUI>();
        tmpUGUI.text = tmpUGUI.text == "〉" ? "〈" : "〉";
        TurnQueueSliding = null;
    }
    public void SyncTurnQueue()
    {
        Tiles tiles = GameManager.Inst.tiles;
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
                float hpRatio = Mathf.Clamp((t.tileObject.realData.CurHP) / t.tileObject.realData.HP, 0.0f, 1.0f);
                SpriteRenderer hpBar = t.tileObject.PrevHealthBar;
                hpBar.transform.localScale = new Vector2(hpRatio, 1.0f);
                if(hpRatio < 0.25f)
                {
                    hpBar.color = Color.red;
                }
                else
                {
                    hpBar.color = Color.white;
                }
            }
        CursorManager.CursorChange(CursorMode.Normal);
        GameManager.Inst.tiles.TurnAct = Act.Operate;
        foreach(Tile t in GameManager.Inst.tiles.TurnChar.AttackRange)
        {
            GameManager.Inst.tiles.SetTileState(t, TileState.RedTile);
        }
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
    #endregion
    public void UnitDataUISet(Character character)
    {
        if(character.IsFocus)
            character.FocusOn();
        UnitDataUISet(character.realData);
    }
    public void UnitDataUISet(CharacterData data)
    {
        UnitImage.sprite = Resources.Load<Sprite>($"Images/Character/{data.id}/{data.id}_Original");

        #region group
        Group.sprite = Resources.Load<Sprite>($"Images/Character/Background/Symbol/{data.group}");
        #endregion
        #region detail
        Name.text = string.Empty;
        Class.text = string.Empty;
        Level.text = string.Empty;
        Level.text = $"<color=#D7D7D7><size=45>LV.</size></color>{data.Lv}";
        string desc = DataManager.textData.ContainsKey($"desc_{data.id}") ? DataManager.textData["desc_" + data.id][GameManager.langCode] : "";
        if(desc.IndexOf($"<s{season}>") > - 1)
            desc = desc.Split(new string[]{$"<s{season}>", $"</s{season}>"}, System.StringSplitOptions.None)[1]; 
        Name.text += $"{DataManager.textData[data.id][GameManager.langCode]} <color=#A6A6A6><size=28>{desc}</size></color>".Replace("\r", "");
        EXP.text = $"{data.EXP} / {data.MaxExp}";
        Class.text += $"{DataManager.textData[data.characterClass][GameManager.langCode]} <color=#A6A6A6><size=16>({data.ClassEXP}%)</size></color>".Replace("\r", "");
        #endregion

        #region status
        HP.text = $"{data.HP}";
        SP.text = $"{data.SP}";
        ATK.text = $"{data.Atk}";
        RYN.text = $"{data.Ryn}";
        SPD.text = $"{data.Spd}";
        DEX.text = $"{data.Dex}";
        DEF.text = $"{data.Def}";
        RES.text = $"{data.Res}";
        DPN.text = $"{data.Dpn}";
        RPN.text = $"{data.Rpn}";
        STR.text = $"{data.Str}";
        HalfSpirit.text = string.IsNullOrEmpty(data.HalfSpirit) ? "-" : $"{data.HalfSpirit}";
        #endregion
        
        #region items
        #endregion
        
        #region skills
        #endregion
    }
    public void UnitDataVisible(bool visible)
    {
        UnitData.gameObject.SetActive(visible);
        GameManager.Inst.InputBlock = visible;
        if(visible)
        {
            CursorManager.CursorChange(CursorMode.Normal);
            
        }
        else if(GameManager.Inst.tiles.TurnAct == Act.SelectSkill)
            CursorManager.CursorChange(CursorMode.Attack);
    }
    public void BattleRun()
    {
        GameManager.Inst.tiles.BattleRun(battleClass);
        AttackOn();
    }
    public void SetExpectTarget(Character caster, string skillId, TileObject _target)
    {
        if(_target == null || _target is not Character)
        {
            ExpectTarget.gameObject.SetActive(false);
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
        Tile assumedTile = caster.FindVTileToAttack(GameManager.Inst.tiles.tileDatas[target.tempPOS.y, target.tempPOS.x], r[0] + rB, r[0]);
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
}
