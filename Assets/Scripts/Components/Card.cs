using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace Components
{
    public class Card : MonoBehaviour
    {

        [HideInInspector]
        public SwipeData data;
        private Swipe swipe;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Text lockText;
        private Animator animator;
        private ICardHUD cardHUD;
        private bool enable;
        private List<CARD_Base> huds;

        private CARD_Simple _CARD_Simple;

        private CARD_Quest _CARD_Quest;

        private CARD_NewLevel _CARD_NewLevel;

        public void UpdateData(SwipeData data)
        {
            this.data = data;

            if (cardHUD != null)
            {
                cardHUD.SetActive(false);
            }

            /*switch (data.CardData.T)
            {
                case 1:
                    cardHUD = (ICardHUD)_CARD_Quest;
                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:
                    cardHUD = (ICardHUD)_CARD_NewLevel;
                    break;
                default:
                    cardHUD = (ICardHUD)_CARD_Simple;
                    break;

            }*/

            cardHUD.UpdateData(data);
            cardHUD.SetActive(true);
        }

        public async UniTaskVoid FadeIn()
        {

            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.5f);

            rectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            await rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).AsyncWaitForCompletion();

            AddListener();
            GC.Collect();
            GetComponent<Swipe>().StartSwipe();

            if (!Services.Player.playerVO.tutorVO.swipeCard)
            {
                Services.Player.playerVO.tutorVO.swipeCard = true;
                GetComponent<Swipe>().Tutor();
            }
        }

        IEnumerator SecondTimer()
        {
            while (true)
            {
                int time = GameTime.Current;
                // lockText.text = TimeFormat.DD_HH_MM (GameTime.TimeLeft (time, data.cardVO.activated, data.cardData.locked));
                yield return new WaitForSeconds(1f);
            }
        }

        void Awake()
        {

            swipe = GetComponent<Swipe>();
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            _CARD_Simple = transform.Find("CARD_Simple").GetComponent<CARD_Simple>();
            _CARD_Quest = transform.Find("CARD_Quest").GetComponent<CARD_Quest>();
            _CARD_NewLevel = transform.Find("CARD_NewLevel").GetComponent<CARD_NewLevel>();

            huds = new List<CARD_Base>() { _CARD_Simple, _CARD_Quest, _CARD_NewLevel };
        }
        void Start()
        {

            foreach (CARD_Base hud in huds)
                hud.gameObject.SetActive(false);
            //UI_Timer = transform.Find ("UI_Timer").gameObject;
            //lockText = UI_Timer.transform.Find("Timer").GetComponent<Text>();
            //lockSlider = UI_Timer.transform.GetComponentInChildren<Slider>();

            // Swipe.OnChangeDirection += OnChangeDirection;
            Swipe.OnEndSwipe += OnEndSwipe;

        }

        private void AddListener()
        {
            Swipe.OnStartSwipe += OnStartSwipe;
            Swipe.OnTakeCard += OnTakeCard;
            Swipe.OnDrop += OnDrop;
            Swipe.OnChangeDeviation += OnChangeDeviation;
            Swipe.OnChangeDirection += OnChangeDirection;
        }

        private void RemoveListener()
        {
            Swipe.OnStartSwipe -= OnStartSwipe;
            Swipe.OnTakeCard -= OnTakeCard;
            Swipe.OnDrop -= OnDrop;
            Swipe.OnChangeDeviation -= OnChangeDeviation;
            Swipe.OnChangeDirection -= OnChangeDirection;
        }

        private void OnChangeDeviation(float obj)
        {
            cardHUD.OnChangeDeviation(obj);
        }

        private void OnDrop()
        {
            cardHUD.OnDrop();
        }

        private void OnStartSwipe()
        {
            cardHUD.OnStartSwipe();
        }

        private void OnTakeCard()
        {
            cardHUD.OnTakeCard();
        }

        private void OnEndSwipe()
        {
            RemoveListener();
            StopAllCoroutines();
        }

        public void OnChangeDirection(int direction)
        {
            cardHUD.OnChangeDirection(direction);
        }

        public void OnEndDrag()
        {

        }
        public void OnDrag(int direction)
        {

        }

        void OnDisable()
        {
            RemoveListener();
        }

    }
}