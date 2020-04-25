// <copyright file="BlankSpanTest.cs" company="OpenTelemetry Authors">
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
using System.Collections.Generic;
using Xunit;

namespace OpenTelemetry.Trace.Test
{
    public class BlankSpanTest
    {
        [Fact]
        public void DoNotCrash()
        {
            IDictionary<string, object> attributes = new Dictionary<string, object>();
            attributes.Add(
                "MyStringAttributeKey", "MyStringAttributeValue");
            IDictionary<string, object> multipleAttributes = new Dictionary<string, object>();
            multipleAttributes.Add(
                "MyStringAttributeKey", "MyStringAttributeValue");
            multipleAttributes.Add("MyBooleanAttributeKey", true);
            multipleAttributes.Add("MyLongAttributeKey", 123);
            multipleAttributes.Add("MyDoubleAttributeKey", 0.005);
            // Tests only that all the methods are not crashing/throwing errors.
            NoopSpan.Instance.SetAttribute(
                "MyStringAttributeKey2", "MyStringAttributeValue2");
            foreach (var a in attributes)
            {
                NoopSpan.Instance.SetAttribute(a.Key, a.Value);
            }

            foreach (var a in multipleAttributes)
            {
                NoopSpan.Instance.SetAttribute(a.Key, a.Value);
            }

            NoopSpan.Instance.AddEvent("MyEvent");
            NoopSpan.Instance.AddEvent(new Event("MyEvent", attributes));
            NoopSpan.Instance.AddEvent(new Event("MyEvent", multipleAttributes));
            NoopSpan.Instance.AddEvent(new Event("MyEvent"));

            Assert.False(NoopSpan.Instance.Context.IsValid);
            Assert.False(NoopSpan.Instance.IsRecording);

            NoopSpan.Instance.Status = Status.Ok;
            NoopSpan.Instance.End();
        }

        [Fact]
        public void BadArguments_DoesNotThrow()
        {
            NoopSpan.Instance.Status = new Status();
            NoopSpan.Instance.UpdateName(null);
            NoopSpan.Instance.SetAttribute(null, string.Empty);
            NoopSpan.Instance.SetAttribute(string.Empty, null);
            NoopSpan.Instance.SetAttribute(null, "foo");
            NoopSpan.Instance.SetAttribute(null, 1L);
            NoopSpan.Instance.SetAttribute(null, 0.1d);
            NoopSpan.Instance.SetAttribute(null, true);
            NoopSpan.Instance.AddEvent((string)null);
            NoopSpan.Instance.AddEvent((Event)null);
        }
    }
}
