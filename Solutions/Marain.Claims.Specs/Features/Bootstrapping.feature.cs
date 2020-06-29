﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.1.0.0
//      SpecFlow Generator Version:3.1.0.0
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Bootstrapping")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useTransientTenant")]
    public partial class BootstrappingFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useTransientTenant"};
        
#line 1 "Bootstrapping.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Bootstrapping", "    As an initial user\r\n    I want to initialise a tenant of the claims framework" +
                    "\r\n    So that I have the ability to manage claims", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useTransientTenant"});
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
        [NUnit.Framework.DescriptionAttribute("Claims framework tenant is uninitialised service unit test")]
        [NUnit.Framework.CategoryAttribute("inMemoryStore")]
        public virtual void ClaimsFrameworkTenantIsUninitialisedServiceUnitTest()
        {
            string[] tagsOfScenario = new string[] {
                    "inMemoryStore"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Claims framework tenant is uninitialised service unit test", null, new string[] {
                        "inMemoryStore"});
#line 10
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
#line 11
    testRunner.Given("the tenant is uninitialised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 12
    testRunner.When("I initialise the tenant with the role id \'adminrole\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 13
    testRunner.Then("the tenant is initialised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 14
    testRunner.And("the service creates a claims permission with id \'adminrole\' with empty resourceAc" +
                        "cessRules and a single resourceAccessRuleSet \'marainClaimsAdministrator\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 15
    testRunner.And("the service creates an access rule set with id \'marainClaimsAdministrator\' with d" +
                        "isplayname \'Claims Administrator Permissions\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                            "resourceUri",
                            "resourceDisplayName",
                            "accessType",
                            "permission"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/claimPermissions/**/*",
                            "Read Claim Permissions",
                            "GET",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/claimPermissions/**/*",
                            "Modify Claim Permissions",
                            "PUT",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/claimPermissions",
                            "Create Claim Permissions",
                            "POST",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/claimPermissions/**/*",
                            "Add to Claim Permissions",
                            "POST",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/resourceAccessRuleSet/**/*",
                            "Read Resource Access Rules",
                            "GET",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/resourceAccessRuleSet/**/*",
                            "Modify Resource Access Rules",
                            "PUT",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/resourceAccessRuleSet",
                            "Create Resource Access Rules",
                            "POST",
                            "Allow"});
                table1.AddRow(new string[] {
                            "<tenantid>/marain/claims/resourceAccessRuleSet/**/*",
                            "Add to Resource Access Rules",
                            "POST",
                            "Allow"});
#line 16
    testRunner.And("the access rule set created has the following rules", ((string)(null)), table1, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Claims framework tenant is uninitialised service integration test")]
        [NUnit.Framework.CategoryAttribute("realStore")]
        public virtual void ClaimsFrameworkTenantIsUninitialisedServiceIntegrationTest()
        {
            string[] tagsOfScenario = new string[] {
                    "realStore"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Claims framework tenant is uninitialised service integration test", null, new string[] {
                        "realStore"});
#line 28
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
#line 29
    testRunner.Given("the tenant is uninitialised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 30
    testRunner.When("I initialise the tenant with the role id \'adminrole\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 31
    testRunner.Then("the tenant is initialised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 35
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to create a claim permiss" +
                        "ions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 36
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to create a claim perm" +
                        "issions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 40
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to read a claim permissio" +
                        "ns", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 41
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to read a claim permis" +
                        "sions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 45
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to read all effective rul" +
                        "es for a claims permission", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 46
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to read all effective " +
                        "rules for a claims permission", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 50
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to add a rule to a claim " +
                        "permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 51
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to add a rule to a cla" +
                        "im permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 55
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to set all rules in a cla" +
                        "im permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 56
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to set all rules in a " +
                        "claim permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 60
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to add a resource access " +
                        "rule set to the claim permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 61
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to add a resource acce" +
                        "ss rule set to the claim permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to set all resource acces" +
                        "s rule sets in a claim permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 66
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to set all resource ac" +
                        "cess rule sets in a claim permissions", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 70
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to create a resource acce" +
                        "ss rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 71
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to create a resource a" +
                        "ccess rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 75
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to read a resource access" +
                        " rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 76
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to read a resource acc" +
                        "ess rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 80
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to add an access rule to " +
                        "the resource access rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 81
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to add an access rule " +
                        "to the resource access rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 85
    testRunner.And("a principal in the \'adminrole\' role gets \'Allow\' trying to set all access rules i" +
                        "n a resource access rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 86
    testRunner.And("a principal in the \'someotherrole\' role gets \'Deny\' trying to set all access rule" +
                        "s in a resource access rule set", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Claims framework tenant is initialised already")]
        [NUnit.Framework.CategoryAttribute("inMemoryStore")]
        public virtual void ClaimsFrameworkTenantIsInitialisedAlready()
        {
            string[] tagsOfScenario = new string[] {
                    "inMemoryStore"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Claims framework tenant is initialised already", null, new string[] {
                        "inMemoryStore"});
#line 89
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
#line 90
    testRunner.Given("the tenant is initialised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 91
    testRunner.When("I initialise the tenant with the role id \'somedifferentrole\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 92
    testRunner.Then("I am told that the tenant is already is initialised", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 93
    testRunner.And("no access rules sets are created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 94
    testRunner.And("no claim permissions are created", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
