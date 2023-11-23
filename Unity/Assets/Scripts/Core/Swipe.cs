using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Core
{
    public class Swipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public enum States
        {
            DISABLE,
            DRAG,
            IDLE
        }

        public const int LEFT_CHOICE = 1;
        public const int RIGHT_CHOICE = 2;
        public const int UP_CHOICE = 3;
        public const int DOWN_CHOICE = 4;

        [System.Serializable] public class mEvent : UnityEvent { }

        [HideInInspector] public int CurrentChoise;
        [HideInInspector] public static event Action<float> OnChangeDeviation;
        [HideInInspector] public static event Action<int> OnChangeDirection;
        [HideInInspector] public static event Action OnTakeCard;
        [HideInInspector] public static event Action OnReadySwipe;
        [HideInInspector] public static event Action OnEndSwipe;
        [HideInInspector] public static event Action OnDrop;
        [HideInInspector] public static event Action OnDoubleTap;
        [HideInInspector] public static event Action OnRestoring;

        [HideInInspector] public float Deviation;
        [HideInInspector] public Vector2 Vector;

        public static States State { get; private set; }

        private RectTransform _rectTransform;
        private int _direction;
        private Card card;
        private Sequence _shake;
        private Vector2 _pivotPoint;
        private Canvas _parent;

        private float _fMovingSpeed = 50;
        private float _swipeDetectionLimit_LR = 150f;
        private float _fRotation = -0.01f;//-0.005f;
        private float _fScale = 1f;
        private bool returnCardBack = false;

        private readonly float doubleTapThreshold = 0.3f;
        private float lastTapThreshold = 0;
        private bool waitTapMode = false;

        public Vector2 PivotPoint => _pivotPoint;

        void Awake()
        {
            _parent = GetComponentInParent<Canvas>();
            State = States.DISABLE;

            Deviation = 0;
            _rectTransform = this.GetComponent<RectTransform>();
            _pivotPoint = new Vector2(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y);
            card = this.GetComponent<Card>();

            //Vector2 right = new Vector2(_pivotPoint.x + 150, _pivotPoint.y);
            //Vector2 left = new Vector2(_pivotPoint.x - 150, _pivotPoint.y);
        }

        public void ConstructNewSwipe()
        {
            Deviation = 0;
            Vector = Vector2.zero;
            CurrentChoise = -1;
            StopAllCoroutines();

            _rectTransform.anchoredPosition = _pivotPoint;
            lastTapThreshold = 0;
            _rectTransform.rotation = Quaternion.Euler(0, 0, 0);
            _rectTransform.localScale = new Vector3(1f, 1f, 1f);
            returnCardBack = false;
            waitTapMode = false;
        }

        public void WaitSwipe()
        {
            State = States.IDLE;
            CurrentChoise = -1;
            returnCardBack = false;
            lastTapThreshold = 0;
            waitTapMode = false;

            OnReadySwipe?.Invoke();
            OnChangeDeviation?.Invoke(0);
        }

        public void Tutor()
        {
            Vector2 right = new Vector2(_pivotPoint.x + 140, _pivotPoint.y);
            Vector2 left = new Vector2(_pivotPoint.x - 140, _pivotPoint.y);

            _shake = DOTween.Sequence();
            _shake.Append(_rectTransform.DOAnchorPos(right, 0.5f, true).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
            _shake.Append(_rectTransform.DOAnchorPos(left, 0.5f, true).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
            _shake.SetLoops(-1);
        }

        public bool IsDisabled()
        {
            return State == States.DISABLE;
        }

        void Update()
        {
            switch (State)
            {
                case States.IDLE:

                    if (returnCardBack)
                    {
                        _rectTransform.anchoredPosition = Vector2.MoveTowards(_rectTransform.anchoredPosition, _pivotPoint, _fMovingSpeed);
                        _rectTransform.localScale = Vector3.MoveTowards(_rectTransform.localScale, new Vector3(1, 1, 1), 0.1f);
                        _rectTransform.rotation = Quaternion.Euler(0, 0, (_rectTransform.anchoredPosition.x - _pivotPoint.x) * _fRotation);
                        MovingDispatcher();
                    }

                    break;

                    bool tapDetected = false;
#if PLATFORM_ARCH_64 || UNITY_EDITOR || PLATFORM_STANDALONE
                    tapDetected = Input.GetMouseButtonDown(0);
#else
                    if (Input.touchCount == 1)
                    {
                        Touch touch = Input.GetTouch(0);
                        tapDetected = touch.phase == TouchPhase.Began;
                    }
#endif
                    if (tapDetected)
                    {
                        if (Time.time - lastTapThreshold <= doubleTapThreshold)
                        {
                            lastTapThreshold = 0;
                            OnDoubleTapDetected();
                        }
                        else
                        {
                            lastTapThreshold = Time.time;
                        }
                    }
                    break;
                case States.DRAG:
                    //rectTransform.localScale = Vector3.MoveTowards (rectTransform.localScale, new Vector3 (fScale, fScale, 0), 0.1f);
                    break;
                default:
                    break;
            }
        }

        private void OnDoubleTapDetected()
        {
            State = States.DISABLE;
            waitTapMode = true;
            card.FadeDown().Forget();
            OnDoubleTap?.Invoke();
        }

        private void MovingDispatcher()
        {
            Vector2 distance = _rectTransform.anchoredPosition - _pivotPoint;
            float proc = distance.magnitude / _swipeDetectionLimit_LR;
            Vector = distance.normalized;

            if (_rectTransform.anchoredPosition.x > _pivotPoint.x)
            {
                Deviation = proc;
                OnChangeDeviation?.Invoke(Deviation);
            }
            else if (_rectTransform.anchoredPosition.x < _pivotPoint.x)
            {
                Deviation = -proc;
                OnChangeDeviation?.Invoke(Deviation);
            }
            else
            {
                OnChangeDeviation?.Invoke(0f);
                Deviation = 0f;
            }
        }

        public void OnPointerUp(PointerEventData eventData) { }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (State == States.DISABLE) return;

            _direction = 0;
            _shake?.Kill();

            OnTakeCard?.Invoke();
            State = States.DRAG;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (State == States.DISABLE) return;

            if (card != null)
            {
                card.OnEndDrag();
            }

            OnDrop?.Invoke();

            Vector2 distance = _rectTransform.anchoredPosition - _pivotPoint;

            /*bool choiceAvailable = true;
            switch (_direction)
            {
                case LEFT_CHOICE:
                    choiceAvailable = _card.Data.Left.Available;
                    break;
                case RIGHT_CHOICE:
                    choiceAvailable = _card.Data.Right.Available;
                    break;
            }*/

            if (distance.magnitude >= _swipeDetectionLimit_LR)// && choiceAvailable)
            {
                CurrentChoise = _direction;
                State = States.DISABLE;

                eventData.pointerDrag = null;
                returnCardBack = false;

                if (isActiveAndEnabled)
                {
                    Coroutine _coroutine = StartCoroutine(OnFideOut());
                    _coroutine = null;
                }

                OnEndSwipe?.Invoke();
            }
            else
            {
                State = States.IDLE;
                returnCardBack = true;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (State == States.DISABLE) return;

            State = States.DRAG;

            _rectTransform.anchoredPosition += new Vector2(eventData.delta.x, 0) / _parent.scaleFactor;
            _rectTransform.rotation = Quaternion.Euler(0, 0, (_rectTransform.anchoredPosition.x - _pivotPoint.x) * _fRotation);

            if (_rectTransform.anchoredPosition.x > _pivotPoint.x)
            {
                if (_direction != Swipe.RIGHT_CHOICE)
                {
                    OnChangeDirection?.Invoke(Swipe.RIGHT_CHOICE);
                }
                _direction = Swipe.RIGHT_CHOICE;
            }
            else if (_rectTransform.anchoredPosition.x < _pivotPoint.x)
            {
                if (_direction != Swipe.LEFT_CHOICE)
                {
                    OnChangeDirection?.Invoke(Swipe.LEFT_CHOICE);
                }
                _direction = Swipe.LEFT_CHOICE;
            }

            if (card != null)
            {
                card.OnDrag(_direction);

                /*if (!cardController.rightAvailable && rectTransform.anchoredPosition.x > pivotPoint.x) {
                    float scale = 0.93f;
                    rectTransform.anchoredPosition = pivotPoint;

                    rectTransform.rotation = Quaternion.Euler (0, 0, 0);
                    rectTransform.localScale = Vector3.MoveTowards (rectTransform.localScale, new Vector3 (scale, scale, scale), 0.02f);

                } else if (!cardController.leftAvailable && rectTransform.anchoredPosition.x < pivotPoint.x) {
                    float scale = 0.93f;
                    rectTransform.anchoredPosition = pivotPoint;
                    rectTransform.rotation = Quaternion.Euler (0, 0, 0);
                    rectTransform.localScale = Vector3.MoveTowards (rectTransform.localScale, new Vector3 (scale, scale, scale), 0.02f);

                } else {

                }*/
            }

            MovingDispatcher();
        }

        private bool CheckOnCamera()
        {
            Camera cam = Camera.main;

            Vector3[] objectCorners = new Vector3[4];
            _rectTransform.GetWorldCorners(objectCorners);

            bool isVisible = false;
            foreach (Vector3 _corner in objectCorners)
            {
                Vector3 corner = cam.WorldToViewportPoint(_corner);
                if ((corner.x >= 0 && corner.x <= 1 && corner.y >= 0 && corner.y <= 1))
                {
                    isVisible = true;
                    break;
                }
            }
            return isVisible;
        }

        private IEnumerator OnFideOut()
        {
            Vector2 v = (_rectTransform.anchoredPosition - _pivotPoint);
            v.Normalize();
            v *= _fMovingSpeed;
            while (CheckOnCamera())
            {
                _rectTransform.anchoredPosition += v;
                yield return null;
            }
            _rectTransform.anchoredPosition += 5 * v;

            gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // if (waitTapMode)
            // {
            //     card.FadeUp().Forget();
            //     OnRestoring?.Invoke();
            //     waitTapMode = false;
            // }
        }
    }
}