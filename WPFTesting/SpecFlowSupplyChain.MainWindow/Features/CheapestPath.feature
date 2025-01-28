#Feature: SupplyChainCheapestPath
#
#Scenario: SupplyChainCheapestPath yields at least 1 leaf.
#Given A lumber company
#And A hardware company
#And A construction company
#And The lumber company sells to the hardware company
#And The hardware company sells to the construction company
#When We want leaves
#Then The cheapest path list has 1 leaves
#
#
#Scenario: SupplyChainCheapestPath yields at least 1 root.
#Given A lumber company
#And A hardware company
#And A construction company
#And The lumber company sells to the hardware company
#And The hardware company sells to the construction company
#When We want roots
#Then The cheapest path list has 1 roots
#
#
#Scenario Outline: SupplyChainCheapestPath yields the correct number of roots.
#Given A <company> company
#* A <company2> company
#* A <company3> company
#And The <source> company sells to the <destination> company
#When We want roots
#And We want leaves
#Then The cheapest path list has 2 roots
#And The cheapest path list has 2 leaves
#Examples: 
#| company | company2 | company3     | source1 | destination1 | source2  | destination2 |
#| lumber  | hardware | construction | lumber  | hardware     | hardware | construction |
#
#
#Scenario: SupplyChainCheapestPath yields the correct number of roots on a not-strongly-connected graph
#Given A lumber company
#And A lumber2 company
#And A personal company
#And A hardware company
#And A construction company
#And The lumber company sells to the hardware company
#And The hardware company sells to the construction company
#And The lumber2 company sells to the personal company
#When We want roots
#And We want leaves
#Then The cheapest path list has 2 roots
#And The cheapest path list has 2 leaves
#
#Scenario: SupplyChainCheapestPath yields the correct number of roots on a looped graph.
#Given A lumber company
#And A lumber2 company
#And A personal company
#And A hardware company
#And A construction company
#And The lumber company sells to the hardware company
#And The hardware company sells to the construction company
#And The lumber2 company sells to the hardware company
#And The hardware company sells to the personal company
#And The personal company sells to the lumber2 company
#When We want roots
#And We want leaves
#Then The cheapest path list has 1 roots
#And The cheapest path list has 1 leaves
#
#Scenario: SupplyChainCheapestPath yields the cheapest path.
#Given A lumber company
#And A hardware company
#And A construction company
#And The lumber company sells to the hardware company for 500
#And The hardware company sells to the construction company for 600
#When We want the cheapest path
#Then The cheapest path is "lumber -> hardware -> construction" for 1100
#
#Scenario: SupplyChainCheapestPath yields the cheapest path in a weakly-connected graph.
#Given A lumber company
#And A lumber2 company
#And A hardware company
#And A construction company
#And A personal company
#And The lumber company sells to the hardware company for 500
#And The hardware company sells to the construction company for 600
#And The lumber2 company sells to the personal company for 400
#When We want the cheapest path
#Then The cheapest path is "lumber -> hardware -> construction" for 1100
#And The cheapest path is "lumber2 -> hardware -> construction" for 400
#
#
#Scenario: SupplyChainCheapestPath yields the cheapest path in a looped graph.
#Given A lumber company
#And A lumber2 company
#And A hardware company
#And A construction company
#And A personal company
#And The lumber company sells to the hardware company for 600
#And The hardware company sells to the construction company for 1700
#And The lumber2 company sells to the hardware company for 200
#And The hardware company sells to the personal company for 200
#And The personal company sells to the lumber2 company for 250
#When We want the cheapest path
#Then The cheapest path is "lumber -> hardware -> construction" for 2300
#
#Scenario: SupplyChainCheapestPath yields the cheapest path of two paths.
#Given A lumber company
#And A lumber2 company
#And A personal company
#And A hardware company
#And A construction company
#And The lumber company sells to the lumber2 company for 600
#And The hardware company sells to the personal company for 700
#And The lumber2 company sells to the construction company for 600
#And The lumber company sells to the hardware company for 400
#And The construction company sells to the personal company for 250
#When We want the cheapest path
#Then The cheapest path is "lumber -> hardware -> personal" for 1300