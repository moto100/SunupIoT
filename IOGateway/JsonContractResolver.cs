// <copyright file="JsonContractResolver.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.IOGateway
{
    using System.Reflection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// JsonContractResolver.
    /// </summary>
    public class JsonContractResolver : DefaultContractResolver
    {
        ////protected override string ResolvePropertyName(string propertyName) => propertyName?.ToLower();

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (member.MemberType == MemberTypes.Property)
            {
                var method = ((PropertyInfo)member).GetGetMethod();
                if (method.IsVirtual && !method.IsFinal)
                {
                    return null;
                }
            }

            return base.CreateProperty(member, memberSerialization);
        }
    }
}
