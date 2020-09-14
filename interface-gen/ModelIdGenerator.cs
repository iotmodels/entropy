using System.Threading;
using System.ComponentModel;
using System;
using System.Linq;
using IoTModels.Resolvers;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure.DigitalTwins.Parser;

namespace DtdlInterfaceGenerator
{
    /// <summary>
    /// Class to generate unique model ids for interfaces
    /// </summary>
    public class ModelIdGenerator
    {
        private static ModelMetadata modelMetadata = ModelMetadata.DeserializeFromFile();
        private static int countDomains = modelMetadata.topLevelDomains.Count();
        private static int countCompanies = modelMetadata.companyNames.Count();
        private static int countProducts = modelMetadata.productNames.Count();

        private static Random random = new Random();
        private IResolver resolver;

        /// <summary>
        /// Fully qualified DTMI including version
        /// </summary>
        /// <value>DTMI with version</value>
        public string AbsoluteModelId { get; private set; }

        /// <summary>
        /// Name of the product for this generated ID
        /// </summary>
        /// <value>name of the product for the interface</value>
        public string ProductName { get; private set; }

        /// <summary>
        /// Static method to create a ModelIdGenerator class
        /// </summary>
        /// <param name="resolver">Resolver to use for checking duplicate DTMIs</param>
        /// <returns>newly created ModelIdGenerator class</returns>
        public static async Task<ModelIdGenerator> GetModelGenerator(IResolver resolver)
        {
            ModelIdGenerator mGen = new ModelIdGenerator();
            mGen.resolver = resolver;
            await mGen.GetModelId();
            return mGen;
        }

        private async Task GetModelId()
        {
            string product = RandomProduct();
            string model = $"dtmi:{RandomDomain()}:{RandomCompany()}:{product}";
            AbsoluteModelId = await GetUniqueAbsoluteId(model);
            ProductName = product.First().ToString().ToUpper() + product.Substring(1);
            return;
        }

        private async Task<string> GetUniqueAbsoluteId(string modelBaseId)
        {
            IEnumerable<string> result = null;
            int version = 1;
            string absoluteModelid = String.Empty;

            while (result == null || result.Count() > 0)
            {
                absoluteModelid = $"{modelBaseId};{version}";
                result = await resolver.DtmiResolver(new Dtmi[] { new Dtmi($"{modelBaseId};{version}") });
                version++;
            }
            return absoluteModelid;
        }

        private static string RandomDomain()
        {
            return modelMetadata.topLevelDomains[random.Next(0, countDomains)];
        }

        private static string RandomCompany()
        {
            return modelMetadata.companyNames[random.Next(0, countCompanies)];
        }

        private static string RandomProduct()
        {
            return modelMetadata.productNames[random.Next(0, countProducts)];
        }
    }
}