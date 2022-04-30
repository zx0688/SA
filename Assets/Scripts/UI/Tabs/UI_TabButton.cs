using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    public UI_TabGroup TabGroup;
    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    public bool Selected => _selected;
    private bool _selected;
    private Image _back;
    private Image _top;

    public void OnPointerClick(PointerEventData eventData)
    {
        _selected = !_selected;

        TabGroup?.OnTabSelector(this);

        if (_selected)
        {
            CloseAnimate();
        }
        else
        {
            OpenAnimate();
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //TabGroup?.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //TabGroup?.OnTabExit(this);
    }

    public void Select()
    {
        OnTabSelected?.Invoke();
    }

    public void Deselect()
    {
        OnTabDeselected?.Invoke();
    }

    private void Awake()
    {
        _selected = false;
        _top = this.transform.Find("Top").GetComponent<Image>();
        _back = this.transform.Find("Back").GetComponent<Image>();
    }

    void Start()
    {
        //_backgroud = GetComponent<Image>();
        TabGroup?.Add(this);

    }

    private void OpenAnimate()
    {
        _top.transform.DOScale(1.4f, 0.1f).OnComplete(() => { });
        _back.transform.DOScale(1.15f, 0.1f).OnComplete(() => { });
    }

    private void CloseAnimate()
    {
        _top.transform.DOScale(1f, 0.1f).OnComplete(() => { });
        _back.transform.DOScale(1f, 0.1f).OnComplete(() => { });
    }
}