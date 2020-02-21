// ------------------------------------------------------------------------------
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
    [NUnit.Framework.DescriptionAttribute("Edit catalog item with SQL")]
    [NUnit.Framework.CategoryAttribute("ignore")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useSqlStores")]
    [NUnit.Framework.CategoryAttribute("setupTenantedSqlDatabase")]
    public partial class EditCatalogItemWithSQLFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "ignore",
                "perFeatureContainer",
                "useSqlStores",
                "setupTenantedSqlDatabase"};
        
#line 1 "EditCatalogItem.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Edit catalog item with SQL", null, ProgrammingLanguage.CSharp, new string[] {
                        "ignore",
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
        [NUnit.Framework.DescriptionAttribute("Edit item in the Waiting for Documentation state")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void EditItemInTheWaitingForDocumentationState()
        {
            string[] tagsOfScenario = new string[] {
                    "useChildObjects"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Edit item in the Waiting for Documentation state", null, new string[] {
                        "useChildObjects"});
#line 8
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
#line 9
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 10
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table79 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table79.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table79.AddRow(new string[] {
                            "Type",
                            "t1"});
                table79.AddRow(new string[] {
                            "AllowWaitingForDocumentationExit",
                            "x"});
                table79.AddRow(new string[] {
                            "AllowPublishedEntry",
                            "x"});
#line 11
 testRunner.And("I have created and persisted a new instance with Id \'id1\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table79, "And ");
#line hidden
                TechTalk.SpecFlow.Table table80 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table80.AddRow(new string[] {
                            "id1",
                            "The new notes",
                            "The new description"});
#line 17
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'patch\'", ((string)(null)), table80, "And ");
#line hidden
                TechTalk.SpecFlow.Table table81 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table81.AddRow(new string[] {
                            "{patch}"});
#line 20
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "editcatalogitemtrigger\'", ((string)(null)), table81, "When ");
#line hidden
#line 23
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 24
 testRunner.Then("the workflow instance with Id \'id1\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 25
 testRunner.And("the workflow instance with Id \'id1\' should be in the state called \'Waiting for do" +
                        "cumentation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
 testRunner.And("the data catalog item with Id \'id1\' should have an Identifier of \'identifier1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
 testRunner.And("the data catalog item with Id \'id1\' should have a Type of \'t1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
 testRunner.And("the data catalog item with Id \'id1\' should have a Description of \'The new descrip" +
                        "tion\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
 testRunner.And("the data catalog item with Id \'id1\' should have Notes of \'The new notes\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table82 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table82.AddRow(new string[] {
                            "Exiting state \'Waiting for documentation\'"});
                table82.AddRow(new string[] {
                            "Executing transition \'Edit\'"});
                table82.AddRow(new string[] {
                            "Entering state \'Waiting for documentation\'"});
#line 30
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table82, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Edit item in the Published state with a change that would keep the item complete")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void EditItemInThePublishedStateWithAChangeThatWouldKeepTheItemComplete()
        {
            string[] tagsOfScenario = new string[] {
                    "useChildObjects"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Edit item in the Published state with a change that would keep the item complete", null, new string[] {
                        "useChildObjects"});
#line 37
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
#line 38
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 39
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table83 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table83.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table83.AddRow(new string[] {
                            "Type",
                            "t1"});
                table83.AddRow(new string[] {
                            "AllowWaitingForDocumentationExit",
                            "x"});
                table83.AddRow(new string[] {
                            "AllowPublishedEntry",
                            "x"});
#line 40
 testRunner.And("I have created and persisted a new instance with Id \'id2\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table83, "And ");
#line hidden
                TechTalk.SpecFlow.Table table84 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table84.AddRow(new string[] {
                            "id2",
                            "The notes",
                            "The description"});
#line 46
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'InitialPatch\'", ((string)(null)), table84, "And ");
#line hidden
                TechTalk.SpecFlow.Table table85 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table85.AddRow(new string[] {
                            "id2",
                            "The new notes",
                            "The new description"});
#line 49
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'TestPatch\'", ((string)(null)), table85, "And ");
#line hidden
                TechTalk.SpecFlow.Table table86 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table86.AddRow(new string[] {
                            "{InitialPatch}"});
#line 52
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table86, "And ");
#line hidden
                TechTalk.SpecFlow.Table table87 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table87.AddRow(new string[] {
                            "id2"});
#line 55
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.publishcatalogitemtrigger\'", ((string)(null)), table87, "And ");
#line hidden
                TechTalk.SpecFlow.Table table88 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table88.AddRow(new string[] {
                            "{TestPatch}"});
#line 58
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "editcatalogitemtrigger\'", ((string)(null)), table88, "When ");
#line hidden
#line 61
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 62
 testRunner.Then("the workflow instance with Id \'id2\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 63
 testRunner.And("the workflow instance with Id \'id2\' should be in the state called \'Published\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 64
 testRunner.And("the data catalog item with Id \'id2\' should have an Identifier of \'identifier1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
 testRunner.And("the data catalog item with Id \'id2\' should have a Type of \'t1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 66
 testRunner.And("the data catalog item with Id \'id2\' should have a Description of \'The new descrip" +
                        "tion\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 67
 testRunner.And("the data catalog item with Id \'id2\' should have Notes of \'The new notes\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table89 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table89.AddRow(new string[] {
                            "Exiting state \'Published\'"});
                table89.AddRow(new string[] {
                            "Executing transition \'Edit (complete)\'"});
                table89.AddRow(new string[] {
                            "Entering state \'Published\'"});
#line 68
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table89, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Edit item in the Published state with a change that would make the item incomplet" +
            "e")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void EditItemInThePublishedStateWithAChangeThatWouldMakeTheItemIncomplete()
        {
            string[] tagsOfScenario = new string[] {
                    "useChildObjects"};
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Edit item in the Published state with a change that would make the item incomplet" +
                    "e", null, new string[] {
                        "useChildObjects"});
#line 75
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
#line 76
 testRunner.Given("I have created and persisted the DataCatalogWorkflow with Id \'dc-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 77
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table90 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table90.AddRow(new string[] {
                            "Identifier",
                            "identifier1"});
                table90.AddRow(new string[] {
                            "Type",
                            "t1"});
                table90.AddRow(new string[] {
                            "AllowWaitingForDocumentationExit",
                            "x"});
                table90.AddRow(new string[] {
                            "AllowPublishedEntry",
                            "x"});
#line 78
 testRunner.And("I have created and persisted a new instance with Id \'id3\' of the workflow with Id" +
                        " \'dc-workflow\' and supplied the following context items", ((string)(null)), table90, "And ");
#line hidden
                TechTalk.SpecFlow.Table table91 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table91.AddRow(new string[] {
                            "id3",
                            "The notes",
                            "The description"});
#line 84
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'InitialPatch\'", ((string)(null)), table91, "And ");
#line hidden
                TechTalk.SpecFlow.Table table92 = new TechTalk.SpecFlow.Table(new string[] {
                            "Id",
                            "Notes",
                            "Description"});
                table92.AddRow(new string[] {
                            "id3",
                            "",
                            "The new description"});
#line 87
 testRunner.And("I have an object of type \'application/vnd.endjin.datacatalog.catalogitempatchdeta" +
                        "ils\' called \'TestPatch\'", ((string)(null)), table92, "And ");
#line hidden
                TechTalk.SpecFlow.Table table93 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table93.AddRow(new string[] {
                            "{InitialPatch}"});
#line 90
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.editcatalogitemtrigger\'", ((string)(null)), table93, "And ");
#line hidden
                TechTalk.SpecFlow.Table table94 = new TechTalk.SpecFlow.Table(new string[] {
                            "CatalogItemId"});
                table94.AddRow(new string[] {
                            "id3"});
#line 93
 testRunner.And("I have sent the workflow engine a trigger of type \'application/vnd.endjin.datacat" +
                        "alog.publishcatalogitemtrigger\'", ((string)(null)), table94, "And ");
#line hidden
                TechTalk.SpecFlow.Table table95 = new TechTalk.SpecFlow.Table(new string[] {
                            "PatchDetails"});
                table95.AddRow(new string[] {
                            "{TestPatch}"});
#line 96
 testRunner.When("I send the workflow engine a trigger of type \'application/vnd.endjin.datacatalog." +
                        "editcatalogitemtrigger\'", ((string)(null)), table95, "When ");
#line hidden
#line 99
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 100
 testRunner.Then("the workflow instance with Id \'id3\' should have status \'Waiting\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 101
 testRunner.And("the workflow instance with Id \'id3\' should be in the state called \'Waiting for do" +
                        "cumentation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 102
 testRunner.And("the data catalog item with Id \'id3\' should have an Identifier of \'identifier1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 103
 testRunner.And("the data catalog item with Id \'id3\' should have a Type of \'t1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 104
 testRunner.And("the data catalog item with Id \'id3\' should have a Description of \'The new descrip" +
                        "tion\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 105
 testRunner.And("the data catalog item with Id \'id3\' should have Notes of \'\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table96 = new TechTalk.SpecFlow.Table(new string[] {
                            "Message"});
                table96.AddRow(new string[] {
                            "Exiting state \'Published\'"});
                table96.AddRow(new string[] {
                            "Executing transition \'Edit (incomplete)\'"});
                table96.AddRow(new string[] {
                            "Entering state \'Waiting for documentation\'"});
#line 106
 testRunner.And("the following trace messages should be the last messages recorded", ((string)(null)), table96, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
