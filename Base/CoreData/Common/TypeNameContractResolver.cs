using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoreData.Common
{
    public class TypeNameContractResolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            if (contract is JsonContainerContract containerContract)
                containerContract.ItemTypeNameHandling ??= TypeNameHandling.None;

            return contract;
        }
    }
}