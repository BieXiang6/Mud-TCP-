using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ClickThis : MonoBehaviour
{
    private main maincs;
    private UIcontroller uicontroller;
    private void Start()
    {
        maincs = GameObject.Find("Global controller").GetComponent<main>();
        uicontroller = GameObject.Find("Global controller").GetComponent<UIcontroller>();
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(Click_this);
        
    }

    private void Click_this()
    {
        uicontroller.CloseHudong();
        if ( uicontroller.Substr(gameObject.name,0,4) == "020")
        {
            uicontroller.WriteToPop(gameObject.name.Substring(4));
        }
        else
        {
            maincs.SendMsg(gameObject.name);
            
        }
        
    }
}
