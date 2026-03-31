using System;
using UnityEngine;
using UnityEngine.Events;

namespace TestTask.NonEditable
{
    [System.Serializable]
    public class MonsterData
    {
        [field: SerializeField] public int MonsterId { get; private set; }
        [field: SerializeField] public MonsterNames MonsterType { get; private set; }
        [field: SerializeField] public string MonsterName { get; private set; }
        [field: SerializeField] public float MonsterMaxHealth { get; private set; }
        [field: SerializeField] public float MonsterCurrentHealth { get; private set; }

        public event Action MonsterDeath;
        public event Action<float> MonsterDamaged;

        public MonsterData(int monsterId, MonsterNames monsterType, float monsterMaxHealth, float monsterCurrentHealth)
        {
            MonsterId = monsterId;
            MonsterType = monsterType;
            MonsterName = MonsterType.ToFriendlyString();
            MonsterMaxHealth = monsterMaxHealth;
            MonsterCurrentHealth = monsterCurrentHealth;
        }

        public void TakeDamage(float damage)
        {
            MonsterCurrentHealth -= damage;
            MonsterDamaged?.Invoke(MonsterCurrentHealth/MonsterMaxHealth);

            if (MonsterCurrentHealth <= 0)
                MonsterDeath?.Invoke();
        }
    }
}
