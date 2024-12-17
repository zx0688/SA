using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public Action pageChanged;
    public float percentThreshold = 0.2f;
    public float Easing = 0.5f;
    public float Offset = 0f;
    public bool isDraggingWhenOut = true;
    public bool showPageButtons = true;

    private int _totalPages = 5;
    private int _currentPage = 0;
    private int _prevPage = 0;
    private float _width;

    private Dictionary<GameObject, ISetData<string>[]> _pages;
    private List<string> _items;
    [SerializeField] private List<GameObject> _panels;
    private int _countItemsPerPage;
    private Vector3 _panelLocation;
    private Vector3 _pivotStartPoint;
    private float _pivotY;
    private float _pivotZ;

    [SerializeField] private PagePanel _pagePanel;

    [SerializeField] private Canvas _parent;

    [SerializeField] private TutorialSwipeFinger tutorialSwipeFinger;
    [SerializeField] private string tutorialKey;

    void Awake()
    {
        _pages = new Dictionary<GameObject, ISetData<string>[]>();
        _pages.Add(_panels[0], _panels[0].GetComponentsInChildren<ISetData<string>>());
        _pages.Add(_panels[1], _panels[1].GetComponentsInChildren<ISetData<string>>());
        _pages.Add(_panels[2], _panels[2].GetComponentsInChildren<ISetData<string>>());

        _countItemsPerPage = _pages[_panels[0]].Length;
    }

    private void RebuildPositions()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        _width = transform.localToWorldMatrix[0] * rectTransform.rect.width + 40;

        RectTransform _rect;

        _rect = _panels[0].GetComponent<RectTransform>();
        _rect.anchoredPosition = new Vector2(0, 0);

        _rect = _panels[1].GetComponent<RectTransform>();

        float width = Screen.width / (2 * _parent.scaleFactor) + _rect.GetWidth() / 2 + Offset;

        _rect.anchoredPosition = new Vector2(width, 0);

        _width = _panels[1].transform.position.x;
        _pivotY = _panels[1].transform.position.y;
        _pivotZ = _panels[1].transform.position.z;

        _rect = _panels[2].GetComponent<RectTransform>();
        _rect.anchoredPosition = new Vector2(width * 2, 0);
    }

    void OnEnable()
    {
        if (!showPageButtons)
            return;
        _pagePanel.ScrollPageLeft += OnScrollPageLeft;
        _pagePanel.ScrollPageRight += OnScrollPageRight;

        _pagePanel.SetActivePageCounter(true);
    }

    void OnDisable()
    {
        if (!showPageButtons)
            return;

        _pagePanel.ScrollPageLeft -= OnScrollPageLeft;
        _pagePanel.ScrollPageRight -= OnScrollPageRight;

    }

    private void OnScrollPageRight()
    {
        SetPage(_totalPages - 1);
    }

    private void OnScrollPageLeft()
    {
        SetPage(0);
    }

    void Start()
    {
        RebuildPositions();
        _panelLocation = transform.position;
        _pivotStartPoint = transform.position;

        SetPage(0);
    }

    public void OnDrag(PointerEventData data)
    {
        if (_totalPages == 1) return;

        float difference = data.pressPosition.x - data.position.x;

        if (isDraggingWhenOut == false)
        {
            if (_currentPage == 0 && -difference > 40)
            {
                //
            }
            else if (_currentPage == _totalPages - 1 && difference > 40)
            {
                //
            }
            else
            {
                transform.position = _panelLocation - new Vector3(difference, 0, 0);
            }
        }
        else
        {
            transform.position = _panelLocation - new Vector3(difference, 0, 0);
        }
    }

    private void UpdatePositions()
    {
        //Vector3 diff = _panelLocation - transform.position;
        //int page = Mathf.FloorToInt(diff.x / _width);
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (_totalPages == 1) return;

        float percentage = (data.pressPosition.x - data.position.x) / _width;

        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = _panelLocation;
            if (percentage > 0 && _currentPage < _totalPages - 1)
            {
                _currentPage++;
                newLocation += new Vector3(-_width, 0, 0);

                if (_currentPage > 1 && _currentPage < _totalPages - 1)
                {
                    _panels = _panels.OrderBy(p => p.transform.position.x).ToList();
                    GameObject first = _panels.FirstOrDefault();
                    GameObject last = _panels.LastOrDefault();
                    first.transform.position = new Vector3(last.transform.position.x + _width, _pivotY, _pivotZ);
                    ClearPage(first, _currentPage + 1);

                }

                UpdateData(_items);
                pageChanged?.Invoke();
            }
            else if (percentage < 0 && _currentPage > 0)
            {
                _currentPage--;
                newLocation += new Vector3(_width, 0, 0);

                if (_currentPage < _totalPages - 2 && _currentPage > 0)
                {
                    _panels = _panels.OrderBy(p => p.transform.position.x).ToList();
                    GameObject first = _panels.FirstOrDefault();
                    GameObject last = _panels.LastOrDefault();
                    last.transform.position = new Vector3(first.transform.position.x - _width, _pivotY, _pivotZ);
                    ClearPage(last, _currentPage - 1);

                }

                UpdateData(_items);
                pageChanged?.Invoke();
            }

            if (showPageButtons)
                _pagePanel.SetTextCounter(_currentPage, _totalPages);

            StartCoroutine(SmoothMove(transform.position, newLocation, Easing));
            _panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, _panelLocation, Easing));
        }
    }

    public void Hide()
    {
        foreach (GameObject page in _pages.Keys)
        {
            ISetData<string>[] _items = _pages[page];
            foreach (ISetData<string> i in _items)
            {
                i.Hide();
            }
        }

        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void UpdateData(List<string> items)
    {
        _items = items;

        _totalPages = items.Count == 0 ? 1 : Mathf.CeilToInt((float)items.Count / (float)_countItemsPerPage);

        _panels = _panels.OrderBy(p => p.transform.position.x).ToList();

        if (_currentPage > _totalPages - 1)
            _currentPage = _totalPages - 1;

        if (_currentPage <= 0)
        {
            UpdatePage(_panels[0], 0);
            UpdatePage(_panels[1], 1);
        }
        else if (_totalPages > 2 && _currentPage >= _totalPages - 1)
        {
            UpdatePage(_panels[1], _currentPage - 1);
            UpdatePage(_panels[2], _currentPage);
        }
        else
        {
            UpdatePage(_panels[0], _currentPage - 1);
            UpdatePage(_panels[1], _currentPage);
            UpdatePage(_panels[2], _currentPage + 1);
        }

        if (showPageButtons)
        {
            _pagePanel.SetTextCounter(_currentPage, _totalPages);
            if (_totalPages > 1)
            {
                _pagePanel.ShowArrow();
            }
            else
            {
                _pagePanel.HideArrow();
            }
        }

        if (tutorialSwipeFinger != null && _totalPages > 1 && Services.Player.IsTutorAvailable(tutorialKey))
        {
            tutorialSwipeFinger.Animate();
            Services.Player.FinishTutor(tutorialKey);
        }

    }

    private void ClearPage(GameObject page, int pageNumber)
    {
        ISetData<string>[] items = _pages[page];
        List<string> range = _items.Where((s, i) => i >= pageNumber *
        items.Length && i < (pageNumber + 1) * items.Length).ToList();

        for (int i = 0; i < items.Length; i++)
        {
            if (i >= range.Count || range[i] == null)
            {
                items[i].Hide();
            }
        }
    }

    private void UpdatePage(GameObject page, int pageNumber)
    {
        ISetData<string>[] items = _pages[page];
        List<string> range = _items.Where((s, i) => i >= pageNumber *
         items.Length && i < (pageNumber + 1) * items.Length).ToList();

        for (int i = 0; i < items.Length; i++)
        {
            if (i >= range.Count || range[i] == null)
            {
                items[i].Hide();
            }
            else
            {
                items[i].SetItem(range[i]);
            }
        }
    }

    public int GetPage()
    {
        return _currentPage;
    }

    public void SetPage(int page)
    {
        if (page == _currentPage)
            return;
        if (page < 0)
            page = 0;

        Vector3 newLocation = _pivotStartPoint;
        newLocation += new Vector3(-_width * page, 0, 0);
        transform.position = newLocation;
        _currentPage = page;
        _panelLocation = newLocation;

        if (showPageButtons)
            _pagePanel.SetTextCounter(_currentPage, _totalPages);

        UpdateData(_items);

        pageChanged?.Invoke();
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

    }
}