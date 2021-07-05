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
notify-pass credentials/client-integration-tests > environment.sh
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

## Working on the client locally

You will need to ensure that you have .Net 4.6.2 Framework and .Net core 2.0 installed on your machine.

If you are not working on a windows OS, .Net Frameworks are not supported but you can use the Makefile to build and run tests, run `make` on your terminal to see the available options.

## Deploying the client to Bintray

If you are a member of the Notify team go to the latest build on https://jenkins.notify.tools/job/run-app-veyor-build/, re-run this build with the `PUBLISH_TO_BINTRAY` checked.
