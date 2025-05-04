using UnityEditor;
using UnityEngine;

namespace Core.GamePlay
{
    public class PickupCarGraphic : MonoBehaviour
    {
        [field: SerializeField] public MeshRenderer MainGraphic {get; private set;}
        
        public void OnColorChange(ColorType colorType)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
            if (!ColorGlobalConfig.Instance.TryGetColorConfig(colorType, out ColorConfig colorConfig)) return;
            MainGraphic.material = colorConfig.Material;
        }
    }
}