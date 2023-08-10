using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
        DataManager = FindObjectOfType<DataManager>();
        uiCon = FindObjectOfType<UIController>();
        tiles = FindObjectOfType<Tiles>();
        cursor = FindObjectOfType<CursorManager>();
        inputManager = FindObjectOfType<InputManager>();
        ControlType = controlType.WithMouse;
        Init();
    }
    private void Init()
    {
        langCode = 0;
    }

    public InputManager inputManager;
    public Tiles tiles;
    public UIController uiCon;
    public DataManager DataManager;
    public CursorManager cursor;
    public static int langCode = 0; //0:eng, 1:kor
    public float UnitMoveSpeed = 1.0f;
    public controlType ControlType = controlType.WithMouse;
}
