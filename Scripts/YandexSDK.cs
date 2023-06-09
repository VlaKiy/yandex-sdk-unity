﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class YandexSDK : MonoBehaviour
{
    public static YandexSDK instance;
    [DllImport("__Internal")]
    private static extern void GetUserData();
    [DllImport("__Internal")]
    private static extern void ShowFullscreenAd();
    /// <summary>
    /// Returns an int value which is sent to index.html
    /// </summary>
    /// <param name="placement"></param>
    /// <returns></returns>
    [DllImport("__Internal")]
    private static extern int ShowRewardedAd(string placement);
    [DllImport("__Internal")]
    private static extern void AuthenticateUser();
    [DllImport("__Internal")]
    private static extern void InitPurchases();
    [DllImport("__Internal")]
    private static extern void playerSetData(string data);
    [DllImport("__Internal")]
    private static extern void playerGetData();
    [DllImport("__Internal")]
    private static extern void playerSetLiderboard(string lbName, string value);
    [DllImport("__Internal")]
    private static extern void Purchase(string id);

    public UserData user;
    public UnityEvent onUserDataReceived = new UnityEvent();

    public UnityEvent onInterstitialShown = new UnityEvent();
    public StringEvent onInterstitialFailed = new StringEvent();
    /// <summary>
    /// Пользователь открыл рекламу
    /// </summary>
    public IntEvent onRewardedAdOpened = new IntEvent();
    /// <summary>
    /// Пользователь должен получить награду за просмотр рекламы
    /// </summary>
    public StringEvent onRewardedAdReward = new StringEvent();
    /// <summary>
    /// Пользователь закрыл рекламу
    /// </summary>
    public IntEvent onRewardedAdClosed = new IntEvent();
    /// <summary>
    /// Вызов/просмотр рекламы повлёк за собой ошибку
    /// </summary>
    public StringEvent onRewardedAdError = new StringEvent();
    /// <summary>
    /// Покупка успешно совершена
    /// </summary>
    public StringEvent onPurchaseSuccess = new StringEvent();
    /// <summary>
    /// Покупка не удалась: в консоли разработчика не добавлен товар с таким id,
    /// пользователь не авторизовался, передумал и закрыл окно оплаты,
    /// истекло отведенное на покупку время, не хватило денег и т. д.
    /// </summary>
    public StringEvent onPurchaseFailed = new StringEvent();

    public DataHolderEvent onPlayerGetData = new DataHolderEvent();

    public UnityEvent onPlayerSetLiderboard = new UnityEvent();

    public UnityEvent onClose = new UnityEvent();

    public Queue<int> rewardedAdPlacementsAsInt = new Queue<int>();
    public Queue<string> rewardedAdsPlacements = new Queue<string>();

    public GameObject _rewardedTestPanel;

    private Transform _canvasTransform;
    private string _dataPath = "data.json";

    private void Start()
    {
        _canvasTransform = GameObject.Find("Canvas").transform;
        GetData();
    }

    [System.Serializable]
    public class DataHolder
    {
        public int _coin = 0;
    }

    public DataHolder _dataHolder;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _dataHolder = new DataHolder();
    }

    /// <summary>
    /// Call this to ask user to authenticate
    /// </summary>
    public void Authenticate()
    {
        AuthenticateUser();
    }

    /// <summary>
    /// Call this to set data
    /// </summary>
    public void SetData()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            var data = JsonUtility.ToJson(_dataHolder);

            playerSetData(data);
            print(data);
#endif
#if UNITY_EDITOR
            SetJSONData();
            print("Set data request will be send");
#endif
    }

    private void SetJSONData()
    {
        var data = JsonUtility.ToJson(_dataHolder);

        File.WriteAllText(_dataPath, data);
        Debug.Log(File.ReadAllText(_dataPath));
    }


    /// <summary>
    /// Call this to get data
    /// </summary>
    public void GetData()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            playerGetData();
#endif
#if UNITY_EDITOR
            PlayerGetData(File.ReadAllText(_dataPath));
            print("Get data request will be send");
#endif
    }

    private void PlayerGetData(string data)
    {
        _dataHolder = JsonUtility.FromJson<DataHolder>(data);
        onPlayerGetData?.Invoke(_dataHolder);
    }

    /// <summary>
    /// Call this to set liderboard
    /// </summary>
    public void SetLiderboard(string lbName, int value)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            Authenticate();
            playerSetLiderboard(lbName, value.ToString());
            onPlayerSetLiderboard?.Invoke();
#endif
#if UNITY_EDITOR
            print("Set Liderboard request will be send");
            onPlayerSetLiderboard?.Invoke();
#endif
    }

    /// <summary>
    /// Call this to show interstitial ad. Don't call frequently. There is a 3 minute delay after each show.
    /// </summary>
    public void ShowInterstitial()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            ShowFullscreenAd();
#endif
#if UNITY_EDITOR
            print("Interstitial AD showed");
#endif
    }

    /// <summary>
    /// Call this to show rewarded ad
    /// </summary>
    /// <param name="placement"></param>
    public void ShowRewarded(string placement)
    {
#if !UNITY_EDITOR && UNITY_WEBGL
            rewardedAdPlacementsAsInt.Enqueue(ShowRewardedAd(placement));
            rewardedAdsPlacements.Enqueue(placement);
#endif
#if UNITY_EDITOR
            rewardedAdPlacementsAsInt.Enqueue(ShowTestRewardedAd(placement));
            rewardedAdsPlacements.Enqueue(placement);
#endif
    }

    private int ShowTestRewardedAd(string placement)
    {
        var panel = Instantiate(_rewardedTestPanel, GameObject.Find("Canvas").transform);
        panel.SetActive(true);
        return 1;
    }

    /// <summary>
    /// Call this to receive user data
    /// </summary>
    public void RequestUserData()
    {
        GetUserData();
    }

    public void InitializePurchases()
    {
        InitPurchases();
    }

    public void ProcessPurchase(string id)
    {
        Purchase(id);
    }

    public void StoreUserData(string data)
    {
        user = JsonUtility.FromJson<UserData>(data);
        onUserDataReceived?.Invoke();
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    public void OnInterstitialShown()
    {
        onInterstitialShown?.Invoke();
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="error"></param>
    public void OnInterstitialError(string error)
    {
        onInterstitialFailed?.Invoke(error);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewardedOpen(int placement)
    {
        onRewardedAdOpened?.Invoke(placement);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewarded(int placement)
    {
        if (placement == rewardedAdPlacementsAsInt.Dequeue())
        {
            onRewardedAdReward?.Invoke(rewardedAdsPlacements.Dequeue());
        }
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewardedClose(int placement)
    {
        onRewardedAdClosed?.Invoke(placement);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="placement"></param>
    public void OnRewardedError(string placement)
    {
        onRewardedAdError?.Invoke(placement);
        rewardedAdsPlacements.Clear();
        rewardedAdPlacementsAsInt.Clear();
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="id"></param>
    public void OnPurchaseSuccess(string id)
    {
        onPurchaseSuccess?.Invoke(id);
    }

    /// <summary>
    /// Callback from index.html
    /// </summary>
    /// <param name="error"></param>
    public void OnPurchaseFailed(string error)
    {
        onPurchaseFailed?.Invoke(error);
    }

    /// <summary>
    /// Browser tab has been closed
    /// </summary>
    /// <param name="error"></param>
    public void OnClose()
    {
        onClose?.Invoke();
    }
}

public struct UserData
{
    public string id;
    public string name;
    public string avatarUrlSmall;
    public string avatarUrlMedium;
    public string avatarUrlLarge;
}

[Serializable]
public class StringEvent : UnityEvent<string> { }

[Serializable]
public class IntEvent : UnityEvent<int> { }

[Serializable]
public class DataHolderEvent : UnityEvent<YandexSDK.DataHolder> { }
