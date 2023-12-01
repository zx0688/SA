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

        // public async UniTaskVoid FadeDown(Action callback)
        // {

        //     RectTransform rectTransform = GetComponent<RectTransform>();
        //     DOTween.Kill(rectTransform);
        //     Vector2 down = new Vector2(swipe.PivotPoint.x, swipe.PivotPoint.y - 150);

        //     await UniTask.DelayFrame(10);
        //     rectTransform.DOAnchorPos(down, 0.2f, false).SetEase(Ease.OutCirc);
        //     rectTransform.DOScale(0.97f, 0.2f).SetEase(Ease.OutCirc).OnComplete(() => callback());
        //     RemoveListeners();
        // }

        // public async UniTaskVoid FadeUp()
        // {
        //     RectTransform rectTransform = GetComponent<RectTransform>();
        //     DOTween.Kill(rectTransform);
        //     rectTransform.DOAnchorPos(swipe.PivotPoint, 0.1f, false).SetEase(Ease.OutCirc);
        //     await rectTransform.DOScale(1f, 0.1f).SetEase(Ease.OutCirc).AsyncWaitForCompletion();
        //     AddListeners();
        //     GC.Collect();
        //     swipe.ConstructNewSwipe();
        //     swipe.WaitSwipe();
        // }


        public void UpdateData(SwipeData data)
        {
            this.Data = data;
            ChangeHUD(data);
            Input.simulateMouseWithTouches = true;
        }

        public async UniTaskVoid FadeIn(Action callback)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.2f);

            rectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            await rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).AsyncWaitForCompletion();

            AddListeners();
            GC.Collect();
            swipe.WaitSwipe();

            callback?.Invoke();

            /*if (!Services.Player.playerVO.tutorVO.swipeCard)
            {
                Services.Player.playerVO.tutorVO.swipeCard = true;
                GetComponent<Swipe>().Tutor();
            }*/
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
            //_hud.OnChangeDeviation(obj);
        }

        private void OnDrop()
        {
            //_hud.OnDrop();
        }

        private void OnStartSwipe()
        {
            //_hud.OnStartSwipe();
        }

        private void OnTakeCard()
        {
            //_hud.OnTakeCard();
        }

        private void OnEndSwipe()
        {
            RemoveListeners();
            StopAllCoroutines();
        }

        public void OnChangeDirection(int direction)
        {
            //_hud.OnChangeDirection(direction);
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