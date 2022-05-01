using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using Controllers;
using Meta;
using UnityEngine;
using UnityEngine.UI;

public class SliderQuant : MonoBehaviour
{
    [SerializeField]
    public int defaultValue = 0;

    [SerializeField]
    public Sprite disable;
    [SerializeField]
    public Sprite enable;
    [SerializeField]
    public List<GameObject> items;

    [SerializeField]
    public GameObject gameScripts;

    private int currentValue;

    private bool showAction;

    // private CardData data;

    // Start is called before the first frame update
    void Start()
    {

        showAction = false;
        currentValue = maxValue;

        for (int i = 0; i < maxValue; i++)
        {
            items[i].SetActive(true);
        }

        Swipe.OnStartSwipe += OnStartShake;
        Swipe.OnEndSwipe += OnStopShake;
        Swipe.OnDrop += OnStopShake;
        Swipe.OnChangeDirection += OnChangeDirection;

        SetValue(defaultValue);
    }

    private void OnChangeDirection(int direction)
    {

        //if (showAction == false)
        //    return;

        int resId = GetComponent<ItemStateController>().itemId;

        ActionData choiseData = null;
        //if (cardItem.me == true)
        //   choiseData = direction == SwipeDirection.LEFT ? cardItem.data.left : cardItem.data.right;
        //else
        /*choiseData = direction == Swipe.LEFT_CHOICE ? data.left : data.right;

        List<RewardData> result = new List<RewardData> ();
        //Services.Data.GetResourceReward(result, choiseData.reward, 0);
        result = choiseData.reward;
        RewardData rc = result.Find (r => r.count < 0 && r.id == resId && r.tp == DataManager.ITEM_ID);
        RewardData rr = result.Find (r => r.count > 0 && r.id == resId && r.tp == DataManager.ITEM_ID);

        for (int i = 0; i < maxValue; i++) {
            items[i].GetComponent<ShakeComponent> ().shake = false;
            if (rc != null && i >= (currentValue + rc.count) && i < currentValue) {
                items[i].GetComponent<ShakeComponent> ().shake = true;
            } else if (rr != null && i < currentValue + rr.count && i >= currentValue) {
                items[i].GetComponent<ShakeComponent> ().shake = true;
            }
        }*/
    }

    private void OnStopShake()
    {
        // showAction = false;

        for (int i = 0; i < maxValue; i++)
        {
            items[i].GetComponent<ShakeComponent>().shake = false;
        }
    }

    private void OnStartShake()
    {
        //showAction = true;
        //cardItem = gameScripts.GetComponent<GameLoop> ().cardItem;
    }

    private void UpdateValue()
    {
        for (int i = 0; i < maxValue; i++)
        {
            Image image = items[i].GetComponent<Image>();
            if (i < currentValue)
            {
                image.sprite = enable;
                items[i].GetComponent<Animator>().SetBool("on", true);
            }
            else
            {
                image.sprite = disable;
                items[i].GetComponent<Animator>().SetBool("on", false);
            }
            items[i].GetComponent<ShakeComponent>().shake = false;
        }
    }

    public int maxValue
    {
        get { return items.Count; }
    }

    public int GetValue()
    {
        return currentValue;
    }

    public void SetValue(int value)
    {
        currentValue = value;
        UpdateValue();
    }
    //private int value;

    // Update is called once per frame
    void Update()
    {

    }
}