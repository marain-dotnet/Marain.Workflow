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
namespace Marain.Workflows.Specs.Features.Cosmos
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.5.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("InvokeExternalServiceAction with Cosmos")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useCosmosStores")]
    [NUnit.Framework.CategoryAttribute("setupTenantedCosmosContainers")]
    public partial class InvokeExternalServiceActionWithCosmosFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useCosmosStores",
                "setupTenantedCosmosContainers"};
        
#line 1 "InvokeExternalServiceAction.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features/Cosmos", "InvokeExternalServiceAction with Cosmos", "    In order to be able to define a workflow that invokes external HTTP endpoints" +
                    "\r\n    As a developer defining a workflow\r\n    I want to be able to define a work" +
                    "flow action that invokes an external HTTP endpoint", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useCosmosStores",
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
        [NUnit.Framework.DescriptionAttribute("External action invoked")]
        [NUnit.Framework.CategoryAttribute("externalServiceRequired")]
        public virtual void ExternalActionInvoked()
        {
            string[] tagsOfScenario = new string[] {
                    "externalServiceRequired"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("External action invoked", null, tagsOfScenario, argumentsOfScenario);
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
    testRunner.Given("I have created and persisted a workflow containing an external action with id \'ex" +
                        "ternal-action-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table103 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table103.AddRow(new string[] {
                            "include1",
                            "value1"});
                table103.AddRow(new string[] {
                            "include2",
                            "value2"});
                table103.AddRow(new string[] {
                            "dontinclude",
                            "value3"});
#line 12
 testRunner.And("I have created and persisted a new instance with Id \'id1\' of the workflow with Id" +
                        " \'external-action-workflow\' and supplied the following context items", ((string)(null)), table103, "And ");
#line hidden
#line 17
    testRunner.And("the external service will return a 200 status", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 18
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 19
    testRunner.When("I send a trigger that will execute the action with a trigger id of \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 20
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 21
    testRunner.Then("the action endpoint should have been invoked", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 22
    testRunner.And("the Authorization header should be of type Bearer, using a token representing the" +
                        " managed service identity with the resource specified by the action", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 23
    testRunner.And("the Content-Type header should be \'application/vnd.marain.workflows.externalservi" +
                        "cerequest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 24
    testRunner.And("the request body WorkflowId should be \'external-action-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 25
    testRunner.And("the request body WorkflowInstanceId should be \'id1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
    testRunner.And("the request body InvokedById should match the action id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 27
    testRunner.And("the request body Trigger matches the input trigger", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 28
    testRunner.And("the request body ContextProperties key \'include1\' has the value \'value1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 29
    testRunner.And("the request body ContextProperties key \'include2\' has the value \'value2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 30
    testRunner.And("the request body ContextProperties has 2 values", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 31
 testRunner.Then("the workflow instance with Id \'id1\' should have status \'Complete\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 32
 testRunner.And("the workflow instance with Id \'id1\' should be in the state called \'Done\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table104 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table104.AddRow(new string[] {
                            "include1",
                            "value1"});
                table104.AddRow(new string[] {
                            "include2",
                            "value2"});
                table104.AddRow(new string[] {
                            "dontinclude",
                            "value3"});
#line 33
    testRunner.And("the workflow instance with Id \'id1\' should contain context items", ((string)(null)), table104, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("External action returns a 200 status code and instance context updates")]
        [NUnit.Framework.CategoryAttribute("externalServiceRequired")]
        public virtual void ExternalActionReturnsA200StatusCodeAndInstanceContextUpdates()
        {
            string[] tagsOfScenario = new string[] {
                    "externalServiceRequired"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("External action returns a 200 status code and instance context updates", null, tagsOfScenario, argumentsOfScenario);
#line 40
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
#line 41
    testRunner.Given("I have created and persisted a workflow containing an external action with id \'ex" +
                        "ternal-action-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table105 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table105.AddRow(new string[] {
                            "include1",
                            "value1"});
                table105.AddRow(new string[] {
                            "include2",
                            "value2"});
                table105.AddRow(new string[] {
                            "dontinclude",
                            "value3"});
#line 42
 testRunner.And("I have created and persisted a new instance with Id \'id2\' of the workflow with Id" +
                        " \'external-action-workflow\' and supplied the following context items", ((string)(null)), table105, "And ");
#line hidden
#line 47
    testRunner.And("the external service will return a 200 status", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table106 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table106.AddRow(new string[] {
                            "include1",
                            "value4"});
                table106.AddRow(new string[] {
                            "added1",
                            "value5"});
#line 48
    testRunner.And("the external service response will specify context items to upsert", ((string)(null)), table106, "And ");
#line hidden
                TechTalk.SpecFlow.Table table107 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key"});
                table107.AddRow(new string[] {
                            "include2"});
#line 52
    testRunner.And("the external service response will specify context items to remove", ((string)(null)), table107, "And ");
#line hidden
#line 55
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 56
    testRunner.When("I send a trigger that will execute the action with a trigger id of \'id2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 57
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 58
    testRunner.Then("the action endpoint should have been invoked", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 59
    testRunner.And("the Authorization header should be of type Bearer, using a token representing the" +
                        " managed service identity with the resource specified by the action", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 60
    testRunner.And("the Content-Type header should be \'application/vnd.marain.workflows.externalservi" +
                        "cerequest\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 61
    testRunner.And("the request body WorkflowId should be \'external-action-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 62
    testRunner.And("the request body WorkflowInstanceId should be \'id2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 63
    testRunner.And("the request body InvokedById should match the action id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 64
    testRunner.And("the request body Trigger matches the input trigger", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 65
    testRunner.And("the request body ContextProperties key \'include1\' has the value \'value1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 66
    testRunner.And("the request body ContextProperties key \'include2\' has the value \'value2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 67
    testRunner.And("the request body ContextProperties has 2 values", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 68
 testRunner.Then("the workflow instance with Id \'id2\' should have status \'Complete\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 69
 testRunner.And("the workflow instance with Id \'id2\' should be in the state called \'Done\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table108 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table108.AddRow(new string[] {
                            "include1",
                            "value4"});
                table108.AddRow(new string[] {
                            "dontinclude",
                            "value3"});
                table108.AddRow(new string[] {
                            "added1",
                            "value5"});
#line 70
    testRunner.And("the workflow instance with Id \'id2\' should contain context items", ((string)(null)), table108, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("External action returns a 500 status code")]
        [NUnit.Framework.CategoryAttribute("externalServiceRequired")]
        public virtual void ExternalActionReturnsA500StatusCode()
        {
            string[] tagsOfScenario = new string[] {
                    "externalServiceRequired"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("External action returns a 500 status code", null, tagsOfScenario, argumentsOfScenario);
#line 77
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
#line 78
    testRunner.Given("I have created and persisted a workflow containing an external action with id \'ex" +
                        "ternal-action-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
                TechTalk.SpecFlow.Table table109 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key",
                            "Value"});
                table109.AddRow(new string[] {
                            "include1",
                            "value1"});
                table109.AddRow(new string[] {
                            "include2",
                            "value2"});
                table109.AddRow(new string[] {
                            "dontinclude",
                            "value3"});
#line 79
 testRunner.And("I have created and persisted a new instance with Id \'id3\' of the workflow with Id" +
                        " \'external-action-workflow\' and supplied the following context items", ((string)(null)), table109, "And ");
#line hidden
#line 84
    testRunner.And("the external service will return a 500 status", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 85
 testRunner.And("the workflow trigger queue is ready to process new triggers", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 86
    testRunner.When("I send a trigger that will execute the action with a trigger id of \'id3\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 87
 testRunner.And("I wait for all triggers to be processed", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 88
    testRunner.Then("the action endpoint should have been invoked", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 89
 testRunner.Then("the workflow instance with Id \'id3\' should have status \'Faulted\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 90
 testRunner.And("the workflow instance with Id \'id3\' should be in the state called \'Waiting to run" +
                        "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
