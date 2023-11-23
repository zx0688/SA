using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Core;
using Cysharp.Threading.Tasks;
using Meta;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_BuildingTooltip : MonoBehaviour, ITick
{

    private BuildingMeta buildingData;
    private UI_Reward reward;
    //private Button button;
    private Text text;
    private Text description;
    private ItemMeta _meta;

    private UIAcceleratePanel acceleratePanel;

    public void ShowTooltip(ItemMeta meta)
    {

        gameObject.SetActive(true);
        this._meta = meta;
        //header.text = LocalizationManager.Localize(itemData.name);
        description.text = LocalizationManager.Localize(meta.Des);

        buildingData = (BuildingMeta)meta;

        BuildingVO buildingVO = null;//Services.Player.buildingHandler.GetVO(buildingData.id, 0);
        // acceleratePanel.SetTimer(4,);

        if (buildingVO.Stact > 0 && buildingVO.Stact > buildingVO.Executed)
        {
            //acceleratePanel.SetTimer(buildingVO.Stact, buildingData.Act.Time);
        }
        else
        {
            //acceleratePanel.SetTimer(0, 0);
        }
    }
    public void Update()
    {
        //base.Update();
    }



    public void Tick(int timestamp)
    {

    }

    public bool IsTickble()
    {
        return false;
    }
}