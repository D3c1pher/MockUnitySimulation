using TestTask.NonEditable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestTask.Editable
{
    public class ServerMobsManager
    {
        [field: SerializeField] public MonsterData MonsterData { get; private set; }

        public ServerMobsManager()
        {
            MonsterData = SpawnMonster();
        }

        public MonsterData SpawnMonster()
        {
            var monsterId = Random.Range(1, 1000);
            var monsterType = MonsterNameExtensions.MonsterTypeFromId(monsterId);
            var monsterMaxHealth = Random.Range(50, 201);
            var monsterCurrentHealth = monsterMaxHealth;

            MonsterData = new MonsterData(monsterId, monsterType, monsterMaxHealth, monsterCurrentHealth);

            MonsterData.MonsterDamaged += OnMonsterDamaged;
            MonsterData.MonsterDeath += OnMonsterDied;

            return MonsterData;
        }

        public void SendMonsterToClient()
        {
            if (MonsterData == null)
                return;

            ServerPacketsHandler.SendMonsterSpawn(MonsterData);
        }

        private void OnMonsterDamaged(float healthRatio)
        {
            ServerPacketsHandler.SendMonsterHealthUpdate(MonsterData.MonsterId, healthRatio);
        }

        public void OnMonsterDied()
        {
            MonsterData.MonsterDamaged -= OnMonsterDamaged;
            MonsterData.MonsterDeath -= OnMonsterDied;

            MonsterData = SpawnMonster();

            SendMonsterToClient();
        }
    }
}
