using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;

using UnityEngine;
using UnityEngine.UI;


public static class Utils
{

    public static void ClearArray<T>(ref T[] arr)
    {
        int count = 0;
        for (int a = 0; a < arr.Length; a++)
        {
            if (arr[a] == null)
                count++;
        }
        T[] n = new T[arr.Length - count];
        count = 0;
        for (int a = 0; a < arr.Length; a++)
        {
            if (arr[a] != null)
            {
                n[count] = arr[a];
                count++;
            }
        }
        arr = n;
    }

    public static void SetAlpha(Image image, float a)
    {
        Color c = image.color;
        c.a = a;
        image.color = c;
    }
    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        if (arr.Length == 0 || index == -1)
            return;

        for (int a = index; a < arr.Length - 1; a++)
        {
            arr[a] = arr[a + 1];
        }
        Array.Resize(ref arr, arr.Length - 1);
    }
    public static bool Intersection<T>(T[] array1, T[] array2)
    {

        if (array1 == null || array2 == null)
            return false;

        foreach (T e1 in array1)
        {
            if (Array.IndexOf<T>(array2, e1) != -1)
                return true;
        }
        return false;
    }
    public static bool Contains<T>(T[] array1, T element)
    {
        return Array.IndexOf(array1, element) != -1;
    }
}