using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using Newtonsoft.Json;
using System.Linq;

public enum ToolType
{
    Brush,
    FilledBox,
    BorderBox,
    Fill,
    Erase,
    Save
}
public class HistoryItem
{
    public HistoryItem(Tile tile, Sprite prevSprite, Sprite sprite, bool prevV, bool RandomVariation, bool sepBool)
    {
        this.prevSprite = prevSprite;
        this.prevRandomVariation = prevV;
        this.sepBool = sepBool;
        this.pos = (Vector2Int)tile.POS;
        this.sprite = sprite;
        this.RandomVariation = RandomVariation;
    }
    public bool sepBool;
    public Vector2Int pos;
    public Sprite sprite;
    public bool RandomVariation;

    public Sprite prevSprite;
    public bool prevRandomVariation;
}
public class MapData
{
    public string id;
    public Vector2Int size;
    public List<List<MapTileData>> mapTileDatas;
}
public class MapTileData
{
    public int x, y;
    public string tileName;
    public bool enableVariation;
}
public class CreatedBaseMap : MonoBehaviour
{
    public Texture2D[] cursors;
    public Sprite[] icons;
    bool sepBool = false;
    Sprite[] AllTile;
    public GameObject PaletteItemPrefab;
    public RectTransform Palette;
    public RectTransform TilePalette;
    public Camera cam;
    public Tile[,] tileDatas;
    public GameObject[,] tileObject;
    public TMP_InputField WidthField, HeightField, AnchoredX, AnchoredY;
    public Vector2Int mapSize = Vector2Int.zero;
    public GameObject TilePrefab;
    public Sprite BlankTile;
    public GameObject MapSizeModify;
    private int CAMMIN = 100;
    private int CAMMAX = 5;
    private bool cameraMoveable = false;
    private Vector3 mousePos;
    public TMP_InputField Searching;
    public Sprite SelSprite = null;
    public List<HistoryItem> History = new();
    public List<HistoryItem> Records = new();
    private bool EnableVariation = true;
    private bool AutoConnect = true;
    public TextMeshProUGUI pos;
    public Transform SelectedTile;
    public ToolType SelectedTool;
    private Vector2Int[] MakeBoxTool = new Vector2Int[2];
    private Vector2Int[] SavePos = new Vector2Int[2];
    public GameObject SaveMap;
    public GameObject ConvertMap;
    public GameObject LoadMap;
    public Image icon;
    public MainAction mainInput;

    private float rollbackTimer = 0.0f;
    private float rollbackAcc = 0.0f;
    private bool canRollback = false; 

    private float reRollbackTimer = 0.0f;
    private float reRollbackAcc = 0.0f;
    private bool canReRollback = false; 

    private void Awake()
    {
        mainInput = new MainAction();
        mapSize = new Vector2Int(50, 50);
        cam = Camera.main;
        MapDefine();
    }
    private void Update()
    {
        rollbackTimer += Time.deltaTime + rollbackAcc;
        if(rollbackTimer >= 0.1f && canRollback)
        {
            rollbackAcc = Mathf.Clamp(rollbackAcc + Time.deltaTime * 0.2f, 0, 0.09f);
            rollbackTimer = 0.0f;
            RollBack();
        }

        reRollbackTimer += Time.deltaTime + reRollbackAcc;
        if(reRollbackTimer >= 0.1f && canReRollback)
        {
            reRollbackAcc = Mathf.Clamp(reRollbackAcc + Time.deltaTime * 0.2f, 0, 0.09f);
            reRollbackTimer = 0.0f;
            ReRollBack();
        }
        
        SelectedTile.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 0.5f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y - 0.5f);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray.origin, ray.direction * 1000.0f, out RaycastHit hit))
        {
            if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector2Int vec = new (System.Convert.ToInt32(hit.collider.name.Split('|')[0]), System.Convert.ToInt32(hit.collider.name.Split('|')[1]));
                switch(SelectedTool)
                {
                    case ToolType.BorderBox:
                        if(MakeBoxTool[1] != new Vector2Int(-1, -1))
                        {
                            MakeBoxTool[0] = vec;
                            MakeBoxTool[1] = new(-1, -1);
                        }
                        else
                        {
                            MakeBoxTool[1] = vec;
                            BorderBox();
                        }
                    break;
                    case ToolType.FilledBox:
                        if(MakeBoxTool[1] != new Vector2Int(-1, -1))
                        {
                            MakeBoxTool[0] = vec;
                            MakeBoxTool[1] = new(-1, -1);
                        }
                        else
                        {
                            MakeBoxTool[1] = vec;
                            FilledBox();
                        }
                    break;
                    case ToolType.Fill:
                        FillTile(vec);
                    break;
                    case ToolType.Save:
                        if(SavePos[1] != new Vector2Int(-1, -1))
                        {
                            SavePos[0] = vec;
                            SavePos[1] = new(-1, -1);
                        }
                        else
                        {
                            SavePos[1] = vec;
                            MapSave();
                        }
                    break;
                }
            }
            if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                Vector2Int vec = new (System.Convert.ToInt32(hit.collider.name.Split('|')[0]), System.Convert.ToInt32(hit.collider.name.Split('|')[1]));
                switch(SelectedTool)
                {
                    case ToolType.Brush:
                        BrushTile(vec);
                    break;
                    case ToolType.Erase:
                        BrushTile(vec);
                    break;
                }
            }
            pos.text = $"({hit.collider.name.Replace("|",", ")})";
        }

        if (Input.GetMouseButtonDown(1))
        {
            cameraMoveable= true;
            mousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            cameraMoveable= false;
        }

        Transform tr = cam.transform;
         if (Input.mouseScrollDelta.y != 0)
        {
            float size = cam.orthographicSize;
            Vector3 mouse = cam.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - Input.mouseScrollDelta.y, CAMMAX, CAMMIN);
            size = Camera.main.orthographicSize;
            tr.position += mouse - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            tr.position = new(
                Mathf.Clamp(tr.position.x, -(CAMMIN - size) * (12.5f / 7), (CAMMIN - size) * (12.5f / 7)),
                Mathf.Clamp(tr.position.y, -(CAMMIN - size), CAMMIN - size), -10);
        }
        if (cameraMoveable)
        {
            float size = cam.orthographicSize;
            Vector2 vector2 = cam.ScreenToViewportPoint(mousePos - Input.mousePosition);
            tr.Translate((CAMMIN - cam.orthographicSize) * Time.deltaTime * vector2);
            tr.position = new(Mathf.Clamp(tr.position.x, -(CAMMIN - size) * (12.5f / 7), (CAMMIN - size) * (12.5f / 7)), Mathf.Clamp(tr.position.y, -(CAMMIN - size), (CAMMIN - size)), -CAMMIN);
        }
    }
    private void OnEnable()
    {
        mainInput.Enable();
        mainInput.CreateMapControl.Brush.performed += (e) => {SetCursor(0); SelectedTool = ToolType.Brush;};
        mainInput.CreateMapControl.Fill.performed += (e) => {SetCursor(1); SelectedTool = ToolType.Fill;};
        mainInput.CreateMapControl.BorderBox.performed += (e) => {SetCursor(2);MakeBoxTool[0] = new Vector2Int(-1, -1);MakeBoxTool[1] = Vector2Int.zero;SelectedTool = ToolType.BorderBox;};
        mainInput.CreateMapControl.FilledBox.performed += (e) => {SetCursor(3);MakeBoxTool[0] = new Vector2Int(-1, -1);MakeBoxTool[1] = Vector2Int.zero;SelectedTool = ToolType.FilledBox;};
        mainInput.CreateMapControl.RollBack.performed += (e) => {RollBack();};
        mainInput.CreateMapControl.RollBackHold.performed += (e) => {canRollback = true; rollbackTimer = 0.0f; rollbackAcc = 0.0f;};
        mainInput.CreateMapControl.RollBackHold.canceled += (e) => {canRollback = false;};
        mainInput.CreateMapControl.ReRollBack.performed += (e) => {ReRollBack();};
        mainInput.CreateMapControl.ReRollBackHold.performed += (e) => {canReRollback = true; reRollbackTimer = 0.0f; reRollbackAcc = 0.0f;};
        mainInput.CreateMapControl.ReRollBackHold.canceled += (e) => {canReRollback = false;};
        mainInput.CreateMapControl.Save.performed += (e) => {SetCursor(4);SelectedTool = ToolType.Save;};
        mainInput.CreateMapControl.Erase.performed += (e) => {SelectedTool = ToolType.Erase;SetCursor(5);SelSprite = BlankTile;SelectedTile.GetComponent<SpriteRenderer>().sprite = null;};
        mainInput.CreateMapControl.LoadMap.performed += (e) => {ShowLoadMap();};
    }
    public void SetCursor(int i)
    {
        icon.sprite = icons[i];
        Cursor.SetCursor(cursors[i], Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
    }
    public void SwitchVariation()
    {
        EnableVariation = !EnableVariation;
    }
    public void SwitchAutoConnect()
    {
        AutoConnect = !AutoConnect;
    }
    public void BrushTile(Vector2Int POS)
    {
        Tile t = tileDatas[POS.y, POS.x];
        GameObject obj = tileObject[POS.y, POS.x];
        if(t.tileSprite.sprite == SelSprite && obj.transform.Find("EnableVariation").gameObject.activeSelf == EnableVariation) return;
        SetTile(POS);
        sepBool = !sepBool;
    }
    public void FillTile(Vector2Int POS)
    {
        List<Vector2Int> openList = new()
        {
            POS
        };
        List<Vector2Int> closeList = new();
        
        Vector2Int sel;
        int x = 2500;
        do
        {
            x--;
            sel = openList[^1];
            closeList.Add(sel);
            openList.RemoveAt(openList.Count - 1);
            for(int i = Mathf.Max(0, sel.x - 1); i <= Mathf.Min(mapSize.x - 1, sel.x + 1); i++)
            {
                for(int j = Mathf.Max(0, sel.y - 1); j <= Mathf.Min(mapSize.y - 1, sel.y + 1); j++)
                {
                    Vector2Int vec = new(i, j);
                    if(Mathf.Abs(i - sel.x) + Mathf.Abs(j - sel.y) > 1 || closeList.Contains(vec) || openList.Contains(vec))
                        continue;
                    if(tileDatas[vec.y, vec.x].tileSprite.sprite == tileDatas[sel.y, sel.x].tileSprite.sprite)
                    {
                        openList.Add(vec);
                    }
                }
            }
        }while(openList.Count > 0 && x > 0);

        foreach(Vector2Int v in closeList)
        {
            SetTile(v);
        }
        
        sepBool = !sepBool;
    }
    public void SetTile(Vector2Int POS)
    {
        Tile t = tileDatas[POS.y, POS.x];
        GameObject tile = tileObject[POS.y, POS.x];
        if(t.tileSprite.sprite == SelSprite && tile.transform.Find("EnableVariation").gameObject.activeSelf == (SelSprite != BlankTile && EnableVariation)) return;
        if(History.Count > 0 && History[^1].prevSprite == History[^1].sprite && History[^1].prevRandomVariation == History[^1].RandomVariation) return;

        if(History.Count > History.Capacity)
            History.RemoveAt(0);

        Sprite prevS = t.tileSprite.sprite;
        t.tileSprite.sprite = SelSprite;
        bool prevB = tile.transform.Find("EnableVariation").gameObject.activeSelf;

        tile.transform.Find("EnableVariation").gameObject.SetActive(SelSprite != BlankTile && EnableVariation);
        History.Add(new HistoryItem(t, prevS, t.tileSprite.sprite, prevB, SelSprite != BlankTile && tile.transform.Find("EnableVariation").gameObject.activeSelf, sepBool));
        Records.Clear();
    }
    public void FilledBox()
    {
        for(int i = MakeBoxTool[0].x; i <= MakeBoxTool[1].x; i++)
        {
            for(int j = MakeBoxTool[0].y; j <= MakeBoxTool[1].y; j++)
            {
                SetTile(new Vector2Int(i, j));
            }
        }   
        sepBool = !sepBool;
    }
    public void BorderBox()
    {
        for(int i = Mathf.Min(MakeBoxTool[0].x, MakeBoxTool[1].x); i <= Mathf.Max(MakeBoxTool[0].x, MakeBoxTool[1].x); i++)
        {
            for(int j = Mathf.Min(MakeBoxTool[0].y, MakeBoxTool[1].y); j <= Mathf.Max(MakeBoxTool[0].y, MakeBoxTool[1].y); j++)
            {
                if(i == MakeBoxTool[0].x || i == MakeBoxTool[1].x || j == MakeBoxTool[0].y || j == MakeBoxTool[1].y)
                {
                    SetTile(new Vector2Int(i, j));
                }
            }
        }
        sepBool = !sepBool;
    }
    public void ShowTilePalette()
    {
        TilePalette.gameObject.SetActive(!TilePalette.gameObject.activeSelf);
    }
    public void MapDefine()
    {
        AllTile = Resources.LoadAll<Sprite>("Images/Sprite/Tilemap");

        foreach(Sprite sprite in AllTile)
        {
            if(sprite.name == "BlankTile")
            {
                SelectedTool = ToolType.Erase;
                SelSprite = sprite;
                SelectedTile.GetComponent<SpriteRenderer>().sprite = null;
                SetCursor(5);
            }
            GameObject item = Instantiate(PaletteItemPrefab, Palette);
            item.GetComponent<Image>().sprite = sprite;
            item.name = sprite.name;
            item.GetComponent<Button>().onClick.AddListener(() => {SelectPaletteItem(item);});
        }

        tileDatas = new Tile[mapSize.y, mapSize.x];
        tileObject = new GameObject[mapSize.y, mapSize.x];

        Vector2 boxSize = new(mapSize.x - 1, mapSize.y - 1);
        for(int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                GameObject tile = Instantiate(TilePrefab, transform);
                tileObject[i, j] = tile;
                tileDatas[i, j] = new Tile()
                {
                    POS = new Vector3Int(j, i, 0),
                    obj = tile,
                    tileSprite = tile.transform.Find("Sprite").GetComponent<SpriteRenderer>(),
                    rangeSprite = tile.transform.Find("Range").GetComponent<SpriteRenderer>(),
                    stateSprite = tile.transform.Find("TileState").GetComponent<SpriteRenderer>(),
                    selectedUnitSprite = tile.transform.Find("SelectedUnit").GetComponent<SpriteRenderer>()
                };
                tile.transform.position = new Vector3(-boxSize.x / 2 + j, boxSize.y / 2 - i, 0.51f - (0.51f * tileDatas[i, j].POS.z));
                tile.name = $"{j}|{i}";
            }
        }

        MapSizeModify.SetActive(false);
    }
    public void SelectPaletteItem(GameObject obj)
    {
        MakeBoxTool[0] = new Vector2Int(-1, -1);
        MakeBoxTool[1] = Vector2Int.zero;
        SelSprite = obj.GetComponent<Image>().sprite;
        if(SelSprite.name == "BlankTile")
        {
            SelectedTool = ToolType.Erase;
            SetCursor(5);
            SelectedTile.GetComponent<SpriteRenderer>().sprite = null;
        }
        else
        {
            if(SelectedTool == ToolType.Erase) SetCursor(0);
            SelectedTool = SelectedTool == ToolType.Erase ? ToolType.Brush : SelectedTool;
            SelectedTile.GetComponent<SpriteRenderer>().sprite = SelSprite;   
        }
    }
    public void SearchTile()
    {
        foreach(RectTransform tr in Palette)
        {
            if(tr.name.ToLower().IndexOf(Searching.text.ToLower()) == -1)
                tr.gameObject.SetActive(false);
            else
                tr.gameObject.SetActive(true);
        }
    }
    public void RollBack()
    {
        if(History.Count > 0) sepBool = !sepBool;
        for(int i = History.Count - 1; i >= 0; i--)
        {
            Records.Add(History[i]);
            tileDatas[History[i].pos.y, History[i].pos.x].tileSprite.sprite = History[i].prevSprite;
            tileObject[History[i].pos.y, History[i].pos.x].transform.Find("EnableVariation").gameObject.SetActive(History[i].prevRandomVariation);
            if(i == 0 || History[i].sepBool != History[i-1].sepBool)
            {
                History.RemoveAt(i);
                return;
            }
            else
            {
                History.RemoveAt(i);
            }
        }
    }
    public void ReRollBack()
    {
        if(Records.Count > 0) sepBool = !sepBool;
        for(int i = Records.Count - 1; i >= 0; i--)
        {
            tileDatas[Records[i].pos.y, Records[i].pos.x].tileSprite.sprite = Records[i].sprite;
            tileObject[Records[i].pos.y, Records[i].pos.x].transform.Find("EnableVariation").gameObject.SetActive(Records[i].RandomVariation);
            if(i == 0 || Records[i].sepBool != Records[i-1].sepBool)
            {
                History.Add(Records[i]);
                Records.RemoveAt(i);
                return;
            }
            else
            {
                History.Add(Records[i]);
                Records.RemoveAt(i);
            }
        }
    }
    public void MapSave()
    {
        SaveMap.SetActive(true);
    }
    public void MakeJson(GameObject obj)
    {
        string id = obj.GetComponent<TMP_InputField>().text;
        string json = ToJson(id);
        ConvertMap.SetActive(true);
        ConvertMap.GetComponentInChildren<TextMeshProUGUI>().text = json;
        SaveMap.SetActive(false);
    }
    public string ToJson(string id)
    {
        int MinX, MaxX, MinY, MaxY;
        MinX = Mathf.Min(SavePos[0].x, SavePos[1].x);
        MinY = Mathf.Min(SavePos[0].y, SavePos[1].y);
        MaxX = Mathf.Max(SavePos[0].x, SavePos[1].x);
        MaxY = Mathf.Max(SavePos[0].y, SavePos[1].y);

        MapData mapData = new()
        {
            mapTileDatas = new(),
            id = id
        };

        for(int j = MinY; j <= MaxY; j++)
        {
            List<MapTileData> maptileDatas = new();
            MapTileData _mapTileData = new();
            for(int i = MinX; i <= MaxX; i++)
            {
                MapTileData mapTileData = new()
                {
                    x = i - MinX,
                    y = j - MinY,
                    tileName = tileDatas[j, i].tileSprite.sprite.name,
                    enableVariation = tileObject[j, i].transform.Find("EnableVariation").gameObject.activeSelf
                };
                if(_mapTileData.tileName == mapTileData.tileName && _mapTileData.enableVariation == mapTileData.enableVariation && i != MaxX)
                    continue;
                maptileDatas.Add(mapTileData);
                _mapTileData = mapTileData;
            }
            mapData.mapTileDatas.Add(maptileDatas);
            mapData.size = new Vector2Int(MaxX - MinX + 1, MaxY - MinY + 1);
        }
        return JsonConvert.SerializeObject(mapData, Formatting.None, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore});
    }
    [STAThread]
    public void CopyData(GameObject obj)
    {
        GUIUtility.systemCopyBuffer = obj.GetComponent<TextMeshProUGUI>().text;
        ConvertMap.SetActive(false);
    }
    public void ShowLoadMap()
    {
        LoadMap.SetActive(!LoadMap.activeSelf);
    }
    public void LoadData(GameObject obj)
    {
        string json = obj.GetComponent<TMP_InputField>().text;
        MapData mapData = JsonConvert.DeserializeObject<MapData>(json);

        for(int i = mapData.mapTileDatas.Count - 1; i >= 0; i--)
        {
            for(int j = 0; j < mapData.mapTileDatas[i].Count; j++)
            {
                SelSprite = Resources.Load<Sprite>($"Images/Sprite/Tilemap/{mapData.mapTileDatas[i][j].tileName}");
                EnableVariation = mapData.mapTileDatas[i][j].enableVariation;
                if(j == mapData.mapTileDatas[i].Count - 1)
                {
                    Tile t = tileDatas[mapData.mapTileDatas[i][j].y, mapData.mapTileDatas[i][j].x];
                    SetTile((Vector2Int)t.POS);
                }
                else
                    for(int x = mapData.mapTileDatas[i][j].x; x < mapData.mapTileDatas[i][j + 1].x; x++)
                    {
                        Tile t = tileDatas[mapData.mapTileDatas[i][j].y, x];
                        SetTile((Vector2Int)t.POS);
                    }
            }
        }
        obj.GetComponent<TMP_InputField>().text = "";
        LoadMap.SetActive(false);
    }
}