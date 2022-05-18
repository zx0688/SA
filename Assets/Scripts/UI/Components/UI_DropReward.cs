using System;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_DropReward : ServiceBehaviour
{

    public float radius = 300f;
    private List<GameObject> items;
    private List<RewardData> waiting;

    private GameObject positionAddAnimation;
    private GameObject positionSpendAnimation;
    private GameObject positionSpecialAnimation;
    private GameObject positionBuildingAnimation;

    protected override void Awake()
    {
        base.Awake();

        items = new List<GameObject>();
        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image i in images)
        {
            i.gameObject.SetActive(false);
            items.Add(i.gameObject);
        }

        positionAddAnimation = transform.Find("Add").gameObject;
        positionSpendAnimation = transform.Find("Spend").gameObject;
        positionSpecialAnimation = transform.Find("Special").gameObject;
        positionBuildingAnimation = transform.Find("Building").gameObject;

        waiting = new List<RewardData>();
    }

    private void CheckWaiting()
    {
        if (waiting.Count == 0)
        {
            //gameObject.SetActive (false);
            return;
        }

        RewardData i = waiting[0];
        waiting.RemoveAt(0);

        if (i.Count > 0)
            Add(i, new Vector3(0, 0, 0));
        else
            Spend(i, 0);
    }

    private void Add(RewardData reward, Vector3 position)
    {
        GameObject item = items[items.Count - 1];

        Debug.Log("drop " + reward.Id);
        //if (reward.id == ItemData.LEVEL_ID)
        //    return;

        if (item.activeSelf)
        {
            waiting.Add(reward);
            return;
        }

        gameObject.SetActive(true);

        items.RemoveAt(items.Count - 1);
        items.Insert(0, item);

        item.SetActive(true);
        item.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);

        if (reward.Tp == 0)//DataService.SKILL_ID)
        {

            Services.Assets.SetSpriteIntoImage(item.GetComponent<Image>(), "Skills/" + reward.Id + "/icon", true).Forget();
            ScenarioSPETIAL(item, position);

        }
        else if (reward.Tp == 0)//DataService.BUILDING_ID)
        {

            Services.Assets.SetSpriteIntoImage(item.GetComponent<Image>(), "Buildings/" + reward.Id + "/icon", true).Forget();
            ScenarioBUILDING(item, position);

        }
        else
        {

            Services.Assets.SetSpriteIntoImage(item.GetComponent<Image>(), "Items/" + reward.Id + "/icon", true).Forget();
            ScenarioITEM(item, position);

        }

    }

    private void ScenarioBUILDING(GameObject item, Vector3 position)
    {
        Vector3 p = positionBuildingAnimation.transform.position;
        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.gameObject.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f).SetDelay(1f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();
                CheckWaiting();
            });
            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(1f);
        });
    }
    private void ScenarioSPETIAL(GameObject item, Vector3 position)
    {
        Vector3 p = positionSpecialAnimation.transform.position;
        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.gameObject.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f).SetDelay(1f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();
                CheckWaiting();
            });
            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(1f);
        });
    }
    private void ScenarioITEM(GameObject item, Vector3 position)
    {
        Vector3 p = positionAddAnimation.transform.position;
        item.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        item.gameObject.transform.DOMove(position + item.gameObject.transform.position, 0.7f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f).SetDelay(0.2f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();

                CheckWaiting();
            });
            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(0.2f);
        });
        item.gameObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.7f);
    }

    private void Spend(RewardData cost, float delay)
    {
        GameObject item = items[items.Count - 1];

        if (cost.Id == ItemMeta.ACCELERATE_ID || cost.Tp == MetaData.BUILDING)
            return;

        if (item.activeInHierarchy)
        {
            waiting.Add(cost);
            return;
        }

        gameObject.SetActive(true);
        ItemMeta data = Services.Data.ItemInfo(cost.Id);

        items.RemoveAt(items.Count - 1);
        items.Insert(0, item);

        item.SetActive(true);
        Vector3 targetPosition = positionAddAnimation.GetComponent<RectTransform>().anchoredPosition;

        item.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        Services.Assets.SetSpriteIntoImage(item.GetComponent<Image>(), "Items/" + data.Id + "/icon", true).Forget();

        Vector3 position = positionSpendAnimation.transform.position;

        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        item.gameObject.transform.DOMove(position, 0.7f).SetDelay(delay).OnComplete(() =>
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.DOKill();
            CheckWaiting();
        });
        item.gameObject.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.7f).SetDelay(delay);

    }

    protected override void OnServicesInited()
    {
        base.OnServicesInited();
        // Services.OnItemReceived += OnItemReceived;

    }

    private void OnItemReceived(List<RewardData> items)
    {

        if (items.Count == 1)
        {
            if (items[0].Count > 0)
                Add(items[0], new Vector3(0, 0, 0));
            else
                Spend(items[0], 0);
            return;
        }

        float angle = UnityEngine.Random.Range(0f, 6.28f);
        float step = 6.28f / items.Count;
        for (int i = 0; i < items.Count; i++)
        {

            if (items[i].Count < 0)
            {
                Spend(items[i], i * 0.6f);
                continue;
            }
            angle += step * i;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            Add(items[i], pos);
        }
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {

    }
}