using Core.GamePlay;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "PrefabGlobalConfig", menuName = "GlobalConfigs/PrefabGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class PrefabGlobalConfig : GlobalConfig<PrefabGlobalConfig>
{
    [field: SerializeField] public Passenger PassengerPrefab {get; private set;}
}