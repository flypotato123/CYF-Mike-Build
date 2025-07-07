using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour attached to the arena used to resize it.
/// </summary>
public class MikeControls : MonoBehaviour {
    public static MikeControls instance; // Static instance of this class for referencing purposes.
    [HideInInspector]
    public static LuaMike luaStatus { get; private set; } // The Lua Arena object on the C# side

    public int resolution = 12;
    public float current = 0;
    public float output = 0;
    public int currentIndex = 0;
    public float testingVal = 0;
    
    [Range(0f, 5f)]
    public float sensitivity = 0.5f;

    private AudioClip microphoneClip;
    
    
    public int microphoneIndex = 0;

    public float debugX = 120;
    public float debugY = 100;

    /// <summary>
    /// Initialization.
    /// </summary>
    private void Awake() {
        // unlike the player we really dont want this on two components at the same time
        if (instance != null)
            throw new CYFException("Currently, the MikeControls may only be attached to one object.");
        instance = this;
        luaStatus = new LuaMike();
    }

    public string[] getMicrophones() {
        return (Microphone.devices);
    }
    public void setMicrophone(int index, bool restart) {
        if(microphoneIndex != index) {
        microphoneIndex = index;
        this.StartRecording();
        }
    }

    private void Start() {

        AudioSource audioSource = GetComponent<AudioSource>();
    
    }

    public void StartRecording() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = Microphone.Start(Microphone.devices[microphoneIndex], true, 300, AudioSettings.outputSampleRate);
        audioSource.Play();
    }

    public void StopRecording() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = null;
    }

    public bool IsPlaying() {
        AudioSource audioSource = GetComponent<AudioSource>();
        return(audioSource.isPlaying);
    }

    public void SetVolume(float volume) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    private void Update() {
        AudioSource audioSource = GetComponent<AudioSource>();
        if(audioSource.isPlaying) {

        float[] spectrum = new float[(int) (Mathf.Pow(2,resolution))];

        
        float squah = 100/audioSource.volume;
        



        int minP = 70;
        int maxP = 900;
        int minIndex = (int) Mathf.Floor (minP / 24000f * ((int) (Mathf.Pow(2,resolution))));
        int maxIndex = (int) Mathf.Ceil (maxP / 24000f * ((int) (Mathf.Pow(2,resolution))));

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
        
        int biggestIndex = 0;
        float biggest = 0;
        testingVal = 0;

        for (int i = minIndex; i < maxIndex; i++)
        {
            if((biggestIndex == null) || ( spectrum[i] > spectrum[biggestIndex])) {
                biggestIndex = i;
                biggest = spectrum[biggestIndex];
                testingVal = spectrum[biggestIndex];
            }
            
            // Debug.DrawLine(new Vector3(0, sensitivity*debugY, 0), new Vector3(50*debugX, sensitivity*debugY, 0), Color.yellow);
            
            // Debug.DrawLine(new Vector3(Mathf.Log(i)*debugX, 0, 0), new Vector3(Mathf.Log(i)*debugX, spectrum[i]*squah*debugY, 0), Color.red);
            }
        testingVal = testingVal * squah;

        if((biggest * squah > sensitivity)) {
        current = biggest * squah;
        currentIndex = biggestIndex;

        output = currentIndex * 24000 / ((int) (Mathf.Pow(2,resolution)));
        }
        // Debug.DrawLine(new Vector3(Mathf.Log(currentIndex)*debugX, 0, 0), new Vector3(Mathf.Log(currentIndex)*debugX, current*debugY, 0), Color.green);
        }
        else
        {
            testingVal = 0;
        }
    }

}
