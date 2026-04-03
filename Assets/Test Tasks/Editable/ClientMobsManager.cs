using System;
using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        public static event Action<MonsterData> OnMonsterSpawned;
        public static event Action<float> OnMonsterHealthChanged;

        private MonsterData currentMonster;

        public void SpawnMonster(MonsterData monster)
        {
            currentMonster = monster;

            Debug.Log($"Monster spawned: ID={monster.MonsterId}, Type={monster.MonsterType}, HP={monster.MonsterCurrentHealth}/{monster.MonsterMaxHealth}");

            OnMonsterSpawned?.Invoke(currentMonster);
        }

        public void DamageMonster()
        {
            if (currentMonster == null)
            {
                Debug.LogWarning("No monster is currently spawned.");
                return;
            }

            float damage = Mathf.Round(UnityEngine.Random.Range(20f, 50f));

            ClientPacketsHandler.SendDamageRequest(currentMonster.MonsterId, damage);
        }

        public void UpdateMonsterHealth(int monsterId, float healthRatio)
        {
            if (currentMonster == null || currentMonster.MonsterId != monsterId)
                return;

            OnMonsterHealthChanged?.Invoke(healthRatio);
        }
    }
}
