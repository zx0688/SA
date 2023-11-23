using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class RequestVO
{
    public RequestVO(string method)
    {
        this.method = method;
    }
    public int rid;
    public string method;

    public Action callback;

}

public class NetworkService
{

    public event Action OnConnectionAvailable;

    private List<RequestVO> queue;

    private int rid;

    void Awake()
    {

        rid = 0;

        Recovery();

    }

    public async UniTask Init(IProgress<float> progress = default)
    {

        await UniTask.DelayFrame(2);

    }

    public void AddRequestToPool(RequestVO request)
    {
        request.rid = rid++;
        queue.Add(request);
        // SecurePlayerPrefs.SetString("requests", JsonUtility.ToJson(queue));
        SecurePlayerPrefs.Save();
    }

    public void SendRequests()
    {
        SecurePlayerPrefs.SetString("requests", "[]");
    }

    public bool IsAvailable
    {
        get { return Application.internetReachability != NetworkReachability.NotReachable; }
    }

    private void Recovery()
    {

        if (SecurePlayerPrefs.HasKey("requests"))
        {
            string json = SecurePlayerPrefs.GetString("requests", null);
            queue = JsonUtility.FromJson<List<RequestVO>>(json);
        }
        else
        {
            queue = new List<RequestVO>();
        }
    }

    void Start()
    {

        if (queue.Count > 0)
        {
            SendRequests();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}