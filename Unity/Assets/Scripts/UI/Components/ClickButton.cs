using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickButton : Button//, IPointerClickHandler
{
    public Action OnAnimationCallback;

    [SerializeField] private Color32 disabledColor;

    private Image image;
    private Color32 downColor;
    private Color32 defaultColor;
    private bool isDisabled = false;

    protected override void Awake()
    {
        base.Awake();

        downColor = new Color32(155, 155, 155, 255);
        image = GetComponent<Image>();
        defaultColor = image.color;

        onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (!gameObject.activeSelf || !IsInteractable())
            return;

        StopAllCoroutines();
        StartCoroutine(Click());
    }



    public void SetActiveButton(bool value)
    {
        //StopAllCoroutines();

        //image.color = value ? defaultColor : disabledColor;
        //isDisabled = !value;
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        
    }*/

    //void OnDisable()
    //{
    //   StopAllCoroutines();
    //}

    IEnumerator Click()
    {
        image.color = downColor;
        yield return new WaitForSeconds(0.1f);
        image.color = isDisabled ? disabledColor : Color.white;
        OnAnimationCallback?.Invoke();
        //OnClick?.Invoke();
    }
}
