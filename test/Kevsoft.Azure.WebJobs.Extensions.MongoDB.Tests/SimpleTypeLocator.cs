using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;

namespace Kevsoft.Azure.WebJobs.Extensions.MongoDB.Tests
{
    internal class SimpleTypeLocator : ITypeLocator
    {
        private readonly Type[] _types;

        public SimpleTypeLocator(params Type[] types)
        {
            _types = types;
        }

        public IReadOnlyList<Type> GetTypes()
        {
            return _types;
        }
    }
}