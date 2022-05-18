using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.UI;



public class UI_BuildingItem : MonoBehaviour, ITick
{

    protected BuildingVO buildingVO;
    protected BuildingMeta data;

    [HideInInspector]
    protected GameObject coin;

    [HideInInspector]
    protected GameObject timerPanel;

    [HideInInspector]
    protected Text timer;

    [HideInInspector]
    protected Text count;

    public Button showTooltipBtn;

    [HideInInspector]
    public Image icon;
    protected bool isEmpty;
    protected UI_BuildingTooltip tooltip;

    private Image rewardImage;
    private Text rewardCount;

    void Start()
    {

        //if (tooltip != null)
        showTooltipBtn.onClick.AddListener(OnClick);
    }

    public void SetItem(ItemVO item)
    {

        if (item == null)
        {
            Clear();
            return;
        }

        /* count.text = item.Count.ToString();
         isEmpty = false;

         int timestamp = GameTime.Current;

         if (this.data != null && this.data.Id == item.Id)
         {
             Tick(timestamp);
             UpdateView(timestamp);
             return;
         }

         buildingVO = item as BuildingVO;

         data = Services.Data.BuildingInfo(item.Id);

         rewardCount.text = data.Act.Reward[0].Count.ToString();

         icon.enabled = true;
         count.enabled = true;
         rewardImage.enabled = true;
         rewardCount.enabled = true;

         if (tooltip != null)
             showTooltipBtn.interactable = true;

         Services.Assets.SetSpriteIntoImage(icon, "Buildings/" + item.Id + "/icon", true).Forget();
         Services.Assets.SetSpriteIntoImage(rewardImage, "Items/" + data.Act.Reward[0].Id + "/icon", true).Forget();

         UpdateView(timestamp);
         Tick(timestamp);*/
    }

    protected void UpdateView(int timestamp)
    {

        if (buildingVO.Stact > 0 && buildingVO.Stact > buildingVO.Executed && GameTime.Left(timestamp, buildingVO.Stact, data.Act.Time) <= 0)
        {
            coin.SetActive(true);
            timerPanel.SetActive(false);
        }
        else if (buildingVO.Stact > 0 && GameTime.Left(timestamp, buildingVO.Stact, data.Act.Time) > 0)
        {
            timerPanel.SetActive(true);
            coin.SetActive(false);
        }
        else
        {
            coin.SetActive(false);
            timerPanel.SetActive(false);
        }
    }

    public void Clear()
    {
        timerPanel.SetActive(false);

        data = null;
        isEmpty = true;
        icon.enabled = false;
        icon.sprite = null;

        rewardImage.enabled = false;
        rewardImage.sprite = null;

        count.enabled = false;

        if (tooltip != null)
            showTooltipBtn.interactable = false;

        coin.SetActive(false);
    }

    void Awake()
    {

        count = transform.Find("CountBuilding").GetComponent<Text>();
        coin = transform.Find("Coin").gameObject;
        rewardImage = coin.transform.Find("Image").GetComponent<Image>();
        rewardCount = coin.transform.Find("Count").GetComponent<Text>();

        timerPanel = transform.Find("TimerPanel").gameObject;
        timer = timerPanel.transform.Find("Timer").GetComponent<Text>();
        icon = transform.Find("ImageBuilding").GetComponent<Image>();
    }

    protected void OnClick()
    {
        int timestamp = GameTime.Current;
        if (buildingVO.Stact > 0 && buildingVO.Stact > buildingVO.Executed && GameTime.Left(timestamp, buildingVO.Stact, data.Act.Time) <= 0)
        {
            // Services.Player.Trigger(Deck.instance.queue, new TriggerVO(TriggerData.BUILD, data.Id, TriggerData.CHOICE_PRODUCTION, null, null, data, null), new List<RewardData>(), timestamp);
            // SetItem(buildingVO);
            UpdateView(timestamp);
        }
        else
            tooltip.ShowTooltip(data);

    }

    public void Tick(int timestamp)
    {

        int timeLeft = GameTime.Left(timestamp, buildingVO.Stact, data.Act.Time);
        if (timeLeft <= 0)
            UpdateView(timestamp);
        else
            timer.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft);

    }

    public void SetTooltip(UI_BuildingTooltip tooltip)
    {
        this.tooltip = tooltip;
    }

    public bool IsTickble()
    {
        return buildingVO != null && buildingVO.Stact > 0 && buildingVO.Executed < buildingVO.Stact;
    }

}