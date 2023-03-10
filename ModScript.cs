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

    private string ShiMen_north = "金水桥 天山山路 刀店 大将军府 西街 易溪部 青龙街 碎石路 乌蒙村落 东大街 西大街 西镇街 东城 土地庙 打铁铺";
    private string ShiMen_south = "崇圣门 西村口 夺宝口 西大街 乌蒙村落 大竹楼 国子监 西街 白河 渔村小屋 西大街 碎石路 北内大街 一品堂大门 小院子 大将军府 楼梯";
    private string ShiMen_west = "东内大街 南大街 青石大道 昆仑山下 碎石路 苍山 永泰大道 南安大道 北大街 步雄部 忠烈祠 觅香楼";
    private string ShiMen_east = "北街 青石大道 南大街 沿湖大道 镇雄 赌场 刘庄 富家侧门";
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
                ShiMenInf.GetComponent<TMP_Text>().text = "师门已经全部完成！";
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
        
        if(StartButton.GetComponentInChildren<TMP_Text>().text == "开始")
        {
            StartButton.GetComponentInChildren<TMP_Text>().text = "结束";
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
        if (!SharedVariables.Hudong_Buttons.Contains("踢出队伍"))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "您需要先组队才能进行师门";
            yield break;
        }
        SharedVariables.Out = "";
        JS.SendMsg("exert recover\nexert regenerate\naccept quest\nquest\nuse ba gua");
        yield return new WaitForSeconds(1);
        if(!int.TryParse(TextGainCenter("本周还可领取师门任务次数：", "次", SharedVariables.Out),out ShiMen_Number_left))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "请不要在领任务后点击本按钮！";
            yield break;
        }
        if(ShiMen_Number_left == 0)
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "请不要在任务全部完成后后点击本按钮！";
            yield break;
        }
        ShiMenInf.GetComponent<TMP_Text>().text = "您已经符合要求了！请<color=red>领取师门任务</color>以后再点击开始！";
        CheckBut.GetComponent<Button>().interactable = false;
        StartButton.GetComponent<Button>().interactable = true;
        StartButton.GetComponentInChildren<TMP_Text>().text = "开始";

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
        if (!str.Contains("使用八卦盘占得在户外"))
        {
            string temp = TextGainCenter("之前割下", "的人头", str);
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

        if (here == "青羊宫")
            JS.SendMsg("northeast");
        else if (here == "玄坛庙")
            JS.SendMsg("southwest");
        else if (here == "中岳大殿")
            JS.SendMsg("northup");
        else if (here == "兵器铺")
            JS.SendMsg("southeast\nwest");
        else if (here == "民房")
            JS.SendMsg("northeast");
        else if (here == "竹楼")
            JS.SendMsg("down");
        else if (here == "宏圣寺塔" || here == "塔基")
            JS.SendMsg("d\nd\nout\nw");
        else if (here == "戈壁滩")
            JS.SendMsg("n\nw");
        else if (here == "祭祀大屋" || here == "大厅" || here == "山神庙")
            JS.SendMsg("out");
        else if (here == "祭祀屋")
            JS.SendMsg("d\nd\nout");
        else if (here == "议事堂")
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
            if(SharedVariables.Obj.IndexOf(ShiMen_Name+"的尸体") != -1)
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
        
        string temp = "已完成 " + ShiMen_Number+ "  个师门\n还剩 " + ShiMen_Number_left
            + "个师门\n师门速度 " + Math.Round(speed, 1)
            + " 个/分钟\n全部完成还需 " + Math.Round(time_left) + " 分钟";
        TaskQueue.Enqueue("<002>" + temp);
        
    }

    public void SavePotential()
    {
        JS.SendMsg("qncunru " + SharedVariables.Potential.ToString() + " qn");
    }

    public void BoDong1()
    {
        GameObject e = GameObject.Find("BoDongStart1");
        if(e.GetComponentInChildren<TMP_Text>().text == "开始")
        {
            if (ThreadList.Contains("BoDong1"))
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            BoDong1_Potential = int.Parse(GameObject.Find("potential1").GetComponent<TMP_InputField>().text);
            newBodong1 = new Thread(BoDong1_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "结束";
            newBodong1.Start();
            uIcontroller.ShowInf("[1;36m开始模式一提升技能！！![0;0m");
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "开始";
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
                if(temp.Contains("恭喜："))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m成功提升" + (i + 1).ToString() + "级技能！！！[0;0m");
            Thread.Sleep(1000);
        }
        ThreadList.Remove("BoDong1");
    }

    


    public void BoDong2()
    {
        
        GameObject e = GameObject.Find("BoDongStart2");
        if (e.GetComponentInChildren<TMP_Text>().text == "开始")
        {
            if (ThreadList.Contains("BoDong2"))
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            Thread newBoDong2 = new Thread(BoDong2_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "结束";
            newBoDong2.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "开始";
            BoDong_isOn = false;
        }
        
    }

    public void BoDong2_Main()
    {
        ThreadList.Add("BoDong2");
        uIcontroller.ShowInf("[1;36m开始模式二提升技能！！![0;0m");
        for (int i=0;i<BoDong_Num;i++)
        {
            if (!BoDong_isOn)
                break;
            List<long> record = new List<long>();
            uIcontroller.ShowInf("[1;36m正在进行50次寻找。。。[0;0m");
            JS.SendMsg("qntiqu 1 qn");
            Thread.Sleep(300);
            for (int j=0;j<5;j++)
            {
                SharedVariables.Out = "";
                string n = "research " + BoDong_Skill + " 1\n";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                MatchCollection a = Regex.Matches(SharedVariables.Out, "要潜能.*?点。");
                for(int k=0;k<a.Count;k++)
                {
                    long temp = long.Parse(TextGainCenter("要潜能", "点。", a[k].Value));
                    record.Add(temp);
                }

            }
            long potential = record.Min();
            
            long left = potential - SharedVariables.Potential;
            JS.SendMsg("qntiqu " + left + " qn");
            float overTime = 0f;
            uIcontroller.ShowInf("[1;36m正在尝试以最小潜能研究。。。[0;0m");
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
                if (temp.Contains("恭喜："))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m成功提升" + (i + 1).ToString() + "级技能！！！[0;0m");
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
        if (butText.text == "开始练功")
        {
            butText.text = "结束练功";
            uIcontroller.ShowInf("[1;36m开始自动练功！！！[0;0m");
            string str = GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text;
            LianGongThread = new Thread(new ParameterizedThreadStart(LianGong_main));
            LianGongThread.Start(str);
            uIcontroller.CloseScript();
        }
        else
        {
            butText.text = "开始练功";
            LianGongThread.Abort();
            JS.SendMsg("halt\nhalt");
            uIcontroller.ShowInf("[1;36m结束自动练功！！！[0;0m");
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
        uIcontroller.ShowInf("[1;36m即将开始练功。。。[0;0m");
        for (int j=0;j<baseSkill.Count;j++)
        {
            uIcontroller.ShowInf("[1;36m切换基础武功为" + baseSkill[j] + "[0;0m");
            JS.SendMsg("alias 自练功 exert recover;lian " + baseSkill[j] + " 9999;dazuo " + blood + "\nset sign5 自练功\nset sign1 1");
            for(int k=0;k<specialSkill[j].Count;k++)
            {
                uIcontroller.ShowInf("[1;36m切换特殊武功为" + specialSkill[j][k] + "[0;0m");
                JS.SendMsg("jifa " + baseSkill[j] + " " + specialSkill[j][k]);
                SharedVariables.Out = "";
                int thingsInHand = 0;
                while (true)
                {
                    string res = SharedVariables.Out;
                    if(res.Length>300)
                    {
                        string endres = res.Substring(res.Length - 300);
                        if (endres.Contains("火候不够，难以继续提升你"))
                        {
                            break;
                        }
                        if(endres.Contains("你必须先找一把剑才能练剑法"))
                        {
                            JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m召唤你的剑[0;0m");
                            thingsInHand = 1;
                        }
                        if (endres.Contains("你必须先找一把刀才能"))
                        {
                            JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m召唤你的刀[0;0m");
                            thingsInHand = 2;
                        }
                        if (endres.Contains("必须空手"))
                        {
                            JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                            thingsInHand = 0;
                        }
                        if (endres.Contains("练了零趟")&& !endres.Contains("你的内力不够") && !endres.Contains("你现在的气太少了"))
                        {
                            switch(thingsInHand)
                            {
                                case 0:
                                    thingsInHand = 1;
                                    JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m召唤你的剑[0;0m");
                                    break;
                                case 1:
                                    thingsInHand = 2;
                                    JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m召唤你的刀[0;0m");
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
                        if (res.Contains("火候不够，难以继续提升你"))
                        {
                            break;
                        }
                        if (res.Contains("你必须先找一把剑才能练剑法"))
                        {
                            thingsInHand = 1;
                            JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m召唤你的剑[0;0m");
                        }
                        if (res.Contains("你必须先找一把刀才能"))
                        {
                            thingsInHand = 2;
                            JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m召唤你的刀[0;0m");
                        }
                        if (res.Contains("必须空手"))
                        {
                            thingsInHand = 0;
                            JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                        }
                        if (res.Contains("练了零趟") && !res.Contains("你的内力不够") && !res.Contains("你现在的气太少了"))
                        {
                            switch (thingsInHand)
                            {
                                case 0:
                                    thingsInHand = 1;
                                    JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m召唤你的剑[0;0m");
                                    break;
                                case 1:
                                    thingsInHand = 2;
                                    JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m召唤你的刀[0;0m");
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
        uIcontroller.ShowInf("[1;36m启动卧龙堡副本[0;0m");
        uIcontroller.CloseScript();

    }



    public void HuangLing()
    {
        Thread HuangLing = new Thread(HuangLing_main);
        HuangLing.Start();
        uIcontroller.ShowInf("[1;36m启动皇陵副本[0;0m");
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
            if(SharedVariables.Here == "卧龙道")
            {
                JS.SendMsg("north");
                Thread.Sleep(500);
                continue;
            }
            else if(SharedVariables.Here == "大门" || SharedVariables.Here == "大院" ||SharedVariables.Here == "西厢房" ||
                SharedVariables.Here == "东厢房" || SharedVariables.Here == "西练武场" || SharedVariables.Here == "东练武场")
            {
                while (true)
                {
                    killAll();
                    if (CheckDeathNumber(4))
                        break;
                    Thread.Sleep(2000);
                }
            }
            else if(SharedVariables.Here == "练武场" || SharedVariables.Here == "大厅" || SharedVariables.Here == "后院")
            {
                while (true)
                {
                    killAll();
                    if (CheckDeathNumber(5))
                        break;
                    Thread.Sleep(2000);
                }
            }

            if(SharedVariables.Here == "大门" || SharedVariables.Here == "大厅")
            {
                while(SharedVariables.Busy !=0)
                    Thread.Sleep(500);
                JS.SendMsg("north");
            }
            else if (SharedVariables.Here == "大院" || SharedVariables.Here == "练武场")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("west");
            }
            else if (SharedVariables.Here == "西厢房" || SharedVariables.Here == "西练武场")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("east\neast");
            }
            else if (SharedVariables.Here == "东厢房" || SharedVariables.Here == "东练武场")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("west\nnorth");
            }
            else if (SharedVariables.Here == "后院")
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
        if(temp.text == "玉罗刹开")
        {
            temp.text = "玉罗刹关";
            YuLuoShaFuBen = new Thread(YuLuoSha_main);
            YuLuoShaFuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "玉罗刹开";
            YuLuoShaFuBen.Abort();
            uIcontroller.CloseScript();
        }
    }

    private void YuLuoSha_main()
    {
        while(true)
        {
            JS.SendMsg("fly guanwai\nn\nask xiu cai about 玉罗刹");
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
        if (temp.text == "准备夺宝")
        {
            temp.text = "结束夺宝";
            DuoBaoFuBen = new Thread(DuoBao_wait);
            DuoBaoFuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "准备夺宝";
            uIcontroller.ShowInf("[1;36m终止夺宝[0;0m");
            DuoBaoFuBen.Abort();
        }
    }

    private void DuoBao_wait()
    {
        JS.SendMsg("fly gw\nn\nn");
        Thread.Sleep(500);
        uIcontroller.ShowInf("[1;36m正在等待夺宝开始，移动自动取消夺宝[0;0m");
        while (true)
        {
            if(!SharedVariables.Here.Contains("夺宝口"))
            {
                if(SharedVariables.Here.Contains("房间"))
                {
                    uIcontroller.ShowInf("[1;36m开始夺宝[0;0m");
                    DuoBao_main();
                    break;
                }
                else
                {
                    uIcontroller.ShowInf("[1;36m终止夺宝[0;0m");
                    break;
                }
            }
            Thread.Sleep(1000);
        }
    }

    private void DuoBao_main()
    {
        while(SharedVariables.Here.Contains("房间"))
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
    /// 判断当前场景的尸体个数
    /// </summary>
    /// <param name="name">目标名字</param>
    /// <param name="number">目标数量</param>
    /// <returns>是否死亡</returns>
    private bool CheckDeathNumber(int number)
    {
        if(Regex.Matches(SharedVariables.Obj,"的尸体").Count == number)
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


