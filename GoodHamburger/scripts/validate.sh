#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT_DIR"

SKIP_TESTS=false
for arg in "$@"; do
  case "$arg" in
    --skip-tests)
      SKIP_TESTS=true
      ;;
    *)
      echo "Unknown option: $arg"
      echo "Usage: ./validate.sh [--skip-tests]"
      exit 1
      ;;
  esac
done

need_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing required command: $1"
    exit 1
  fi
}

wait_http_ok() {
  local url="$1"
  local label="$2"
  local attempts=40
  local delay=2

  for ((i=1; i<=attempts; i++)); do
    if curl -fsS "$url" >/dev/null 2>&1; then
      echo "[ok] $label is reachable: $url"
      return 0
    fi
    echo "[wait] $label not ready yet ($i/$attempts)"
    sleep "$delay"
  done

  echo "[error] $label did not become ready: $url"
  return 1
}

need_cmd docker
need_cmd curl

echo "[step] Building and starting API + Web containers..."
docker compose up -d --build

echo "[step] Waiting for services..."
wait_http_ok "http://localhost:5000/api/menu" "API"
wait_http_ok "http://localhost:5200" "Web"

echo "[step] Smoke test: create order..."
create_code=$(curl -sS -o /tmp/gh_create_order.json -w "%{http_code}" \
  -X POST "http://localhost:5000/api/orders" \
  -H "Content-Type: application/json" \
  -d '{"sandwich":3,"hasFries":true,"hasDrink":false}')

if [[ "$create_code" != "201" ]]; then
  echo "[error] Expected 201 from POST /api/orders, got $create_code"
  cat /tmp/gh_create_order.json || true
  exit 1
fi

echo "[ok] POST /api/orders returned 201"

echo "[step] Smoke test: list orders..."
list_code=$(curl -sS -o /tmp/gh_list_orders.json -w "%{http_code}" \
  "http://localhost:5000/api/orders")

if [[ "$list_code" != "200" ]]; then
  echo "[error] Expected 200 from GET /api/orders, got $list_code"
  cat /tmp/gh_list_orders.json || true
  exit 1
fi

echo "[ok] GET /api/orders returned 200"

if [[ "$SKIP_TESTS" == "false" ]]; then
  echo "[step] Running .NET tests in SDK container..."
  docker run --rm \
    -v "$ROOT_DIR":/work \
    -w /work \
    mcr.microsoft.com/dotnet/sdk:8.0 \
    dotnet test GoodHamburger.sln --verbosity minimal
  echo "[ok] Tests passed"
else
  echo "[skip] Tests skipped (--skip-tests)"
fi

echo "[done] Validation completed successfully"
