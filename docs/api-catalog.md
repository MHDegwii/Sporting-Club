# Sporting Club API Catalog

Base URL: `http://localhost:5000`

## Auth
- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/forgot-password`
- `POST /api/v1/auth/reset-password`
- `POST /api/v1/auth/verify-email`

## Core/P1 + P2 Resources
- `GET|POST /api/v1/{resource}`
- `GET|PUT|DELETE /api/v1/{resource}/{id}`

Supported resources:
`employees`, `departments`, `members`, `sports`, `services`, `subscriptions`, `tickets`, `reservations`, `branches`, `stores`, `offers`, `announcements`, `faqs`

## Files / Notifications / Analytics / Operations
- `POST /api/v1/files/upload`
- `POST /api/v1/notifications/broadcast`
- `POST /api/v1/notifications/targeted`
- `GET /api/v1/analytics/dashboard-summary`
- `GET /api/v1/operations/ready`
- `GET /api/v1/operations/live`
- `GET /api/v1/health`
