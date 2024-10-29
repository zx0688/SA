using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEditor;
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

    public LocalizationData GetLocalizationData() => LocalDic;

    public string Localize(string key, LocalizePartEnum part) => part switch
    {
        LocalizePartEnum.GUI => LocalDic.GUI.TryGetValue(key, out string res) ? res : $"[{key}]",
        LocalizePartEnum.CardDescription => LocalDic.CardDescription.TryGetValue(key, out string res) ? res : key,
        LocalizePartEnum.CardAction => LocalDic.ActionText.TryGetValue(key, out string res) ? res : key,
        LocalizePartEnum.CardName => LocalDic.CardName.TryGetValue(key, out string res) ? res : key,//$"[{key}]",
        //LocalizePartEnum.ChoiceText => LocalDic.ChoiceText.TryGetValue(key, out string res) ? res : key,//$"[{key}]",
        _ => $"[{key}]"
    };

    private LocalizationData LocalDic;

    private static readonly string BASE_URL = "";

    private int saveCount;


    //private Dictionary<string, Sprite> spriteCache;

    public async UniTask LoadLocalization(string lang, IProgress<float> progress = null)
    {
        string json = await GetJson($"localization_{lang}", GOOGLE_DRIVE_LOCALIZATION, progress, LoadContentOption.UseFileVersion);
        Debug.Log(json);

        await UniTask.SwitchToThreadPool();
        LocalDic = JSON.Deserialize<LocalizationData>(json);
        await UniTask.SwitchToMainThread();
    }

    public async UniTask Init(IProgress<float> progress = null)
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

    }

    public async UniTask<string> GetJson(string name, string url, IProgress<float> progress = null, LoadContentOption option = LoadContentOption.KeepInResources)
    {

        option = LoadContentOption.DoNotSave;

        if (option == LoadContentOption.KeepInResources)
        {
            var (isCanceled, asset) = await Resources.LoadAsync<TextAsset>(name).ToUniTask(progress: progress).SuppressCancellationThrow();
            if (asset != null && isCanceled == false)
                return (asset as TextAsset).text;
        }

        string namev = null;
        string version = null;
        if (option == LoadContentOption.UseFileVersion)
        {
            namev = ZString.Format($"{0}_version", name);
            version = "3434";//await GetJson(namev, "", progress, LoadContentOption.DoNotSave);
            if (version == PlayerPrefs.GetString(namev))
            {
                var cacheFilePath = Path.Combine(Application.persistentDataPath, ZString.Format("{0}.json", name));
                if (File.Exists(cacheFilePath))
                {
                    if (TryGetJson(name, out string fromFile))
                        return fromFile;
                    else
                        Debug.LogWarning($"can't parse data {name} from cache.. trying to download from {url}");
                }
                else
                {
                    Debug.Log($"file {name} doesn't exist.. trying to download from {url}");
                }
            }
        }

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            try
            {
                await request.SendWebRequest().ToUniTask(progress: progress).Timeout(TimeSpan.FromSeconds(20));
            }
            catch (Exception)
            {
                OnBadConnection?.Invoke();
            }

            // if (request.result != UnityWebRequest.Result.Success)
            // {
            //     request.Abort();
            //     OnBadConnection?.Invoke();
            //     Debug.LogWarning($"can't connect to {url}.. try again in 3 sec");
            //     await UniTask.Delay(3000);
            //     await request.SendWebRequest().ToUniTask(progress: progress).Timeout(TimeSpan.FromSeconds(20));
            // }
            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    if (option == LoadContentOption.KeepInFile)
                    {
                        Debug.LogWarning($"can't download data {name} from {url}.. trying to get from cache");
                        if (TryGetJson(name, out string fromFile))
                            return fromFile;
                    }
                    Debug.LogWarning($"can't download data {name} from cache.. hope this won't invoke an issue");
                    break;
                case UnityWebRequest.Result.Success:
                    string json = request.downloadHandler.text;
                    if (option != LoadContentOption.DoNotSave)
                    {
                        //CheckFreeSpace ()
                        if (TrySaveJson(name, json) && option == LoadContentOption.UseFileVersion)
                        {
                            PlayerPrefs.SetString(namev, version);
                        }
                    }
                    return json;
            }
            throw new Exception($"Http request RESULT:{request.result.ToString()} CODE:{request.responseCode} name:{name} url:{url}");
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

    public async UniTaskVoid SetSpriteIntoImage(Image icon, string name, bool fromResources, IProgress<float> progress = null, Action callback = null)
    {
        //Debug.Log($"load:{name}");
        icon.sprite = await Services.Assets.GetSprite(name, fromResources, progress);
        callback?.Invoke();
    }

    public async UniTaskVoid PlaySound(string name, AudioSource source)
    {
        var (isCanceled, asset) = await Resources.LoadAsync<AudioClip>(ZString.Format("Sound/{0}", name)).ToUniTask().SuppressCancellationThrow();
        if (asset != null && isCanceled == false)
            source.PlayOneShot(asset as AudioClip);
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
    private bool TrySaveJson(string name, string json)
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

    private bool TryGetJson(string name, out string json)
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


    /*private float  GetFreeSpace (){

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

public enum LoadContentOption
{
    DoNotSave,
    KeepInResources,
    UseFileVersion,
    KeepInFile
}

public enum LocalizePartEnum
{
    GUI, CardName, CardDescription, CardAction
}

[Serializable]
public class LocalizationData
{
    public string Lang;
    public Dictionary<String, String> GUI;
    public Dictionary<String, String> CardName;
    public Dictionary<String, String> CardDescription;
    public Dictionary<String, String> ActionText;

    public int Version;
}
