using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using GameServer;
using Cysharp.Threading.Tasks;

public class UIDropReward : ServiceBehaviour
{
    public float radius = 300f;

    [SerializeField] private BlackLayer layer;
    [SerializeField] private List<GameObject> _particalEffects = new List<GameObject>();

    [SerializeField] private List<GameObject> items;
    private List<ItemData> waiting;

    [SerializeField] private GameObject positionAddAnimation;
    [SerializeField] private GameObject positionSpendAnimation;
    [SerializeField] private GameObject positionSpecialAnimation;
    [SerializeField] private GameObject positionQuestAnimation;

    protected override void Awake()
    {
        base.Awake();

        foreach (GameObject i in items)
            i.gameObject.SetActive(false);

        waiting = new List<ItemData>();
    }

    private void PlayParticle(int index)
    {
        GameObject particle = _particalEffects[index].gameObject;
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ParticleSystem[] psc = particle.GetComponentsInChildren<ParticleSystem>();
        particle.SetActive(true);
        if (ps != null) ps.Play();
        foreach (ParticleSystem item in psc) { item.Play(); }
    }

    private async void StopParticle(int index, Action callback)
    {
        GameObject particle = _particalEffects[index].gameObject;
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        ParticleSystem[] psc = particle.GetComponentsInChildren<ParticleSystem>();
        if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        foreach (ParticleSystem item in psc) { item.Stop(true, ParticleSystemStopBehavior.StopEmitting); }
        if (ps == null)
            ps = psc[0];

        while (!ps.isStopped) { await Task.Yield(); }

        particle.SetActive(false);
        callback?.Invoke();
    }

    private void CheckWaiting()
    {
        if (waiting.Count == 0)
        {
            //gameObject.SetActive (false);
            return;
        }

        ItemData i = waiting[0];
        waiting.RemoveAt(0);

        if (i.Count > 0)
            Add(i, new Vector3(0, 0, 0));
        else
            Spend(i, 0);
    }

    private void Add(ItemData reward, Vector3 position)
    {
        GameObject item = items[items.Count - 1];

        if (item.activeSelf)
        {
            waiting.Add(reward);
            return;
        }


        items.RemoveAt(items.Count - 1);
        items.Insert(0, item);

        item.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        ItemMeta itemMeta = Services.Meta.Game.Items[reward.Id];
        item.GetComponent<Image>().LoadItemIcon(reward.Id);

        if (itemMeta.Particle > 0)
        {
            ScriptSPECIAL(item, position, itemMeta.Particle).Forget();
        }
        else
        {
            ScriptITEM(item, position, positionAddAnimation).Forget();
        }

    }

    private async UniTaskVoid ScriptSPECIAL(GameObject item, Vector3 position, int particleNumber)
    {
        layer.Show(0.1f);
        item.SetActive(false);
        await UniTask.DelayFrame(10);
        item.SetActive(true);

        PlayParticle(particleNumber - 1);
        Vector3 p = positionSpecialAnimation.transform.position;
        item.gameObject.transform.DOKill();
        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.gameObject.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f, true).SetDelay(1.7f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();
                CheckWaiting();

                StopParticle(particleNumber - 1, () => { layer.Hide(); });

            });

            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(1.7f);
        });
    }

    private async UniTaskVoid ScriptITEM(GameObject item, Vector3 position, GameObject targetPosition)
    {

        Vector3 p = targetPosition.transform.position;

        item.SetActive(false);
        await UniTask.DelayFrame(10);
        item.SetActive(true);

        item.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        item.gameObject.transform.DOKill();
        item.gameObject.transform.DOMove(position + item.gameObject.transform.position, 0.6f).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f, true).SetDelay(0.5f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();

                CheckWaiting();
            });
            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(0.5f);
        });
        item.gameObject.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), 0.6f);

    }

    private void Spend(ItemData cost, float delay)
    {
        GameObject item = items[items.Count - 1];

        if (item.activeInHierarchy)
        {
            waiting.Add(cost);
            return;
        }

        gameObject.SetActive(true);
        ItemMeta data = null;

        items.RemoveAt(items.Count - 1);
        items.Insert(0, item);

        item.SetActive(true);
        Vector3 targetPosition = positionAddAnimation.GetComponent<RectTransform>().anchoredPosition;

        item.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        item.GetComponent<Image>().LoadItemIcon(cost.Id);

        Vector3 position = positionSpendAnimation.transform.position;

        item.gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        item.gameObject.transform.DOMove(position, 0.5f, true).SetDelay(delay).OnComplete(() =>
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.DOKill();

            // Image i = item.GetComponent<Image>();
            // Color32 color = i.color;
            // color.a = 255;
            // i.color = color;
            // i.DOKill();
            CheckWaiting();
        });

        // Image i = item.GetComponent<Image>();
        // Color32 color = i.color;
        // color.a = 255;
        // i.color = color;

        //i.DOFade(0.1f, 0.1f).SetDelay(0.4f + delay);
        item.gameObject.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.6f).SetDelay(delay);

    }

    protected override void OnServicesInited()
    {
        base.OnServicesInited();
        HttpBatchServer.OnGetReward += OnItemReceived;
        Services.Player.OnQuestStart += OnQuestStart;
    }

    private void OnQuestStart()
    {
        GameObject item = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        items.Insert(0, item);

        item.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        item.GetComponent<Image>().LoadItemIcon("1000");
        item.SetActive(true);
        ScriptITEM(item, new Vector3(0, 0, 0), positionQuestAnimation);
    }

    private void OnItemReceived(List<RewardMeta> rewards)
    {
        List<ItemData> itemsData = rewards.Where(r => r.Type == ConditionMeta.ITEM).Select(r => new ItemData(Id: r.Id, Count: r.Count)).ToList();

        if (itemsData.Count == 1 && itemsData[0].Count == 1)
        {
            if (itemsData[0].Count > 0)
                Add(itemsData[0], new Vector3(0, 0, 0));
            else
                Spend(itemsData[0], 0);
            return;
        }

        float angle = UnityEngine.Random.Range(0f, 6.28f);
        float step = 6.28f / itemsData.Count;
        for (int i = 0; i < itemsData.Count; i++)
        {
            ItemData item = itemsData[i];
            if (item.Count < 0)
            {
                Spend(itemsData[i], i * 0.15f + 0.1f);
                continue;
            }
            angle += step * i;
            for (int j = 0; j < item.Count; j++)
            {
                float da = UnityEngine.Random.Range(0f, 6.28f);
                Vector3 pos = new Vector3(Mathf.Cos(angle + da - 3.14f) * radius, Mathf.Sin(angle + da - 3.14f) * radius, 0f);
                Add(itemsData[i], pos);
            }
        }
    }
}