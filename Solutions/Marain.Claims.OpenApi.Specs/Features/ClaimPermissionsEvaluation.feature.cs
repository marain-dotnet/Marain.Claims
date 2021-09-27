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
namespace Marain.Claims.OpenApi.Specs.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("ClaimPermissionsEvaluation")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useClaimsApi")]
    public partial class ClaimPermissionsEvaluationFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useClaimsApi"};
        
#line 1 "ClaimPermissionsEvaluation.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "ClaimPermissionsEvaluation", "    Service endpoints that evaluate permissions based on the configured rules", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useClaimsApi"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 7
#line hidden
#line 8
    testRunner.Given("a unique ClaimsPermission id named \'id-none\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 9
    testRunner.And("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 10
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table15 = new TechTalk.SpecFlow.Table(new string[] {
                        "AccessType",
                        "ResourceUri",
                        "ResourceName",
                        "Permission"});
            table15.AddRow(new string[] {
                        "GET",
                        "foo",
                        "Foo",
                        "Allow"});
            table15.AddRow(new string[] {
                        "POST",
                        "foo",
                        "Foo",
                        "Deny"});
            table15.AddRow(new string[] {
                        "GET",
                        "bar",
                        "Bar",
                        "Allow"});
#line 11
    testRunner.And("the new ClaimsPermission has these rules", ((string)(null)), table15, "And ");
#line hidden
#line 16
    testRunner.And("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 17
    testRunner.And("a unique ClaimsPermission id named \'id2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
    testRunner.And("a new ClaimsPermission with id named \'id2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table16 = new TechTalk.SpecFlow.Table(new string[] {
                        "AccessType",
                        "ResourceUri",
                        "ResourceName",
                        "Permission"});
            table16.AddRow(new string[] {
                        "GET",
                        "quux",
                        "Quux",
                        "Deny"});
            table16.AddRow(new string[] {
                        "POST",
                        "spong",
                        "Spong",
                        "Allow"});
            table16.AddRow(new string[] {
                        "GET",
                        "spong",
                        "Spong",
                        "Allow"});
#line 19
    testRunner.And("the new ClaimsPermission has these rules", ((string)(null)), table16, "And ");
#line hidden
#line 24
    testRunner.And("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get resource access rules for unknown ClaimPermission id")]
        public virtual void GetResourceAccessRulesForUnknownClaimPermissionId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get resource access rules for unknown ClaimPermission id", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 26
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
this.FeatureBackground();
#line hidden
#line 27
    testRunner.When("the effective rules for ClaimsPermission with id named \'id-none\' are fetched via " +
                        "the getClaimPermissionsResourceAccessRules endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 28
    testRunner.Then("the HTTP status returned by the Claims service is 404", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluate single permission for unknown ClaimPermission id")]
        public virtual void EvaluateSinglePermissionForUnknownClaimPermissionId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluate single permission for unknown ClaimPermission id", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 30
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
this.FeatureBackground();
#line hidden
#line 31
    testRunner.When("permissions are evaluated via the getClaimPermissionsPermission endpoint ClaimsPe" +
                        "rmission id \'id-none\', resource \'foo\' and access type \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 32
    testRunner.Then("the HTTP status returned by the Claims service is 404", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluate permissions batch including unknown ClaimPermission id")]
        public virtual void EvaluatePermissionsBatchIncludingUnknownClaimPermissionId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluate permissions batch including unknown ClaimPermission id", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 34
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table17 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimsPermissionsId",
                            "ResourceUri",
                            "AccessType"});
                table17.AddRow(new string[] {
                            "id-none",
                            "foo",
                            "GET"});
#line 35
    testRunner.When("these permissions are evaluated via the getClaimPermissionsPermissionBatch endpoi" +
                        "nt", ((string)(null)), table17, "When ");
#line hidden
#line 38
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table18 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimsPermissionId",
                            "ResourceUri",
                            "AccessType",
                            "ResponseCode",
                            "Permission"});
                table18.AddRow(new string[] {
                            "id-none",
                            "foo",
                            "GET",
                            "404",
                            "deny"});
#line 39
    testRunner.And("the permissions batch response items are", ((string)(null)), table18, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get resource access rules for known ClaimPermission id")]
        public virtual void GetResourceAccessRulesForKnownClaimPermissionId()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get resource access rules for known ClaimPermission id", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 43
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
this.FeatureBackground();
#line hidden
#line 44
    testRunner.When("the effective rules for ClaimsPermission with id named \'id1\' are fetched via the " +
                        "getClaimPermissionsResourceAccessRules endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 45
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table19 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table19.AddRow(new string[] {
                            "GET",
                            "foo",
                            "Foo",
                            "Allow"});
                table19.AddRow(new string[] {
                            "POST",
                            "foo",
                            "Foo",
                            "Deny"});
                table19.AddRow(new string[] {
                            "GET",
                            "bar",
                            "Bar",
                            "Allow"});
#line 46
    testRunner.And("the effective rules returns by the Claims service are", ((string)(null)), table19, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluate single permission for known ClaimPermission id")]
        [NUnit.Framework.TestCaseAttribute("id1", "foo", "GET", "Allow", null)]
        [NUnit.Framework.TestCaseAttribute("id1", "foo", "POST", "Deny", null)]
        [NUnit.Framework.TestCaseAttribute("id1", "foo", "PATCH", "Deny", null)]
        [NUnit.Framework.TestCaseAttribute("id1", "unknown", "GET", "Deny", null)]
        public virtual void EvaluateSinglePermissionForKnownClaimPermissionId(string claimsPermissionsId, string resourceUri, string accessType, string permission, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("ClaimsPermissionsId", claimsPermissionsId);
            argumentsOfScenario.Add("ResourceUri", resourceUri);
            argumentsOfScenario.Add("AccessType", accessType);
            argumentsOfScenario.Add("Permission", permission);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluate single permission for known ClaimPermission id", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 52
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
this.FeatureBackground();
#line hidden
#line 53
    testRunner.When(string.Format("permissions are evaluated via the getClaimPermissionsPermission endpoint ClaimsPe" +
                            "rmission id \'{0}\', resource \'{1}\' and access type \'{2}\'", claimsPermissionsId, resourceUri, accessType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 54
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 55
    testRunner.And(string.Format("the permission returned by the Claims service is \'{0}\'", permission), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluate permissions batch for known ClaimPermissions")]
        public virtual void EvaluatePermissionsBatchForKnownClaimPermissions()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluate permissions batch for known ClaimPermissions", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 65
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 7
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table20 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimsPermissionsId",
                            "ResourceUri",
                            "AccessType"});
                table20.AddRow(new string[] {
                            "id1",
                            "foo",
                            "GET"});
                table20.AddRow(new string[] {
                            "id1",
                            "foo",
                            "POST"});
                table20.AddRow(new string[] {
                            "id1",
                            "foo",
                            "DELETE"});
                table20.AddRow(new string[] {
                            "id1",
                            "unknown",
                            "GET"});
                table20.AddRow(new string[] {
                            "id2",
                            "spong",
                            "POST"});
#line 66
    testRunner.When("these permissions are evaluated via the getClaimPermissionsPermissionBatch endpoi" +
                        "nt", ((string)(null)), table20, "When ");
#line hidden
#line 73
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table21 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimsPermissionId",
                            "ResourceUri",
                            "AccessType",
                            "ResponseCode",
                            "Permission"});
                table21.AddRow(new string[] {
                            "id1",
                            "foo",
                            "GET",
                            "200",
                            "allow"});
                table21.AddRow(new string[] {
                            "id1",
                            "foo",
                            "POST",
                            "200",
                            "deny"});
                table21.AddRow(new string[] {
                            "id1",
                            "foo",
                            "DELETE",
                            "200",
                            "deny"});
                table21.AddRow(new string[] {
                            "id1",
                            "unknown",
                            "GET",
                            "200",
                            "deny"});
                table21.AddRow(new string[] {
                            "id2",
                            "spong",
                            "POST",
                            "200",
                            "allow"});
#line 74
    testRunner.And("the permissions batch response items are", ((string)(null)), table21, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion