using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Cmdsa : MonoBehaviour
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
        string temp = gameObject.name;
        uicontroller.CloseHudong();
        if (uicontroller.Substr(temp, 0, 4) == "020")
        {
            
            uicontroller.WriteToPop(temp.Substring(4));
        }
        else
        {

            maincs.SendMsg(temp);

        }
        
    }
}
