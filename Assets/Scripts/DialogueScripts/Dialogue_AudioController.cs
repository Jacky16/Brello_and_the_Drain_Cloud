using UnityEngine;
using TMPro;
using DG.Tweening;

public class Dialogue_AudioController : MonoBehaviour
{
    private Dialogue_NPC villager;
    private TMP_Dialogue animatedText;
    public Transform mouthQuad;

    public AudioClip[] voices;
    public AudioClip[] punctuations;

    [Space]
    public AudioSource voiceSource;
    public AudioSource punctuationSource;
    public AudioSource effectSource;

    [Space]
    public AudioClip sparkleClip;
    public AudioClip rainClip;
    public AudioClip angryClip;
    public AudioClip doubtClip;

    // Start is called before the first frame update
    void Start()
    {
        villager = GetComponent<Dialogue_NPC>();

        animatedText = Dialogue_Manager.instance.animatedText;

        animatedText.onTextReveal.AddListener((newChar) => ReproduceSound(newChar));
    }

    public void ReproduceSound(char c)
    {

        if (villager != Dialogue_Manager.instance.currentVillager)
            return;

        if (char.IsPunctuation(c) && !punctuationSource.isPlaying)
        {
            voiceSource.Stop();
            punctuationSource.clip = punctuations[Random.Range(0, punctuations.Length)];
            punctuationSource.Play();
        }

        if (char.IsLetter(c) && !voiceSource.isPlaying)
        {
            punctuationSource.Stop();
            voiceSource.clip = voices[Random.Range(0, voices.Length)];
            voiceSource.Play();

            //mouthQuad.localScale = new Vector3(1, 0, 1);
            //mouthQuad.DOScaleY(1, .2f).OnComplete(() => mouthQuad.DOScaleY(0, .2f));
        }

    }
}
