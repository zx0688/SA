using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public Action pageChanged;
    public float percentThreshold = 0.2f;
    public float Easing = 0.5f;

    private int _totalPages = 5;
    private int _currentPage = 0;
    private int _prevPage = 0;
    private float _width;

    private Dictionary<GameObject, ISetItem[]> _pages;
    private List<ItemVO> _items;
    private List<GameObject> _panels;
    private int _countItemsPerPage;
    private Vector3 _panelLocation;
    private Vector3 _pivotStartPoint;

    [SerializeField] private PagePanel _pagePanel;

    [SerializeField] private Canvas _parent;

    void Awake()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        _panels = new List<GameObject>() { transform.Find("0").gameObject, transform.Find("1").gameObject, transform.Find("2").gameObject };
        _width = transform.localToWorldMatrix[0] * rectTransform.rect.width + 40;

        RectTransform _rect = _panels[1].GetComponent<RectTransform>();
        float width = Screen.width / (2 * _parent.scaleFactor) + 40 + _rect.GetWidth() / 2;

        _rect.anchoredPosition = new Vector2(width, 0);

        _width = _panels[1].transform.position.x;

        _rect = _panels[2].GetComponent<RectTransform>();
        _rect.anchoredPosition = new Vector2(width * 2, 0);

        _pages = new Dictionary<GameObject, ISetItem[]>();
        _pages.Add(_panels[0], _panels[0].GetComponentsInChildren<ISetItem>());
        _pages.Add(_panels[1], _panels[1].GetComponentsInChildren<ISetItem>());
        _pages.Add(_panels[2], _panels[2].GetComponentsInChildren<ISetItem>());

        _countItemsPerPage = _pages[_panels[0]].Length;
    }

    private void OnEnable()
    {
        _pagePanel.ScrollPageLeft += OnScrollPageLeft;
        _pagePanel.ScrollPageRight += OnScrollPageRight;

        _pagePanel.SetActivePageCounter(true);
    }

    void OnDisable()
    {
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
        _panelLocation = transform.position;
        _pivotStartPoint = transform.position;
        Clear();
    }

    public void OnDrag(PointerEventData data)
    {
        if (_totalPages == 1) return;
        float difference = data.pressPosition.x - data.position.x;
        transform.position = _panelLocation - new Vector3(difference, 0, 0);
    }

    private void UpdatePositions()
    {
        Vector3 diff = _panelLocation - transform.position;
        int page = Mathf.FloorToInt(diff.x / _width);
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
                    first.transform.position = new Vector3(last.transform.position.x + _width, first.transform.position.y, first.transform.position.z);
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
                    last.transform.position = new Vector3(first.transform.position.x - _width, last.transform.position.y, last.transform.position.z);
                    ClearPage(last, _currentPage - 1);

                }

                UpdateData(_items);
                pageChanged?.Invoke();
            }

            _pagePanel.SetTextCounter(_currentPage, _totalPages);

            StartCoroutine(SmoothMove(transform.position, newLocation, Easing));
            _panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, _panelLocation, Easing));
        }
    }

    public void Clear()
    {
        foreach (GameObject page in _pages.Keys)
        {
            ISetItem[] _items = _pages[page];
            foreach (ISetItem i in _items)
            {
                i.Clear();
            }
        }
    }

    public void UpdateData(List<ItemVO> items)
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

    private void ClearPage(GameObject page, int pageNumber)
    {
        ISetItem[] items = _pages[page];
        List<ItemVO> range = _items.Where((s, i) => i >= pageNumber *
        items.Length && i < (pageNumber + 1) * items.Length).ToList();

        for (int i = 0; i < items.Length; i++)
        {
            items[i].Clear();
        }
    }

    private void UpdatePage(GameObject page, int pageNumber)
    {
        ISetItem[] items = _pages[page];
        List<ItemVO> range = _items.Where((s, i) => i >= pageNumber *
         items.Length && i < (pageNumber + 1) * items.Length).ToList();

        for (int i = 0; i < items.Length; i++)
        {
            if (i >= range.Count || range[i] == null)
            {
                items[i].Clear();
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