using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class UIColorDisplay : MonoBehaviour
    {
        [Header("Color UI References")]
        [SerializeField] private Transform colorContainer;
        [SerializeField] private GameObject colorPrefab;

        private void OnEnable()
        {
            ClientColors.OnColorsReceived += InitializeColorUI;
        }

        private void OnDisable()
        {
            ClientColors.OnColorsReceived -= InitializeColorUI;
        }

        private void InitializeColorUI(List<Color> colors)
        {
            foreach (Color color in colors)
            {
                GameObject colorObject = Instantiate(colorPrefab, colorContainer);
                colorObject.GetComponent<Image>().color = color;
            }
        }
    }
}
