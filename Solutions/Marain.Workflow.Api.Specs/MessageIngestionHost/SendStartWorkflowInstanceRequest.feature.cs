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
namespace Marain.Workflows.Functions.Specs.MessageIngestionHost
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("SendStartWorkflowInstanceRequest")]
    [NUnit.Framework.CategoryAttribute("setupContainer")]
    public partial class SendStartWorkflowInstanceRequestFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "SendStartWorkflowInstanceRequest.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SendStartWorkflowInstanceRequest", "\tIn order to tell the workflow engine to carry out actions\r\n\tAs an external user " +
                    "of the workflow engine\r\n\tI want to send a request to start a new workflow instan" +
                    "ce", ProgrammingLanguage.CSharp, new string[] {
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
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Send a request to start a new workflow instance")]
        public virtual void SendARequestToStartANewWorkflowInstance()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send a request to start a new workflow instance", null, ((string[])(null)));
#line 7
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
 testRunner.Given("I start a functions instance for the local project \'Endjin.Workflow.Functions.Mes" +
                    "sageIngestionHost\' on port 7071", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.Given("I start a functions instance for the local project \'Endjin.Operations.Functions.O" +
                    "perationsControlHost\' on port 7078", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table13 = new TechTalk.SpecFlow.Table(new string[] {
                        "WorkflowId"});
            table13.AddRow(new string[] {
                        "target-workflow-id"});
#line 10
 testRunner.And("I have an object of type \"application/vnd.marain.workflows.hosted.startworkflowin" +
                    "stancerequest\" called \"request\"", ((string)(null)), table13, "And ");
#line 13
 testRunner.And("I am listening for events from the event hub", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 14
 testRunner.When("I post the object called \"request\" to the endpoint \"http://localhost:7071/startne" +
                    "wworkflowinstancerequests\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 15
 testRunner.And("wait for up to 3 seconds for incoming events from the event hub", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 16
 testRunner.Then("I should have received a 202 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 17
 testRunner.And("I should have received a start new workflow instance message containing JSON data" +
                    " that represents the object called \"request\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 18
 testRunner.And("I should not have received an exception from processing events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Send multiple requests to start a new workflow instance")]
        public virtual void SendMultipleRequestsToStartANewWorkflowInstance()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send multiple requests to start a new workflow instance", null, ((string[])(null)));
#line 20
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 21
 testRunner.Given("I start a functions instance for the local project \'Endjin.Workflow.Functions.Mes" +
                    "sageIngestionHost\' on port 7071", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 22
 testRunner.Given("I start a functions instance for the local project \'Endjin.Operations.Functions.O" +
                    "perationsControlHost\' on port 7078", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table14 = new TechTalk.SpecFlow.Table(new string[] {
                        "WorkflowId",
                        "WorkflowInstanceId"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-0"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-1"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-2"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-3"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-4"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-5"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-6"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-7"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-8"});
            table14.AddRow(new string[] {
                        "target-workflow-id",
                        "instance-9"});
#line 23
 testRunner.And("I have an object of type \"application/vnd.marain.workflows.hosted.startworkflowin" +
                    "stancerequest\" called \"requests\"", ((string)(null)), table14, "And ");
#line 35
 testRunner.And("I am listening for events from the event hub", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 36
 testRunner.When("I post the object called \"requests\" to the endpoint \"http://localhost:7071/startn" +
                    "ewworkflowinstancerequests\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 37
 testRunner.And("wait for up to 3 seconds for incoming events from the event hub", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 38
 testRunner.Then("I should have received 10 202 status codes from the HTTP requests", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 39
 testRunner.And("I should have received at least 10 start new workflow instance messages containin" +
                    "g JSON data that represents the object called \"requests\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 40
 testRunner.And("I should not have received an exception from processing events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
