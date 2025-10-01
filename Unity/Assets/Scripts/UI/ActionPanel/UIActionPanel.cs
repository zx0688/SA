using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System.Data;
using UI.Components;
using System.Linq;
using System.Drawing;
using haxe.root;
using Random = UnityEngine.Random;
using UnityEditor;
using UnityJSON;
using UnityEngine.Assertions.Must;
using Unity.Android.Gradle.Manifest;

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {
        //[SerializeField] private UIReward needed;

        [SerializeField] private UIReward randomReward;
        [SerializeField] private GameObject randomPanel;

        [SerializeField] private Text description;
        [SerializeField] private Text descChoice;
        [SerializeField] private UICurrent backpack;
        [SerializeField] private List<Color32> colors;

        [SerializeField] private UIReward reward;

        //[SerializeField] private UIChoicePanel choicePanel;

        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Image nextCardImage;
        [SerializeField] private Text title;
        [SerializeField] private Image heroImage;
        [SerializeField] private Text rewardText;
        //[SerializeField] private UIChoicePanel leftChoice;
        //[SerializeField] private UIChoicePanel rightChoice;
        private List<ItemData> totalChangeItems;

        private Swipe swipe;

        private ProfileData profile => Services.Player.Profile;
        private GameMeta meta => Services.Meta.Game;
        private string descriptionText;


        private SwipeData data;

        void Awake()
        {
            HideAll();
        }


        void OnSet()
        {
            HideAll();

            DeckItem deckItem = data.Item;
            rewardText.Localize("Card.Reward", LocalizePartEnum.GUI);


            if (deckItem.S == CardData.REWARD && Services.Player.RewardCollected.Count > 0)
            {
                rewardPanel.SetActive(true);
                var add = Services.Player.RewardCollected.Where(r => r.Count > 0).ToReward().ToList();
                var sub = Services.Player.RewardCollected.Where(r => r.Count < 0).ToReward()
                    .Select(r => { r.Count = -r.Count; return r; }).ToList();
                reward.SetItems(add, sub);
            }

            if (deckItem.S == CardData.REWARD || deckItem.S == CardData.NOTHING)
            {
                var addr = data.Card.Reward.HasReward() ? data.Card.Reward[0].Where(r => r.Chance > 0).ToList() : null;
                var subr = data.Card.Cost.HasReward() ? data.Card.Cost[0].Where(r => r.Chance > 0).ToList() : null;
                randomPanel.SetActive((addr != null && addr.Count > 0) || (subr != null && subr.Count > 0));
                randomReward.SetItems(addr, subr);
            }

            SetDecription(descriptionText, description);

        }


        public void Show(SwipeData data, Swipe swipe)
        {
            this.data = data;
            this.swipe = swipe;

#if UNITY_EDITOR
            MetaService.ShowUnvalidateCard(data.Card);
#endif

            descriptionText = Services.Player.TryGetCardDescription(data.Card, out string desc) ?
                desc : "";

            if (data.Item.Ch.Count > 1)
            {
                //createBackpack();
                OnSwipeListenerEnable();
            }

            OnSet();
            gameObject.SetActive(true);
        }

        public void Hide()
        {

            HideAll();

            if (swipe)
                OnSwipeListenerDisable();

            gameObject.SetActive(false);
        }

        private void SetDecription(string desc, Text field, bool localize = true)
        {
            field.gameObject.SetActive(true);
            field.text = localize ? desc.Localize(LocalizePartEnum.CardDescription) : desc;

            if (desc.EndsWith("ask"))
                field.color = colors[1];
            else if (desc.EndsWith("tell"))
                field.color = colors[2];
            else
                field.color = colors[0];
        }

        private void HideAll()
        {
            //choicePanel.Hide();
            nextCardImage.gameObject.SetActive(false);
            randomReward.Hide();
            descChoice.gameObject.SetActive(false);
            //needed.Hide();
            backpack.Hide();
            heroImage.gameObject.SetActive(false);

            randomPanel.SetActive(false);

            // next.gameObject.SetActive(false);
            // next.GetComponent<RectTransform>().DOKill();
            description.gameObject.SetActive(false);
            rewardPanel.SetActive(false);

            //left.HideAll();
            //right.HideAll();
            //delem.SetActive(false);

        }
        private void OnSwipeListenerDisable()
        {
            swipe.OnTakeCard -= OnTakeCard;
            swipe.OnDrop -= OnDropCard;
            swipe.OnChangeDirection -= OnChangeDirection;
            swipe.OnEndSwipe -= OnEndSwipe;
        }

        private void OnSwipeListenerEnable()
        {
            swipe.OnTakeCard += OnTakeCard;
            swipe.OnDrop += OnDropCard;
            swipe.OnChangeDirection += OnChangeDirection;
            swipe.OnEndSwipe += OnEndSwipe;
        }

        private void OnEndSwipe()
        {
            HideAll();
        }

        private void OnChangeDirection(int obj)
        {
            var cardMeta = data.Choices[obj];
            ShowChoice(cardMeta, data.Item.Ch[obj], false);
        }

        private void OnDropCard()
        {
            OnSet();
        }

        private void OnTakeCard()
        {
            //needed.Hide();
            description.gameObject.SetActive(false);

            if (data.Item.Ch.Count > 0)
            {
                //leftChoice.Hide();
                //rightChoice.Hide();
                backpack.Hide();


                //madeChoicePanel.gameObject.SetActive(false);
                OnChangeDirection(0);
            }


        }


        private void SetHero(CardMeta card, string text)
        {
            if (text != null && text.EndsWith("ask"))
            {
                heroImage.LoadHeroImage(Services.Player.Profile.Hero);
                heroImage.gameObject.SetActive(true);
            }
            else
            {
                heroImage.gameObject.SetActive(false);
            }
        }

        private void ShowChoice(CardMeta cardMeta, ChoiceInfo info, bool showFollowPrompt)
        {
            bool hasRewardOrCost = info.RI != -1 || info.CI != -1;
            rewardPanel.SetActive(false);

            nextCardImage.gameObject.SetActive(true);
            nextCardImage.LoadCardImage(cardMeta.Image);

            title.Localize(cardMeta.Name, LocalizePartEnum.CardName);

            string text = "";

            if (cardMeta.Shure != null && this.data.Card.Id == cardMeta.Id)
            {
                text = cardMeta.Shure.Localize(LocalizePartEnum.CardAction);
            }
            else if (cardMeta.Act != null)
            {
                SetHero(cardMeta, cardMeta.Act);

                if (cardMeta.Act.EndsWith("ask") || cardMeta.Act.EndsWith("tell"))
                {
                    SetDecription(cardMeta.Act, descChoice, true);
                    return;
                }
                else
                    text = cardMeta.Act.Localize(LocalizePartEnum.CardAction);

            }
            else if (cardMeta.Cost.HasReward())
            {
                text = Services.Assets.Localize("spend1", LocalizePartEnum.CardAction);
            }
            else if (cardMeta.Reward.HasReward())
            {
                text = Services.Assets.Localize("learn1", LocalizePartEnum.CardAction);
            }
            else
            {
                text = cardMeta.Name.Localize(LocalizePartEnum.CardName);
            }

            List<RewardMeta> _reward = null;
            List<RewardMeta> _cost = null;
            if (data.Card.TradeLimit > 0)
            {
                var tradeSkill = SL.GetSkillValue("1", profile, meta, -5);
                RewardMeta rr = new RewardMeta();
                rr.Id = cardMeta.Reward[0][info.RI].Id;
                rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, profile, meta, tradeSkill);
                _reward = new List<RewardMeta> { rr };

                rr = new RewardMeta();
                rr.Id = cardMeta.Cost[0][info.CI].Id;
                rr.Count = SL.GetCostMinCount(rr.Id, cardMeta, profile, meta, tradeSkill);
                _cost = new List<RewardMeta> { rr };
            }
            else
            {
                _reward = cardMeta.Reward.GetReward().Where(r => r.Chance == 0).ToList();
                _cost = cardMeta.Cost.GetReward().Where(c => c.Chance == 0).ToList();
            }

            reward.SetItems(_reward, _cost);
            randomReward.SetItems(
                cardMeta.Reward.GetReward().Where(r => r.Chance > 0).ToList(),
                cardMeta.Cost.GetReward().Where(c => c.Chance > 0).ToList());
            randomPanel.SetActive(randomReward.HasReward);

            if (reward.HasReward)
            {
                rewardPanel.SetActive(true);
                rewardText.Localize("Choose", LocalizePartEnum.GUI);
            }

            SetDecription(text, descChoice, false);
        }

        private void createBackpack()
        {
            List<string> total = new List<string>();

            if (data.Choices[0].Reward != null)
                total.AddRange(data.Choices[0].Reward
                    .SelectMany(row => row)
                    .Select(r => r.Id)
                    .ToList());
            if (data.Choices[0].Cost != null)
                total.AddRange(data.Choices[0].Cost
                    .SelectMany(row => row)
                    .Select(r => r.Id)
                    .ToList());
            if (data.Choices[1].Reward != null)
                total.AddRange(data.Choices[1].Reward
                    .SelectMany(row => row)
                    .Select(r => r.Id)
                    .ToList());
            if (data.Choices[1].Cost != null)
                total.AddRange(data.Choices[1].Cost
                    .SelectMany(row => row)
                    .Select(r => r.Id)
                    .ToList());
            totalChangeItems = total.Distinct().Select(id => new ItemData(id, 0)).ToList();
        }

    }
}
