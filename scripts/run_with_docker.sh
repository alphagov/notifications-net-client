DOCKER_IMAGE_NAME=notifications-net-client

source environment.sh

docker run \
  --rm \
  -v "`pwd`:/var/project" \
  -e API_CLIENT_INTEGRATION_TESTS_NOTIFY_API_URL=${API_CLIENT_INTEGRATION_TESTS_NOTIFY_API_URL} \
  -e API_CLIENT_INTEGRATION_TESTS_TEST_API_KEY=${API_CLIENT_INTEGRATION_TESTS_TEST_API_KEY} \
  -e API_CLIENT_INTEGRATION_TESTS_NUMBER=${API_CLIENT_INTEGRATION_TESTS_NUMBER} \
  -e API_CLIENT_INTEGRATION_TESTS_EMAIL=${API_CLIENT_INTEGRATION_TESTS_EMAIL} \
  -e API_CLIENT_INTEGRATION_TESTS_EMAIL_TEMPLATE_ID=${API_CLIENT_INTEGRATION_TESTS_EMAIL_TEMPLATE_ID} \
  -e API_CLIENT_INTEGRATION_TESTS_SMS_TEMPLATE_ID=${API_CLIENT_INTEGRATION_TESTS_SMS_TEMPLATE_ID} \
  -e API_CLIENT_INTEGRATION_TESTS_LETTER_TEMPLATE_ID=${API_CLIENT_INTEGRATION_TESTS_LETTER_TEMPLATE_ID} \
  -e API_CLIENT_INTEGRATION_TESTS_EMAIL_REPLY_TO_ID=${API_CLIENT_INTEGRATION_TESTS_EMAIL_REPLY_TO_ID} \
  -e API_CLIENT_INTEGRATION_TESTS_SMS_SENDER_ID=${API_CLIENT_INTEGRATION_TESTS_SMS_SENDER_ID} \
  -e API_CLIENT_INTEGRATION_TESTS_TEAM_API_KEY=${API_CLIENT_INTEGRATION_TESTS_TEAM_API_KEY} \
  -e API_CLIENT_INTEGRATION_TESTS_INBOUND_SMS_API_KEY=${API_CLIENT_INTEGRATION_TESTS_INBOUND_SMS_API_KEY} \
  -it \
  ${DOCKER_IMAGE_NAME} \
  ${@}
