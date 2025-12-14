using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ControlType
{
    WithMouse,
    WithoutMouse,
}
public class InputManager : MonoBehaviour
{
    MainAction mainAction;
    Tiles tiles;
    Vector3 movement;
    private float rot;
    public float camDistance = 15;
    private float MoveHoldDur = 0f;
    private float MoveHoldCycle = 0.5f;
    private bool MoveOn = false;
    private readonly float ZoomSpeed = 0.2f;
    private readonly float SkewSpeed = 1f;
    private bool RPress = false;
    private bool MPress = false;
    private Vector2 MousePos = Vector2.zero;
    private Vector2 curMousePos = Vector2.zero;
    private Vector2 preMousePos = Vector2.zero;
    public Transform camTr;
    Vector2 StartMouseMovementForRot = Vector2.zero;
    Vector2 MouseMovementForRot = Vector2.zero;
    private float holdTimer = 0.0f;
    public float MinholdTimer = 0.75f;
    private readonly float MaxholdTimer = 1.0f;
    private TileObject dataObject;
    private bool HoldSelect = false;
    public TMPro.TextMeshProUGUI fps;
    public SpriteRenderer FocusSprite;
    public SpriteRenderer MouseFocusSprite;
    public Tile FocusingTile;
    private bool IsMouseOverOnUI = false;
    public GraphicRaycaster gr;

    private void Awake()
    {
        tiles = FindFirstObjectByType<Tiles>();
        mainAction = new MainAction();
        FocusSprite = tiles.Focus.GetComponent<SpriteRenderer>();
        MouseFocusSprite = tiles.MouseFocus.GetComponent<SpriteRenderer>();
        camTr = Camera.main.transform;
        gr = GameManager.Inst.uiCon.GetComponent<GraphicRaycaster>();
    }
    private void Update()
    {
        fps.text = $"{1.0f / Time.deltaTime}";
    }
    private void FixedUpdate()
    {
        PointerEventData ped = new(null)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new();
        gr.Raycast(ped, results);
        IsMouseOverOnUI = results.Count > 0;

        if(GameManager.Inst.InputBlock)
            return;
        Move();
        Zoom();
        Skew();
        RotationFromMouse();
        UnitDataHold();
        if(GameManager.Inst.ControlType == ControlType.WithMouse)
        {
            FocusSprite.enabled = false;
            tiles.MouseFocus.gameObject.SetActive(true);
        }
        else
        {
            FocusSprite.enabled = true;
            tiles.MouseFocus.gameObject.SetActive(false);
        }

        camTr.position = Vector3.Lerp(camTr.position, 
                        new Vector3(tiles.Focus.position.x, tiles.Focus.position.y, tiles.Focus.position.z - camDistance),
                        Time.fixedDeltaTime * 5f);
    }
    private void Move()
    {
        Ray ray = new(tiles.Focus.position, tiles.Focus.forward);
        Physics.Raycast(ray, out RaycastHit hit, 1000);

        if(GameManager.Inst.ControlType == ControlType.WithMouse)
        {
            FocusingTile = ImportTileData();
            if(FocusingTile != null)
            {
                tiles.MouseFocus.localPosition = tiles.TilePos(FocusingTile.POS);
                tiles.FocusPos = (Vector2Int)FocusingTile.POS;
            }
        }

        if(MPress && GameManager.Inst.ControlType == ControlType.WithMouse)
        {
            curMousePos = mainAction.MouseControl.Position.ReadValue<Vector2>();
            if(MousePos != curMousePos)
            {
                Vector3 v = (camDistance - 2.5f) * tiles.sensitive * Time.deltaTime * tiles.cam.ScreenToViewportPoint(MousePos - curMousePos);
                movement = v;
            }
        }
        else
        {
            if(MoveHoldDur >= MoveHoldCycle)
            {
                movement = mainAction.KeyboardControl.Move.ReadValue<Vector2>();
                MoveHoldDur = 0f;
            }
             movement = movement.x != 0 ? new Vector2(Mathf.Sign(movement.x), movement.y) : new Vector2(0, movement.y);
             movement = movement.y != 0 ? new Vector2(movement.x, Mathf.Sign(movement.y)) : new Vector2(movement.x, 0);
             if(hit.collider != null)
             {
                Transform tr = hit.collider.transform;
                tiles.Focus.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, tiles.Focus.localPosition.z);
                if(GameManager.Inst.ControlType == ControlType.WithoutMouse)
                {
                    Vector2Int v = new Vector2Int(System.Convert.ToInt32(tr.name.Split('|')[0]), System.Convert.ToInt32(tr.name.Split('|')[1]));
                    tiles.FocusPos = v;
                }
             }
        }
        if(MoveOn)
        {
            MoveHoldDur += Time.fixedDeltaTime;
            MoveHoldCycle = Mathf.Max(MoveHoldCycle - Time.fixedDeltaTime, 0.05f);
        }
        
        switch (Mathf.RoundToInt(tiles.transform.eulerAngles.z))
        {
            case 0:
                tiles.Focus.localPosition += movement;
                break;
            case 90:
                tiles.Focus.localPosition += new Vector3(movement.y, -movement.x);
                break;
            case 180:
                tiles.Focus.localPosition += new Vector3(-movement.x, -movement.y);
                break;
            case 270:
                tiles.Focus.localPosition += new Vector3(-movement.y, movement.x);
                break;
        }

        tiles.Focus.localPosition = 
        new Vector3(
            Mathf.Clamp(tiles.Focus.localPosition.x, tiles.tileObject[0, 0].transform.localPosition.x, tiles.tileObject[0, tiles.tileObject.GetLength(1) - 1].transform.localPosition.x),
            Mathf.Clamp(tiles.Focus.localPosition.y, tiles.tileObject[tiles.tileObject.GetLength(0) - 1, 0].transform.localPosition.y, tiles.tileObject[0, 0].transform.localPosition.y),
            -0.1f
            );
            
        movement = Vector2.zero;
        preMousePos = curMousePos;
    }
    private void Rotation(InputAction.CallbackContext context)
    {
        if(GameManager.Inst.InputBlock)
            return;
        switch(context.ReadValue<Vector2>())
        {
            case Vector2 v when v.Equals(Vector2.left):
                tiles.transform.eulerAngles += new Vector3(0, 0, 90f);
                break;
            case Vector2 v when v.Equals(Vector2.right): 
                tiles.transform.eulerAngles -= new Vector3(0, 0, 90f);
                break;
        }
        tiles.Focus.transform.localPosition = tiles.tileObject[tiles.FocusPos.y, tiles.FocusPos.x].transform.localPosition; 
        TileObject[] objs = FindObjectsByType<TileObject>(FindObjectsSortMode.None);
        
        foreach(TileObject obj in objs)
        {
            Transform unit = obj.transform.Find("Unit");
            Transform hpBar = obj.transform.Find("HealthBar");

            switch(Mathf.RoundToInt(tiles.transform.eulerAngles.z)/90)
            {
                case 0:
                    unit.localPosition = new Vector3(0, -0.3f, 0.2f);
                    unit.localEulerAngles = new Vector3(Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, 0, 0);
                    hpBar.localPosition = new Vector3(-0.48f, -0.43f, 0.2f);
                    hpBar.localEulerAngles = Vector3.zero;
                    break;
                case 1:
                    unit.localPosition = new Vector3(-0.3f, 0, 0.2f);
                    unit.localEulerAngles = new Vector3(0, -Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, -90);
                    hpBar.localPosition = new Vector3(-0.43f, 0.48f, 0.2f);
                    hpBar.localEulerAngles = Vector3.back * 90f;
                    break;
                case 2:
                    unit.localPosition = new Vector3(0, 0.3f, 0.2f);
                    unit.localEulerAngles = new Vector3(Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, 0, -180);
                    hpBar.localPosition = new Vector3(0.48f, 0.43f, 0.2f);
                    hpBar.localEulerAngles = Vector3.back * 180f;
                    break;
                case 3:
                    unit.localPosition = new Vector3(0.3f, 0, 0.2f);
                    unit.localEulerAngles = new Vector3(0, -Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, -270);
                    hpBar.localPosition = new Vector3(0.43f, -0.48f, 0.2f);
                    hpBar.localEulerAngles = Vector3.back * 270f;
                    break;
            }
        }
    }
    private void RotationFromMouse()
    {
        if(RPress && GameManager.Inst.ControlType == ControlType.WithMouse)
        {
            MouseMovementForRot = mainAction.MouseControl.Position.ReadValue<Vector2>();
            if(StartMouseMovementForRot.x - MouseMovementForRot.x > Screen.width * 0.5f)
            {
                StartMouseMovementForRot = MouseMovementForRot;
                tiles.transform.eulerAngles += new Vector3(0, 0, 90f);
            }
            else if(StartMouseMovementForRot.x - MouseMovementForRot.x < -Screen.width * 0.5f)
            {
                StartMouseMovementForRot = MouseMovementForRot;
                tiles.transform.eulerAngles -= new Vector3(0, 0, 90f);    
            }
        }
        TileObject[] objs = FindObjectsByType<TileObject>(FindObjectsSortMode.None);
        
        foreach(TileObject obj in objs)
        {
            Transform unit = obj.transform.Find("Unit");
            Transform hpBar = obj.transform.Find("HealthBar");

            switch(Mathf.RoundToInt(tiles.transform.eulerAngles.z)/90)
            {
                case 0:
                    unit.localPosition = new Vector3(0, -0.3f, 0.2f);
                    unit.localEulerAngles = new Vector3(Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, 0, 0);
                    hpBar.localPosition = new Vector3(-0.4875f, -0.43f, 0.2f);
                    hpBar.localEulerAngles = Vector3.zero;
                    break;
                case 1:
                    unit.localPosition = new Vector3(-0.3f, 0, 0.2f);
                    unit.localEulerAngles = new Vector3(0, -Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, -90);
                    hpBar.localPosition = new Vector3(-0.43f, 0.4875f, 0.2f);
                    hpBar.localEulerAngles = Vector3.back * 90f;
                    break;
                case 2:
                    unit.localPosition = new Vector3(0, 0.3f, 0.2f);
                    unit.localEulerAngles = new Vector3(Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, 0, -180);
                    hpBar.localPosition = new Vector3(0.4875f, 0.43f, 0.2f);
                    hpBar.localEulerAngles = Vector3.back * 180f;
                    break;
                case 3:
                    unit.localPosition = new Vector3(0.3f, 0, 0.2f);
                    unit.localEulerAngles = new Vector3(0, -Mathf.Sign(tiles.transform.eulerAngles.z - 179) * tiles.transform.eulerAngles.x, -270);
                    hpBar.localPosition = new Vector3(0.43f, -0.4875f, 0.2f);
                    hpBar.localEulerAngles = Vector3.back * 270f;
                    break;
            }
        }
    
    }
    private void Skew()
    {
        Vector2 MouseMovement = Vector2.zero;
        if(RPress && GameManager.Inst.ControlType == ControlType.WithMouse)
        {
            MouseMovement = (MousePos - mainAction.MouseControl.Position.ReadValue<Vector2>()) * 0.005f;
        }
        rot = Mathf.Clamp(rot - (MouseMovement.y + mainAction.KeyboardControl.Skew.ReadValue<Vector2>().y) * SkewSpeed, 10, 60);
        tiles.transform.eulerAngles = new Vector3(Mathf.Clamp(rot, 10, 60), 0, tiles.transform.eulerAngles.z);
}
    private void Zoom()
    {
        switch(mainAction.KeyboardControl.Zoom.ReadValue<Vector2>())
        {
            case Vector2 v when v.Equals(Vector2.up):
                camDistance = Mathf.Clamp(camDistance - ZoomSpeed, 3, 15);
                break;
            case Vector2 v when v.Equals(Vector2.down):
                camDistance = Mathf.Clamp(camDistance + ZoomSpeed, 3, 15);
                break;
        }
    }
    private void UnitDataHold()
    {
        if(RPress)
        {
            tiles.uiCon.SettedUnitFieldPrieview = null;
            holdTimer = 0.0f;
            tiles.uiCon.HoldRing.fillAmount = 0.0f;
            tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
            return;
        }
        if(FocusingTile == null) 
        {
            tiles.uiCon.SettedUnitFieldPrieview = null;
            HoldSelect = false;
            holdTimer = 0.0f;
            tiles.uiCon.HoldRing.fillAmount = 0.0f;
            tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
            return;
        }
        if(FocusingTile.TempTileObject != null && FocusingTile.TempTileObject is Character character)
        {
            holdTimer += Time.fixedDeltaTime;
            if(dataObject == FocusingTile.TempTileObject)
            {
                tiles.uiCon.HoldRing.rectTransform.anchoredPosition = mainAction.MouseControl.Position.ReadValue<Vector2>();
                if(holdTimer >= MaxholdTimer)
                {   
                    tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 1.0f);
                    HoldSelect = true;
                    tiles.uiCon.SettedUnitFieldPrieview = character;
                }
                else if(holdTimer > MinholdTimer)
                {
                    tiles.uiCon.SettedUnitFieldPrieview = null;
                    float val = (holdTimer - MinholdTimer) / (MaxholdTimer - MinholdTimer);
                    tiles.uiCon.HoldRing.fillAmount = val;
                    tiles.uiCon.HoldRing.color = new Color(255, 255, 255, Mathf.Clamp(val, 0, 0.5f));
                }
            }
            else
            {
                tiles.uiCon.SettedUnitFieldPrieview = null;
                HoldSelect = false;
                holdTimer = 0.0f;
                tiles.uiCon.HoldRing.fillAmount = 0.0f;
                tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
            }
        }
        else
        {
            tiles.uiCon.SettedUnitFieldPrieview = null;
            HoldSelect = false;
            holdTimer = 0.0f;
            tiles.uiCon.HoldRing.fillAmount = 0.0f;
            tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
        }
        dataObject = FocusingTile.TempTileObject;
    }
    public void SetTarget()
    {
        if(tiles.TurnChar.group.groupName == "Player" && tiles.TurnAct == Act.SelectSkill)
        {
            tiles.TurnChar.SetTarget(FocusingTile);
        }
    }
    private void FoucsTile(InputAction.CallbackContext context)
    {
        if(IsMouseOverOnUI) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1<<7))
        {
            tiles.Focus.localPosition = new Vector3(hit.transform.localPosition.x, hit.transform.localPosition.y, -0.1f);
        }
    }
    private void OnEnable() 
    {
        mainAction.KeyboardControl.Enable();
        mainAction.KeyboardControl.Rotation.performed += Rotation;
        mainAction.KeyboardControl.Move.performed += (e) => 
        {
            MoveOn = true; 
            movement = mainAction.KeyboardControl.Move.ReadValue<Vector2>();
        };
        mainAction.KeyboardControl.Move.canceled += (e) => 
        {
            MoveHoldDur = 0f;
            MoveOn = false;
            MoveHoldCycle = 0.5f;
        };
        mainAction.KeyboardControl.Map.performed += (e) => {GameManager.Inst.uiCon.MapExpand();};
        mainAction.KeyboardControl.MainInteract.performed += (e) => 
        {
            if(tiles.FocusPos != tiles.TurnChar.POS && tiles.TurnAct == Act.Ready)
            {
                if(tiles.TurnChar.group.groupName == "Player")
                {
                    Debug.Log("키보드");
                    tiles.TurnChar.ActOnTile(tiles.tileDatas[tiles.FocusPos.y, tiles.FocusPos.x]);
                }
            }
        };
        
        mainAction.MouseControl.Enable();
        mainAction.MouseControl.Middle.performed += (e) => 
        {
            MPress = true;
            MousePos = mainAction.MouseControl.Position.ReadValue<Vector2>();
            preMousePos = MousePos;
        };
        mainAction.MouseControl.Middle.canceled += (e) => 
        {
            MPress = false;
        };
        mainAction.MouseControl.Right.performed += (e) => 
        {
            if(HoldSelect)
            {
                tiles.uiCon.UnitDataUISet(dataObject.realData);
                tiles.uiCon.UnitDataVisible(true);
                holdTimer = 0.0f;
                tiles.uiCon.HoldRing.fillAmount = 0.0f;
                tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
                return;
            }
            RPress = true;
            MousePos = mainAction.MouseControl.Position.ReadValue<Vector2>();
            StartMouseMovementForRot = mainAction.MouseControl.Position.ReadValue<Vector2>();
        };
        mainAction.MouseControl.Right.canceled += (e) => 
        {
            RPress = false;
            StartMouseMovementForRot = MouseMovementForRot;
        };
        mainAction.MouseControl.RightDouble.performed += FoucsTile;
        mainAction.MouseControl.Left.performed += (e) => 
        {
            Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(GameManager.Inst.ControlType != ControlType.WithMouse)
                return;
            if(!IsMouseOverOnUI && Physics.Raycast(_ray) && tiles.TurnChar.group.groupName == "Player" && tiles.TurnAct == Act.Ready)
            {
                tiles.TurnChar.ActOnTile(tiles.tileDatas[tiles.FocusPos.y, tiles.FocusPos.x]);
            }
            else if(!IsMouseOverOnUI && tiles.TurnChar.group.groupName == "Player" && tiles.TurnAct == Act.SelectSkill)
            {
                tiles.TurnAct = Act.SetTarget;
                tiles.SelectedChecker.Clear();
                CursorManager.CursorChange(CursorMode.Normal);
                if(FocusingTile != null && FocusingTile.tileObject != null)
                {
                    foreach(Tile checker in tiles.Checker)
                    {
                        tiles.SelectedChecker.Add(checker);
                        checker.obj.transform.Find("Checker").gameObject.SetActive(true);
                    }
                    tiles.Checker.Clear();
                }
                else
                {
                    tiles.Checker.Clear();
                    tiles.SelectedChecker.Clear();
                    tiles.TurnAct = Act.Operate;
                    foreach(Tile _t in tiles.uiCon.backupTiles) GameManager.Inst.tiles.SetTileState(_t, TileState.RedTile);
                }
            }
        };
        mainAction.MouseControl.Scroll.performed += (e) => 
        {
            if(GameManager.Inst.ControlType == ControlType.WithMouse)
                camDistance = Mathf.Clamp(camDistance - Time.fixedDeltaTime * e.ReadValue<Vector2>().y, 3, 15);
        };
        mainAction.MouseControl.RightHold.performed += (e) =>
        {
            if(!HoldSelect && FocusingTile != null && FocusingTile.tileObject != null && FocusingTile.tileObject != null && FocusingTile.tileObject is Character)
            {
                    tiles.ShowEnemiesRange();
                    holdTimer = 0.0f;
                    tiles.uiCon.HoldRing.fillAmount = 0.0f;
                    tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
                    HoldSelect = false;
                return;
            }
        };
        mainAction.MouseControl.MiddleHold.performed += (e) =>
        {
            if(dataObject == null || FocusingTile.tileObject is not Character) return;
            tiles.ShowEnemyRange((Character)dataObject);
            holdTimer = 0.0f;
            tiles.uiCon.HoldRing.fillAmount = 0.0f;
            tiles.uiCon.HoldRing.color = new Color(255, 255, 255, 0.0f);
            HoldSelect = false;
        };
    }
    private void OnDisable() 
    {
        mainAction.KeyboardControl.Rotation.performed -= Rotation;
        mainAction.KeyboardControl.Disable();  
    }
    private Tile ImportTileData()
    {
        if(IsMouseOverOnUI) return null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1<<7))
        {
            string[] a = hit.collider.name.Split('|');
            return GameManager.Inst.tiles.tileDatas[Convert.ToInt32(a[1]), Convert.ToInt32(a[0])];
        }
        return null;
    }
}