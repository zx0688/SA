using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;

namespace GameServer
{
    public class HttpBatchServer
    {
        public static readonly string URL = "http";
        public static Action ListenExceptions;
        public static Action<GameServerEvent> ListenEvents;
        public static bool HasFatal { get; private set; }

        // private static string uid = null;
        // private static string token = null;
        // private static string platform = null;
        private static int rid = 0;
        private static WWWForm form = null;

        private static readonly float cooldownTimer = 30f; //seconds
        private static readonly int cooldownChangesCount = 20;
        private static float timer = 0f;

        private static List<GameServerTrigger> batch = null;
        private static Action<int> fixGlobalTime;
        public delegate int ExternalTimeManager();
        private static ExternalTimeManager GetTimestamp;

        public static void Init(
            string uid,
            string token,
            string platform,
            Action<int> fixTime,
            ExternalTimeManager getTimestamp)
        {
            //uid = uid;
            //token = token;
            //platform = platform;
            HasFatal = false;
            fixGlobalTime = fixTime;
            GetTimestamp = getTimestamp;

            timer = 0f;
            batch = SecurePlayerPrefs.GetListOrEmpty<GameServerTrigger>("batch");

            form = new WWWForm();
            form.AddField("token", token);
            form.AddField("uid", uid);
            form.AddField("pt", platform);
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
                        string json = SecurePlayerPrefs.GetString("profile") as string;

                        GameServerEvent t = null;
                        ListenEvents?.Invoke(t);
                        break;
                }
            }
        }

        public static async UniTask OpenConnection()
        {
            await ForceSendBatch();
            while (true && !HasFatal)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(5));
                if (timer > 0)
                    timer -= 5;
                if (timer <= 0)
                {
                    await ForceSendBatch();
                }
            }
        }

        public static async UniTask<T> Profile<T>() where T : GameServerData
        {
            using (UnityWebRequest request = UnityWebRequest.Post($"{URL}/profile", form))
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
                        string json = SecurePlayerPrefs.GetString("profile") as string;
                        T data = JsonUtility.FromJson<T>(json);
                        rid = data.rid;

                        fixGlobalTime?.Invoke(23);

                        return data;
                }
            }

            return null;
        }

        public static void Change(GameServerTrigger data)
        {
            //SL.change({ });
            batch.Add(data);
            data.time = GetTimestamp();


            if (batch.Count >= cooldownChangesCount)
            {
                ForceSendBatch().Forget();
            }
            else
            {
                SecurePlayerPrefs.AddToList("batch", data);
                SecurePlayerPrefs.Save();
            }
        }
    }

    [Serializable]
    public class GameServerData
    {
        public int rid = 0;
        public int timestamp = 0;
    }
    [Serializable]
    public class GameServerEvent
    {
        public int rid = 0;
        public string hash = null;
    }

    [Serializable]
    public class GameServerTrigger
    {
        public int rid = 0;
        public int time = 0;
    }
}