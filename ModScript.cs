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

    private string ShiMen_north = "��ˮ�� ��ɽɽ· ���� �󽫾��� ���� ��Ϫ�� ������ ��ʯ· ���ɴ��� ����� ����� ����� ���� ������ ������";
    private string ShiMen_south = "��ʥ�� ����� �ᱦ�� ����� ���ɴ��� ����¥ ���Ӽ� ���� �׺� ���С�� ����� ��ʯ· ���ڴ�� һƷ�ô��� СԺ�� �󽫾��� ¥��";
    private string ShiMen_west = "���ڴ�� �ϴ�� ��ʯ��� ����ɽ�� ��ʯ· ��ɽ ��̩��� �ϰ���� ����� ���۲� ������ ����¥";
    private string ShiMen_east = "���� ��ʯ��� �ϴ�� �غ���� ���� �ĳ� ��ׯ ���Ҳ���";
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
                ShiMenInf.GetComponent<TMP_Text>().text = "ʦ���Ѿ�ȫ����ɣ�";
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
        
        if(StartButton.GetComponentInChildren<TMP_Text>().text == "��ʼ")
        {
            StartButton.GetComponentInChildren<TMP_Text>().text = "����";
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
        if (!SharedVariables.Hudong_Buttons.Contains("�߳�����"))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "����Ҫ����Ӳ��ܽ���ʦ��";
            yield break;
        }
        SharedVariables.Out = "";
        JS.SendMsg("exert recover\nexert regenerate\naccept quest\nquest\nuse ba gua");
        yield return new WaitForSeconds(1);
        if(!int.TryParse(TextGainCenter("���ܻ�����ȡʦ�����������", "��", SharedVariables.Out),out ShiMen_Number_left))
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "�벻Ҫ���������������ť��";
            yield break;
        }
        if(ShiMen_Number_left == 0)
        {
            ShiMenInf.GetComponent<TMP_Text>().text = "�벻Ҫ������ȫ����ɺ��������ť��";
            yield break;
        }
        ShiMenInf.GetComponent<TMP_Text>().text = "���Ѿ�����Ҫ���ˣ���<color=red>��ȡʦ������</color>�Ժ��ٵ����ʼ��";
        CheckBut.GetComponent<Button>().interactable = false;
        StartButton.GetComponent<Button>().interactable = true;
        StartButton.GetComponentInChildren<TMP_Text>().text = "��ʼ";

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
        if (!str.Contains("ʹ�ð�����ռ���ڻ���"))
        {
            string temp = TextGainCenter("֮ǰ����", "����ͷ", str);
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

        if (here == "����")
            JS.SendMsg("northeast");
        else if (here == "��̳��")
            JS.SendMsg("southwest");
        else if (here == "�������")
            JS.SendMsg("northup");
        else if (here == "������")
            JS.SendMsg("southeast\nwest");
        else if (here == "��")
            JS.SendMsg("northeast");
        else if (here == "��¥")
            JS.SendMsg("down");
        else if (here == "��ʥ����" || here == "����")
            JS.SendMsg("d\nd\nout\nw");
        else if (here == "���̲")
            JS.SendMsg("n\nw");
        else if (here == "�������" || here == "����" || here == "ɽ����")
            JS.SendMsg("out");
        else if (here == "������")
            JS.SendMsg("d\nd\nout");
        else if (here == "������")
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
            if(SharedVariables.Obj.IndexOf(ShiMen_Name+"��ʬ��") != -1)
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
        
        string temp = "����� " + ShiMen_Number+ "  ��ʦ��\n��ʣ " + ShiMen_Number_left
            + "��ʦ��\nʦ���ٶ� " + Math.Round(speed, 1)
            + " ��/����\nȫ����ɻ��� " + Math.Round(time_left) + " ����";
        TaskQueue.Enqueue("<002>" + temp);
        
    }

    public void SavePotential()
    {
        JS.SendMsg("qncunru " + SharedVariables.Potential.ToString() + " qn");
    }

    public void BoDong1()
    {
        GameObject e = GameObject.Find("BoDongStart1");
        if(e.GetComponentInChildren<TMP_Text>().text == "��ʼ")
        {
            if (ThreadList.Contains("BoDong1"))
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            BoDong1_Potential = int.Parse(GameObject.Find("potential1").GetComponent<TMP_InputField>().text);
            newBodong1 = new Thread(BoDong1_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "����";
            newBodong1.Start();
            uIcontroller.ShowInf("[1;36m��ʼģʽһ�������ܣ���![0;0m");
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "��ʼ";
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
                if(temp.Contains("��ϲ��"))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m�ɹ�����" + (i + 1).ToString() + "�����ܣ�����[0;0m");
            Thread.Sleep(1000);
        }
        ThreadList.Remove("BoDong1");
    }

    


    public void BoDong2()
    {
        
        GameObject e = GameObject.Find("BoDongStart2");
        if (e.GetComponentInChildren<TMP_Text>().text == "��ʼ")
        {
            if (ThreadList.Contains("BoDong2"))
                return;
            BoDong_Skill = GameObject.Find("BoDongSkill").GetComponent<TMP_InputField>().text;
            BoDong_Num = int.Parse(GameObject.Find("BoDongNum").GetComponent<TMP_InputField>().text);
            Thread newBoDong2 = new Thread(BoDong2_Main);
            BoDong_isOn = true;
            e.GetComponentInChildren<TMP_Text>().text = "����";
            newBoDong2.Start();
            uIcontroller.CloseScript();
        }
        else
        {
            e.GetComponentInChildren<TMP_Text>().text = "��ʼ";
            BoDong_isOn = false;
        }
        
    }

    public void BoDong2_Main()
    {
        ThreadList.Add("BoDong2");
        uIcontroller.ShowInf("[1;36m��ʼģʽ���������ܣ���![0;0m");
        for (int i=0;i<BoDong_Num;i++)
        {
            if (!BoDong_isOn)
                break;
            List<long> record = new List<long>();
            uIcontroller.ShowInf("[1;36m���ڽ���50��Ѱ�ҡ�����[0;0m");
            JS.SendMsg("qntiqu 1 qn");
            Thread.Sleep(300);
            for (int j=0;j<5;j++)
            {
                SharedVariables.Out = "";
                string n = "research " + BoDong_Skill + " 1\n";
                JS.SendMsg(n + n + n + n + n + n + n + n + n + "research " + BoDong_Skill + " 1");
                Thread.Sleep(1000);
                MatchCollection a = Regex.Matches(SharedVariables.Out, "ҪǱ��.*?�㡣");
                for(int k=0;k<a.Count;k++)
                {
                    long temp = long.Parse(TextGainCenter("ҪǱ��", "�㡣", a[k].Value));
                    record.Add(temp);
                }

            }
            long potential = record.Min();
            
            long left = potential - SharedVariables.Potential;
            JS.SendMsg("qntiqu " + left + " qn");
            float overTime = 0f;
            uIcontroller.ShowInf("[1;36m���ڳ�������СǱ���о�������[0;0m");
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
                if (temp.Contains("��ϲ��"))
                {
                    break;
                }
            }
            uIcontroller.ShowInf("[1;36m�ɹ�����" + (i + 1).ToString() + "�����ܣ�����[0;0m");
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
        if (butText.text == "��ʼ����")
        {
            butText.text = "��������";
            uIcontroller.ShowInf("[1;36m��ʼ�Զ�����������[0;0m");
            string str = GameObject.Find("LianGongInput").GetComponent<TMP_InputField>().text;
            LianGongThread = new Thread(new ParameterizedThreadStart(LianGong_main));
            LianGongThread.Start(str);
            uIcontroller.CloseScript();
        }
        else
        {
            butText.text = "��ʼ����";
            LianGongThread.Abort();
            JS.SendMsg("halt\nhalt");
            uIcontroller.ShowInf("[1;36m�����Զ�����������[0;0m");
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
        uIcontroller.ShowInf("[1;36m������ʼ����������[0;0m");
        for (int j=0;j<baseSkill.Count;j++)
        {
            uIcontroller.ShowInf("[1;36m�л������书Ϊ" + baseSkill[j] + "[0;0m");
            JS.SendMsg("alias ������ exert recover;lian " + baseSkill[j] + " 9999;dazuo " + blood + "\nset sign5 ������\nset sign1 1");
            for(int k=0;k<specialSkill[j].Count;k++)
            {
                uIcontroller.ShowInf("[1;36m�л������书Ϊ" + specialSkill[j][k] + "[0;0m");
                JS.SendMsg("jifa " + baseSkill[j] + " " + specialSkill[j][k]);
                SharedVariables.Out = "";
                int thingsInHand = 0;
                while (true)
                {
                    string res = SharedVariables.Out;
                    if(res.Length>300)
                    {
                        string endres = res.Substring(res.Length - 300);
                        if (endres.Contains("��򲻹������Լ���������"))
                        {
                            break;
                        }
                        if(endres.Contains("���������һ�ѽ�����������"))
                        {
                            JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m�ٻ���Ľ�[0;0m");
                            thingsInHand = 1;
                        }
                        if (endres.Contains("���������һ�ѵ�����"))
                        {
                            JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m�ٻ���ĵ�[0;0m");
                            thingsInHand = 2;
                        }
                        if (endres.Contains("�������"))
                        {
                            JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                            thingsInHand = 0;
                        }
                        if (endres.Contains("��������")&& !endres.Contains("�����������") && !endres.Contains("�����ڵ���̫����"))
                        {
                            switch(thingsInHand)
                            {
                                case 0:
                                    thingsInHand = 1;
                                    JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m�ٻ���Ľ�[0;0m");
                                    break;
                                case 1:
                                    thingsInHand = 2;
                                    JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m�ٻ���ĵ�[0;0m");
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
                        if (res.Contains("��򲻹������Լ���������"))
                        {
                            break;
                        }
                        if (res.Contains("���������һ�ѽ�����������"))
                        {
                            thingsInHand = 1;
                            JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m�ٻ���Ľ�[0;0m");
                        }
                        if (res.Contains("���������һ�ѵ�����"))
                        {
                            thingsInHand = 2;
                            JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                            uIcontroller.ShowInf("[1;36m�ٻ���ĵ�[0;0m");
                        }
                        if (res.Contains("�������"))
                        {
                            thingsInHand = 0;
                            JS.SendMsg("halt\nhalt\ndrop " + swordName + "\ndrop " + bladeName + "\nset sign1 1");
                        }
                        if (res.Contains("��������") && !res.Contains("�����������") && !res.Contains("�����ڵ���̫����"))
                        {
                            switch (thingsInHand)
                            {
                                case 0:
                                    thingsInHand = 1;
                                    JS.SendMsg("halt\nhalt\nsummon " + swordName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m�ٻ���Ľ�[0;0m");
                                    break;
                                case 1:
                                    thingsInHand = 2;
                                    JS.SendMsg("halt\nhalt\nsummon " + bladeName + "\nset sign1 1");
                                    uIcontroller.ShowInf("[1;36m�ٻ���ĵ�[0;0m");
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
        uIcontroller.ShowInf("[1;36m��������������[0;0m");
        uIcontroller.CloseScript();

    }



    public void HuangLing()
    {
        Thread HuangLing = new Thread(HuangLing_main);
        HuangLing.Start();
        uIcontroller.ShowInf("[1;36m�������긱��[0;0m");
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
            if(SharedVariables.Here == "������")
            {
                JS.SendMsg("north");
                Thread.Sleep(500);
                continue;
            }
            else if(SharedVariables.Here == "����" || SharedVariables.Here == "��Ժ" ||SharedVariables.Here == "���᷿" ||
                SharedVariables.Here == "���᷿" || SharedVariables.Here == "�����䳡" || SharedVariables.Here == "�����䳡")
            {
                while (true)
                {
                    killAll();
                    if (CheckDeathNumber(4))
                        break;
                    Thread.Sleep(2000);
                }
            }
            else if(SharedVariables.Here == "���䳡" || SharedVariables.Here == "����" || SharedVariables.Here == "��Ժ")
            {
                while (true)
                {
                    killAll();
                    if (CheckDeathNumber(5))
                        break;
                    Thread.Sleep(2000);
                }
            }

            if(SharedVariables.Here == "����" || SharedVariables.Here == "����")
            {
                while(SharedVariables.Busy !=0)
                    Thread.Sleep(500);
                JS.SendMsg("north");
            }
            else if (SharedVariables.Here == "��Ժ" || SharedVariables.Here == "���䳡")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("west");
            }
            else if (SharedVariables.Here == "���᷿" || SharedVariables.Here == "�����䳡")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("east\neast");
            }
            else if (SharedVariables.Here == "���᷿" || SharedVariables.Here == "�����䳡")
            {
                while (SharedVariables.Busy != 0)
                    Thread.Sleep(500);
                JS.SendMsg("west\nnorth");
            }
            else if (SharedVariables.Here == "��Ժ")
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
        if(temp.text == "����ɲ��")
        {
            temp.text = "����ɲ��";
            YuLuoShaFuBen = new Thread(YuLuoSha_main);
            YuLuoShaFuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "����ɲ��";
            YuLuoShaFuBen.Abort();
            uIcontroller.CloseScript();
        }
    }

    private void YuLuoSha_main()
    {
        while(true)
        {
            JS.SendMsg("fly guanwai\nn\nask xiu cai about ����ɲ");
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
        if (temp.text == "׼���ᱦ")
        {
            temp.text = "�����ᱦ";
            DuoBaoFuBen = new Thread(DuoBao_wait);
            DuoBaoFuBen.Start();
            uIcontroller.CloseScript();

        }
        else
        {
            temp.text = "׼���ᱦ";
            uIcontroller.ShowInf("[1;36m��ֹ�ᱦ[0;0m");
            DuoBaoFuBen.Abort();
        }
    }

    private void DuoBao_wait()
    {
        JS.SendMsg("fly gw\nn\nn");
        Thread.Sleep(500);
        uIcontroller.ShowInf("[1;36m���ڵȴ��ᱦ��ʼ���ƶ��Զ�ȡ���ᱦ[0;0m");
        while (true)
        {
            if(!SharedVariables.Here.Contains("�ᱦ��"))
            {
                if(SharedVariables.Here.Contains("����"))
                {
                    uIcontroller.ShowInf("[1;36m��ʼ�ᱦ[0;0m");
                    DuoBao_main();
                    break;
                }
                else
                {
                    uIcontroller.ShowInf("[1;36m��ֹ�ᱦ[0;0m");
                    break;
                }
            }
            Thread.Sleep(1000);
        }
    }

    private void DuoBao_main()
    {
        while(SharedVariables.Here.Contains("����"))
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
    /// �жϵ�ǰ������ʬ�����
    /// </summary>
    /// <param name="name">Ŀ������</param>
    /// <param name="number">Ŀ������</param>
    /// <returns>�Ƿ�����</returns>
    private bool CheckDeathNumber(int number)
    {
        if(Regex.Matches(SharedVariables.Obj,"��ʬ��").Count == number)
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


