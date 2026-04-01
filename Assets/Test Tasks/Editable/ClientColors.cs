using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class ClientColors : MonoBehaviour
    {
        [SerializeField] private Transform colorListContent;
        [SerializeField] private GameObject colorSquarePrefab;

        private const int PoolSize = 1000;
        private List<Image> colorSquarePool = new List<Image>();
        private List<Color> receivedColors = new List<Color>();

        private void Start()
        {
            PrewarmPool();
        }

        private void PrewarmPool()
        {
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject square = Instantiate(colorSquarePrefab, colorListContent);
                Image image = square.GetComponent<Image>();
                square.SetActive(false);
                colorSquarePool.Add(image);
            }
        }

        public void RequestColors()
        {
            if (ClientManager.Instance.ClientId == 0)
            {
                Debug.LogWarning("Cannot request colors before logging in.");
                return;
            }

            ClientPacketsHandler.SendColorRequest();
        }

        public void OnColorsReceived(List<Color> colors)
        {
            receivedColors = colors;

            Debug.Log($"Colors received: {receivedColors.Count}");

            InitializeColorUI();
        }

        private void InitializeColorUI()
        {
            for (int i = 0; i < colorSquarePool.Count; i++)
            {
                if (i < receivedColors.Count)
                {
                    colorSquarePool[i].color = receivedColors[i];
                    colorSquarePool[i].gameObject.SetActive(true);
                }
                else
                {
                    colorSquarePool[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
