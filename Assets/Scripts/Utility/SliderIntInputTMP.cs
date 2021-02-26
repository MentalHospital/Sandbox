using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(Slider))]

public class SliderIntInputTMP : MonoBehaviour, IInputUI
{
    [SerializeField] private TextMeshProUGUI sliderValueMesh;

    public Slider slider;

    public int value { get; private set; }

    UnityAction<float> reaction;
    public event Action OnValueChanged;
    
    void Start()
    {
        SubscribeToValueChange();
    }

    public void AddValueChangedListener(Action delegateToAdd)
    {
        OnValueChanged += delegateToAdd;
    }

    public void SubscribeToValueChange()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        reaction = delegate
        {
            UpdateValueFromSlider();
            UpdateText();
            OnValueChanged?.Invoke();
        };
            
        slider.onValueChanged.AddListener(reaction);
    }

    public void UnsubscribeFromValueChange()
    {
        slider.onValueChanged.RemoveListener(reaction);
    }

    private void UpdateValueFromSlider()
    {
        this.value = Mathf.Clamp((int)slider.value, (int)slider.minValue, (int)slider.maxValue);
    }
    public void SetValue(int value)
    {
        this.value = Mathf.Clamp(value, (int)slider.minValue, (int)slider.maxValue);
        UpdateText();
    }

    public void Refresh()
    {
        slider.value = Mathf.Clamp(slider.value, (int)slider.minValue, (int)slider.maxValue);
        UpdateText();
    }

    private void UpdateText()
    {
        sliderValueMesh.text = value.ToString();
    }
}
