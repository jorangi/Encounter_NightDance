using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurnQueueItem : MonoBehaviour, IPointerClickHandler
{
    public UnitPortrait unitP;
    public float DCtimer = 0f;
    public void Update()
    {
        DCtimer = Mathf.Max(0, DCtimer - Time.deltaTime);
    }
    public void FindUnitLoc()
    {
        Vector2 p = GameManager.Inst.tiles.TilePos(unitP.tileObject.tempPOS);
        GameManager.Inst.tiles.Focus.localPosition = new Vector3(p.x, p.y, GameManager.Inst.tiles.Focus.localPosition.z);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(DCtimer == 0)
        {
            DCtimer = 0.5f;
            return;
        }
        GameManager.Inst.inputManager.camDistance = GameManager.Inst.inputManager.camDistance == 3 ? 15 : 3;
    }

    public void Outlining()
    {
        unitP.tileObject.Outlining();
    }
    public void RemoveOutline()
    {
        unitP.tileObject.RemoveOutlining();
    }
}
