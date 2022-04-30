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
        /*        foreach (ITick t in tickList)
                    if (t.IsTickble())
                        t.Tick(timestamp);*/
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