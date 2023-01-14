using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class EmojiClick : MonoBehaviour
{

    private string text;
    private void Start()
    {

        Button btn = GetComponent<Button>();
        text = btn.GetComponentInChildren<TMP_Text>().text;
        btn.onClick.AddListener(Click_this);

    }

    private void Click_this()
    {
        TMP_InputField temp = GameObject.Find("Input(Clone)").GetComponent<TMP_InputField>();
        temp.text = temp.text + text;

    }
}
