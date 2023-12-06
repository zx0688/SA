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

        private CARD_Simple CARD_Simple;
        //private CARD_Quest _CARD_Quest;
        //private CARD_NewLevel _CARD_NewLevel;




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

            CARD_Simple = transform.Find("CARD_Simple").GetComponent<CARD_Simple>();
            //_CARD_Quest = transform.Find("CARD_Quest").GetComponent<CARD_Quest>();
            //_CARD_NewLevel = transform.Find("CARD_NewLevel").GetComponent<CARD_NewLevel>();

            huds = new List<ICard>() { CARD_Simple };
        }
        void Start()
        {
            foreach (ICard hud in huds)
                hud.SetActive(false);

            //UI_Timer = transform.Find ("UI_Timer").gameObject;
            //lockText = UI_Timer.transform.Find("Timer").GetComponent<Text>();
            //lockSlider = UI_Timer.transform.GetComponentInChildren<Slider>();

            // Swipe.OnChangeDirection += OnChangeDirection;
            //Swipe.OnEndSwipe += OnEndSwipe;
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

            switch (10)
            {
                case 1:
                    //cardHUD = (ICard)_CARD_Quest;
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    //cardHUD = (ICardHUD)_CARD_NewLevel;
                    break;
                default:
                    hud = CARD_Simple;
                    break;
            }
            hud?.UpdateData(data);
            hud?.SetActive(true);
        }

    }
}