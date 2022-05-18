using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour, IPointerClickHandler
{
    public Action OnClick;

    [SerializeField] private Sprite _default;
    //[SerializeField] private Sprite _down;
    private Image _image;
    private Color32 _downColor;

    private void Awake()
    {
        _downColor = new Color32(155, 155, 155, 255);
        _image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
        StopAllCoroutines();

        StartCoroutine(Click());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Click()
    {
        _image.color = _downColor;
        yield return new WaitForSeconds(0.1f);
        _image.color = Color.white;
    }
}
