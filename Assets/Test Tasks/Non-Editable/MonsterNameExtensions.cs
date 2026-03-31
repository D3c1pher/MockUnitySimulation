namespace TestTask.NonEditable
{
    public static class MonsterNameExtensions
    {
        public static string ToFriendlyString(this MonsterNames monsterName)
        {
            return monsterName.ToString();
        }

        public static MonsterNames MonsterTypeFromId(int id)
        {
            var monsterTypeNumber = id % System.Enum.GetValues(typeof(MonsterNames)).Length;
            return (MonsterNames)monsterTypeNumber;
        }

        public static string MonsterNameFromId(int id)
        {
            var monsterNameNumber = id % System.Enum.GetValues(typeof(MonsterNames)).Length;
            return ((MonsterNames)monsterNameNumber).ToFriendlyString();
        }

    }

    public enum MonsterNames
    {
        Goblin,
        Troll,
        Dragon,
        Skeleton,
        Orc
    }
}
