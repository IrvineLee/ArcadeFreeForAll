using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float startTime = 1f;

    float mTimer;
    Text mSecondText, mDotText, mMilliSecondText;

    void Start()
    {
        startTime = GameManager.sSingleton.roundTime;
        mTimer = startTime;
        mSecondText = transform.GetChild(0).GetComponent<Text>();
        mDotText = transform.GetChild(1).GetComponent<Text>();
        mMilliSecondText = transform.GetChild(2).GetComponent<Text>();
    }

    void Update()
    {
        if (!GameManager.sSingleton.IsBattle()) return;

        if (mTimer != 0)
        {
            mTimer -= Time.deltaTime;
            if (mTimer < 0) mTimer = 0;
            UpdateTimer(mTimer);
        }
        else if (mTimer <= 0)
        {
            UIManager.sSingleton.RoundEnd();
            gameObject.SetActive(false);
        }
    }

    public void Initialize()
    {
        mTimer = startTime;
        UpdateColor(Color.white);
        UpdateTimer(mTimer);
        gameObject.SetActive(true);
    }  

    void UpdateTimer(float duration)
    {
        float seconds = (int)duration;
        mSecondText.text = seconds.ToString();

        float milliSeconds = duration % 1;
        string milliSecStr = milliSeconds.ToString("F2");

        char c1 = milliSecStr[2];
        char c2 = milliSecStr[3];
        milliSecStr = c1.ToString() + c2.ToString();

        mMilliSecondText.text = milliSecStr;

        if (seconds <= 10) UpdateColor(Color.red);
    }

    void UpdateColor(Color color)
    {
        mSecondText.color = color;
        mMilliSecondText.color = color;
        mDotText.color = color;
    }
}
