using WPFTesting.Shapes;
using SpecFlowSupplyChain.MainWindow.Test_Classes;
using YourNamespace;
using System.Windows;
using NUnit.Framework;

namespace SpecFlowSupplyChain.MainWindowTest.StepDefinitions
{
    [Binding]
    public sealed class MainWindowStepDefinitions
    {
        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        private readonly ScenarioContext _scenarioContext;
        public MainWindowStepDefinitions(ScenarioContext sc)
        {
            _scenarioContext = sc;
        }

        [Given("a standard box is generated")]
        public void GivenAStandardBoxIsGenerated(int number)
        {
            //TODO: implement arrange (precondition) logic
            // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
            // To use the multiline text or the table argument of the scenario,
            // additional string/Table parameters can be defined on the step definition
            // method. 
            TestMainWindow testWindow = new TestMainWindow();
            BoxValues boxValues = new BoxValues();
            DraggableBox db = new DraggableBox(boxValues);
            db.Name = "goodbox";
            db.CornerClicked = "NE_Radial";

            _scenarioContext.Add("Window", testWindow);
            _scenarioContext.Add("Box", db);
        }

        [When("the point for the line is computed")]
        public void WhenThePointForTheLineIsComputed()
        {
            var testWindow = _scenarioContext.Get<TestMainWindow>("Window");
            var db =_scenarioContext.Get<DraggableBox>("Box");

            Point actualpoint = testWindow.GetLineOffset(db);

            _scenarioContext.Add("actualpoint", actualpoint);
        }

        [Then("the line's Point should be (.*), (.*)")]
        public void ThenTheResultShouldBe(double x, double y)
        {
            Point expected = new Point(x, y);
            var actual = _scenarioContext.Get<Point>("actualpoint");

            actual.Should().Be(expected);
        }
    }
}
