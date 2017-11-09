.DEFAULT_GOAL := help
SHELL := /bin/bash
DATE = $(shell date +%Y-%m-%d:%H:%M:%S)

APP_VERSION_FILE = src/Notify/Properties/AssemblyInfo.cs

.PHONY: help
help:
	@cat $(MAKEFILE_LIST) | grep -E '^[a-zA-Z_-]+:.*?## .*$$' | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: dependencies
dependencies: ## Install build dependencies
	nuget restore
	msbuild

.PHONY: build
build: dependencies ## Build project

.PHONY: test
test: ## Run unit tests
	nunit-console NotifyTests/bin/Debug/NotifyTests.dll -include=Unit/NotificationClient -labels

.PHONY: build-test
build-test: dependencies ## build and test
	make test
