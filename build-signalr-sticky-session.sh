#!/usr/bin/env bash
set -euo pipefail

REPO_URL="https://github.com/TomaszKrasienko/signalR-sticky-session"
BRANCH="develop"
API_IMAGE_NAME="${API_IMAGE_NAME:-sticky_session_api}"
API_IMAGE_TAG="${API_IMAGE_TAG:-1.0}"
UI_IMAGE_NAME="${UI_IMAGE_NAME:-sticky_session_ui}"
UI_IMAGE_TAG="${UI_IMAGE_TAG:-1.0}"
CLONE_DIR="${CLONE_DIR:-$(pwd)/signalr-sticky-session}"

if [ -d "${CLONE_DIR}/.git" ]; then
  echo "Repo exists in ${CLONE_DIR}, pulling ${BRANCH}..."
  git -C "${CLONE_DIR}" fetch origin "${BRANCH}"
  git -C "${CLONE_DIR}" checkout "${BRANCH}"
  git -C "${CLONE_DIR}" pull origin "${BRANCH}"
else
  echo "Cloning ${REPO_URL} (branch: ${BRANCH}) into ${CLONE_DIR}..."
  git clone --depth 1 -b "${BRANCH}" "${REPO_URL}" "${CLONE_DIR}"
fi

echo "Building image ${API_IMAGE_NAME}:${API_IMAGE_TAG}..."
docker build -t "${API_IMAGE_NAME}:${API_IMAGE_TAG}" -f "${CLONE_DIR}/StickySessionApi/Dockerfile" "${CLONE_DIR}/StickySessionApi"

echo "Building image ${UI_IMAGE_NAME}:${UI_IMAGE_TAG}..."
docker build -t "${UI_IMAGE_NAME}:${UI_IMAGE_TAG}" -f "${CLONE_DIR}/StickySessionUi/Dockerfile" "${CLONE_DIR}/StickySessionUi"

echo "Importing images into k3s containerd..."
docker save "${API_IMAGE_NAME}:${API_IMAGE_TAG}" | sudo k3s ctr images import -
docker save "${UI_IMAGE_NAME}:${UI_IMAGE_TAG}" | sudo k3s ctr images import -

API_DEPLOY_DIR="${CLONE_DIR}/StickySessionApi/deploy"
UI_DEPLOY_DIR="${CLONE_DIR}/StickySessionUi/deploy"

echo "Applying configmap..."
kubectl apply -f "${API_DEPLOY_DIR}/configmap.yaml"

echo "Deploying redis..."
kubectl apply -f "${API_DEPLOY_DIR}/redis-deployment.yaml"

sleep 10

echo "Deploying api..."
kubectl apply -f "${API_DEPLOY_DIR}/deployment.yaml"

echo "Applying api service..."
kubectl apply -f "${API_DEPLOY_DIR}/service.yaml"

echo "Deploying ui..."
kubectl apply -f "${UI_DEPLOY_DIR}/deployment.yaml"

echo "Applying ui service..."
kubectl apply -f "${UI_DEPLOY_DIR}/service.yaml"

echo "Applying ui ingress..."
kubectl apply -f "${UI_DEPLOY_DIR}/ingress.yaml"

echo "Done."
