using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace MonsterClassLibrary
{
    public static class CreateMonsterData
    {
        private static DataTable _monsterDataTable = new DataTable("Monsters");
        private static bool _isImageDownloadingActive = false;
        private static HttpClient client = new HttpClient();
       
        private const string InputFileNameMonsters = "cleaned_monsters_basic.csv";
        private const string OutputFileNameMonsters = "cleand_monsters_complete.csv";
        private const string ImageOutputDirectoryName = "images";
        private const string UrlToScrapeMonsterFrom = @"https://www.aidedd.org/dnd/monstres.php?vo=";
        private const string ImageSourceColumnName = "image_source";
        private const string ImageNameColumnName = "image_name";
        
        static CreateMonsterData()
        {

        }

        public static async Task ScrapeMonstersToDataTableAsync()
        {
            using (StreamReader sr = new StreamReader(InputFileNameMonsters))
            {
                // Leest: id,name,size,monster_type,alignment,ac,hp,strength,str_mod,dex,dex_mod,con,con_mod,intel,int_mod,wis,wis_mod,cha,cha_mod,senses,languages,cr,str_save,dex_save,con_save,int_save,wis_save,cha_save,speed,swim,fly,climb,burrow,number_legendary_actions,history,perception,stealth,persuasion,insight,deception,arcana,religion,acrobatics,athletics,intimidation
                string[] headers = sr.ReadLine().Split(',');

                foreach (string header in headers)
                {
                    _monsterDataTable.Columns.Add(header);
                }
                _monsterDataTable.Columns.Add(ImageSourceColumnName);
                _monsterDataTable.Columns.Add(ImageNameColumnName);

                while (!sr.EndOfStream)
                {
                    string? line = sr.ReadLine();
                    if (line == null) continue;

                    string[] values = SplitAndIgnoreQuotedComma(line);

                    string monsterName = values[1];
                    string monsterUrl = BuildUrl(monsterName);

                    string monsterImageSource = await ScrapeImageSource(monsterUrl);

                    if (_isImageDownloadingActive)
                    {
                        await ScrapeImage(monsterImageSource, monsterName);
                    }

                    DataRow row = _monsterDataTable.NewRow();
                    for (int i = 0; i < values.Length; i++)
                    {
                        row[i] = values[i];
                    }
                    row[ImageSourceColumnName] = monsterImageSource;
                    row[ImageNameColumnName] = monsterImageSource.Split('/').Last();
                    _monsterDataTable.Rows.Add(row);
                }
            }
        }

        private static string[] SplitAndIgnoreQuotedComma(string quotedCommaCsv)
        {
            bool isQuoted = false;
            List<string> values = new();
            StringBuilder currentValue = new();
            foreach (char c in quotedCommaCsv)
            {
                if (c == '\"')
                {
                    isQuoted = !isQuoted;
                }
                else if (c == ',' && !isQuoted)
                {
                    values.Add(currentValue.ToString());
                    currentValue = new();
                }
                else
                {
                    currentValue.Append(c);
                }
            }
            values.Add(currentValue.ToString());
            return values.ToArray();
        }

        public static void WriteDataTableToCsv()
        {
            using (StreamWriter sw = new StreamWriter(OutputFileNameMonsters))
            {
                sw.WriteLine(string.Join(";", _monsterDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));

                foreach (DataRow row in _monsterDataTable.Rows)
                {
                    var fields = row.ItemArray.Select(f => f.ToString()?.Replace("\"", ""));
                    sw.WriteLine(string.Join(";", fields));
                }
            }
        }

        private static async Task<string> ScrapeImageSource(string monsterUrl)
        {
            try
            {
                using HttpResponseMessage response = await client.GetAsync(monsterUrl);
                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();

                Match match = Regex.Match(html, @"<div class='picture'>\s*<img\s+src='([^']+)'", RegexOptions.IgnoreCase);
                // LET OP: niet alle monsters hebben een image of de site
                return match.Success ? match.Groups[1].Value : ""; // geef lege string, indien er geen image is
            }
            catch (Exception ex)
            {
                throw new Exception($"Error scraping {monsterUrl}: {ex.Message}");
            }
        }

        private static async Task ScrapeImage(string imageSource, string monsterName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageSource))
                    return;

                Directory.CreateDirectory(ImageOutputDirectoryName);

                string fileExtension = Path.GetExtension(imageSource);
                string safeName = monsterName.Replace(" ", "_").ToLower();
                string fileName = $"{safeName}{fileExtension}";
                string filePath = Path.Combine(ImageOutputDirectoryName, fileName);

                using HttpResponseMessage response = await client.GetAsync(imageSource);
                response.EnsureSuccessStatusCode();

                await using var fs = new FileStream(filePath, FileMode.Create);
                await response.Content.CopyToAsync(fs);

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download image {imageSource}: {ex.Message}");
            }
        }

        static string BuildUrl(string monsterName)
        {
            return UrlToScrapeMonsterFrom + monsterName.ToLower().Replace(" ", "-");
        }
    }
}
