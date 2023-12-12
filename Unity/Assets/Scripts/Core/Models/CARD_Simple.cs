using System;
using System.Collections;
using System.Collections.Generic;

using Core;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
using UI.Components;
using Cysharp.Threading.Tasks;

namespace Core
{
    public class CARD_Simple : MonoBehaviour, ICard
    {
        private SwipeData data = null;

        [SerializeField] private Image art;
        [SerializeField] private Image hero;
        [SerializeField] private GameObject eventIcon;

        private CardMeta card => data.Card;
        private string current;
        private int ind;


        void Awake()
        {
            art.DisableSpriteOptimizations();
        }

        public void OnChangeDeviation(float vvv)
        {

            return;
            if (Math.Abs(vvv) < 0.9f)
            {
                ChangeArt(card.Image);
                return;
            }

            if (data.LastCard)
            {
                ChangeArt("endturn");
            }
            else if (data.Right == null && data.Left == null)
            {
                DropCard();
            }
            else if (ind == CardMeta.LEFT || data.Right == null)
            {
                ChangeArt(data.Left.Image);
            }
            else
            {
                ChangeArt(data.Right.Image);
            }
        }

        public void ChangeDirection(int i)
        {
            int ind = i;


        }

        private void ChangeArt(string image)
        {
            if (current == image)
                return;
            current = image;
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
            current = card.Image;
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


            if (card.Image != null)
            {
                current = card.Image;
                art.LoadCardImage(card.Image);
                art.gameObject.SetActive(true);
            }
            else
            {
                art.gameObject.SetActive(false);
            }


            if (card.Hero != null && data.Hero != null)
            {
                hero.LoadHeroImage(data.Hero.Id);
                hero.gameObject.SetActive(true);
            }
            else
            {
                hero.gameObject.SetActive(false);
            }


        }
    }

}