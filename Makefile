.DEFAULT_GOAL := help

DOCKER_BUILDER_IMAGE_NAME = govuknotify/net-client-tests


.PHONY: help
help:
	@cat $(MAKEFILE_LIST) | grep -E '^[a-zA-Z_-]+:.*?## .*$$' | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: build
build: ## Build project
	dotnet build -f=netcoreapp2.0

.PHONY: integration-test
integration-test: test=TestCategory=Integration ## Run integration tests
integration-test: single-test

.PHONY: test
test: test=TestCategory=Unit
test: single-test

.PHONY: single-test
single-test: build ## run a single test. usage: "make single-test test=[test name]"
	dotnet test ./src/GovukNotify.Tests/GovukNotify.Tests.csproj -f=netcoreapp2.0 --no-build -v=n --filter $(test)

.PHONY: build-release
build-release: ## Build release version
	dotnet build -c=Release -f=netcoreapp2.0

.PHONY: generate-env-file
generate-env-file: ## Generate the environment file for running the tests inside a Docker container
	scripts/generate_docker_env.sh

.PHONY: bootstrap-with-docker
bootstrap-with-docker: generate-env-file ## Prepare the Docker builder image
	docker build -t ${DOCKER_BUILDER_IMAGE_NAME} .

.PHONY: build-with-docker
build-with-docker: ## Build with Docker
	docker run -it --rm \
		--name "${USER}-notifications-net-client-manual-test" \
		-v "`pwd`:/var/project" \
		--env-file docker.env \
		${DOCKER_BUILDER_IMAGE_NAME} \
		make build

.PHONY: test-with-docker
test-with-docker: build-with-docker ## Test with Docker
	docker run -it --rm \
		--name "${USER}-notifications-net-client-manual-test" \
		-v "`pwd`:/var/project" \
		--env-file docker.env \
		${DOCKER_BUILDER_IMAGE_NAME} \
		make test

.PHONY: integration-test-with-docker
integration-test-with-docker: build-with-docker ## Integration test with Docker
	docker run -it --rm \
		--name "${USER}-notifications-net-client-manual-test" \
		-v "`pwd`:/var/project" \
		--env-file docker.env \
		${DOCKER_BUILDER_IMAGE_NAME} \
		make integration-test

.PHONY: bash-with-docker
bash-with-docker: generate-env-file ## bash with docker
	docker build -t ${DOCKER_BUILDER_IMAGE_NAME} .
	docker run -it --rm \
		--name "${USER}-notifications-net-client-manual-test" \
		-v "`pwd`:/var/project" \
		--env-file docker.env \
		${DOCKER_BUILDER_IMAGE_NAME} \
		bash

.PHONY: build-package
build-package: build-release ## Build and package NuGet
	dotnet pack -c=Release ./src/GovukNotify/GovukNotify.csproj /p:TargetFrameworks=netcoreapp2.0 -o=publish
