using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameServer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Services : MonoBehaviour
{
    public enum State
    {
        INITED,
        INITIALIZATING
    }

    //Facade pattern
    public static PlayerService Player;
    public static MetaService Meta;
    public static AssetsService Assets;
    private static Services _instance;

    public static event Action OnInited;

    public static bool isInited => _instance is { _state: State.INITED };

    private State _state;

    [SerializeField] private Text loadText;
    [SerializeField] private Slider slider;
    [SerializeField] private RawImage background;

    [SerializeField] private Canvas canvas;

    void Awake()
    {
        //create Facade
        if (_instance == null)
        {
            _state = State.INITIALIZATING;

            _instance = this;

            Player = new PlayerService();
            Meta = new MetaService();
            Assets = new AssetsService();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    //PRELOADER
    void Start()
    {
        Application.targetFrameRate = 60;
        Application.backgroundLoadingPriority = ThreadPriority.Low;
      
        Init().Forget();
    }

    public async UniTaskVoid Init()
    {
        //create global time
        GameTime.Fix((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);

        //there should be await PlatformAdapter.Init (Google Play, AppStore)
        if (loadText != null) loadText.text = $"{PreLoadingText("RU")}...";
        await Assets.LoadLocalization(
                    "RU", // PlatformAdapter.GetLocale() 
                    Progress.Create<float>(x => UpdateProgressUI(x)));

        UpdateTextUI("Loading.Assets");
        await Assets.Init(Progress.Create<float>(x => UpdateProgressUI(x)));

        UpdateTextUI("Loading.Data");
        await Meta.Init(Progress.Create<float>(x => UpdateProgressUI(x)));

        await HttpBatchServer.Init(
                "3243", // PlatformAdapter.GetToken()
                "3434", // PlatformAdapter.GetUID()
                "gp", // PlatformAdapter.GetShortPlatformName()
                true, // false on realease build                
                Meta.Game,
                // correct global time
                serverTimestamp => GameTime.Fix(serverTimestamp));

        UpdateTextUI("Loading.Profile");
        await Player.Init(Progress.Create<float>(x => UpdateProgressUI(x)));

        TimeFormat.Init();

        UpdateTextUI("Loading icons");
        //Resources.LoadAll<Sprite>("Cards");
        //Resources.LoadAll<Sprite>("Items");
        UpdateTextUI("Loading.Scene");
        Scene s = SceneManager.GetActiveScene();

        if (s.name != "Main")
        {
            await SceneManager.LoadSceneAsync("Main").ToUniTask(Progress.Create<float>(x => UpdateProgressUI(x)));
        }

        // await UniTask.Delay(10);

        _state = State.INITED;
        OnInited?.Invoke();

        //await UniTask.Delay(1000);

        //DestroyImmediate(canvas.gameObject);
        //GC.Collect();

    }

    private void UpdateTextUI(string text)
    {
        if (loadText != null)
            loadText.text = $"{text.Localize(LocalizePartEnum.GUI)}...";
    }
    private void UpdateProgressUI(float progress)
    {
        if (slider != null)
            slider.value = progress;
    }

    private string PreLoadingText(string locale)
    {
        switch (locale)
        {
            case "RU":
                return "Загрузка локализации";
            case "EN":
                return "Loading language";
            default:
                return "Loading language";
        }
    }

}
