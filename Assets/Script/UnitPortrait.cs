using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
    public TileObject tileObject;
    public Image Group;
    public GameObject Unit;

    public void Init(TileObject tObj)
    {
        tileObject = tObj;
        GameObject unit = Instantiate(Resources.Load<GameObject>($"Prefab/Standing_Character/{tObj.id}"), Unit.transform.parent);
        unit.name = "Base";
        unit.transform.localScale = new Vector2(0.33f, 0.33f);
        unit.transform.SetSiblingIndex(1);
        Destroy(Unit);
        Unit = unit;
    }
}
