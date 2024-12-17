using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BlackLayer : MonoBehaviour
{
    [SerializeField] private Image _blackBack;

    public void Show(float delay, Action callback = null)
    {
        this.gameObject.SetActive(true);

        Color c = _blackBack.color;
        c.a = 0;

        _blackBack.DOKill();
        _blackBack.color = c;

        _blackBack.DOColor(new Color(c.r, c.g, c.b, 0.6f), 0.25f).SetDelay(delay).OnComplete(() => callback?.Invoke());
    }

    public void Hide(float delay, Action callback = null)
    {
        Color c = _blackBack.color;
        _blackBack.DOColor(new Color(c.r, c.g, c.b, 0f), 0.25f).SetDelay(delay).OnComplete(() =>
        {
            _blackBack.DOKill();
            this.gameObject.SetActive(false);
            callback?.Invoke();
        });
    }


    void Awake()
    {
        this.gameObject.SetActive(false);
    }

}
