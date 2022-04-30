using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class UI_BuildingTooltip : UI_InventoryTooltip, ITick
{

    private BuildingData buildingData;
    private UI_Reward reward;
    //private Button button;
    private Text text;


    private UI_AcceleratePanel acceleratePanel;

    public override void ShowTooltip(ItemData itemData)
    {

        gameObject.SetActive(true);
        this.itemData = itemData;
        //header.text = LocalizationManager.Localize(itemData.name);
        description.text = LocalizationManager.Localize(itemData.Des);

        buildingData = (BuildingData)itemData;

        BuildingVO buildingVO = null;//Services.Player.buildingHandler.GetVO(buildingData.id, 0);
        // acceleratePanel.SetTimer(4,);

        if (buildingVO.stact > 0 && buildingVO.stact > buildingVO.executed)
        {
            acceleratePanel.SetTimer(buildingVO.stact, buildingData.Act.Time);
        }
        else
        {
            acceleratePanel.SetTimer(0, 0);
        }
    }
    public override void Update()
    {
        //base.Update();
    }

    protected override void Start()
    {
        // reward.SetHeader ("Card.Reward");
        //  button.onClick.AddListener(() => HideTooltip());
    }
    protected override void Awake()
    {
        base.Awake();
        reward = transform.Find("UI_Reward").GetComponent<UI_Reward>();
        //button = transform.Find("Button").GetComponent<Button>();
        acceleratePanel = transform.Find("AcceleratePanel").GetComponent<UI_AcceleratePanel>();

    }

    public override void Tick(int timestamp)
    {

    }

    public override bool IsTickble()
    {
        return false;
    }
}