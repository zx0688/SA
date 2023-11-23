using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using UnityEngine;

public static class TimeFormat
{

    private static string[] timeNameLocalization;
    private static string twoDigits(int num)
    {
        if (num < 10)
            return "0" + num.ToString();
        return num.ToString();
    }

    public static void Init()
    {
        string second = ' ' + LocalizationManager.Localize("Time.Second").Substring(0, 3).ToLower() + ' ';
        string day = ' ' + LocalizationManager.Localize("Time.Day").ToLower() + ' ';
        string hour = ' ' + LocalizationManager.Localize("Time.Hour").Substring(0, 3).ToLower() + ' ';
        string min = ' ' + LocalizationManager.Localize("Time.Minute").Substring(0, 3).ToLower() + ' ';
        timeNameLocalization = new string[4] { day, hour, min, second };
    }
    public static string ONE_CELL_FULLNAME(int timeLeft)
    {

        int sInMinutes = 60;
        int sInHours = sInMinutes * 60;
        int sInDay = sInHours * 24;
        int remaining = timeLeft;
        int days = Mathf.FloorToInt(remaining / sInDay);
        remaining -= (sInDay * days);
        int hours = Mathf.FloorToInt(remaining / sInHours);
        remaining -= (sInHours * hours);
        int minutes = Mathf.FloorToInt(remaining / sInMinutes);
        remaining -= (sInMinutes * minutes);
        int seconds = Mathf.FloorToInt(remaining);

        //this.timeNameLocalization[0] = localpositions[timeleft].

        if (days > 0)
        {
            return days + timeNameLocalization[0];
        }
        else if (hours > 0)
        {
            return hours + timeNameLocalization[1];
        }
        else if (minutes > 0)
        {
            return minutes + timeNameLocalization[2];
        }
        else
            return seconds + timeNameLocalization[3];

    }
    public static string TWO_CELLS_FULLNAME(int timeLeft)
    {

        int sInMinutes = 60;
        int sInHours = sInMinutes * 60;
        int sInDay = sInHours * 24;
        int remaining = timeLeft;
        int days = Mathf.FloorToInt(remaining / sInDay);
        remaining -= (sInDay * days);
        int hours = Mathf.FloorToInt(remaining / sInHours);
        remaining -= (sInHours * hours);
        int minutes = Mathf.FloorToInt(remaining / sInMinutes);
        remaining -= (sInMinutes * minutes);
        int seconds = Mathf.FloorToInt(remaining);

        //ThreadStaticAttribute.private void OnParticleCollision(GameObject other)
        // {

        //}


        if (days > 0)
        {
            if (hours > 0)
                return days + timeNameLocalization[0] + hours + timeNameLocalization[1];
            else
                return days + timeNameLocalization[0];
        }
        else if (hours > 0)
        {
            if (minutes > 0)
                return hours + timeNameLocalization[1] + minutes + timeNameLocalization[2];
            else
                return hours + timeNameLocalization[1];
        }
        else if (minutes > 0)
        {
            if (seconds > 0)
                return minutes + timeNameLocalization[2] + seconds + timeNameLocalization[3];
            else
                return minutes + timeNameLocalization[2];
        }
        else
            return seconds + timeNameLocalization[3];

    }
}