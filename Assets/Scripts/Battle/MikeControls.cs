using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// microphone control
/// </summary>
public class MikeControls : MonoBehaviour {
    public static MikeControls instance; // Static instance of this class for referencing purposes.
    [HideInInspector]
    public static LuaMike luaStatus { get; private set; } // The Lua Arena object on the C# side

    public bool test = false;

    public int resolution = 12; 
    // Unity can only receive audio depths of powers of 2, this determines the exponent 

    public float current = 0;
    // current volume of note played

    public int currentIndex = 0;
    // current index of note played in the GetSpectrumData array

    public float output = 0;
    // currentIndex, converted to hertz


    public float testingVal = 0;
    // used for sensitivity bar

    [Range(0f, 5f)]
    public float sensitivity = 0.5f;
    // lowest possible volume "current" needs to be in order to be registered

    private AudioClip microphoneClip;
    // microphone will be put here
    
    
    public int microphoneIndex = 0;
    // plugged in microphones are stored in Microphone.devices


    public float debugX = 120;
    public float debugY = 100;
    // used for testing with gizmos


    /// <summary>
    /// Initialization.
    /// </summary>
    private void Awake() {
        // unlike the player we really dont want this on two components at the same time
        if (instance != null)
            throw new CYFException("Currently, the MikeControls may only be attached to one object.");
        instance = this;
        luaStatus = new LuaMike();
        // lua initialization.. i frankly don't know what this does, I copied this from the Arena object
    }


    // lua functions

    // returns microphone list
    public string[] getMicrophones() {
        return (Microphone.devices);
    }

    // sets microphone from index
    // restart bool determines if start recording or not
    public void setMicrophone(int index, bool restart) {
        if(microphoneIndex != index) {
        microphoneIndex = index;
        this.StartRecording();
        }
    }

    // starts recording
    // for some reason, I had to input a maximum length for the clip, I set it to 5 minutes (300), chance at your own precaution
    public void StartRecording() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = Microphone.Start(Microphone.devices[microphoneIndex], true, 300, AudioSettings.outputSampleRate);
        audioSource.Play();
    }

    // pauses the recording, crucial for optimization!! don't let it run forever!!
    public void StopRecording() {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = null;
    }

    // returns if microphone is playing, unity built-in function
    public bool IsPlaying() {
        AudioSource audioSource = GetComponent<AudioSource>();
        return(audioSource.isPlaying);
    }

    // this is a tad embarassing, but... the AudioListener needs to hear the AudioSource ingame, so I had to make the microphone output audiable:
    // set the volume to something low like 0.1
    // output gets divided by this value, so that the volume is irellevent to the output
    public void SetVolume(float volume) {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.volume = volume;
    }

    // whatever
    private void Start() {

        AudioSource audioSource = GetComponent<AudioSource>();
    
    }

    // calculation of output
    private void Update() {
        AudioSource audioSource = GetComponent<AudioSource>();
        if(audioSource.isPlaying) {

        float[] spectrum = new float[(int) (Mathf.Pow(2,resolution))];

        // adjust output for volume.
        float squah = 100/audioSource.volume;
        



        int minP = 60;
        int maxP = 900;
        int minIndex = (int) Mathf.Floor (minP / 24000f * ((int) (Mathf.Pow(2,resolution))));
        int maxIndex = (int) Mathf.Ceil (maxP / 24000f * ((int) (Mathf.Pow(2,resolution))));

        // this is the +magical+ function that makes it all work! dunno how it works, dont care!
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
        
        int biggestIndex = 0;
        float biggest = 0;
        testingVal = 0;
        float howManyBroke = 0;

        // very, very cheap way of finding the biggest note
        for (int i = 0; i < maxIndex; i++)
        {
            if((biggestIndex == null) || ( spectrum[i] > spectrum[biggestIndex])) {
                biggestIndex = i;
                biggest = spectrum[biggestIndex];
                testingVal = spectrum[biggestIndex];
                

                if((biggest * squah > sensitivity)) {
                    howManyBroke++;
                }

            }
            
            // debug gizmo lines i made
            
            if(test){
            Debug.DrawLine(new Vector3(0, sensitivity*debugY, 0), new Vector3(50*debugX, sensitivity*debugY, 0), Color.yellow);
            Debug.DrawLine(new Vector3(Mathf.Log(i)*debugX, 0, 0), new Vector3(Mathf.Log(i)*debugX, spectrum[i]*squah*debugY, 0), Color.red);
            }}

        testingVal = testingVal * squah;
        if(biggestIndex < minIndex)  {
            testingVal = 0;
        }

        // needs to be bigger than sensitivity
        // and higher than lowest index
        if((biggest * squah > sensitivity) && (howManyBroke < 15) && (biggestIndex > minIndex)) {
        current = biggest * squah;
        currentIndex = biggestIndex;

        // index in array -> to hertz calculation
        output = currentIndex * 24000 / ((int) (Mathf.Pow(2,resolution)));
        }


        // sensitivity gizmo
        if(test){
        Debug.DrawLine(new Vector3(Mathf.Log(currentIndex)*debugX, 0, 0), new Vector3(Mathf.Log(currentIndex)*debugX, current*debugY, 0), Color.green);
        }}
        else
        {
            testingVal = 0;
        }
    }

}
