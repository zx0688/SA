using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UITabButton : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public UITabGroup TabGroup;

    [SerializeField] private Image back;
    [SerializeField] private Image top;
    [SerializeField] private RawImage dot;

    public float Delay = 0f;

    void Awake()
    {
        dot?.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Delay > 0)
        {
            StartCoroutine(DelayMethod());
        }
        else
        {
            TabGroup?.OnTabSelect(this);
        }
    }


    IEnumerator DelayMethod()
    {
        Select();
        yield return new WaitForSeconds(Delay);
        TabGroup?.OnTabSelect(this);
    }

    public void Select()
    {
        top.transform.DOScale(1.4f, 0.1f);
        back.transform.DOScale(1.15f, 0.1f);
        dot?.gameObject.SetActive(true);
        dot?.transform.DOScale(1f, 0.1f);
    }

    public void Deselect()
    {
        top.transform.DOScale(1f, 0.1f);
        back.transform.DOScale(1f, 0.1f);
        dot?.transform.DOScale(0f, 0.1f).OnComplete(() =>
        {
            dot?.gameObject.SetActive(false);
        });

    }
}