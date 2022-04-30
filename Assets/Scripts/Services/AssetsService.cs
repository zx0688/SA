using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;
using UnityEngine.UI;


public class AssetsService
{
    private static readonly string BASE_URL = "";

    private int saveCount;

    void Awake()
    {

    }

    //private Dictionary<string, Sprite> spriteCache;

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
        await UniTask.Yield();
    }



    public async UniTask<PlayerVO> GetProfile(IProgress<float> progress = null)
    {
        string json = SecurePlayerPrefs.GetString("profile") as string;
        return null;//json == "" ? null : JsonUtility.FromJson<PlayerVO>(json);
    }
    public async UniTask<bool> SetProfile(object data, IProgress<float> progress = null)
    {
        //SecurePlayerPrefs.SetString("profile", JsonUtility.ToJson(data));

        saveCount--;
        if (saveCount <= 0)
        {
            //  SecurePlayerPrefs.Save();
            //await 
            saveCount = 10;
            PlayerPrefs.SetInt("savecount", 10);
        }

        return true;
    }

    public async UniTask<string> GetJson(string name, bool fromResources, string url, bool usecache, IProgress<float> progress = null)
    {

        if (fromResources)
        {
            var (isCanceled, asset) = await Resources.LoadAsync<TextAsset>(name).ToUniTask(progress: progress).SuppressCancellationThrow();
            if (asset != null && isCanceled == false)
            {
                return (asset as TextAsset).text;
            }
        }

        if (usecache == true)
        {
            string fromFile = GetJsonFromCache(name);
            if (fromFile != null && fromFile.Length > 0)
            {
                return fromFile;
            }
        }

        if (usecache == false)
            url += "&" + GameTime.Current;
        Debug.Log(url);

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest().ToUniTask(progress: progress);

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogWarning("Cant find " + name);
            return null;
        }

        string jsonFromUrl = request.downloadHandler.text;
        return jsonFromUrl;
    }

    public async UniTaskVoid SetSpriteIntoImage(Image icon, string name, bool fromResources, IProgress<float> progress = null)
    {
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

        Sprite sprite = GetSpriteFromCache(name);
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
        CacheTexture(name, request.downloadHandler.data);

        sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        return sprite;
    }
    private void CacheTexture(string name, byte[] data)
    {
        var cacheFilePath = Path.Combine("CachePath", name + ".texture");
        File.WriteAllBytes(cacheFilePath, data);
    }
    private void CacheJson(string name, string json)
    {
        var cacheFilePath = Path.Combine("CachePath", name + ".json");
        File.WriteAllText(cacheFilePath, json);
    }

    private string GetJsonFromCache(string name)
    {
        var cacheFilePath = name; //Path.Combine ("", name + ".json");
        if (!File.Exists(cacheFilePath))
            return null;
        return null; //File.ReadAllText (cacheFilePath);
    }
    private Sprite GetSpriteFromCache(string name)
    {
        var cacheFilePath = Path.Combine("", name + ".texture");
        if (!File.Exists(cacheFilePath))
            return null;

        var data = File.ReadAllBytes(cacheFilePath);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(data, true);

        return Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    /*private float GetFreeSpace () {

        var availableSpace = 10000000; //SimpleDiskUtils.DiskUtils.CheckAvailableSpace ();

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
