using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json.Serialization;
/*
TODO:
1. Fix FetchAnyAsync() by replacing GetAsync

    */
public partial class _Default : System.Web.UI.Page
{
    public HttpClient client;
    HttpClient client1;
    SS ss;  //sesion state object to store application state variables such as counters

    protected void Page_Unload(object sender, EventArgs e)
    {
        Session["SS"] = ss;

    }

        protected void Page_Load(object sender, EventArgs e) {
        if (!this.IsPostBack) { ss=new SS(); Session["SS"] = ss; }  //first request for page load
        else
        {   
            ss = (SS)Session["SS"];
            // move below code to 'Application Start'
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Mashape-Key", "6ITLSlrg1smsh6ghYbKlJLpSh3Ndp1xL4EZjsnTea4iom");
            client.DefaultRequestHeaders.Add("X-Twaip-Key", "3DwSdiTO4NkBUzKVP78Be8C4qFGWcyVUBXvDZRyglnPj2aJEAf9Mm2yHRZnXgjl2a+fqGWLFLLUquggsksi");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client1 = new HttpClient();  // new Xamarin.Android.Net.AndroidClientHandler() when because default handler is not working while debugging
            client1.DefaultRequestHeaders.Add("X-Twaip-Key", "3DwSdiTO4NkBUzKVP78Be8C4qFGWcyVUBXvDZRyglnPj2aJEAf9Mm2yHRZnXgjl2a+fqGWLFLLUquggsksi");
            client1.DefaultRequestHeaders.Add("Accept", "application/json");

        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {   
            if (ss.gstate == gamestate.end)
            {
                string sfrS = sfr.Text;  //word set rating in the Slider control
                string log = ss.json.word + "," + H1.Text + "," + H2.Text + "," + H3.Text + "," + H4.Text + "," + H5.Text + "," + sfrS;
                WriteSetAsync2("https://guesssetrating.azurewebsites.net/api/HttpPOST-CRUD-guessSetRating", log);
                H1.Text = H2.Text = H3.Text = H4.Text = H5.Text = "- - -";
                Guess_label.Text = "**s*e*c*r*e*t  w*o*r*d**";
                HCount.Text = "";
                ExampleS1.Text = "...Example Sentence... ";
                ExampleS2.Text = "...Example Sentence...";
                PartOfSpeechText.Text = "()";
                TypeOfText.Text = "-1"; TypeOfText.Visible = false;
                HasPartsText.Text = "-2"; HasPartsText.Visible = false;
                HasTypesText.Text = "-3"; HasTypesText.Visible = false;
                InstanceOfText.Text = "-4"; InstanceOfText.Visible = false;
                HasInstancesText.Text = "-5"; HasInstancesText.Visible = false;
                MemberOfText.Text = "-6"; MemberOfText.Visible = false;
                ss.windex = 0;
                ss.gstate = gamestate.start;
                UserGuessText.Text = "";
                sfr.Text = "0.22";
                ss.letter_counter = 0;
                ss.etc_counter = 0;
                ss.etcs = 0;
                Guess_result.Text = "";
            }
            if (ss.gstate == gamestate.start)
            {
                int corpuscount = 40000;
                if (LevelDropDownList.SelectedValue == "Medium") corpuscount = 20000;
                //String url = "https://wordsapiv1.p.mashape.com/words/?random=true&frequencymin=7.99";
                ss.url = "http://api.wordnik.com:80/v4/words.json/randomWord?hasDictionaryDef=false&minCorpusCount=" + corpuscount + "&maxCorpusCount=-1&minDictionaryCount=5&maxDictionaryCount=-1&minLength=4&maxLength=-1&api_key=a2a73e7b926c924fad7001ca3111acd55af2ffabf50e";
                ss.json = (RW) FetchAnyAsync<RW>(client, ss.url);
                Guess_label.Text = underline(ss.json.word);
                UserGuessText.Text = "...Type your guess for the secret word here...";
                //url = "https://twinword-word-graph-dictionary.p.mashape.com/association/?entry=" + json.word;
                ss.url = "https://api.twinword.com/api/v4/word/associations/?entry=" + ss.json.word;
                ss.url2 = "https://wordsapiv1.p.mashape.com/words/" + ss.json.word + "/examples";
                ss.url3 = "https://wordsapiv1.p.mashape.com/words/" + ss.json.word;
                try
                {
                    ss.jsonAAWS = (AAWS) FetchAnyAsync<AAWS>(client1, ss.url);
                    ss.jsonE = (EE) FetchAnyAsync<EE>(client, ss.url2);
                    ss.jsonWW = (WORDetc) FetchWordEtcAsync(client, ss.url3);
                }
                catch (Exception ex)
                {
                    ss.jsonAAWS = null; ss.jsonE = null; ss.jsonWW = null;
                    ss.gstate = gamestate.end;
                    System.Diagnostics.Debug.WriteLine(ex);
                    //this.DisplayAlert("EXCEPTION", "Exception thrown for " + json.word + jsonE.ToString(), "Yes", "No");
                }
                if ((ss.jsonAAWS == null)||(ss.jsonAAWS.associations_scored==null)) ss.gstate = gamestate.end;
                else if (ss.jsonAAWS.associations_scored.Count > 0)
                {
                    ss.gstate = gamestate.ingame;
                    HCount.Text = ss.jsonAAWS.associations_scored.Count.ToString();
                    ss.aws = orderAWs(ss.jsonAAWS.associations_scored);
                }
                else ss.gstate = gamestate.end;
            }
            if (ss.gstate == gamestate.ingame)
            {
                if ((ss.windex < ss.jsonAAWS.associations_scored.Count) && (ss.windex < 5))
                {
                    if (ss.windex == 0) { H1.Text = ss.aws[ss.windex]; PartOfSpeechText.Text = "(" + ss.jsonWW.results[0].partOfSpeech + ")"; }
                    else if (ss.windex == 1) { H2.Text = ss.aws[ss.windex]; UserGuessText.Text = ""; }
                    else if (ss.windex == 2) { H3.Text = ss.aws[ss.windex]; }
                    else if (ss.windex == 3) { H4.Text = ss.aws[ss.windex]; }
                    else if (ss.windex == 4) { H5.Text = ss.aws[ss.windex]; }
                    ss.windex++;
                }
                else if (((ss.windex == ss.jsonAAWS.associations_scored.Count) || (ss.windex == 5)) && (ss.jsonE != null) && (ss.jsonE.examples != null) && (ss.jsonE.examples.Count > 0))
                {
                    ExampleS1.Text = blankTheWord(ss.jsonE.examples.First(), ss.json.word) + ".";
                    ss.windex++;
                }
                else if (((ss.windex == (ss.jsonAAWS.associations_scored.Count) + 1) || (ss.windex == 6)) && (ss.jsonE != null) && (ss.jsonE.examples != null) && (ss.jsonE.examples.Count > 1))
                {
                    ExampleS2.Text = blankTheWord(ss.jsonE.examples[1], ss.json.word) + ".";
                    ss.windex++;
                }
                else if ((ss.jsonWW != null) && (ss.jsonWW.results != null) && (ss.jsonWW.results.Count > 0) && (ss.etc_counter < ss.etcs))
                {
                    //if (etc_counter==0) PartOfSpeechText.Text = "(" + jsonWW.results[0].partOfSpeech + ")";
                    if ((ss.etc_counter == 0) && (ss.jsonWW.results[0].availables.Contains("typeOf"))) { TypeOfText.Text = "It is a type of " + blankTheWord(ss.jsonWW.results[0].typeOf, ss.json.word); TypeOfText.Visible = true; }
                    else if ((ss.etc_counter == 2) && (ss.jsonWW.results[0].availables.Contains("hasParts"))) { HasPartsText.Text = "It has a part: " + blankTheWord(ss.jsonWW.results[0].hasParts, ss.json.word); HasPartsText.Visible = true; }
                    else if ((ss.etc_counter == 1) && (ss.jsonWW.results[0].availables.Contains("hasTypes"))) { HasTypesText.Text = "It has a type: " + blankTheWord(ss.jsonWW.results[0].hasTypes, ss.json.word); HasTypesText.Visible = true; }
                    else if ((ss.etc_counter == 3) && (ss.jsonWW.results[0].availables.Contains("instanceOf"))) { InstanceOfText.Text = "It is an instance of " + blankTheWord(ss.jsonWW.results[0].instanceOf, ss.json.word); InstanceOfText.Visible = true; }
                    else if ((ss.etc_counter == 4) && (ss.jsonWW.results[0].availables.Contains("hasInstances"))) { HasInstancesText.Text = "It has an instance: " + blankTheWord(ss.jsonWW.results[0].hasInstances, ss.json.word); HasInstancesText.Visible = true; }
                    else if ((ss.etc_counter == 5) && (ss.jsonWW.results[0].availables.Contains("memberOf"))) { MemberOfText.Text = "It is a member of " + blankTheWord(ss.jsonWW.results[0].memberOf, ss.json.word); MemberOfText.Visible = true; }
                    ss.etc_counter++;
                }
                else if (ss.letter_counter < ss.json.word.Length)
                {
                    Guess_label.Text = set_letter_clue(); //based on ss.json.word and letter_counter
                    ss.letter_counter++;
                }
                else
                {
                    ss.gstate = gamestate.end;
                    Guess_label.Text = ss.json.word;
                }
            }

    }

    // Instead used: LevelDropDownList.SelectedValue
    //public void OnSetLevel(object sender, EventArgs e)
    //{
    //    var action = "Easy"; // await DisplayActionSheet("Select Level?", "Easy", "Medium");
    //    Level.Text = "Set Level = " + action;
    //}

    public string underline(string s)
    {
        string ss = "";
        for (int i = 0; i < s.Length; i++) ss = ss + "_ ";
        return ss;
    }

    private T FetchAnyAsync<T>(HttpClient client, String uri)
    {
        T Itemss = default(T);
        var response = client.GetAsync(uri).Result;
        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            Itemss = JsonConvert.DeserializeObject<T>(content);
        }
        return Itemss;

    }

    private WORDetc FetchWordEtcAsync(HttpClient client, String uri)
    {//partOfSpeech, typeOf, hasTypes, hasParts, instanceOf, hasInstances, memberOf
        WORDetc Itemss = new WORDetc();
        Itemss.results = new List<RR>();
        RR rr = new RR();
        Itemss.results.Add(rr);
        var response = client.GetAsync(uri).Result;
        if (response.IsSuccessStatusCode)
        {
            var content = response.Content.ReadAsStringAsync().Result;
            //Itemss = JsonConvert.DeserializeObject<WORDetc>(content);
            var jObject = Newtonsoft.Json.Linq.JObject.Parse(content);
            if ((jObject["results"] == null) || (jObject["results"][0] == null)) return null;
            if (jObject["results"][0]["partOfSpeech"] != null)
            {
                rr.partOfSpeech = (string)jObject["results"][0]["partOfSpeech"];
                //etcs++;
            }
            if (jObject["results"][0]["typeOf"] != null)
            {
                rr.typeOf = (string)jObject["results"][0]["typeOf"][0];
                rr.availables.Add("typeOf");
                ss.etcs++;
            }
            if (jObject["results"][0]["hasTypes"] != null)
            {
                rr.hasTypes = (string)jObject["results"][0]["hasTypes"][0];
                rr.availables.Add("hasTypes");
                ss.etcs++;
            }
            if (jObject["results"][0]["hasParts"] != null)
            {
                rr.hasParts = (string)jObject["results"][0]["hasParts"][0];
                rr.availables.Add("hasParts");
                ss.etcs++;
            }
            if (jObject["results"][0]["instanceOf"] != null)
            {
                rr.instanceOf = (string)jObject["results"][0]["instanceOf"][0];
                rr.availables.Add("instanceOf");
                ss.etcs++;
            }
            if (jObject["results"][0]["hasInstances"] != null)
            {
                rr.hasInstances = (string)jObject["results"][0]["hasInstances"][0];
                rr.availables.Add("hasInstances");
                ss.etcs++;
            }
            if (jObject["results"][0]["memberOf"] != null)
            {
                rr.memberOf = (string)jObject["results"][0]["memberOf"][0];
                rr.availables.Add("memberOf");
                ss.etcs++;
            }
        }
        return Itemss;

    }

    private async Task<string> WriteSetAsync(HttpClient client, String uri)
    {   // not working probably because of dataP serialized object format
        var dataP = new
        {
            name = "Foo",
            //category = "article"
        };
        var postBodyDataP = JsonConvert.SerializeObject(dataP);
        var response = await client.PostAsync(uri, new StringContent(postBodyDataP, Encoding.UTF8, "application/json"));
        //this.DisplayAlert("LOG RESULT", response.Content, "Yes", "No"); 
        if (response.IsSuccessStatusCode)
        {
            return "Success";
        }
        else return "Fail";

    }

    public string WriteSetAsync2(string uri, string log)
    {
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
        req.Method = "POST";
        req.ContentType = "application/json";
        Stream stream = req.GetRequestStream();
        string json = "{\"name\": \"" + log + "\" }";
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        stream.Write(buffer, 0, buffer.Length);
        HttpWebResponse res = (HttpWebResponse)req.GetResponse();
        return res.StatusCode.ToString();
    }

    public string blankTheWord(string s, string word)
    {
        string blank = "";
        for (int i = 0; i < word.Length; i++) blank += "_";
        string si = s.Replace(word, blank);
        si = char.ToUpper(si[0]) + si.Substring(1); //First letter of the sentence is capitalized
        return si;
    }
    public string[] orderAWs(Dictionary<string, double> aws)
    {
        int le;
        if (aws.Count() < 5) le = aws.Count();
        else le = 5;
        string[] s = new string[le];
        for (int i = 0; (i < le); i++) s[i] = aws.ElementAt(i).Key;
        if (aws.Count() > 9)
        {
            //s[4] = aws.ElementAt(10).Key;
            for (int i = le; i > 0; i--) s[i - 1] = aws.ElementAt(((aws.Count() / 2) * (le - i) / le)).Key;
        }
        return s;

    }
    string set_letter_clue()
    { //returns the clue string based on ss.json.word, and letter_counter and Guess_label.Text
        StringBuilder next = new StringBuilder(Guess_label.Text);
        if (ss.letter_counter < ss.json.word.Length)
            next[ss.letter_counter * 2] = ss.json.word.ElementAt(ss.letter_counter);
        return next.ToString();
    }




    protected void check_Click(object sender, EventArgs e)
    {
        if (String.Equals(ss.json.word, UserGuessText.Text, StringComparison.OrdinalIgnoreCase))
        {
            //this.DisplayAlert("CONGRATUTATIONS", "You guessed it right. The word was " + json.word, "Yes", "No");
            Guess_result.Text = "CONGRATUTATIONS ! You guessed it right. The word was " + ss.json.word;
            Guess_label.Text = ss.json.word.ToUpper();
            ss.letter_counter = ss.json.word.Length;
        }
        else Guess_result.Text = "Nope.";
    }

    protected void LevelDropDownList_SelectedIndexChanged(object sender, EventArgs e)
    {
    }
}

public class RW  // random word class to hold the result of web service call
{
    public string word { get; set; }
}
public class AA  //from mashape.com
{
    public string entry { get; set; }
    public List<string> assoc_word { get; set; }
    public List<string> assoc_word_ex { get; set; }
    public string result_msg { get; set; }
}

public class AAWS
{
    public string entry { get; set; }
    public Dictionary<string, double> associations_scored { get; set; }
}

public class EE
{
    public string word { get; set; }
    public List<string> examples { get; set; }
}

public class WORDetc
{
    public string word { get; set; }
    public IList<RR> results { get; set; }

}
public class RR
{  //partOfSpeech, typeOf, hasTypes, hasParts, instanceOf, hasInstances, memberOf
    public string partOfSpeech { get; set; }
    public string definition { get; set; }
    public string typeOf { get; set; }
    public string hasTypes { get; set; }
    public string hasParts { get; set; }
    public string instanceOf { get; set; }
    public string hasInstances { get; set; }
    public string memberOf { get; set; }
    public List<string> availables = new List<string>();
}

public enum gamestate { start, end, ingame };

public class SS
{  //session state class to store gamer's game's state
    public gamestate gstate = gamestate.start;
    public int windex = 0;
    //AA jsonA= null;
    public RW json = null;
    public EE jsonE = null;
    public AAWS jsonAAWS = null;
    public WORDetc jsonWW = null;
    public string url = "";
    public string url2 = "";
    public string url3 = "";
    public string[] aws;
    public int letter_counter = 0;
    public int etc_counter = 0;
    public int etcs = 0; //etc size
}

