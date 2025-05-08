using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Collections;

public class ChecklistScript : MonoBehaviour
{
    [Header("Objets à trouver")]
    [Tooltip("Liste des objets à trouver")]
    public List<GameObject> objectsToFind;

    [Header("Interface")]
    [Tooltip("Le TextMeshPro qui affichera la progression")]
    public TextMeshProUGUI progressText;

    [Tooltip("Le TextMeshPro qui affichera le timer")]
    public TextMeshProUGUI timerText;

    [Tooltip("Le bouton pour démarrer le timer")]
    public Button startTimerButton;

    [Header("Audio")]
    [Tooltip("Son joué lorsqu'un objet est trouvé")]
    public AudioClip successSound;

    [Tooltip("Volume du son de succès")]
    [Range(0f, 1f)]
    public float successVolume = 1.0f;

    [Tooltip("Son joué lorsque tous les objets sont trouvés")]
    public AudioClip completionSound;

    [Tooltip("Volume du son de complétion")]
    [Range(0f, 1f)]
    public float completionVolume = 1.0f;

    [Header("Écran de fin")]
    [Tooltip("L'écran de fin à afficher quand tous les objets sont trouvés")]
    public GameObject endScreen;

    [Tooltip("Délai avant d'afficher l'écran de fin (en secondes)")]
    public float endScreenDelay = 5f;

    [Header("Options")]
    [Tooltip("Affiche les noms des objets trouvés dans la console")]
    public bool logFoundObjects = true;

    [Tooltip("Comportement du bouton quand appuyé pendant que le timer est en cours")]
    public ButtonBehavior buttonBehavior = ButtonBehavior.RestartTimer;

    [Header("Références pour l'écran de fin")]
    [Tooltip("Référence directe au TextMeshProUGUI qui affichera le temps final")]
    public TextMeshProUGUI finalTimeDisplay;

    public enum ButtonBehavior
    {
        RestartTimer,
        DoNothing,
        TogglePause
    }

    private HashSet<GameObject> foundObjects = new HashSet<GameObject>();
    private int totalObjects;
    private bool timerRunning = false;
    private float elapsedTime = 0f;
    private bool isPaused = false;
    private AudioSource audioSource;
    private bool endScreenCoroutineStarted = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (objectsToFind == null || objectsToFind.Count == 0)
        {
            Debug.LogWarning("Aucun objet à trouver n'a été configuré dans ChecklistScript!");
            totalObjects = 0;
        }
        else
        {
            totalObjects = objectsToFind.Count;
        }

        if (progressText == null)
        {
            Debug.LogError("Le champ TextMeshPro pour la progression n'est pas configuré!");
        }

        if (timerText == null)
        {
            Debug.LogError("Le champ TextMeshPro pour le timer n'est pas configuré!");
        }
        else
        {
            UpdateTimerText();
        }

        if (startTimerButton == null)
        {
            Debug.LogError("Le bouton de démarrage du timer n'est pas configuré!");
        }
        else
        {
            startTimerButton.onClick.AddListener(StartTimer);
        }

        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }
        else
        {
            Debug.LogWarning("L'écran de fin n'est pas configuré!");
        }

        UpdateProgressText();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckForClickedObject();
        }

        if (timerRunning && !isPaused)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void CheckForClickedObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject clickedObject = hit.collider.gameObject;

            if (objectsToFind.Contains(clickedObject) && !foundObjects.Contains(clickedObject))
            {
                foundObjects.Add(clickedObject);

                PlaySuccessSound();

                if (logFoundObjects)
                {
                    Debug.Log("Objet trouvé: " + clickedObject.name);
                }

                UpdateProgressText();

                if (foundObjects.Count >= totalObjects)
                {
                    if (timerRunning)
                    {
                        StopTimer();
                    }

                    PlayCompletionSound();

                    if (!endScreenCoroutineStarted)
                    {
                        endScreenCoroutineStarted = true;
                        StartCoroutine(ShowEndScreenAfterDelay());
                    }
                }
            }
        }
    }

    private void PlaySuccessSound()
    {
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound, successVolume);
        }
    }

    private void PlayCompletionSound()
    {
        if (audioSource != null && completionSound != null)
        {
            audioSource.PlayOneShot(completionSound, completionVolume);
            Debug.Log("Son de complétion joué!");
        }
        else if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound, completionVolume);
            Debug.Log("Son de succès joué comme complétion (aucun son de complétion défini)");
        }
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = foundObjects.Count + " / " + totalObjects;
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            timerText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

    private IEnumerator ShowEndScreenAfterDelay()
    {
        Debug.Log($"Tous les objets ont été trouvés! L'écran de fin s'affichera dans {endScreenDelay} secondes.");

        yield return new WaitForSeconds(endScreenDelay);

        ShowEndScreen();

        endScreenCoroutineStarted = false;
    }

    private void ShowEndScreen()
    {
        if (endScreen != null)
        {
            Debug.Log("Affichage de l'écran de fin.");
            endScreen.SetActive(true);

            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);
            string formattedTime = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);

            if (finalTimeDisplay != null)
            {
                finalTimeDisplay.text = "Temps final: " + formattedTime;
            }
            else
            {
                TextMeshProUGUI[] endScreenTexts = endScreen.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (TextMeshProUGUI text in endScreenTexts)
                {
                    if (text.gameObject.name.Contains("FinalTime"))
                    {
                        text.text = "Temps final: " + formattedTime;
                        break;
                    }
                }
            }
        }
    }

    public void StartTimer()
    {
        if (!timerRunning)
        {
            timerRunning = true;
            isPaused = false;
            elapsedTime = 0f;
            Debug.Log("Timer démarré!");
        }
        else
        {
            switch (buttonBehavior)
            {
                case ButtonBehavior.RestartTimer:
                    elapsedTime = 0f;
                    Debug.Log("Timer redémarré!");
                    break;

                case ButtonBehavior.DoNothing:
                    break;

                case ButtonBehavior.TogglePause:
                    isPaused = !isPaused;
                    Debug.Log(isPaused ? "Timer en pause!" : "Timer repris!");
                    break;
            }
        }
    }

    private void StopTimer()
    {
        if (timerRunning)
        {
            timerRunning = false;
            isPaused = false;
            Debug.Log("Timer arrêté! Temps final: " + timerText.text);
        }
    }

    public void Reset()
    {
        StopAllCoroutines();
        endScreenCoroutineStarted = false;

        foundObjects.Clear();
        UpdateProgressText();

        StopTimer();
        elapsedTime = 0f;
        UpdateTimerText();

        if (endScreen != null)
        {
            endScreen.SetActive(false);
        }
    }
}
