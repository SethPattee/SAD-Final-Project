Feature: MainWindow
![Calculator](https://specflow.org/wp-content/uploads/2020/09/calculator.png)
Simple calculator for adding **two** numbers

Link to a feature: [Calculator](SpecFlowSupplyChain.MainWindow/Features/Calculator.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**

@mytag
Scenario: Get line offset for a standard box
	Given a standard box is generated
	And the box's NE_Radial button is clicked
	When the point for the line is computed
	Then the line's Point should be 100, 0

@mytag
Scenario: Get line offset for a big box
	Given a box with height 325 and width 450 is generated
	And the box's S_Radial button is clicked
	When the point for the line is computed
	Then the line's Point should be 225, 325 