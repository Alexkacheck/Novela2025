using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillTimer : MonoBehaviour
{
    [SerializeField, Required]
    private Image _timerFillImage;

    [SerializeField, Required]
    private TextMeshProUGUI _timerText;

    private Sequence _sequence;

    public void InitTimer(int time, Action onExpired)
    {
        Cleanup();

        // No need to sync, can use UtcNow + time for this project
        DateTime current = DateTime.UtcNow;
        DateTime expiration = current.AddSeconds(time);

        // Get the total time as the duration
        float totalTime = (float)time;

        void OnTimerStart()
        {
            _timerText.text = time.ToString();
        }

        void OnTimerUpdate()
        {
            DateTime current = DateTime.UtcNow;

            if (current < expiration)
            {
                TimeSpan leftTime = expiration - current;
                _timerText.text = Mathf.CeilToInt((float)leftTime.TotalSeconds).ToString();

                float remainingTime = (float)leftTime.TotalSeconds;
                _timerFillImage.fillAmount = remainingTime / totalTime;
            }
        }

        void OnTimerExpired()
        {
            onExpired?.Invoke();
        }

        _sequence = DOTween.Sequence()
            .OnStart(OnTimerStart)
            .AppendInterval(totalTime)
            .OnUpdate(OnTimerUpdate)
            .OnComplete(OnTimerExpired);
    }

    public void Cleanup()
    {
        _timerFillImage.fillAmount = 1f;
        _timerText.text = "";

        _sequence?.Kill(complete: false);
    }
}