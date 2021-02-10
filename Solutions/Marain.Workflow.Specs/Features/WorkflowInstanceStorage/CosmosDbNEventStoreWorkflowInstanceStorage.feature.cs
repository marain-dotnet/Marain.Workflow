﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.5.0.0
//      SpecFlow Generator Version:3.5.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Marain.Workflows.Specs.Features.WorkflowInstanceStorage
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.5.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Workflow instance store - CosmosDb NEventStore version")]
    [NUnit.Framework.CategoryAttribute("perScenarioContainer")]
    [NUnit.Framework.CategoryAttribute("usingCosmosDbNEventStore")]
    public partial class WorkflowInstanceStore_CosmosDbNEventStoreVersionFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perScenarioContainer",
                "usingCosmosDbNEventStore"};
        
#line 1 "CosmosDbNEventStoreWorkflowInstanceStorage.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/WorkflowInstanceStorage", "Workflow instance store - CosmosDb NEventStore version", null, ProgrammingLanguage.CSharp, new string[] {
                        "perScenarioContainer",
                        "usingCosmosDbNEventStore"});
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
        
        public virtual void FeatureBackground()
        {
#line 6
#line hidden
#line 7
 testRunner.Given("I have a data catalog workflow definition with Id \'workflow1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table118 = new TechTalk.SpecFlow.Table(new string[] {
                        "Key",
                        "Value"});
            table118.AddRow(new string[] {
                        "Context1",
                        "Value1"});
            table118.AddRow(new string[] {
                        "Context2",
                        "Value2"});
#line 8
 testRunner.And("I have a context dictionary called \'Context1\':", ((string)(null)), table118, "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Store a workflow instance that is in the initialising state")]
        public virtual void StoreAWorkflowInstanceThatIsInTheInitialisingState()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Store a workflow instance that is in the initialising state", null, tagsOfScenario, argumentsOfScenario);
#line 13
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table119 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table119.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 14
 testRunner.Given("I have created a new workflow instance called \'instance1\'", ((string)(null)), table119, "Given ");
#line hidden
#line 17
 testRunner.When("I store the workflow instance called \'instance1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 18
 testRunner.Then("no exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table120 = new TechTalk.SpecFlow.Table(new string[] {
                            "Property",
                            "Value"});
                table120.AddRow(new string[] {
                            "IsDirty",
                            "false"});
#line 19
 testRunner.And("the workflow instance called \'instance1\' should have the following properties:", ((string)(null)), table120, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Store a workflow instance that is initialised")]
        public virtual void StoreAWorkflowInstanceThatIsInitialised()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Store a workflow instance that is initialised", null, tagsOfScenario, argumentsOfScenario);
#line 23
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table121 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table121.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 24
 testRunner.Given("I have created a new workflow instance called \'instance1\'", ((string)(null)), table121, "Given ");
#line hidden
#line 27
 testRunner.And("I have set the workflow instance called \'instance1\' as having entered the state \'" +
                        "initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
 testRunner.When("I store the workflow instance called \'instance1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 29
 testRunner.Then("no exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table122 = new TechTalk.SpecFlow.Table(new string[] {
                            "Property",
                            "Value"});
                table122.AddRow(new string[] {
                            "IsDirty",
                            "false"});
#line 30
 testRunner.And("the workflow instance called \'instance1\' should have the following properties:", ((string)(null)), table122, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Load an initialized workflow instance")]
        public virtual void LoadAnInitializedWorkflowInstance()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Load an initialized workflow instance", null, tagsOfScenario, argumentsOfScenario);
#line 34
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table123 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table123.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 35
 testRunner.Given("I have created a new workflow instance called \'instance1\'", ((string)(null)), table123, "Given ");
#line hidden
#line 38
 testRunner.And("I have set the workflow instance called \'instance1\' as having entered the state \'" +
                        "initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 39
 testRunner.And("I have stored the workflow instance called \'instance1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 40
 testRunner.When("I load the workflow instance with Id \'instance1\' and call it \'result\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 41
 testRunner.Then("no exception is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
                TechTalk.SpecFlow.Table table124 = new TechTalk.SpecFlow.Table(new string[] {
                            "Property",
                            "Value"});
                table124.AddRow(new string[] {
                            "Id",
                            "instance1"});
                table124.AddRow(new string[] {
                            "Status",
                            "Waiting"});
                table124.AddRow(new string[] {
                            "StateId",
                            "initializing"});
                table124.AddRow(new string[] {
                            "IsDirty",
                            "false"});
#line 42
 testRunner.And("the workflow instance called \'result\' should have the following properties:", ((string)(null)), table124, "And ");
#line hidden
                TechTalk.SpecFlow.Table table125 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table125.AddRow(new string[] {
                            "Context1",
                            "Value1"});
                table125.AddRow(new string[] {
                            "Context2",
                            "Value2"});
#line 48
 testRunner.And("the workflow instance called \'result\' should have the following context:", ((string)(null)), table125, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Load, update and persist a workflow instance")]
        public virtual void LoadUpdateAndPersistAWorkflowInstance()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Load, update and persist a workflow instance", null, tagsOfScenario, argumentsOfScenario);
#line 53
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table126 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table126.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 54
 testRunner.Given("I have created a new workflow instance called \'originalInstance\'", ((string)(null)), table126, "Given ");
#line hidden
#line 57
 testRunner.And("I have set the workflow instance called \'originalInstance\' as having entered the " +
                        "state \'initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 58
 testRunner.And("I have stored the workflow instance called \'originalInstance\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 59
 testRunner.And("I have loaded the workflow instance with Id \'instance1\' and called it \'firstReloa" +
                        "d\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table127 = new TechTalk.SpecFlow.Table(new string[] {
                            "PropertyName",
                            "Value"});
                table127.AddRow(new string[] {
                            "CatalogItemId",
                            "id1"});
#line 60
 testRunner.And("I have started the transition \'create\' for the workflow instance called \'firstRel" +
                        "oad\' with a trigger of type \'application/vnd.endjin.datacatalog.createcatalogite" +
                        "mtrigger\'", ((string)(null)), table127, "And ");
#line hidden
                TechTalk.SpecFlow.Table table128 = new TechTalk.SpecFlow.Table(new string[] {
                            "Operation",
                            "Key",
                            "Value"});
                table128.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context1",
                            "Value1.1"});
#line 63
 testRunner.And("I have set the workflow instance called \'firstReload\' as having exited the curren" +
                        "t state with the following context updates:", ((string)(null)), table128, "And ");
#line hidden
                TechTalk.SpecFlow.Table table129 = new TechTalk.SpecFlow.Table(new string[] {
                            "Operation",
                            "Key",
                            "Value"});
                table129.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context1",
                            "Value1.2"});
                table129.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context3",
                            "Value3"});
                table129.AddRow(new string[] {
                            "Remove",
                            "Context2",
                            ""});
#line 66
 testRunner.And("I have set the workflow instance called \'firstReload\' as having executed transiti" +
                        "on actions with the following context updates:", ((string)(null)), table129, "And ");
#line hidden
                TechTalk.SpecFlow.Table table130 = new TechTalk.SpecFlow.Table(new string[] {
                            "Operation",
                            "Key",
                            "Value"});
                table130.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context4",
                            "Value4.1"});
                table130.AddRow(new string[] {
                            "Remove",
                            "Context3",
                            ""});
#line 71
 testRunner.And("I have set the workflow instance called \'firstReload\' as having entered the state" +
                        " \'waiting-for-documentation\' with the following context updates:", ((string)(null)), table130, "And ");
#line hidden
#line 75
 testRunner.And("I have stored the workflow instance called \'firstReload\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 76
 testRunner.When("I load the workflow instance with Id \'instance1\' and call it \'result\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table131 = new TechTalk.SpecFlow.Table(new string[] {
                            "Property",
                            "Value"});
                table131.AddRow(new string[] {
                            "Id",
                            "instance1"});
                table131.AddRow(new string[] {
                            "Status",
                            "Waiting"});
                table131.AddRow(new string[] {
                            "StateId",
                            "waiting-for-documentation"});
                table131.AddRow(new string[] {
                            "IsDirty",
                            "false"});
#line 77
 testRunner.Then("the workflow instance called \'result\' should have the following properties:", ((string)(null)), table131, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table132 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table132.AddRow(new string[] {
                            "Context1",
                            "Value1.2"});
                table132.AddRow(new string[] {
                            "Context4",
                            "Value4.1"});
#line 83
 testRunner.And("the workflow instance called \'result\' should have the following context:", ((string)(null)), table132, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Attempt to persist a workflow instance that has been modified elsewhere since it " +
            "was loaded")]
        public virtual void AttemptToPersistAWorkflowInstanceThatHasBeenModifiedElsewhereSinceItWasLoaded()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Attempt to persist a workflow instance that has been modified elsewhere since it " +
                    "was loaded", null, tagsOfScenario, argumentsOfScenario);
#line 88
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table133 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table133.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 89
 testRunner.Given("I have created a new workflow instance called \'originalInstance\'", ((string)(null)), table133, "Given ");
#line hidden
#line 92
 testRunner.And("I have set the workflow instance called \'originalInstance\' as having entered the " +
                        "state \'initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 93
 testRunner.And("I have stored the workflow instance called \'originalInstance\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 94
 testRunner.And("I have loaded the workflow instance with Id \'instance1\' and called it \'loadedInst" +
                        "ance1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 95
 testRunner.And("I have loaded the workflow instance with Id \'instance1\' and called it \'loadedInst" +
                        "ance2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table134 = new TechTalk.SpecFlow.Table(new string[] {
                            "PropertyName",
                            "Value"});
                table134.AddRow(new string[] {
                            "CatalogItemId",
                            "id1"});
#line 96
 testRunner.And("I have started the transition \'create\' for the workflow instance called \'loadedIn" +
                        "stance1\' with a trigger of type \'application/vnd.endjin.datacatalog.createcatalo" +
                        "gitemtrigger\'", ((string)(null)), table134, "And ");
#line hidden
                TechTalk.SpecFlow.Table table135 = new TechTalk.SpecFlow.Table(new string[] {
                            "PropertyName",
                            "Value"});
                table135.AddRow(new string[] {
                            "CatalogItemId",
                            "id2"});
#line 99
 testRunner.And("I have started the transition \'create\' for the workflow instance called \'loadedIn" +
                        "stance2\' with a trigger of type \'application/vnd.endjin.datacatalog.createcatalo" +
                        "gitemtrigger\'", ((string)(null)), table135, "And ");
#line hidden
#line 102
 testRunner.And("I have stored the workflow instance called \'loadedInstance1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 103
 testRunner.When("I store the workflow instance called \'loadedInstance2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 104
 testRunner.Then("an \'InvalidOperationException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Attempt to persist a new workflow instance when an instance with the same Id alre" +
            "ady exists")]
        public virtual void AttemptToPersistANewWorkflowInstanceWhenAnInstanceWithTheSameIdAlreadyExists()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Attempt to persist a new workflow instance when an instance with the same Id alre" +
                    "ady exists", null, tagsOfScenario, argumentsOfScenario);
#line 106
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table136 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table136.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 107
 testRunner.Given("I have created a new workflow instance called \'instance1\'", ((string)(null)), table136, "Given ");
#line hidden
#line 110
 testRunner.And("I have set the workflow instance called \'instance1\' as having entered the state \'" +
                        "initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table137 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table137.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 111
 testRunner.Given("I have created a new workflow instance called \'instance2\'", ((string)(null)), table137, "Given ");
#line hidden
#line 114
 testRunner.And("I have set the workflow instance called \'instance2\' as having entered the state \'" +
                        "initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 115
 testRunner.And("I have stored the workflow instance called \'instance1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 116
 testRunner.When("I store the workflow instance called \'instance2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 117
 testRunner.Then("a \'ConcurrencyException\' is thrown", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Write and read a commit containing a large number of events")]
        public virtual void WriteAndReadACommitContainingALargeNumberOfEvents()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Write and read a commit containing a large number of events", null, tagsOfScenario, argumentsOfScenario);
#line 119
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
#line 6
this.FeatureBackground();
#line hidden
                TechTalk.SpecFlow.Table table138 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table138.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 120
 testRunner.Given("I have created a new workflow instance called \'instance1\'", ((string)(null)), table138, "Given ");
#line hidden
#line 123
 testRunner.And("I have set the workflow instance called \'instance1\' as having entered the state \'" +
                        "initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table139 = new TechTalk.SpecFlow.Table(new string[] {
                            "InstanceId",
                            "WorkflowId",
                            "Context"});
                table139.AddRow(new string[] {
                            "instance1",
                            "workflow1",
                            "{Context1}"});
#line 124
 testRunner.Given("I have created a new workflow instance called \'originalInstance\'", ((string)(null)), table139, "Given ");
#line hidden
#line 127
 testRunner.And("I have set the workflow instance called \'originalInstance\' as having entered the " +
                        "state \'initializing\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table140 = new TechTalk.SpecFlow.Table(new string[] {
                            "PropertyName",
                            "Value"});
                table140.AddRow(new string[] {
                            "CatalogItemId",
                            "id1"});
#line 128
 testRunner.And("I have started the transition \'create\' for the workflow instance called \'original" +
                        "Instance\' with a trigger of type \'application/vnd.endjin.datacatalog.createcatal" +
                        "ogitemtrigger\'", ((string)(null)), table140, "And ");
#line hidden
                TechTalk.SpecFlow.Table table141 = new TechTalk.SpecFlow.Table(new string[] {
                            "Operation",
                            "Key",
                            "Value"});
                table141.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context1",
                            "Value1.1"});
#line 131
 testRunner.And("I have set the workflow instance called \'originalInstance\' as having exited the c" +
                        "urrent state with the following context updates:", ((string)(null)), table141, "And ");
#line hidden
                TechTalk.SpecFlow.Table table142 = new TechTalk.SpecFlow.Table(new string[] {
                            "Operation",
                            "Key",
                            "Value"});
                table142.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context1",
                            "Value1.2"});
                table142.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context3",
                            "Value3"});
                table142.AddRow(new string[] {
                            "Remove",
                            "Context2",
                            ""});
#line 134
 testRunner.And("I have set the workflow instance called \'originalInstance\' as having executed tra" +
                        "nsition actions with the following context updates:", ((string)(null)), table142, "And ");
#line hidden
                TechTalk.SpecFlow.Table table143 = new TechTalk.SpecFlow.Table(new string[] {
                            "Operation",
                            "Key",
                            "Value"});
                table143.AddRow(new string[] {
                            "AddOrUpdate",
                            "Context4",
                            "Value4.1"});
                table143.AddRow(new string[] {
                            "Remove",
                            "Context3",
                            ""});
#line 139
 testRunner.And("I have set the workflow instance called \'originalInstance\' as having entered the " +
                        "state \'waiting-for-documentation\' with the following context updates:", ((string)(null)), table143, "And ");
#line hidden
#line 143
 testRunner.And("I have stored the workflow instance called \'originalInstance\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 144
 testRunner.And("I have loaded the workflow instance with Id \'instance1\' and called it \'reloaded\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 145
 testRunner.And("I apply the \'edit\' transition 200 times to the workflow instance called \'reloaded" +
                        "\', saving on every iteration", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 146
 testRunner.When("I load the workflow instance with Id \'instance1\' and call it \'result\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table144 = new TechTalk.SpecFlow.Table(new string[] {
                            "Property",
                            "Value"});
                table144.AddRow(new string[] {
                            "Id",
                            "instance1"});
                table144.AddRow(new string[] {
                            "Status",
                            "Waiting"});
                table144.AddRow(new string[] {
                            "StateId",
                            "waiting-for-documentation"});
                table144.AddRow(new string[] {
                            "IsDirty",
                            "false"});
#line 147
 testRunner.Then("the workflow instance called \'result\' should have the following properties:", ((string)(null)), table144, "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
