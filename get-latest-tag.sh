#!/usr/bin/env bash
set -euo pipefail

if [ $# -lt 1 ]; then
  echo "Usage: $0 <namespace/repo>"
  exit 2
fi

REPO="$1"
DOCKER_USERNAME="${DOCKER_USERNAME:-}"
DOCKERHUB_TOKEN="${DOCKERHUB_TOKEN:-}"

API_URL="https://hub.docker.com/v2/repositories/${REPO}/tags/?page_size=100"

# Fetch tags (authenticated if credentials provided)
if [ -n "$DOCKER_USERNAME" ] && [ -n "$DOCKERHUB_TOKEN" ]; then
  API_JSON=$(curl -s -u "${DOCKER_USERNAME}:${DOCKERHUB_TOKEN}" "$API_URL")
else
  API_JSON=$(curl -s "$API_URL")
fi

# 1) Prefer timestamp-like tags (yyyyMMdd_HHMMSS), excluding "latest"
TAG=$(printf '%s' "$API_JSON" \
  | jq -r '.results
      | map(select(.name != "latest"))
      | map(select(.name | test("[0-9]{8}_[0-9]{6}")))
      | sort_by(.last_updated)
      | .[-1].name // empty')

# 2) Fallback: newest tag excluding "latest"
if [ -z "$TAG" ]; then
  TAG=$(printf '%s' "$API_JSON" \
    | jq -r '.results
        | map(select(.name != "latest"))
        | sort_by(.last_updated)
        | .[-1].name // empty')
fi

if [ -z "$TAG" ]; then
  >&2 echo "{\"error\":\"no-tag-found\",\"repo\":\"$REPO\"}"
  exit 3
fi

# Resolve digest (request auth token then manifest HEAD to read Docker-Content-Digest)
TOKEN=""
# request token (works for public and with credentials)
if [ -n "$DOCKER_USERNAME" ] && [ -n "$DOCKERHUB_TOKEN" ]; then
  TOKEN=$(curl -s -u "${DOCKER_USERNAME}:${DOCKERHUB_TOKEN}" "https://auth.docker.io/token?service=registry.docker.io&scope=repository:${REPO}:pull" | jq -r '.token // empty')
else
  TOKEN=$(curl -s "https://auth.docker.io/token?service=registry.docker.io&scope=repository:${REPO}:pull" | jq -r '.token // empty')
fi

DIGEST=""
if [ -n "$TOKEN" ]; then
  DIGEST=$(curl -sI \
    -H "Authorization: Bearer $TOKEN" \
    -H "Accept: application/vnd.docker.distribution.manifest.v2+json" \
    "https://registry-1.docker.io/v2/${REPO}/manifests/${TAG}" \
    | awk -F': ' '/Docker-Content-Digest/ {print $2}' \
    | tr -d '\r' || true)
fi

# Emit JSON to stdout
jq -n --arg repository "$REPO" --arg tag "$TAG" --arg digest "${DIGEST:-}" \
  '{repository:$repository, tag:$tag, digest:$digest, picked_at:(now|strftime("%Y-%m-%dT%H:%M:%SZ"))}'