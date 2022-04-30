using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Components;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class Deck : ServiceBehaviour
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


    // [SerializeField]
    //public List<CardData> queue;
    public States State;
    //private Coroutine waitingTrigger;

    //public GameObject backCard;
    //public UI_AcceleratePanel acceleratePanel;


    //public CardParams currentData;
    // private CardIconQueue cardIconQueue;
    private List<GameObject> _cards;
    private GameObject _currentCard;
    private SwipeData _swipeData;

    protected override void Awake()
    {
        State = States.WAITING;
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

        //Services.Player.Trigger(queue,
        //    new TriggerVO(TriggerData.START_GAME, 0, 0, null, null, null, null), new List<RewardData>(), time);

        //queue.Sort((c1, c2) => c1.Pri.CompareTo(c2.Pri));
        //queue.Reverse();

        //currentData.CardData = queue[0];
        //queue.RemoveAt(0);
        OpenCard(time);

        while (true)
        {

            await UniTask.WaitUntil(() => State == States.IDLE);

            Swipe swipe = _currentCard.GetComponent<Swipe>();
            await UniTask.WaitUntil(() => swipe.currentChoise != -1);

            time = GameTime.Current;

            //Services.Player.Trigger(queue,
            //    new TriggerVO(TriggerData.CARD, currentData.CardData.Id, swipe.currentChoise, currentData, null, null, null), new List<RewardData>(), time);

            /*if (queue.Count == 0)
            {
                State = States.WAITING;

                //backgroundCG.alpha = 0;
                //backgroundCG.DOFade (1f, 3f).OnComplete (() => { });
                waitingTrigger = StartCoroutine(Tick());
                waitingTrigger = null;
                background.SetActive(true);
            }*/

            GC.Collect();
            GC.WaitForPendingFinalizers();

            /*await UniTask.WaitUntil(() => queue.Count > 0);

            State = States.IDLE;

            StopAllCoroutines();

            if (background.activeInHierarchy)
            {
                backgroundCG.DOKill();
                // backgroundCG.DOFade (0f, 3f).OnComplete (() => {
                backgroundCG.DOKill();
                background.SetActive(false);
                //});

            }

            queue.Sort((c1, c2) => c1.Pri.CompareTo(c2.Pri));
            queue.Reverse();

            currentData.CardData = queue[0];
            queue.RemoveAt(0);

            //noCardTimer.gameObject.SetActive (false);
*/
            OpenCard(time);
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

    private void OpenCard(int time)
    {

        /*string q = '(' + currentData.CardData.Id.ToString() + ')';
        foreach (CardData c in queue)
        {
            q += " " + c.Id;
        }
        Debug.Log(q);

        List<CardData> _queue = new List<CardData>(queue);
        //Services.Data.TriggerNewCard (_queue, currentData.cardData, Swipe.LEFT_CHOICE, time);
        currentData.left.nextCard = _queue.Count > 0 ? _queue[0] : null;

        _queue = new List<CardData>(queue);
        //Services.Data.TriggerNewCard (_queue, currentData.cardData, Swipe.RIGHT_CHOICE, time);
        currentData.right.nextCard = _queue.Count > 0 ? _queue[0] : null;
        _queue.Clear();

        int i = _currentCard != null ? _cards.IndexOf(_currentCard) + 1 : 0;
        i = i >= _cards.Count ? 0 : i;

        _currentCard = _cards[i];
        currentData.left.action = currentData.right.action = null;

        currentData.cardVO = Services.Player.OpenCard(currentData, time);

        currentData.left.available = true; //= currentData.left == null ? true : Services.Data.CheckConditions (currentData.left.condi, time);
        currentData.right.available = true; // = currentData.right == null ? true : Services.Data.CheckConditions (currentData.right.condi, time);
        currentData.left.chance = currentData.right.chance = 0;

        /*Services.Player.skillHandler.ApplyCardSkill(
            currentData,
            new TriggerVO(TriggerData.CARD, currentData.cardData.id, 0, currentData, null, null, null),
            time);
*/
        //if (currentData.leftAvailable == false && currentData.rightAvailable == false)
        //   throw new Exception ("card " + currentData.cardData.id + " has left and right unavailable");
        _currentCard.SetActive(true);
        _currentCard.GetComponent<Swipe>().ConstructNewSwipe();
        //_currentCard.GetComponent<Card>().UpdateData(currentData);
        _currentCard.GetComponent<Card>().FadeIn().Forget();

        /*if (currentData.CardData.sound != null && currentData.CardData.sound.Length > 0)
        {
            int r = UnityEngine.Random.Range(0, currentData.CardData.sound.Length);
            //string sound = currentData.CardData.ound[r];
            Services.Assets.PlaySound(sound, audioSource).Forget();
        }*/

        //OnNewCard?.Invoke(_currentCard);
    }
}