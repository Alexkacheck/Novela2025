using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public sealed class ChoiceButton : MonoBehaviour
{
    [SerializeField, Required]
    private TextMeshProUGUI _choiceText;

    [SerializeField, Required]
    private Button _button;

    [SerializeField, Required]
    private FillTimer _timer;

    private int _choiseId;
    private Action<int> _selectCallback;

    [Button, DisableInEditorMode]
    public void DebugInit(int num, string phrase, int time, bool auto) => Init(num, phrase, (num) => { Debug.Log($"auto {num}"); }, time, auto);

    public void Init(int choiseId, string choicePhrase, Action<int> selectCallback, int time = 0, bool autoSelect = false)
    {
        Cleanup();

        _choiseId = choiseId;
        _selectCallback = selectCallback;
        _choiceText.text = choicePhrase;

        _button.onClick.AddListener(RaiseClick);

        if (time > 0)
        {
            ToggleTimer(true);

            _timer.InitTimer(time, onExpired: () =>
            {
                DisableButton();

                if (autoSelect)
                {
                    RaiseClick();
                }
            });
        }
    }

    private void DisableButton()
    {
        _button.interactable = false;
        _timer.Cleanup();
        ToggleTimer(false);
    }

    private void ToggleTimer(bool active)
    {
        _timer.gameObject.SetActive(active);
    }

    private void RaiseClick()
    {
        DisableButton();
        _selectCallback?.Invoke(_choiseId);
    }

    public void Cleanup()
    {
        _timer.Cleanup();
        ToggleTimer(false);
        _button.onClick.RemoveAllListeners();
        _button.interactable = true;
    }

    private void OnDisable()
    {
        Cleanup();
    }
}