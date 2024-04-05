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


#if UNITY_STANDALONE_WIN
    private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=1dXlv4S11TvwJTnc6qqFJIJJ_K6yXgzel";
#else
    private static readonly string GOOGLE_DRIVE = "https://drive.google.com/uc?export=download&id=1dXlv4S11TvwJTnc6qqFJIJJ_K6yXgzel"; //"https://drive.google.com/uc?export=download&id=1tWCVbt3hUimhZPh6lABPLJefNZYotS8K";
#endif
    private static readonly string URL_META = "";
    private static readonly string URL_VERSION = "";

    public GameMeta Game;

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

        await UniTask.SwitchToThreadPool();
        Game = JSON.Deserialize<GameMeta>(asset);
        await UniTask.SwitchToMainThread();

        Debug.Log(JsonUtility.ToJson(Game.Cards["28440109"]));
    }

}
