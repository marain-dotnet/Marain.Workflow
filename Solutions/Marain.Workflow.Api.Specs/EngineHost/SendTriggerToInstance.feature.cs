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
namespace Marain.Workflows.Api.Specs.EngineHost
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("SendTriggerToInstance")]
    [NUnit.Framework.CategoryAttribute("perFeatureContainer")]
    [NUnit.Framework.CategoryAttribute("useWorkflowEngineApi")]
    public partial class SendTriggerToInstanceFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "SendTriggerToInstance.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "SendTriggerToInstance", "\tIn order to tell the workflow engine to carry out actions\r\n\tAs an external user " +
                    "of the workflow engine\r\n\tI want to send a trigger to a specific workflow instanc" +
                    "e", ProgrammingLanguage.CSharp, new string[] {
                        "perFeatureContainer",
                        "useWorkflowEngineApi"});
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
        [NUnit.Framework.DescriptionAttribute("Send a trigger")]
        [NUnit.Framework.CategoryAttribute("useChildObjects")]
        public virtual void SendATrigger()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send a trigger", null, new string[] {
                        "useChildObjects"});
#line 9
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 10
 testRunner.Given("I have added the workflow \'SimpleExpensesWorkflow\' to the workflow store with Id " +
                    "\'simple-expenses-workflow\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 11
 testRunner.And("The workflow instance store is empty", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "Key",
                        "Value"});
            table5.AddRow(new string[] {
                        "Claimant",
                        "J George"});
            table5.AddRow(new string[] {
                        "CostCenter",
                        "GD3724"});
#line 12
 testRunner.And("I have a dictionary called \"context\"", ((string)(null)), table5, "And ");
#line 16
 testRunner.And("I have started an instance of the workflow \'simple-expenses-workflow\' with instan" +
                    "ce id \'instance\' and using context object \'context\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "TriggerName"});
            table6.AddRow(new string[] {
                        "Submit"});
#line 17
 testRunner.And("I have an object of type \"application/vnd.marain.workflows.hosted.trigger\" called" +
                    " \"trigger\"", ((string)(null)), table6, "And ");
#line 20
 testRunner.When("I post the object called \'trigger\' to the workflow engine path \'/{tenantId}/marai" +
                    "n/workflow/engine/workflowinstances/instance/triggers\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 21
 testRunner.Then("I should have received a 200 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 22
 testRunner.And("the workflow instance with id \'instance\' should be in the state with name \'Waitin" +
                    "g for approval\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Send a trigger with an invalid workflow instance Id")]
        public virtual void SendATriggerWithAnInvalidWorkflowInstanceId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send a trigger with an invalid workflow instance Id", null, ((string[])(null)));
#line 24
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "TriggerName"});
            table7.AddRow(new string[] {
                        "Submit"});
#line 25
 testRunner.Given("I have an object of type \"application/vnd.marain.workflows.hosted.trigger\" called" +
                    " \"trigger\"", ((string)(null)), table7, "Given ");
#line 28
 testRunner.When("I post the object called \'trigger\' to the workflow engine path \'/{tenantId}/marai" +
                    "n/workflow/engine/workflowinstances/a-non-existant-workflow-id/triggers\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 29
 testRunner.Then("I should have received a 404 status code from the HTTP request", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
