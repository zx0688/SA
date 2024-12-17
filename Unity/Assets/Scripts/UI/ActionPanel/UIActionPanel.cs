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
        //[SerializeField] private Text name;
        [SerializeField] private UIReward needed;
        //[SerializeField] private UIChoicePanel left;
        //[SerializeField] private UIChoicePanel right;

        [SerializeField] private UIReward randomReward;

        [SerializeField] private Text description;
        [SerializeField] private GameObject descPanel;

        //[SerializeField] private GameObject delem;
        [SerializeField] private UICurrent backpack;
        //[SerializeField] private Text next;

        [SerializeField] private List<Color32> colors;

        private Swipe swipe;

        private ProfileData profile => Services.Player.Profile;


        private SwipeData data;

        void Awake()
        {


            //followPrompt.gameObject.SetActive(false);
        }


        void OnSet()
        {
            HideAll();

            DeckItem deckItem = SL.GetCurrentCard(profile);
            if (deckItem.State == CardData.NOTHING)
            {
                if (data.Card.Next.HasTriggers())
                {
                    var cards = new List<CardMeta>();
                    Services.Meta.GetAllRecursiveCardsFromGroup(data.Card.Next, cards);
                    RewardMeta[] cost = cards.SelectMany(c => c.Cost[0]).ToArray();
                    needed.SetItems(null, cost, false);
                }
            }
            else if (deckItem.State == CardData.REWARD)
            {
                if (data.Card.Reward != null)
                    randomReward.SetItems(data.Card.Reward[0].Where(r => r.Chance > 0).ToArray(), null, false);

                if (Services.Player.RewardCollected.Count > 0)
                {
                    backpack.gameObject.SetActive(true);
                    backpack.SetItems(Services.Player.RewardCollected, profile, Services.Meta.Game);

                    if (data.Card.RewardText == null)
                        throw new Exception($"Description is needed for reward {data.Card.Id}");
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

            OnSet();

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            //conditions.Hide();
            //rewardLeft.Hide();
            //description.gameObject.SetActive(false);
            //followPrompt.gameObject.SetActive(false);
            HideAll();

            gameObject.SetActive(false);
        }

        private void SetDecription(string desc)
        {
            descPanel.gameObject.SetActive(true);

            description.gameObject.SetActive(true);
            description.text = desc.Localize(LocalizePartEnum.CardDescription);

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

            //left.HideAll();
            //right.HideAll();
            //delem.SetActive(false);

            descPanel.gameObject.SetActive(false);
        }

        /*
        void OnChangeDeviation(float dev)
        {
            //if (data == null || this.State != Swipe.States.DRAG || choiceble == false) return;

            if (System.Math.Abs(dev) < threshold)
            {
                if (choice == -10)
                    return;
                choice = -10;
            }
            else if (dev < -threshold)
            {
                if (choice == CardMeta.LEFT)
                    return;
                choice = CardMeta.LEFT;
            }
            else if (dev > threshold)
            {
                if (choice == CardMeta.RIGHT)
                    return;
                choice = CardMeta.RIGHT;
            }

            if (choice == -10)
            {
                // left.FadeOut();
                // right.FadeOut();
            }
            else if (choice == CardMeta.LEFT)
            {
                // left.FadeIn();
                // right.FadeOut();
            }
            else if (choice == CardMeta.RIGHT)
            {
                // left.FadeOut();
                // right.FadeIn();
            }


        } */

    }
}
