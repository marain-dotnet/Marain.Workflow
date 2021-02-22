﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.7.0.0
//      SpecFlow Generator Version:3.7.0.0
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
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.7.0.0")]
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
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Sql", "Conditions with SQL", null, ProgrammingLanguage.CSharp, new string[] {
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
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An unmet exit condition on the current state prevents a transition being selected" +
                    "", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
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
                TechTalk.SpecFlow.Table table59 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table59.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table59.AddRow(new string[] {
                            "Type",
                            "t1"});
                table59.AddRow(new string[] {
                            "AllowPublishedEntry",
                            "x"});
#line 10
 testRunner.And("I have created and persisted a new instance with Id \'id1\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table59, "And ");
#line hidden
                TechTalk.SpecFlow.Table table60 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table60.AddRow(new string[] {
                            "id1",
                            "The new notes",
                            "The new description"});
#line 15
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'patch\'", ((string)(null)), table60, "And ");
#line hidden
                TechTalk.SpecFlow.Table table61 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table61.AddRow(new string[] {
                            "{patch}"});
#line 18
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table61, "And ");
#line hidden
                TechTalk.SpecFlow.Table table62 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table62.AddRow(new string[] {
                            "id1"});
#line 21
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "publishcatalogitemtrigger\'", ((string)(null)), table62, "When ");
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
                TechTalk.SpecFlow.Table table63 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table63.AddRow(new string[] {
                            "Entering state \'Waiting for documentation\'"});
#line 27
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table63, "And ");
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
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("An unmet entry condition on a target state prevents a transition being selected", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 32
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
#line 33
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 34
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table64 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table64.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table64.AddRow(new string[] {
                            "Type",
                            "t1"});
                table64.AddRow(new string[] {
                            "AllowWaitingForDocumentationExit",
                            "x"});
#line 35
 testRunner.And("I have created and persisted a new instance with Id \'id2\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table64, "And ");
#line hidden
                TechTalk.SpecFlow.Table table65 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table65.AddRow(new string[] {
                            "id2",
                            "The new notes",
                            "The new description"});
#line 40
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'patch\'", ((string)(null)), table65, "And ");
#line hidden
                TechTalk.SpecFlow.Table table66 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table66.AddRow(new string[] {
                            "{patch}"});
#line 43
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table66, "And ");
#line hidden
                TechTalk.SpecFlow.Table table67 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table67.AddRow(new string[] {
                            "id2"});
#line 46
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "publishcatalogitemtrigger\'", ((string)(null)), table67, "When ");
#line hidden
#line 49
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 50
 testRunner.Then("the workflow instance with Id \'id2\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 51
 testRunner.And("the workflow instance with Id \'id2\' should be in the state called \'Waiting for do" +
                        "cumentation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table68 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table68.AddRow(new string[] {
                            "Entering state \'Waiting for documentation\'"});
#line 52
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table68, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
