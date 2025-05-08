using UnityEngine;
using UnityEngine.UI;

public class SliderImageController : MonoBehaviour
{
    public Slider slider;

    [Header("Traces Papillaires (254-365 nm)")]
    public GameObject[] tracesPapillaires;
    public Button powderButton;
    [Tooltip("Jouer un effet sonore quand on applique la poudre")]
    public AudioSource powderSound;
    [Tooltip("Effet de particule quand on applique la poudre")]
    public ParticleSystem powderEffect;

    [Header("Traces Labiales (365-415 nm)")]
    public GameObject[] tracesLabiales;

    [Header("Traces de Sang (415-485 nm)")]
    public GameObject[] tracesDeSang;

    [Header("Traces de Pas (485-530 nm)")]
    public GameObject[] tracesDePas;

    [Header("Paramètres")]
    [Tooltip("Activer le mode debug qui affiche les changements de plage dans la console")]
    public bool debugMode = false;

    private string currentActiveRange = "";

    private bool powderApplied = false;
    private bool inPapillaireRange = false;

    private void Start()
    {
        slider.minValue = 200;
        slider.maxValue = 530;

        SetActiveArray(tracesPapillaires, false);

        if (powderButton != null)
        {
            powderButton.onClick.AddListener(ApplyPowder);
            powderButton.gameObject.SetActive(false);
        }

        UpdateImages(slider.value);

        slider.onValueChanged.AddListener(delegate { UpdateImages(slider.value); });
    }

    void ApplyPowder()
    {
        powderApplied = true;

        if (powderSound != null)
        {
            powderSound.Play();
        }

        if (powderEffect != null)
        {
            powderEffect.Play();
        }

        UpdatePapillairesVisibility();
        UpdatePowderButtonVisibility();

        if (TutorialManager.Instance != null)
        {
            if (TutorialManager.Instance.CurrentState == TutorialManager.TutorialState.ActivationDuFiltre ||
                TutorialManager.Instance.CurrentState == TutorialManager.TutorialState.AjustementDuFiltre)
            {
                TutorialManager.Instance.SetTutorialState(TutorialManager.TutorialState.AnalyseDesRésultats);
            }
        }
    }

    public void UpdateImages(float wavelength)
    {
        bool wasInPapillaireRange = inPapillaireRange;
        inPapillaireRange = (wavelength >= 254 && wavelength < 365);

        if (wasInPapillaireRange != inPapillaireRange)
        {
            UpdatePowderButtonVisibility();
        }

        string activeRange = "";
        if (wavelength >= 254 && wavelength < 365)
        {
            activeRange = "papillaires";
        }
        else if (wavelength >= 365 && wavelength < 415)
        {
            activeRange = "labiales";
        }
        else if (wavelength >= 415 && wavelength < 485)
        {
            activeRange = "sang";
        }
        else if (wavelength >= 485 && wavelength <= 530)
        {
            activeRange = "pas";
        }

        if (activeRange != currentActiveRange)
        {
            if (debugMode)
            {
                Debug.Log($"Changement de plage: {currentActiveRange} -> {activeRange} à {wavelength} nm");
            }

            HideAllImages();

            switch (activeRange)
            {
                case "papillaires":
                    UpdatePapillairesVisibility();
                    break;
                case "labiales":
                    SetActiveArray(tracesLabiales, true);
                    break;
                case "sang":
                    SetActiveArray(tracesDeSang, true);
                    break;
                case "pas":
                    SetActiveArray(tracesDePas, true);
                    break;
            }

            currentActiveRange = activeRange;
        }
    }

    private void UpdatePapillairesVisibility()
    {
        bool shouldShowTraces = inPapillaireRange && powderApplied;
        SetActiveArray(tracesPapillaires, shouldShowTraces);
    }

    private void UpdatePowderButtonVisibility()
    {
        if (powderButton != null)
        {
            powderButton.gameObject.SetActive(inPapillaireRange && !powderApplied);
        }
    }

    private void SetActiveArray(GameObject[] objects, bool active)
    {
        if (objects != null)
        {
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    obj.SetActive(active);
                }
            }
        }
    }

    public void HideAllImages()
    {
        SetActiveArray(tracesPapillaires, false);
        SetActiveArray(tracesLabiales, false);
        SetActiveArray(tracesDeSang, false);
        SetActiveArray(tracesDePas, false);
    }

    public string GetCurrentTraceType()
    {
        return currentActiveRange;
    }

    public void ResetPowderSystem()
    {
        powderApplied = false;
        UpdatePowderButtonVisibility();
        UpdatePapillairesVisibility();
    }
}
