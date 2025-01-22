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

namespace UI.ActionPanel
{
    public class UIActionPanel : MonoBehaviour
    {
        [SerializeField] private UIReward needed;
        [SerializeField] private UIReward randomReward;
        [SerializeField] private Text description;
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private UICurrent backpack;
        [SerializeField] private List<Color32> colors;

        [SerializeField] private UIReward reward;

        [SerializeField] private GameObject choiceTitlePanel;
        [SerializeField] private Text choiceName;
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Image nextCard;


        private Swipe swipe;

        private ProfileData profile => Services.Player.Profile;


        private SwipeData data;

        void Awake()
        {


            //followPrompt.gameObject.SetActive(false);
        }


        void OnSet(bool staticState)
        {
            HideAll();

            choicePanel.SetActive(true);

            DeckItem deckItem = SL.GetCurrentCard(profile);
            choiceTitlePanel.SetActive(false);
            nextCard.gameObject.SetActive(false);

            if (deckItem.State == CardData.NOTHING)
            {
                if (data.Card.Next.HasTriggers())
                {
                    var cards = new List<CardMeta>();
                    Services.Meta.GetAllRecursiveCardsFromGroup(data.Card.Next, cards);
                    RewardMeta[] cost = cards.SelectMany(c => c.Cost[0]).ToArray();
                    needed.SetItems(null, cost.ToList());
                }
            }
            else if (deckItem.State == CardData.REWARD)
            {
                randomReward.SetItems(
                    data.Card.Reward.HasReward() ? data.Card.Reward[0].Where(r => r.Chance > 0).ToList() : null,
                    data.Card.Cost.HasReward() ? data.Card.Cost[0].Where(r => r.Chance > 0).ToList() : null);

                if (Services.Player.RewardCollected.Count > 0)
                {
                    backpack.gameObject.SetActive(true);
                    backpack.SetItems(Services.Player.RewardCollected, profile, Services.Meta.Game, staticState);

                    //if (data.Card.RewardText == null)
                    //    throw new Exception($"Description is needed for reward {data.Card.Id}");
                }
            }

            if (Services.Player.TryGetCardDescription(data.Card, out string desc))
                SetDecription(desc);
        }


        public void Show(SwipeData data, Swipe swipe)
        {
            this.data = data;
            this.swipe = swipe;

#if UNITY_EDITOR
            MetaService.ShowUnvalidateCard(data.Card);
#endif

            OnSet(true);

            choicePanel.gameObject.SetActive(true);

            if (data.ChoiceMode)
                OnSwipeListenerEnable();

            gameObject.SetActive(true);
        }

        public void Hide()
        {

            HideAll();

            if (swipe)
                OnSwipeListenerDisable();

            gameObject.SetActive(false);
        }

        private void SetDecription(string desc, bool localize = true)
        {


            description.gameObject.SetActive(true);
            description.text = localize ? desc.Localize(LocalizePartEnum.CardDescription) : desc;

            if (desc.EndsWith("ask"))
                description.color = colors[1];
            else if (desc.EndsWith("tell"))
                description.color = colors[2];
            else
                description.color = colors[0];
        }

        private void HideAll()
        {
            randomReward.Hide();
            needed.Hide();
            backpack.Hide();

            // next.gameObject.SetActive(false);
            // next.GetComponent<RectTransform>().DOKill();
            description.gameObject.SetActive(false);
            rewardPanel.SetActive(false);

            //left.HideAll();
            //right.HideAll();
            //delem.SetActive(false);

            choicePanel.gameObject.SetActive(false);
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
            List<string> allReward = new List<string>();
            var cardMeta = data.Choices[obj];
            ShowChoice(cardMeta, profile.Choices[obj], false, allReward);
            //backpack.gameObject.SetActive(allReward.Count > 0);

            //if (allReward.Count > 0)
            //   backpack.SetItems(allReward.Distinct().Select(s => new ItemData(s, 0)).ToList(), profile, Services.Meta.Game);
        }

        private void OnDropCard()
        {
            OnSet(false);
        }

        private void OnTakeCard()
        {
            needed.Hide();
            backpack.Hide();
            description.gameObject.SetActive(false);
            rewardPanel.SetActive(true);

            OnChangeDirection(0);
        }

        private void ShowChoice(CardMeta cardMeta, ChoiceInfo info, bool showFollowPrompt, List<string> allRewards)
        {
            bool hasRewardOrCost = info.RewardIndex != -1 || info.CostIndex != -1;
            rewardPanel.SetActive(false);

            choiceTitlePanel.SetActive(true);
            choiceName.Localize(cardMeta.Name, LocalizePartEnum.CardName);

            nextCard.gameObject.SetActive(true);
            nextCard.LoadCardImage(cardMeta.Image);

            if (cardMeta.TradeLimit > 0)
            {
                //                 RewardMeta rr = new RewardMeta();
                //                 rr.Id = cardMeta.Reward[0][info.RewardIndex].Id;
                //                 rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, Services.Meta.Game);
                //                 //reward.SetItems(new RewardMeta[1] { rr }, null, false);
                //                 allRewards.Add(rr.Id);
                // 
                //                 rr = new RewardMeta();
                //                 rr.Id = cardMeta.Cost[0][info.CostIndex].Id;
                //                 rr.Count = SL.GetRewardMinCount(rr.Id, cardMeta, Services.Meta.Game);
                //                 // cost.SetItems(null, new RewardMeta[1] { rr }, false);
                //                 allRewards.Add(rr.Id);


                rewardPanel.SetActive(true);
            }
            else if (hasRewardOrCost)
            {
                rewardPanel.SetActive(true);

                List<RewardMeta> total = new List<RewardMeta>();
                if (info.RewardIndex != -1)
                    total.AddRange(cardMeta.Reward[info.RewardIndex].ToList());
                if (info.CostIndex != -1 && cardMeta.Cost.HasReward())
                    total.AddRange(cardMeta.Cost[info.CostIndex].ToList());

                reward.SetItems(
                    info.RewardIndex != -1 && cardMeta.Reward.HasReward() ? cardMeta.Reward[info.RewardIndex].ToList() : null,
                    info.CostIndex != -1 && cardMeta.Cost.HasReward() ? cardMeta.Cost[info.CostIndex].ToList() : null);

                allRewards.AddRange(total.Select(r => r.Id));

                //backpack.gameObject.SetActive(true);
                //backpack.SetItems(total.Select(r => new ItemData(r.Id, 0)).ToList(), profile, Services.Meta.Game, false);
            }
            else
            {
                reward.Hide();
            }

            //if (cardMeta.RewardText.HasText())
            {
                //    SetDecription(cardMeta.RewardText);
            }
            // else if (info.Next == info.Id && cardMeta.TradeLimit == 0)
            //     SetDecription(cardMeta.Shure.Localize(LocalizePartEnum.CardAction), false);
            // else if (cardMeta.Act.HasText())
            //     SetDecription(cardMeta.Act.Localize(LocalizePartEnum.CardAction), false);
            // else
            //     SetDecription(cardMeta.Name.Localize(LocalizePartEnum.CardName), false);


            //followPrompt.gameObject.SetActive(showFollowPrompt);


            // if (cardMeta.Hero != null)
            // {
            //     hero.LoadHeroImage(cadMeta.Hero);
            //     hero.gameObject.SetActive(true);
            // }
            // else
            //     hero.gameObject.SetActive(false);
            //             
        }

    }
}
