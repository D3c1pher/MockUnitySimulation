using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        private MonsterData currentMonster;

        public void OnMonsterSpawn(MonsterData monster)
        {
            currentMonster = monster;

            Debug.Log($"Monster spawned: ID={monster.MonsterId}, Type={monster.MonsterType}, HP={monster.MonsterCurrentHealth}/{monster.MonsterMaxHealth}");

            currentMonster.MonsterDeath += OnMonsterDeath;
        }

        private void OnMonsterDeath()
        {
            Debug.Log($"Monster died: ID={currentMonster.MonsterId}, Type={currentMonster.MonsterType}");

            currentMonster.MonsterDeath -= OnMonsterDeath;

            currentMonster = null;
        }
    }       
}
