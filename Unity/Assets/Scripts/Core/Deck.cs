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
        [SerializeField] private Text name;

        // [SerializeField]
        //public List<CardData> queue;
        public States State;
        //private Coroutine waitingTrigger;

        //public GameObject backCard;

        // private CardIconQueue cardIconQueue;
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

            State = States.IDLE;

            Loop().Forget();


            //StartCoroutine (TriggerTimer ());
        }

        /*private void OnAccelerated()
        {

            StopCoroutine(Tick());
            waitingTrigger = null;
            int timestamp = GameTime.Current;
            int timeLeft = GameTime.Left(timestamp, startTimeLeft, waitingTimeLeft);
            noCardTimer.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft > 0 ? timeLeft : 0);

            if (State == States.WAITING)
            {
                startTimeLeft = 0;
                background.SetActive(false);
                Services.Player.Trigger(queue,
                    new TriggerVO(TriggerData.START_GAME, 0, 0, null, null, null, null), new List<RewardData>(), timestamp);
            }
        }*/

        void Start()
        {
            //_rerollBtn.OnClick += OnReroll;
            //_rerollBtn.OnAnimationCallback += () =>
            //{
            //    _rerollBtn.gameObject.SetActive(false);
            //};

            //_rerollBtn.gameObject.SetActive(false);
            Swipe.OnDrop += OnDrop;
            Swipe.OnTakeCard += OnTake;
        }

        private void OnTake()
        {
        }

        private void OnDrop()
        {
        }

        /*async UniTask FadeIn()
        {

            Vector2 pivotPoint = new Vector2(0, 0);
            CanvasGroup cg = backCard.GetComponent<CanvasGroup>();
            RectTransform rect = backCard.GetComponent<RectTransform>();
            cg.alpha = 1f;
            for (int i = 0; i < 5; i++)
            {

                GameObject clone = Instantiate(backCard, backCard.transform.position, backCard.transform.rotation, transform) as GameObject;
                RectTransform r = clone.GetComponent<RectTransform>();
                r.DOAnchorPos(pivotPoint, 0.4f, true).SetEase(Ease.OutCirc).SetAutoKill(true).SetDelay(0.1f * i).OnComplete(() =>
                {
                    r.DOKill();
                    Destroy(clone);
                });
            }

            await rect.DOAnchorPos(pivotPoint, 0.4f, true).SetEase(Ease.OutCirc).SetAutoKill(true).AsyncWaitForCompletion();
            await cg.DOFade(0f, 0.3f).SetDelay(1f).SetAutoKill(true).AsyncWaitForCompletion();
            backCard.SetActive(false);
            GC.Collect();

            return;
        }*/

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
                await UniTask.WaitUntil(() => swipe.CurrentChoise != -1);
                swipeData.Choice = swipe.CurrentChoise;
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

                //noCardTimer.gameObject.SetActive (false);
                OpenCard();
            }
        }

        /*  private int GetEarlyDuration(int timestamp)
          {
              int earlyierTimeLeft = int.MaxValue;
              int earlyierDuration = int.MaxValue;
              foreach (CardVO cardVO in Services.Player.playerVO.cards)
              {
                  CardData cardData = Services.Data.CardInfo(cardVO.id);
                  if (cardData == null || cardData.Act.Time == 0 || cardVO.executed == 0)
                      continue;
                  int timeLeft = GameTime.Left(timestamp, cardVO.executed, cardData.Act.Time);
                  if (timeLeft > 0 && timeLeft < earlyierTimeLeft && Services.Data.CheckConditions(cardData.Act.Con, int.MaxValue))
                  {
                      earlyierTimeLeft = timeLeft;
                      earlyierDuration = timeLeft; //cardData.act.time - timeLeft;
                  }
              }

              return earlyierDuration;
          }*/

        /*IEnumerator Tick()
        {
            //Swipe swipe = currentCard.GetComponent<Swipe> ();
            startTimeLeft = GameTime.Current;
            waitingTimeLeft = GetEarlyDuration(startTimeLeft);
            noCardTimer.text = TimeFormat.ONE_CELL_FULLNAME(GameTime.Left(startTimeLeft, startTimeLeft, waitingTimeLeft));
            acceleratePanel.SetTimer(startTimeLeft, waitingTimeLeft);
            while (queue.Count == 0 && State == States.WAITING)
            {

                int time = GameTime.Current;
                int timeLeft = GameTime.Left(time, startTimeLeft, waitingTimeLeft);
                noCardTimer.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft > 0 ? timeLeft : 0);

                if (timeLeft <= 0 && State == States.WAITING)
                {
                    startTimeLeft = 0;
                    background.SetActive(false);
                    Services.Player.Trigger(queue,
                        new TriggerVO(TriggerData.START_GAME, 0, 0, null, null, null, null), new List<RewardData>(), time);
                }
                yield return new WaitForSeconds(1f);
            }
        }*/

        private void OpenCard()
        {
            Services.Player.CreateSwipeData(swipeData);

            int i = currentCardObject != null ? cards.IndexOf(currentCardObject) + 1 : 0;
            i = i >= cards.Count ? 0 : i;

            currentCardObject = cards[i];
            currentCardObject.SetActive(true);
            currentCardObject.GetComponent<RectTransform>().SetAsFirstSibling();

            currentSwipe.ConstructNewSwipe();
            currentCard.UpdateData(swipeData);

            if (swipeData.Card.Hero == null)
            {
                name.text = swipeData.Card.Name.Localize(LocalizePartEnum.CardName);
            }
            else
            {
                name.text = swipeData.Hero.Name.Localize(LocalizePartEnum.CardName);
            }


            action.Show(swipeData);
            currentCard.FadeIn(() => GC.Collect());
            OnDrop();

            //desc.gameObject.SetActive(true);
            /*if (currentData.CardData.sound != null && currentData.CardData.sound.Length > 0)
            {
                int r = UnityEngine.Random.Range(0, currentData.CardData.sound.Length);
                //string sound = currentData.CardData.ound[r];
                Services.Assets.PlaySound(sound, audioSource).Forget();
            }*/
        }

        public void Show()
        {
            pages.SetActivePageCounter(false);
            pages.HideArrow();
        }

        public string GetName() => "Swipe Adv".Localize(LocalizePartEnum.GUI);

        public GameObject GetGameObject() => gameObject;

        public void Hide()
        {

        }
    }
}