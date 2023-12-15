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

    private Color32 downColor;
    private Color32 defaultColor;
    private Sprite defaultSprite;
    private bool isDisabled = false;

    private Button button;
    private Image image;

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
        if (!gameObject.activeSelf || !button.interactable || isDisabled)
            return;

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



    // public void SetActiveButton(bool value)
    // {
    //     //StopAllCoroutines();

    //     //image.color = value ? defaultColor : disabledColor;
    //     //isDisabled = !value;
    // }

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

        yield return new WaitForSeconds(0.2f);

        if (asToggle)
        {
            SetAsDisabled = true;
        }

        OnClick?.Invoke();
    }
}
