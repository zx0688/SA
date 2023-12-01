using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityJSON;


public class AssetsService
{
    private static readonly string GOOGLE_DRIVE_LOCALIZATION = "https://drive.google.com/uc?export=download&id=1Jwzs1es7S_hsdFoL76dov7_51c1gX-on";
    // private static AssetsService _meta = null;
    // public static AssetsService Meta
    // {
    //     get
    //     {
    //         if (_meta == null)
    //             _meta = new MetaService();
    //         return _meta;
    //     }
    // }

    public Action OnBadConnection;
    public string Localize(string key) => LocalDic.Dic.TryGetValue(key, out string res) ? res : $"[{key}";
    private LocalizationData LocalDic;

    private static readonly string BASE_URL = "";

    private int saveCount;


    //private Dictionary<string, Sprite> spriteCache;

    public async UniTask Init(string lang, IProgress<float> progress = null)
    {

        saveCount = PlayerPrefs.GetInt("savecount", 10);
        /*
        #if UNITY_EDITOR
                    var path = Path.Combine (Directory.GetParent (Application.dataPath).FullName, "_EditorCache");
        #else
                    var path = Path.Combine (Application.persistentDataPath, "_AppCache");
        #endif
                    if (!System.IO.Directory.Exists (path)) {
                        System.IO.Directory.CreateDirectory (path);
        #if UNITY_IOS
                        UnityEngine.iOS.Device.SetNoBackupFlag (path);
        #endif
                    }

                    Caching.currentCacheForWriting = Caching.AddCache (path);
        */

        var json = await GetJson($"localization_{lang}", false, GOOGLE_DRIVE_LOCALIZATION, true, progress);
        LocalDic = JSON.Deserialize<LocalizationData>(json);

        await UniTask.Yield();
    }

    public async UniTask<string> GetJson(string name, bool fromResources, string url, bool saveLocal, IProgress<float> progress = null)
    {
        if (fromResources)
        {
            var (isCanceled, asset) = await Resources.LoadAsync<TextAsset>(name).ToUniTask(progress: progress).SuppressCancellationThrow();
            if (asset != null && isCanceled == false)
                return (asset as TextAsset).text;
        }
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            await request.SendWebRequest().ToUniTask(progress: progress).Timeout(TimeSpan.FromSeconds(5));
            if (request.result != UnityWebRequest.Result.Success)
            {
                OnBadConnection?.Invoke();
                Debug.LogWarning($"can't connect to {url}.. try again in 3 sec");
                await UniTask.Delay(3000);
                await request.SendWebRequest().ToUniTask(progress: progress).Timeout(TimeSpan.FromSeconds(3));
            }
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    if (saveLocal == true)
                    {
                        Debug.LogWarning($"can't get data {name} from {url}.. trying to get from cache");
                        string fromFile;
                        if (TryGetJsonFromCache(name, out fromFile))
                            return fromFile;
                    }
                    Debug.LogWarning($"can't get data {name} from cache.. hope this dont invoke an issue");
                    break;
                case UnityWebRequest.Result.Success:
                    string json = request.downloadHandler.text;
                    if (saveLocal)
                        TryCacheJson(name, json);

                    return json;
            }
            return null;
        }
    }

    public async UniTaskVoid SetSpriteIntoImageData(Image icon, int type, int id, bool fromResources, IProgress<float> progress = null)
    {
        //Debug.Log($"load:{name}");
        string name;
        switch (type)
        {
            case 1:
                name = $"Items/{id}";
                break;
            case 2:
                name = $"Building/{id}/icon";
                break;
            default:
                name = $"Items/{0}/icon";
                break;
        }

        icon.sprite = await Services.Assets.GetSprite(name, fromResources, progress);
    }

    public async UniTaskVoid SetSpriteIntoImage(Image icon, string name, bool fromResources, IProgress<float> progress = null)
    {
        //Debug.Log($"load:{name}");
        icon.sprite = await Services.Assets.GetSprite(name, fromResources, progress);
    }

    public async UniTaskVoid PlaySound(string name, AudioSource source)
    {
        AudioClip clip = await GetSound(name, true);
        source.PlayOneShot(clip);
    }
    public async UniTask<AudioClip> GetSound(string name, bool fromResources, IProgress<float> progress = null)
    {

        if (fromResources)
        {

            return Resources.Load<AudioClip>("Sound/" + name);
            //var (isCanceled, asset) = await Resources.LoadAsync<Sprite> (name).ToUniTask (progress: progress).SuppressCancellationThrow ();
            // if (asset != null && isCanceled == false) {

            //    return asset as Sprite;
            //}
        }
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(BASE_URL + name, AudioType.WAV);

        await request.SendWebRequest().ToUniTask(progress: progress);

        if (request.isNetworkError || request.isHttpError)
        {
            //Debug.LogWarning (request.error);
            Debug.LogWarning("Cant find " + name);
            return null;
        }

        AudioClip clip = ((DownloadHandlerAudioClip)request.downloadHandler).audioClip;
        return clip;
    }

    public async UniTask<Sprite> GetSprite(string name, bool fromResources, IProgress<float> progress = null)
    {

        if (fromResources)
        {

            return Resources.Load<Sprite>(name);
            //var (isCanceled, asset) = await Resources.LoadAsync<Sprite> (name).ToUniTask (progress: progress).SuppressCancellationThrow ();
            // if (asset != null && isCanceled == false) {

            //    return asset as Sprite;
            //}
        }

        Sprite sprite = null;//GetSpriteFromCache(name);
        if (sprite != null)
        {

            //await UniTask.SwitchToMainThread();
            return sprite;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(BASE_URL + name);

        await request.SendWebRequest().ToUniTask(progress: progress);

        if (request.isNetworkError || request.isHttpError)
        {
            //Debug.LogWarning (request.error);
            Debug.LogWarning("Cant find " + name);
            return null;
        }

        Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        //CacheTexture(name, request.downloadHandler.data);

        sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        return sprite;
    }
    // private void CacheTexture(string name, byte[] data)
    // {
    //     var cacheFilePath = Path.Combine("CachePath", name + ".texture");
    //     File.WriteAllBytes(cacheFilePath, data);
    // }
    private bool TryCacheJson(string name, string json)
    {
        //SimpleDiskUtils.DiskUtils.CheckAvailableSpace ();
        var cacheFilePath = Path.Combine(Application.persistentDataPath, ZString.Format("{0}.json", name));
        File.WriteAllText(cacheFilePath, SecurePlayerPrefs.Encrypt(json));
        try
        {
            File.WriteAllText(cacheFilePath, SecurePlayerPrefs.Encrypt(json));
            return File.Exists(cacheFilePath);
        }
        catch (Exception)
        {
            Debug.LogError($"Couldn't save data {name} to file");
            return false;
        }
    }

    private bool TryGetJsonFromCache(string name, out string json)
    {
        var cacheFilePath = Path.Combine(Application.persistentDataPath, ZString.Format("{0}.json", name));
        if (!File.Exists(cacheFilePath))
        {
            json = "";
            return false;
        }
        try
        {
            json = SecurePlayerPrefs.Decrypt(File.ReadAllText(cacheFilePath));
        }
        catch (Exception)
        {
            json = "";
            return false;
        }
        return true;
    }
    // private Sprite GetSpriteFromCache(string name)
    // {
    //     var cacheFilePath = Path.Combine("", name + ".texture");
    //     if (!File.Exists(cacheFilePath))
    //         return null;

    //     var data = File.ReadAllBytes(cacheFilePath);
    //     Texture2D texture = new Texture2D(1, 1);
    //     texture.LoadImage(data, true);

    //     return Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    // }


    /*private float GetFreeSpace () {

        var availableSpace = 10000000; //

        return availableSpace;
    }*/
    /*
            public async UniTask<T> GetJson<T> (string file) {
                //check cache
                string json = GetTextFromCache (file);

                if (json == null) {
                    async UniTask<string> GetTextAsync (UnityWebRequest req) {
                        var op = await req.SendWebRequest ();
                        return op.downloadHandler.text;
                    }

                    UniTask task = GetTextAsync (UnityWebRequest.Get (BASE_URL + file));

                    var txt = (await UnityWebRequest.Get (BASE_URL + file).SendWebRequest ().ToUniTask ()).downloadHandler.text;
                }

                T data = JsonUtility.FromJson<T> (json);
                return data;
            }

            private static void CacheText (string fileName, string data) {
                var cacheFilePath = Path.Combine ("CachePath", fileName + ".text");
                File.WriteAllText (cacheFilePath, data);
            }
            private static void CacheTexture (string fileName, byte[] data) {
                var cacheFilePath = Path.Combine ("CachePath", fileName + ".texture");
                File.WriteAllBytes (cacheFilePath, data);
            }
            private static string GetTextFromCache (string fileName) {
                var cacheFilePath = Path.Combine ("sad", fileName + ".text");

                if (File.Exists (cacheFilePath)) {
                    return File.ReadAllText (cacheFilePath);
                }

                return null;
            }


    */
}

[Serializable]
public class LocalizationData
{
    public string Lang;
    public Dictionary<String, String> Dic;
    public int Version;
}
