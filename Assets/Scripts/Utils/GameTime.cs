using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameTime
{
    private static int _timestamp;
    public static void Init(int serverTimestamp)
    {
        //int clientTimestamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        _timestamp = serverTimestamp - (int)Time.realtimeSinceStartup;
    }

    public static int Left(int timestamp, int start, int duration) => duration - timestamp + start;

    public static int Current => (int)Time.realtimeSinceStartup + _timestamp;

    public static bool isExpired(int time) => ((int)Time.realtimeSinceStartup + _timestamp) > time;

}