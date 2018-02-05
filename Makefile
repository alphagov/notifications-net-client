.DEFAULT_GOAL := help

.PHONY: help
help:
	@cat $(MAKEFILE_LIST) | grep -E '^[a-zA-Z_-]+:.*?## .*$$' | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: build
build: ## Build project
	dotnet build -f=netcoreapp2.0

.PHONY: test
test: ## Run unit, authentication, integration tests
	make unit-test
	make authentication-test
	make integration-test

.PHONY: authentication-test
authentication-test: ## Run authentication tests
	dotnet test ./src/Notify.Tests/Notify.Tests.csproj --no-build -f=netcoreapp2.0 --filter TestCategory=Unit/AuthenticationTests

.PHONY: integration-test
integration-test: ## Run integration tests
	dotnet test ./src/Notify.Tests/Notify.Tests.csproj --no-build -f=netcoreapp2.0 --filter TestCategory=Integration

.PHONY: unit-test
unit-test: ## Run unit tests
	dotnet test ./src/Notify.Tests/Notify.Tests.csproj --no-build -f=netcoreapp2.0 --filter TestCategory=Unit/NotificationClient

.PHONY: single-test
single-test: ## Run a single test: make single-test test=[test name]
	dotnet test ./src/Notify.Tests/Notify.Tests.csproj --no-build -f=netcoreapp2.0 --filter $(test)

.PHONY: build-single-test
build-single-test: ## Run a single test: make single-test test=[test name]
	dotnet test ./src/Notify.Tests/Notify.Tests.csproj -f=netcoreapp2.0 --filter $(test)

.PHONY: build-test
build-test: ## Build and test
	make build
	make unit-test
	make authentication-test
	make integration-test

.PHONY: build-integration-test
build-integration-test: ## Build and integration test
	make build
	make integration-test

.PHONY: build-unit-test
build-unit-test: ## Build and integration test
	make build
	make unit-test

.PHONY: build-release
build-release: ## Build release version
	dotnet build -c=Release -f=netcoreapp2.0

.PHONY: build-package
build-package: build-release ## Build and package NuGet
	dotnet pack -c=Release ./src/Notify/Notify.csproj /p:TargetFrameworks=netcoreapp2.0 -o=publish
