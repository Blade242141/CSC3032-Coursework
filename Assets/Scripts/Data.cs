using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Data : MonoBehaviour {
    string dataURL = "https://ipwhois.app/json/";
    string weatherURL = "https://api.met.no/weatherapi/locationforecast/2.0/compact?lat=";
    string locationURL = "https://geocode.xyz/";
    RawData rawData = new RawData ();
    Units units;

    public VoiceRecognition vr;
    public DebugManager dm;
    public Speech speech;
    public WeatherData wd;
    public UIManager uim;
    public PlayerMovement pm;

    String yourLoc = "your location";
    // Start is called before the first frame update
    private void Start() {
    }

    // Update is called once per frame
    void Update() {
    }

    [ContextMenu ("Get Data Test")]
    async void WeBGetData() {
        var www = UnityWebRequest.Get (dataURL);

        www.SetRequestHeader ("Content-Type", "application/json");

        var operation = www.SendWebRequest ();

        while (!operation.isDone)
            await Task.Yield ();

        if (www.result == UnityWebRequest.Result.Success)
            ConvertData (www.downloadHandler.text);
        else
            Debug.LogError ($"Failed -  { www.error}");
    }

    void ConvertData(string data) {
        data = data.Replace ("{", "");
        data = data.Replace ("}", "");

        data = data.Replace ('"', ' ');
        data = data.Replace (" ", "");

        string [] strArr = data.Split (',');
        Dictionary<string, string> dataDic = new Dictionary<string, string> ();

        for (int i = 0; i < strArr.Length; i++) {
            string [] strSplit = strArr [i].Split (':');
            dataDic.Add (strSplit [0], strSplit [1]);
        }

        rawData.SetIp (dataDic ["ip"]);
        rawData.SetIpType (dataDic ["type"]);
        rawData.SetContinent (dataDic ["continent"]);
        rawData.SetContinent_code (dataDic ["continent_code"]);
        rawData.SetCountry (dataDic ["country"]);
        rawData.SetCountry_code (dataDic ["country_code"]);
        rawData.SetRegion (dataDic ["region"]);
        rawData.SetLatitude (double.Parse (dataDic ["latitude"]));
        rawData.SetLongitude (double.Parse (dataDic ["longitude"]));
        rawData.SetTimezone_name (dataDic ["timezone_name"]);
        rawData.SetTimezone_gmt (dataDic ["timezone_gmt"]);
    }

    #region Commands

    #region Weather Command
    [ContextMenu ("Get Weather Test")]
    public void Weather() {
        string arg = vr.GetResults ();
        if (arg != "") {
            WebGetLocation (locationURL + arg + "?geoit=xml&auth=139617122402493e15914917x44953");
        } else {
            WeBGetData ();
            CheckExsistingWeatherReports (rawData.GetLatitude (), rawData.GetLongitude (), yourLoc);
        }
    }

    async void WebGetWeather(string url, string place) {
        var www = UnityWebRequest.Get (url);

        www.SetRequestHeader ("Content-Type", "application/json");

        var operation = www.SendWebRequest ();

        while (!operation.isDone)
            await Task.Yield ();

        if (www.result == UnityWebRequest.Result.Success)
            ConvertWeather (www.downloadHandler.text, place);
        else
            Debug.LogError ($"Failed -  { www.error}");

    }


    async void WebGetLocation(string url) {
        var www = UnityWebRequest.Get (url);

        www.SetRequestHeader ("Content-Type", "application/json");

        var operation = www.SendWebRequest ();

        while (!operation.isDone)
            await Task.Yield ();

        if (www.result == UnityWebRequest.Result.Success)
            ConvertLocation (www.downloadHandler.text);
        else
            Debug.LogError ($"Failed -  { www.error}");

    }

    void ConvertLocation(string str) {
        int cityNo = 0;
        int lonNo = 0;
        int latNo = 0;

        if (str.Contains ("error")) {
            speech.Say ("Sorry' that place name could not be found");
            dm.DebugOut ("Error", "Place Name Not Found", false, true);
        } else {
            string [] split = str.Split ('\n');
            for (int i = 0; i < split.Length; i++) {
                if (split [i].Contains ("<city>"))
                    cityNo = i;
                else if (split [i].Contains ("<longt>"))
                    lonNo = i;
                else if (split [i].Contains ("<latt>"))
                    latNo = i;
            }

            string city = split [cityNo].Replace ("<city>", "");
            city = city.Replace ("</city>", "");
            string lon = split [lonNo].Replace ("<longt>", "");
            lon = lon.Replace ("</longt>", "");
            string lat = split [latNo].Replace ("<latt>", "");
            lat = lat.Replace ("</latt>", "");

            if (cityNo == 0 || lonNo == 0 || latNo == 0) {
                speech.Say ("Sorry, there was a error recieving the location data");
                dm.DebugOut ("Error", "city - " + cityNo + ", lon - " + lonNo + "latNo - " + latNo, false, true);
            } else {
                CheckExsistingWeatherReports (Convert.ToDouble (lat), Convert.ToDouble (lon), city);
            }

        }
    }

    void CheckExsistingWeatherReports(double webLat, double weblon, string place) {
        if (wd != null) {
            if (wd.locations.ContainsKey (place)) {
                Dictionary<string, Times> tempDic;
                wd.locations.TryGetValue (place, out tempDic);

                if (tempDic.ContainsKey (WeatherDataTimeformat (true))) {
                    if (tempDic.ContainsKey (WeatherDataTimeformat (false)) || WeatherDataTimeformat (false).Contains("-1")) {
                        wd.locations.Remove (place);
                        GetWeather (webLat, weblon, place);
                    }
                    GiveUserWeatherFeedback (tempDic, place);
                } else {
                    wd.locations.Remove (place);
                    GetWeather (webLat, weblon, place);
                }
            } else {
                GetWeather (webLat, weblon, place);
            }
        } else {
            wd = new WeatherData ();
            GetWeather (webLat, weblon, place);
        }
    }

    void GetWeather(double webLat, double weblon, string place) {
        double lat;
        double lon;

        lat = Math.Round ((float) webLat, 4);
        lon = Math.Round ((float) weblon, 4);

        string localWeather = weatherURL + lat + "&lon=" + lon;
        dm.DebugOut ("Data", "Data from https://met.no/ MET Norway under the https://api.met.no/doc/License Norwegian Licence for Open Government Data (NLOD) 2.0 and Creative Commons 4.0 BY International licences", false, false);

        WebGetWeather (localWeather, place);
    }

    void ConvertWeather(string str, string place) {
        units = new Units ();
        string [] split = str.Split (',');

        int endOfUnits = 0;

        //Sort Units
        for (int i = 0; i < split.Length; i++) {
            split [i] = split [i].Replace ("\"", "");
            split [i] = split [i].Replace ("{", "");
            split [i] = split [i].Replace ("}", "");

            string [] temp = split [i].Split (':');

            if (split [i].Contains ("air_pressure_at_sea_level")) {
                units.air_pressure_at_sea_level = temp [1];
            } else if (split [i].Contains ("air_temperature")) {
                if (temp [1].Contains ("celsius"))
                    units.air_temperature = "°C";
            } else if (split [i].Contains ("cloud_area_fraction")) {
                units.cloud_area_fraction = temp [1];
            } else if (split [i].Contains ("precipitation_amount")) {
                units.precipitation_amount = temp [1];
            } else if (split [i].Contains ("relative_humidity")) {
                units.relative_humidity = temp [1];
            } else if (split [i].Contains ("wind_from_direction")) {
                units.wind_from_direction = temp [1];
            } else if (split [i].Contains ("wind_speed")) {
                units.wind_speed = temp [1];
            }
            if (split [i].Contains ("timeseries")) {
                endOfUnits = i;
                break;
            }
        }

        //Move remaining data into a new array & time all the different time sets
        string [] splitData = new string [(split.Length - endOfUnits)];

        int numberOfTimeSets = 0;

        for (int i = 0; i < splitData.Length; i++) {
            splitData [i] = split [endOfUnits];
            endOfUnits++;

            if (splitData [i].Contains ("\"time\""))
                numberOfTimeSets++;
        }

        Dictionary<string, Times> times = new Dictionary<string, Times> ();

        //Split Each time set
        for (int i = 0; i < splitData.Length; i++) {
            if (splitData [i].Contains ("\"time\"")) {
                //New Time Set
                string [] timeSplit = splitData [i].Split (new string [] { "\":\"" }, StringSplitOptions.None);

                int skipToNewTime = 0;


                Times time = new Times ();

                //Remove "\", "{" and "}" from lines between times
                for (int n = i; n < splitData.Length; n++) {
                    if (splitData [n].Contains ("\"time\"") && n != i)
                        break;

                    splitData [n] = splitData [n].Replace ("\"", "");
                    splitData [n] = splitData [n].Replace ("{", "");
                    splitData [n] = splitData [n].Replace ("}", "");
                    skipToNewTime++;

                    string [] temp = splitData [n].Split (':');

                    if (temp [0].Contains ("air_pressure_at_sea_level")) {
                        time.air_pressure_at_sea_level = temp [1];
                    } else if (temp [0].Contains ("air_temperature")) {
                        time.air_temperature = temp [1];
                    } else if (temp [0].Contains ("cloud_area_fraction")) {
                        time.cloud_area_fraction = temp [1];
                    } else if (temp [1].Contains ("precipitation_amount")) {
                        time.precipitation_amount = temp [2];
                    } else if (temp [0].Contains ("relative_humidity")) {
                        time.relative_humidity = temp [1];
                    } else if (temp [0].Contains ("wind_from_direction")) {
                        time.wind_from_direction = temp [1];
                    } else if (temp [0].Contains ("wind_speed")) {
                        time.wind_speed = temp [1];
                    } else if (temp [0].Contains ("next_12_hours")) {
                        time.next_12_hours = temp [3];
                    } else if (temp [0].Contains ("next_1_hours")) {
                        time.next_1_hours = temp [3];
                    } else if (temp [0].Contains ("next_6_hours")) {
                        time.next_6_hours = temp [3];
                    }
                }

                times.Add (timeSplit [1].Replace ("\"", ""), time);

                skipToNewTime--;
                i += skipToNewTime;
            }
        }

        //Add Data to location dictionary
        wd.locations.Add (place, times);

        Dictionary<string, Times> tempDic;
        wd.locations.TryGetValue (place, out tempDic);
        GiveUserWeatherFeedback (tempDic, place);

    }

    void GiveUserWeatherFeedback(Dictionary<string, Times> location, String locationStr) {
        dm.DebugOut ("System", "Stop Latency", false, true);
        pm.DisplayWeather ();

        Times tempTimes;

        location.TryGetValue (WeatherDataTimeformat (true), out tempTimes);

        StartCoroutine (WaitForAnimationIntro (tempTimes, locationStr));

    }

    IEnumerator WaitForAnimationIntro(Times time, String locationStr) {
        yield return new WaitForSeconds (1);

        uim.DisplayWeather (time, units);
        speech.Say ("The estimated weather cast for " + locationStr + " is " + ConvertWeatherTxtToStr (time.next_1_hours));
    }

    String ConvertWeatherTxtToStr(string str) {
        if (str.Contains ("clearsky"))
            return "clear sky";
        else if (str.Contains ("fair"))
            return "fair";
        else if (str.Contains ("partlycloudy"))
            return "partly cloudy";
        else if (str.Contains ("cloudy"))
            return "cloudy";
        else if (str.Contains ("fog"))
            return "fog";
        else if (str.Contains ("showers"))
            return "shower";
        else if (str.Contains ("rain"))
            return "rain";
        else if (str.Contains ("sleet"))
            return "sleet";
        else if (str.Contains ("snow"))
            return "snow";

        return null;
    }


    string WeatherDataTimeformat(bool now) {
        if (now) {
            return System.DateTime.Now.ToString ("yyyy-MM-dd") + "T" + System.DateTime.Now.ToString ("HH") + ":00:00Z";
        } else {
            string tempStr = System.DateTime.Now.ToString ("yyyy-MM-dd") + "T";
            int tempInt = Int32.Parse (System.DateTime.Now.ToString ("HH"));
            tempInt--;
            tempStr += tempInt.ToString("00") + ":00:00Z";
            return tempStr;
        }
    }

        #endregion

        public void TimeDate() {
        String str = vr.GetFullResults ();

        if (str.Contains ("date") && str.Contains ("time"))
            speech.Say ("The date is " + DateTime.Now.ToString ("D", DateTimeFormatInfo.InvariantInfo) + "and the time is " + DateTime.Now.ToString ("t", DateTimeFormatInfo.InvariantInfo));
        else if (str.Contains ("day") && str.Contains ("time"))
            speech.Say ("The date is " + DateTime.Now.ToString ("D", DateTimeFormatInfo.InvariantInfo) + "and the time is " + DateTime.Now.ToString ("t", DateTimeFormatInfo.InvariantInfo));
        else if (str.Contains ("date"))
            speech.Say ("The date is " + DateTime.Now.ToString ("D", DateTimeFormatInfo.InvariantInfo));
        else if (str.Contains ("day"))
            speech.Say ("The date is " + DateTime.Now.ToString ("D", DateTimeFormatInfo.InvariantInfo));
        else if (str.Contains ("time"))
            speech.Say ("The time is " + DateTime.Now.ToString ("t", DateTimeFormatInfo.InvariantInfo));


    }


    String [] ignoreWords = {"called", "new", "a", "create", "delete" };

    public List<DifferentLists> lists = new List<DifferentLists> ();

    public void Lists() {
        String str = vr.GetFullResults ();
        String [] split;

        split = str.Split (' ');

        int foundNo = -1;

        if (str.Contains ("create")) {
            //Create a new list
            for (int i = 0; i < split.Length; i++) {
                if (foundNo >= -2) {
                    for (int w = 0; w < ignoreWords.Length; w++) {
                        if (!split [i].Contains (ignoreWords [w]) && split [i] != "list" && split [i] != "lists") {
                            // Name of new list
                            lists.Add (new DifferentLists (split [i], new List<string> ()));
                            speech.Say ("I create a new list called " + split [i]);
                            foundNo = -2;
                            break;
                        }
                    }
                }
            }

            if (foundNo >= -1)
                speech.Say ("Please specify a name when creating a list");

        } else if (str.Contains ("add to") || str.Contains ("delete")) {
            //Add to selected list or delelte selected list
            for (int i = 0; i < lists.Count; i++) {
                if (str.Contains (lists [i].name)) {
                    // Add to this list
                    foundNo = i;
                }
            }

            if (foundNo >= -1) {
                //List was not found.
                speech.Say ("Sorry, I was unable to find that list");
            } else if (str.Contains ("add to")) {
                for (int i = 0; i < split.Length; i++) {
                    for (int w = 0; w < ignoreWords.Length; w++) {
                        if (!split [i].Contains (ignoreWords [w])) {
                            lists [foundNo].AddToList (split [i]);
                            speech.Say ("Sorry, I was unable to find that list");
                        }
                    }
                }
            } else if (str.Contains ("delete")) {
                lists.RemoveAt (foundNo);
            }
        }
    }

    #endregion
}
