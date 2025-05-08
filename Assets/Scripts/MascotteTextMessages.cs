using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MascotteTextMessages : MonoBehaviour
{
    private Text messageText;
    private Image backgroundImage;

    [Header("GameObjects Cliquables Supplémentaires")]
    [Tooltip("Un GameObject qui pourra aussi être cliqué pour avancer dans les messages")]
    [SerializeField] private GameObject additionalClickableObject;

    [Tooltip("Un deuxième GameObject qui pourra aussi être cliqué pour avancer dans les messages")]
    [SerializeField] private GameObject secondClickableObject;

    [Header("Messages prédéfinis")]
    [TextArea(2, 4)] public string message1 = "Premier message du tutoriel.";
    [TextArea(2, 4)] public string message2 = "Deuxième message du tutoriel.";
    [TextArea(2, 4)] public string message3 = "Troisième message du tutoriel.";
    [TextArea(2, 4)] public string message4 = "Quatrième message du tutoriel.";
    [TextArea(2, 4)] public string message5 = "Cinquième message du tutoriel.";
    [TextArea(2, 4)] public string message6 = "Sixième message du tutoriel.";
    [TextArea(2, 4)] public string message7 = "Septième message du tutoriel.";
    [TextArea(2, 4)] public string message8 = "Huitième message du tutoriel.";
    [TextArea(2, 4)] public string message9 = "Neuvième message du tutoriel.";

    [Header("Voix-off pour chaque message")]
    [Tooltip("Son de voix-off pour le message 1")]
    [SerializeField] private AudioClip voiceMessage1;
    [Tooltip("Son de voix-off pour le message 2")]
    [SerializeField] private AudioClip voiceMessage2;
    [Tooltip("Son de voix-off pour le message 3")]
    [SerializeField] private AudioClip voiceMessage3;
    [Tooltip("Son de voix-off pour le message 4")]
    [SerializeField] private AudioClip voiceMessage4;
    [Tooltip("Son de voix-off pour le message 5")]
    [SerializeField] private AudioClip voiceMessage5;
    [Tooltip("Son de voix-off pour le message 6")]
    [SerializeField] private AudioClip voiceMessage6;
    [Tooltip("Son de voix-off pour le message 7")]
    [SerializeField] private AudioClip voiceMessage7;
    [Tooltip("Son de voix-off pour le message 8")]
    [SerializeField] private AudioClip voiceMessage8;
    [Tooltip("Son de voix-off pour le message 9")]
    [SerializeField] private AudioClip voiceMessage9;

    [Header("Progression automatique")]
    [Tooltip("Délai supplémentaire après la fin de la voix-off avant de passer à l'étape suivante")]
    [SerializeField] private float autoAdvanceDelay = 1.0f;

    private int currentMessageIndex = 0;

    private const int TOTAL_MESSAGES = 9;

    [Header("Options")]
    [SerializeField] private Animator animator;
    [SerializeField] private string animationTriggerName = "NewMessage";

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip messageSound;

    [SerializeField] private AudioClip finishSound;

    [Tooltip("Interrompre la voix-off en cours si on passe au message suivant")]
    [SerializeField] private bool interruptVoiceOnMessageChange = true;

    [Header("Comportement")]
    [Tooltip("Masque automatiquement le message après avoir lu tous les messages")]
    [SerializeField] private bool hideAfterLastMessage = true;

    [Tooltip("Délai avant de masquer le message (secondes)")]
    [SerializeField] private float hideDelay = 0.5f;

    private Coroutine autoAdvanceCoroutine = null;

    void Start()
    {
        Transform backgroundTransform = transform.Find("BackgroundText");
        if (backgroundTransform != null)
        {
            backgroundImage = backgroundTransform.GetComponent<Image>();

            Transform textTransform = backgroundTransform.Find("Text");
            if (textTransform != null)
            {
                messageText = textTransform.GetComponent<Text>();
            }
        }

        if (messageText == null)
        {
            Debug.LogError("Composant Text non trouvé. Assurez-vous que la structure est: MascotteMessages -> BackgroundText -> Text");
            return;
        }

        if (backgroundImage == null)
        {
            Debug.LogError("Composant Image (BackgroundText) non trouvé.");
            return;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("AudioSource ajouté automatiquement à MascotteTextMessages");
            }
        }

        ShowCurrentMessage();

        SetupClickDetection(backgroundTransform.gameObject);

        if (additionalClickableObject != null)
        {
            SetupClickDetection(additionalClickableObject);
        }

        if (secondClickableObject != null)
        {
            SetupClickDetection(secondClickableObject);
        }
    }

    private void SetupClickDetection(GameObject clickableObject)
    {
        EventTrigger eventTrigger = clickableObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = clickableObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;

        entry.callback.AddListener((data) => { OnMessageClicked(); });

        if (eventTrigger.triggers == null)
            eventTrigger.triggers = new List<EventTrigger.Entry>();

        eventTrigger.triggers.Add(entry);
    }

    public void OnMessageClicked()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        currentMessageIndex++;

        if (currentMessageIndex >= TOTAL_MESSAGES)
        {
            if (audioSource != null && finishSound != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(finishSound);
            }

            if (hideAfterLastMessage)
            {
                StartCoroutine(HideAfterDelay(hideDelay));
                return;
            }
            else
            {
                currentMessageIndex = 0;
            }
        }

        ShowCurrentMessage();

        if (animator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            animator.SetTrigger(animationTriggerName);
        }

        if (audioSource != null && messageSound != null)
        {
            audioSource.PlayOneShot(messageSound);
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void ShowCurrentMessage()
    {
        AudioClip voiceClip = null;

        switch (currentMessageIndex)
        {
            case 0:
                messageText.text = message1;
                voiceClip = voiceMessage1;
                break;
            case 1:
                messageText.text = message2;
                voiceClip = voiceMessage2;
                break;
            case 2:
                messageText.text = message3;
                voiceClip = voiceMessage3;
                break;
            case 3:
                messageText.text = message4;
                voiceClip = voiceMessage4;
                break;
            case 4:
                messageText.text = message5;
                voiceClip = voiceMessage5;
                break;
            case 5:
                messageText.text = message6;
                voiceClip = voiceMessage6;
                break;
            case 6:
                messageText.text = message7;
                voiceClip = voiceMessage7;
                break;
            case 7:
                messageText.text = message8;
                voiceClip = voiceMessage8;
                break;
            case 8:
                messageText.text = message9;
                voiceClip = voiceMessage9;
                break;
        }

        PlayVoiceOver(voiceClip);
    }

    private void PlayVoiceOver(AudioClip voiceClip)
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (audioSource != null && voiceClip != null)
        {
            if (interruptVoiceOnMessageChange || !audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            StartCoroutine(PlayVoiceWithDelay(voiceClip, 0.2f));

            if (currentMessageIndex == 0 || currentMessageIndex == 3 || currentMessageIndex == 4 ||
                currentMessageIndex == 5 || currentMessageIndex == 6 || currentMessageIndex == 7)
            {
                float totalDelay = voiceClip.length + autoAdvanceDelay;
                autoAdvanceCoroutine = StartCoroutine(AutoAdvanceAfterDelay(totalDelay));
            }
            else if (currentMessageIndex == 8)
            {
                float totalDelay = voiceClip.length + autoAdvanceDelay;
                autoAdvanceCoroutine = StartCoroutine(CloseAfterLastMessage(totalDelay));
            }
        }
    }

    private IEnumerator PlayVoiceWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.clip = clip;
        audioSource.Play();
    }

    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameObject.activeInHierarchy)
        {
            OnMessageClicked();
        }
    }

    private IEnumerator CloseAfterLastMessage(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (gameObject.activeInHierarchy)
        {
            if (audioSource != null && finishSound != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(finishSound);
            }

            StartCoroutine(HideAfterDelay(hideDelay));
        }
    }

    public void ResetTutorial()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        currentMessageIndex = 0;
        ShowCurrentMessage();
        gameObject.SetActive(true);
    }

    public void SetMessageByState(int stateIndex)
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }

        if (stateIndex >= 0 && stateIndex < TOTAL_MESSAGES)
        {
            currentMessageIndex = stateIndex;
            ShowCurrentMessage();

            if (animator != null && !string.IsNullOrEmpty(animationTriggerName))
            {
                animator.SetTrigger(animationTriggerName);
            }

            if (audioSource != null && messageSound != null)
            {
                audioSource.PlayOneShot(messageSound);
            }
        }
    }

    public int GetCurrentMessageIndex()
    {
        return currentMessageIndex;
    }

    public void StopVoiceOver()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }
    }

    void OnDisable()
    {
        if (autoAdvanceCoroutine != null)
        {
            StopCoroutine(autoAdvanceCoroutine);
            autoAdvanceCoroutine = null;
        }
    }
}
