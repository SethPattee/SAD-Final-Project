Feature: SupplyChainCheapestPath

Scenario: SupplyChainCheapestPath yields the correct number of leaves.
	Given A lumber company
	And A hardware company
	And A construction company
	And The lumber company sells to the hardware company
	And The hardware company sells to the construction company
	When We want leaves
	Then The cheapest path list has 1 leaves


Scenario: SupplyChainCheapestPath yields the correct number of roots.
	Given The lumber company sells to the hardware company
	And The hardware company sells to the construction company
	When The cheapest path has leaves
	Then The cheapest path list has 1 roots