using UnityEngine;
using UnityEngine.UI;

public class ToggleSliderControl : MonoBehaviour
{
    public Button toggleButton;
    public Image toggleImage;
    public Sprite onSprite;
    public Sprite offSprite;

    public Slider controlledSlider;
    public SliderImageController imageController;
    public GameObject uvFilterPanel;
    public GameObject scanImage;

    private bool isOn = false;

    void Start()
    {
        toggleButton.onClick.AddListener(ToggleState);
        UpdateUI();
    }

    void ToggleState()
    {
        isOn = !isOn;
        UpdateUI();

        if (isOn)
        {
            if (TutorialManager.Instance.GetCurrentMessageIndex() < 3)
            {
                TutorialManager.Instance.SetTutorialState(TutorialManager.TutorialState.AjustementDuFiltre);
            }
        }
    }

    void UpdateUI()
    {
        toggleImage.sprite = isOn ? onSprite : offSprite;
        controlledSlider.interactable = isOn;

        if (isOn)
        {
            imageController.UpdateImages(controlledSlider.value);
        }
        else
        {
            imageController.HideAllImages();
        }

        uvFilterPanel.SetActive(isOn);
        scanImage.SetActive(isOn);
    }
}
