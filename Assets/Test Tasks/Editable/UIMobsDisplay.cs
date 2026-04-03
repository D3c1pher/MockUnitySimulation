using TestTask.NonEditable;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TestTask.Editable
{
    public class UIMobsDisplay : MonoBehaviour
    {
        [Header("Monster UI References")]
        [SerializeField] private Image monsterImage;
        [SerializeField] private TextMeshProUGUI monsterNameText;
        [SerializeField] private Slider monsterHpBar;

        [Header("Monster Sprite References")]
        [SerializeField] private Sprite[] monsterSprites;

        private void OnEnable()
        {
            ClientMobsManager.OnMonsterSpawned += InitializeMonsterUI;
            ClientMobsManager.OnMonsterHealthChanged += UpdateHealthBar;
        }

        private void OnDisable()
        {
            ClientMobsManager.OnMonsterSpawned -= InitializeMonsterUI;
            ClientMobsManager.OnMonsterHealthChanged -= UpdateHealthBar;
        }

        private void InitializeMonsterUI(MonsterData monster)
        {
            monsterImage.sprite = monsterSprites[(int)monster.MonsterType];
            monsterNameText.text = monster.MonsterName;
            monsterHpBar.value = 1f;
        }

        private void UpdateHealthBar(float healthRatio)
        {
            monsterHpBar.value = healthRatio;
        }
    }
}
