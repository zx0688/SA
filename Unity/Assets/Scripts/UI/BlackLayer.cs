using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BlackLayer : MonoBehaviour
{

    private Image _blackBack;

    public void Show(float delay)
    {
        this.gameObject.SetActive(true);

        Color c = _blackBack.color;
        c.a = 0f;
        _blackBack.DOKill();
        _blackBack.color = c;

        _blackBack.DOFade(0.6f, 0.25f).SetDelay(delay);
    }

    public void Hide(Action callback = null)
    {

        _blackBack.DOFade(0f, 0.25f).OnComplete(() =>
        {
            Color c = _blackBack.color;
            c.a = 0f;
            _blackBack.color = c;
            _blackBack.DOKill();
            this.gameObject.SetActive(false);
            callback?.Invoke();
        });
    }


    void Awake()
    {
        _blackBack = GetComponent<Image>();
    }

}
