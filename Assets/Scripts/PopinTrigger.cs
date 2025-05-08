using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopinTrigger : MonoBehaviour
{
    [Header("Contenu de la popin")]
    public string titre;
    [TextArea]
    public string description;

    [Header("Références de la popin")]
    public GameObject popinCanvas;
    public TextMeshProUGUI titreTMP;
    public TextMeshProUGUI descriptionTMP;
    public Button closeButton;

    private void Start()
    {
        if (popinCanvas != null)
            popinCanvas.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePopin);
    }

    private void OnMouseDown()
    {
        ShowPopin();
    }

    void ShowPopin()
    {
        if (popinCanvas != null)
        {
            popinCanvas.SetActive(true);
            if (titreTMP != null) titreTMP.text = titre;
            if (descriptionTMP != null) descriptionTMP.text = description;
        }
    }

    void ClosePopin()
    {
        if (popinCanvas != null)
            popinCanvas.SetActive(false);
    }
}
