using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTextUpdater : MonoBehaviour
{
    public Slider slider;
    public TMP_Text sliderValueText;
    public TMP_Text traceTypeText;

    private bool sliderHasBeenMoved = false;

    private float initialValue;

    private SliderImageController imageController;

    void Start()
    {
        imageController = FindObjectOfType<SliderImageController>();

        if (imageController == null)
        {
            Debug.LogError("SliderImageController non trouvé dans la scène");
        }

        initialValue = slider.value;

        UpdateText(slider.value);

        slider.onValueChanged.AddListener(UpdateText);
        slider.onValueChanged.AddListener(CheckForTutorialProgress);
    }

    void UpdateText(float value)
    {
        sliderValueText.text = value.ToString("F0") + " nm";

        if (traceTypeText != null && imageController != null)
        {
            string traceType = imageController.GetCurrentTraceType();
            switch (traceType)
            {
                case "papillaires":
                    traceTypeText.text = "Traces Papillaires";
                    break;
                case "labiales":
                    traceTypeText.text = "Traces Labiales";
                    break;
                case "sang":
                    traceTypeText.text = "Traces de Sang";
                    break;
                case "pas":
                    traceTypeText.text = "Traces de Pas";
                    break;
                default:
                    traceTypeText.text = "Aucune trace détectée";
                    break;
            }
        }
    }

    void CheckForTutorialProgress(float value)
    {
        if (!sliderHasBeenMoved && Mathf.Abs(value - initialValue) > 5f)
        {
            sliderHasBeenMoved = true;

            if (TutorialManager.Instance != null)
            {
                if (TutorialManager.Instance.CurrentState <= TutorialManager.TutorialState.AjustementDuFiltre)
                {
                    TutorialManager.Instance.SetTutorialState(TutorialManager.TutorialState.AnalyseDesRésultats);
                }
            }
        }
    }

    public void ResetSliderMovedStatus()
    {
        sliderHasBeenMoved = false;
        initialValue = slider.value;
    }
}
