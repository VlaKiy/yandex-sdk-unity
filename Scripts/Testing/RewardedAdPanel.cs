using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RewardedAdPanel : MonoBehaviour
{
    [SerializeField] private int _timeWait = 3;
    private float _timeSeconds = 1f;
    private WaitForSeconds _delay;

    [SerializeField] private Text _countTimeUI;

    [SerializeField] private GameObject _exitObject;

    private YandexSDK _sdk;

    private void Start()
    {
        _exitObject.SetActive(false);

        _sdk = YandexSDK.instance;
        RewardedADOpen();
        _delay = new WaitForSeconds(_timeSeconds);
        StartCoroutine(MakeWaiter());
    }

    public void Close()
    {
        Debug.Log("Reward AD Close");
        _sdk.OnRewardedClose(1);
        Destroy(gameObject);
    }

    public void RewardedADOpen()
    {
        Debug.Log("Reward AD Open");
        _sdk.OnRewardedOpen(1);
    }

    public void RewardedADReward(int playcment)
    {
        Debug.Log("Reward AD Rewarded");
        _sdk.OnRewarded(playcment);
    }

    IEnumerator MakeWaiter()
    {
        for (int i = _timeWait; i >= 0; i--)
        {
            yield return _delay;

            _countTimeUI.text = $"{i}";

            if (i == 0)
            {
                RewardedADReward(1);
                _exitObject.SetActive(true);
            }
        }
    }

}
