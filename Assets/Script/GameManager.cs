using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Material baseMaterial;
    public bool InputBlock = true;
    private static GameManager inst = null;
    public static GameManager Inst
    {
        get
        {
            if(inst == null)
            {
                return null;
            }
            return inst;
        }
    }
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(inst);
            return;
        }
        Application.targetFrameRate = 120;
        DataManager = FindFirstObjectByType<DataManager>();
        uiCon = FindFirstObjectByType<UIController>();
        tiles = FindFirstObjectByType<Tiles>();
        cursor = FindFirstObjectByType<CursorManager>();
        inputManager = FindFirstObjectByType<InputManager>();
        ControlType = ControlType.WithMouse;
        Init();
    }
    private void Init()
    {
        langCode = 0;
        UnitMoveSpeed = 2.75f;
    }

    public InputManager inputManager;
    public Tiles tiles;
    public UIController uiCon;
    public DataManager DataManager;
    public CursorManager cursor;
    public static int langCode = 0; //0:kor, 1:eng
    public float UnitMoveSpeed = 2.5f;
    public ControlType ControlType = ControlType.WithMouse;
}
