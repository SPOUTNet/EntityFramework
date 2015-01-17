// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Utilities;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.Data.Entity.SqlServer
{
    public class SqlServerOptionsExtension : RelationalOptionsExtension
    {
        private const string ProviderPrefix = "SqlServer";
        private const string MaxBatchSizeKey = "MaxBatchSize";

        private int? _maxBatchSize;

        public virtual int? MaxBatchSize
        {
            get { return _maxBatchSize; }

            [param:CanBeNull]
            set
            {
                if (value <= 0)
                {
                    throw new InvalidOperationException(Strings.MaxBatchSizeMustBePositive);
                }
                _maxBatchSize = value;
            }
        }

        protected override void Configure(IReadOnlyDictionary<string, string> rawOptions)
        {
            base.Configure(rawOptions);
            if (!_maxBatchSize.HasValue)
            {
                var maxBatchSizeConfigurationKey = ProviderPrefix + Constants.KeyDelimiter + MaxBatchSizeKey;
                string maxBatchSizeString;
                if (rawOptions.TryGetValue(maxBatchSizeConfigurationKey, out maxBatchSizeString))
                {
                    int maxBatchSizeInt;
                    if (!Int32.TryParse(maxBatchSizeString, out maxBatchSizeInt))
                    {
                        throw new InvalidOperationException(Strings.IntegerConfigurationValueFormatError(maxBatchSizeConfigurationKey, maxBatchSizeString));
                    }
                    _maxBatchSize = maxBatchSizeInt;
                }
            }
        }

        protected override void ApplyServices(EntityServicesBuilder builder)
        {
            Check.NotNull(builder, "builder");

            builder.AddSqlServer();
        }
    }
}
