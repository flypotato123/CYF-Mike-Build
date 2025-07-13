/// <summary>
/// Lua binding to set and retrieve information for Microphone.
/// </summary>
public class LuaMike {
    // MikeControls

    
    public float currentNote { get{return MikeControls.instance.output;}}
    
    public bool isNote { get{return MikeControls.instance.isNote;}}
    
    
    public float testingVal { get{return MikeControls.instance.testingVal;}}
    

    public string[] getMicrophones { get{return MikeControls.instance.getMicrophones();}}

    
    public void setMicrophone(int index, bool restart) {
        MikeControls.instance.setMicrophone(index, restart);
    }

    public float sensitivity {
        get { return MikeControls.instance.sensitivity; }
        set { MikeControls.instance.sensitivity = value; }
    }

    public void setVolume(float volume) {
        MikeControls.instance.SetVolume(volume);
    }

    public void startRecording() {
        MikeControls.instance.StartRecording();
    }
    public void stopRecording() {
        MikeControls.instance.StopRecording();
    }
    
    public bool isPlaying() {
        return MikeControls.instance.IsPlaying();
    }
}