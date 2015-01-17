// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using Moq;
using Xunit;

namespace Microsoft.Data.Entity.Relational.Tests
{
    public class RelationalOptionsExtensionTest
    {
        private const string ConnectionStringKey = "ConnectionString";
        private const string CommandTimeoutKey = "CommandTimeout";

        private const string _connectionString = "Fraggle=Rock";

        [Fact]
        public void Can_set_Connection()
        {
            var optionsExtension = new TestRelationalOptionsExtension();

            Assert.Null(optionsExtension.Connection);

            var connection = Mock.Of<DbConnection>();
            optionsExtension.Connection = connection;

            Assert.Same(connection, optionsExtension.Connection);
        }

        [Fact]
        public void Throws_when_setting_Connection_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => { new TestRelationalOptionsExtension().Connection = null; });
        }

        [Fact]
        public void Can_set_ConnectionString()
        {
            var optionsExtension = new TestRelationalOptionsExtension();

            Assert.Null(optionsExtension.ConnectionString);

            optionsExtension.ConnectionString = _connectionString;

            Assert.Equal(_connectionString, optionsExtension.ConnectionString);
        }

        [Fact]
        public void Throws_when_setting_ConnectionString_to_null()
        {
            Assert.Throws<ArgumentNullException>(() => { new TestRelationalOptionsExtension().ConnectionString = null; });
        }

        [Fact]
        public void Configure_sets_ConnectionString_to_value_specified_in_raw_options()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { ConnectionStringKey, _connectionString } };
            var optionsExtension = new TestRelationalOptionsExtension();

            optionsExtension.Configure(rawOptions);

            Assert.Equal(_connectionString, optionsExtension.ConnectionString);
        }

        [Fact]
        public void Configure_does_not_set_ConnectionString_if_value_already_set()
        {
            const string originalConnectionString = "The=Doozers";
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { ConnectionStringKey, _connectionString } };

            var optionsExtension = new TestRelationalOptionsExtension { ConnectionString = originalConnectionString };

            optionsExtension.Configure(rawOptions);

            Assert.Equal(originalConnectionString, optionsExtension.ConnectionString);
        }


        [Fact]
        public void Configure_does_not_set_ConnectionString_if_not_specified_in_raw_options()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var optionsExtension = new TestRelationalOptionsExtension();

            optionsExtension.Configure(rawOptions);

            Assert.Null(optionsExtension.ConnectionString);
        }

        [Fact]
        public void Can_set_CommandTimeout()
        {
            var optionsExtension = new TestRelationalOptionsExtension();

            Assert.Null(optionsExtension.CommandTimeout);

            optionsExtension.CommandTimeout = 1;

            Assert.Equal(1, optionsExtension.CommandTimeout);
        }

        [Fact]
        public void Throws_if_CommandTimeout_out_of_range()
        {
            Assert.Equal(
                Strings.InvalidCommandTimeout,
                Assert.Throws<ArgumentException>(() => { new TestRelationalOptionsExtension().CommandTimeout = -1; }).Message);
        }

        [Fact]
        public void Configure_sets_CommandTimeout_to_value_specified_in_raw_options()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { CommandTimeoutKey, "1" } };
            var optionsExtension = new TestRelationalOptionsExtension();

            optionsExtension.Configure(rawOptions);

            Assert.Equal(1, optionsExtension.CommandTimeout);
        }

        [Fact]
        public void Configure_does_not_set_CommandTimeout_if_value_already_set()
        {
            var optionsExtension = new TestRelationalOptionsExtension { CommandTimeout = 42 };

            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { CommandTimeoutKey, "1" } };

            optionsExtension.Configure(rawOptions);

            Assert.Equal(42, optionsExtension.CommandTimeout);
        }

        [Fact]
        public void Configure_does_not_set_CommandTimeout_if_not_specified_in_raw_options()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var optionsExtension = new TestRelationalOptionsExtension();

            optionsExtension.Configure(rawOptions);

            Assert.Null(optionsExtension.CommandTimeout);
        }

        [Fact]
        public void Configure_throws_if_CommandTimeout_specified_in_raw_options_is_invalid()
        {
            var rawOptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { CommandTimeoutKey, "one" } };

            Assert.Equal(
                Strings.IntegerConfigurationValueFormatError(CommandTimeoutKey, "one"),
                Assert.Throws<InvalidOperationException>(
                    () => new TestRelationalOptionsExtension().Configure(rawOptions)).Message);
        }

        private class TestRelationalOptionsExtension : RelationalOptionsExtension
        {
            protected override void ApplyServices(EntityServicesBuilder builder)
            {
                throw new NotImplementedException();
            }

            new public void Configure(IReadOnlyDictionary<string, string> rawOptions)
            {
                base.Configure(rawOptions);
            }
        }
    }
}
