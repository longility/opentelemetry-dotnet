// <copyright file="TracerFactoryBaseTests.cs" company="OpenTelemetry Authors">
// Copyright 2018, OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using Xunit;

namespace OpenTelemetry.Tests.Impl.Trace.Config
{
    public class TracerFactoryBaseTests : IDisposable
    {
        public TracerFactoryBaseTests()
        {
            TracerProvider.Default.Reset();
        }

        [Fact]
        public void TraceFactory_Default()
        {
            Assert.NotNull(TracerProvider.Default);
            var defaultTracer = TracerProvider.Default.GetTracer("");
            Assert.NotNull(defaultTracer);
            Assert.Same(defaultTracer, TracerProvider.Default.GetTracer("named tracerSdk"));

            var span = defaultTracer.StartSpan("foo");
            Assert.IsType<NoopSpan>(span);
        }

        [Fact]
        public void TraceFactory_SetDefault()
        {
            var factory = TracerProviderSdk.Create(b => { });
            TracerProvider.SetDefault(factory);

            var defaultTracer = TracerProvider.Default.GetTracer("");
            Assert.NotNull(defaultTracer);
            Assert.IsType<TracerSdk>(defaultTracer);

            Assert.NotSame(defaultTracer, TracerProvider.Default.GetTracer("named tracerSdk"));

            var span = defaultTracer.StartSpan("foo");
            Assert.IsType<SpanSdk>(span);
        }

        [Fact]
        public void TraceFactory_SetDefaultNull()
        {
            Assert.Throws<ArgumentNullException>(() => TracerProvider.SetDefault(null));
        }

        [Fact]
        public void TraceFactory_SetDefaultTwice_Throws()
        {
            TracerProvider.SetDefault(TracerProviderSdk.Create(b => { }));
            Assert.Throws<InvalidOperationException>(() => TracerProvider.SetDefault(TracerProviderSdk.Create(b => { })));
        }

        [Fact]
        public void TraceFactory_UpdateDefault_CachedTracer()
        {
            var defaultTracer = TracerProvider.Default.GetTracer("");
            var noopSpan = defaultTracer.StartSpan("foo");
            Assert.IsType<NoopSpan>(noopSpan);

            TracerProvider.SetDefault(TracerProviderSdk.Create(b => { }));
            var span = defaultTracer.StartSpan("foo");
            Assert.IsType<SpanSdk>(span);

            var newDefaultTracer = TracerProvider.Default.GetTracer("");
            Assert.NotSame(defaultTracer, newDefaultTracer);
            Assert.IsType<TracerSdk>(newDefaultTracer);
        }

        public void Dispose()
        {
            TracerProvider.Default.Reset();
        }
    }
}
