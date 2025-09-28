using System;
using System.Collections;
using System.Collections.Generic;

using Core;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
using UI.Components;
using Cysharp.Threading.Tasks;
using haxe.root;
using Unity.Android.Gradle.Manifest;

namespace Core
{
    public class CARD_Simple : MonoBehaviour, ICard
    {
        private SwipeData data = null;

        [SerializeField] private Image art;
        [SerializeField] private Image hero;

        [SerializeField] private GameObject eventIcon;
        [SerializeField] private Text description;

        [SerializeField] private GameObject random;
        [SerializeField] private Text randomValue;


        private CardMeta card => data.Card;
        //private string current;
        private int ind;


        void Awake()
        {
            //art.DisableSpriteOptimizations();
        }

        public void OnChangeDeviation(float vvv)
        {

            return;


            // if (data.LastCard)
            // {
            //     ChangeArt("endturn");
            // }
            // else if (data.Down == null && data.Up == null)
            // {
            //     DropCard();
            // }
            // else if (ind == CardMeta.LEFT || data.Down == null)
            // {
            //     ChangeArt(data.Up.Image);
            // }
            // else
            // {
            //     ChangeArt(data.Down.Image);
            // }
        }

        public void ChangeDirection(int i)
        {
            return;
            if (data.Item.Ch.Count == 0)
                return;

            // var cardMeta = data.Choices[i];
            // var info = Services.Player.Profile.Choices[i];
            // if (info.Next == info.Id && cardMeta.TradeLimit == 0)
            //     name.Localize(cardMeta.Shure, LocalizePartEnum.CardAction);
            // else if (cardMeta.Act.HasText())
            //     name.Localize(cardMeta.Act, LocalizePartEnum.CardAction);
            // else
            //     name.Localize(cardMeta.Name, LocalizePartEnum.CardName);

            var card = data.Choices[i];
            var prev = hero.gameObject.activeInHierarchy;

            SetHero(card, card.RewardText[0]);

            if (hero.gameObject.activeInHierarchy == true && prev == false)
            {
                hero.DOKill();
                hero.color = new Color(a: 0f, r: 255, g: 255, b: 255);
                hero.DOColor(new Color(a: 1f, r: 255, g: 255, b: 255), 0.1f);
            }

            if (data.Choices[0].Image != data.Choices[1].Image)
            {
                var image = card.Image;
                art.gameObject.SetActive(true);

                art.DOKill();
                art.DOColor(new Color(a: 0f, r: 255, g: 255, b: 255), 0.2f).OnComplete(() =>
                {
                    art.LoadCardImage(image);
                    art.DOColor(new Color(a: 1f, r: 255, g: 255, b: 255), 0.2f);
                });
            }
        }


        private void SetHero(CardMeta card, string desc)
        {
            if (desc != null && SL.HeroInWordPattern.match(desc))
            {
                var heroId = SL.HeroInWordPattern.matched(1);
                if (heroId == "0")
                {
                    hero.LoadHeroImage("3");
                }
                else
                    hero.LoadHeroImage(heroId);
                hero.gameObject.SetActive(true);
            }
            else if (desc != null && desc.EndsWith("ask"))
            {
                hero.LoadHeroImage(Services.Player.Profile.Hero);
                hero.gameObject.SetActive(true);
            }
            else
            {
                hero.gameObject.SetActive(false);
            }
        }

        public void DropCard()
        {
            return;
            // if (data.Item.State != CardData.CHOICE)
            //     return;


            var prev = hero.gameObject.activeInHierarchy;
            Services.Player.TryGetCardDescription(data.Card, out string desc);

            if (card.Hero != null && Services.Meta.Game.Heroes.TryGetValue(card.Hero, out var heroData))
            {
                hero.LoadHeroImage(heroData.Image);
                hero.gameObject.SetActive(true);
            }
            else
            {
                SetHero(card, desc);
            }

            if (hero.gameObject.activeInHierarchy == false && prev == true)
            {
                hero.gameObject.SetActive(true);
                hero.DOKill();
                hero.DOColor(new Color(a: 0f, r: 255, g: 255, b: 255), 0.1f).OnComplete(() =>
                {
                    hero.gameObject.SetActive(false);
                    hero.color = new Color(a: 1f, r: 255, g: 255, b: 255);
                });
            }

            if (data.Choices[0].Image != data.Choices[1].Image)
            {
                art.DOKill();
                art.DOColor(new Color(a: 0f, r: 255, g: 255, b: 255), 0.2f).OnComplete(() =>
                {
                    art.LoadCardImage(card.Image);
                    art.DOColor(new Color(a: 1f, r: 255, g: 255, b: 255), 0.2f);
                });
            }

        }

        public void SetActive(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public void TakeCard()
        {
            return;


            ChangeDirection(CardMeta.LEFT);
            //art.DOKill();
            //art.DOColor(Color.black, 0.2f).OnComplete(() =>
            // {
            //ChangeDirection(CardMeta.LEFT);
            //    art.DOColor(Color.white, 0.2f);
            //});

            //ChangeDirection(CardMeta.LEFT);
        }

        public void UpdateData(SwipeData data)
        {
            this.data = data;
            art.DOKill();

            /*if (data.Card.Hero == null)
            {
                name.Localize(data.Card.Name, LocalizePartEnum.CardName);
            }
            else
            {
                name.Localize(data.Hero.Name, LocalizePartEnum.CardName);
            }*/

            if (card.Image != null)
            {
                art.LoadCardImage(card.Image);
                art.gameObject.SetActive(true);
            }
            else
            {
                art.gameObject.SetActive(false);
            }



            hero.DOKill();
            hero.color = new Color(a: 1f, r: 255, g: 255, b: 255);
            if (card.Hero != null && Services.Meta.Game.Heroes.TryGetValue(card.Hero, out var heroData))
            {
                hero.LoadHeroImage(heroData.Image);
                hero.gameObject.SetActive(true);
            }
            else if (Services.Player.TryGetCardDescription(data.Card, out string desc))
            {
                SetHero(card, desc);
            }

            random.SetActive(card.Chance > 0);
            randomValue.text = $"{card.Chance}%";


        }


    }

}