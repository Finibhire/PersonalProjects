using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;

namespace DragonKnight2_Farmer
{
    public partial class MainInterfaceForm : Form
    {
        private DKPostData woodPost, fishPost, stonePost, ironPost, innPost;
        private List<Cookie> postCookies;
        private int PeriodsCompleted;
        private Queue<Tuple<Exception, DateTime>> lastExceptions;
        private int exceptionCount;
        private Thread farmResourcesWorkerThread;
        private Thread farmMonstersWorkerThread;
        private Thread gamblingWorkerThread;
        private bool KillFarmResourcesThread;
        private bool KillFarmMonstersThread;
        private bool KillGamblingThread;
        private bool SleepingForTurns;
        private int MinAcceptableTurns;
        private int MaxAcceptableTurns;

        public MainInterfaceForm()
        {
            InitializeComponent();

            PeriodsCompleted = 0;
            PeriodsCompleted = 0;
            exceptionCount = 0;

            woodPost.URL = @"https://dknight2.com/index.php?do=woodcut";
            woodPost.Buffer50TP = Encoding.ASCII.GetBytes("ac=t6&submit=Yes");
            woodPost.Buffer30TP = Encoding.ASCII.GetBytes("ac=t5&submit=Yes");
            woodPost.Buffer20TP = Encoding.ASCII.GetBytes("ac=t1&submit=Yes");
            woodPost.Buffer15TP = Encoding.ASCII.GetBytes("ac=t2&submit=Yes");
            woodPost.Buffer10TP = Encoding.ASCII.GetBytes("ac=t3&submit=Yes");
            woodPost.Buffer5TP = Encoding.ASCII.GetBytes("ac=t4&submit=Yes");
            fishPost.URL = @"https://dknight2.com/index.php?do=fish";
            fishPost.Buffer50TP = Encoding.ASCII.GetBytes("ac=t6&submit=Yes!");
            fishPost.Buffer30TP = Encoding.ASCII.GetBytes("ac=t5&submit=Yes!");
            fishPost.Buffer20TP = Encoding.ASCII.GetBytes("ac=t1&submit=Yes!");
            fishPost.Buffer15TP = Encoding.ASCII.GetBytes("ac=t2&submit=Yes!");
            fishPost.Buffer10TP = Encoding.ASCII.GetBytes("ac=t3&submit=Yes!");
            fishPost.Buffer5TP = Encoding.ASCII.GetBytes("ac=t4&submit=Yes!");
            stonePost.URL = @"https://dknight2.com/index.php?do=stone";
            stonePost.Buffer50TP = Encoding.ASCII.GetBytes("ac=t6&submit=Yes!");
            stonePost.Buffer30TP = Encoding.ASCII.GetBytes("ac=t5&submit=Yes!");
            stonePost.Buffer20TP = Encoding.ASCII.GetBytes("ac=t1&submit=Yes!");
            stonePost.Buffer15TP = Encoding.ASCII.GetBytes("ac=t2&submit=Yes!");
            stonePost.Buffer10TP = Encoding.ASCII.GetBytes("ac=t3&submit=Yes!");
            stonePost.Buffer5TP = Encoding.ASCII.GetBytes("ac=t4&submit=Yes!");
            ironPost.URL = @"https://dknight2.com/index.php?do=iron";
            ironPost.Buffer50TP = Encoding.ASCII.GetBytes("ac=t6&submit=Yes!");
            ironPost.Buffer30TP = Encoding.ASCII.GetBytes("ac=t5&submit=Yes!");
            ironPost.Buffer20TP = Encoding.ASCII.GetBytes("ac=t1&submit=Yes!");
            ironPost.Buffer15TP = Encoding.ASCII.GetBytes("ac=t2&submit=Yes!");
            ironPost.Buffer10TP = Encoding.ASCII.GetBytes("ac=t3&submit=Yes!");
            ironPost.Buffer5TP = Encoding.ASCII.GetBytes("ac=t4&submit=Yes!");
            innPost.URL = @"https://dknight2.com/index.php?do=inn";
            innPost.Buffer50TP = Encoding.ASCII.GetBytes("submit=Sleep+Comfortably");
            innPost.Buffer30TP = innPost.Buffer50TP;
            innPost.Buffer20TP = innPost.Buffer50TP;
            innPost.Buffer15TP = innPost.Buffer50TP;
            innPost.Buffer10TP = innPost.Buffer50TP;
            innPost.Buffer5TP = innPost.Buffer50TP;

            lastExceptions = new Queue<Tuple<Exception, DateTime>>();
            postCookies = new List<Cookie>();
            ddlCookie.SelectedIndex = 0;  // event fires and updates cookie
            //postCookies = new CookieContainer();
            //postCookies.Add(new Cookie("dkgame", ddlCookie.Text, "/", "dknight2.com"));
            KillFarmResourcesThread = true;
            KillFarmMonstersThread = true;
            KillGamblingThread = true;
            SleepingForTurns = false;
        }

        private struct DKPostData
        {
            public string URL;
            public byte[] Buffer50TP;
            public byte[] Buffer30TP;
            public byte[] Buffer20TP;
            public byte[] Buffer15TP;
            public byte[] Buffer10TP;
            public byte[] Buffer5TP;
        }

        private HttpWebRequest MakeDKRquest(string url, byte[] postBuffer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Host = "dknight2.com";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
            request.Referer = url;
            request.CookieContainer = new CookieContainer();
            foreach (Cookie cook in postCookies)
                request.CookieContainer.Add(cook);
            request.KeepAlive = false;

            if (postBuffer == null || postBuffer.Length == 0)
            {
                request.Method = "GET";
            }
            else
            {
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(postBuffer, 0, postBuffer.Length);
                    postStream.Close();
                }
            }

            return request;
        }

        private void btnFarmMonsters_Click(object sender, EventArgs e)
        {
            if (KillFarmMonstersThread)// farmMonstersWorkerThread == null || !farmMonstersWorkerThread.IsAlive)
            {
                StartFarmingMonstersAsyc();
            }
            else
            {
                StopFarmingMonstersAsyc();
            }

        }

        private class FarmMonsterThreadStartData
        {
            public int TurnsPerMinute;
            public bool BreakOnEnchanter;
        }

        private void StartFarmingMonstersAsyc()
        {
            if (this.InvokeRequired)
            {
                DelDefVoidNoParams del = StartFarmingMonstersAsyc;
                this.BeginInvoke(del);
            }
            else
            {
                KillFarmMonstersThread = false;
                ddlCookie.Enabled = false;
                btnFarmResources.Enabled = false;
                btnGambling.Enabled = false;
                btnFarmMonsters.Text = "Stop Farming!";
                farmMonstersWorkerThread = new Thread(new ParameterizedThreadStart(FarmMonstersMain));
                farmMonstersWorkerThread.Start(new FarmMonsterThreadStartData() { TurnsPerMinute = Convert.ToInt32(tbTurnsPerMinute.Text), BreakOnEnchanter = chbBreakOnEnchanter.Checked });
            }
        }

        private void StopFarmingMonstersAsyc()
        {
            if (this.InvokeRequired)
            {
                DelDefVoidNoParams del = StopFarmingMonstersAsyc;
                this.BeginInvoke(del);
            }
            else
            {
                KillFarmMonstersThread = true;
                ddlCookie.Enabled = true;
                if (farmMonstersWorkerThread != null)
                    farmMonstersWorkerThread.Join();
                farmMonstersWorkerThread = null;
                btnFarmResources.Enabled = true;
                btnGambling.Enabled = true;
                btnFarmMonsters.Text = "Farm Monsters!";
                //UpdateTextAsyc();
                tbOut.Text = "User Aborted!" + Environment.NewLine + tbOut.Text;
            }
        }

        private Character LoadInitialCharacterData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://dknight2.com/index.php");

            request.Host = "dknight2.com";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
            request.Referer = @"https://dknight2.com/index.php";
            request.CookieContainer = new CookieContainer();
            foreach (Cookie cook in postCookies)
                request.CookieContainer.Add(cook);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = false;

            return ParseResponse(request);
        }

        private void FarmMonstersMain(object ThreadData)
        {
            FarmMonsterThreadStartData threadData = (FarmMonsterThreadStartData)ThreadData;
            PeriodsCompleted = 0;
            HttpWebRequest request = null;

            Character c = LoadInitialCharacterData();
            c.TurnsPerMin = threadData.TurnsPerMinute;
            c.BreakOnEnchanter = threadData.BreakOnEnchanter;

            do
            {
                CharacterAction act = c.GetNextMonsterFarmAction();

                request = (HttpWebRequest)WebRequest.Create(act.ActionURL);

                request.Host = "dknight2.com";
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
                request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
                request.Referer = act.ActionRefererURL;
                request.CookieContainer = new CookieContainer();
                foreach (Cookie cook in postCookies)
                    request.CookieContainer.Add(cook);
                request.KeepAlive = false;
                if (act.ActionPOST == null)
                {
                    request.Method = "GET";
                }
                else
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    if (!string.IsNullOrWhiteSpace(act.ActionPOST))
                    {
                        byte[] postBuffer = Encoding.ASCII.GetBytes(act.ActionPOST);
                        using (Stream postStream = request.GetRequestStream())
                        {
                            postStream.Write(postBuffer, 0, postBuffer.Length);
                            postStream.Close();
                        }
                    }
                }

                PeriodsCompleted++;
                UpdateTextAsyc(c);

                try
                {
                    ParseResponse(request, c);
                }
                catch (Exception ex)
                {
                    ManageConnectionException(ex);
                }

                if (c.CurrentTurns < MinAcceptableTurns)
                {
                    SleepingForTurns = true;
                    UpdateTextAsyc(c);
                    while (!KillFarmMonstersThread && c.CurrentTurns < MaxAcceptableTurns)
                    {
                        Thread.Sleep(5000);
                    }
                    SleepingForTurns = false;
                }
                
            }
            while (!KillFarmMonstersThread);
        }

        private Character ParseResponse(HttpWebRequest request, Character c = null)
        {
            string fullResponse = null;

            if (c == null)
                c = new Character();

            byte[] responseBuffer = new byte[2048];
            int byteCount = 0;
            StringBuilder responseString = new StringBuilder();
            Decoder decoder = Encoding.UTF8.GetDecoder();

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            {
                do
                {
                    byteCount = responseStream.Read(responseBuffer, 0, responseBuffer.Length);

                    char[] chars = new char[decoder.GetCharCount(responseBuffer, 0, byteCount)];
                    decoder.GetChars(responseBuffer, 0, byteCount, chars, 0);
                    responseString.Append(chars);
                }
                while (byteCount > 0);

                fullResponse = responseString.ToString();

                if (response.ResponseUri.ToString().Contains("fight"))
                {
                    c.InCombat = true;
                    if (response.ResponseUri.ToString().Contains("boss"))
                    {
                        c.FightingBoss = true;
                    }
                    else
                    {
                        c.FightingBoss = false;
                    }
                }
                else
                {
                    if (c.InCombat)
                    {
                        c.InCombat = false;
                        c.FightingBoss = false;
                        c.TargetAsleep = false;
                    }
                }

                if (response.ResponseUri.ToString().ToLower().Contains("check"))
                {
                    MessageBox.Show("CHECK FOUND!", "reCAPTCHA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (c.BreakOnEnchanter && response.ResponseUri.ToString().ToLower().Contains("move") && fullResponse.Contains("<form action = 'index.php?do=enchantitem' method='post'>"))
                {
                    MessageBox.Show("Enchanter Found!", "Enchanter Found!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                int len = response.Cookies.Count;
                for (int i = 0; i < len; i++)
                {
                    Cookie rc = response.Cookies[i];
                    bool found = false;
                    for (int j = 0; j < postCookies.Count; j++)
                    {
                        Cookie pc = postCookies[j];
                        if (pc.Name == rc.Name && pc.Path == rc.Path)
                        {
                            found = true;
                            postCookies[j] = rc;
                            break;
                        }
                    }
                    if (!found)
                    {
                        postCookies.Add(rc);
                    }
                }
            }

            if (fullResponse.ToLower().Contains("captcha"))
            {
                MessageBox.Show("CHECK FOUND! - 2", "reCAPTCHA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            int parseStart = 0;
            int parseEnd = 0;

            parseEnd = fullResponse.IndexOf("<a href='index.php?do=buypotions'>") + 70;

            parseStart = fullResponse.IndexOf("(", parseEnd) + 1;
            parseEnd = fullResponse.IndexOf(")", parseStart);
            c.CurrentPotionHPCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseStart = fullResponse.IndexOf("(", parseEnd) + 1;
            parseEnd = fullResponse.IndexOf(")", parseStart);
            c.CurrentPotionMPCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseStart = fullResponse.IndexOf("(", parseEnd) + 1;
            parseEnd = fullResponse.IndexOf(")", parseStart);
            c.CurrentPotionTPCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseStart = fullResponse.IndexOf("<center class='blackbg'>You own ", parseEnd) + 32;
            parseEnd = fullResponse.IndexOf(" Wood", parseStart);
            c.CurrentWoodCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseStart = fullResponse.IndexOf("<center class='blackbg'>You own ", parseEnd) + 32;
            parseEnd = fullResponse.IndexOf(" Fish", parseStart);
            c.CurrentFishCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseStart = fullResponse.IndexOf("<center class='blackbg'>You own ", parseEnd) + 32;
            parseEnd = fullResponse.IndexOf(" Stone", parseStart);
            c.CurrentStoneCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseStart = fullResponse.IndexOf("<center class='blackbg'>You own ", parseEnd) + 32;
            parseEnd = fullResponse.IndexOf(" Iron", parseStart);
            c.CurrentIronCount = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            parseEnd = fullResponse.IndexOf("Hello <a href='index.php?do=onlinechar:", parseEnd) + 350;
            parseEnd = fullResponse.IndexOf("<img src=\"images/bars_", parseEnd);
            parseStart = fullResponse.IndexOf("HP: ", parseEnd - 15) + 4;
            c.CurrentHitPoints = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

            parseEnd = fullResponse.IndexOf("<img src=\"images/bars_", parseEnd + 40);
            parseStart = fullResponse.IndexOf("MP: ", parseEnd - 15) + 4;
            c.CurrentManaPoints = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

            parseEnd = fullResponse.IndexOf("<img src=\"images/bars_", parseEnd + 40);
            parseStart = fullResponse.IndexOf("TP: ", parseEnd - 15) + 4;
            c.CurrentTravelPoints = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

            Point loc = new Point();
            parseEnd = fullResponse.IndexOf("<form name=\"navi\" action=\"index.php?do=move\" method=\"post\">", parseEnd + 40);
            parseStart = fullResponse.IndexOf(" at ", parseEnd - 35) + 4;
            parseEnd = fullResponse.IndexOfAny(new char[2] { 'N', 'S' }, parseStart);
            loc.Y = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));
            loc.Y *= (fullResponse[parseEnd] == 'N') ? 1 : -1;
            parseStart = fullResponse.IndexOf(',', parseEnd + 1) + 1;
            parseEnd = fullResponse.IndexOfAny(new char[2] { 'E', 'W' }, parseStart);
            loc.X = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));
            loc.X *= (fullResponse[parseEnd] == 'E') ? 1 : -1;
            c.CurrentLocation = loc;

            if (c.InCombat)
            {
                int sleepStart = parseEnd;
                parseStart = fullResponse.IndexOf(">Health<", parseEnd) + 47;
                if (parseStart >= parseEnd)  //some sort of bug is causing it to think it's in combat when it isn't
                {
                    parseStart = fullResponse.IndexOf('>', parseStart) + 1;
                    parseEnd = fullResponse.IndexOf('<', parseStart + 1);
                    c.TargetHitPoints = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                    int tmp = fullResponse.IndexOf(" is asleep.<br", sleepStart, parseStart - sleepStart - 300);
                    if (tmp > 0)
                    {
                        c.TargetAsleep = true;
                    }
                    tmp = fullResponse.IndexOf(" has woken up.<br", sleepStart, parseStart - sleepStart - 300);
                    if (tmp > 0)
                    {
                        c.TargetAsleep = false;
                    }
                }
                else
                {
                    c.InCombat = false;
                    c.TargetAsleep = false;
                    c.FightingBoss = false;
                }
            }

            parseStart = fullResponse.IndexOf("<b>Level</b>: ", parseEnd) + 14;
            parseEnd = fullResponse.IndexOf('<', parseStart);
            c.CurrentLevel = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));
            parseStart = fullResponse.IndexOf("<b>Turns</b>: ", parseEnd) + 14;
            parseEnd = fullResponse.IndexOf('<', parseStart);
            c.CurrentTurns = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));
            parseStart = fullResponse.IndexOf("<b>Gold</b>: ", parseEnd) + 13;
            parseEnd = fullResponse.IndexOf('<', parseStart);
            c.CurrentGold = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));
            parseStart = fullResponse.IndexOf("<b>Bank</b>: ", parseEnd) + 13;
            parseEnd = fullResponse.IndexOf('<', parseStart);
            c.CurrentGoldBank = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));
            parseStart = fullResponse.IndexOf("<b>Dragon Points</b>: ", parseEnd) + 22;
            parseEnd = fullResponse.IndexOf('<', parseStart);
            c.CurrentDragonPoints = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));
            parseStart = fullResponse.IndexOf("<b>DP Bank</b>: ", parseEnd) + 16;
            parseEnd = fullResponse.IndexOf('<', parseStart);
            c.CurrentDPBank = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart).Replace(",", ""));

            if (request.RequestUri.Query.ToLower() == "?do=viewvillages")
            {
                int tableStart, tableEnd;
                tableStart = fullResponse.IndexOf("<th>Teleport</th>") + 15;
                tableEnd = fullResponse.IndexOf("</table>", tableStart);

                parseStart = tableStart;
                while (parseStart >= tableStart && parseStart < tableEnd)
                {
                    Village v = new Village();
                    loc = new Point();

                    parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                    parseEnd = fullResponse.IndexOf("</td>", parseStart);
                    loc.Y = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                    parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                    parseEnd = fullResponse.IndexOf("</td>", parseStart);
                    loc.X = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));
                    v.Location = loc;

                    parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                    parseEnd = fullResponse.IndexOf("</td>", parseStart);
                    v.Fish = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                    parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                    parseEnd = fullResponse.IndexOf("</td>", parseStart);
                    v.Wood = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                    parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                    parseEnd = fullResponse.IndexOf("</td>", parseStart);
                    v.Iron = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                    parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                    parseEnd = fullResponse.IndexOf("</td>", parseStart);
                    v.Stone = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                    c.Villages.Add(v);
                    parseStart = fullResponse.IndexOf("</tr><tr>", parseStart) + 9;
                }

            }
            else if (request.RequestUri.Query.ToLower() == "?do=transfer")
            {
                parseStart = fullResponse.IndexOf("<p class='center'>Summon Troops and Resources from Village</p>") + 63;
                
                Village v = new Village();
                v.Location = c.CurrentLocation;

                parseStart = fullResponse.IndexOf("<td>Fish</td>", parseStart) + 13;
                parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                parseEnd = fullResponse.IndexOf("</td>", parseStart);
                v.Fish = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                parseStart = fullResponse.IndexOf("<td>Wood</td>", parseStart) + 14;
                parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                parseEnd = fullResponse.IndexOf("</td>", parseStart);
                v.Wood = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                parseStart = fullResponse.IndexOf("<td>Stone</td>", parseStart) + 15;
                parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                parseEnd = fullResponse.IndexOf("</td>", parseStart);
                v.Stone = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                parseStart = fullResponse.IndexOf("<td>Iron</td>", parseStart) + 14;
                parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                parseEnd = fullResponse.IndexOf("</td>", parseStart);
                v.Iron = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                parseStart = fullResponse.IndexOf("<td>Gold</td>", parseStart) + 14;
                parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                parseEnd = fullResponse.IndexOf("</td>", parseStart);
                v.Gold = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                parseStart = fullResponse.IndexOf("<td>DP</td>", parseStart) + 12;
                parseStart = fullResponse.IndexOf("<td>", parseStart) + 4;
                parseEnd = fullResponse.IndexOf("</td>", parseStart);
                v.DragonPoints = int.Parse(fullResponse.Substring(parseStart, parseEnd - parseStart));

                c.CurrentVillage = v;
            }

            return c;
        }

        //private delegate void DelDefBtnFarmResources_Click(object sender, EventArgs e);
        private delegate void DelDefVoidNoParams();
        private void btnFarmResources_Click(object sender, EventArgs e)
        {
            if (KillFarmResourcesThread) //farmResourcesWorkerThread == null || !farmResourcesWorkerThread.IsAlive)
            {
                StartFarmingResourcesAsyc();
            }
            else
            {
                StopFarmingResourcesAsyc();
            }

        }

        private void StartFarmingResourcesAsyc()
        {
            if (this.InvokeRequired)
            {
                DelDefVoidNoParams del = StartFarmingResourcesAsyc;
                this.BeginInvoke(del);
            }
            else
            {
                KillFarmResourcesThread = false;
                ddlCookie.Enabled = false;
                btnFarmResources.Text = "Stop Farming!";
                btnFarmMonsters.Enabled = false;
                btnGambling.Enabled = false;
                farmResourcesWorkerThread = new Thread(new ParameterizedThreadStart(FarmResourcesMain));
                farmResourcesWorkerThread.Start(Convert.ToInt32(tbTurnsPerMinute.Text));
            }
        }

        private void StopFarmingResourcesAsyc()
        {
            if (this.InvokeRequired)
            {
                DelDefVoidNoParams del = StopFarmingResourcesAsyc;
                this.BeginInvoke(del);
            }
            else
            {
                KillFarmResourcesThread = true;
                ddlCookie.Enabled = true;
                if (farmResourcesWorkerThread != null)
                    farmResourcesWorkerThread.Join();
                farmResourcesWorkerThread = null;
                btnFarmMonsters.Enabled = true;
                btnGambling.Enabled = true;
                btnFarmResources.Text = "Farm Resources!";
                UpdateTextAsyc();
                PeriodsCompleted = 0;
                //textBox1.Text += Environment.NewLine + "Finished Farming Resources!";
            }
        }


        private void FarmResourcesMain(object TurnsPerMinute)
        {
            HttpWebRequest request = null;

            Character c = LoadInitialCharacterData();
            c.TurnsPerMin = (int)TurnsPerMinute;

            while (!KillFarmResourcesThread)
            {
                CharacterAction act = c.GetNextRangerFarmAction();

                request = (HttpWebRequest)WebRequest.Create(act.ActionURL);

                request.Host = "dknight2.com";
                request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
                request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
                request.Referer = act.ActionRefererURL;
                request.CookieContainer = new CookieContainer();
                foreach (Cookie cook in postCookies)
                    request.CookieContainer.Add(cook);
                request.KeepAlive = false;
                if (act.ActionPOST == null)
                {
                    request.Method = "GET";
                }
                else
                {
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    if (!string.IsNullOrWhiteSpace(act.ActionPOST))
                    {
                        byte[] postBuffer = Encoding.ASCII.GetBytes(act.ActionPOST);
                        using (Stream postStream = request.GetRequestStream())
                        {
                            postStream.Write(postBuffer, 0, postBuffer.Length);
                            postStream.Close();
                        }
                    }
                }

                PeriodsCompleted++;
                UpdateTextAsyc(c);

                try
                {
                    ParseResponse(request, c);
                }
                catch (Exception ex)
                {
                    ManageConnectionException(ex);
                }
                if (c.CurrentLocation.X != 0 || c.CurrentLocation.Y != 0)
                {
                    throw new Exception("Not in Town!");
                }

                if (c.CurrentTurns < MinAcceptableTurns)
                {
                    SleepingForTurns = true;
                    UpdateTextAsyc(c);
                    while (!KillFarmResourcesThread && c.CurrentTurns < MaxAcceptableTurns)
                    {
                        Thread.Sleep(5000);
                    }
                    SleepingForTurns = false;
                }

            }
       
            StopFarmingResourcesAsyc();
            UpdateTextAsyc(c);
        }

        private void PostRequest(string requestURL, byte[] requestPostData, Character c)
        {
            //const int bufferSize = 2048;


            //byte[] responseBuffer = new byte[bufferSize];
            try
            {
                HttpWebRequest request = MakeDKRquest(requestURL, requestPostData);
                ParseResponse(request, c);
                //using (WebResponse response = request.GetResponse())
                //using (Stream responseStream = response.GetResponseStream())
                //{
                //    responseStream.Read(responseBuffer, 0, bufferSize);
                //}
            }
            catch (Exception ex)
            {
                ManageConnectionException(ex);
            }
        }

        private void ManageConnectionException(Exception ex)
        {
            const int SleepDurationOnTooManyExceptions = 15 * 60 * 1000; // 15 min
            const double ExceptionExpireation = 5; // 5 min

            exceptionCount++;
            lastExceptions.Enqueue(new Tuple<Exception, DateTime>(ex, DateTime.Now));
            while ((DateTime.Now - lastExceptions.Peek().Item2) > TimeSpan.FromMinutes(ExceptionExpireation))
            {
                lastExceptions.Dequeue();
            }
            UpdateTextAsyc();
            if (lastExceptions.Count > 10)
            {
                Thread.Sleep(SleepDurationOnTooManyExceptions);
            }
        }

        delegate void DelDefUpdateText(Character c = null);
        private void UpdateTextAsyc(Character c = null)
        {
            if (this.InvokeRequired)
            {
                DelDefUpdateText del = UpdateTextAsyc;
                this.BeginInvoke(del, c);
            }
            else
            {
                StringBuilder sb = new StringBuilder();


                if (c != null)
                {
                    if (SleepingForTurns)
                    {
                        lblAction.Text =
                            "Sleeping Until: " +
                            DateTime.Now.AddMinutes((double)(MaxAcceptableTurns - c.CurrentTurns) / (double)c.TurnsPerMin)
                            .ToShortTimeString();
                    }
                    else if (farmMonstersWorkerThread != null && farmMonstersWorkerThread.IsAlive)
                    {
                        lblAction.Text = c.GetNextMonsterFarmAction().Action.ToString();
                    }
                    else if (farmResourcesWorkerThread != null && farmResourcesWorkerThread.IsAlive)
                    {
                        lblAction.Text = c.GetNextRangerFarmAction().Action.ToString();
                    }
                    else
                    {
                        lblAction.Text = "UNKNOWN";
                    }

                    lblTurns.Text = c.CurrentTurns.ToString();
                    lblGold.Text = c.CurrentGold.ToString("#,0,") + "k";
                    lblBankGold.Text = c.CurrentGoldBank.ToString("#,0,") + "k";
                    lblDragonPoints.Text = c.CurrentDragonPoints.ToString("#,0");
                    lblBankDragonPoints.Text = c.CurrentDPBank.ToString("#,0");
                    lblCycles.Text = PeriodsCompleted.ToString("#,#");

                    lblFish.Text = c.CurrentFishCount.ToString("#,0,") + "k";
                    lblIron.Text = c.CurrentIronCount.ToString("#,0,") + "k";
                    lblStone.Text = c.CurrentStoneCount.ToString("#,0,") + "k";
                    lblWood.Text = c.CurrentWoodCount.ToString("#,0,") + "k";
                    
                    if (c.CurrentLocation.Y < 0)
                    {
                        sb.Append(c.CurrentLocation.Y * -1);
                        sb.Append("S,");
                    }
                    else
                    {
                        sb.Append(c.CurrentLocation.Y);
                        sb.Append("N,");
                    }
                    if (c.CurrentLocation.X < 0)
                    {
                        sb.Append(c.CurrentLocation.X * -1);
                        sb.Append('W');
                    }
                    else
                    {
                        sb.Append(c.CurrentLocation.X);
                        sb.Append('E');
                    }
                    lblLocation.Text = sb.ToString();
                    sb.Clear();
                }

                sb.Append("Total Execption Count: ");
                sb.AppendLine(exceptionCount.ToString());
                sb.AppendLine();
                sb.AppendLine("Exception Queue:");

                Tuple<Exception, DateTime>[] exceptionList = new Tuple<Exception, DateTime>[lastExceptions.Count];
                lastExceptions.CopyTo(exceptionList, 0);

                for (int i = exceptionList.Length - 1; i >= 0; i--)
                {
                    sb.AppendLine(exceptionList[i].Item2.ToString());
                    sb.AppendLine(exceptionList[i].Item1.ToString());
                    sb.AppendLine();
                }

                tbOut.Text = sb.ToString();
            }
        }

        private void ddlCookie_SelectedIndexChanged(object sender, EventArgs e)
        {
            postCookies.Clear();
            postCookies.Add(new Cookie("dkgame", ddlCookie.Text, "/", "dknight2.com"));
            if (ddlCookie.SelectedIndex == 0)
            {
                tbTurnsPerMinute.Text = "6";
                MinAcceptableTurns = 30;
                MaxAcceptableTurns = 299;
            }
            else
            {
                tbTurnsPerMinute.Text = "6";
                MinAcceptableTurns = 30;
                MaxAcceptableTurns = 299;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (farmResourcesWorkerThread != null && farmResourcesWorkerThread.IsAlive)
            {
                KillFarmResourcesThread = true;
                farmResourcesWorkerThread.Join(15000);
                if (farmResourcesWorkerThread.IsAlive)
                {
                    farmResourcesWorkerThread.Abort();
                    farmResourcesWorkerThread.Join(15000);
                    if (farmResourcesWorkerThread.IsAlive)
                    {
                        Environment.ExitCode = -1;
                    }
                }
            }
            if (farmMonstersWorkerThread != null && farmMonstersWorkerThread.IsAlive)
            {
                KillFarmMonstersThread = true;
                farmMonstersWorkerThread.Join(15000);
                if (farmMonstersWorkerThread.IsAlive)
                {
                    farmMonstersWorkerThread.Abort();
                    farmMonstersWorkerThread.Join(15000);
                    if (farmMonstersWorkerThread.IsAlive)
                        Environment.ExitCode = -1;
                    //throw new Exception("Unable to close farmMonstersWorkerThread.");
                }
            }
            if (gamblingWorkerThread != null && gamblingWorkerThread.IsAlive)
            {
                KillFarmMonstersThread = true;
                gamblingWorkerThread.Join(15000);
                if (gamblingWorkerThread.IsAlive)
                {
                    gamblingWorkerThread.Abort();
                    gamblingWorkerThread.Join(15000);
                    if (gamblingWorkerThread.IsAlive)
                        Environment.ExitCode = -1;
                    //throw new Exception("Unable to close farmMonstersWorkerThread.");
                }
            }
            Application.Exit();
        }

        private void MainInterfaceForm_Load(object sender, EventArgs e)
        {

        }

        private void btnCollectResources_Click(object sender, EventArgs e)
        {
            //const string GoldBankURL = "https://dknight2.com/index.php?do=bank";
            const string VillagesURL = "https://dknight2.com/index.php?do=viewvillages";
            const string TeleportURL = "https://dknight2.com/index.php?do=teleport";
            const string TransferURL = "https://dknight2.com/index.php?do=transfer";
            //const string GoldBankPOST = "bank=Withdraw&withdraw=10000";
            const string TeleportPOST = "longitude={0}&latitude={1}&teleport=Visit";
            const string TransferPOST = "fish={0}&wood={1}&stone={2}&iron={3}&gold={4}&dp={5}&swords=0&knights=0&pults=0&bows=0&scouts=0&withdrawres=Summon";


            var request = (HttpWebRequest)WebRequest.Create(VillagesURL);

            request.Host = "dknight2.com";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
            request.Referer = VillagesURL;
            request.CookieContainer = new CookieContainer();
            foreach (Cookie cook in postCookies)
                request.CookieContainer.Add(cook);
            request.KeepAlive = false;
            request.Method = "GET";

            Character c = ParseResponse(request);

            foreach (Village v in c.Villages)
            {
                if (c.CurrentTravelPoints <= 20)
                {
                    if (c.CurrentLocation.X != 0 || c.CurrentLocation.Y != 0)
                    {
                        PostRequest(CharacterAction.LookupActionURL(ActionID.GoToTown01, false), null, c);
                    }
                    if (c.CurrentGold < c.CurrentLevel * 10)
                    {
                        PostRequest(
                            CharacterAction.LookupActionURL(ActionID.WithdrawGold, false),
                            Encoding.ASCII.GetBytes(CharacterAction.LookupActionPOST(ActionID.WithdrawGold, c.CurrentLevel * 10)), 
                            c);
                    }
                    PostRequest(
                        CharacterAction.LookupActionURL(ActionID.SleepAtInn, false),
                        Encoding.ASCII.GetBytes(CharacterAction.LookupActionPOST(ActionID.SleepAtInn, c.CurrentLevel * 10)),
                        c);
                }
                PostRequest(
                    TeleportURL,
                    Encoding.ASCII.GetBytes(string.Format(TeleportPOST, v.Location.X.ToString(), v.Location.Y.ToString())),
                    c);

                PostRequest(TransferURL, null, c);
                string postData = string.Format(
                    TransferPOST,
                    c.CurrentVillage.Fish.ToString(),
                    c.CurrentVillage.Wood.ToString(),
                    c.CurrentVillage.Stone.ToString(),
                    c.CurrentVillage.Iron.ToString(),
                    c.CurrentVillage.Gold.ToString(),
                    c.CurrentVillage.DragonPoints.ToString());
                PostRequest(TransferURL, Encoding.ASCII.GetBytes(postData), c);
            }

        }

        private void chkAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkAlwaysOnTop.Checked;
        }

        private void GamblingMain(object TurnsPerMinuet)
        {

            const int MinimumBankGold = 5000000;
            int turnsPerMinuet = (int)TurnsPerMinuet;

            //string[] requestURLs = new string[requestsPerPeriod];
            //byte[][] requestPostDatas = new byte[requestsPerPeriod][];

            const string GoldBankURL = "https://dknight2.com/index.php?do=bank";
            const string GambleURL = "https://dknight2.com/index.php?do=slotsgame";
            const string DPBankURL = "https://dknight2.com/index.php?do=dpbank";

            const string GoldBankPOST = "bank=Withdraw&withdraw=10000";
            const string GamblePOST = "PlayerBet=10000&submit=Play";
            const string DPBankPOST = "dpbank=Deposit&deposit=";

            //postCookies = new CookieContainer();
            //postCookies.Add(new Cookie("dkgame", ddlCookie.Text, "/", "dknight2.com"));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://dknight2.com/index.php");

            request.Host = "dknight2.com";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; rv:41.0) Gecko/20100101 Firefox/41.0";
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Headers.Add("Accept-Language", "en-GB,en;q=0.5");
            request.Referer = @"https://dknight2.com/index.php";
            request.CookieContainer = new CookieContainer();
            foreach (Cookie cook in postCookies)
                request.CookieContainer.Add(cook);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.KeepAlive = false;

            Character c = ParseResponse(request);
            c.TurnsPerMin = turnsPerMinuet;

            try
            {
                while (!KillGamblingThread && c.CurrentGoldBank > MinimumBankGold)
                {
                    UpdateTextAsyc(c);
                    PostRequest(GoldBankURL, ASCIIEncoding.ASCII.GetBytes(GoldBankPOST), c);
                    PostRequest(GambleURL, ASCIIEncoding.ASCII.GetBytes(GamblePOST), c);
                    if (c.CurrentDragonPoints > 0)
                    {
                        PostRequest(DPBankURL, ASCIIEncoding.ASCII.GetBytes(DPBankPOST + c.CurrentDragonPoints), c);
                    }

                    if (c.CurrentTurns < MinAcceptableTurns)
                    {
                        SleepingForTurns = true;
                        UpdateTextAsyc(c);
                        while (!KillGamblingThread && c.CurrentTurns < MaxAcceptableTurns)
                        {
                            Thread.Sleep(5000);
                        }
                        SleepingForTurns = false;
                    }
                    PeriodsCompleted++;
                }
                StopGamblingAsyc();
                UpdateTextAsyc(c);
            }
            catch (Exception ex)
            {
                Exception ex2 = new Exception("Critical Exception!  Worker Thread Aborted!", ex);
                ManageConnectionException(ex);
                ManageConnectionException(ex2);
                throw ex2;
            }
        }

        private void btnGambling_Click(object sender, EventArgs e)
        {
            if (KillGamblingThread)//gamblingWorkerThread == null || !gamblingWorkerThread.IsAlive)
            {
                StartGamblingAsyc();
            }
            else
            {
                StopGamblingAsyc();
            }

        }

        private void StartGamblingAsyc()
        {
            if (this.InvokeRequired)
            {
                DelDefVoidNoParams del = StartFarmingResourcesAsyc;
                this.BeginInvoke(del);
            }
            else
            {
                KillGamblingThread = false;
                ddlCookie.Enabled = false;
                btnGambling.Text = "Stop Gambling!";
                btnFarmMonsters.Enabled = false;
                btnFarmResources.Enabled = false;
                gamblingWorkerThread = new Thread(new ParameterizedThreadStart(GamblingMain));
                gamblingWorkerThread.Start(Convert.ToInt32(tbTurnsPerMinute.Text));
            }
        }

        private void StopGamblingAsyc()
        {
            if (this.InvokeRequired)
            {
                DelDefVoidNoParams del = StopFarmingResourcesAsyc;
                this.BeginInvoke(del);
            }
            else
            {
                KillGamblingThread = true;
                ddlCookie.Enabled = true;
                if (gamblingWorkerThread != null)
                    gamblingWorkerThread.Join();
                gamblingWorkerThread = null;
                btnFarmMonsters.Enabled = true;
                btnFarmResources.Enabled = true;
                btnGambling.Text = "Start Gambling!";
                //UpdateTextAsyc();
                //textBox1.Text += Environment.NewLine + "Finished Gambling!";
            }
        }
    }
}
