# Contributing

Pull requests are welcome.

## Setting Up

### Docker container

This app uses dependencies that are difficult to install locally. In order to make local development easy, we run app commands through a Docker container. Run the following to set this up:

```shell
make bootstrap-with-docker
```

### `environment.sh`

In the root directory of the repo, run:

```
notify-pass credentials/staging/ssm/api_client_integration_tests_environment > environment.sh
```

Unless you're part of the GOV.UK Notify team, you won't be able to run this command or the Integration Tests. However, the file still needs to exist - run `touch environment.sh` instead.

## Tests

To run the tests, you'll first need to build the binaries:

```
make build-with-docker
```

### Unit Tests

To run the unit tests:

```
make test-with-docker
```

### Integration Tests

To run the integration tests:

```
make integration-test-with-docker
```
