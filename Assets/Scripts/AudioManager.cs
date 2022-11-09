using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using System;
using UnityEngine;
[DefaultExecutionOrder(-1)] //força esse script a carregar antes de todo mundo a não ser que algém tenha um número menor
public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    public Sound[] sounds;



    void Awake() {

        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.playOnAwake = false;
            s.source.name = s.clipName;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.clipName == name);
        s.source.Play();
    }
}
