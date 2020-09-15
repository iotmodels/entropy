using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class to generate the contents of an DtdlV2 Intefaces
    /// </summary>
    public class ContentGenerator
    {
        const int MAX_COMPONENTS = 5;
        const int MAX_TELEMETRIES = 10;
        const int MAX_PROPERTIES = 10;
        Dictionary<string, Content> nodes = new Dictionary<string, Content>();

        private static readonly string[] nodeNames = {
            "temperature", "acceleration", "brightness", "pressure", "tiltAngle", 
            "flowRate", "intensity", "brightness", "differentialPressure",
            "displacement","frequency", "inductance","conductance","angularAcceleration",
            "moisture","speed","mass","tilt","force","viscosity", "wavelength",
            "accuracy","weight","powerRange","radiationLevel","voltage" };

        private static readonly string[] dtdlTypes = {
            "boolean","date", "dateTime", "double", "duration","float",
            "integer", "long", "string", "time"};

        private static int countNodeNames = nodeNames.Length;
        private static int countTypes = dtdlTypes.Length;
        private static Random random = new Random();

        private List<string> usedNames = new List<string>();

        /// <summary>
        /// Generate data for interface Telemetry, Proerty, Commands and Components
        /// </summary>
        /// <param name="knownDtmis">list of possible component schemas to add to the interface</param>
        /// <returns></returns>
        public IEnumerable<Content> GenerateInterfacedata(IEnumerable<string> knownDtmis)
        {
            GenerateTelemetryNodes();
            GeneratePropertyNodes();
            GenerateCommandNodes();
            if (knownDtmis != null && knownDtmis.Count() > 0)
            {
                GenerateComponentNodes(knownDtmis);
            }
            return nodes.Values;
        }
        private void GenerateTelemetryNodes()
        {
            int numNodes = random.Next(0, countNodeNames);

            for (int i = 0; i < MAX_TELEMETRIES; i++)
            {
                GenerateRandomNode("Telemetry");
            }
        }

        private void GeneratePropertyNodes()
        {
            int numNodes = random.Next(0, countNodeNames);

            for (int i = 0; i < MAX_PROPERTIES; i++)
            {
                GenerateRandomNode("Property");
            }
        }

        private void GenerateCommandNodes()
        {
            int numNodes = random.Next(0, countNodeNames);

            for (int i = 0; i < numNodes; i++)
            {
                GenerateRandomCommand();
            }
        }

        private void GenerateComponentNodes(IEnumerable<string> knownDtmis)
        {

            for (int i = 0; i < MAX_COMPONENTS; i++)
            {
                if (random.Next() % 5 == 0)
                {
                    AddRandomComponent(knownDtmis);
                }
            }
        }

        private void AddRandomComponent(IEnumerable<string> knownDtmis)
        {
            int count = knownDtmis.Count();
            Content tc = new Content();
            tc.type = "Component";
            tc.schema = knownDtmis.ElementAt(random.Next(0, count));
            string baseName = tc.schema.Split(":").Last().Split(";").First();
            AddUniqueComponentName(tc, baseName);
        }

        private void AddUniqueComponentName(Content tc, string baseName)
        {
            string componentName = baseName;
            int compVersion = 2;
            while (!nodes.TryAdd(componentName, tc))
            {
                componentName = $"{baseName}{compVersion}";
                compVersion++;
            }
            tc.name = componentName;
            tc.displayName = componentName;
            tc.description = componentName;
        }

        private Content GenerateRandomCommand()
        {
            Content tc = new Content();
            string commandName = RandomTelemetryName();
            commandName = $"set{commandName.First().ToString().ToUpper() + commandName.Substring(1)}";
            string versionName = commandName;
            int dif = 1;
            while (!nodes.TryAdd(versionName, tc))
            {
                dif++;
                versionName = $"{commandName}{dif}";
            }
            tc.name = versionName;
            tc.type = "Command";
            tc.request = new Request()
            {
                name = $"target{versionName}",
                schema = RandomType()
            };
            return tc;
        }

        private Content GenerateRandomNode(string type)
        {
            Content tc = new Content();
            string name = RandomTelemetryName();
            string versionName = name;
            int dif = 1;
            while (!nodes.TryAdd(versionName, tc))
            {
                versionName = $"{name}{dif}";
                dif++;
            }

            tc.name = versionName.Trim();
            tc.type = type.Trim();
            tc.schema = RandomType();
            return tc;
        }

        private string RandomTelemetryName()
        {
            string nextName = "";
            int i = 1;
            while (String.IsNullOrWhiteSpace(nextName) || usedNames.Contains(nextName))
            {
                nextName = nodeNames[random.Next(0, countNodeNames)];
                i++;
                if (i == countNodeNames)
                {
                    nextName = "safety";
                }
            }
            return nextName;
        }

        private string RandomType()
        {
            return dtdlTypes[random.Next(0, countTypes)];
        }
    }
}