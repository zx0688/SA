using System.Reflection.Emit;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Meta;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

public class UI_DropReward : ServiceBehaviour
{
    public float radius = 300f;

    [SerializeField] private BlackLayer layer;
    [SerializeField] private List<GameObject> _particalEffects = new List<GameObject>();

    private List<GameObject> items;
    private List<RewardMeta> waiting;

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

        waiting = new List<RewardMeta>();
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

        RewardMeta i = waiting[0];
        waiting.RemoveAt(0);

        if (i.Count > 0)
            Add(i, new Vector3(0, 0, 0));
        else
            Spend(i, 0);
    }

    private void Add(RewardMeta reward, Vector3 position)
    {
        GameObject item = items[items.Count - 1];

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

        ItemMeta itemMeta = reward.Tp == GameMeta.ITEM ? Services.Data.ItemInfo(reward.Id) : null;

        if (itemMeta != null)
        {
            Services.Assets.SetSpriteIntoImage(item.GetComponent<Image>(), AssetsService.ITEM_ADDRESS(itemMeta.Id), true).Forget();

            if (itemMeta.Particle > 0)
            {
                ScenarioSPECIAL(item, position, itemMeta.Particle);
            }
            else
                ScenarioITEM(item, position);
        }
        else
        {
            throw new NotImplementedException("Animation for drop only item");
        }

    }

    private void ScenarioBUILDING(GameObject item, Vector3 position)
    {


        Vector3 p = positionBuildingAnimation.transform.position;
        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.gameObject.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f).SetDelay(2.5f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();
                CheckWaiting();
            });
            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(1f);
        });
    }

    private void ScenarioSPECIAL(GameObject item, Vector3 position, int particleNumber)
    {
        layer.Show();
        PlayParticle(particleNumber - 1);
        Vector3 p = positionSpecialAnimation.transform.position;
        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        item.gameObject.transform.DOScale(new Vector3(1.6f, 1.6f, 1.6f), 0.6f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            item.gameObject.transform.DOMove(p, 0.4f).SetDelay(1.7f).OnComplete(() =>
            {
                item.gameObject.SetActive(false);
                item.gameObject.transform.DOKill();
                CheckWaiting();

                StopParticle(particleNumber - 1, () => { layer.Hide(); });

            });

            item.gameObject.transform.DOScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f).SetDelay(1.7f);
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

    private void Spend(RewardMeta cost, float delay)
    {
        GameObject item = items[items.Count - 1];

        if (cost.Id == ItemMeta.ACCELERATE_ID || cost.Tp == GameMeta.BUILDING)
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
        Services.Assets.SetSpriteIntoImage(item.GetComponent<Image>(), "Items/" + data.Id, true).Forget();

        Vector3 position = positionSpendAnimation.transform.position;

        item.gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        item.gameObject.transform.DOMove(position, 0.5f).SetDelay(delay).OnComplete(() =>
        {
            item.gameObject.SetActive(false);
            item.gameObject.transform.DOKill();

            Image i = item.GetComponent<Image>();
            Color32 color = i.color;
            color.a = 255;
            i.color = color;
            i.DOKill();
            CheckWaiting();
        });

        Image i = item.GetComponent<Image>();
        Color32 color = i.color;
        color.a = 255;
        i.color = color;

        i.DOFade(0.1f, 0.05f).SetDelay(0.35f + delay);
        item.gameObject.transform.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 0.4f).SetDelay(delay);

    }

    protected override void OnServicesInited()
    {
        base.OnServicesInited();
        Services.Player.OnItemReceived += OnItemReceived;
    }


    private void OnItemReceived(List<RewardMeta> rewards)
    {
        List<RewardMeta> items = rewards.Where(r => r.Tp == GameMeta.ITEM).ToList();
        if (items.Count == 1)
        {
            if (items[0].Count > 0)
                Add(items[0], new Vector3(0, 0, 0));
            else
            {
                if (Services.Player.GetCountItemByID(items[0].Id) + Math.Abs(items[0].Count) + items[0].Count > 0)
                    Spend(items[0], 0);
            }

            return;
        }

        float angle = UnityEngine.Random.Range(0f, 6.28f);
        float step = 6.28f / items.Count;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].Count < 0)
            {
                if (Services.Player.GetCountItemByID(items[i].Id) + Math.Abs(items[i].Count) + items[i].Count > 0)
                    Spend(items[i], i * 0.4f);
                continue;
            }
            angle += step * i;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
            Add(items[i], pos);
        }
    }
}