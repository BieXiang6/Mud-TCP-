using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class LinkHandler : MonoBehaviour, IPointerClickHandler
{

    private main JS;
    private UIcontroller uIcontroller;
    

    private void Start()
    {
        JS = GameObject.Find("Global controller").GetComponent<main>();
        uIcontroller = GameObject.Find("Global controller").GetComponent<UIcontroller>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TextMeshProUGUI textMeshPro = eventData.pointerPress.GetComponent<TextMeshProUGUI>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, Input.mousePosition, Camera.main);

        if (linkIndex != -1) // if a link is found
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            uIcontroller.CloseHudong();
            JS.SendMsg(linkInfo.GetLinkID());

        }
    }
}
