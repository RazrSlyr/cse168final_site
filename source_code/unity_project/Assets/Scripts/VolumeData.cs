using UnityEngine;

public class VolumeData
{
    public int[] dimensions;
    public float[] data;

    public static VolumeData GetVolumeDataFromJson(string jsonData) {
        return JsonUtility.FromJson<VolumeData>(jsonData);
    }
}
