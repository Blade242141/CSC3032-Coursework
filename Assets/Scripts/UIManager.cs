using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public DebugManager dm;

    public GameObject canvas;
    public float fadeConvasDuration = 1.8f;

    public void ActivateCanvas(bool canvasActive = false) {
        CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup> ();
        StartCoroutine (FadeCanvas(canvasGroup, canvasGroup.alpha, canvasActive ? 1 : 0));
    }

    public IEnumerator FadeCanvas(CanvasGroup canvasGroup, float start, float end) {
        float timer = 0.0f;

        while (timer < fadeConvasDuration) {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp (start, end, timer / fadeConvasDuration);

            yield return null;
        }
    }

    #region Weather Panel

    public GameObject weatherPanel;
    public TMP_Text windSpeedTxt;
    public TMP_Text precipitationAmountTxt;
    public TMP_Text airTemperatureTxt;
    public Image nextHourImg;
    public Image SixHourImg;
    public Sprite [] weatherSprites;

    public void DisplayWeather(Times times, Units units) {
        windSpeedTxt.text = times.wind_speed + units.wind_speed;
        precipitationAmountTxt.text = times.precipitation_amount + units.precipitation_amount;
        airTemperatureTxt.text = times.air_temperature + units.air_temperature;
        nextHourImg.sprite = ConvertWeatherTxtToImg (times.next_1_hours);
        SixHourImg.sprite = ConvertWeatherTxtToImg (times.next_6_hours);
        ActivateCanvas (true);
        StartCoroutine (DisplayTime (10));
    }

    IEnumerator DisplayTime(float waitTime) {
        yield return new WaitForSeconds (waitTime);
        ActivateCanvas (false);
        dm.DebugOut ("System", "Weather - Displayed", true, false);
    }

    Sprite ConvertWeatherTxtToImg(string str) {
        if (str.Contains ("clearsky"))
            return weatherSprites [0];
        else if (str.Contains ("fair"))
            return weatherSprites [2];
        else if (str.Contains ("partlycloudy"))
            return weatherSprites [2];
        else if (str.Contains ("cloudy"))
            return weatherSprites [1];
        else if (str.Contains ("fog"))
            return weatherSprites [3];
        else if (str.Contains ("showers"))
            return weatherSprites [5];
        else if (str.Contains ("rain"))
            return weatherSprites [4];
        else if (str.Contains ("sleet"))
            return weatherSprites [6];
        else if (str.Contains ("snow"))
            return weatherSprites [7];

        return null;
    }

    #endregion
}
