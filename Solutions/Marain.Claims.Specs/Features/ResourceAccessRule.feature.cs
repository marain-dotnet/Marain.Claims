﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Marain.Claims.Specs.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("ResourceAccessRule")]
    public partial class ResourceAccessRuleFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "ResourceAccessRule.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "ResourceAccessRule", null, ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rules with identical properties are equal")]
        public void ResourceAccessRulesWithIdenticalPropertiesAreEqual()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rules with identical properties are equal", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 3
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 4
 testRunner.Given("two resource access rules have identical properties", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 5
 testRunner.When("the resource access rules are compared", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 6
 testRunner.Then("the resource access rule comparison result should be true", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rules with differing access types are not equal")]
        public void ResourceAccessRulesWithDifferingAccessTypesAreNotEqual()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rules with differing access types are not equal", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 8
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 9
 testRunner.Given("two resource access rules have differing access types", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 10
 testRunner.When("the resource access rules are compared", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 11
 testRunner.Then("the resource access rule comparison result should be false", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rules with differing resources are not equal")]
        public void ResourceAccessRulesWithDifferingResourcesAreNotEqual()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rules with differing resources are not equal", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 13
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 14
 testRunner.Given("two resource access rules have differing resources", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 15
 testRunner.When("the resource access rules are compared", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 16
 testRunner.Then("the resource access rule comparison result should be false", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rules with differing permissions are not equal")]
        public void ResourceAccessRulesWithDifferingPermissionsAreNotEqual()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rules with differing permissions are not equal", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 18
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 19
 testRunner.Given("two resource access rules have differing permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 20
 testRunner.When("the resource access rules are compared", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 21
 testRunner.Then("the resource access rule comparison result should be false", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rule matching without patterns")]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Allow", "foo", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("FOO", "Foo", "get", "Allow", "foo", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Allow", "FOO", "get", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Deny", "foo", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Allow", "bar", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Allow", "foo", "PUT", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Allow", "foo/bar", "PUT", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET", "Allow", "foo/bar", "GET/a", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo/bar", "Foo", "GET", "Allow", "foo", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET/a", "Allow", "foo", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("pages/home", "Home page", "Edit", "Allow", "pages/home", "Edit", "true", null)]
        [NUnit.Framework.TestCaseAttribute("pages/home", "Home page", "Read", "Allow", "pages/home", "Edit", "false", null)]
        [NUnit.Framework.TestCaseAttribute("pages/home", "Home page", "Read", "Allow", "pages/admin", "Read", "false", null)]
        public void ResourceAccessRuleMatchingWithoutPatterns(string resourceName, string resourceDisplayName, string accessType, string permission, string targetResourceName, string targetAccessType, string result, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("resourceName", resourceName);
            argumentsOfScenario.Add("resourceDisplayName", resourceDisplayName);
            argumentsOfScenario.Add("accessType", accessType);
            argumentsOfScenario.Add("permission", permission);
            argumentsOfScenario.Add("targetResourceName", targetResourceName);
            argumentsOfScenario.Add("targetAccessType", targetAccessType);
            argumentsOfScenario.Add("result", result);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rule matching without patterns", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 23
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 24
 testRunner.Given(string.Format("I have a resource access rule for a resource with name \'{0}\' and display name \'{1" +
                            "}\', with an access type \'{2}\', and permission \'{3}\'", resourceName, resourceDisplayName, accessType, permission), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 25
 testRunner.When(string.Format("I check if the resource access rule is a match for a target with resource name \'{" +
                            "0}\' and target \'{1}\'", targetResourceName, targetAccessType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 26
 testRunner.Then(string.Format("the match result should be \'{0}\'", result), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rule matching with access type patterns")]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "*", "Allow", "foo", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "*", "Allow", "foo", "GET/a", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "**", "Allow", "foo", "GET/a", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET/*", "Allow", "foo", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET/*", "Allow", "foo", "GET/a", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET/*", "Allow", "foo", "GET/a/b", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo", "Foo", "GET/**", "Allow", "foo", "GET/a/b", "true", null)]
        [NUnit.Framework.TestCaseAttribute("books/**", "All books", "Read", "Allow", "books/1984", "Read", "true", null)]
        [NUnit.Framework.TestCaseAttribute("books/**", "All books", "Read", "Allow", "books/1984", "Update", "false", null)]
        [NUnit.Framework.TestCaseAttribute("books/**", "All books", "**", "Allow", "books/1984", "Update", "true", null)]
        public void ResourceAccessRuleMatchingWithAccessTypePatterns(string resourceName, string resourceDisplayName, string accessType, string permission, string targetResourceName, string targetAccessType, string result, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("resourceName", resourceName);
            argumentsOfScenario.Add("resourceDisplayName", resourceDisplayName);
            argumentsOfScenario.Add("accessType", accessType);
            argumentsOfScenario.Add("permission", permission);
            argumentsOfScenario.Add("targetResourceName", targetResourceName);
            argumentsOfScenario.Add("targetAccessType", targetAccessType);
            argumentsOfScenario.Add("result", result);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rule matching with access type patterns", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 45
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 46
 testRunner.Given(string.Format("I have a resource access rule for a resource with name \'{0}\' and display name \'{1" +
                            "}\', with an access type \'{2}\', and permission \'{3}\'", resourceName, resourceDisplayName, accessType, permission), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 47
 testRunner.When(string.Format("I check if the resource access rule is a match for a target with resource name \'{" +
                            "0}\' and target \'{1}\'", targetResourceName, targetAccessType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 48
 testRunner.Then(string.Format("the match result should be \'{0}\'", result), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Resource access rule matching with resource name patterns")]
        [NUnit.Framework.TestCaseAttribute("*", "All", "GET", "Allow", "foo", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("*", "All", "GET", "Allow", "foo/bar", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("**", "All", "GET", "Allow", "foo/bar", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*", "All", "GET", "Allow", "foo", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*", "All", "GET", "Allow", "foo/bar", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*", "All", "GET", "Allow", "foo/bar/baz", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo/**", "All", "GET", "Allow", "foo/bar/baz", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*/quux", "All", "GET", "Allow", "foo/bar/quux", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*/quux", "All", "GET", "Allow", "foo/123/quux", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*/quux", "All", "GET", "Allow", "foo/quux", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("foo/*/quux", "All", "GET", "Allow", "foo/123/baz", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("aul/api/cases/*/quotes/*/underwriting", "All", "GET", "Allow", "foo/123/baz", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("aul/api/cases/*/quotes/*/underwriting", "All", "GET", "Allow", "aul/api/cases/1/quotes/2/underwriting", "GET", "true", null)]
        [NUnit.Framework.TestCaseAttribute("aul/api/cases/*/quotes/*/binding-quote-*/actuarial-review/*", "All", "GET", "Allow", "aul/api/cases/1/quotes/2/underwriting", "GET", "false", null)]
        [NUnit.Framework.TestCaseAttribute("aul/api/cases/*/quotes/*/binding-quote-*/actuarial-review/*", "All", "GET", "Allow", "aul/api/cases/1/quotes/2/binding-quote-v1/actuarial-review/something", "GET", "true", null)]
        public void ResourceAccessRuleMatchingWithResourceNamePatterns(string resourceName, string resourceDisplayName, string accessType, string permission, string targetResourceName, string targetAccessType, string result, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("resourceName", resourceName);
            argumentsOfScenario.Add("resourceDisplayName", resourceDisplayName);
            argumentsOfScenario.Add("accessType", accessType);
            argumentsOfScenario.Add("permission", permission);
            argumentsOfScenario.Add("targetResourceName", targetResourceName);
            argumentsOfScenario.Add("targetAccessType", targetAccessType);
            argumentsOfScenario.Add("result", result);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Resource access rule matching with resource name patterns", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 64
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 65
 testRunner.Given(string.Format("I have a resource access rule for a resource with name \'{0}\' and display name \'{1" +
                            "}\', with an access type \'{2}\', and permission \'{3}\'", resourceName, resourceDisplayName, accessType, permission), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 66
 testRunner.When(string.Format("I check if the resource access rule is a match for a target with resource name \'{" +
                            "0}\' and target \'{1}\'", targetResourceName, targetAccessType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 67
 testRunner.Then(string.Format("the match result should be \'{0}\'", result), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
