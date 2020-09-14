using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class for managing the list of interfaces which are known and may
    /// be used as compionents when generating new interfaces
    /// </summary>
    public class KnownInterfaces
    {
        private const string FILE_NAME_KI = "known-interfaces.json";
        private const int SAVE_INTERVAL = 10;
        private int recordsSinceSave = 0;
        private string filePath;
        private HashSet<string> items = new HashSet<string>();
        private KnownInterfaces(string repoRoot)
        {
            filePath = Path.Join(repoRoot, FILE_NAME_KI);
        }

        private bool shouldSaveState
        {
            get
            {
                if (recordsSinceSave > SAVE_INTERVAL)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Set of interfaces DTMIs in the collection
        /// </summary>
        /// <value>collection of DTMI as strings</value>
        public IReadOnlyCollection<string> Items
        {
            get
            {
                return items;
            }
        }
        
        /// <summary>
        /// Static method for creating a KnownInterfaces class
        /// </summary>
        /// <param name="repoRoot">root directory for the repo. known-interfaces.json will be saved here </param>
        /// <returns>Newly created KnownInterfaces class</returns>
        public static async Task<KnownInterfaces> GetKnownInterfaces(string repoRoot)
        {
            var ki = new KnownInterfaces(repoRoot);

            if (File.Exists(ki.filePath))
            {
                using (FileStream fs = File.OpenRead(ki.filePath))
                {
                    ki.items = await JsonSerializer.DeserializeAsync<HashSet<string>>(fs);
                }
            }
            return ki;
        }

        /// <summary>
        /// Add new interface to the list of known interfaces
        /// </summary>
        /// <param name="item">DTMI of the item to add</param>
        /// <returns>true if add was successful false otherwise</returns>
        public async Task<bool> Add(string item)
        {
            bool addSuccess = items.Add(item);
            recordsSinceSave++;

            if (shouldSaveState)
            {
                await serialize();
                recordsSinceSave = 0;
            }

            return addSuccess;
        }

        /// <summary>
        /// Persists the list of interfaces to the root of the repo
        /// </summary>
        /// <returns>Task</returns>
        public async Task SaveItems()
        {
            if (recordsSinceSave > 0)
            {
                await serialize();
            }
            return;
        }

        private async Task serialize()
        {
            using (FileStream fs = File.Create(filePath))
            {
                await JsonSerializer.SerializeAsync<HashSet<string>>(fs, items);
            }
        }
    }
}