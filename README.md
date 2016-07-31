# Abstractor Framework

# Abstractor.Cqrs

Framework that provides the basis for the application of CQRS (Command and Query Responsibility Segregation) pattern of architecture, and some common tactical patterns of DDD (Domain-Driven Design ), namely,  Aggregates, Entities, Value Objects, Domain Events and Application Events.

Provides a generic compensation algorithm that allows that custom Units of Work are implementated in a simplified way through extension points (see Abstractor.Cqrs.AzureStorage module for an extension example for the Azure Cloud Storage service). It implements the AOP (Aspected-Oriented Programming) paradigm, enabling the application composition in a highly decoupled and modularized way, favoring the structuring of a hexagonal architecture.

Provides logging functionality throughout the framework, enabling the tracking of the entire application lifecycle.

# Abstractor.Cqrs.EntityFramework

Implementation of the Generic Repository and Unit of Work patterns for Entity Framework.
		
# Abstractor.Cqrs.AzureStorage

Implements the Generic Repository pattern for the Azure Cloud Storage libraries. Uses the generic compensation algorithm, implemented in Abstractor.Cqrs module, which allows the execution of transactional operations across multiple containers, tables and queues.
		
# Abstractor.Cqrs.UnitOfWork

Coordinates and synchronizes the contexts defined in Abstractor.Cqrs.AzureStorage and Abstractor.Cqrs.EntityFramework modules, allowing transactional operations across multiple containers, tables and queues, from the Azure Cloud Storage, and multiple database tables via the Entity Framework.
	
# Abstractor.Cqrs.SimpleInjector

Implements the Adapter pattern for the Simple Injector inversion of control container. It should be used as a basis for future implementation of other containers adapters.
