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

namespace Core
{
    public class CARD_Simple : MonoBehaviour, ICard
    {
        private SwipeData data = null;

        [SerializeField] private Image art;
        [SerializeField] private Image hero;
        //[SerializeField] private Text name;
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
            int ind = i;


        }

        private void ChangeArt(string image)
        {
            //if (current == image)
            return;
            //current = image;
            //art.LoadCardImage(image);
            art.DOKill();
            art.DOColor(new Color(a: 0f, r: 255, g: 255, b: 255), 0.1f).OnComplete(() =>
            {
                art.LoadCardImage(image);
                art.DOColor(new Color(a: 1f, r: 255, g: 255, b: 255), 0.1f);
            });
        }


        public void DropCard()
        {
            //current = card.Image;
            return;
            art.DOKill();
            //art.DOColor(Color.black, 0.2f).OnComplete(() =>
            //{
            art.LoadCardImage(card.Image);

            //    art.DOColor(Color.white, 0.2f);
            //});

            // current = card.Image;
            // art.LoadCardImage(card.Image);
            //ChangeArt(card.Image);
        }

        public void SetActive(bool enable)
        {
            gameObject.SetActive(enable);
        }

        public void TakeCard()
        {
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

            Services.Player.TryGetCardDescription(data.Card, out string desc);

            if (card.Hero != null && Services.Meta.Game.Heroes.TryGetValue(card.Hero, out var heroData))
            {
                hero.LoadHeroImage(heroData.Image);
                hero.gameObject.SetActive(true);
            }
            else if (desc != null && SL.HeroInWordPattern.match(desc))
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

            random.SetActive(card.Chance > 0);
            randomValue.text = $"{card.Chance}%";


        }

        public void OnDoubleClick()
        {
            //             if (!data.Card.Descs.HasTexts())
            //                 return;
            // 
            //             if (groupDescription.gameObject.activeInHierarchy)
            //                 DescriptionFadeOut();
            //             else
            //                 DescriptionFadeIn();
        }

        //         private void DescriptionFadeIn()
        //         {
        //             string desc = data.Card.Descs.GetCurrentDescription();
        //             if (desc.HasText())
        //             {
        //                 //this.transform.DORotate(new Vector3(0f, 180f, 0f), 0.7f, RotateMode.Fast);
        //                 groupDescription.gameObject.SetActive(true);
        //                 groupDescription.alpha = 0f;
        //                 description.gameObject.SetActive(true);
        //                 description.text = desc.Localize(LocalizePartEnum.CardDescription);
        //                 groupDescription.DOKill();
        //                 groupDescription.DOFade(1f, 0.15f).OnComplete(() =>
        //                 {
        //                     //description.gameObject.SetActive(true);
        //                     description.text = desc.Localize(LocalizePartEnum.CardDescription);
        //                 });
        //             }
        //         }
        // 
        //         private void DescriptionFadeOut()
        //         {
        //             //this.transform.DORotate(new Vector3(0f, 0f, 0f), 0.7f, RotateMode.Fast);
        //             groupDescription.DOKill();
        //             groupDescription.DOFade(0f, 0.15f).OnComplete(() =>
        //             {
        //                 description.gameObject.SetActive(false);
        //                 groupDescription.gameObject.SetActive(false);
        //             });
        //         }

    }

}