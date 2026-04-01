using TestTask.NonEditable;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        [Header("Monster UI References")]
        [SerializeField] private Image monsterImage;
        [SerializeField] private TextMeshProUGUI monsterNameText;
        [SerializeField] private Slider monsterHpBar;

        [Header("Monster Sprite References")]
        [SerializeField] private Sprite[] monsterSprites;

        private MonsterData currentMonster;

        public void OnMonsterSpawn(MonsterData monster)
        {
            currentMonster = monster;

            Debug.Log($"Monster spawned: ID={monster.MonsterId}, Type={monster.MonsterType}, HP={monster.MonsterCurrentHealth}/{monster.MonsterMaxHealth}");

            InitializeMonsterUI();
        }

        public void DamageMonster()
        {
            if (currentMonster == null)
                return;

            float damage = Mathf.Round(Random.Range(20f, 50f));
            ClientPacketsHandler.SendDamageRequest(currentMonster.MonsterId, damage);
        }

        public void OnMonsterHealthUpdate(float healthRatio) =>
            monsterHpBar.value = healthRatio;

        private void InitializeMonsterUI()
        {
            monsterImage.sprite = monsterSprites[(int)currentMonster.MonsterType];
            monsterNameText.text = currentMonster.MonsterName;
            monsterHpBar.value = 1;
        }
    }
}
