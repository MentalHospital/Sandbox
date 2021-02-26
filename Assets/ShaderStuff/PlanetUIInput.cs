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
    [SerializeField] private Slider widthSlider;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider depthSlider;
    [SerializeField] private Slider minMassSlider;
    [SerializeField] private Slider maxMassSlider;
    [SerializeField] private Slider minStartSpeedSlider;
    [SerializeField] private Slider maxStartSpeedSlider;
    [SerializeField] private TMP_InputField gInputField;
    [Space]
    [SerializeField] private TextMeshProUGUI widthTextMesh;
    [SerializeField] private TextMeshProUGUI heightTextMesh;
    [SerializeField] private TextMeshProUGUI depthTextMesh;
    [SerializeField] private TextMeshProUGUI minMassMesh;
    [SerializeField] private TextMeshProUGUI maxMassMesh;
    [SerializeField] private TextMeshProUGUI minStartSpeedMesh;
    [SerializeField] private TextMeshProUGUI maxStartSpeedMesh;
    [SerializeField] private TextMeshProUGUI gMesh;

    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int Depth { get { return depth; } }
    public int MinMass { get { return minMass; } }
    public int MaxMass { get { return maxMass; } }
    public int MinStartSpeed { get { return minStartSpeed; } }
    public int MaxStartSpeed { get { return maxStartSpeed; } }
    public float G { get { return g; } }

    public event Action OnGenerateButtonClick;
    public event Action OnClearButtonClick;
    public event Action<bool> OnSphericalToggleClick;

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
        SubscribeAllSliders();
        UpdateAllSliderValues();
        UpdateAllSliderTexts();
        generateButton.onClick.AddListener(() => { OnGenerateButtonClick.Invoke(); });
        sphericalToggle.onValueChanged.AddListener((bool b) => { OnSphericalToggleClick.Invoke(b); });
        clearButton.onClick.AddListener(() => { OnClearButtonClick.Invoke(); });
    }

    private void SubscribeAllSliders()
    {
        widthSlider.onValueChanged.AddListener(delegate { UpdateSliderText(widthSlider, widthTextMesh); });
        heightSlider.onValueChanged.AddListener(delegate { UpdateSliderText(heightSlider, heightTextMesh); });
        depthSlider.onValueChanged.AddListener(delegate { UpdateSliderText(depthSlider, depthTextMesh); });
        minMassSlider.onValueChanged.AddListener(delegate { UpdateSliderText(minMassSlider, minMassMesh); });
        minMassSlider.onValueChanged.AddListener(
            delegate
            {
                maxMassSlider.minValue = minMassSlider.value;
                UpdateValue(maxMassSlider, ref maxMass);
                UpdateSliderText(maxMassSlider, maxMassMesh);
            }
            );
        maxMassSlider.onValueChanged.AddListener(delegate { UpdateSliderText(maxMassSlider, maxMassMesh); });
        minStartSpeedSlider.onValueChanged.AddListener(delegate { UpdateSliderText(minStartSpeedSlider, minStartSpeedMesh); });
        maxStartSpeedSlider.onValueChanged.AddListener(delegate { UpdateSliderText(maxStartSpeedSlider, maxStartSpeedMesh); });
        gInputField.onEndEdit.AddListener(delegate { UpdateInputFieldText(gInputField, gMesh); });

        widthSlider.onValueChanged.AddListener(delegate { UpdateValue(widthSlider, ref width); });
        heightSlider.onValueChanged.AddListener(delegate { UpdateValue(heightSlider, ref height); });
        depthSlider.onValueChanged.AddListener(delegate { UpdateValue(depthSlider, ref depth); });
        minMassSlider.onValueChanged.AddListener(delegate { UpdateValue(minMassSlider, ref minMass); });
        maxMassSlider.onValueChanged.AddListener(delegate { UpdateValue(maxMassSlider, ref maxMass); });
        minStartSpeedSlider.onValueChanged.AddListener(delegate { UpdateValue(minStartSpeedSlider, ref minStartSpeed); });
        maxStartSpeedSlider.onValueChanged.AddListener(delegate { UpdateValue(maxStartSpeedSlider, ref maxStartSpeed); });
        gInputField.onEndEdit.AddListener(delegate { UpdateValue(gInputField, ref g); });
    }
    
    private void UpdateAllSliderTexts()
    {
        UpdateSliderText(widthSlider, widthTextMesh);
        UpdateSliderText(heightSlider, heightTextMesh);
        UpdateSliderText(depthSlider, depthTextMesh);
        UpdateSliderText(minMassSlider, minMassMesh);
        UpdateSliderText(maxMassSlider, maxMassMesh);
        UpdateSliderText(minStartSpeedSlider, minStartSpeedMesh);
        UpdateSliderText(maxStartSpeedSlider, maxStartSpeedMesh);
        UpdateInputFieldText(gInputField, gMesh);
    }
    private void UpdateAllSliderValues()
    {
        UpdateValue(widthSlider, ref width);
        UpdateValue(heightSlider, ref height);
        UpdateValue(depthSlider, ref depth);
        UpdateValue(minMassSlider, ref minMass);
        UpdateValue(maxMassSlider, ref maxMass);
        UpdateValue(minStartSpeedSlider, ref minStartSpeed);
        UpdateValue(maxStartSpeedSlider, ref maxStartSpeed);
        UpdateValue(gInputField, ref g);
    }

    private void UpdateValue(Slider slider, ref int value)
    {
        value = Mathf.Clamp((int)slider.value, (int) slider.minValue, (int) slider.maxValue);
    }
    private void UpdateValue(TMP_InputField inputField, ref float value)
    {
        value = float.Parse(inputField.text, CultureInfo.InvariantCulture.NumberFormat);
    }

    private void UpdateSliderText(Slider slider, TextMeshProUGUI sliderText)
    {
        sliderText.text = ((int)slider.value).ToString();
    }
    private void UpdateInputFieldText(TMP_InputField inputField, TextMeshProUGUI sliderText)
    {
        sliderText.text = (float.Parse(inputField.text, CultureInfo.InvariantCulture.NumberFormat)).ToString("F1", CultureInfo.InvariantCulture);
    }
}