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
namespace Marain.Workflows.Api.Specs.EngineHost
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.7.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Update workflow")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useWorkflowEngineApi")]
    [NUnit.Framework.CategoryAttribute("useTransientTenant")]
    [NUnit.Framework.CategoryAttribute("useChildObjects")]
    public partial class UpdateWorkflowFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "perFeatureContainer",
                "useWorkflowEngineApi",
                "useTransientTenant",
                "useChildObjects"};
        
#line 1 "UpdateWorkflow.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "EngineHost", "Update workflow", "\tIn order to modify a workflow definition in the engine\r\n\tAs an external user of " +
                    "the workflow engine\r\n\tI want to be able to store an updated workflow definition", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useWorkflowEngineApi",
                        "useTransientTenant",
                        "useChildObjects"});
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
        [NUnit.Framework.DescriptionAttribute("Update a workflow definition without supplying an eTag returns a Conflict respons" +
            "e")]
        public virtual void UpdateAWorkflowDefinitionWithoutSupplyingAnETagReturnsAConflictResponse()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Update a workflow definition without supplying an eTag returns a Conflict respons" +
                    "e", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
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
 testRunner.Given("I have added the workflow \'SimpleExpensesWorkflow\' to the workflow store with Id " +
                        "\'simple-expenses-workflow-1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 12
 testRunner.When("I put the workflow called \'SimpleExpensesWorkflow\' to the workflow engine path \'/" +
                        "{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 13
 testRunner.Then("I should have received a 409 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Update a workflow definition with an etag succeeds")]
        public virtual void UpdateAWorkflowDefinitionWithAnEtagSucceeds()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Update a workflow definition with an etag succeeds", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 15
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
#line 16
 testRunner.Given("I have added the workflow \'SimpleExpensesWorkflow\' to the workflow store with Id " +
                        "\'simple-expenses-workflow-2\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 17
 testRunner.When("I put the workflow called \'SimpleExpensesWorkflow\' to the workflow engine path \'/" +
                        "{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-2\' with an " +
                        "If-Match header value from the etag of the workflow", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 18
 testRunner.Then("I should have received a 200 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Attempt to update a workflow definition with an etag that does not match the stor" +
            "ed version returns a Precondition Failed response")]
        public virtual void AttemptToUpdateAWorkflowDefinitionWithAnEtagThatDoesNotMatchTheStoredVersionReturnsAPreconditionFailedResponse()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Attempt to update a workflow definition with an etag that does not match the stor" +
                    "ed version returns a Precondition Failed response", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 20
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
#line 21
 testRunner.Given("I have added the workflow \'SimpleExpensesWorkflow\' to the workflow store with Id " +
                        "\'simple-expenses-workflow-3\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 22
 testRunner.When("I put the workflow called \'SimpleExpensesWorkflow\' to the workflow engine path \'/" +
                        "{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-3\' with \'\"3" +
                        "3a64df551425fcc55e4d42a148795d9f25f89d4\"\' as the If-Match header value", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 23
 testRunner.Then("I should have received a 412 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Attempt to store a workflow definition when a definition with the supplied Id doe" +
            "s not exist succeeds and creates the new workflow in the store")]
        public virtual void AttemptToStoreAWorkflowDefinitionWhenADefinitionWithTheSuppliedIdDoesNotExistSucceedsAndCreatesTheNewWorkflowInTheStore()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Attempt to store a workflow definition when a definition with the supplied Id doe" +
                    "s not exist succeeds and creates the new workflow in the store", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 25
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
#line 26
 testRunner.Given("I have an instance of the workflow \'SimpleExpensesWorkflow\' with Id \'simple-expen" +
                        "ses-workflow-4\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 27
 testRunner.When("I put the workflow called \'SimpleExpensesWorkflow\' to the workflow engine path \'/" +
                        "{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-4\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 28
 testRunner.Then("I should have received a 200 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 29
 testRunner.And("there should be a workflow with the id \'simple-expenses-workflow-4\' in the workfl" +
                        "ow store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Attempt to store a workflow definition when the workflow Id in the path doesn\'t m" +
            "atch that in the body returns a Bad Request response")]
        public virtual void AttemptToStoreAWorkflowDefinitionWhenTheWorkflowIdInThePathDoesntMatchThatInTheBodyReturnsABadRequestResponse()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Attempt to store a workflow definition when the workflow Id in the path doesn\'t m" +
                    "atch that in the body returns a Bad Request response", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 31
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
#line 32
 testRunner.Given("I have added the workflow \'SimpleExpensesWorkflow\' to the workflow store with Id " +
                        "\'simple-expenses-workflow-5\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 33
 testRunner.When("I put the workflow called \'SimpleExpensesWorkflow\' to the workflow engine path \'/" +
                        "{tenantId}/marain/workflow/engine/workflows/the-incorrect-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 34
 testRunner.Then("I should have received a 400 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
