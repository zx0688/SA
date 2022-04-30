using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
{

    public Action pageChanged;
    private Vector3 panelLocation;
    public float percentThreshold = 0.2f;

    public float easing = 0.5f;
    private int totalPages = 5;
    private int currentPage = 0;
    private int prevPage = 0;

    private float width;

    //private Dictionary<GameObject, ISetItem[]> pages;
    private List<ItemVO> items;

    private List<GameObject> panels;
    private int countItemsPerPage;

    [SerializeField]
    public Text pageNumberText;

    void Awake()
    {

        RectTransform rectTransform = GetComponent<RectTransform>();

        panels = new List<GameObject>() { transform.Find("0").gameObject, transform.Find("1").gameObject, transform.Find("2").gameObject };

        width = transform.localToWorldMatrix[0] * rectTransform.rect.width;

        Transform trans = panels[0].transform;
        panels[1].transform.position = new Vector3(trans.position.x + width, trans.position.y, trans.position.z);
        trans = panels[1].transform;
        panels[2].transform.position = new Vector3(trans.position.x + width, trans.position.y, trans.position.z);

        /*pages = new Dictionary<GameObject, ISetItem[]>();
        pages.Add(panels[0], panels[0].GetComponentsInChildren<ISetItem>());
        pages.Add(panels[1], panels[1].GetComponentsInChildren<ISetItem>());
        pages.Add(panels[2], panels[2].GetComponentsInChildren<ISetItem>());

        countItemsPerPage = pages[panels[0]].Length;
        */

    }
    // Start is called before the first frame update
    void Start()
    {
        panelLocation = transform.position;
        Clear();
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        transform.position = panelLocation - new Vector3(difference, 0, 0);
    }

    private void UpdatePositions()
    {
        Vector3 diff = panelLocation - transform.position;
        int page = Mathf.FloorToInt(diff.x / width);
    }

    public void OnEndDrag(PointerEventData data)
    {
        float percentage = (data.pressPosition.x - data.position.x) / width;

        if (Mathf.Abs(percentage) >= percentThreshold)
        {
            Vector3 newLocation = panelLocation;
            if (percentage > 0 && currentPage < totalPages - 1)
            {
                currentPage++;
                newLocation += new Vector3(-width, 0, 0);
                Debug.Log("right");

                if (currentPage > 1 && currentPage < totalPages - 1)
                {
                    panels = panels.OrderBy(p => p.transform.position.x).ToList();
                    GameObject first = panels.FirstOrDefault();
                    GameObject last = panels.LastOrDefault();
                    first.transform.position = new Vector3(last.transform.position.x + width, first.transform.position.y, first.transform.position.z);
                    ClearPage(first, currentPage + 1);

                }

                UpdateData(items);
                pageChanged?.Invoke();
            }
            else if (percentage < 0 && currentPage > 0)
            {
                currentPage--;
                newLocation += new Vector3(width, 0, 0);
                Debug.Log("left");
                if (currentPage < totalPages - 2 && currentPage > 0)
                {
                    panels = panels.OrderBy(p => p.transform.position.x).ToList();
                    GameObject first = panels.FirstOrDefault();
                    GameObject last = panels.LastOrDefault();
                    last.transform.position = new Vector3(first.transform.position.x - width, last.transform.position.y, last.transform.position.z);
                    ClearPage(last, currentPage - 1);

                }

                UpdateData(items);
                pageChanged?.Invoke();
            }

            if (pageNumberText != null)
            {
                pageNumberText.text = (currentPage + 1) + "/" + totalPages;
            }

            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }
        else
        {
            StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
        }
    }

    public void Clear()
    {

        /* foreach (GameObject page in pages.Keys)
         {
             ISetItem[] _items = pages[page];
             foreach (ISetItem i in _items)
             {
                 i.SetItem(null);
             }
         }*/
    }

    public void UpdateData(List<ItemVO> items)
    {
        this.items = items;
        totalPages = items.Count == 0 ? 1 : Mathf.CeilToInt((float)items.Count / (float)countItemsPerPage);


        panels = panels.OrderBy(p => p.transform.position.x).ToList();

        if (currentPage > totalPages - 1)
            currentPage = totalPages - 1;

        if (currentPage <= 0)
        {
            UpdatePage(panels[0], 0);
            UpdatePage(panels[1], 1);
        }
        else if (currentPage >= totalPages - 1)
        {
            UpdatePage(panels[1], currentPage - 1);
            UpdatePage(panels[2], currentPage);
        }
        else
        {
            UpdatePage(panels[0], currentPage - 1);
            UpdatePage(panels[1], currentPage);
            UpdatePage(panels[2], currentPage + 1);
        }

        if (pageNumberText != null)
        {
            pageNumberText.text = (currentPage + 1) + "/" + totalPages;
        }
    }

    private void ClearPage(GameObject page, int pageNumber)
    {
        /* ISetItem[] _items = pages[page];
         List<ItemVO> range = items.Where((s, i) => i >= pageNumber *
             _items.Length && i < (pageNumber + 1) * _items.Length).ToList();

         for (int i = 0; i < _items.Length; i++)
         {
             _items[i].SetItem(null);
         }*/
    }
    private void UpdatePage(GameObject page, int pageNumber)
    {
        /*ISetItem[] _items = pages[page];
        List<ItemVO> range = items.Where((s, i) => i >= pageNumber *
            _items.Length && i < (pageNumber + 1) * _items.Length).ToList();

        for (int i = 0; i < _items.Length; i++)
        {
            if (i >= range.Count || range[i] == null)
            {
                _items[i].SetItem(null);
            }
            else
            {
                _items[i].SetItem(range[i]);
            }
        }*/

    }

    public int GetPage()
    {
        return currentPage;
    }

    public void SetPage(int page)
    {

        if (page == currentPage)
            return;
        if (page < 0)
            page = 0;

        Vector3 newLocation = transform.position;
        newLocation += new Vector3(-width * (page - currentPage), 0, 0);
        transform.position = newLocation;
        currentPage = page;

        UpdateData(this.items);

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