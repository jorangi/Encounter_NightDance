using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorMode
{
    Normal,
    Attack,
    DisabledAttack
}
public class CursorManager : MonoBehaviour
{
    public Texture2D NormalCursor;
    public Texture2D AttackCursor;
    public Texture2D DisabledAttackCursor;
    public static CursorMode cursorMode;

    public void Awake()
    {
        CursorChange(CursorMode.Normal);
    }
    public static void CursorChange(CursorMode mode)
    {
        cursorMode = mode;
        switch(mode)
        {
            case CursorMode.Normal:
                Cursor.SetCursor(GameManager.Inst.cursor.NormalCursor, Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
                break;
            case CursorMode.Attack:
                Cursor.SetCursor(GameManager.Inst.cursor.AttackCursor, Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
                break;
            case CursorMode.DisabledAttack:
                Cursor.SetCursor(GameManager.Inst.cursor.DisabledAttackCursor, Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
                break;

        }
    }
}
