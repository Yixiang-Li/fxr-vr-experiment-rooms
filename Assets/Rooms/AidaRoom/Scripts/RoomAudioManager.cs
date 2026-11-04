
using System.Collections;
using UnityEngine;

public class RoomAudioManager : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip startSound;
    [SerializeField] private AudioClip resetSound;
    [SerializeField] private AudioClip timerSound;
    [SerializeField] private AudioClip lightSwitchSound;

    [Header("Sound Durations (0 = full clip)")]
    [SerializeField] private float startDuration = 0.4f;
    [SerializeField] private float resetDuration = 0.4f;
    [SerializeField] private float timerDuration = 0.8f;
    [SerializeField] private float lightSwitchDuration = 0f;

    [Header("Sound Volume")]
    [SerializeField, Range(0f, 1f)]
    private float volume = 0.7f;

    [Header("Researcher Voiceovers")]
    [SerializeField] private AudioClip welcome;
    [SerializeField] private AudioClip afterConsent;
    [SerializeField] private AudioClip afterExp;

    [SerializeField, Range(0f, 1f)]
    private float voiceVolume = 1f;

    [Header("Voiceover Timing")]
    [SerializeField] private float welcomeDelay = 3f;
    [SerializeField] private float consentDelay = 0.3f;
    [SerializeField] private float endingDelay = 1f;

    [Header("Consent Detection")]
    [SerializeField] private ConsentPaperInteraction consent;

    [Header("Lamp Switch")]
    [SerializeField] private Collider lampSwitchCollider;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float maxClickDistance = 5f;

    private AudioSource[] sources;
    private Coroutine[] stopCoroutines;
    private AudioSource voiceSource;


    private int nextSource;
    private bool consentDetected;
    private bool endingPlayed;
    private Coroutine voiceCoroutine;

    private void Awake()
    {
        sources = new AudioSource[4];
        stopCoroutines = new Coroutine[4];

        for (int i = 0; i < sources.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
            source.volume = volume;
            sources[i] = source;
        }

        voiceSource = gameObject.AddComponent<AudioSource>();
        voiceSource.playOnAwake = false;
        voiceSource.loop = false;
        voiceSource.spatialBlend = 0f;
        voiceSource.volume = voiceVolume;
    }

    private void Start()
    {
        consentDetected = consent != null && consent.HasConsent;

        if (consentDetected)
        {
            ScheduleVoice(afterConsent, consentDelay);
        }
        else
        {
            ScheduleVoice(welcome, welcomeDelay);
        }
    }

    private void Update()
    {
        CheckConsent();
        CheckLampClick();
    }

    private void CheckConsent()
    {
        if (consent == null || consentDetected)
            return;

        if (consent.HasConsent)
        {
            consentDetected = true;
            ScheduleVoice(afterConsent, consentDelay);
        }
    }

    public void ConfigureCamera(Camera camera) { playerCamera = camera; }

    private void CheckLampClick()
    {
        if (lampSwitchCollider == null)
            return;

        if (DeskPointer.PressedOn(lampSwitchCollider, playerCamera))
            PlayLightSwitch();
    }

    public void ResetExperimentAudio()
    {
        endingPlayed = false;
        if (voiceCoroutine != null) StopCoroutine(voiceCoroutine);
        voiceCoroutine = null;
        if (voiceSource != null) voiceSource.Stop();
    }

    public void PlayStart()
    {
        PlayClip(startSound, startDuration);
    }

    public void PlayReset()
    {
        PlayClip(resetSound, resetDuration);
    }

    public void PlayTimer()
    {
        PlayClip(timerSound, timerDuration);
    }

    public void PlayLightSwitch()
    {
        PlayClip(lightSwitchSound, lightSwitchDuration);
    }

    public void PlayExperimentEnding()
    {
        if (endingPlayed)
            return;

        endingPlayed = true;
        ScheduleVoice(afterExp, endingDelay);
    }

    private void ScheduleVoice(AudioClip clip, float delay)
    {
        if (clip == null)
        {
            Debug.LogWarning("Researcher voiceover not assigned!");
            return;
        }

        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;
        }

        voiceSource.Stop();
        voiceCoroutine = StartCoroutine(
            PlayVoiceAfterDelay(clip, delay)
        );
    }

    private IEnumerator PlayVoiceAfterDelay(
        AudioClip clip,
        float delay)
    {
        yield return new WaitForSeconds(
            Mathf.Max(0f, delay)
        );

        voiceSource.clip = clip;
        voiceSource.volume = voiceVolume;
        voiceSource.Play();

        voiceCoroutine = null;
    }

    private void PlayClip(AudioClip clip, float duration)
    {
        if (clip == null)
        {
            Debug.LogWarning("Audio clip missing!");
            return;
        }

        int index = nextSource;
        nextSource = (nextSource + 1) % sources.Length;

        AudioSource source = sources[index];

        if (stopCoroutines[index] != null)
        {
            StopCoroutine(stopCoroutines[index]);
            stopCoroutines[index] = null;
        }

        source.Stop();
        source.clip = clip;
        source.volume = volume;
        source.Play();

        if (duration > 0f)
        {
            stopCoroutines[index] =
                StartCoroutine(StopAfter(index, duration));
        }
    }

    private IEnumerator StopAfter(int index, float duration)
    {
        yield return new WaitForSeconds(duration);

        sources[index].Stop();
        stopCoroutines[index] = null;
    }
}
