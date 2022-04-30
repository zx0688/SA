using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Components;
using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public enum SwipeState
{
    DISABLE,
    RETURN,
    DRAG,
    IDLE
}

public class Swipe : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public const int LEFT_CHOICE = 1;
    public const int RIGHT_CHOICE = 2;
    public const int UP_CHOICE = 3;
    public const int DOWN_CHOICE = 4;

    [System.Serializable] public class mEvent : UnityEvent { }

    private Canvas _parent;
    private float fMovingSpeed = 32;
    private float swipeDetectionLimit_LR = 316f;
    private float fRotation = -0.005f;
    private float fScale = 1f;
    private Sequence Shake;

    private Vector2 pivotPoint;

    [HideInInspector]
    public int currentChoise;

    [HideInInspector]
    public static event Action<float> OnChangeDeviation;
    [HideInInspector]
    public static event Action<int> OnChangeDirection;
    [HideInInspector]
    public static event Action OnTakeCard;
    [HideInInspector]
    public static event Action OnStartSwipe;
    [HideInInspector]
    public static event Action OnEndSwipe;
    [HideInInspector]
    public static event Action OnDrop;

    [HideInInspector]
    public float deviation;
    private int direction;
    [HideInInspector]
    public Vector2 vector;

    private Card cardController;

    public static SwipeState state { get; private set; }

    private RectTransform rectTransform;

    void Awake()
    {
        _parent = GetComponentInParent<Canvas>();

        // _parent = GameObject.FindGameObjectsWithTag("CanvasName");
        state = SwipeState.DISABLE;
        deviation = 0;
        rectTransform = this.GetComponent<RectTransform>();
        pivotPoint = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y);
        cardController = this.GetComponent<Card>();

        Vector2 right = new Vector2(pivotPoint.x + 150, pivotPoint.y);
        Vector2 left = new Vector2(pivotPoint.x - 150, pivotPoint.y);
    }

    public void ConstructNewSwipe()
    {
        deviation = 0;
        vector = Vector2.zero;
        currentChoise = -1;
        StopAllCoroutines();
        rectTransform.anchoredPosition = pivotPoint;
        rectTransform.rotation = Quaternion.Euler(0, 0, 0);
        rectTransform.localScale = new Vector3(1f, 1f, 1f);

    }
    public void StartSwipe()
    {
        state = SwipeState.IDLE;
        currentChoise = -1;
        OnStartSwipe?.Invoke();
        OnChangeDeviation?.Invoke(0);
    }

    public void Tutor()
    {
        Vector2 right = new Vector2(pivotPoint.x + 140, pivotPoint.y);
        Vector2 left = new Vector2(pivotPoint.x - 140, pivotPoint.y);

        Shake = DOTween.Sequence();
        Shake.Append(rectTransform.DOAnchorPos(right, 0.5f, true).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
        Shake.Append(rectTransform.DOAnchorPos(left, 0.5f, true).SetEase(Ease.OutExpo).SetLoops(2, LoopType.Yoyo));
        Shake.SetLoops(-1);
    }

    public bool isDisabled()
    {
        return state == SwipeState.DISABLE;
    }

    void Start()
    {

    }

    void Update()
    {

        switch (state)
        {
            case SwipeState.IDLE:

                break;
            case SwipeState.RETURN:

                rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, pivotPoint, fMovingSpeed);
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, new Vector3(1, 1, 1), 0.1f);

                rectTransform.rotation = Quaternion.Euler(0, 0, (rectTransform.anchoredPosition.x - pivotPoint.x) * fRotation);
                MovingDispatcher();

                break;
            case SwipeState.DRAG:
                //rectTransform.localScale = Vector3.MoveTowards (rectTransform.localScale, new Vector3 (fScale, fScale, 0), 0.1f);
                break;

            default:
                break;
        }

    }

    private void MovingDispatcher()
    {
        Vector2 distance = rectTransform.anchoredPosition - pivotPoint;
        float proc = distance.magnitude / swipeDetectionLimit_LR;
        vector = distance.normalized;

        if (rectTransform.anchoredPosition.x > pivotPoint.x)
        {

            deviation = proc;
            OnChangeDeviation?.Invoke(deviation);

        }
        else if (rectTransform.anchoredPosition.x < pivotPoint.x)
        {

            deviation = -proc;
            OnChangeDeviation?.Invoke(deviation);

        }
        else
        {

            OnChangeDeviation?.Invoke(0f);
            deviation = 0f;

        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (state == SwipeState.DISABLE)
        {
            return;
        }

        direction = 0;
        Shake?.Kill();
        OnTakeCard?.Invoke();
        state = SwipeState.DRAG;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        if (state == SwipeState.DISABLE)
        {
            return;
        }

        if (cardController != null)
        {
            cardController.OnEndDrag();
        }

        OnDrop?.Invoke();

        Vector2 distance = rectTransform.anchoredPosition - pivotPoint;

        bool choiceAvailable = true;
        switch (direction)
        {
            case LEFT_CHOICE:
                choiceAvailable = cardController.data.Left.Available;
                break;
            case RIGHT_CHOICE:
                choiceAvailable = cardController.data.Right.Available;
                break;
        }

        if (distance.magnitude >= swipeDetectionLimit_LR && choiceAvailable)
        {

            currentChoise = direction;
            state = SwipeState.DISABLE;

            eventData.pointerDrag = null;
            OnEndSwipe?.Invoke();

            if (isActiveAndEnabled)
            {
                Coroutine _coroutine = StartCoroutine(OnFideOut());
                _coroutine = null;
            }

        }
        else
        {
            state = SwipeState.RETURN;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (state == SwipeState.DISABLE)
        {
            return;
        }

        state = SwipeState.DRAG;

        rectTransform.anchoredPosition += new Vector2(eventData.delta.x, 0) / _parent.scaleFactor;

        rectTransform.rotation = Quaternion.Euler(0, 0, (rectTransform.anchoredPosition.x - pivotPoint.x) * fRotation);

        if (rectTransform.anchoredPosition.x > pivotPoint.x)
        {
            if (direction != Swipe.RIGHT_CHOICE)
            {
                OnChangeDirection?.Invoke(Swipe.RIGHT_CHOICE);
            }
            direction = Swipe.RIGHT_CHOICE;
        }
        else if (rectTransform.anchoredPosition.x < pivotPoint.x)
        {
            if (direction != Swipe.LEFT_CHOICE)
            {
                OnChangeDirection?.Invoke(Swipe.LEFT_CHOICE);
            }
            direction = Swipe.LEFT_CHOICE;
        }

        if (cardController != null)
        {

            cardController.OnDrag(direction);

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
        rectTransform.GetWorldCorners(objectCorners);

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

        Vector2 v = (rectTransform.anchoredPosition - pivotPoint);
        v.Normalize();
        v *= fMovingSpeed;

        while (CheckOnCamera())
        {

            rectTransform.anchoredPosition += v;
            yield return null;
        }
        rectTransform.anchoredPosition += 5 * v;

        gameObject.SetActive(false);

        // GetComponent<Animator> ().SetTrigger ("fadeout");
    }

}