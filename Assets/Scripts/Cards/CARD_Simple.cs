using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

public class CARD_Simple : CARD_Base, ICardHUD
{

    protected UI_Reward reward;
    protected bool hasReward;
    protected CanvasGroup canvasGroupReward;
    protected GameObject chanceTf;
    protected Text chanceValue;
    private bool isOneReward;
    private GameObject costPanel;
    private UI_RewardItem costItem;

    protected override void Awake()
    {
        base.Awake();
        reward = GetComponentInChildren<UI_Reward>();
        canvasGroupReward = reward.GetComponentInChildren<CanvasGroup>();
        chanceTf = transform.Find("Chance").gameObject;
        chanceValue = chanceTf.transform.Find("ChanceValue").GetComponent<Text>();
        costPanel = transform.Find("CostPanel").gameObject;
        costItem = costPanel.GetComponentInChildren<UI_RewardItem>();
    }

    protected override async void UpdateHUD()
    {

        base.UpdateHUD();

        hasReward = (data.Right.action.Reward.Count > 0) || (data.Left.action != null && data.Left.action.Reward.Count > 0);

        costPanel.SetActive(false);
        chanceTf.SetActive(false);
        reward.gameObject.SetActive(false);
        hero.gameObject.SetActive(false);
        isOneReward = false;
        chanceValue.color = Color.yellow;

        if (hasReward)
        {

            if (data.Left.action == null || data.Left.action.Reward.Count == 0)
            {
                if (data.Right.Chance > 0)
                {
                    chanceTf.SetActive(true);
                    if (data.Right.Chance > data.Right.action.Chance)
                        chanceValue.color = Color.green;
                    else if (data.Right.Chance < data.Right.action.Chance)
                        chanceValue.color = Color.red;

                    chanceValue.text = data.Right.Chance + "%";
                }

                RewardData cost = data.Right.action.Reward.Find(r => r.Count < 0);
                RewardData buy = data.Right.action.Reward.Find(r => r.Count > 0);

                if (cost != null && buy != null)
                {
                    costPanel.SetActive(true);
                    cost = cost.Clone();
                    cost.Count = Math.Abs(cost.Count);
                    costItem.SetItem(cost);
                }

                if (data.Right.action.Reward.Count == 1 && data.Right.action.Reward[0].Count == 1)
                {
                    // hero.gameObject.SetActive (true);
                    int id = data.Right.action.Reward[0].Id;
                    isOneReward = true;
                    switch (data.Right.action.Reward[0].Tp)
                    {
                        case DataService.SKILL_ID:
                            Services.Assets.SetSpriteIntoImage(hero, "Skills/" + id + "/icon", true).Forget();
                            break;
                        case DataService.ITEM_ID:
                            //Services.Assets.SetSpriteIntoImage (back, "Actions/back", true).Forget ();
                            Services.Assets.SetSpriteIntoImage(hero, "Items/" + id + "/icon", true).Forget();
                            break;
                        case DataService.BUILDING_ID:
                            Services.Assets.SetSpriteIntoImage(hero, "Buildings/" + id + "/icon", true).Forget();
                            break;
                    }

                }
                else
                {
                    reward.gameObject.SetActive(true);
                    reward.SetItems(data.Right.action.Reward);
                }

            }
            else if (data.Right.action.Reward.Count == 0)
            {
                if (data.Left.Chance > 0)
                {
                    chanceTf.SetActive(true);
                    if (data.Left.Chance > data.Left.action.Chance)
                        chanceValue.color = Color.green;
                    else if (data.Left.Chance < data.Left.action.Chance)
                        chanceValue.color = Color.red;
                    chanceValue.text = data.Left.Chance + "%";
                }
                reward.gameObject.SetActive(true);
                reward.SetItems(data.Left.action.Reward);
            }
        }

        if (data.Left.action == null)
        {
            choice.text = LocalizationManager.Localize(data.Right.action.Text);
            choice.gameObject.SetActive(true);
        }
        else
            choice.gameObject.SetActive(false);

        if (data.CardData.Hero == -1)
        {
            reward.gameObject.SetActive(false);
            title.gameObject.SetActive(false);
            hero.gameObject.SetActive(true);
            //Utils.SetAlpha(art, 0.6f);
        }
        else if (data.CardData.Hero > 0)
        {
            reward.gameObject.SetActive(false);
            //Utils.SetAlpha(art, 0.6f);
            ItemData heroData = Services.Data.SkillInfo(data.CardData.Hero);
            //title.text = heroData.Name;
            hero.gameObject.SetActive(true);
            Services.Assets.SetSpriteIntoImage(hero, "Heroes/" + data.CardData.Hero + "/icon", true).Forget();
        }
        else if (hero != null)
        {

            //Utils.SetAlpha(art, 1f);
        }

        // description.text = data.cardData.text != null ? LocalizationManager.Localize (data.cardData.text) : LocalizationManager.Localize (data.cardData.id + "cardd");
        //hero.gameObject.SetActive (true);
        //canvasGroupReward.DOKill ();
        //canvasGroupReward.alpha = 1f;
    }

    public override void OnTakeCard()
    {
        //canvasGroupReward.alpha = 0f;
        base.OnTakeCard();

        choice.gameObject.SetActive(true);
        reward.gameObject.SetActive(hasReward && data.CardData.Hero == 0 && isOneReward == false);

        if (data.Left.action != null && data.Left.Chance > 0)
        {
            chanceTf.SetActive(true);
            chanceValue.text = data.Left.Chance + "%";
        }
        else if (data.Right.Chance > 0)
        {
            chanceTf.SetActive(true);
            chanceValue.text = data.Right.Chance + "%";
        }

        if (hasReward && hero.gameObject.activeSelf)
        {
            // Utils.SetAlpha(hero, 0.2f);
            //hero.gameObject.SetActive (false);
        }
        if (art.gameObject.activeSelf)
        {
            // Utils.SetAlpha (art, 0.8f);
        }
    }

    public override void OnChangeDirection(int obj)
    {

        base.OnChangeDirection(obj);

        /* if (!isRewarded) {
             reward.gameObject.SetActive (false);
             return;
         }*/

        //canvasGroupReward.DOKill ();
        //canvasGroupReward.DOFade (0f, 0.1f).OnComplete (() => {

        if (data.Left.action == null)
        {

            if (data.Right.Chance > 0)
            {
                chanceValue.text = data.Right.Chance + "%";
            }
            if (hasReward && data.Right.action.Reward.Count > 0 && isOneReward == false)
                reward.SetItems(data.Right.action.Reward);

        }
        else if (obj == Swipe.LEFT_CHOICE)
        {

            if (data.Left.Chance > 0)
            {
                chanceValue.text = data.Left.Chance + "%";
            }

            chanceTf.SetActive(data.Left.Chance > 0);
            if (hasReward && data.Left.action.Reward.Count > 0)
                reward.SetItems(data.Left.action.Reward);

            choice.text = LocalizationManager.Localize(data.Left.action.Text);

        }
        else if (obj == Swipe.RIGHT_CHOICE)
        {

            if (data.Right.Chance > 0)
            {
                chanceValue.text = data.Right.Chance + "%";
            }
            chanceTf.SetActive(data.Right.Chance > 0);
            if (hasReward && data.Right.action.Reward.Count > 0)
                reward.SetItems(data.Right.action.Reward);

            choice.text = LocalizationManager.Localize(data.Right.action.Text);

        }

        // reward.PlaceAround ();
        //canvasGroupReward.DOFade (1f, 0.1f);

        // });
    }

    public override void OnDrop()
    {

        base.OnDrop();

        if (data.Left.action != null && data.Left.action.Text != null)
            choice.gameObject.SetActive(false);

        if (hasReward)
        {
            chanceTf.SetActive(data.Left.action == null && data.Right.Chance > 0);
            reward.gameObject.SetActive(isOneReward == false && (data.Left.action == null || data.Right.action.Reward.Count == 0));
        }
        //reward.gameObject.SetActive (false);

        if (hasReward && hero.gameObject.activeSelf)
        {
            //Utils.SetAlpha(hero, 1f);

        }
        if (art != null)
        {
            //art.gameObject.SetActive (true);
            // Utils.SetAlpha (art, 1f);
        }

        // reward.gameObject.SetActive (false);

        /*if (data == null || gameObject.activeSelf == false || !isRewarded) {
            reward.gameObject.SetActive (false);
            return;
        }*/

        /*canvasGroupReward.DOFade (0f, 0.1f).OnComplete (() => {
            reward.gameObject.SetActive (false);
            canvasGroupReward.DOKill ();
        });*/

    }

}