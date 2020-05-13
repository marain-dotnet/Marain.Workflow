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
namespace Marain.Workflows.Specs.Features.Sql
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Publish an item with SQL")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useSqlStores")]
    [NUnit.Framework.CategoryAttribute("setupTenantedSqlDatabase")]
    public partial class PublishAnItemWithSQLFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useSqlStores",
                "setupTenantedSqlDatabase"};
        
#line 1 "Publish.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Publish an item with SQL", null, ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useSqlStores",
                        "setupTenantedSqlDatabase"});
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
        [NUnit.Framework.DescriptionAttribute("Publish item when it is in the waiting for documentation state and it is complete" +
            "")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void PublishItemWhenItIsInTheWaitingForDocumentationStateAndItIsComplete()
        {
            string[] tagsOfScenario = new string[] {
                    "useChildObjects"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Publish item when it is in the waiting for documentation state and it is complete" +
                    "", null, new string[] {
                        "useChildObjects"});
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
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 9
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table155 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table155.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table155.AddRow(new string[] {
                            "Type",
                            "t1"});
                table155.AddRow(new string[] {
                            "AllowWaitingForDocumentationExit",
                            "x"});
                table155.AddRow(new string[] {
                            "AllowPublishedEntry",
                            "x"});
#line 10
 testRunner.And("I have created and persisted a new instance with Id \'id1\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table155, "And ");
#line hidden
                TechTalk.SpecFlow.Table table156 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table156.AddRow(new string[] {
                            "id1",
                            "The new notes",
                            "The new description"});
#line 16
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'patch\'", ((string)(null)), table156, "And ");
#line hidden
                TechTalk.SpecFlow.Table table157 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table157.AddRow(new string[] {
                            "{patch}"});
#line 19
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table157, "And ");
#line hidden
                TechTalk.SpecFlow.Table table158 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table158.AddRow(new string[] {
                            "id1"});
#line 22
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "publishcatalogitemtrigger\'", ((string)(null)), table158, "When ");
#line hidden
#line 25
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
 testRunner.Then("the workflow instance with Id \'id1\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 27
 testRunner.And("the workflow instance with Id \'id1\' should be in the state called \'Published\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
 testRunner.And("the workflow instance with Id \'id1\' should have 4 change log entries", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table159 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table159.AddRow(new string[] {
                            "Exiting state \'Waiting for documentation\'"});
                table159.AddRow(new string[] {
                            "Executing transition \'Publish\'"});
                table159.AddRow(new string[] {
                            "Entering state \'Published\'"});
#line 29
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table159, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
