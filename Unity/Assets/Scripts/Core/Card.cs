using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class Card : MonoBehaviour
    {
        [HideInInspector] public SwipeData Data;

        private Swipe swipe;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Text lockText;
        private ICard hud;
        private bool enable;

        [SerializeField] private List<ICard> huds;

        [SerializeField] private CARD_Simple CARD_Simple;
        [SerializeField] private CARD_Quest CARD_Quest;

        public void UpdateData(SwipeData data)
        {
            this.Data = data;
            ChangeHUD(data);
            Input.simulateMouseWithTouches = true;
        }

        public void FadeIn(Action callback)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.DOFade(1f, 0.1f);

            rectTransform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            rectTransform.DOKill();
            rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.15f).OnComplete(() =>
            {
                AddListeners();
                //   GC.Collect();
                swipe.WaitSwipe();
                callback?.Invoke();
            });
        }

        // public void FadeDrop()
        // {
        //     DOTween.Kill(rectTransform);
        //     //rectTransform.DOAnchorPos(swipe.PivotPoint, 0.1f, false).SetEase(Ease.OutCirc);
        //     rectTransform.DOScale(0.9f, 0.1f);
        // }

        public void FadeTake()
        {
            DOTween.Kill(rectTransform);
            //rectTransform.DOAnchorPos(swipe.PivotPoint, 0.1f, false).SetEase(Ease.OutCirc);
            //rectTransform.DOScale(0.8f, 0.12f);
        }

        void Awake()
        {
            swipe = GetComponent<Swipe>();
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            huds = new List<ICard>() { CARD_Simple, CARD_Quest };
        }
        void Start()
        {
            foreach (ICard hud in huds)
                hud.SetActive(false);
        }

        private void AddListeners()
        {
            Swipe.OnReadySwipe += OnStartSwipe;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnDrop += OnDrop;
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnChangeDirection += OnChangeDirection;
        }

        private void RemoveListeners()
        {
            Swipe.OnReadySwipe -= OnStartSwipe;
            Swipe.OnTakeCard -= OnTakeCard;
            Swipe.OnDrop -= OnDrop;
            Swipe.OnChangeDeviation -= OnChangeDeviation;
            Swipe.OnChangeDirection -= OnChangeDirection;
        }

        private void OnChangeDeviation(float obj)
        {
            hud.OnChangeDeviation(obj);
        }

        private void OnDrop()
        {
            hud.DropCard();
            //FadeDrop();
        }

        private void OnStartSwipe()
        {
            //_hud.OnStartSwipe();
        }

        private void OnTakeCard()
        {
            hud.TakeCard();
            FadeTake();
        }

        private void OnEndSwipe()
        {
            RemoveListeners();
            StopAllCoroutines();
        }

        public void OnChangeDirection(int direction)
        {
            hud.ChangeDirection(direction);
        }

        public void OnEndDrag()
        {

        }
        public void OnDrag(int direction)
        {

        }

        void OnDisable()
        {
            RemoveListeners();
        }


        private void ChangeHUD(SwipeData data)
        {
            hud?.SetActive(false);

            if (data.Card.Type == CardMeta.TYPE_QUEST)
                hud = CARD_Quest;
            else
                hud = CARD_Simple;
            hud?.UpdateData(data);
            hud?.SetActive(true);
        }

    }
}