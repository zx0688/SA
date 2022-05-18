using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UI_TickChildren : MonoBehaviour
{
    private ITick[] tickList;

    [SerializeField]
    public void UpdateTickList()
    {
        tickList = GetComponentsInChildren<ITick>(true);
    }
    void Start()
    {
        TickAll();
    }
    void Awake()
    {
        UpdateTickList();
    }
    void OnDisable()
    {
        StopAllCoroutines();
    }
    void OnEnable()
    {
        StartCoroutine(Tick());
        TickAll();
    }

    void TickAll()
    {
        int timestamp = GameTime.Current;
        for (int i = 0; i < tickList.Length; i++)
        {
            ITick tick = tickList[i];
            if (tick.IsTickble())
                tick.Tick(timestamp);

        }
    }

    IEnumerator Tick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            TickAll();
        }
    }
}