using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Threading;
using System;
using TMPro;
using UnityEngine;

public class UIcontroller : MonoBehaviour
{
    
    public GameObject Obj;
	public GameObject Chat;
	public GameObject Title;
	
	public GameObject DirGrid;
	public GameObject DirVertical;
	public GameObject Out;
	public GameObject Hudong;
	public GameObject Mycmds;
	public GameObject Script;
	public GameObject Enemy;


	public GameObject ShiMen;
	public GameObject BoDong;
	public GameObject Setting;
	public GameObject LianGong;
	public GameObject FuBen;

	GameObject Dirout;

	public Canvas Main;
	public Canvas Game;
	public GameObject HudongView;

	public Queue line = new Queue();

	private GameObject dire, dirw, dirn, dirso, dirsw, dirse, dirnw, dirne, dircen;
	private Slider enemyHP_s, enemyJP_s, enemyPP_s, enemyEP_s, HP_B, JP_B, PP_B;
	private TMP_Text HP_Text, BT_Text;

	private string enemyName_Last;
	string tmpurl = "";

	TMPObjectPool TMP_Pool;
	ButtonObjectPool Button_Pool;

	private float width_HD;
	private float height_HD;
	private float DirHeight;
	private bool isMap;

	private Dictionary<string, string> color;
	private ModScript Mod;
	private main JS;
	

	private void Start()
    {
		Dirout = Resources.Load("OutText") as GameObject;
		Enemy.SetActive(true);
		dirnw = DirGrid.transform.GetChild(0).gameObject;
		dirn = DirGrid.transform.GetChild(1).gameObject;
		dirne = DirGrid.transform.GetChild(2).gameObject;
		dirw = DirGrid.transform.GetChild(3).gameObject;
		dircen = DirGrid.transform.GetChild(4).gameObject;
		dire = DirGrid.transform.GetChild(5).gameObject;
		dirsw = DirGrid.transform.GetChild(6).gameObject;
		dirso = DirGrid.transform.GetChild(7).gameObject;
		dirse = DirGrid.transform.GetChild(8).gameObject;

		enemyHP_s = GameObject.Find("enemyHP").GetComponent<Slider>();
		enemyJP_s = GameObject.Find("enemyJP").GetComponent<Slider>();
		enemyEP_s = GameObject.Find("enemyEP").GetComponent<Slider>();
		enemyPP_s = GameObject.Find("enemyPP").GetComponent<Slider>();

		HP_Text = GameObject.Find("HP Text").GetComponent<TMP_Text>();
		BT_Text = GameObject.Find("Busy Text").GetComponent<TMP_Text>();
		

		HP_B = GameObject.Find("HP Bar").GetComponent<Slider>();
		PP_B = GameObject.Find("PP Bar").GetComponent<Slider>();
		JP_B = GameObject.Find("JP Bar").GetComponent<Slider>();

		width_HD = Hudong.GetComponent<RectTransform>().rect.width;
		height_HD = HudongView.GetComponent<RectTransform>().rect.height;
		DirHeight = DirVertical.GetComponent<RectTransform>().rect.height;
		Enemy.SetActive(false);
		Mod = gameObject.GetComponent<ModScript>();
		JS = gameObject.GetComponent<main>();

		TMP_Pool = new TMPObjectPool();
		Button_Pool = new ButtonObjectPool();

		color = new Dictionary<string, string>
		{
			["1m"] = "#000000",
			["30m"] = "#000000",
			["31m"] = "#BB0000",
			["32m"] = "#00BB00",
			["33m"] = "#BBBB00",
			["34m"] = "#0000BB",
			["35m"] = "#BB00BB",
			["36m"] = "#00BBBB",
			["37m"] = "#000000",
			["1;31m"] = "#CC0000",
			["1;32m"] = "#00CC00",
			["1;33m"] = "#FF9900",
			["1;34m"] = "#0000CC",
			["1;35m"] = "#CC00CC",
			["1;36m"] = "#00B3B3",
			["1;37m"] = "#000000",
			["0;0m"] = "#000000"

		};
		StartCoroutine(TaskList());

	}


	private IEnumerator TaskList()
    {
		while(true)
        {
			if (line.Count > 0)
			{
				string str = line.Dequeue() as string;
				var tempstr = str.Split("\n");
				for (var i = 0; i < tempstr.Length; i++)
				{
					WriteToUI(tempstr[i]);
					if(i%3 == 0)
						yield return new WaitForEndOfFrame();
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}


	
	

    public void StrSelector(string str)
    {
		line.Enqueue(str);
		//Debug.Log(str);
		
	}

	public string Substr(string text,int start,int length)
    {
		if (text.Length < length)
			return "error";
		return text.Substring(start, length);
    }

	private void WriteToUI(string param)
    {
		
		string str = param;
		if (str.Length < 4)
			return;
		while (str.IndexOf("[2;37;0m") >= 0)
		{
			str = str.Replace("[2;37;0m", "[0;0m");
		}
		if (Substr(str,0, 6) == "[0;0m")
		{
			str = str.Replace("[0;0m", "");
		}
		if (Substr(str,0,8) == "0000007")
        {
			JS.StopRotate();
			Main.GetComponent<CanvasGroup>().alpha = 0;
			Main.GetComponent<Canvas>().sortingOrder = 0;
			Game.GetComponent<CanvasGroup>().alpha = 1;
			Game.GetComponent<Canvas>().sortingOrder = 1;
			
			Main.enabled = false;

			return;
		}

		string temp = Substr(str, 0, 4);

		if (temp == "100")//ÁÄÌìÐÅÏ¢
        {
			WriteToChat(str.Substring(4));

		}
		else if(temp == "001")//ÊäÈë¿ò
        {
			WriteInput(str.Substring(4));
        }
		else if(temp == "002")//µØµã
        {
			ChangeHere(str.Substring(4));

		}
		else if (temp == "003")//³ö¿ÚºÍ×ßÎ»
        {
			WriteToEX(str.Substring(4));
			
		}
		else if (temp == "903")//ÒÆ³ý³ö¿ÚºÍ×ßÎ»
        {
			RemoveEX(str.Substring(4));
        }
		else if (temp == "006")//µ×ÏÂ°´Å¥
        {
			str = Regex.Replace(str, "\\$br#", "\n");
			WriteToMU(str.Substring(4));
			
        }
		else if (temp == "004")//longÄÚÎÄ±¾£¬³¤ÃèÊö
        {

			WriteToLong(str.Substring(4));
        }
		else if (temp == "005")//objÄÚµÄ°´Å¥Ôö¼Ó
        {
			WriteToObj(str.Substring(4));
        }
		else if(temp == "905")//É¾È¥objÄÚ°´Å¥
        {
			RemoveObj(str.Substring(4));
        }
		else if (temp == "007")//»¥¶¯ÎÄ±¾
        {
			WriteToHD(str.Substring(4));
		}
		else if (temp == "008" || temp == "009")//±íÇé¶¯×÷ÎÞÓÃ
        {
			str = Regex.Replace(str, "\\$br#", "\n");
			WriteToAct(str.Substring(4));
        }
		else if (temp == "011")//µØÍ¼
        {
			WriteToMap(str.Substring(4));
        }
		else if (temp == "012")//×´Ì¬²Û
        {
			WriteToHP(str.Substring(4));
        }
		else if (temp == "013")//ÏàÍ¬µÄ»¥¶¯ÎÄ±¾
		{
			WriteToHD(str.Substring(4));
		}
		else if (temp == "014")//¿ØÖÆ·¢ËÍ
        {
			JS.SendMsg(str.Substring(4));
        }
		else if (temp == "015")//ÔÚÐ¡ÆÁÄ»Êä³ö
        {
			
			
		}
		else if (temp == "020")
        {
			WriteToPop(str.Substring(4));
        }
		else if (temp == "021")//ÉÏ¿ì½Ý¼ü
        {
			WriteToBar(str.Substring(4));
        }
		else if (temp == "022")//¸üÐÂ
        {
			Debug.Log(str.Substring(4));
        }
        else //Ê£ÏÂµÄÖ±½ÓÔÚÆÁÄ»Êä³ö
		{
			
			WriteToOut(str);

			SharedVariables.Out += ClearFormat(str) + "%SV%";//ÒÔ·Ö¸î·û·Ö¸îÖ¸¶¨ÎÄ±¾
			if (SharedVariables.Out.Length > 2000)
			{
				SharedVariables.Out = SharedVariables.Out.Substring(500);
			}
			/*´Ë¶Î´úÂë±íÊ¾³¤¶È¶àÓÚ2000¾ÍÉ¾500£¬·ÀÖ¹ÊýÄ¿¹ý¶à
			 */
			//Debug.Log(SharedVariables.Out);
		}


	}


	private void WriteToLong(string str)
    {
		SharedVariables.Long = str;

		if (SharedVariables.Long.IndexOf("ÆøÑª.") != -1)
		{
			Enemy.SetActive(true);
			string temp = ClearFormat(SharedVariables.Long);
			string enemyName = temp.Substring(0, temp.IndexOf("|"));
			string enemyHP, enemyPP, enemyJP, enemyEP;
			string busyTime = "";
			if (!temp.Contains("<busy"))
			{
				enemyHP = TextGainCenter("ÆøÑª.", "¡¡ÄÚÁ¦.", temp);
				enemyPP = TextGainCenter("ÄÚÁ¦.", "¡¡¾«Éñ.", temp);
				enemyJP = TextGainCenter("¾«Éñ.", "¡¡¾«Á¦.", temp);
				enemyEP = temp.Substring(temp.IndexOf("¾«Á¦.") + 3);
			}
			else
			{
				enemyHP = TextGainCenter("ÆøÑª.", "¡¡ÄÚÁ¦.", temp);
				enemyPP = TextGainCenter("ÄÚÁ¦.", "¡¡¾«Éñ.", temp);
				enemyJP = TextGainCenter("¾«Éñ.", "<busy", temp);
				busyTime = TextGainCenter("<busy ", ">", temp);
				if (busyTime == "")
				{
					busyTime = "ÖÐ";
				}
				enemyEP = "Î´Öª";
			}
			int enemyHP_i, enemyPP_i, enemyJP_i, enemyEP_i;
			int.TryParse(enemyHP, out enemyHP_i);
			int.TryParse(enemyPP, out enemyPP_i);
			int.TryParse(enemyEP, out enemyEP_i);
			int.TryParse(enemyJP, out enemyJP_i);
			if (enemyName != enemyName_Last)
			{
				enemyName_Last = enemyName;


				enemyHP_s.maxValue = enemyHP_i;
				enemyJP_s.maxValue = enemyJP_i;
				enemyPP_s.maxValue = enemyPP_i;
				enemyEP_s.maxValue = enemyEP_i;
				enemyHP_s.value = enemyHP_i;
				enemyJP_s.value = enemyJP_i;
				enemyPP_s.value = enemyPP_i;
				enemyEP_s.value = enemyEP_i;
			}
			else
			{
				enemyHP_s.value = enemyHP_i;
				enemyJP_s.value = enemyJP_i;
				enemyPP_s.value = enemyPP_i;
				enemyEP_s.value = enemyEP_i;
			}

			if (busyTime != "")
			{
				GameObject.Find("enemyName").GetComponent<TMP_Text>().text = "Ä¿±ê£º" + enemyName + "\nÃ¦Âµ" + busyTime;
			}
			else
			{
				GameObject.Find("enemyName").GetComponent<TMP_Text>().text = "Ä¿±ê£º" + enemyName;
			}
			GameObject.Find("enemyText").GetComponent<TMP_Text>().text = "Æø£º" + enemyHP + "\nÄÚ£º" + enemyPP + "\n¾«£º" + enemyJP + "\nÁ¦£º" + enemyEP;

		}

		string res = str.Substring(str.IndexOf("Ã÷ÏÔµÄ³ö¿Ú£º") + 6);
		string strk = ClearFormat(res);
		strk = strk.Replace("\r", "");
		strk = strk.Replace("\n", "");
		if (strk != null)
		{
			SharedVariables.Direction = strk;
			
		}
	}

	private void WriteToEX(string str)
    {
		var strs = str.Split("$zj#");
		string idss = "";

		for (int i = 0;i<strs.Length;i++)
        {
			
			if (strs[i].Length < 2)
				continue;

			var dirs = strs[i].Split(":");
			if(dirs.Length==3)
            {
				idss =  dirs[2];
			}
            else 
			{
				idss = dirs[0] ;
			}
			dirs[1] = ColorPut(dirs[1]);
			if (dirs[0] == "east" || dirs[0] == "eastup" || dirs[0] == "eastdown")
			{
				dire.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dire.name = idss;
				dire.GetComponent<CanvasGroup>().alpha = 1;
				dire.GetComponent<Button>().interactable = true;

			}
			else if (dirs[0] == "west" || dirs[0] == "westup" || dirs[0] == "westdown")
			{

				dirw.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirw.name = idss;
				dirw.GetComponent<CanvasGroup>().alpha = 1;
				dirw.GetComponent<Button>().interactable = true;

			}
			else if (dirs[0] == "north" || dirs[0] == "northup" || dirs[0] == "northdown")
			{

				dirn.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirn.name = idss;
				dirn.GetComponent<CanvasGroup>().alpha = 1;
				dirn.GetComponent<Button>().interactable = true;
			}
			else if (dirs[0] == "south" || dirs[0] == "southup" || dirs[0] == "southdown")
			{

				dirso.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirso.name = idss;
				dirso.GetComponent<CanvasGroup>().alpha = 1;
				dirso.GetComponent<Button>().interactable = true;
			}
			else if (dirs[0] == "southwest")
			{

				dirsw.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirsw.name = idss;
				dirsw.GetComponent<CanvasGroup>().alpha = 1;
				dirsw.GetComponent<Button>().interactable = true;
			}
			else if (dirs[0] == "southeast")
			{

				dirse.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirse.name = idss;
				dirse.GetComponent<CanvasGroup>().alpha = 1;
				dirse.GetComponent<Button>().interactable = true;
			}
			else if (dirs[0] == "northwest")
			{

				dirnw.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirnw.name = idss;
				dirnw.GetComponent<CanvasGroup>().alpha = 1;
				dirnw.GetComponent<Button>().interactable = true;
			}
			else if (dirs[0] == "northeast")
			{

				dirne.GetComponentInChildren<TMP_Text>().text = dirs[1];
				dirne.name = idss;
				dirne.GetComponent<CanvasGroup>().alpha = 1;
				dirne.GetComponent<Button>().interactable = true;
			}
			else
			{
				Button_Pool.GetObject(DirVertical.transform, dirs[1], idss, new Vector2(0 ,DirHeight / 5),30);
				if(DirVertical.transform.childCount>5)
                {
					DirVertical.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                }
				else
                {
					DirVertical.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
				}
			}

		}


	}

	private void RemoveEX(string str)
    {
		Transform temp = DirGrid.transform.Find(str);
		if(temp == null)
        {
			str = str.Replace("\n", "");
			str = str.Replace("\r", "");
			temp = DirGrid.transform.Find(str);
		}
		if(temp != null)
        {
			temp.GetComponent<Button>().interactable = false;
			temp.GetComponent<CanvasGroup>().alpha = 0;
		}
    }


	private void ChangeHere(string str)
    {
		ReleaseButtons(DirVertical);
		ReleaseButtons(Obj);
		SharedVariables.Obj = "";
		
		Title.GetComponent<TMP_Text>().text = ColorPut(str);
		string strk = ClearFormat(str);
		strk = strk.Replace("\r", "");
		strk = strk.Replace("\n", "");

		SharedVariables.Here = strk;
		

		dire.GetComponent<CanvasGroup>().alpha = 0;
		dire.GetComponent<Button>().interactable = false;
		dirso.GetComponent<CanvasGroup>().alpha = 0;
		dirso.GetComponent<Button>().interactable = false;
		dirsw.GetComponent<CanvasGroup>().alpha = 0;
		dirsw.GetComponent<Button>().interactable = false;
		dirse.GetComponent<CanvasGroup>().alpha = 0;
		dirse.GetComponent<Button>().interactable = false;
		dirw.GetComponent<CanvasGroup>().alpha = 0;
		dirw.GetComponent<Button>().interactable = false;
		dirn.GetComponent<CanvasGroup>().alpha = 0;
		dirn.GetComponent<Button>().interactable = false;
		dirnw.GetComponent<CanvasGroup>().alpha = 0;
		dirnw.GetComponent<Button>().interactable = false;
		dirne.GetComponent<CanvasGroup>().alpha = 0;
		dirne.GetComponent<Button>().interactable = false;

		Mycmds.SetActive(false);
		CloseHudong();
		dircen.GetComponentInChildren<TMP_Text>().text = ColorPut(str);

		
	}


	private void WriteToObj(string str)
    {
		var strs = str.Split("$zj#");
		for (int i=0;i<strs.Length;i++)
        {
			var dirs = strs[i].Split(':');
			Button_Pool.GetObject(Obj.transform, ColorPut(dirs[0]), dirs[1], new Vector2(0, height_HD / 12),35);
			
			SharedVariables.Obj += ClearFormat(dirs[0]) + ":" + dirs[1] + "%SV%";//Ôö¼Ó¹²Ïí±äÁ¿ÖÐµÄObj
			

		}

	}

	private void WriteToChat(string str)
    {
		GameObject ChatText = Instantiate(Resources.Load("ChatText")) as GameObject;
		string temp = ColorPut(str);
		temp = Regex.Replace(temp, "<e:", "<sprite=");
		
		ChatText.GetComponent<TMP_Text>().text = temp;
		ChatText.transform.SetParent(Chat.transform, false);
		if (Chat.transform.childCount > 30)
		{
			Destroy(Chat.transform.GetChild(0).gameObject);
		}

		SharedVariables.Chat += ClearFormat(str) + "%SV%";//¹²ÏíÁÄÌìÐÅÏ¢£¬¼ä¸ôÎª%SV%

	}


	private void RemoveObj(string str)
    {
		GameObject a = GameObject.Find(str);
		if(!a)
        {
			str = str.Replace("\n", "");
			str = str.Replace("\r", "");
			a = GameObject.Find(str);
		}
		var s = SharedVariables.Obj.Split("%SV%");//É¾³ý¹²Ïí±äÁ¿ÖÐµÄObj
		string res = "";
		if(s.Length != 0)
        {
			for (int i = 0; i < s.Length - 1; i++)
			{
				if (s[i].Contains(str))
				{
					continue;
				}
				res += s[i] + "%SV%";
			}
		}
		
		SharedVariables.Obj = res;
		
		Button_Pool.ReleaseObject(a);

	}


	private void WriteToOut(string str)
    {
		
		str = str.Replace("$br#", "\n");
		GameObject OutText = TMP_Pool.GetObject(Out.transform,ColorPut(str),35, new Vector2(width_HD, 0));
		if(Out.transform.childCount>70)
        {
			TMP_Pool.ReleaseObject(Out.transform.GetChild(0).gameObject);
		}
		StartCoroutine(InsSrollBar());

	}
	//ÐÞ¸´¿ò×Ó²»µ½µ×µÄÎÊÌâ
	IEnumerator InsSrollBar()
	{
		yield return new WaitForEndOfFrame();
		GameObject.Find("OutView").GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
	}


	private void WriteToHP(string str)
    {
		
		var strs = str.Split("¨U");
		if (strs.Length < 8)
			return;

		var blood = strs[1].Split(":")[1].Split("/");
		SharedVariables.HP = long.Parse(blood[0]);
		SharedVariables.HP_MAX = long.Parse(blood[2]);
		HP_B.value = float.Parse(blood[0]) / float.Parse(blood[2]);
		GameObject.Find("HP Bar").transform.GetChild(0).GetComponent<Slider>().value = float.Parse(blood[1]) / float.Parse(blood[2]);


		var force = strs[2].Split(":")[1].Split("/");
		SharedVariables.Force = long.Parse(force[0]);
		SharedVariables.Force_MAX = long.Parse(force[1]) * 2;
		PP_B.value = float.Parse(force[0]) / float.Parse(force[1]) / 2;

		var will = strs[3].Split(":")[1].Split("/");
		SharedVariables.Mind = long.Parse(will[0]);
		JP_B.value = float.Parse(will[0]) / float.Parse(will[2]);
		GameObject.Find("JP Bar").transform.GetChild(0).GetComponent<Slider>().value = float.Parse(will[1]) / float.Parse(will[2]);

		var EXP = strs[5].Split(":")[1].Split("/")[0];
		SharedVariables.EXP = long.Parse(EXP);

		var potential = strs[6].Split(":")[1].Split("/")[0];
		SharedVariables.Potential = long.Parse(potential);

		HP_Text.text = "Æø£º" + blood[0] + "\nÄÚ£º" + force[0] + "\n¾«£º" + will[0] + "\nÇ±£º" + potential;


		var energy = strs[7].Split(":")[1].Split("/")[0];
		SharedVariables.Energy = long.Parse(energy);

		if(strs[0].Contains("<busy"))
        {
			if(strs[0].Contains("<busy>"))
            {
				SharedVariables.Busy = -1;
				BT_Text.text = "Ã¦ÂµÖÐ";
			}
			else
            {
				string a = Regex.Match(strs[0], "<busy .*?s>").Value;
				string temp = Regex.Replace(a, @"[^0-9]+","");
				SharedVariables.Busy = int.Parse(temp);
				BT_Text.text = "Ã¦Âµ " + SharedVariables.Busy + "s";
				if (SharedVariables.Busy == 1)
                {
					SharedVariables.Busy = 0;
					BT_Text.text = "¿ÕÏÐÖÐ";
				}
            }
        }
		else
        {
			SharedVariables.Busy = 0;
			BT_Text.text = "¿ÕÏÐÖÐ";

		}

		
		

	}

	private void WriteToMap(string str)
    {
		GameObject dirce = Instantiate(Resources.Load("HudongText")) as GameObject;
		dirce.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, height_HD / 20);
		dirce.GetComponent<TMP_Text>().fontSize = 30;
		dirce.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
		HudongView.SetActive(true);
		Hudong.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		Hudong.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
		HudongView.GetComponent<ScrollRect>().horizontal = true;
		HudongView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Unrestricted;
		string strt = str.Replace("$br#", "\n");
		
		dirce.GetComponent<TMP_Text>().text = ColorPut(strt);
		dirce.transform.SetParent(Hudong.transform);
		isMap = true;
	}


	private void WriteToHD(string str)
    {

		GameObject dirce = Resources.Load("HudongText") as GameObject;
		HudongView.SetActive(true);
		if (isMap)
        {
			Hudong.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
			Hudong.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
			HudongView.GetComponent<ScrollRect>().horizontal = false;
			HudongView.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;
			isMap = false;
		}
		
		SharedVariables.Hudong = str;

		var strs = str.Replace("$br#", "\n");
		GameObject eeee = Instantiate(dirce);
		eeee.GetComponent<TMP_Text>().text = ColorPut(strs);
		eeee.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, height_HD / 20);
		eeee.transform.SetParent(Hudong.transform, false);
	}


	private void WriteToAct(string str)
    {
		SharedVariables.Hudong_Buttons = str;
		GameObject HudongButtons = Instantiate(Resources.Load("HudongButtons"))as GameObject;
		HudongButtons.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, 0);
		HudongButtons.name = "HDButtons";
		int r = 3, w = 3, h = 10, s = 30;
		Regex strw = new Regex("\\$.*?\\#");
		string rs = strw.Match(str).Value;
		string[] ss = null;
		if(rs != "$zj#" && rs!="")
        {
			Regex strh = new Regex("[\\$,\\#]");
			ss = strh.Split(rs);
			
			if (ss.Length>4)
			{
				r = int.Parse(ss[1]);
				if (r == 1)
					r = 4;
				w = int.Parse(ss[2]);
				h = int.Parse(ss[3]);
				s = int.Parse(ss[4]);
			}
			str = str.Replace(rs, "");
		}
		var strs = str.Split("$zj#");
		HudongButtons.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width_HD / w-w*5, height_HD / h);
		HudongButtons.GetComponent<GridLayoutGroup>().spacing = new Vector2(5, 5);
		HudongButtons.transform.SetParent(Hudong.transform, false);
		for (var i = 0; i < (strs.Length / r + 1); i++)
		{
			for (var j = 0; j < r; j++)
			{
				if ((i * r + j) > (strs.Length - 1)) break;
				var dirs = strs[i * r + j].Split(':');
				if (dirs.Length < 2) continue;
				var hi = height_HD / h;
				var wi = width_HD / w;
				Button_Pool.GetObject(HudongButtons.transform, ColorPut(dirs[0]), dirs[1], new Vector2(wi, hi),35);
				
			}
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(HudongView.GetComponent<RectTransform>());
	}



	private void WriteToBar(string str)
    {
		
		GameObject a = GameObject.Find("TitleBar");
		ReleaseButtons(a);

		float height = a.GetComponent<RectTransform>().rect.height;
		float weight = a.GetComponent<RectTransform>().rect.width;
		var strs = str.Split("$zj#");
		for (var i = 0; i < strs.Length; i++)
		{
			var dirs = strs[i].Split(':');
			Button_Pool.GetObject(a.transform, dirs[0], dirs[1], new Vector2(weight * 0.22f, height),30);
			
		}
	}


	private void WriteToMU(string str)
    {
		Mycmds.SetActive(true);
		int num = Mycmds.transform.childCount;
		int j = 0;
		for (int i = 0; i < num; i++)
		{
			var child = Mycmds.transform.GetChild(j);
			if (child.gameObject.name != "CloseCmds")
			{
				Button_Pool.ReleaseObject(child.gameObject);
			}
			else
			{
				Destroy(child.gameObject);
				j++;
			}
		}

		var strs = str.Split("$zj#");
		float width = Mycmds.GetComponent<RectTransform>().rect.width;
		float height = Mycmds.GetComponent<RectTransform>().rect.height;
		Mycmds.GetComponent<GridLayoutGroup>().cellSize = new Vector2((width - 25) / 6, height / 2);
		Mycmds.GetComponent<GridLayoutGroup>().spacing = new Vector2(2, 2);
		for (var i = 0; i < strs.Length; i++)
		{
			if (strs[i].Length < 2) continue;
			var dirs = strs[i].Split(':');
			if (dirs[1] == "***" || dirs[1] == "ÔÝÎ´Éè¶¨") continue;
			Button_Pool.GetObject(Mycmds.transform, ColorPut(dirs[1]), dirs[2],35);
			if (i == 4)
			{
				GameObject dddd = Instantiate(Resources.Load("ObjButton")) as GameObject;
				dddd.GetComponent<Button>().onClick.AddListener(CloseMycmds);
				dddd.GetComponentInChildren<TMP_Text>().text = "¹Ø±Õ";
				dddd.name = "CloseCmds";
				dddd.transform.SetParent(Mycmds.transform, false);

			}
		}

	}


	public void WriteToPop(string str)
    {
		HudongView.SetActive(true);
		
		GameObject HudongButtons = Instantiate(Resources.Load("HudongButtons")) as GameObject;
		HudongButtons.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD, 0);
		HudongButtons.GetComponent<GridLayoutGroup>().cellSize = new Vector2(width_HD / 3, height_HD / 10);
		HudongButtons.transform.SetParent(Hudong.transform,false);
		GameObject dirce = Resources.Load("ActButton") as GameObject;
		
		var strs = str.Split("$z2#");
		
		for (var i = 0; i < strs.Length; i++)
		{

			var dirs = strs[i].Split('|');
			if (dirs.Length < 2) continue;
			var hi = height_HD / 3;
			var wi = width_HD / 20;
			
			dirs[0] = dirs[0].Replace("$br#", "");
			GameObject eeee = Instantiate(dirce);
			eeee.name = dirs[1];
			eeee.GetComponentInChildren<TMP_Text>().text = ColorPut(dirs[0]);
			eeee.GetComponent<RectTransform>().sizeDelta = new Vector2(wi, hi);
			eeee.transform.SetParent(HudongButtons.transform,false);
			
		}
	}

	private void WriteInput(string str)
    {
		var ss = str.Split("$zj#");
		ss[0] = ss[0].Replace("$br#", "\n");
		WriteToHD(ss[0]);
		GameObject eeee = Instantiate(Resources.Load("Input")) as GameObject;
		eeee.transform.SetParent(Hudong.transform,false);
		
		eeee.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(width_HD * 0.8), height_HD/10);
		GameObject dddd = Instantiate(Resources.Load("ObjButton")) as GameObject;
		dddd.GetComponent<Button>().onClick.AddListener(delegate {Says(ss[1],eeee);}) ;
		dddd.transform.SetParent(Hudong.transform,false);
		dddd.GetComponent<RectTransform>().sizeDelta = new Vector2(width_HD/3, height_HD/10);
		dddd.GetComponentInChildren<TMP_Text>().text = "È·ÈÏ";
		


	}


	

	public void Says(string str,GameObject a)
    {
		string strs = a.GetComponent<TMP_InputField>().text;
		strs = Regex.Replace(strs, "<sprite=", "<e:");
		if (strs == "")
			return;
		JS.SendMsg(str.Replace("$txt#",strs));
		CloseHudong();
    }

	public void CloseMycmds()
    {
		int num = Mycmds.transform.childCount;
		int j = 0;
		for (int i = 0; i < num; i++)
		{
			var child = Mycmds.transform.GetChild(j);
			if (child.gameObject.name != "CloseCmds")
			{
				Button_Pool.ReleaseObject(child.gameObject);
			}
			else
			{
				Destroy(child.gameObject);
				j++;
			}
		}
		GameObject.Find("QuickButton").GetComponentInChildren<TMP_Text>().text = "´ò¿ª¿ì½Ý°´Å¥";
		Mycmds.SetActive(false);

	}

	public void CloseHudong()
    {
		if (!HudongView.active)
			return;
		var temp = Hudong.transform.Find("HDButtons");
		if(temp != null)
        {
			int a = temp.childCount;
			Transform transform;
			for (int i = 0; i < a; i++)
			{
				transform = temp.GetChild(0);
				if(transform.name != "EmojiButton")
                {
					Button_Pool.ReleaseObject(transform.gameObject);
				}

			}
		}

		DestroyAllObjectsInChildren(Hudong);
		HudongView.SetActive(false);
		
	}

	
	private string ColorPut(string str)
    {
		string strt = str;
		strt = Regex.Replace(strt,"\\[s:(?<std>.*?)]", "#B#${std}#X#");
		var raw_text_chunks = strt.Split("[");
		string res = "";
		for(int i=1;i<raw_text_chunks.Length;i++)
        {
			string temp = Chunk_Process(raw_text_chunks[i]);
			
			Match fontsize = Regex.Match(temp, "#B#(?<str>.*?)#X#");
			if(fontsize.Success)
            {
				int value;
				if (int.TryParse(fontsize.Groups["str"].Value, out value))
				{
					value = value / 2;
					temp = Regex.Replace(temp, "#B#(?<str>.*?)#X#", "<size=+" + value + ">");
					temp += "</size>";
				}
			}

			res += temp;
        }
		return raw_text_chunks[0] + res;
	}

	private string Chunk_Process(string text)
    {
		string res;
		if (text.Contains("u:cmds:"))
		{
			var ss = text.Split("]");

			if (ss[1] != "")
            {
				res = "<u><link=\"" + ss[0].Substring(7) + "\">" + ss[1] + "</link></u>";
				return res;
			}
			else
			{
				tmpurl = ss[0].Substring(7);
				return "";
			}
		}
		if (tmpurl != "")
		{
			text = Regex.Replace(text, "\\d.+m", "");
			res = "<u><link=\"" + tmpurl + "\">" + text + "</link></u>";
			tmpurl = "";
			return res;
		}

		var matches = Regex.Match(text, "^([!\x3c-\x3f]*)([\\d;]*)([\x20-\x2c]*[\x40-\x7e])([\\s\\S]*)");
		if (!matches.Success)
			return text;
		string orig_txt = matches.Groups[4].Value;
		if (orig_txt == "\r" || orig_txt == "\t" || orig_txt == "")
			return "";
		if (matches.Groups[1].Value  != "" || matches.Groups[3].Value != "m")
		{
			return orig_txt;
		}

		if(color.ContainsKey(matches.Groups[2].Value + "m"))
        {
			res = "<color=" + color[matches.Groups[2].Value + "m"] + ">" + orig_txt + "</color>";
		}
        else
        {
			res = "<color=#000000>" + orig_txt + "</color>";
		}


		
		return res;

	}


	public string ClearFormat(string str)
    {
		string strs = str;
		string temp = Regex.Replace(strs, "\\[\\d.*?m", "");
		temp = Regex.Replace(temp, "\\[.*?]", "");
		return temp;
    }

	public void OpenSetting()
    {
		OpenScript();
		Setting.SetActive(true);
		ShiMen.SetActive(false);
		FuBen.SetActive(false);
		BoDong.SetActive(false);
		LianGong.SetActive(false);
		Slider slider = GameObject.Find("performanceSlider").GetComponent<Slider>();
		TMP_Text lable = GameObject.Find("performanceLable").GetComponent<TMP_Text>();
		int Performance = PlayerPrefs.GetInt("Performance", 1);
		int isBackData = PlayerPrefs.GetInt("isBackData", 0);
		if(isBackData == 1)
        {
			GameObject.Find("isBackData").GetComponent<Toggle>().isOn = true;
        }
		else
        {
			GameObject.Find("isBackData").GetComponent<Toggle>().isOn = false;
		}
		switch (Performance)
		{
			case 0:
				lable.text = "½ÚÄÜ";
				break;
			case 1:
				lable.text = "Á÷³©";
				break;
			case 2:
				lable.text = "¸ßÖ¡ÂÊ";
				break;
			case 3:
				lable.text = "³¬¸ßÖ¡";
				break;
		}
		slider.value = Performance;
	}

	public void ChangePerformance()
    {
		float value = GameObject.Find("performanceSlider").GetComponent<Slider>().value;
		TMP_Text lable = GameObject.Find("performanceLable").GetComponent<TMP_Text>();
		int refreashRate = Screen.currentResolution.refreshRate;
		switch((int)value)
        {
            case 0:
                Application.targetFrameRate = 30;
				PlayerPrefs.SetInt("Performance", 0);
				lable.text = "½ÚÄÜ";
				break;
            case 1:
				Application.targetFrameRate = 60;
				PlayerPrefs.SetInt("Performance", 1);
				lable.text = "Á÷³©";
				break;
            case 2:
				Screen.SetResolution(0, 0, true, 0);
				Application.targetFrameRate = 90;
				PlayerPrefs.SetInt("Performance", 2);
				lable.text = "¸ßÖ¡ÂÊ";
				if(refreashRate != 90)
                {
					ShowInf("[1;36mÄúµ±Ç°µÄÆÁÄ»Ë¢ÐÂÂÊÎª " + refreashRate + " Hz£¬¿ÉÄÜ²¢²»ÊÊºÏ¸ÃÖ¡ÂÊ[0;0m");
				}
                else
                {
					ShowInf("[1;36mÄúµ±Ç°µÄÆÁÄ»Ë¢ÐÂÂÊÎª " + refreashRate + " Hz£¬ÓÐ¿ÉÄÜ´ïµ½¸ÃÖ¡ÂÊ£¬Êµ¼ÊÇé¿ö¿ÉÄÜÊÜÐÔÄÜÓ°Ïì[0;0m");
				}
                break;
            case 3:
				Screen.SetResolution(0, 0, true, 0);
				Application.targetFrameRate = 120;
				PlayerPrefs.SetInt("Performance", 3);
				lable.text = "³¬¸ßÖ¡";
				if (refreashRate < 120)
				{
					ShowInf("[1;36mÄúµ±Ç°µÄÆÁÄ»Ë¢ÐÂÂÊÎª " + refreashRate + " Hz£¬¿ÉÄÜ²¢²»ÊÊºÏ¸ÃÖ¡ÂÊ[0;0m");
				}
				else
				{
					ShowInf("[1;36mÄúµ±Ç°µÄÆÁÄ»Ë¢ÐÂÂÊÎª " + refreashRate + " Hz£¬ÓÐ¿ÉÄÜ´ïµ½¸ÃÖ¡ÂÊ£¬Êµ¼ÊÇé¿ö¿ÉÄÜÊÜÐÔÄÜÓ°Ïì[0;0m");
				}
				break;
		}
    }

	public static string TextGainCenter(string left, string right, string text)
	{

		if (string.IsNullOrEmpty(left))
			return "";
		if (string.IsNullOrEmpty(right))
			return "";
		if (string.IsNullOrEmpty(text))
			return "";
		int Lindex = text.IndexOf(left);
		if (Lindex == -1)
		{
			return "";
		}
		Lindex = Lindex + left.Length;
		int Rindex = text.IndexOf(right, Lindex);
		if (Rindex == -1)
		{
			return "";
		}
		return text.Substring(Lindex, Rindex - Lindex);
	}


	public void ShowInf(string inf)
    {
		line.Enqueue(inf+"\n");
    }

	public void OpenCmd()
    {
		WriteInput("ÇëÊäÈëÖ¸Áî£º$zj#$txt#");
    }
	
	public void OpenScript()
    {
		Script.SetActive(true);
    }

	public void ChangeBackData()
    {
		bool temp = GameObject.Find("isBackData").GetComponent<Toggle>().isOn;
		if(temp)
        {
			PlayerPrefs.SetInt("isBackData", 1);
		}
		else
        {
			PlayerPrefs.SetInt("isBackData", 0);
		}
    }

	public void CloseScript()
    {
		Script.SetActive(false);
    }


	public void OpenShiMen()
    {
		ShiMen.SetActive(true);
		BoDong.SetActive(false);
		Setting.SetActive(false);
		FuBen.SetActive(false);
		LianGong.SetActive(false);
	}
	public void OpenBoDong()
	{
		ShiMen.SetActive(false);
		Setting.SetActive(false);
		BoDong.SetActive(true);
		FuBen.SetActive(false);
		LianGong.SetActive(false);
	}

	public void OpenFuBen()
    {
		Setting.SetActive(false);
		ShiMen.SetActive(false);
		FuBen.SetActive(true);
		BoDong.SetActive(false);
		LianGong.SetActive(false);
	}

	public void CloseEnemy()
    {
		Enemy.SetActive(false);
    }

	public void AddQuick()
    {
		string temp = PlayerPrefs.GetString("Quick", "");
		if (temp.Split("$zj#").Length >= 11)
			return;
		string str = GameObject.Find("QuickInput").GetComponent<TMP_InputField>().text;
		if(!str.Contains(":"))
        {
			return;
        }
		if(temp == "")
        {
			temp = "b2:" + str;
		}
        else 
		{
			temp = temp + "$zj#b2:" + str;
		}
		PlayerPrefs.SetString("Quick", temp);
		ShowInf("[1;36m³É¹¦ÉèÖÃ¿ì½Ý¼ü£¡[0;0m");
		CloseScript();
		
	}

	public void DestroyAllObjectsInChildren(GameObject temp)
    {
		Transform[] children = temp.GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child != temp.transform)
			{
                Destroy(child.gameObject);
			}
		}
	}

	public void ReleaseButtons(GameObject temp)
    {
		int a = temp.transform.childCount;
		Transform transform;
		for (int i = 0; i < a; i++)
		{
			transform = temp.transform.GetChild(0);
			Button_Pool.ReleaseObject(transform.gameObject);
			
		}
	}


	public void DelQuick()
    {
		string temp = PlayerPrefs.GetString("Quick", "");
		if (temp == "")
			return;
		if(temp.IndexOf("$zj#") !=-1)
        {
			temp = temp.Substring(0, temp.LastIndexOf("$zj#"));
		}
		else
        {
			temp = "";
        }
		PlayerPrefs.SetString("Quick", temp);
		ShowInf("[1;36m³É¹¦É¾³ý¿ì½Ý¼ü£¡[0;0m");
		CloseScript();
	}

	public void OpenQuick()
    {
		TMP_Text buttonText = GameObject.Find("QuickButton").GetComponentInChildren<TMP_Text>();
		if(buttonText.text == "´ò¿ª¿ì½Ý°´Å¥")
        {
			string temp = PlayerPrefs.GetString("Quick", "b1:¿Õ:look");
			WriteToMU(temp);
			buttonText.text = "¹Ø±Õ¿ì½Ý°´Å¥";
			
		}
        else
        {
			CloseMycmds();
		}
		
    }

	public void OpenLianGong()
    {
		ShiMen.SetActive(false);
		Setting.SetActive(false);
		BoDong.SetActive(false);
		FuBen.SetActive(false);
		LianGong.SetActive(true);
		int LianGongSetting = PlayerPrefs.GetInt("LianGongSetting", 0);
		GameObject.Find("LianGongSetting").GetComponent<TMP_Dropdown>().value = LianGongSetting;
		GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("LianGongSetting" + LianGongSetting, "");

	}



	public void LiaoTian()
    {
		//WriteInput("ÇëÊäÈëÁÄÌìÄÚÈÝ£º$zj#chat $txt#");
		//WriteToAct("$3,3,9,30#Ñ¡ÔñÆµµÀ:liaotian pindao$zj#±íÇé¶¯×÷:emote$zj#ºÃÓÑ:friend");
		JS.SendMsg("liaotian");
		Invoke("LiaoTian_main", 0.3f);
		
	}
	private void LiaoTian_main()
    {
		GameObject dirce = Resources.Load("EmojiButton") as GameObject;
		GameObject HudongButtons = GameObject.Find("HDButtons");
		if (HudongButtons == null)
			return;
		for (int i = 0; i < 15; i++)
		{
			GameObject eeee = Instantiate(dirce);
			eeee.GetComponentInChildren<TMP_Text>().text = "<sprite=" + i + ">";
			eeee.name = "EmojiButton";
			eeee.GetComponent<RectTransform>().sizeDelta = new Vector2(473, 207);
			eeee.transform.SetParent(HudongButtons.transform, false);
		}
	}

}
