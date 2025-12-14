using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContextLinking : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI text;


    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, Camera.main);

        if(linkIndex != -1)
        {
            string clickWord = text.textInfo.linkInfo[linkIndex].GetLinkID();
            string[] con = DataManager.DescConvert(clickWord);
            if(transform.parent.name.IndexOf("Main") > -1)
            {
                GameManager.Inst.uiCon.SubDescBox.gameObject.SetActive(true);
                GameManager.Inst.uiCon.SubDescBox.Find("Name").GetComponent<TextMeshProUGUI>().text = $"▼ {con[0]}";
                GameManager.Inst.uiCon.SubDescBox.Find("Desc").GetComponent<TextMeshProUGUI>().text = con[1];
            }
            else
            {
                GameManager.Inst.uiCon.SubDescBox.Find("Name").GetComponent<TextMeshProUGUI>().text = $"▼ {con[0]}";
                GameManager.Inst.uiCon.SubDescBox.Find("Desc").GetComponent<TextMeshProUGUI>().text = con[1];
            }
            StartCoroutine(GameManager.Inst.uiCon.ForceUpdateUI(GameManager.Inst.uiCon.TooltipList));
        }
    }
}
