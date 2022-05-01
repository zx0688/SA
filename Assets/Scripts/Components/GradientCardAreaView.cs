using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.UI;

public class GradientCardAreaView : MonoBehaviour
{
    // Start is called before the first frame update

    private UIGradient gradient;
    private Image image;
    private RectTransform rectTransform;

    private SwipeData cardParam;

    void Start()
    {
        gameObject.SetActive(false);
        OnChangeDeviation(0);

        gradient = GetComponent<UIGradient>();
        image = GetComponent<Image>();

        Swipe.OnChangeDeviation += OnChangeDeviation;
        Swipe.OnChangeDirection += OnChangeDirection;
        Swipe.OnStartSwipe += OnStartSwipe;
        Swipe.OnEndSwipe += OnEndSwipe;
    }

    private void OnChangeDirection(int choice)
    {

        if (!cardParam.Left.Available && choice == Swipe.RIGHT_CHOICE)
        {
            gradient.m_angle = 90f;
            rectTransform.anchoredPosition = new Vector2(950f, 0f);
        }
        else if (!cardParam.Left.Available && choice == Swipe.LEFT_CHOICE)
        {
            gradient.m_angle = -90f;
            rectTransform.anchoredPosition = new Vector2(0f, 0f);
        }
    }

    private void OnEndSwipe()
    {
        gameObject.SetActive(false);
    }

    private void OnStartSwipe()
    {

        //cardParam = Deck.instance.currentData;
        rectTransform = gameObject.GetComponent<RectTransform>();

        if (!cardParam.Right.Available || !cardParam.Left.Available)
            gameObject.SetActive(true);
    }

    private void OnChangeDeviation(float i)
    {
        if (gameObject.activeSelf && i >= 0)
        {
            Color tempColor = image.color;
            tempColor.a = i;
            image.color = tempColor;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}