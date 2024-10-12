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
    public class TapMechanic : MonoBehaviour, IPointerUpHandler, IPointerClickHandler
    {
        public enum States
        {
            DISABLE,
            DRAG,
            IDLE
        }

        [System.Serializable] public class mEvent : UnityEvent { }

        [HideInInspector] public bool IsTaped;
        [HideInInspector] public event Action OnReadyTap;
        [HideInInspector] public event Action OnRealTaped;
        [HideInInspector] public event Action OnEndTap;

        [SerializeField] public int tapCount = 1;
        [SerializeField] public RectTransform targetRect;

        [HideInInspector] public event Action OnDoubleClickDetected;

        public States State { get; private set; }

        private RectTransform _rectTransform;
        private Sequence _shake;
        private Canvas _parent;
        private Vector2 _pivotPoint;
        private Vector2 _targetPoint;

        void Awake()
        {
            _parent = GetComponentInParent<Canvas>();
            State = States.DISABLE;
            _rectTransform = this.GetComponent<RectTransform>();
            _pivotPoint = new Vector2(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y);
            //_targetPoint = new Vector2(targetRect.position.x, targetRect.position.y);
        }

        public void Disable()
        {
            ConstructNewTap();
            State = States.DISABLE;
            this.enabled = false;

        }

        public void ConstructNewTap()
        {
            IsTaped = false;
            this.enabled = false;
            StopAllCoroutines();
            _rectTransform.localScale = new Vector3(1f, 1f, 1f);
            _rectTransform.anchoredPosition = _pivotPoint;
        }

        public void WaitTap()
        {
            State = States.IDLE;
            IsTaped = false;
            this.enabled = true;

            OnReadyTap?.Invoke();
        }

        public void Tutor()
        {
            //             Vector2 right = new Vector2(_pivotPoint.x + 140, _pivotPoint.y);
            //             Vector2 left = new Vector2(_pivotPoint.x - 140, _pivotPoint.y);
            // 
            //             _shake = DOTween.Sequence();
            //             _shake.Append(_rectTransform.DOAnchorPos(right, 0.5f, true).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
            //             _shake.Append(_rectTransform.DOAnchorPos(left, 0.5f, true).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
            //             _shake.SetLoops(-1);
        }

        public bool IsDisabled()
        {
            return State == States.DISABLE;
        }

        private void OnTapAnimation()
        {
            _rectTransform.DOScale(0.8f, 0.04f).OnComplete(() =>
            {
                IsTaped = true;
                OnEndTap?.Invoke();

                StopAllCoroutines();
                this.enabled = false;
            });

            OnRealTaped?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData) { }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnTapAnimation();
        }

    }
}