using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CARD_NewLevel : CARD_Base, ICardHUD {

    protected UI_Reward reward;
    //protected CanvasGroup canvasGroupReward;
    protected override void Awake () {
        base.Awake ();
        reward = GetComponentInChildren<UI_Reward> ();
        //canvasGroupReward = reward.GetComponentInChildren<CanvasGroup> ();
        
    }

    protected override async void UpdateHUD () {

        title.text = LocalizationManager.Localize ("Новый уровень");
        description.text = 15 + ' ' + LocalizationManager.Localize ("уровень");

        ExpData expData = new ExpData();

       // reward.PlaceAround ();
       // reward.gameObject.SetActive (true);
       // reward.SetAsItems (expData.reward);
    }

    public override void OnStartSwipe () {

    }

    public override void OnChangeDirection (int obj) {
       
    }

    public override void OnDrop () {

    }

}