using UnityEngine;

public class AmbientByTime : MonoBehaviour
{
    [SerializeField] private Skyboxspin timeSystem;

    public AudioClip night;
    public AudioClip morning;
    public AudioClip day;
    public AudioClip sunset;

    private AudioClip current;

    void Update()
    {
        float min = timeSystem.CurrentMinutesOfDay();
        AudioClip target = GetClip(min);

        if (target != current)
        {
            current = target;
            AudioManager.instancia.PlayAmbient(target);
        }
    }

    AudioClip GetClip(float min)
    {
        if (min < 360) return night;
        if (min < 480) return morning;
        if (min < 960) return day;
        if (min < 1320) return sunset;
        return night;
    }
}