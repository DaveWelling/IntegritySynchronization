Feature: Synchronization
	In order to have disconnected clients
	As a service provider
	I want to be able to synchronize data changed in various replicas

@Synchronization
Scenario: Insert a client record
	Given a client app adds a new record R1
	And the record R1 does not exist on the server
	When the client app submits the record for synchronization
	Then the record R1 should be added to the server

Scenario: Delete a client record
	Given a client app deletes a record R0
	And the R0 record exists on the server
	When the client app submits the record for synchronization
	Then the R0 record should be removed from the server

Scenario: Delete a client record which is already deleted
	Given a client app deletes a record R0 
	And the R0 record has been deleted from the server
	When the client app submits the record for synchronization
	Then nothing should happen to record R0 on the server

Scenario: Update a client record which is already deleted
	Given a client has a change record R0 in field A
	And the R0 record has been deleted from the server
	When the client app submits the record for synchronization
	Then nothing should happen to record R0 on the server
	And the R0 record should be deleted from the client

Scenario: Update a client record which does not exist on server
	Given a client has a change record R1 in field A
	And the record R1 does not exist on the server
	When the client app submits the record for synchronization
	Then the record R1 should be added to the server

Scenario: Update a client record which is unchanged on the server
	Given a client has a change record R0 in field A
	And the R0 record exists on the server
	When the client app submits the record for synchronization
	Then the R0 record field A should be updated on the server

Scenario: Update a client record which has been updated on the server in a different field
	Given a client has a change record R0 in field A
	And field B of the record R0 has been changed on the server
	When the client app submits the record for synchronization
	Then the server R0 record should have client value in field A and server value in field B

Scenario: Update a client record which has been updated on the server in the same field
	Given a client has a change record R0 in field A
	And field A of the record R0 has been changed on the server
	When the client app submits the record for synchronization
	Then the fieldA conflict should be saved in the server record R0

Scenario: Update a client record while a sync is in progress
	Given a client sync is in progress
	And a new change record R3 occurs on the client
	When changes are pulled to sync
	Then the new change record R3 will not be pulled

Scenario: Update a server record while a sync is in progress
	Given a server sync is in progress
	And a new changes is made to record R3 on the client
	When changes are considered for conflicts
	Then the new change in R3 will not be considered

Scenario: Insert a client record while a sync is in progress

Scenario: Insert a server record while a sync is in progress

Scenario: Delete a client record while a sync is in progress

Scenario: Delete a server record while a sync is in progress

Scenario: Server sends client changes that contain conflicts

Scenario: Server needs to save items resolutions sent back from client

Scenario: Server changes have conflicts on client side