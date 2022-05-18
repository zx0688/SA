using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Data;
using UnityEngine;
using UnityEngine.UI;


public class UI_AcceleratePanel : ServiceBehaviour
{
    // Start is called before the first frame update

    public Button accelerateBtn;
    public Button buyBtn;
    [HideInInspector]
    public UI_InventoryItem iconItem;
    [HideInInspector]
    public UI_InventoryItem accelerateItem;

    private ItemVO iconItemVO;
    private ItemVO accelerateItemVO;
    private int availableCount;
    private Coroutine timer;
    private int timePerItem;
    private Text actionTf;

    private int start;
    private int wait;

    void Start()
    {
        start = 0;
        wait = 0;
        accelerateBtn.onClick.AddListener(() => Accelerate());
        buyBtn.onClick.AddListener(() => Buy());
    }

    void OnEnable()
    {
        StopAllCoroutines();

        if (Services.isInited && iconItemVO != null)
        {
            availableCount = Services.Player.AvailableItem(iconItemVO.Id);
            SetItem(availableCount, GameTime.Current);
            timer = StartCoroutine(Tick());
            timer = null;
        }

    }

    public void SetTimer(int start, int wait)
    {
        this.start = start;
        this.wait = wait;
        OnProfileUpdated();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        timer = null;
    }

    protected override void OnServicesInited()
    {
        base.OnServicesInited();
        return;
        /*iconItemVO = new ItemVO(ItemMeta.ACCELERATE_ID, 0);

        Services.Player.OnProfileUpdated += OnProfileUpdated;
        timePerItem = Services.Data.Meta.Config.Accelerate;

        List<RewardData> priceData = new List<RewardData>();//Services.Data.GameData.Config.Price;
        ItemVO ivo = new ItemVO(ItemMeta.ACCELERATE_ID, priceData[0].Id);
        buyBtn.transform.Find("Price").GetComponent<Text>().text = priceData[0].Count.ToString();
        buyBtn.GetComponent<UI_InventoryItem>().SetItem(ivo);

        accelerateItemVO = new ItemVO(ItemMeta.ACCELERATE_ID, 0);
        accelerateItem.SetItem(accelerateItemVO);

        availableCount = Services.Player.AvailableItem(iconItemVO.Id);
        SetItem(availableCount, GameTime.Current);*/
    }
    private void OnProfileUpdated()
    {
        if (gameObject.activeInHierarchy == false)
            return;

        availableCount = Services.Player.AvailableItem(iconItemVO.Id);
        if (GameTime.Left(GameTime.Current, start, wait) > 0)
        {
            timer = StartCoroutine(Tick());
            timer = null;
        }
        else
        {
            StopAllCoroutines();
            timer = null;
        }

        SetItem(availableCount, GameTime.Current);
    }

    private void SetItem(int availableCount, int timestamp)
    {

        accelerateBtn.gameObject.SetActive(availableCount > 0);
        buyBtn.gameObject.SetActive(availableCount <= 0);
        actionTf.text = LocalizationManager.Localize(availableCount > 0 ? "accelerate" : "buyfor");

        if (availableCount > 0)
        {
            float timeLeft = GameTime.Left(timestamp, start, wait);
            int count = Mathf.CeilToInt(timeLeft / timePerItem);
            count = count < 0 ? 0 : count;
            accelerateItemVO.Count = Math.Min(availableCount, count);
            accelerateItem.SetItem(accelerateItemVO);
        }

        iconItemVO.Count = availableCount;
        iconItem.SetItem(iconItemVO);
    }

    IEnumerator Tick()
    {
        while (true)
        {
            SetItem(availableCount, GameTime.Current);
            yield return new WaitForSeconds(1f);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        accelerateBtn = transform.Find("ButtonAccelerate").GetComponent<Button>();
        buyBtn = transform.Find("ButtonBuy").GetComponent<Button>();
        actionTf = transform.Find("AccelerateTF").GetComponent<Text>();

        iconItem = transform.Find("UI_Item").GetComponent<UI_InventoryItem>();
        accelerateItem = accelerateBtn.GetComponent<UI_InventoryItem>();
    }
    private void Buy()
    {
        Services.Player.Buy(GameTime.Current);
    }
    private void Accelerate()
    {
        if (accelerateItemVO.Count == 0)
            return;
        if (wait <= 0)
            return;
        if (wait <= timePerItem)
            return;
        int available = Services.Player.AvailableItem(ItemMeta.ACCELERATE_ID);
        if (available <= 0)
            return;

        int timestamp = GameTime.Current;
        SetItem(available, timestamp);

        Services.Player.Accelerate(timestamp, accelerateItemVO.Count);
    }

}