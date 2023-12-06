
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
        public static Action<GameResponse> OnResponse;
        public static Action<List<RewardMeta>> ListenRewards;

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
        private static Action<int> fixGlobalTime;
        private static ExternalTimeManager getTimestamp;
        private static bool enableConnection = false;
        private static GameResponse response = default;

        private static ProfileData profile = null;
        private static GameMeta meta = null;
        private static bool noServer = false;

        public static void Init(
            string uid,
            string token,
            string platform,
            bool _noServer,
            GameMeta _meta,
            Action<int> fixTime,
            ExternalTimeManager timeManager)
        {
            HasFatal = false;
            fixGlobalTime = fixTime;
            getTimestamp = timeManager;
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


            //restore changes
            batch = SecurePlayerPrefs.GetListOrEmpty<GameRequest>("batch");
        }

        public static async UniTask ForceSendBatch()
        {
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

                        fixGlobalTime?.Invoke(data.Timestamp);
                        profile.Events = data.Events.ToDictionary(e => e.Hash, e => e);

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
                profile = SL.CreateProfile(meta, GameTime.Current, SL.GetRandomInstance());
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
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        break;
                    case UnityWebRequest.Result.Success:
                        string json = request.downloadHandler.text;
                        GameResponse r = JsonUtility.FromJson<GameResponse>(json);
                        profile = r.Profile;

                        fixGlobalTime?.Invoke(r.Timestamp);
                        return profile;
                }
            }
            return default;
        }

        public static void Change(GameRequest request)
        {
            request.Timestamp = getTimestamp();
            request.Rid = profile.Rid;
            request.Version = meta.Version;

            //local profile changing
            response.Error = null;
            List<RewardMeta> reward = new List<RewardMeta>();

            Debug.Log($"REQUEST:{JSON.Serialize(request)}");
            SL.Change(request, meta, profile, request.Timestamp, response, reward, SL.GetRandomInstance());
            if (response.Error != null)
                throw new Exception(response.Error);

            if (reward.Count > 0)
                ListenRewards?.Invoke(reward);

            if (noServer)
            {
                string json = JsonUtility.ToJson(profile);
                SecurePlayerPrefs.SetString("profile", json);
                SecurePlayerPrefs.Save();
                return;
            }

            //send change to server
            batch.Add(request);

            //data.time = GetTimestamp();
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