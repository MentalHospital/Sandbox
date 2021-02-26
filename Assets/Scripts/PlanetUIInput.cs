using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class PlanetUIInput : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [Space]
    [SerializeField] private Button generateButton;
    [SerializeField] private Button clearButton;
    [Space]
    [SerializeField] private Toggle sphericalToggle;
    [Space]
    [SerializeField] private SliderIntInputTMP widthSlider;
    [SerializeField] private SliderIntInputTMP heightSlider;
    [SerializeField] private SliderIntInputTMP depthSlider;
    [SerializeField] private SliderIntInputTMP minMassSlider;
    [SerializeField] private SliderIntInputTMP maxMassSlider;
    [SerializeField] private SliderIntInputTMP minStartSpeedSlider;
    [SerializeField] private SliderIntInputTMP maxStartSpeedSlider;
    [SerializeField] private TMP_InputField gInputField;
    [Space]
    [SerializeField] private TextMeshProUGUI minMassMesh;
    [SerializeField] private TextMeshProUGUI maxMassMesh;
    [SerializeField] private TextMeshProUGUI gMesh;

    public event Action OnGenerateButtonClick;
    public event Action OnClearButtonClick;
    public event Action<bool> OnSphericalToggleClick;

    public int Width { get { return widthSlider.value; } }
    public int Height { get { return heightSlider.value; } }
    public int Depth { get { return depthSlider.value; } }
    public int MinMass { get { return minMassSlider.value; } }
    public int MaxMass { get { return maxMassSlider.value; } }
    public int MinStartSpeed { get { return minStartSpeedSlider.value; } }
    public int MaxStartSpeed { get { return maxStartSpeedSlider.value; } }
    public float G { get { return g; } }

    private int width = 1;
    private int height = 1;
    private int depth = 1;
    private int minMass = 5;
    private int maxMass = 50;
    private int minStartSpeed = 5;
    private int maxStartSpeed = 10;
    private float g = .1f;

    private void Start()
    {
        SubscribeNonSliderInputs();
        UpdateAllValues();
        UpdateAllTexts();
        generateButton.onClick.AddListener(() => { OnGenerateButtonClick.Invoke(); });
        sphericalToggle.onValueChanged.AddListener((bool b) => { OnSphericalToggleClick.Invoke(b); });
        clearButton.onClick.AddListener(() => { OnClearButtonClick.Invoke(); });
    }

    private void OnSliderUpdate(SliderIntInputTMP sliderIntInput , ref int value)
    {
        value = sliderIntInput.value;
    }

    private void SubscribeNonSliderInputs()
    {
        minMassSlider.OnValueChanged += () =>
            {
                maxMassSlider.slider.minValue = minMassSlider.value;
                maxMassSlider.Refresh();
            };
        
        gInputField.onEndEdit.AddListener(delegate { UpdateInputFieldText(gInputField, gMesh); });
        gInputField.onEndEdit.AddListener(delegate { UpdateInputFieldValue(gInputField, ref g); });
    }
    
    private void UpdateAllTexts()
    {
        UpdateInputFieldText(gInputField, gMesh);
    }

    private void UpdateAllValues()
    {
        widthSlider.SetValue(width);
        heightSlider.SetValue(height);
        depthSlider.SetValue(depth);
        minMassSlider.SetValue(minMass);
        maxMassSlider.SetValue(maxMass);
        minStartSpeedSlider.SetValue(minStartSpeed);
        maxStartSpeedSlider.SetValue(maxStartSpeed);
        UpdateInputFieldValue(gInputField, ref g);
    }

    private void UpdateInputFieldValue(TMP_InputField inputField, ref float value)
    {
        value = float.Parse(inputField.text, CultureInfo.InvariantCulture.NumberFormat);
    }

    private void UpdateInputFieldText(TMP_InputField inputField, TextMeshProUGUI sliderText)
    {
        sliderText.text = (float.Parse(inputField.text, CultureInfo.InvariantCulture.NumberFormat)).ToString("F1", CultureInfo.InvariantCulture);
    }
}