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
    [NUnit.Framework.DescriptionAttribute("ClaimPermissionsStore")]
    [NUnit.Framework.CategoryAttribute("setupTenantedCloudBlobContainer")]
    [NUnit.Framework.CategoryAttribute("setupContainer")]
    public partial class ClaimPermissionsStoreFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "ClaimPermissionsStore.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "ClaimPermissionsStore", null, ProgrammingLanguage.CSharp, new string[] {
                        "setupTenantedCloudBlobContainer",
                        "setupContainer"});
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
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "AccessType",
                        "Resource",
                        "Permission"});
            table2.AddRow(new string[] {
                        "GET",
                        "marain/test/resource1",
                        "Allow"});
#line 7
 testRunner.Given("I have a list of resource access rules called \"rules-1\"", ((string)(null)), table2, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "AccessType",
                        "Resource",
                        "Permission"});
            table3.AddRow(new string[] {
                        "GET",
                        "marain/test/resource2",
                        "Allow"});
#line 10
 testRunner.Given("I have a list of resource access rules called \"rules-2\"", ((string)(null)), table3, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "DisplayName",
                        "Rules"});
            table4.AddRow(new string[] {
                        "rulesets-1",
                        "Ruleset 1",
                        "{rules-1}"});
            table4.AddRow(new string[] {
                        "rulesets-2",
                        "Ruleset 2",
                        "{rules-2}"});
#line 13
 testRunner.And("I have resource access rulesets called \"rulesets\"", ((string)(null)), table4, "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "ResourceAccessRules",
                        "ResourceAccessRulesets"});
            table5.AddRow(new string[] {
                        "claimpermissions-1",
                        "",
                        "{rulesets}"});
#line 17
 testRunner.And("I have claim permissions called \"claimpermissions\"", ((string)(null)), table5, "And ");
#line 20
 testRunner.And("I have saved the resource access rulesets called \"rulesets\" to the resource acces" +
                    "s ruleset store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 21
 testRunner.And("I have saved the claim permissions called \"claimpermissions\" to the claim permiss" +
                    "ions store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieving claim permissions from the repository")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void RetrievingClaimPermissionsFromTheRepository()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieving claim permissions from the repository", null, new string[] {
                        "useChildObjects"});
#line 24
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 25
 testRunner.When("I request the claim permission with Id \"claimpermissions-1\" from the claim permis" +
                    "sions store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 26
 testRunner.Then("the claim permission is returned", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 27
 testRunner.And("the resource access rulesets on the claim permission match the rulesets \"rulesets" +
                    "\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieving claim permissions with an invalid Id")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void RetrievingClaimPermissionsWithAnInvalidId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieving claim permissions with an invalid Id", null, new string[] {
                        "useChildObjects"});
#line 30
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line 31
 testRunner.When("I request the claim permission with Id \"incorrectid\" from the claim permissions s" +
                    "tore", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 32
 testRunner.Then("a \"ClaimPermissionsNotFoundException\" exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieving claim permissions when one or more of the referenced rule sets are mis" +
            "sing")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void RetrievingClaimPermissionsWhenOneOrMoreOfTheReferencedRuleSetsAreMissing()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieving claim permissions when one or more of the referenced rule sets are mis" +
                    "sing", null, new string[] {
                        "useChildObjects"});
#line 35
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 6
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "DisplayName",
                        "Rules"});
            table6.AddRow(new string[] {
                        "rulesets-3",
                        "Ruleset 3",
                        "{rules-1}"});
            table6.AddRow(new string[] {
                        "rulesets-4",
                        "Ruleset 4",
                        "{rules-2}"});
#line 36
 testRunner.Given("I have resource access rulesets called \"rulesets-unsaved\"", ((string)(null)), table6, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "ResourceAccessRules",
                        "ResourceAccessRulesets"});
            table7.AddRow(new string[] {
                        "claimpermissions-2",
                        "",
                        "{rulesets-unsaved}"});
#line 40
 testRunner.And("I have claim permissions called \"claimpermissions-2\"", ((string)(null)), table7, "And ");
#line 43
 testRunner.And("I have saved the claim permissions called \"claimpermissions-2\" to the claim permi" +
                    "ssions store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 44
 testRunner.When("I request the claim permission with Id \"claimpermissions-2\" from the claim permis" +
                    "sions store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 45
 testRunner.Then("a \"ResourceAccessRuleSetNotFoundException\" exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
