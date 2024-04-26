using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameTime
{
    private static int _timestamp = 0;
    public static void Fix(int serverTimestampInSeconds)
    {
        _timestamp = serverTimestampInSeconds - (int)Time.realtimeSinceStartup;
    }

    public static int Left(int time, int start, int duration) => duration - time + start;

    public static int Create(int duration) => duration + (int)Time.realtimeSinceStartup + _timestamp;

    public static int Get() => (int)Time.realtimeSinceStartup + _timestamp;
    public static long Get64()
    {
        var r = Convert.ToInt64((int)(Time.realtimeSinceStartup * 1000));
        long t = Convert.ToInt64((int)_timestamp);
        t = t * 1000;

        return r + t;
    }
    public static bool IsExpired(int time) => ((int)Time.realtimeSinceStartup + _timestamp) > time;

}