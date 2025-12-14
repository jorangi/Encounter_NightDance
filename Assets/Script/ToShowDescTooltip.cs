using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ToShowDescTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    UIController ui;
    private void Awake()
    {
        ui = GameManager.Inst.uiCon;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ui.ToShowTooltipSelectedUI = GetComponent<RectTransform>();
        ui.SelectedUI = name.ToUpper();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.tempToShowTooltipSelectedUI = GetComponent<RectTransform>();
        ui.TempSelectedUI = name.ToUpper();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ui.TempSelectedUI = "";
    }
}
