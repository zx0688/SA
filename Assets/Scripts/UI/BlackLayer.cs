using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlackLayer : MonoBehaviour
{
    public static BlackLayer Instance { get; private set; }

    [SerializeField] private Image _blackBack;

    public void Show()
    {
        this.gameObject.SetActive(true);

        Color c = _blackBack.color;
        c.a = 0f;
        _blackBack.DOKill();
        _blackBack.color = c;

        _blackBack.DOFade(0.4f, 0.2f);
    }

    public void Hide()
    {

        _blackBack.DOFade(0f, 0.2f).OnComplete(() =>
        {
            Color c = _blackBack.color;
            c.a = 0f;
            _blackBack.color = c;
            _blackBack.DOKill();
            this.gameObject.SetActive(false);
        });
    }


    void Awake()
    {
        Instance = this;
        _blackBack = GetComponent<Image>();
    }

}
