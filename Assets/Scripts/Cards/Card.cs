using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Cards
{
    public class Card : MonoBehaviour
    {

        [HideInInspector] public SwipeData Data;

        private Swipe _swipe;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Text _lockText;
        private CARD_Base _hud;
        private bool _enable;

        [SerializeField] private List<CARD_Base> _huds;

        private CARD_Simple _CARD_Simple;
        //private CARD_Quest _CARD_Quest;
        //private CARD_NewLevel _CARD_NewLevel;

        public void UpdateData(SwipeData data)
        {
            this.Data = data;
            ChangeHUD(data);
        }

        public async UniTaskVoid FadeIn()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, 0.5f);

            _rectTransform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            await _rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.5f).AsyncWaitForCompletion();

            AddListener();
            GC.Collect();
            GetComponent<Swipe>().StartSwipe();

            /*if (!Services.Player.playerVO.tutorVO.swipeCard)
            {
                Services.Player.playerVO.tutorVO.swipeCard = true;
                GetComponent<Swipe>().Tutor();
            }*/
        }

        void Awake()
        {

            _swipe = GetComponent<Swipe>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();

            _CARD_Simple = transform.Find("CARD_Simple").GetComponent<CARD_Simple>();
            //_CARD_Quest = transform.Find("CARD_Quest").GetComponent<CARD_Quest>();
            //_CARD_NewLevel = transform.Find("CARD_NewLevel").GetComponent<CARD_NewLevel>();

            _huds = new List<CARD_Base>() { _CARD_Simple };
        }
        void Start()
        {
            foreach (CARD_Base hud in _huds)
                hud.gameObject.SetActive(false);

            //UI_Timer = transform.Find ("UI_Timer").gameObject;
            //lockText = UI_Timer.transform.Find("Timer").GetComponent<Text>();
            //lockSlider = UI_Timer.transform.GetComponentInChildren<Slider>();

            // Swipe.OnChangeDirection += OnChangeDirection;
            //Swipe.OnEndSwipe += OnEndSwipe;
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
            _hud.OnChangeDeviation(obj);
        }

        private void OnDrop()
        {
            _hud.OnDrop();
        }

        private void OnStartSwipe()
        {
            _hud.OnStartSwipe();
        }

        private void OnTakeCard()
        {
            _hud.OnTakeCard();
        }

        private void OnEndSwipe()
        {
            RemoveListener();
            StopAllCoroutines();
        }

        public void OnChangeDirection(int direction)
        {
            _hud.OnChangeDirection(direction);
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


        private void ChangeHUD(SwipeData data)
        {
            _hud?.SetActive(false);

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
                    _hud = (CARD_Base)_CARD_Simple;
                    break;
            }
            _hud?.UpdateData(data);
            _hud?.SetActive(true);
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
    }
}