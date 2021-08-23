# ChatApp

This is a support ticket processing application.

## Getting Started
Use these instructions to get the project up and running.

### Prerequisites
You will need the following tools:

* [Visual Studio Code or Visual Studio 2019](https://visualstudio.microsoft.com/vs/)
* [.NET Core SDK 5](https://dotnet.microsoft.com/download/dotnet-core/5.0)
* Azure subscription with a CosmosDB and ServiceBus Queue

### Setup
Follow these steps to get your development environment set up:

  1. Clone the repository
  2. At the root directory, restore required packages by running:
      ```
     dotnet restore
     ```
  3. Next, build the solution by running:
     ```
     dotnet build
     ```
  4. Next, navigate to `\Chat.API` directory, launch the application by running:
     ```
	 dotnet run
	 ```
  5. Next, navigate to `\Chat.Agent` directory, launch the application by running:
     ```
	 dotnet run
	 ```
  6. Launch [https://localhost:5001/swagger](http://localhost:5001/swagger) in your browser to view the API

## Technologies
* ASP.NET Core 5
* Azure Cosmos
* Azure ServiceBus
