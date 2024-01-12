
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using haxe.root;

using UnityEngine;
using UnityEngine.Networking;
using UnityJSON;

namespace GameServer
{
    public class HttpBatchServer
    {
        public static event Action<GameResponse> OnResponse;
        public static event Action<List<RewardMeta>> OnGetReward;

        public delegate int ExternalTimeManager();
        public static bool HasFatal { get; private set; }

        // private static string uid = null;
        // private static string token = null;
        // private static string platform = null;

        private static WWWForm form = null;
        private static readonly string URL = "http";

        private static readonly float cooldownTimer = 30f; //seconds
        private static readonly int cooldownChangesCount = 20;
        private static float timer = 0f;

        private static List<GameRequest> batch = null;
        private static Action<int> OnFixClientTime;
        private static ExternalTimeManager getFixedTimestamp;
        private static bool enableConnection = false;
        private static GameResponse response = default;

        private static ProfileData profile = null;
        private static GameMeta meta = null;
        private static bool noServer = false;

        public static async UniTask Init(
           string uid,
           string token,
           string platform,
           bool _noServer,
           GameMeta _meta,
           Action<int> fixTime,
           ExternalTimeManager timeManager)
        {
            HasFatal = false;
            OnFixClientTime = fixTime;
            getFixedTimestamp = timeManager;
            enableConnection = false;
            response = new GameResponse();
            meta = _meta;
            noServer = _noServer;

            timer = 0f;

            form = new WWWForm();
            form.AddField("token", token);
            form.AddField("uid", uid);
            form.AddField("pt", platform);
            form.AddField("v", meta.Version);

            batch = SecurePlayerPrefs.GetListOrEmpty<GameRequest>("batch");
            if (noServer == false && batch.Count > 0)
                await ForceSendBatch();
        }

        public static async UniTask ForceSendBatch(float addDelaySeconds = 0f)
        {
            if (noServer)
                return;

            //try to fix some specific issues with connection platform->server
            if (addDelaySeconds > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(addDelaySeconds));
            }

            form.AddField("batch", JsonUtility.ToJson(batch));

            batch.Clear();

            SecurePlayerPrefs.ClearList("batch");
            SecurePlayerPrefs.Save();
            timer = cooldownTimer;

            using (UnityWebRequest request = UnityWebRequest.Post($"{URL}/change", form))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                await request.SendWebRequest().ToUniTask();

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        break;
                    case UnityWebRequest.Result.Success:
                        string json = request.downloadHandler.text;

                        GameResponse data = JSON.Deserialize<GameResponse>(json);

                        OnFixClientTime?.Invoke(data.Timestamp);

                        //callbacks from server
                        profile.Accept = data.Events.ToDictionary(e => e.Hash, e => e);

                        OnResponse?.Invoke(data);
                        break;
                }
            }
        }

        public static void CloseConnection()
        {
            enableConnection = false;
        }

        public static void OpenConnection()
        {
            if (noServer)
                return;

            enableConnection = true;
            connectionTask().AsAsyncUnitUniTask().Forget();
        }

        private static async UniTask connectionTask()
        {
            await ForceSendBatch();

            do
            {
                await UniTask.Delay(TimeSpan.FromSeconds(5));
                if (timer > 0)
                    timer -= 5;
                if (timer <= 0 && enableConnection)
                {
                    await ForceSendBatch();
                }

            } while (!HasFatal && enableConnection);
        }

        public static async UniTask<ProfileData> GetProfile(IProgress<float> progress = null)
        {
            if (noServer)
            {
                // if (SecurePlayerPrefs.HasKey("profile"))
                // {
                //     profile = JSON.Deserialize<ProfileData>(SecurePlayerPrefs.GetString("profile"));
                //     return profile;
                // 
                profile = SL.CreateProfile(meta, GameTime.Get(), SL.GetRandomInstance());
                return profile;
            }


            using (UnityWebRequest request = UnityWebRequest.Post($"{URL}/profile", form))
            {
                request.SetRequestHeader("Content-Type", "application/json");
                await request.SendWebRequest().ToUniTask(progress: progress);

                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        //resend request...
                        break;
                    case UnityWebRequest.Result.Success:
                        string json = request.downloadHandler.text;
                        GameResponse r = JsonUtility.FromJson<GameResponse>(json);
                        profile = r.Profile;

                        OnFixClientTime?.Invoke(r.Timestamp);
                        return profile;
                }
            }
            throw new Exception("can't get profile");
        }

        public static void Change(GameRequest request)
        {
            request.Timestamp = getFixedTimestamp();
            request.Rid = profile.Rid;
            request.Version = meta.Version;

            //local profile changing
            response.Error = null;

            //Debug.Log($"REQUEST:{JSON.Serialize(request)}");
            if (request.Id != null)
                Debug.Log($"HASH:{request.Hash} TO {request.Id}");
            else
                Debug.Log($"HASH:{request.Hash}");

            SL.Change(request, meta, profile, request.Timestamp, response);
            if (response.Error != null)
                throw new Exception(response.Error);

            Debug.Log($"DECK:{JSON.Serialize(profile.Deck)} LEFT:{profile.Left} RIGHT:{profile.Right}");


            if (profile.RewardEvents.Count > 0)
                OnGetReward?.Invoke(profile.RewardEvents);

            if (noServer)
            {
                string json = JsonUtility.ToJson(profile);
                SecurePlayerPrefs.SetString("profile", json);
                SecurePlayerPrefs.Save();
                return;
            }

            //send change to server
            batch.Add(request);

            if (batch.Count >= cooldownChangesCount)
            {
                ForceSendBatch().Forget();
            }
            else
            {

                SecurePlayerPrefs.AddToList("batch", request);
                SecurePlayerPrefs.Save();
            }
        }
    }
}