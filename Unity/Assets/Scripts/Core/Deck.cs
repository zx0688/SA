using System.Diagnostics.SymbolStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UI.ActionPanel;
using Core;
using UI.Components;
using haxe.root;
using Unity.Collections;
using GameServer;

namespace Core
{
    public class Deck : ServiceBehaviour, IPage
    {
        public enum States
        {
            IDLE,
            WAITING
        }

        [SerializeField] private PagePanel pages;
        [SerializeField] private UIActionPanel action;
        [SerializeField] private UIAcceleratePanel accelerate;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Text name;


        public States State;

        private List<GameObject> cards;

        private GameObject currentCardObject;

        private SwipeData swipeData;

        private Swipe currentSwipe => currentCardObject.GetComponent<Swipe>() ?? throw new Exception("card doesn't exists");
        private Card currentCard => currentCardObject.GetComponent<Card>() ?? throw new Exception("card doesn't exists");

        protected override void Awake()
        {
            State = States.WAITING;
            base.Awake();
        }

        protected override void OnServicesInited()
        {
            base.OnServicesInited();
            swipeData = new SwipeData();

            cards = new List<GameObject>();
            foreach (Card c in transform.GetComponentsInChildren<Card>())
            {
                c.gameObject.SetActive(false);
                cards.Add(c.gameObject);
            }

            foreach (GameObject c in cards)
                c.SetActive(false);

            Loop().Forget();
        }

        //GAME LOOP
        async UniTaskVoid Loop()
        {
            await UniTask.WaitUntil(() => State == States.IDLE);
            Services.Player.StartGame();
            OpenCard();

            while (true)
            {
                await UniTask.WaitUntil(() => State == States.IDLE);


                Swipe swipe = currentSwipe;
                await UniTask.WaitUntil(() => swipe.CurrentChoice != -1);
                swipeData.CurrentChoice = swipe.CurrentChoice;

                Services.Player.Swipe(swipeData);
                StopAllCoroutines();

                if (Services.Player.Profile.Deck.Count == 0)
                {
                    action.Hide();
                    accelerate.Show();
                }

                await UniTask.WaitUntil(() => Services.Player.Profile.Deck.Count > 0);

                /*if (background.activeInHierarchy)
                {
                    backgroundCG.DOKill();
                    // backgroundCG.DOFade (0f, 3f).OnComplete (() => {
                    backgroundCG.DOKill();
                    background.SetActive(false);
                    //});

                }*/
                OpenCard();
            }
        }

        private void OpenCard()
        {
            Services.Player.CreateSwipeData(swipeData);

            name.Localize(swipeData.Card.Name, LocalizePartEnum.CardName);
            int i = currentCardObject != null ? cards.IndexOf(currentCardObject) + 1 : 0;
            i = i >= cards.Count ? 0 : i;

            currentCardObject = cards[i];
            currentCardObject.SetActive(true);
            currentCardObject.GetComponent<RectTransform>().SetAsFirstSibling();
            action.GetComponent<RectTransform>().SetAsFirstSibling();



            currentSwipe.ConstructNewSwipe();
            currentCard.UpdateData(swipeData);

            action.Hide();
            action.Show(swipeData, currentSwipe);


            currentCard.FadeIn(() => GC.Collect());



            if (swipeData.Card.Sound.TryGetRandom(out string sound))
                Services.Assets.PlaySound(sound, audioSource).Forget();
        }

        public void Show()
        {
            pages.SetActivePageCounter(false);
            pages.HideArrow();

            if (Services.Player.Profile.Deck.Count == 0)
            {
                action.Hide();
                accelerate.Show();
            }
            else if (swipeData != null && currentSwipe != null)
            {
                action.Show(swipeData, currentSwipe);
            }

            State = States.IDLE;
        }

        public string GetName() => swipeData != null && swipeData.Card != null ?
            swipeData.Card.Name.Localize(LocalizePartEnum.CardName) : "Swipe.UI".Localize(LocalizePartEnum.GUI);

        public GameObject GetGameObject() => gameObject;

        public void Hide()
        {
            State = States.WAITING;

            action.Hide();
            accelerate.Hide();
        }
    }
}