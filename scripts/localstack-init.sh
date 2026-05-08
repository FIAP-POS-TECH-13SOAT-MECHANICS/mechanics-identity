#!/bin/bash

QUEUES=(
  "fiap-mechanics-dev-customer-created"
  "fiap-mechanics-dev-user-changed"
)

for QUEUE in "${QUEUES[@]}"; do
  awslocal sqs create-queue --queue-name "$QUEUE"
done
