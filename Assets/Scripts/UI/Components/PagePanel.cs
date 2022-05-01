using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PagePanel : MonoBehaviour
{
    private readonly float _hidePosition = 650f;
    private readonly float _showPosition = 451f;

    public Action ScrollPageLeft;
    public Action ScrollPageRight;

    [SerializeField] private RectTransform _leftPanel;
    private ClickButton _leftButton;

    [SerializeField] private RectTransform _rightPanel;
    private ClickButton _rightButton;

    void Awake()
    {
        _leftButton = _leftPanel.transform.GetComponentInChildren<ClickButton>();
        _leftButton.OnClick += OnLeftButtonClick;
        _rightButton = _rightPanel.transform.GetComponentInChildren<ClickButton>();
        _rightButton.OnClick += OnRightButtonClick;

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
        _leftPanel.DOKill();
        _rightPanel.DOKill();

        _leftPanel.anchoredPosition = new Vector2(-_hidePosition, 0f);
        _rightPanel.anchoredPosition = new Vector2(_hidePosition, 0f);
    }

    public void Show()
    {
        _leftPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-_showPosition, 0f), 0.3f, true);
        _rightPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(_showPosition, 0f), 0.3f, true);
    }

    public void Hide()
    {
        _leftPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-_hidePosition, 0f), 0.3f, true);
        _rightPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(_hidePosition, 0f), 0.3f, true);
    }

}
