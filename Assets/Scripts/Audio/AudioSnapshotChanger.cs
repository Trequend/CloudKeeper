using UnityEngine;

public class AudioSnapshotChanger : MonoBehaviour
{
    public void SetDefaultSnapshot()
    {
        AudioVolume.SetSnapshot(AudioVolume.DefaultSnapshot);
    }

    public void SetMenuSnapshot()
    {
        AudioVolume.SetSnapshot(AudioVolume.MenuSnapshot);
    }
}
