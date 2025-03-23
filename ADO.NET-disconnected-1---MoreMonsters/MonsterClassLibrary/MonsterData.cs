using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterClassLibrary
{
    public static class MonsterData
    {
        private const string FileNameMonster = "cleand_monsters_complete.csv";

        private static DataTable _monsterDataTable;
        public static DataTable MonsterDataTable { get { return _monsterDataTable; } }

        private static DataTable _legendaryMonsterDataTable;
        public static DataTable LegendaryMonsterDataTable { get { return _legendaryMonsterDataTable; } }

        private static DataTable _flyingMonsterDataTable;
        public static DataTable FlyingMonsterDataTable { get { return _flyingMonsterDataTable; } }

        public static void CreateMonsterDataTable()
        {
            // 1. CSV naar DataTable
            
            // 3. Verrijk de data met de "flying" en "legendary" kolom.
            
            // 2. DataTable filteren
            
        }
        public static void CreateLegendaryMonsterDataTable()
        {
            
        }
        public static void CreateFlyingMonsterDataTable()
        {
            
        }
    }
}
