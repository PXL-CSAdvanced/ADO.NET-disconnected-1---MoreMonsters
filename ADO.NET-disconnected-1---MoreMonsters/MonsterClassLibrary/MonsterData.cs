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
            _monsterDataTable = new DataTable("Monsters");

            using (StreamReader sr = new StreamReader(FileNameMonster))
            {
                string header = sr.ReadLine();
                foreach (string columnName in header.Split(";"))
                {
                    _monsterDataTable.Columns.Add(columnName);
                }

                while (!sr.EndOfStream)
                {
                    _monsterDataTable.Rows.Add(sr.ReadLine().Split(';'));
                }
            }

            // 3. 
            _monsterDataTable.Columns.Add("flying", typeof(bool));
            _monsterDataTable.Columns.Add("legendary", typeof(bool));
            for (int i = 0; i < _monsterDataTable.Rows.Count; i++)
            {
                _monsterDataTable.Rows[i]["flying"] = Convert.ToInt32(_monsterDataTable.Rows[i]["fly"]) > 0;
                _monsterDataTable.Rows[i]["legendary"] = Convert.ToInt32(_monsterDataTable.Rows[i]["number_legendary_actions"]) > 0;
            }


            // 2. DataTable filteren
            string allHeadersText = "id;name;size;monster_type;alignment;ac;hp;strength;str_mod;dex;dex_mod;con;con_mod;intel;int_mod;wis;wis_mod;cha;cha_mod;senses;languages;cr;str_save;dex_save;con_save;int_save;wis_save;cha_save;speed;swim;fly;climb;burrow;number_legendary_actions;history;perception;stealth;persuasion;insight;deception;arcana;religion;acrobatics;athletics;intimidation;image_source;image_name";
            string wantedHeaders = "id;name;size;monster_type;alignment;ac;hp;strength;dex;con;intel;wis;cha;senses;languages;cr;speed;image_source;image_name;flying;legendary";
            List<string> allHeaders = allHeadersText.Split(';').ToList();
            wantedHeaders.Split(';').ToList().ForEach(x=> allHeaders.Remove(x));

            allHeaders.ForEach(x => _monsterDataTable.Columns.Remove(x));

        }
        public static void CreateLegendaryMonsterDataTable()
        {
            _legendaryMonsterDataTable = _monsterDataTable.Copy();
            _legendaryMonsterDataTable.Rows.Clear();
            foreach (DataRow row in _monsterDataTable.Rows)
            {
                if (Convert.ToBoolean(row["legendary"]))
                {
                    _legendaryMonsterDataTable.Rows.Add(row.ItemArray);
                }
            }
        }
        public static void CreateFlyingMonsterDataTable()
        {
            _flyingMonsterDataTable = _monsterDataTable.Copy();
            _flyingMonsterDataTable.Rows.Clear();
            foreach (DataRow row in _monsterDataTable.Rows)
            {
                if (Convert.ToBoolean(row["flying"]))
                {
                    _flyingMonsterDataTable.Rows.Add(row.ItemArray);
                }
            }
        }
    }
}
