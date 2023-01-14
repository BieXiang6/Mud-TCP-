using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using TMPro;
using System;
using System.Security.Cryptography;
using System.Linq;

public class main : MonoBehaviour
{

    public TcpClient sock;
    public Thread clock;
    public GameObject passText;
    public GameObject idText;
    public GameObject region;
    public GameObject LoginButton;
    public GameObject Notice;
    public GameObject Window;


    private NetworkStream networkStream;
    private UIcontroller uicontroller;
    private string id;
    private string pass;
    private int port;
    private bool isConnected_once = false;
    private Coroutine rotate;



    private Thread ClientTask;

    private void Start()
    {
        StartCoroutine(VersionCheck());
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.runInBackground = true;
        int Performance = PlayerPrefs.GetInt("Performance", 1);
        switch (Performance)
        {
            case 0:
                Application.targetFrameRate = 30;
                break;
            case 1:
                Application.targetFrameRate = 60;
                break;
            case 2:
                Screen.SetResolution(0, 0, true, 0);
                Application.targetFrameRate = 90;
                break;
            case 3:
                Screen.SetResolution(0, 0, true, 0);
                Application.targetFrameRate = 120;
                break;
        }
        uicontroller = GetComponent<UIcontroller>();
        
        idText.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("id");
        passText.GetComponent<TMP_InputField>().text = PlayerPrefs.GetString("pass");
        region.GetComponent<TMP_Dropdown>().value = PlayerPrefs.GetInt("region",0);

        LoginButton.GetComponent<Button>().onClick.AddListener(LoginListener);

    }

    private void LoginListener()
    {
        id = idText.GetComponent<TMP_InputField>().text;
        pass = passText.GetComponent<TMP_InputField>().text;
        StartCoroutine(LoginCheck());
    }


    private void Update()
    {
        if(isConnected_once)
        {
            isConnected_once = false;
            StartCoroutine(Clock());
        }
    }



    void OnDestroy()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        if(sock.Connected)
        {
            sock.Close();
            if(ClientTask.IsAlive)
                ClientTask.Abort();
        }
    }

    public void Connect()
    {
        sock = new TcpClient();
        int option = region.GetComponent<TMP_Dropdown>().value;
        PlayerPrefs.SetInt("region", option);
        switch(option)
        {
            case 0:
                port = 4017;
                break;
            case 1:
                port = 4027;
                break;
            case 2:
                port = 4037;
                break;
            case 3:
                port = 3067;
                break;
            case 4:
                port = 3070;
                break;
            case 5:
                port = 3060;
                break;
            case 6:
                port = 3077;
                break;
            case 7:
                port = 3090;
                break;

        }

        try
        {
            sock.Connect(IPAddress.Parse("154.23.179.24"), port);
            networkStream = sock.GetStream();
            ClientTask = new Thread(ReceiveMessage);
            ClientTask.Start();
            SendMsg("zjmDMaIpOvxdb");
            isConnected_once = true;
            
        }
        catch (Exception e)
        {
            GameObject.Find("LoginInf").GetComponent<TMP_Text>().text = e.Message;

            throw;
        }
        
    }

    IEnumerator Clock()
    {
        while(true)
        {
            Ping a = new Ping("154.23.179.24");
            while (!a.isDone)
            {
                yield return new WaitForSeconds(0.1f);
            }
            SharedVariables.Ping = a.time;
            long ping_part = SharedVariables.Ping;
            string tempText = "";
            if (ping_part < 100 && ping_part > 0)
            {
                tempText = "<color=#669900>" + ping_part + "ms</color>";
            }
            else if (ping_part >= 100 && ping_part < 300)
            {
                tempText = "<color=#FF9900>" + ping_part + "ms</color>";
                
            }
            else if(ping_part>=300 &&ping_part<=3500)
            {
                tempText = "<color=#FF0000>" + ping_part + "ms</color>";
                
            }
            else
            {
                tempText = "<color=#FF0000>∂œœﬂ</color>";
            }
            tempText = tempText + "\n<color=#00B3B3>" + Math.Round(1 / Time.deltaTime) + "FPS</color>";
            
            GameObject.Find("PingText").GetComponent<TMP_Text>().text = tempText;



            if(!sock.Connected)
            {
                if(SharedVariables.Ping == -1)
                    uicontroller.ShowInf("[1;36m”Îƒø±Íª˙∆˜ ß»•¡¨Ω”£¨’˝‘⁄≥¢ ‘÷ÿ¡¨£¨«ÎÕ¨ ±ºÏ≤Èƒ˙µƒÕ¯¬Á…Ë÷√[0;0m");
                else
                    uicontroller.ShowInf("[1;36m”Î”Œœ∑∑˛ŒÒ¡¨Ω”÷–∂œ£¨º¥Ω´÷ÿ¡¨°£°£°£[0;0m");
                while(true)
                {
                    yield return new WaitForSeconds(1f);
                    try
                    {
                        uicontroller.ShowInf("[1;36m’˝‘⁄÷ÿ¡¨÷–°£°£°£[0;0m");
                        sock.Close();
                        sock.Dispose();
                        sock = null;
                        sock = new TcpClient();
                        sock.Connect(IPAddress.Parse("154.23.179.24"), port);
                        networkStream.Close();
                        networkStream = null;
                        networkStream = sock.GetStream();
                        ClientTask.Abort();
                        ClientTask = null;
                        ClientTask = new Thread(ReceiveMessage);
                        ClientTask.Start();
                        SendMsg("zjmDMaIpOvxdb");
                        SendMsg(id + "®U" + pass + "®UAbcd1234Zwy®U3213@qq.com");
                        break;

                    }
                    catch (Exception e)
                    {
                        uicontroller.ShowInf("[1;36m÷ÿ¡¨ ß∞‹£¨‘≠“Ú£∫[0;0m\n" + e.Message);

                        throw;
                    }
                }
            }
            yield return new WaitForSeconds(3.5f);
        }
    }



    public void ReceiveMessage()
    {

        List<byte> temp = new List<byte>();
        while (true)
        {
            byte[] buffer = new byte[4096];
            int size = networkStream.Read(buffer,0,buffer.Length);
            temp.AddRange(buffer.Take(size));
            if(buffer.Length>0 && buffer[size-1] == '\n')
            {
                byte[] buf = temp.ToArray();
                temp.Clear();
                string res = Encoding.GetEncoding("GB2312").GetString(buf);
                uicontroller.StrSelector(res);
            }
            else
            {
                continue;
            }
        }

    }

    IEnumerator VersionCheck()
    {
        TokenJson token = new TokenJson();
        token.exp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + 1800;
        token.iss = "BieXiang";
        
        string token_str = JsonUtility.ToJson(token);
        
        string token_str_s = HmacSHA256(token_str,token.GetKey());
        
        string token_str_b = Convert.ToBase64String(Encoding.Default.GetBytes(token_str));
        string token_str_s_b = Convert.ToBase64String(Encoding.Default.GetBytes(token_str_s));
        string temp = token_str_b + "." + token_str_s_b;
        WWW version = new WWW("http://175.27.160.226:5500/version.php?token=" + temp);
        yield return version;
        if(version.error !=null)
        {
            Notice.SetActive(true);
            GameObject.Find("NoticeText").GetComponent<TMP_Text>().text = "ºÏ≤È ß∞‹£¨«ÎºÏ≤ÈÕ¯¬Á¡¨Ω”°£°£";
            yield break;
        }
        
        var inf = version.text.Split("#");
        if(float.Parse(Application.version)<float.Parse(inf[0].Split(":")[1]))
        {
            Notice.SetActive(true);
            TMP_Text login_inf = GameObject.Find("NoticeText").GetComponent<TMP_Text>();
            login_inf.text = inf[1] + "\n" + inf[2];
            login_inf.text = login_inf.text.Replace("\\n", "\n");

        }
    }

    public void CloseNotice()
    {
        Notice.SetActive(false);
    }


    public async void SendMsg(string msg)
    {
        try
        {
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(msg + "\n");
            await networkStream.WriteAsync(bytes, 0, bytes.Length);
        }
        catch (Exception)
        {
        }
    }

    IEnumerator LoginCheck()
    {
        Window.SetActive(true);
        rotate = StartCoroutine(RotateLoad(GameObject.Find("Loading")));
        WWW login = new WWW("http://154.23.179.24/mud_login/loginto_zf.php?id=" + id + "&pass=" + pass + "&page=1");
        yield return login;
        if (login.error != null)
        {
            GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "Õ¯¬Á≤ª’˝≥££¨«ÎºÏ≤ÈÕ¯¬Á.";
            Invoke("StopRotate", 2);
        }
        else
        {
            if (string.IsNullOrEmpty(login.text))
            {
                GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "∑˛ŒÒ∆˜“—Õ—ª˙.";
                Invoke("StopRotate", 2);
            }
            else
            {
                if(login.text != "√‹¬Î¥ÌŒÛ" && login.text != "”√ªß≤ª¥Ê‘⁄")
                {
                    GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "—È÷§≥…π¶£¨’˝‘⁄µ«»Î”Œœ∑..";
                    Connect();
                    PlayerPrefs.SetString("id", id);
                    PlayerPrefs.SetString("pass", pass);
                    SharedVariables.ID = id;
                    SendMsg(id + "®U" + pass + "®UAbcd1234Zwy®U3213@qq.com");
                }
                else
                {
                    GameObject.Find("LoadText").GetComponent<TMP_Text>().text = "’À∫≈ªÚ√‹¬Î¥ÌŒÛ£¨«ÎºÏ≤È£°";
                    Invoke("StopRotate", 2);
                }

            }

        }

    }


    public void StopRotate()
    {
        GameObject temp = GameObject.Find("LoadText");
        if (temp == null)
            return;
        temp.GetComponent<TMP_Text>().text = "’˝‘⁄—È÷§’À∫≈√‹¬Î÷–..";
        StopCoroutine(rotate);
        Window.SetActive(false);
    }

    private IEnumerator RotateLoad(GameObject rotate)
    {
        int x = 0;
        while(true)
        {
            x -= 10;
            rotate.transform.Rotate(new Vector3(0, 0, x), 3f);
            yield return new WaitForFixedUpdate();
        }
    }


    private string HmacSHA256(string secret, string signKey)
    {
        string signRet = string.Empty;
        using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey)))
        {
            byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(secret));
            signRet = Convert.ToBase64String(hash);
            signRet = ToHexString(hash); 
        }
        
        return signRet;
    }

    private static string ToHexString(byte[] bytes)
    {
        string hexString = string.Empty;
        if (bytes != null)
        {
            StringBuilder strB = new StringBuilder();
            foreach (byte b in bytes)
            {
                strB.AppendFormat("{0:x2}", b);
            }
            hexString = strB.ToString();
        }
        return hexString;

    }

    private void OnApplicationPause(bool focus)
    {
        if (!focus)   
        {
            if (PlayerPrefs.GetInt("isBackData", 0) == 1)
            {
                uicontroller.line.Clear();
            }
        }
        
    }




   


}
