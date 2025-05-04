using Core.GamePlay;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "ColorGlobalConfig", menuName = "GlobalConfigs/ColorGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ColorGlobalConfig : GlobalConfig<ColorGlobalConfig>
{
    [field: SerializeField] private ColorConfig[] ColorConfigs {get; set;}
    
    public bool TryGetColorConfig(ColorType colorType, out ColorConfig colorConfig)
    {
        colorConfig = null;
        foreach (ColorConfig config in ColorConfigs)
        {
            if (config.ColorType != colorType) continue;
            colorConfig = config;
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class ColorConfig
{
    public ColorType ColorType;
    public Material Material;
}