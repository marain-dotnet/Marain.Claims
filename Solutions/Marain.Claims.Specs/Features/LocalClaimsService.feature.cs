// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.0.0.0
//      SpecFlow Generator Version:3.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Marain.Operations.Specs.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("LocalClaimsService")]
    public partial class LocalClaimsServiceFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "LocalClaimsService.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "LocalClaimsService", "    In order to secure access to the Claims service\r\n    As a developer of the Cl" +
                    "aims service\r\n    I want the RoleBasedOpenApiAccessControlPolicy to be able to e" +
                    "valuate claim permissions without calling into itself over HTTP", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        public virtual void ScenarioTearDown()
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
        [NUnit.Framework.DescriptionAttribute("Claim forwards arguments to service")]
        [NUnit.Framework.TestCaseAttribute("cid1", "r/1", "GET", "t1", null)]
        [NUnit.Framework.TestCaseAttribute("cid2", "r/2", "PUT", "t2", null)]
        public virtual void ClaimForwardsArgumentsToService(string claimPermissionsId, string resourceUri, string accessType, string tenantId, string[] exampleTags)
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Claim forwards arguments to service", null, exampleTags);
#line 6
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 7
    testRunner.When(string.Format("I have passed a claim permissions id of \'{0}\', a resource URI of \'{1}\', an access" +
                        " type of \'{2}\', and a tenant id of \'{3}\'", claimPermissionsId, resourceUri, accessType, tenantId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 8
    testRunner.Then(string.Format("the service should have been passed a claim permissions id of \'{0}\'", claimPermissionsId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 9
    testRunner.Then(string.Format("the service should have been passed a resource URI of \'{0}\'", resourceUri), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 10
    testRunner.Then(string.Format("the service should have been passed an access type of \'{0}\'", accessType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 11
    testRunner.Then(string.Format("the service should have been passed a tenant id of \'{0}\'", tenantId), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluation grants permission")]
        public virtual void EvaluationGrantsPermission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluation grants permission", null, ((string[])(null)));
#line 19
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 20
    testRunner.When("I have passed a claim permissions id of \'c1\', a resource URI of \'r1\', an access t" +
                    "ype of \'GET\', and a tenant id of \'t1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 21
    testRunner.And("the service returns an OK result of \'Deny\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
    testRunner.Then("the response wrapper should have a status code of 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 23
    testRunner.And("the response body should contain a single permissions batch response item contain" +
                    "ing \'deny\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluation denies permission")]
        public virtual void EvaluationDeniesPermission()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluation denies permission", null, ((string[])(null)));
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 26
    testRunner.When("I have passed a claim permissions id of \'c1\', a resource URI of \'r1\', an access t" +
                    "ype of \'GET\', and a tenant id of \'t1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 27
    testRunner.And("the service returns an OK result of \'Allow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
    testRunner.Then("the response wrapper should have a status code of 200", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 29
    testRunner.And("the response body should contain a single permissions batch response item contain" +
                    "ing \'allow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Evaluation does not find claim")]
        public virtual void EvaluationDoesNotFindClaim()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Evaluation does not find claim", null, ((string[])(null)));
#line 31
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 32
    testRunner.When("I have passed a claim permissions id of \'c1\', a resource URI of \'r1\', an access t" +
                    "ype of \'GET\', and a tenant id of \'t1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 33
    testRunner.And("the service returns a Not Found result", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 34
    testRunner.Then("the response wrapper should have a status code of 404", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 35
    testRunner.And("the response should not have a body", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
