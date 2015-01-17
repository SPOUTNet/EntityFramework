// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Microsoft.Data.Entity.SqlServer.Tests
{
    public class SqlServerOptionsExtensionTest
    {
        private const string MaxBatchSizeKey = "SqlServer:MaxBatchSize";

        private static readonly MethodInfo _applyServices
            = typeof(SqlServerOptionsExtension).GetTypeInfo().DeclaredMethods.Single(m => m.Name == "ApplyServices");

        [Fact]
        public void ApplyServices_adds_SQL_server_services()
        {
            var services = new ServiceCollection();
            var builder = new EntityServicesBuilder(services);

            _applyServices.Invoke(new SqlServerOptionsExtension(), new object[] { builder });

            Assert.True(services.Any(sd => sd.ServiceType == typeof(SqlServerDataStore)));
        }

        [Fact]
        public void Can_set_MaxBatchSize()
        {
            var optionsExtension = new SqlServerOptionsExtension();

            Assert.Null(optionsExtension.MaxBatchSize);

            optionsExtension.MaxBatchSize = 1;

            Assert.Equal(1, optionsExtension.MaxBatchSize);
        }

        [Fact]
        public void Throws_if_MaxBatchSize_out_of_range()
        {
            Assert.Equal(
                Strings.MaxBatchSizeMustBePositive,
                Assert.Throws<InvalidOperationException>(() => { new SqlServerOptionsExtension().MaxBatchSize = -1; }).Message);
        }

        [Fact]
        public void Configure_sets_MaxBatchSize_to_value_specified_in_raw_options()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { MaxBatchSizeKey, "1" } };
            var optionsExtension = new SqlServerOptionsExtension();

            optionsExtension.Configure(rawOptions);

            Assert.Equal(1, optionsExtension.MaxBatchSize);
        }

        [Fact]
        public void Configure_does_not_set_MaxBatchSize_if_value_already_set()
        {
            var optionsExtension = new SqlServerOptionsExtension { MaxBatchSize = 42 };

            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { MaxBatchSizeKey, "1" } };

            optionsExtension.Configure(rawOptions);

            Assert.Equal(42, optionsExtension.MaxBatchSize);
        }


        [Fact]
        public void Configure_does_not_set_MaxBatchSize_if_not_specified_in_raw_options()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var optionsExtension = new SqlServerOptionsExtension();

            optionsExtension.Configure(rawOptions);

            Assert.Null(optionsExtension.MaxBatchSize);
        }

        [Fact]
        public void Configure_throws_if_MaxBatchSize_specified_in_raw_options_is_invalid()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { MaxBatchSizeKey, "one" } };

            Assert.Equal(
                Strings.IntegerConfigurationValueFormatError(MaxBatchSizeKey, "one"),
                Assert.Throws<InvalidOperationException>(() => new SqlServerOptionsExtension().Configure(rawOptions)).Message);
        }
    }
}
