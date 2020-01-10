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
namespace Marain.Workflows.Functions.Specs.EngineHost
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("StartNewInstance")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useWorkflowEngineApi")]
    [NUnit.Framework.CategoryAttribute("useChildObjects")]
    public partial class StartNewInstanceFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "StartNewInstance.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "StartNewInstance", "\tIn order manage a new thing through a workflow\r\n\tAs an external user of the work" +
                    "flow engine\r\n\tI want to be able to start a new instance of a workflow", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useWorkflowEngineApi",
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
        [NUnit.Framework.DescriptionAttribute("Start a new instance with a specified instance id")]
        public virtual void StartANewInstanceWithASpecifiedInstanceId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Start a new instance with a specified instance id", null, ((string[])(null)));
#line 9
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
 testRunner.Given("I have added the workflow \"SimpleExpensesWorkflow\" to the workflow store with Id " +
                    "\"simple-expenses-workflow\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.And("I have cleared down the workflow instance store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                        "Key",
                        "Value"});
            table8.AddRow(new string[] {
                        "Claimant",
                        "J George"});
            table8.AddRow(new string[] {
                        "CostCenter",
                        "GD3724"});
#line 12
 testRunner.And("I have a dictionary called \"context\"", ((string)(null)), table8, "And ");
#line hidden
            TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                        "WorkflowId",
                        "WorkflowInstanceId",
                        "Context"});
            table9.AddRow(new string[] {
                        "simple-expenses-workflow",
                        "instance",
                        "{context}"});
#line 16
 testRunner.And("I have an object of type \"application/vnd.marain.workflows.hosted.startworkflowin" +
                    "stancerequest\" called \"request\"", ((string)(null)), table9, "And ");
#line 19
 testRunner.When("I post the object called \'request\' to the workflow engine path \'/{tenantId}/marai" +
                    "n/workflow/engine/workflowinstances\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 20
 testRunner.Then("I should have received a 201 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 21
 testRunner.And("there should be a workflow instance with the id \"instance\" in the workflow instan" +
                    "ce store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 22
 testRunner.And("the workflow instance with id \"instance\" should be an instance of the workflow wi" +
                    "th id \"simple-expenses-workflow\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 23
 testRunner.And("the workflow instance with id \"instance\" should have a context dictionary that ma" +
                    "tches \"context\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Start a new instance without specifying an instance id")]
        public virtual void StartANewInstanceWithoutSpecifyingAnInstanceId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Start a new instance without specifying an instance id", null, ((string[])(null)));
#line 25
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 26
 testRunner.Given("I have added the workflow \"SimpleExpensesWorkflow\" to the workflow store with Id " +
                    "\"simple-expenses-workflow\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 27
 testRunner.And("I have cleared down the workflow instance store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                        "Key",
                        "Value"});
            table10.AddRow(new string[] {
                        "Claimant",
                        "J George"});
            table10.AddRow(new string[] {
                        "CostCenter",
                        "GD3724"});
#line 28
 testRunner.And("I have a dictionary called \"context\"", ((string)(null)), table10, "And ");
#line hidden
            TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                        "WorkflowId",
                        "Context"});
            table11.AddRow(new string[] {
                        "simple-expenses-workflow",
                        "{context}"});
#line 32
 testRunner.And("I have an object of type \"application/vnd.marain.workflows.hosted.startworkflowin" +
                    "stancerequest\" called \"request\"", ((string)(null)), table11, "And ");
#line 35
 testRunner.When("I post the object called \'request\' to the workflow engine path \'/{tenantId}/marai" +
                    "n/workflow/engine/workflowinstances\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 36
 testRunner.Then("I should have received a 201 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 37
 testRunner.And("there should be 1 workflow instance in the workflow instance store", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Start a new instance with a non-existent workflow id")]
        public virtual void StartANewInstanceWithANon_ExistentWorkflowId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Start a new instance with a non-existent workflow id", null, ((string[])(null)));
#line 39
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line hidden
            TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                        "WorkflowId"});
            table12.AddRow(new string[] {
                        "4629f9f3-a706-4901-a215-df8313376b52"});
#line 40
 testRunner.Given("I have an object of type \"application/vnd.marain.workflows.hosted.startworkflowin" +
                    "stancerequest\" called \"request\"", ((string)(null)), table12, "Given ");
#line 43
 testRunner.When("I post the object called \'request\' to the workflow engine path \'/{tenantId}/marai" +
                    "n/workflow/engine/workflowinstances\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 44
 testRunner.Then("I should have received a 404 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
