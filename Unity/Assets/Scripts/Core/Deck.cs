using System.Diagnostics.SymbolStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Cysharp.Threading.Tasks;
using Meta;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UI.ActionPanel;
using Core;
using UI.Components;
using haxe.root;

namespace Core
{
    public class Deck : ServiceBehaviour, IPage
    {
        public enum States
        {
            IDLE,
            WAITING
        }

        //public event Action OnQueueTrigger;
        //public event Action OnNewItem;

        //[HideInInspector]
        //public event Action<GameObject> OnNewCard;
        [SerializeField] private PagePanel pagePanel;
        [SerializeField] private UIActionPanel action;
        [SerializeField] private UIAcceleratePanel acceleratePanel;
        [SerializeField] private UIConditions conditionPanel;
        [SerializeField] private Text name;

        // [SerializeField]
        //public List<CardData> queue;
        public States State;
        //private Coroutine waitingTrigger;

        //public GameObject backCard;
        //public UI_AcceleratePanel acceleratePanel;

        // private CardIconQueue cardIconQueue;
        private List<GameObject> _cards;
        private GameObject currentCard;

        private List<CardMeta> Queue => Services.Player.QueueMeta;
        private PlayerService Player => Services.Player;
        private SwipeData SwipeData => Services.Player.SwipeData;

        private Swipe CurrentSwipe => currentCard.GetComponent<Swipe>() ?? throw new Exception("card doesn't exists");
        private Card Card => currentCard.GetComponent<Card>() ?? throw new Exception("card doesn't exists");

        protected override void Awake()
        {
            State = States.WAITING;
            SL.GetNewRandom(0, 0, 0);


            base.Awake();
        }

        protected override void OnServicesInited()
        {
            base.OnServicesInited();

            _cards = new List<GameObject>();
            foreach (Card c in transform.GetComponentsInChildren<Card>())
            {
                c.gameObject.SetActive(false);
                _cards.Add(c.gameObject);
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
            name.gameObject.SetActive(false);
        }

        private void OnDrop()
        {
            name.text = SwipeData.CurrentCardMeta.Name;
            name.gameObject.SetActive(true);
        }

        void OnEnable()
        {

            int time = GameTime.Current;
            /*if (queue.Count == 0 && State == States.WAITING && startTimeLeft > 0)
            {
                int timeLeft = GameTime.Left(time, startTimeLeft, waitingTimeLeft);
                if (timeLeft > 0)
                {
                    waitingTrigger = StartCoroutine(Tick());
                    waitingTrigger = null;
                    noCardTimer.text = TimeFormat.ONE_CELL_FULLNAME(timeLeft > 0 ? timeLeft : 0);
                }
                else
                {
                    startTimeLeft = 0;
                    background.SetActive(false);
                    Services.Player.Trigger(queue,
                        new TriggerVO(TriggerData.START_GAME, 0, 0, null, null, null, null), new List<RewardData>(), time);
                }
            }*/
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
        async UniTaskVoid Loop()
        {
            await UniTask.WaitUntil(() => State == States.IDLE);

            //await FadeIn();

            int time = GameTime.Current;

            Player.ApplyReroll(GameTime.Current, 0);

            OpenCard();

            while (true)
            {
                await UniTask.WaitUntil(() => State == States.IDLE);
                Swipe swipe = CurrentSwipe;

                await UniTask.WaitUntil(() => swipe.CurrentChoise != -1);

                time = GameTime.Current;
                SwipeData.Choice = swipe.CurrentChoise;

                Player.ApplySwipe(SwipeData, () => acceleratePanel.ShowMe());

                GC.Collect();
                GC.WaitForPendingFinalizers();

                //await UniTask.WaitUntil(() => queue.Count > 0);

                State = States.IDLE;

                StopAllCoroutines();

                await UniTask.WaitUntil(() => Player.GetPlayerVO.Layers.Count > 0);

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
            int time = GameTime.Current;

            Player.CreateSwipeData();

            /*string q = '(' + SwipeData.CurrentCardMeta.Id.ToString() + ')';
            foreach (CardMeta c in Queue)
            {
                q += " " + c.Id;
            }
            Debug.Log(q);
*/
            //List<CardData> _queue = new List<CardData>(queue);
            //Services.Data.TriggerNewCard (_queue, currentData.cardData, Swipe.LEFT_CHOICE, time);
            //currentData.left.nextCard = _queue.Count > 0 ? _queue[0] : null;

            //_queue = new List<CardData>(queue);
            //Services.Data.TriggerNewCard (_queue, currentData.cardData, Swipe.RIGHT_CHOICE, time);
            //currentData.right.nextCard = _queue.Count > 0 ? _queue[0] : null;
            //_queue.Clear();
            //*/

            int i = currentCard != null ? _cards.IndexOf(currentCard) + 1 : 0;
            i = i >= _cards.Count ? 0 : i;

            currentCard = _cards[i];
            currentCard.SetActive(true);
            CurrentSwipe.ConstructNewSwipe();
            Card.UpdateData(SwipeData);
            Card.FadeIn().Forget();

            OnDrop();

            action.UpdateData(SwipeData);
            //desc.gameObject.SetActive(true);

            conditionPanel.SetItem(SwipeData.Conditions);
            /*if (currentData.CardData.sound != null && currentData.CardData.sound.Length > 0)
            {
                int r = UnityEngine.Random.Range(0, currentData.CardData.sound.Length);
                //string sound = currentData.CardData.ound[r];
                Services.Assets.PlaySound(sound, audioSource).Forget();
            }*/
        }

        public void Show()
        {
            pagePanel.SetActivePageCounter(false);
            pagePanel.HideArrow();
        }

        public string GetName() => "Приключение";

        public GameObject GetGameObject() => gameObject;

        public void Hide()
        {

        }
    }
}