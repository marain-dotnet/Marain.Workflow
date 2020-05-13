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
    [NUnit.Framework.DescriptionAttribute("Conditions with SQL")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useSqlStores")]
    [NUnit.Framework.CategoryAttribute("setupTenantedSqlDatabase")]
    public partial class ConditionsWithSQLFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useSqlStores",
                "setupTenantedSqlDatabase"};
        
#line 1 "Conditions.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Conditions with SQL", null, ProgrammingLanguage.CSharp, new string[] {
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
        [NUnit.Framework.DescriptionAttribute("An unmet exit condition on the current state prevents a transition being selected" +
            "")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void AnUnmetExitConditionOnTheCurrentStatePreventsATransitionBeingSelected()
        {
            string[] tagsOfScenario = new string[] {
                    "useChildObjects"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An unmet exit condition on the current state prevents a transition being selected" +
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
                TechTalk.SpecFlow.Table table107 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table107.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table107.AddRow(new string[] {
                            "Type",
                            "t1"});
                table107.AddRow(new string[] {
                            "AllowPublishedEntry",
                            "x"});
#line 10
 testRunner.And("I have created and persisted a new instance with Id \'id1\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table107, "And ");
#line hidden
                TechTalk.SpecFlow.Table table108 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table108.AddRow(new string[] {
                            "id1",
                            "The new notes",
                            "The new description"});
#line 15
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'patch\'", ((string)(null)), table108, "And ");
#line hidden
                TechTalk.SpecFlow.Table table109 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table109.AddRow(new string[] {
                            "{patch}"});
#line 18
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table109, "And ");
#line hidden
                TechTalk.SpecFlow.Table table110 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table110.AddRow(new string[] {
                            "id1"});
#line 21
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "publishcatalogitemtrigger\'", ((string)(null)), table110, "When ");
#line hidden
#line 24
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 25
 testRunner.Then("the workflow instance with Id \'id1\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 26
 testRunner.And("the workflow instance with Id \'id1\' should be in the state called \'Waiting for do" +
                        "cumentation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
 testRunner.And("the workflow instance with Id \'id1\' should have 2 change log entries", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table111 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table111.AddRow(new string[] {
                            "Entering state \'Waiting for documentation\'"});
#line 28
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table111, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("An unmet entry condition on a target state prevents a transition being selected")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void AnUnmetEntryConditionOnATargetStatePreventsATransitionBeingSelected()
        {
            string[] tagsOfScenario = new string[] {
                    "useChildObjects"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An unmet entry condition on a target state prevents a transition being selected", null, new string[] {
                        "useChildObjects"});
#line 33
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
#line 34
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 35
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table112 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table112.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table112.AddRow(new string[] {
                            "Type",
                            "t1"});
                table112.AddRow(new string[] {
                            "AllowWaitingForDocumentationExit",
                            "x"});
#line 36
 testRunner.And("I have created and persisted a new instance with Id \'id2\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table112, "And ");
#line hidden
                TechTalk.SpecFlow.Table table113 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table113.AddRow(new string[] {
                            "id2",
                            "The new notes",
                            "The new description"});
#line 41
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'patch\'", ((string)(null)), table113, "And ");
#line hidden
                TechTalk.SpecFlow.Table table114 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table114.AddRow(new string[] {
                            "{patch}"});
#line 44
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table114, "And ");
#line hidden
                TechTalk.SpecFlow.Table table115 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table115.AddRow(new string[] {
                            "id2"});
#line 47
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "publishcatalogitemtrigger\'", ((string)(null)), table115, "When ");
#line hidden
#line 50
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 51
 testRunner.Then("the workflow instance with Id \'id2\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 52
 testRunner.And("the workflow instance with Id \'id2\' should be in the state called \'Waiting for do" +
                        "cumentation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 53
 testRunner.And("the workflow instance with Id \'id2\' should have 3 change log entries", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table116 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table116.AddRow(new string[] {
                            "Entering state \'Waiting for documentation\'"});
#line 54
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table116, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
