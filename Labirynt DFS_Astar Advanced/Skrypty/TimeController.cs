using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeController
{
    private static float timeGate;
    private static float time = 0.0f;
    private static float timer = 0.0f;
    private static bool timerStart = false;

    public static float TimeGate { get => timeGate;}
    public static bool TimerStart { get => timerStart; set => timerStart = value; }

    public static void InitTime(float newTime)
    {
        timeGate = newTime;
    }

    public static bool TimePass()
    {
        time += Time.deltaTime;
        if(time >= timeGate)
        {
            return true;
        }
        return false;
    }

    public static void TimeZero()
    {
        time = 0.0f;
    }

    public static void ResetTimer()
    {
        timer = 0.0f;
    }

    public static string Timer()
    {
        if (timerStart)
        {
            timer += Time.deltaTime;
            float minutes = Mathf.FloorToInt(timer / 60.0f);
            float seconds = Mathf.FloorToInt(timer % 60.0f);
            string currentTime = string.Format("{0:00}:{1:00}", minutes, seconds);
            return currentTime;
        }
        return "00:00";
    }
}
