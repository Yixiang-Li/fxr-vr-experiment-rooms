using UnityEngine;

namespace FXR.Memory
{
    // Audio stays outside the switched decoration roots so transitions can fade naturally.
    public sealed class MemoryAtmosphere : MonoBehaviour
    {
        public AudioSource[] officeSources, natureSources;
        public AudioSource feedbackSource;
        public AudioClip flipClip, matchClip, mismatchClip;
        public float fadeSeconds = .65f;
        public bool soundEnabled = true;
        float blend;
        bool natural;
        float[] officeLevels, natureLevels;

        void Awake()
        {
            officeLevels = CaptureLevels(officeSources);
            natureLevels = CaptureLevels(natureSources);
        }

        static float[] CaptureLevels(AudioSource[] sources)
        {
            var levels = new float[sources.Length];
            for (int i = 0; i < sources.Length; i++) levels[i] = sources[i].volume;
            return levels;
        }

        public void SetCondition(bool nature, bool immediate)
        {
            natural = nature;
            if (immediate) blend = nature ? 1 : 0;
            foreach (var source in officeSources) if (!source.isPlaying) source.Play();
            foreach (var source in natureSources) if (!source.isPlaying) source.Play();
            UpdateLevels();
        }

        void Update()
        {
            blend = Mathf.MoveTowards(blend, natural ? 1 : 0, Time.deltaTime / Mathf.Max(.01f, fadeSeconds));
            UpdateLevels();
        }

        void UpdateLevels()
        {
            if (officeLevels == null) return;
            for (int i = 0; i < officeSources.Length; i++)
                officeSources[i].volume = soundEnabled ? officeLevels[i] * (1 - blend) : 0;
            for (int i = 0; i < natureSources.Length; i++)
                natureSources[i].volume = soundEnabled ? natureLevels[i] * blend : 0;
        }

        public void Feedback(bool? match = null)
        {
            if (!soundEnabled || feedbackSource == null) return;
            var clip = match.HasValue ? (match.Value ? matchClip : mismatchClip) : flipClip;
            if (clip != null) feedbackSource.PlayOneShot(clip, .45f);
        }
    }
}
