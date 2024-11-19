using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PagePanel : MonoBehaviour
{
    private readonly float hidePosition = 90f;
    private readonly float showPosition = -150f;

    public Action ScrollPageLeft;
    public Action ScrollPageRight;

    [SerializeField] private RectTransform _leftPanel;
    private ClickButton leftButton;

    [SerializeField] private RectTransform _rightPanel;
    private ClickButton rightButton;

    [SerializeField] private GameObject _pageCounter;
    [SerializeField] private Text _pageText;

    void Awake()
    {
        leftButton = _leftPanel.transform.GetComponentInChildren<ClickButton>();
        //leftButton.OnClick += OnLeftButtonClick;
        rightButton = _rightPanel.transform.GetComponentInChildren<ClickButton>();
        //rightButton.OnClick += OnRightButtonClick;

        ForceHide();

    }

    private void OnRightButtonClick()
    {
        ScrollPageRight?.Invoke();
    }

    private void OnLeftButtonClick()
    {
        ScrollPageLeft?.Invoke();
    }

    public void ForceHide()
    {
        gameObject.SetActive(false);
        _pageCounter.SetActive(false);
        _leftPanel.DOKill();
        _rightPanel.DOKill();

        _leftPanel.anchoredPosition = new Vector2(-hidePosition, 0f);
        _rightPanel.anchoredPosition = new Vector2(hidePosition, 0f);
    }

    public void SetActivePageCounter(bool active)
    {
        _pageCounter.gameObject.SetActive(active);
    }

    public void SetTextCounter(int value, int total)
    {
        _pageText.text = $"{(value + 1)}/{total}";
    }

    public void ShowArrow()
    {
        _leftPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-showPosition, 0f), 0.3f, true);
        _rightPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(showPosition, 0f), 0.3f, true);
    }

    public void HideArrow()
    {
        _leftPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-hidePosition, 0f), 0.3f, true);
        _rightPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(hidePosition, 0f), 0.3f, true);
    }

}
