using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighScore : MonoBehaviour
{
    Text highscore;

    public void OnEnable()
    {
        highscore = GetComponent<Text>();
        highscore.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";
    }
}