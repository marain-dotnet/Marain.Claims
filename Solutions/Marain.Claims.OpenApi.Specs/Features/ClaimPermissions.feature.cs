﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.8.0.0
//      SpecFlow Generator Version:3.8.0.0
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.8.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("ClaimPermissions")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useClaimsApi")]
    public partial class ClaimPermissionsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useClaimsApi"};
        
#line 1 "ClaimPermissions.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "ClaimPermissions", "    Service endpoints for creating and getting claim permissions", ProgrammingLanguage.CSharp, new string[] {
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
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("GET non-existent ClaimPermissions")]
        public virtual void GETNon_ExistentClaimPermissions()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("GET non-existent ClaimPermissions", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 7
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
#line 8
    testRunner.Given("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 9
    testRunner.When("ClaimsPermission with id named \'id1\' is fetched from the getClaimPermissions endp" +
                        "oint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
    testRunner.Then("the HTTP status returned by the Claims service is 404", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("POST new ClaimPermissions with no rules or rulesets")]
        public virtual void POSTNewClaimPermissionsWithNoRulesOrRulesets()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("POST new ClaimPermissions with no rules or rulesets", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 12
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
#line 13
    testRunner.Given("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 14
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 15
    testRunner.When("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 17
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get existing ClaimPermissions with no rules or rulesets")]
        public virtual void GetExistingClaimPermissionsWithNoRulesOrRulesets()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get existing ClaimPermissions with no rules or rulesets", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 19
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
#line 20
    testRunner.Given("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 21
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 22
    testRunner.And("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
    testRunner.When("ClaimsPermission with id named \'id1\' is fetched from the getClaimPermissions endp" +
                        "oint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 24
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 25
    testRunner.And("the ClaimPermissions returned by the Claims service\'s id matches \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get existing ClaimPermissions that was created with rules")]
        public virtual void GetExistingClaimPermissionsThatWasCreatedWithRules()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get existing ClaimPermissions that was created with rules", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 27
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
#line 28
    testRunner.Given("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 29
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table1.AddRow(new string[] {
                            "GET",
                            "foo",
                            "Foo",
                            "Allow"});
                table1.AddRow(new string[] {
                            "POST",
                            "foo",
                            "Foo",
                            "Deny"});
                table1.AddRow(new string[] {
                            "GET",
                            "bar",
                            "Bar",
                            "Allow"});
#line 30
    testRunner.And("the new ClaimsPermission has these rules", ((string)(null)), table1, "And ");
#line hidden
#line 35
    testRunner.And("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 36
    testRunner.When("ClaimsPermission with id named \'id1\' is fetched from the getClaimPermissions endp" +
                        "oint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 37
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 38
    testRunner.And("the ClaimPermissions returned by the Claims service\'s id matches \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table2.AddRow(new string[] {
                            "GET",
                            "foo",
                            "Foo",
                            "Allow"});
                table2.AddRow(new string[] {
                            "POST",
                            "foo",
                            "Foo",
                            "Deny"});
                table2.AddRow(new string[] {
                            "GET",
                            "bar",
                            "Bar",
                            "Allow"});
#line 39
    testRunner.And("the ClaimPermissions returned by the Claims service has exactly these defined rul" +
                        "es", ((string)(null)), table2, "And ");
#line hidden
#line 44
    testRunner.And("the ClaimPermissions returned by the Claims service has no rulesets", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table3.AddRow(new string[] {
                            "GET",
                            "foo",
                            "Foo",
                            "Allow"});
                table3.AddRow(new string[] {
                            "POST",
                            "foo",
                            "Foo",
                            "Deny"});
                table3.AddRow(new string[] {
                            "GET",
                            "bar",
                            "Bar",
                            "Allow"});
#line 45
    testRunner.And("the ClaimPermissions returned by the Claims service has exactly these effective r" +
                        "ules", ((string)(null)), table3, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get existing ClaimPermissions that was created with rulesets")]
        public virtual void GetExistingClaimPermissionsThatWasCreatedWithRulesets()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get existing ClaimPermissions that was created with rulesets", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 51
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
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table4.AddRow(new string[] {
                            "GET",
                            "r1a",
                            "R1a",
                            "Allow"});
                table4.AddRow(new string[] {
                            "POST",
                            "r1a",
                            "R1a",
                            "Deny"});
                table4.AddRow(new string[] {
                            "GET",
                            "r1b",
                            "R1b",
                            "Allow"});
#line 52
    testRunner.Given("an existing ruleset with id \'rs1\' named \'Ruleset 1\' and these rules", ((string)(null)), table4, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table5.AddRow(new string[] {
                            "GET",
                            "r2a",
                            "R2a",
                            "Allow"});
                table5.AddRow(new string[] {
                            "GET",
                            "r2b",
                            "R2b",
                            "Allow"});
#line 57
    testRunner.Given("an existing ruleset with id \'rs2\' named \'Ruleset 2\' and these rules", ((string)(null)), table5, "Given ");
#line hidden
                TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table6.AddRow(new string[] {
                            "PATCH",
                            "r2a",
                            "R2a",
                            "Allow"});
#line 61
    testRunner.Given("an existing ruleset with id \'rs3\' named \'Ruleset 3\' and these rules", ((string)(null)), table6, "Given ");
#line hidden
#line 64
    testRunner.And("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                            "ID"});
                table7.AddRow(new string[] {
                            "rs1"});
                table7.AddRow(new string[] {
                            "rs2"});
                table7.AddRow(new string[] {
                            "rs3"});
#line 66
    testRunner.And("the new ClaimsPermission has these ruleset IDs", ((string)(null)), table7, "And ");
#line hidden
#line 71
    testRunner.And("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 72
    testRunner.When("ClaimsPermission with id named \'id1\' is fetched from the getClaimPermissions endp" +
                        "oint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 73
    testRunner.Then("the HTTP status returned by the Claims service is 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 74
    testRunner.And("the ClaimPermissions returned by the Claims service\'s id matches \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
#line 75
    testRunner.And("the ClaimPermissions returned by the Claims service has exactly these defined rul" +
                        "es", ((string)(null)), table8, "And ");
#line hidden
                TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table9.AddRow(new string[] {
                            "GET",
                            "r1a",
                            "R1a",
                            "Allow"});
                table9.AddRow(new string[] {
                            "POST",
                            "r1a",
                            "R1a",
                            "Deny"});
                table9.AddRow(new string[] {
                            "GET",
                            "r1b",
                            "R1b",
                            "Allow"});
#line 77
    testRunner.And("the ClaimPermissions returned by the Claims service has a ruleset with id \'rs1\' n" +
                        "amed \'Ruleset 1\' with these rules", ((string)(null)), table9, "And ");
#line hidden
                TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table10.AddRow(new string[] {
                            "GET",
                            "r2a",
                            "R2a",
                            "Allow"});
                table10.AddRow(new string[] {
                            "GET",
                            "r2b",
                            "R2b",
                            "Allow"});
#line 82
    testRunner.And("the ClaimPermissions returned by the Claims service has a ruleset with id \'rs2\' n" +
                        "amed \'Ruleset 2\' with these rules", ((string)(null)), table10, "And ");
#line hidden
                TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table11.AddRow(new string[] {
                            "PATCH",
                            "r2a",
                            "R2a",
                            "Allow"});
#line 86
    testRunner.And("the ClaimPermissions returned by the Claims service has a ruleset with id \'rs3\' n" +
                        "amed \'Ruleset 3\' with these rules", ((string)(null)), table11, "And ");
#line hidden
                TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table12.AddRow(new string[] {
                            "GET",
                            "r1a",
                            "R1a",
                            "Allow"});
                table12.AddRow(new string[] {
                            "POST",
                            "r1a",
                            "R1a",
                            "Deny"});
                table12.AddRow(new string[] {
                            "GET",
                            "r1b",
                            "R1b",
                            "Allow"});
                table12.AddRow(new string[] {
                            "GET",
                            "r2a",
                            "R2a",
                            "Allow"});
                table12.AddRow(new string[] {
                            "GET",
                            "r2b",
                            "R2b",
                            "Allow"});
                table12.AddRow(new string[] {
                            "PATCH",
                            "r2a",
                            "R2a",
                            "Allow"});
#line 89
    testRunner.And("the ClaimPermissions returned by the Claims service has exactly these effective r" +
                        "ules", ((string)(null)), table12, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("POST ClaimPermissions with ruleset IDs for non-existent rulesets")]
        public virtual void POSTClaimPermissionsWithRulesetIDsForNon_ExistentRulesets()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("POST ClaimPermissions with ruleset IDs for non-existent rulesets", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 99
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
                TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                            "AccessType",
                            "ResourceUri",
                            "ResourceName",
                            "Permission"});
                table13.AddRow(new string[] {
                            "GET",
                            "r1a",
                            "R1a",
                            "Allow"});
                table13.AddRow(new string[] {
                            "POST",
                            "r1a",
                            "R1a",
                            "Deny"});
                table13.AddRow(new string[] {
                            "GET",
                            "r1b",
                            "R1b",
                            "Allow"});
#line 100
    testRunner.Given("an existing ruleset with id \'rs11\' named \'Ruleset 11\' and these rules", ((string)(null)), table13, "Given ");
#line hidden
#line 105
    testRunner.And("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 106
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                            "ID"});
                table14.AddRow(new string[] {
                            "rs11"});
                table14.AddRow(new string[] {
                            "rs12"});
                table14.AddRow(new string[] {
                            "rs13"});
#line 107
    testRunner.And("the new ClaimsPermission has these ruleset IDs", ((string)(null)), table14, "And ");
#line hidden
#line 112
    testRunner.When("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 113
    testRunner.Then("the HTTP status returned by the Claims service is 400", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("POST ClaimPermissions with existing ID")]
        public virtual void POSTClaimPermissionsWithExistingID()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("POST ClaimPermissions with existing ID", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 117
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
#line 118
    testRunner.Given("a unique ClaimsPermission id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 119
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 120
    testRunner.And("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 121
    testRunner.And("a new ClaimsPermission with id named \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 122
    testRunner.When("the new ClaimsPermission is POSTed to the createClaimPermissions endpoint", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 123
    testRunner.Then("the HTTP status returned by the Claims service is 400", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
