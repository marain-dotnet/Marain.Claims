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
    [NUnit.Framework.DescriptionAttribute("RoleBasedOpenApiAccessControlPolicy")]
    [NUnit.Framework.CategoryAttribute("rolebased")]
    public partial class RoleBasedOpenApiAccessControlPolicyFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = new string[] {
                "rolebased"};
        
#line 1 "RoleBasedOpenApiAccessControlPolicy.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "RoleBasedOpenApiAccessControlPolicy", "    In order to secure an OpenApi service\r\n    As a developer\r\n    I want to appl" +
                    "y application role-based security", ProgrammingLanguage.CSharp, featureTags);
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
        [NUnit.Framework.DescriptionAttribute("The client is unauthenticated")]
        public void TheClientIsUnauthenticated()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The client is unauthenticated", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 8
    testRunner.Given("I am not authenticated", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 9
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 10
    testRunner.Then("the result type should be \'NotAuthenticated\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 11
    testRunner.And("the policy should not have attempted to use the claims service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The client belongs to no roles")]
        public void TheClientBelongsToNoRoles()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The client belongs to no roles", null, tagsOfScenario, argumentsOfScenario, featureTags);
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
    testRunner.Given("I have a ClaimsPrincipal with 0 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 15
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 16
    testRunner.Then("the result type should be \'NotAllowed\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 17
    testRunner.And("the policy should not have attempted to use the claims service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The client belongs to one role")]
        [NUnit.Framework.TestCaseAttribute("", "/foo/bar", "GET", "foo/bar", null)]
        [NUnit.Framework.TestCaseAttribute("", "/baz/ick", "GET", "baz/ick", null)]
        [NUnit.Framework.TestCaseAttribute("", "/foo/bar", "PUT", "foo/bar", null)]
        [NUnit.Framework.TestCaseAttribute("testPrefix/", "/foo/bar", "GET", "testPrefix/foo/bar", null)]
        [NUnit.Framework.TestCaseAttribute("testPrefix/", "/foo/bar", "PUT", "testPrefix/foo/bar", null)]
        public void TheClientBelongsToOneRole(string resourcePrefix, string path, string method, string resourceUri, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("resourcePrefix", resourcePrefix);
            argumentsOfScenario.Add("path", path);
            argumentsOfScenario.Add("method", method);
            argumentsOfScenario.Add("resourceUri", resourceUri);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The client belongs to one role", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 19
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 20
    testRunner.Given("I have a ClaimsPrincipal with 1 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 21
    testRunner.And(string.Format("the policy has a resource prefix of \'{0}\'", resourcePrefix), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 22
    testRunner.When(string.Format("I invoke the policy with a path of \'{0}\' and a method of \'{1}\'", path, method), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 23
    testRunner.Then("the policy should pass the claim permissions id 0 to the claims service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 24
    testRunner.And("the policy should pass the tenant id to the claims service in call for claims per" +
                        "missions ID 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 25
    testRunner.And(string.Format("the policy should pass a resource URI of \'{0}\' to the claims service in call for " +
                            "claims permissions ID 0", resourceUri), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
    testRunner.And(string.Format("the policy should pass an access type of \'{0}\' to the claims service in call for " +
                            "claims permissions ID 0", method), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The client\'s only role grants it permission")]
        public void TheClientsOnlyRoleGrantsItPermission()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The client\'s only role grants it permission", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 36
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 37
    testRunner.Given("I have a ClaimsPrincipal with 1 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 38
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 39
    testRunner.And("the evaluator returns \'Allow\' for claim permissions ID 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 40
    testRunner.Then("the result should grant access", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The client\'s only role denies it permission")]
        public void TheClientsOnlyRoleDeniesItPermission()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The client\'s only role denies it permission", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 42
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 43
    testRunner.Given("I have a ClaimsPrincipal with 1 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 44
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 45
    testRunner.And("the evaluator returns \'Deny\' for claim permissions ID 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 46
    testRunner.Then("the result type should be \'NotAllowed\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The claims service doesn\'t recognize the role")]
        public void TheClaimsServiceDoesntRecognizeTheRole()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The claims service doesn\'t recognize the role", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 48
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 49
    testRunner.Given("I have a ClaimsPrincipal with 1 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 50
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 51
    testRunner.And("the evaluator does not find the claim permissions ID 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 52
    testRunner.Then("the result type should be \'NotAllowed\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("The client belongs to three roles")]
        public void TheClientBelongsToThreeRoles()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("The client belongs to three roles", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 54
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 55
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 56
    testRunner.When("I invoke the policy with a path of \'foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 57
    testRunner.Then("the policy should pass the claim permissions id 0 to the claims service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 58
    testRunner.And("the policy should pass a resource URI of \'foo/bar\' to the claims service in call " +
                        "for claims permissions ID 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 59
    testRunner.And("the policy should pass an access type of \'GET\' to the claims service in call for " +
                        "claims permissions ID 0", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 60
    testRunner.And("the policy should pass the claim permissions id 1 to the claims service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 61
    testRunner.And("the policy should pass a resource URI of \'foo/bar\' to the claims service in call " +
                        "for claims permissions ID 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 62
    testRunner.And("the policy should pass an access type of \'GET\' to the claims service in call for " +
                        "claims permissions ID 1", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 63
    testRunner.And("the policy should pass the claim permissions id 2 to the claims service", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 64
    testRunner.And("the policy should pass a resource URI of \'foo/bar\' to the claims service in call " +
                        "for claims permissions ID 2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
    testRunner.And("the policy should pass an access type of \'GET\' to the claims service in call for " +
                        "claims permissions ID 2", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("All of the client\'s roles grant it permission")]
        public void AllOfTheClientsRolesGrantItPermission()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("All of the client\'s roles grant it permission", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 67
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 68
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 69
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table19 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimPermissionsId",
                            "Result"});
                table19.AddRow(new string[] {
                            "0",
                            "allow"});
                table19.AddRow(new string[] {
                            "1",
                            "allow"});
                table19.AddRow(new string[] {
                            "2",
                            "allow"});
#line 70
 testRunner.And("the evaluator returns the following results", ((string)(null)), table19, "And ");
#line hidden
#line 75
    testRunner.Then("the result should grant access", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("All of the client\'s roles deny it permission")]
        public void AllOfTheClientsRolesDenyItPermission()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("All of the client\'s roles deny it permission", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 77
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 78
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 79
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table20 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimPermissionsId",
                            "Result"});
                table20.AddRow(new string[] {
                            "0",
                            "deny"});
                table20.AddRow(new string[] {
                            "1",
                            "deny"});
                table20.AddRow(new string[] {
                            "2",
                            "deny"});
#line 80
 testRunner.And("the evaluator returns the following results", ((string)(null)), table20, "And ");
#line hidden
#line 85
    testRunner.Then("the result type should be \'NotAllowed\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One client role grants permission and the other two deny it in Allow If Any mode")]
        public void OneClientRoleGrantsPermissionAndTheOtherTwoDenyItInAllowIfAnyMode()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One client role grants permission and the other two deny it in Allow If Any mode", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 87
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 88
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 89
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table21 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimPermissionsId",
                            "Result"});
                table21.AddRow(new string[] {
                            "0",
                            "allow"});
                table21.AddRow(new string[] {
                            "1",
                            "deny"});
                table21.AddRow(new string[] {
                            "2",
                            "deny"});
#line 90
 testRunner.And("the evaluator returns the following results", ((string)(null)), table21, "And ");
#line hidden
#line 95
    testRunner.Then("the result should grant access", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two client roles grant permission and the other denies it in Allow If Any mode")]
        public void TwoClientRolesGrantPermissionAndTheOtherDeniesItInAllowIfAnyMode()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two client roles grant permission and the other denies it in Allow If Any mode", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 97
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 98
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 99
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table22 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimPermissionsId",
                            "Result"});
                table22.AddRow(new string[] {
                            "0",
                            "allow"});
                table22.AddRow(new string[] {
                            "1",
                            "allow"});
                table22.AddRow(new string[] {
                            "2",
                            "deny"});
#line 100
 testRunner.And("the evaluator returns the following results", ((string)(null)), table22, "And ");
#line hidden
#line 105
    testRunner.Then("the result should grant access", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("One client role grants permission and the other two deny it in All Only If All mo" +
            "de")]
        public void OneClientRoleGrantsPermissionAndTheOtherTwoDenyItInAllOnlyIfAllMode()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("One client role grants permission and the other two deny it in All Only If All mo" +
                    "de", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 107
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 108
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 109
    testRunner.And("the policy is configured in allow only if all mode", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 110
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table23 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimPermissionsId",
                            "Result"});
                table23.AddRow(new string[] {
                            "0",
                            "allow"});
                table23.AddRow(new string[] {
                            "1",
                            "deny"});
                table23.AddRow(new string[] {
                            "2",
                            "deny"});
#line 111
 testRunner.And("the evaluator returns the following results", ((string)(null)), table23, "And ");
#line hidden
#line 116
    testRunner.Then("the result type should be \'NotAllowed\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Two client roles grant permission and the other denies it in All Only If All mode" +
            "")]
        public void TwoClientRolesGrantPermissionAndTheOtherDeniesItInAllOnlyIfAllMode()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Two client roles grant permission and the other denies it in All Only If All mode" +
                    "", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 118
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 119
    testRunner.Given("I have a ClaimsPrincipal with 3 roles claims", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 120
    testRunner.And("the policy is configured in allow only if all mode", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 121
    testRunner.When("I invoke the policy with a path of \'/foo/bar\' and a method of \'GET\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table24 = new TechTalk.SpecFlow.Table(new string[] {
                            "ClaimPermissionsId",
                            "Result"});
                table24.AddRow(new string[] {
                            "0",
                            "allow"});
                table24.AddRow(new string[] {
                            "1",
                            "allow"});
                table24.AddRow(new string[] {
                            "2",
                            "deny"});
#line 122
 testRunner.And("the evaluator returns the following results", ((string)(null)), table24, "And ");
#line hidden
#line 127
    testRunner.Then("the result type should be \'NotAllowed\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
