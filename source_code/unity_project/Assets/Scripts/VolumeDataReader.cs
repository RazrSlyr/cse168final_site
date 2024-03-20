using UnityEngine;
using NumSharp;

public class VolumeDataReader
{
    public enum Data
    {
        Skull,
        SkullZScores,
        Rabbit,
    }
    public static int numDataOptions = 3;
    private static float[,,] skullData = null;
    private static float[,,] skullZScoreData = null;
    public static float skullDataThreshold = 0.3f;
    public static float rabbitDataThreshold = 0.3f;
    public static float skullZScoreDataThreshold = 0.15f;
    private static float[,,] rabbitData = null;

    public static float[,,] GetSkullZScoreData()
    {
        if (skullZScoreData != null) return skullZScoreData;
        VolumeData skullDataContainer = VolumeData.GetVolumeDataFromJson(Resources.Load<TextAsset>("Preprocessed/skullDataZScores").text);
        var array = np.array<float>(skullDataContainer.data);
        var reshaped = array.reshape(skullDataContainer.dimensions[0],
            skullDataContainer.dimensions[1],
            skullDataContainer.dimensions[2]);
        reshaped = (reshaped - reshaped.min()) / (reshaped.max() - reshaped.min());
        reshaped = reshaped.swapaxes(0, 1);
        skullZScoreData = (float[,,])reshaped.ToMuliDimArray<float>();
        return skullZScoreData;
    }

    public static float[,,] GetSkullData()
    {
        if (skullData != null) return skullData;
        VolumeData skullDataContainer = VolumeData.GetVolumeDataFromJson(Resources.Load<TextAsset>("Preprocessed/skullData").text);
        var array = np.array<float>(skullDataContainer.data);
        var reshaped = array.reshape(skullDataContainer.dimensions[0],
            skullDataContainer.dimensions[1],
            skullDataContainer.dimensions[2]);
        reshaped = (reshaped - reshaped.min()) / (reshaped.max() - reshaped.min());
        reshaped = reshaped.swapaxes(0, 1);
        skullData = (float[,,])reshaped.ToMuliDimArray<float>();
        return skullData;

    }

    public static float[,,] GetRabbitData()
    {
        if (rabbitData != null) return rabbitData;
        VolumeData rabbitDataContainer = VolumeData.GetVolumeDataFromJson(Resources.Load<TextAsset>("Preprocessed/bunnyData").text);
        var array = np.array<float>(rabbitDataContainer.data);
        var reshaped = array.reshape(rabbitDataContainer.dimensions[0],
            rabbitDataContainer.dimensions[1],
            rabbitDataContainer.dimensions[2]);
        reshaped = (reshaped - reshaped.min()) / (reshaped.max() - reshaped.min());
        reshaped = reshaped.swapaxes(0, 1);
        rabbitData = (float[,,])reshaped.ToMuliDimArray<float>();
        return rabbitData;
    }


}
