// <copyright file="MultiTestHostBase.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using NUnit.Framework.Interfaces;
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Builders;

    /// <summary>
    /// Base class for tests that need to run for both Functions emulation and direct ASP.NET
    /// pipeline hosting.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Tests can derive from this and must set the following class-level attribute:
    /// </para>
    /// <code><![CDATA[
    /// [MultiHostTest]
    /// ]]></code>
    /// <para>
    /// This needs to be specified on each deriving class, because NUnit does not walk up the
    /// inheritance chain when looking for test fixture sources.
    /// </para>
    /// <para>
    /// Deriving classes should also define a constructor that has the same signature as this
    /// class's constructor, forwarding the argument on. This, in conjunction with the test
    /// fixture source attribute, will cause NUnit to run the fixture for this class multiple
    /// times, once for each of the host types specified in <see cref="FixtureArgs"/>.
    /// </para>
    /// <para>
    /// When using SpecFlow, bindings can detect the mode by casting the reference in
    /// <c>TestExecutionContext.CurrentContext.TestObject</c> to <see cref="IMultiModeTest{TestHostTypes}"/>
    /// and then inspecting the <see cref="IMultiModeTest{TestHostTypes>.TestType"/> property.
    /// </para>
    /// </remarks>
    public class MultiTestHostBase : IMultiModeTest<TestHostModes>
    {
        protected static readonly object[] FixtureArgs =
        {
            new object[] { TestHostModes.DirectInvocation },
            new object[] { TestHostModes.InProcessEmulateFunctionWithActionResult },
            new object[] { TestHostModes.UseFunctionHost },
        };

        /// <summary>
        /// Creates a <see cref="MultiTestHostBase"/>.
        /// </summary>
        /// <param name="testType">
        /// Hosting style to test for.
        /// </param>
        private protected MultiTestHostBase(TestHostModes testType)
        {
            this.TestType = testType;
        }

        /// <inheritdoc />
        public TestHostModes TestType { get; }

        /// <summary>
        /// Attribute to be applied to tests deriving from <see cref="MultiTestHostBase"/> to
        /// create multiple instances of the test, one for each test mode.
        /// </summary>
        /// <remarks>
        /// Annoyingly, applying this to the base type doesn't work. NUnit appears not to support
        /// inheritance of fixture-building test attributes, which is why we can't just slap this
        /// on <see cref="MultiTestHostBase"/> once and for all.
        /// </remarks>
        [AttributeUsage(AttributeTargets.Class)]
        public class MultiHostTestAttribute : Attribute, IFixtureBuilder2
        {
            private readonly NUnitTestFixtureBuilder builder = new();

            public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo, IPreFilter filter)
            {
                // This whole method does more or less the same thing as NUnit's own
                // TestFixtureSourceAttribute, but with two differences:
                //  1) this is hard-coded to use the list of test modes as its input
                //  2) this adds a Category of "fast" when the DirectInvocation mode is used
                // That second one is important for enabling a quick developer loop. Developers
                // can create a test playlist that runs only the "fast" tests, or if they have
                // NCrunch, they can define a custom Engine Mode that automatically runs only
                // fast tests. These "fast" tests can run without spinning up a new functions
                // host. Also, because they do not set up an HTTP listener, they can be executed
                // in parallel without hitting port conflicts.
                var fixtureSuite = new ParameterizedFixtureSuite(typeInfo);
                fixtureSuite.ApplyAttributesToTest(typeInfo.Type.GetTypeInfo());
                ICustomAttributeProvider assemblyLifeCycleAttributeProvider = typeInfo.Type.GetTypeInfo().Assembly;
                ICustomAttributeProvider typeLifeCycleAttributeProvider = typeInfo.Type.GetTypeInfo();

                foreach (object[] args in FixtureArgs)
                {
                    var arg = (TestHostModes)args[0];
                    ITestFixtureData parms = new TestFixtureParameters(new object[] { arg });
                    TestSuite fixture = this.builder.BuildFrom(typeInfo, filter, parms);

                    switch (arg)
                    {
                        case TestHostModes.DirectInvocation:
                            fixture.Properties["Category"].Add("fast");
                            fixture.Properties["Category"].Add("parallelizable");
                            break;

                        case TestHostModes.InProcessEmulateFunctionWithActionResult:
                            fixture.Properties["Category"].Add("fast");
                            break;
                    }

                    fixture.ApplyAttributesToTest(assemblyLifeCycleAttributeProvider);
                    fixture.ApplyAttributesToTest(typeLifeCycleAttributeProvider);
                    fixtureSuite.Add(fixture);
                }

                yield return fixtureSuite;
            }

            public IEnumerable<TestSuite> BuildFrom(ITypeInfo typeInfo)
            {
                return this.BuildFrom(typeInfo, NullPrefilter.Instance);
            }

            private class NullPrefilter : IPreFilter
            {
                public static readonly NullPrefilter Instance = new();

                public bool IsMatch(Type type) => true;

                public bool IsMatch(Type type, MethodInfo method) => true;
            }
        }
    }
}