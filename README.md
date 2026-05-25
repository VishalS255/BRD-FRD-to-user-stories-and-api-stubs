# AI-Powered BRD to User Story & API Generator

A RAG-based C# ASP.NET Web API that converts BRD/FRD requirement documents into development-ready artifacts such as:

- User Stories
- Acceptance Criteria
- API Stubs
- OpenAPI YAML
- Exportable JSON/CSV outputs

The system uses Retrieval-Augmented Generation (RAG), embeddings, and structured generation with traceability support.

---

# Features

## Document Upload
Supports:
- `.txt`
- `.docx`

## RAG Pipeline
- Document chunking
- Vector embeddings
- Similarity retrieval
- TopK search

## Requirement Extraction
Extracts:
- Actors
- Actions
- Business Rules
- Constraints
- Unknowns

## User Story Generation
Generates:
- Story Title
- As a / I want / So that
- Acceptance Criteria (Given/When/Then)

## API Stub Generation
Generates:
- REST API methods
- API paths
- Request/Response payloads
- Derived assumption flags

## Traceability
Outputs include:
- Chunk IDs
- Source file name
- Evidence snippets

## Metrics & Validation
- Metrics endpoint
- Reviewer checklist

## Export Support
Exports:
- JSON
- CSV
- OpenAPI YAML

---

# Architecture Flow

Upload Document  
→ Extract Text  
→ Chunking  
→ Embeddings  
→ Retrieval  
→ Requirement Extraction  
→ Story Generation  
→ API Stub Generation  
→ Export

---

# Technology Stack

- C#
- ASP.NET Core Web API
- OpenAI API
- Embeddings
- RAG (Retrieval-Augmented Generation)
- Swagger
- JSON / CSV / YAML Export

---

# API Endpoints

## Document
- Upload document
- View chunks

## Retrieval
- Similarity search

## Requirement Extraction
- Extract actors/actions/rules

## Story Generation
- Generate user stories

## API Stub Generation
- Generate API stubs

## Metrics
- View system metrics

## Reviewer Checklist
- Validate generated outputs

## Export
- Export JSON/CSV/OpenAPI YAML

---

# How to Run

## 1. Clone repository

```bash
git clone <repo-url>