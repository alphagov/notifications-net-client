#!/usr/bin/env bash

set -eo pipefail

function exit_with_msg {
    echo $1
    exit $2
}

echo -n "" > docker.env

env_vars=(
    NOTIFY_API_URL
    API_KEY
    FUNCTIONAL_TEST_EMAIL
    FUNCTIONAL_TEST_NUMBER
    EMAIL_TEMPLATE_ID
    SMS_TEMPLATE_ID
    EMAIL_REPLY_TO_ID
    LETTER_TEMPLATE_ID
    API_SENDING_KEY
    INBOUND_SMS_QUERY_KEY
)

for env_var in "${env_vars[@]}"; do
    echo "${env_var}=${!env_var}" >> docker.env
done
