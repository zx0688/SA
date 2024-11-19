using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickButton : MonoBehaviour
{
    public Action OnClick;

    [SerializeField] private Color32 disabledColor;
    [SerializeField] private Sprite disabledSprite;
    [SerializeField] private bool asToggle = false;
    [SerializeField] private float waitAnimation = 0.2f;

    private Color32 downColor;
    private Color32 defaultColor;
    private Sprite defaultSprite;
    private bool isDisabled = false;

    private Button button;
    private Image image;
    private Boolean waitIsPocess;

    void Awake()
    {
        downColor = new Color32(155, 155, 155, 255);
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        defaultColor = image.color;
        defaultSprite = image.sprite;
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (!gameObject.activeSelf || !button.interactable || isDisabled || waitIsPocess)
            return;

        waitIsPocess = true;
        StopAllCoroutines();
        StartCoroutine(Click());
    }

    public bool SetAsDisabled
    {
        get => isDisabled;
        set
        {
            button.interactable = !value;
            isDisabled = value;

            if (disabledSprite != null)
            {
                image.sprite = value ? disabledSprite : defaultSprite;
            }
            else
            {
                image.color = value ? defaultColor : downColor;
            }

        }
    }

    public bool SetAsToggled
    {
        get => asToggle;
        set => asToggle = value;
    }


    IEnumerator Click()
    {
        if (disabledSprite != null)
        {
            image.sprite = disabledSprite;
        }
        else
        {
            image.color = downColor;
        }

        yield return new WaitForSeconds(waitAnimation);

        if (asToggle)
        {
            SetAsDisabled = true;
        }
        else if (disabledSprite != null)
        {
            image.sprite = defaultSprite;
        }
        else
        {
            image.color = defaultColor;
        }

        waitIsPocess = false;
        OnClick?.Invoke();
    }
}
