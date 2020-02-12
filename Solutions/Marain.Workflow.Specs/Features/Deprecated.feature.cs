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
namespace Marain.Workflows.Specs.Features
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Deprecate an item")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("setupTenantedCosmosContainers")]
    public partial class DeprecateAnItemFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Deprecated.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Deprecate an item", null, ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "setupTenantedCosmosContainers"});
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
        [NUnit.Framework.DescriptionAttribute("Deprecate item when it is in the published state")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void DeprecateItemWhenItIsInThePublishedState()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Deprecate item when it is in the published state", null, new string[] {
                        "useChildObjects"});
#line 6
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 7
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 8
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table20 = new TechTalk.SpecFlow.Table(new string[] {
                        "Key",
                        "Value"});
            table20.AddRow(new string[] {
                        "Identifier",
                        "identifier1"});
            table20.AddRow(new string[] {
                        "Type",
                        "t1"});
            table20.AddRow(new string[] {
                        "AllowWaitingForDocumentationExit",
                        "x"});
            table20.AddRow(new string[] {
                        "AllowPublishedEntry",
                        "x"});
#line 9
 testRunner.And("I have created and persisted a new instance with Id \'id1\' of the workflow with Id" +
                    " \'dc-workflow\' and supplied the following context items", ((string)(null)), table20, "And ");
#line hidden
            TechTalk.SpecFlow.Table table21 = new TechTalk.SpecFlow.Table(new string[] {
                        "Id",
                        "Notes",
                        "Description"});
            table21.AddRow(new string[] {
                        "id1",
                        "The new notes",
                        "The new description"});
#line 15
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                    "ils\' called \'patch\'", ((string)(null)), table21, "And ");
#line hidden
            TechTalk.SpecFlow.Table table22 = new TechTalk.SpecFlow.Table(new string[] {
                        "PatchDetails"});
            table22.AddRow(new string[] {
                        "{patch}"});
#line 18
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                    "alog.editcatalogitemtrigger\'", ((string)(null)), table22, "And ");
#line hidden
            TechTalk.SpecFlow.Table table23 = new TechTalk.SpecFlow.Table(new string[] {
                        "CatalogItemId"});
            table23.AddRow(new string[] {
                        "id1"});
#line 21
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                    "alog.publishcatalogitemtrigger\'", ((string)(null)), table23, "And ");
#line hidden
            TechTalk.SpecFlow.Table table24 = new TechTalk.SpecFlow.Table(new string[] {
                        "CatalogItemId"});
            table24.AddRow(new string[] {
                        "id1"});
#line 24
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                    "deprecatecatalogitemtrigger\'", ((string)(null)), table24, "When ");
#line 27
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
 testRunner.Then("the workflow instance with Id \'id1\' should have status \'Complete\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 29
 testRunner.And("the workflow instance with Id \'id1\' should be in the state called \'Deprecated\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table25 = new TechTalk.SpecFlow.Table(new string[] {
                        "Message"});
            table25.AddRow(new string[] {
                        "Exiting state \'Published\'"});
            table25.AddRow(new string[] {
                        "Executing transition \'Deprecate\'"});
            table25.AddRow(new string[] {
                        "Entering state \'Deprecated\'"});
#line 30
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table25, "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
