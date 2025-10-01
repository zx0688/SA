using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityJSON;

public class MetaService
{


    private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=16pS8YzoOkOwu_sPPvm9JKKFya9-XUN96"; //"https://drive.google.com/uc?export=download&id=1tWCVbt3hUimhZPh6lABPLJefNZYotS8K";

    private static readonly string URL_META = "";
    private static readonly string URL_VERSION = "";

    public GameMeta Game;

    public SkillMeta GetSkillItemBySlot(int index) => Game.Skills.Values.FirstOrDefault(s => s.Slot == index);
    public CardMeta GetCard(TriggerMeta t) => Game.Cards[t.Id];

    //public Dictionary<List<ConditionMeta>, List<RewardMeta>> Recipes = new Dictionary<List<ConditionMeta>, List<RewardMeta>>();

    //public static readonly List<RewardMeta> EMPTY_REWARD = new List<RewardMeta>();
    //public static readonly List<ConditionMeta> EMPTY_CONDITIONS = new List<ConditionMeta>();

    // public CardMeta[] CardDeckMeta => Services.Player.GetPlayerVO.Location switch
    // {
    //     27912732 => ConditionMeta.CARDs,
    //     _ => throw new Exception("undefined location ID")
    // };
    // public ItemData accelerateItem; 


    // public List<ItemMeta> ItemInfoByType(int type)
    // {
    //     return ConditionMeta.ITEMs.Where(c => c.Type == type).ToList();
    // }


    public bool MatchReward(List<RewardMeta> reward1, List<RewardMeta> reward2)
    {
        for (int i = 0; i < reward1.Count; i++)
        {
            RewardMeta r1 = reward1[i];
            RewardMeta r2 = reward2.Find(r => r.Type == r1.Type);
            if (r1.Count != r2.Count)
                return false;
        }
        return true;
    }

    public async UniTask Init(IProgress<float> progress = null)
    {
        string asset = await Services.Assets.GetJson("meta", GOOGLE_DRIVE, progress, LoadContentOption.UseFileVersion);
        Debug.Log(asset);

        //await UniTask.SwitchToThreadPool();
        Game = JSON.Deserialize<GameMeta>(asset);
        //await UniTask.SwitchToMainThread();
#if UNITY_EDITOR
        ShowUnvalidateMetaData();
#endif
        //Debug.Log(JsonUtility.ToJson(Game.Cards["28440109"]));
    }

    public void GetAllRecursiveCardsFromGroup(TriggerMeta[] triggers, List<CardMeta> cards)
    {
        if (triggers == null)
            return;
        foreach (TriggerMeta trigger in triggers)
        {
            if (trigger.Type == CardMeta.TYPE_CARD)
            {
                var card = Game.Cards[trigger.Id];
                cards.Add(card);
            }
            else if (trigger.Type == CardMeta.TYPE_GROUP)
            {
                var group = Game.Groups[trigger.Id];
                GetAllRecursiveCardsFromGroup(group.Cards, cards);
            }
        }
    }

#if UNITY_EDITOR
    //------------------
    private static void ShowUnvalidateMetaData()
    {
        var cards = Services.Meta.Game.Cards.Values.ToList();


        foreach (var card in cards)
        {

            //ShowUnvalidateCard(card);
        }

        // var localizationData = Services.Assets.GetLocalizationData();
        // foreach (var key in localizationData.CardName.Keys)
        // {
        //     if (!cards.Exists(c => c.Name == key))
        //         DebugMessage($"ключ {key} с именем карты {localizationData.CardName[key]} нет ни в одной карте");
        // }
        // foreach (var key in localizationData.CardDescription.Keys)
        // {
        //     if (!cards.Exists(c => (c.Descs.HasTexts() && c.Descs.Contains(key)) || (c.OnlyOnce.HasTexts() && c.OnlyOnce.Contains(key)) || c.RewardText == key))
        //         DebugMessage($"ключ {key} с описанием карты {localizationData.CardDescription[key]} нет ни в одной карте");
        // }

    }

    private static void DebugMessage(string text)
    {
        Debug.LogError(text);
    }

    public static void ShowUnvalidateCard(CardMeta card)
    {
        return;
        if ((card.Reward.HasReward() || card.Cost.HasReward()) && card.RewardText == null)
            CardException(card, "должен быть задан текст награды!");
        if (card.Reward.HasReward() && !card.Reward.ToList().Exists(r => r.ToList().Exists(rr => rr.Chance == 0)) &&
            !card.IfNothing.HasTexts())
            CardException(card, "карта должна содержать текст если награды нет");
        if (card.Shure != null && card.Next == null)
            CardException(card, "карта должна содержать карточку подтверждения!");
        if (card.Call && card.Next == null)
            CardException(card, "карта вызываема и должна иметь выбор");
        //if (card.Call && card.Next != null && card.Next.Any(n => n.Next != null))
        //    CardException(card, "карта с вызовом не должна иметь Next в выборах");

        if (card.TradeLimit > 0)
        {

        }

        var cards = Services.Meta.Game.Cards.Values.ToList();
        var groups = Services.Meta.Game.Groups.Values.ToList();
        // if (!groups.Exists(g => Array.Exists(g.Cards, g => g.Id == card.Id) && !cards.Exists(c =>
        //     Array.Exists(c.Over, o => o.Id == card.Id)
        //     || Array.Exists(c.Next, n => n.Id == card.Id)
        //     || Array.Exists(c.IfNot, c => c.Id == card.Id))))
        //     CardException(card, "Карта нигде не исользуется!");

        if (cards.Exists(c => c.Id == card.Id && c != card))
            CardException(card, "У карты есть Дубликат!");

    }

    private static void CardException(CardMeta card, string message)
    {
        string m = $"card {card.Id.ColorizeHH("ff0000")}: {message} ";
        Debug.LogError(m);
        //throw new Exception();
    }
#endif

}
