using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestTask.NonEditable
{
    public class ServerColors
    {
        [field: SerializeField] public List<Color> GeneratedColors { get; private set; } = new List<Color>();

        private int _minColorCount = 20;
        private int _maxColorCount = 1000;

        private int _colorCount = 5;

        public ServerColors()
        {
            _colorCount = Random.Range(_minColorCount, _maxColorCount);
            GenerateColors();
        }
        private void GenerateColors()
        {
            GeneratedColors.Clear();
            for (int i = 0; i < _colorCount; i++)
            {
                Color newColor = new Color(Random.value, Random.value, Random.value, 1f);
                GeneratedColors.Add(newColor);
            }
        }

        public IEnumerable<Color> GetServerColors()
        {
            return GeneratedColors;
        }
    }
}

