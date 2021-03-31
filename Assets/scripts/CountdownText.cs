using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour
{
    public delegate void CoundownFinished();
    public static event CoundownFinished OnCountdownFinished;
    Text countdown;

    private void OnEnable()
    {
        countdown = GetComponent<Text>();
        countdown.text = "READY!";
        StartCoroutine("Countdown");
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);
        countdown.text = "SET!";
        yield return new WaitForSeconds(1);
        countdown.text = "GO!";
        yield return new WaitForSeconds(1);

        OnCountdownFinished();
    }
}