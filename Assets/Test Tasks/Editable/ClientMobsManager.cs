using TestTask.NonEditable;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        [SerializeField] private Image monsterImage;
        [SerializeField] private TextMeshProUGUI monsterNameText;
        [SerializeField] private Slider monsterHpBar;

        [SerializeField] private Sprite[] monsterSprites;

        private MonsterData currentMonster;

        public void OnMonsterSpawn(MonsterData monster)
        {
            currentMonster = monster;

            Debug.Log($"Monster spawned: ID={monster.MonsterId}, Type={monster.MonsterType}, HP={monster.MonsterCurrentHealth}/{monster.MonsterMaxHealth}");

            UpdateMonsterUI();

            currentMonster.MonsterDamaged += OnMonsterDamaged;
            currentMonster.MonsterDeath += OnMonsterDeath;
        }

        private void UpdateMonsterUI()
        {
            monsterImage.sprite = monsterSprites[(int)currentMonster.MonsterType];

            monsterNameText.text = currentMonster.MonsterName;

            monsterHpBar.minValue = 0f;
            monsterHpBar.maxValue = 1f;
            monsterHpBar.value = currentMonster.MonsterCurrentHealth / currentMonster.MonsterMaxHealth;
        }

        private void OnMonsterDamaged(float hpRatio)
        {
            monsterHpBar.value = hpRatio;
        }

        private void OnMonsterDeath()
        {
            Debug.Log($"Monster died: ID={currentMonster.MonsterId}, Type={currentMonster.MonsterType}");

            currentMonster.MonsterDamaged -= OnMonsterDamaged;
            currentMonster.MonsterDeath -= OnMonsterDeath;

            monsterImage.sprite = null;
            monsterNameText.text = string.Empty;
            monsterHpBar.value = 0f;

            currentMonster = null;
        }
    }       
}
