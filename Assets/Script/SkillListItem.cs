using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillListItem : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public string id;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Inst.tiles.TurnChar.SelectSkill(id);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.Inst.tiles.TurnChar.SelectSkill(string.Empty);
    }
}
