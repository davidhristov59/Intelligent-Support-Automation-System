# Intelligent Support Automation System

An **AI-powered support automation platform** built with **ASP.NET Core**, **Azure Cognitive Search**, and **Azure OpenAI**. The system uses **Retrieval-Augmented Generation (RAG)** to provide intelligent, context-aware answers to user queries based on ingested PDF documents and other knowledge sources.

## 🚀 Features

- **📄 Document Ingestion** – Upload PDFs into Azure Blob Storage and automatically index them with Azure Cognitive Search
- **🔍 Semantic Search** – Query documents using Cognitive Search with chunked indexing and filtering
- **🧠 RAG with Azure OpenAI** – Retrieve relevant passages and pass them into OpenAI for natural language answers
- **💾 Session Persistence** – Store Q&A interactions and sessions in Azure Cosmos DB
- **⚡ Scalable & Cloud-Native** – Containerized with Docker and deployable to Azure

## 📋 Prerequisites

Before setting up the project, ensure you have the following:

- **.NET 8.0 SDK** 
- **Azure subscription** with access to:
  - Azure OpenAI Service
  - Azure Cognitive Search
  - Azure Cosmos DB
  - Azure Blob Storage
- **Docker** (optional, for containerized deployment)

## 🛠️ Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/davidhristov59/Intelligent-Support-Automation-System.git
cd Intelligent-Support-Automation-System
```

### 2. Configure Azure Services

Create and configure the following Azure services in your subscription:

#### Azure Cognitive Search
1. Create a new Azure Cognitive Search service
2. Note down the endpoint URL and admin API key

#### Azure OpenAI
1. Create an Azure OpenAI resource
2. Deploy a model (e.g., GPT-3.5-turbo or GPT-4)
3. Note down the endpoint, API key, and deployment name

#### Azure Cosmos DB
1. Create a new Cosmos DB account
2. Create a database for storing sessions and interactions
3. Note down the endpoint URL and primary key

#### Azure Blob Storage
1. Create a storage account
2. Create a container for document storage
3. Note down the connection string

### 3. Update Configuration

Update the `appsettings.json` file in `Intelligent-Support-Automation-System.Web` with your Azure service credentials:

```json
{
  "AzureSearch": {
    "Endpoint": "https://<your-search-service>.search.windows.net",
    "IndexName": "<your-index-name>",
    "ApiKey": "<your-search-api-key>"
  },
  "AzureOpenAI": {
    "Endpoint": "https://<your-openai-resource>.openai.azure.com/",
    "ApiKey": "<your-openai-api-key>",
    "DeploymentName": "<your-model-deployment-name>"
  },
  "CosmosDB": {
    "Endpoint": "https://<your-cosmosdb-account>.documents.azure.com:443/",
    "Key": "<your-cosmosdb-primary-key>",
    "Database": "<your-database-name>"
  },
  "BlobStorage": {
    "ConnectionString": "<your-storage-connection-string>",
    "ContainerName": "<your-container-name>"
  }
}
```

### 4. Build and Run

Navigate to the project directory and run the following commands:

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run --project Intelligent-Support-Automation-System.Web
```

The backend will be available at 'https://localhost:7187/swagger/index.html'

## 🏗️ Architecture Overview

The system follows a microservices architecture with the following components:

- **Web Application** – ASP.NET Core web interface for user interactions
- **Document Processing Service** – Handles PDF ingestion and indexing
- **Search Service** – Manages semantic search operations using Azure Cognitive Search
- **AI Service** – Integrates with Azure OpenAI for generating responses
- **Data Layer** – Azure Cosmos DB for session and interaction persistence

## 📚 Usage

1. **Upload Documents**: Use the web interface to upload PDF documents to the system
2. **Ask Questions**: Enter your questions in natural language
3. **Get AI-Powered Answers**: The system will search through your documents and provide contextual answers
4. **View Session History**: Access previous conversations and interactions

## 🔧 Configuration Options

### Environment Variables

You can also configure the application using environment variables:

- `AZURE_SEARCH_ENDPOINT`
- `AZURE_SEARCH_API_KEY`
- `AZURE_OPENAI_ENDPOINT`
- `AZURE_OPENAI_API_KEY`
- `COSMOS_DB_ENDPOINT`
- `COSMOS_DB_KEY`
