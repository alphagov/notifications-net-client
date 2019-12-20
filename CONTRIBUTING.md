# Contributing

Pull requests are welcome.

## Working on the client locally

You will need to ensure that you have .Net 4.6.2 Framework and .Net core 2.0 installed on your machine.
If you are not working on a windows OS, .Net Frameworks are not supported but you can use the Makefile to build and run tests, run `make` on your terminal to see the available options.

## Tests

To run the tests, you'll first need to build the binaries:

```
make build-with-docker
```

Then you can run the tests themselves by calling:

```
make test-with-docker
```

To run the integration tests, you will need to ensure that the environment variables are set up for the test project.

```sh
export NOTIFY_API_URL="https://example.notify-api.url"
export API_KEY="example_API_test_key"
export FUNCTIONAL_TEST_NUMBER="valid mobile number"
export FUNCTIONAL_TEST_EMAIL="valid email address"
export EMAIL_TEMPLATE_ID="valid email_template_id"
export SMS_TEMPLATE_ID="valid sms_template_id"
export LETTER_TEMPLATE_ID="valid letter_template_id"
export SMS_SENDER_ID="valid sms_sender_id - to test sending to a receiving number, so needs to be a real number"
export API_SENDING_KEY="API_whitelist_key for sending an SMS to a receiving number"
export INBOUND_SMS_QUERY_KEY="API_test_key to get received text messages"
```

Then run the integration tests by running:

```
make integration-test-with-docker
```

## Deploying the client to Bintray
If you are a member of the Notify team go to the latest build on https://jenkins.notify.tools/job/run-app-veyor-build/, re-run this build with the `PUBLISH_TO_BINTRAY` checked.
