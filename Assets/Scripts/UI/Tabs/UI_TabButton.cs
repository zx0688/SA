using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TabButton : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public UI_TabGroup TabGroup;
    //public UnityEvent OnTabSelected;
    //public UnityEvent OnTabDeselected;

    //public bool Selected => _selected;
    //private bool _selected;
    private Image _back;
    private Image _top;

    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup?.OnTabSelect(this);
    }

    private void Awake()
    {
        _top = this.transform.Find("Top").GetComponent<Image>();
        _back = this.transform.Find("Back").GetComponent<Image>();
    }

    public void Select()
    {
        _top.transform.DOScale(1.4f, 0.1f);
        _back.transform.DOScale(1.15f, 0.1f);
    }

    public void Deselect()
    {
        _top.transform.DOScale(1f, 0.1f);
        _back.transform.DOScale(1f, 0.1f);
    }
}