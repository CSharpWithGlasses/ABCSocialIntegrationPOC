using ABC.Shared.Interfaces.Repository;
using ABC.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ABC.Integration.Managers
{
    public class BaseManager
    {
        private static bool _AreEmojisLoaded = false;
        public virtual void LoadEmojiDataFromFileResource(string resourceName, IRepository _repository)
        {

            if (!_AreEmojisLoaded && !_repository.GetAll<Shared.Models.Emoji>().Any())
            {
                var emojiJsonDataLst = ReadJsonFromResourceFile<EmojiJsonFile>(resourceName);

                List<Shared.Models.Emoji> emojis = emojiJsonDataLst.Select(item => new Shared.Models.Emoji
                {
                    EmojiId = Guid.NewGuid(),
                    EmojiName = item.name,
                    EmojiShortName = item.short_name,
                    EmojiCategory = item.short_name,
                    EmojiUnified = item.unified,
                    EmojiNonQualified = item.non_qualified
                }).ToList();

                _repository.Insert<Shared.Models.Emoji>(emojis);
                _repository.Save();
                _AreEmojisLoaded = _repository.GetAll<Shared.Models.Emoji>().Any();
            }
        }

        protected virtual List<T> ReadJsonFromResourceFile<T>(string fullyQualifiedResourceFileName)
        {
            var fileResults = new List<T>();

            using (Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fullyQualifiedResourceFileName))
            {
                
                using (StreamReader sReader = new StreamReader(fileStream))
                {
                    var jFileData = sReader.ReadToEnd();
                    fileResults = JsonConvert.DeserializeObject<List<T>>(jFileData);
                }
            }


            return fileResults;
        }
    }
}
