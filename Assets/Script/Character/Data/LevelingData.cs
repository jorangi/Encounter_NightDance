namespace Encounter.NightDance.Core.Datas
{
    public class LevelingData
    {
        public int Level { get; private set; }
        public Percentage Experience { get; private set; }
        public int SP { get; private set; }
        public LevelingData(int level, Percentage experience, int sp)
        {
            Level = level;
            Experience = experience;
            SP = sp;
        }
    }
}