using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomCardData
{
    public int Value;
}
public class RandomCard : MonoBehaviour
{
    public Color PositiveColor , NegativeColor;
    public RandomCardData[] data = new RandomCardData[2];
    public Image[] box = new Image[2];
    public Sprite[] Icons = new Sprite[2];

    public void Init(params RandomCardData[] datas)
    {
        if(datas.Length == 1)
        {
            data[0] = datas[0];
            box[0].color = datas[0].Value > 0 ? PositiveColor : NegativeColor;
            box[0].transform.Find("Icon").GetComponent<Image>().sprite = Icons[0];
            box[0].transform.Find("ValueBox").gameObject.SetActive(true);
            box[0].transform.Find("ValueBox").GetComponentInChildren<TextMeshProUGUI>().text = datas[0].Value.ToString();
            
            box[1].color = box[0].color;
            box[1].transform.Find("Icon").GetComponent<Image>().sprite = Icons[0];
            box[1].transform.Find("ValueBox").gameObject.SetActive(false);
        }
        else
        {
            data[0] = datas[0];
            box[0].color = datas[0].Value > 0 ? PositiveColor : NegativeColor;
            box[0].transform.Find("Icon").GetComponent<Image>().sprite = Icons[0];
            box[0].transform.Find("ValueBox").gameObject.SetActive(true);
            box[0].transform.Find("ValueBox").GetComponentInChildren<TextMeshProUGUI>().text = datas[0].Value.ToString();
            data[0] = datas[0];

            box[1].transform.Find("ValueBox").gameObject.SetActive(true);
            box[1].color = datas[1].Value > 0 ? PositiveColor : NegativeColor;
            box[1].transform.Find("Icon").GetComponent<Image>().sprite = Icons[1];
            box[1].transform.Find("ValueBox").gameObject.SetActive(true);
            box[1].transform.Find("ValueBox").GetComponentInChildren<TextMeshProUGUI>().text = datas[1].Value.ToString();
        }
    }
    public void Select()
    {

    }
}
