using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleLocalization;
using Cysharp.Threading.Tasks;
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

    public static PlayerService Player;
    public static DataService Data;
    public static NetworkService Network;
    public static AssetsService Assets;
    private static Services _instance;

    public static event Action OnInited;


    public static bool isInited => _instance is { _state: State.INITED };

    private State _state;

    [SerializeField] private Text loadText;
    [SerializeField] private Slider slider;

    void Awake()
    {
        _state = State.INITIALIZATING;

        if (_instance == null)
        {
            _instance = this;

            Player = new PlayerService();
            Data = new DataService();
            Network = new NetworkService();
            Assets = new AssetsService();

            DontDestroyOnLoad(gameObject);

        }
        else
        {
            DestroyImmediate(gameObject);
        }

    }
    void Start()
    {

        Application.targetFrameRate = 45;

        Init().Forget();
    }

    public async UniTaskVoid Init()
    {

        GameTime.Fix((int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);

        UpdateTextUI("Loading assets...");
        await Assets.Init(Progress.Create<float>(x => UpdateProgressUI(x)));

        //UpdateTextUI("Loading network...");
        //await network.Init(Progress.Create<float>(x => UpdateProgressUI(x)));
        HttpBatchServer.Init(
            "3243",
            "3434",
            "gp",
            serverTimestamp => GameTime.Fix(serverTimestamp),
            GameTime.Get);

        //UpdateTextUI("Loading game data...");
        //await HttpBatchServer.Profile<PlayerVO>();

        //HttpBatchServer.OpenConnection().Forget();

        UpdateTextUI("Loading game data...");
        await Data.Init(Progress.Create<float>(x => UpdateProgressUI(x)));

        UpdateTextUI("Loading profile...");
        await Player.Init(Progress.Create<float>(x => UpdateProgressUI(x)));


        UpdateTextUI("Loading scene...");
        Scene s = SceneManager.GetActiveScene();
        if (s.name != "Main")
        {
            await SceneManager.LoadSceneAsync("Main").ToUniTask(Progress.Create<float>(x => UpdateProgressUI(x)));
        }

        LocalizationManager.Read();
        TimeFormat.Init();

        UpdateProgressUI(1);

        await UniTask.DelayFrame(2);

        _state = State.INITED;
        OnInited?.Invoke();
    }

    private void UpdateTextUI(string text)
    {
        if (loadText != null)
            loadText.text = text;
    }
    private void UpdateProgressUI(float progress)
    {
        if (slider != null)
            slider.value = progress;
    }

}
