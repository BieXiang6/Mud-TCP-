using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Threading;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class ModScript : MonoBehaviour
{

    private main JS;
    public GameObject isSkill;
    public GameObject isCmd;
    public GameObject CheckBut;
    public GameObject StartButton;
    public GameObject ShiMenInf;
    private UIcontroller uIcontroller;
    private bool isSkill_bool = false;
    private bool isCmd_bool = false;
    private bool ShiMen_isFight = false;
    private bool ShiMen_isOn = false;
    private bool BoDong_isOn = false;


    private string SkillName;
    private string Cmds;
    private int Cmds_num;

    private string BoDong_Skill;
    private int BoDong_Num;
    private int BoDong1_Potential;

    private List<string> ThreadList = new List<string>();

    private string ShiMen_north = "½ðË®ÇÅ ÌìÉ½É½Â· µ¶µê ´ó½«¾ü¸® Î÷½Ö Ò×Ïª²¿ ÇàÁú½Ö ËéÊ¯Â· ÎÚÃÉ´åÂä ¶«´ó½Ö Î÷´ó½Ö Î÷Õò½Ö ¶«³Ç ÍÁµØÃí ´òÌúÆÌ";
    private string ShiMen_south = "³çÊ¥ÃÅ Î÷´å¿Ú ¶á±¦¿Ú Î÷´ó½Ö ÎÚÃÉ´åÂä ´óÖñÂ¥ ¹ú×Ó¼à Î÷½Ö °×ºÓ Óæ´åÐ¡ÎÝ Î÷´ó½Ö ËéÊ¯Â· ±±ÄÚ´ó½Ö Ò»Æ·ÌÃ´óÃÅ Ð¡Ôº×Ó ´ó½«¾ü¸® Â¥ÌÝ";
    private string ShiMen_west = "¶«ÄÚ´ó½Ö ÄÏ´ó½Ö ÇàÊ¯´óµÀ À¥ÂØÉ½ÏÂ ËéÊ¯Â· ²ÔÉ½ ÓÀÌ©´óµÀ ÄÏ°²´óµÀ ±±´ó½Ö ²½ÐÛ²¿ ÖÒÁÒìô ÃÙÏãÂ¥";
    private string ShiMen_east = "±±½Ö ÇàÊ¯´óµÀ ÄÏ´ó½Ö ÑØºþ´óµÀ ÕòÐÛ ¶Ä³¡ Áõ×¯ ¸»¼Ò²àÃÅ";
    private int ShiMen_Number_left;
    private int ShiMen_Number;
    private long ShiMen_StartTime;

    Thread newBodong1;
    Thread LianGongThread;
    Thread YuLuoShaFuBen;
    Thread DuoBaoFuBen;

    private Queue<string> TaskQueue = new Queue<string>();

    


    private void Start()
    {
        JS = gameObject.GetComponent<main>();
        uIcontroller = gameObject.GetComponent<UIcontroller>();
        Button but = CheckBut.GetComponent<Button>();
        but.onClick.AddListener(ShiMen_Check_main);

    }

    private void Update()
    {
        if(TaskQueue.Count != 0)
        {
            string line = TaskQueue.Dequeue();

            if(line.Substring(0,5) == "<001>")
            {
                ShiMenInf.GetComponent<TMP_Text>().text = "Ê¦ÃÅÒÑ¾­È«²¿Íê³É£¡";
                ShiMen_isOn = false;
                StartButton.GetComponent<Button>().interactable = false;
                CheckBut.GetComponent<Button>().interactable = true;
                StartButton.GetComponentInChildren<TMP_Text>().text = "<--------";
            }
            if(line.Substring(0,5) == "<002>")
            {
                ShiMenInf.GetComponent<TMP_Text>().text = line.Substring(5);
                    
            }
        }
    }

    private void ShiMen_Check_main()
    {
        StartCoroutine(ShiMen_Check());
    }

    public void IsSkill()
    {
        if(isSkill.GetComponent<Toggle>().isOn)
        {
            isSkill_bool = true;
            
        }
        else
        {
            isSkill_bool = false;
        }
    }

    public void IsCmd()
    {
        if(isCmd.GetComponent<Toggle>().isOn)
        {
            isCmd_bool = true;
        }
        else
        {
            isCmd_bool = false;
        }
    }

    public void ShiMen()
    {
        
        if(StartButton.GetComponentInChildren<TMP_Text>().text == "¿ªÊ¼")
        {
            StartButton.GetComponentInChildren<TMP_Text>().text = "½áÊø";
            ShiMen_isOn = true;
            ShiMen_StartTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            ShiMen_Number = 0;
            Thread ShiMen_Task = new Thread(ShiMen_Main);
            ShiMen_Task.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            ShiMen_isOn = false;
            StartButton.GetComponent<Button>().interactable = false;
            CheckBut.GetComponent<Button>().interactable = true;
            StartButton.GetComponentInChildren<TMP_Text>().text = "<--------";
        }
    }

    

    IEnumerator ShiMen_Check()
    {
        Cmds = GameObject.Find("CmdInput").GetComponent<TMP_InputField>().text;
        Cmds_num = int.Parse(GameObject.Find("ShiMenNum").GetComponent<TMP_InputField>().text);
        SkillName = GameObject.Find("SkillInput").GetComponent<TMP_InputField>().text;
        JS.SendMsg("team");
        yield return new WaitForSeconds(1);
        if (!SharedVariables.Hudong_Buttons.Contains("Ìß³ö¶ÓÎé"))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "ÄúÐèÒªÏÈ×é¶Ó²ÅÄÜ½øÐÐÊ¦ÃÅ";
            yield break;
        }
        SharedVariables.Out = "";
        JS.SendMsg("exert recover\nexert regenerate\naccept quest\nquest\nuse ba gua");
        yield return new WaitForSeconds(1);
        if(!int.TryParse(TextGainCenter("±¾ÖÜ»¹¿ÉÁìÈ¡Ê¦ÃÅÈÎÎñ´ÎÊý£º", "´Î", SharedVariables.Out),out ShiMen_Number_left))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "Çë²»ÒªÔÚÁìÈÎÎñºóµã»÷±¾°´Å¥£¡";
            yield break;
        }
        if(ShiMen_Number_left == 0)
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "Çë²»ÒªÔÚÈÎÎñÈ«²¿Íê³Éºóºóµã»÷±¾°´Å¥£¡";
            yield break;
        }
        ShiMenInf.GetComponent<TMP_Text>().text = "ÄúÒÑ¾­·ûºÏÒªÇóÁË£¡Çë<color=red>ÁìÈ¡Ê¦ÃÅÈÎÎñ</color>ÒÔºóÔÙµã»÷¿ªÊ¼£¡";
        CheckBut.GetComponent<Button>().interactable = false;
        StartButton.GetComponent<Button>().interactable = true;
        StartButton.GetComponentInChildren<TMP_Text>().text = "¿ªÊ¼";

    }

    public void ShiMen_Main()
    {
        if (ShiMen_isOn == false)
            return;
        SharedVariables.Out = "";
        string ShiMen_Name, ShiMen_ID;
        JS.SendMsg("exert recover\nexert regenerate\naccept quest\nquest\nuse ba gua");
        Thread.Sleep(700);
        string str = SharedVariables.Out;
        if (!str.Contains("Ê¹ÓÃ°ËØÔÅÌÕ¼µÃÔÚ»§Íâ"))
        {
            string temp = TextGainCenter("Ö®Ç°¸îÏÂ", "µÄÈËÍ·", str);
            ShiMen_Name = temp.Split("(")[0];
            ShiMen_ID = TextGainCenter("(", ")", temp);
            if (ShiMen_ID != "")
            {
                JS.SendMsg("team kill " + ShiMen_ID);
                ShiMen_isFight = true;
                if(isSkill_bool)
                {
                    Thread doSkill = new Thread(DoSkill);
                    doSkill.Start();
                }
                Thread.Sleep(150);
                ObjScanner(ShiMen_Name);

            }
            else
            {
                ShiMen_Main();
            }
        }
        else
        {
            GetOut();
        }
    }


    public void DoSkill()
    {
        while(ShiMen_isFight)
        {
            Thread.Sleep(500);
            JS.SendMsg(SkillName);
        }
    }

   

    public void GetOut()
    {
        string here = SharedVariables.Here;
        if(ShiMen_north.Contains(here))
            JS.SendMsg("north");
        else if(ShiMen_south.Contains(here))
            JS.SendMsg("south");
        else if (ShiMen_west.Contains(here))
            JS.SendMsg("west");
        else if (ShiMen_east.Contains(here))
            JS.SendMsg("east");
        Thread.Sleep(300);
        if (SharedVariables.Here != here)
        {
            ShiMen_Main();
            return;
        }

        if (here == "ÇàÑò¹¬")
            JS.SendMsg("northeast");
        else if (here == "ÐþÌ³Ãí")
            JS.SendMsg("southwest");
        else if (here == "ÖÐÔÀ´óµî")
            JS.SendMsg("northup");
        else if (here == "±øÆ÷ÆÌ")
            JS.SendMsg("southeast\nwest");
        else if (here == "Ãñ·¿")
            JS.SendMsg("northeast");
        else if (here == "ÖñÂ¥")
            JS.SendMsg("down");
        else if (here == "ºêÊ¥ËÂËþ" || here == "Ëþ»ù")
            JS.SendMsg("d\nd\nout\nw");
        else if (here == "¸ê±ÚÌ²")
            JS.SendMsg("n\nw");
        else if (here == "¼Àìë´óÎÝ" || here == "´óÌü" || here == "É½ÉñÃí")
            JS.SendMsg("out");
        else if (here == "¼ÀìëÎÝ")
            JS.SendMsg("d\nd\nout");
        else if (here == "ÒéÊÂÌÃ")
            JS.SendMsg("down");
        Thread.Sleep(300);
        if(SharedVariables.Here != here)
        {
            ShiMen_Main();
        }
        else
        {
            AutoWayFinding();
            
        }

    }


    public void AutoWayFinding()
    {
        JS.SendMsg("look");
        Thread.Sleep(300);
        var temp = SharedVariables.Direction.Split("  ");
        System.Random r = new System.Random();
        int i = r.Next(0,temp.Length);
        JS.SendMsg(temp[i]);
        Thread.Sleep(300);
        ShiMen_Main();
        
    }

    
    public void ObjScanner(string ShiMen_Name)
    {
        while(true)
        {
            if(SharedVariables.Obj.IndexOf(ShiMen_Name) == -1)
            {
                ShiMen_isFight = false;
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                ShiMen_Main();
                break;
            }
            if(SharedVariables.Obj.IndexOf(ShiMen_Name+"µÄÊ¬Ìå") != -1)
            {
                ShiMen_isFight = false;
                Thread.Sleep(2000);
                ShiMen_Counter();
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                ShiMen_Main();
                break;
            }
            Thread.Sleep(100);
        }
    }

    public void ShiMen_Counter()
    {
        ShiMen_Number++;
        ShiMen_Number_left--;
        long nowTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        
        if( isCmd_bool && ShiMen_Number % Cmds_num == 0)
        {
            JS.SendMsg(Cmds);
        }
        float speed = (float)ShiMen_Number / (float)(nowTime - ShiMen_StartTime) * 60;
        float time_left = (float)ShiMen_Number_left / speed;
        
        if(ShiMen_Number_left == 0)
        {
            TaskQueue.Enqueue("<001>");
            return;
        }
        
        string temp = "ÒÑÍê³É " + ShiMen_Number+ "  ¸öÊ¦ÃÅ\n»¹Ê£ " + ShiMen_Number_left
            + "¸öÊ¦ÃÅ\nÊ¦ÃÅËÙ¶È " + Math.Round(speed, 1)
            + " ¸ö/·ÖÖÓ\nÈ«²¿Íê³É»¹Ðè " + Math.Round(time_left) + " ·ÖÖÓ";
        TaskQueue.Enqueue("<002>" + temp);
        
    }

    public void SavePotential()
    {
        JS.SendMsg("qncunru " + SharedVariables.Potential.ToString() + " qn");
    }

    public void BoDong1()
    {
        GameObject e = GameObject.Find("BoDongStart1");
        if(e.GetComponentInChildren<TMP_Text>().text == "¿ªÊ¼")
        {
            if (ThreadList.Contains("BoDong1"))
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            BoDong1_Potential = int.Parse(GameObject.Find("potential1").GetComponent<TMP_InputField>().text);
            newBodong1 = new Thread(BoDong1_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "½áÊø";
            newBodong1.Start();
            uIcontroller.ShowInf("[1;36m¿ªÊ¼Ä£Ê½Ò»ÌáÉý¼¼ÄÜ£¡£¡![0;0m");
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "¿ªÊ¼";
            BoDong_isOn = false;
            newBodong1.Abort();
            ThreadList.Remove("BoDong1");
        }
        
    }

    public void BoDong1_Main()
    {
        ThreadList.Add("BoDong1");
       
        for (int i=0;i<BoDong_Num;i++)
        {
            if (!BoDong_isOn)
                break;
            int left = BoDong1_Potential - (int)SharedVariables.Potential + 1;
            JS.SendMsg("qntiqu " + left + " qn");
            float overTime = 0f;
            while(SharedVariables.Potential<BoDong1_Potential)
            {
                Thread.Sleep(300);
                overTime += 0.3f;
                if (overTime > 10)
                    break;
            }
            if (overTime > 10) continue;
            while (true)
            {
                string n = "research " + BoDong_Skill + " 1\n";
                SharedVariables.Out = "";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                string temp = SharedVariables.Out;
                if(temp.Contains("¹§Ï²£º"))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m³É¹¦ÌáÉý" + (i + 1).ToString() + "¼¶¼¼ÄÜ£¡£¡£¡[0;0m");
            Thread.Sleep(1000);
        }
        ThreadList.Remove("BoDong1");
    }

    


    public void BoDong2()
    {
        
        GameObject e = GameObject.Find("BoDongStart2");
        if (e.GetComponentInChildren<TMP_Text>().text == "¿ªÊ¼")
        {
            if (ThreadList.Contains("BoDong2"))
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            Thread newBoDong2 = new Thread(BoDong2_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "½áÊø";
            newBoDong2.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "¿ªÊ¼";
            BoDong_isOn = false;
        }
        
    }

    public void BoDong2_Main()
    {
        ThreadList.Add("BoDong2");
        uIcontroller.ShowInf("[1;36m¿ªÊ¼Ä£Ê½¶þÌáÉý¼¼ÄÜ£¡£¡![0;0m");
        for (int i=0;i<BoDong_Num;i++)
        {
            if (!BoDong_isOn)
                break;
            List<long> record = new List<long>();
            uIcontroller.ShowInf("[1;36mÕýÔÚ½øÐÐ50´ÎÑ°ÕÒ¡£¡£¡£[0;0m");
            JS.SendMsg("qntiqu 1 qn");
            Thread.Sleep(300);
            for (int j=0;j<5;j++)
            {
                SharedVariables.Out = "";
                string n = "research " + BoDong_Skill + " 1\n";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                MatchCollection a = Regex.Matches(SharedVariables.Out, "ÒªÇ±ÄÜ.*?µã¡£");
                for(int k=0;k<a.Count;k++)
                {
                    long temp = long.Parse(TextGainCenter("ÒªÇ±ÄÜ", "µã¡£", a[k].Value));
                    record.Add(temp);
                }

            }
            long potential = record.Min();
            
            long left = potential - SharedVariables.Potential;
            JS.SendMsg("qntiqu " + left + " qn");
            float overTime = 0f;
            uIcontroller.ShowInf("[1;36mÕýÔÚ³¢ÊÔÒÔ×îÐ¡Ç±ÄÜÑÐ¾¿¡£¡£¡£[0;0m");
            while (SharedVariables.Potential < potential)
            {
                Thread.Sleep(300);
                overTime += 0.3f;
                if (overTime > 10)
                    break;
            }
            if (overTime > 10) continue;
            while (true)
            {
                SharedVariables.Out = "";
                string n = "research " + BoDong_Skill + " 1\n";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                string temp = SharedVariables.Out;
                if (temp.Contains("¹§Ï²£º"))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m³É¹¦ÌáÉý" + (i + 1).ToString() + "¼¶¼¼ÄÜ£¡£¡£¡[0;0m");
            record.Clear();
            Thread.Sleep(1000);
        }
        ThreadList.Remove("BoDong2");
    }

    public void LianGong()
    {
        TMP_Text butText = GameObject.Find("LianGongStartButton").GetComponentInChildren<TMP_Text>();
        
        int set = GameObject.Find("LianGongSetting").GetComponent<TMP_Dropdown>().value;
        string text = GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text;
        PlayerPrefs.SetInt("LianGongSetting", set);
        PlayerPrefs.SetString("LianGongSetting" + set, text);
        if (butText.text == "¿ªÊ¼Á·¹¦")
        {
            butText.text = "½áÊøÁ·¹¦";
            uIcontroller.ShowInf("[1;36m¿ªÊ¼×Ô¶¯Á·¹¦£¡£¡£¡[0;0m");
            string str = GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text;
            LianGongThread = new Thread(new ParameterizedThreadStart(LianGong_main));
            LianGongThread.Start(str);
            uIcontroller.CloseScript();
        }
        else
        {
            butText.text = "¿ªÊ¼Á·¹¦";
            LianGongThread.Abort();
            JS.SendMsg("halt\nhalt");
            uIcontroller.ShowInf("[1;36m½áÊø×Ô¶¯Á·¹¦£¡£¡£¡[0;0m");
            uIcontroller.CloseScript();
        }
        

    }

    public void LianGong_Change()
    {
        int value = GameObject.Find("LianGongSetting").GetComponent<TMP_Dropdown>().value;
        string text = PlayerPrefs.GetString("LianGongSetting" + value,"");
        GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text = text;
    }

    private void LianGong_main(object str)
    {
        string temp = str.ToString();
        var temp1 = temp.Split("\n");
        List<string> baseSkill = new List<string>();
        List<List<string>> specialSkill = new List<List<string>>();
        if (temp.Length == 0)
            return;
        long HP_LG = SharedVariables.HP_MAX;
        long Force_LG = SharedVariables.Force_MAX;
        long blood;
        if(HP_LG * 0.7 >Force_LG)
        {
            blood = (long)Math.Ceiling(Force_LG * 0.9);
        }
        else
        {
            blood = (long)Math.Ceiling(HP_LG * 0.7);
        }
        for (int i = 0; i < temp1.Length-1; i++)
        {
            var temp2 = temp1[i].Split(":");
            if (temp2.Length == 0)
                return;
            baseSkill.Add(temp2[0]);
            var temp3 = temp2[1].Split("#");
            if (temp3.Length == 0)
                return;
            specialSkill.Add(new List<string>(temp3));
        }
        string swordName = temp1[temp1.Length - 1].Split("#")[0];
        string bladeName = temp1[temp1.Length - 1].Split("#")[1];
        uIcontroller.ShowInf("[1;36m¼´½«¿ªÊ¼Á·¹¦¡£¡£¡£[0;0m");
        for (int j=0;j<baseSkill.Count;j++)
        {
            uIcontroller.ShowInf("[1;36mÇÐ»»»ù´¡Îä¹¦Îª" + baseSkill[j] + "[0;0m");
            JS.SendMsg("alias ×ÔÁ·¹¦ exert recover;lian " + baseSkill[j] + " 9999;dazuo " + blood + "\nset sign5 ×ÔÁ·¹¦\nset sign1 1");
            for(int k=0;k<specialSkill[j].Count;k++)
            {
                uIcontroller.ShowInf("[1;36mÇÐ»»ÌØÊâÎä¹¦Îª" + specialSkill[j][k] + "[0;0m");
                JS.SendMsg("jifa " + baseSkill[j] + " " + specialSkill[j][k]);
                SharedVariables.Out = "";
                int thingsInHand = 0;
                while (true)
                {
                    string res = SharedVariables.Out;
                    if(res.Length>300)
                    {
                        string endres = res.Substring(res.Length - 300);
                        if (endres.Contains("»ðºò²»¹»£¬ÄÑÒÔ¼ÌÐøÌáÉýÄã"))
                        {
                            break;
                        }
                        if(endres.Contains("Äã±ØÐëÏÈÕÒÒ»°Ñ½£²ÅÄÜÁ·½£·¨"))
                        {
                            JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄ½£[0;0m");
                            thingsInHand = 1;
                        }
                        if (endres.Contains("Äã±ØÐëÏÈÕÒÒ»°Ñµ¶²ÅÄÜ"))
                        {
                            JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄµ¶[0;0m");
                            thingsInHand = 2;
                        }
                        if (endres.Contains("±ØÐë¿ÕÊÖ"))
                        {
                            JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                            thingsInHand = 0;
                        }
                        if (endres.Contains("Á·ÁËÁãÌË")&& !endres.Contains("ÄãµÄÄÚÁ¦²»¹»") && !endres.Contains("ÄãÏÖÔÚµÄÆøÌ«ÉÙÁË"))
                        {
                            switch(thingsInHand)
                            {
                                case 0:
                                    thingsInHand = 1;
                                    JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄ½£[0;0m");
                                    break;
                                case 1:
                                    thingsInHand = 2;
                                    JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄµ¶[0;0m");
                                    break;
                                case 2:
                                    thingsInHand = 0;
                                    JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                                    break;

                            }
                        }
                    }
                    else
                    {
                        if (res.Contains("»ðºò²»¹»£¬ÄÑÒÔ¼ÌÐøÌáÉýÄã"))
                        {
                            break;
                        }
                        if (res.Contains("Äã±ØÐëÏÈÕÒÒ»°Ñ½£²ÅÄÜÁ·½£·¨"))
                        {
                            thingsInHand = 1;
                            JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄ½£[0;0m");
                        }
                        if (res.Contains("Äã±ØÐëÏÈÕÒÒ»°Ñµ¶²ÅÄÜ"))
                        {
                            thingsInHand = 2;
                            JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄµ¶[0;0m");
                        }
                        if (res.Contains("±ØÐë¿ÕÊÖ"))
                        {
                            thingsInHand = 0;
                            JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                        }
                        if (res.Contains("Á·ÁËÁãÌË") && !res.Contains("ÄãµÄÄÚÁ¦²»¹»") && !res.Contains("ÄãÏÖÔÚµÄÆøÌ«ÉÙÁË"))
                        {
                            switch (thingsInHand)
                            {
                                case 0:
                                    thingsInHand = 1;
                                    JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄ½£[0;0m");
                                    break;
                                case 1:
                                    thingsInHand = 2;
                                    JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36mÕÙ»½ÄãµÄµ¶[0;0m");
                                    break;
                                case 2:
                                    thingsInHand = 0;
                                    JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                                    break;

                            }
                        }
                    }
                    Thread.Sleep(500);
                }
            }
        }

        
    }


    public void WoLongBao()
    {
        
        Thread WoLongBao = new Thread(WoLongBao_main);
        WoLongBao.Start();
        uIcontroller.ShowInf("[1;36mÆô¶¯ÎÔÁú±¤¸±±¾[0;0m");
        uIcontroller.CloseScript();

    }



    public void HuangLing()
    {
        Thread HuangLing = new Thread(HuangLing_main);
        HuangLing.Start();
        uIcontroller.ShowInf("[1;36mÆô¶¯»ÊÁê¸±±¾[0;0m");
        uIcontroller.CloseScript();
    }

    private void HuangLing_main()
    {
        HuangLing_temp("east");
        HuangLing_temp("east");
        HuangLing_temp("north");
        HuangLing_temp("west");
        HuangLing_temp("e\nn");
        HuangLing_temp("west");
        HuangLing_temp("e\ne");
        HuangLing_temp("w\ns\ns\nw\ns");
        HuangLing_temp("south");
        HuangLing_temp("n\ne");
        HuangLing_temp("south");
        HuangLing_temp("east");
        HuangLing_temp("east");
        HuangLing_temp("east");
        HuangLing_temp("w\nw\nn");
        HuangLing_temp("west");
        JS.SendMsg("n\nout");
    }
    private void HuangLing_temp(string direction,int num = 1)
    {
        JS.SendMsg(direction);
        Thread.Sleep(2000);
        while(!CheckDeathNumber(num))
            Thread.Sleep(500);
        Thread.Sleep(1000);
        while (SharedVariables.Busy != 0)
            Thread.Sleep(500);
        Thread.Sleep(1000);
    }

    private string[] GetAllIDInRoom()
    {
        string temp = SharedVariables.Obj;
        var temp1 = temp.Split("%SV%");
        string[] res = new string[temp1.Length-1];
        for(int i=0;i<res.Length;i++)
        {
            var a = temp1[i].Split(":");
            res[i] = a[1].Substring(5);
            
        }
        return res;
    }

    private void killAll()
    {
        string[] enemy = GetAllIDInRoom();
        string msg = "";
        for (int i = 0; i < enemy.Length; i++)
        {
            if(enemy[i] != "" && enemy[i] != " ")
                msg += "kill " + enemy[i] + "\n";
        }
        msg = msg.Substring(0, msg.Length - 1);
        JS.SendMsg(msg);
    }


    private void WoLongBao_main()
    {
        while(true)
        {
            if(SharedVariables.Here == "ÎÔÁúµÀ")
            {
                JS.SendMsg("north");
                Thread.Sleep(500);
                continue;
            }
            else if(SharedVariables.Here == "´óÃÅ" || SharedVariables.Here == "´óÔº" ||SharedVariables.Here == "Î÷Ïá·¿" ||
                SharedVariables.Here == "¶«Ïá·¿" || SharedVariables.Here == "Î÷Á·Îä³¡" || SharedVariables.Here == "¶«Á·Îä³¡")
            {
                while (true)
                {
                    killAll();
                    if (CheckDeathNumber(4))
                        break;
                    Thread.Sleep(2000);
                }
            }
            else if(SharedVariables.Here == "Á·Îä³¡" || SharedVariables.Here == "´óÌü" || SharedVariables.Here == "ºóÔº")
            {
                while (true)
                {
                    killAll();
                    if (CheckDeathNumber(5))
                        break;
                    Thread.Sleep(2000);
                }
            }

            if(SharedVariables.Here == "´óÃÅ" || SharedVariables.Here == "´óÌü")
            {
                while(SharedVariables.Busy !=0)
                    Thread.Sleep(500);
                JS.SendMsg("north");
            }
            else if (SharedVariables.Here == "´óÔº" || SharedVariables.Here == "Á·Îä³¡")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("west");
            }
            else if (SharedVariables.Here == "Î÷Ïá·¿" || SharedVariables.Here == "Î÷Á·Îä³¡")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("east\neast");
            }
            else if (SharedVariables.Here == "¶«Ïá·¿" || SharedVariables.Here == "¶«Á·Îä³¡")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("west\nnorth");
            }
            else if (SharedVariables.Here == "ºóÔº")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(1000);
                JS.SendMsg("s\ns\ns\ns\ns\nout\nd\nw");
                break;
            }

            Thread.Sleep(1000);

        }
    }

    public void YuLuoSha()
    {
        TMP_Text temp = GameObject.Find("YuLuoShaButton").GetComponentInChildren<TMP_Text>();
        if(temp.text == "ÓñÂÞÉ²¿ª")
        {
            temp.text = "ÓñÂÞÉ²¹Ø";
            YuLuoShaFuBen = new Thread(YuLuoSha_main);
            YuLuoShaFuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "ÓñÂÞÉ²¿ª";
            YuLuoShaFuBen.Abort();
            uIcontroller.CloseScript();
        }
    }

    private void YuLuoSha_main()
    {
        while(true)
        {
            JS.SendMsg("fly guanwai\nn\nask xiu cai about ÓñÂÞÉ²");
            Thread.Sleep(500);
            if (!OpenFuBen())
                continue;
            Thread.Sleep(500);
            while (!CheckDeathNumber(1))
                Thread.Sleep(500);
            JS.SendMsg("out\ndown\nw");
            Thread.Sleep(500);
        }
    }


    public void DuoBao()
    {
        TMP_Text temp = GameObject.Find("DuoBaoButton").GetComponentInChildren<TMP_Text>();
        if (temp.text == "×¼±¸¶á±¦")
        {
            temp.text = "½áÊø¶á±¦";
            DuoBaoFuBen = new Thread(DuoBao_wait);
            DuoBaoFuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "×¼±¸¶á±¦";
            uIcontroller.ShowInf("[1;36mÖÕÖ¹¶á±¦[0;0m");
            DuoBaoFuBen.Abort();
        }
    }

    private void DuoBao_wait()
    {
        JS.SendMsg("fly gw\nn\nn");
        Thread.Sleep(500);
        uIcontroller.ShowInf("[1;36mÕýÔÚµÈ´ý¶á±¦¿ªÊ¼£¬ÒÆ¶¯×Ô¶¯È¡Ïû¶á±¦[0;0m");
        while (true)
        {
            if(!SharedVariables.Here.Contains("¶á±¦¿Ú"))
            {
                if(SharedVariables.Here.Contains("·¿¼ä"))
                {
                    uIcontroller.ShowInf("[1;36m¿ªÊ¼¶á±¦[0;0m");
                    DuoBao_main();
                    break;
                }
                else
                {
                    uIcontroller.ShowInf("[1;36mÖÕÖ¹¶á±¦[0;0m");
                    break;
                }
            }
            Thread.Sleep(1000);
        }
    }

    private void DuoBao_main()
    {
        while(SharedVariables.Here.Contains("·¿¼ä"))
        {
            var temp = SharedVariables.Direction.Split("  ");
            System.Random r = new System.Random();
            int i = r.Next(0, temp.Length);
            JS.SendMsg(temp[i]);
            Thread.Sleep(100);
            JS.SendMsg("wa");
            Thread.Sleep(500);
        }
    }
    

    private bool OpenFuBen()
    {
        string id = SharedVariables.ID;
        if (id == "")
            return false;
        JS.SendMsg("fuben");
        Thread.Sleep(1000);
        Match temp = Regex.Match(SharedVariables.Hudong_Buttons, "fuben " + id + "\\w*");
        if (!temp.Success)
            return false;
        JS.SendMsg(temp.Value);
        return true;


    }


    /// <summary>
    /// ÅÐ¶Ïµ±Ç°³¡¾°µÄÊ¬Ìå¸öÊý
    /// </summary>
    /// <param name="name">Ä¿±êÃû×Ö</param>
    /// <param name="number">Ä¿±êÊýÁ¿</param>
    /// <returns>ÊÇ·ñËÀÍö</returns>
    private bool CheckDeathNumber(int number)
    {
        if(Regex.Matches(SharedVariables.Obj,"µÄÊ¬Ìå").Count == number)
        {
            return true;
        }
        return false;
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

}


