using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    private MascotteTextMessages mascotteMessages;

    public enum TutorialState
    {
        Introduction,
        ActivationDuFiltre,
        AjustementDuFiltre,
        AnalyseDesRésultats,
        ExplicationDesRésultats,
        ObservationFinale,
        NouveauMessage1,
        NouveauMessage2,
        Conclusion
    }

    public TutorialState CurrentState { get; private set; } = TutorialState.Introduction;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        mascotteMessages = FindObjectOfType<MascotteTextMessages>();
        if (mascotteMessages == null)
        {
            Debug.LogError("MascotteTextMessages non trouvé dans la scène");
        }
        else
        {
            UpdateMascotteMessage();
        }
    }

    public void AdvanceToNextState()
    {
        switch (CurrentState)
        {
            case TutorialState.Introduction:
                CurrentState = TutorialState.ActivationDuFiltre;
                break;
            case TutorialState.ActivationDuFiltre:
                CurrentState = TutorialState.AjustementDuFiltre;
                break;
            case TutorialState.AjustementDuFiltre:
                CurrentState = TutorialState.AnalyseDesRésultats;
                break;
            case TutorialState.AnalyseDesRésultats:
                CurrentState = TutorialState.ExplicationDesRésultats;
                break;
            case TutorialState.ExplicationDesRésultats:
                CurrentState = TutorialState.ObservationFinale;
                break;
            case TutorialState.ObservationFinale:
                CurrentState = TutorialState.NouveauMessage1;
                break;
            case TutorialState.NouveauMessage1:
                CurrentState = TutorialState.NouveauMessage2;
                break;
            case TutorialState.NouveauMessage2:
                CurrentState = TutorialState.Conclusion;
                break;
            case TutorialState.Conclusion:
                Debug.Log("Tutoriel terminé!");
                break;
        }

        UpdateMascotteMessage();
    }

    public void SetTutorialState(TutorialState newState)
    {
        CurrentState = newState;
        UpdateMascotteMessage();
    }

    private void UpdateMascotteMessage()
    {
        if (mascotteMessages == null) return;

        mascotteMessages.SetMessageByState((int)CurrentState);
    }

    public int GetCurrentMessageIndex()
    {
        return (int)CurrentState;
    }

    public bool IsTutorialCompleted()
    {
        return CurrentState == TutorialState.Conclusion
            && mascotteMessages.GetCurrentMessageIndex() >= 8;
    }

    public void ResetTutorial()
    {
        CurrentState = TutorialState.Introduction;
        mascotteMessages.ResetTutorial();
        UpdateMascotteMessage();
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
