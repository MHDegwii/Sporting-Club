# ? Sporting Club Backend - Implementation Checklist

## ?? PROJECT COMPLETION TRACKER

### Legend
- ? = Completed
- ? = In Progress
- ?? = To Do
- ?? = Blocked/At Risk

---

## PHASE 1: Foundation & Authentication (Sprints 1-2) - CURRENT PHASE

### Setup & Architecture ?
- [x] Clean Architecture project structure
- [x] Domain, Application, Infrastructure, API layers
- [x] Dependency injection configured
- [x] .NET 7 SDK projects created
- [x] SQLite local database setup
- [x] EF Core DbContext configured
- [x] Initial migrations prepared

### Authentication Endpoints ?
- [ ] User Registration (`POST /api/v1/auth/register`)
  - [ ] Model: RegisterRequest (email, password, fullName)
  - [ ] Validation: Email format, password strength
  - [ ] Email verification token generation
  - [ ] Send verification email (Mailgun/SendGrid)
  - [ ] Test with Swagger

- [ ] User Login (`POST /api/v1/auth/login`)
  - [ ] Model: LoginRequest (email, password)
  - [ ] Password verification (BCrypt)
  - [ ] JWT token generation (exp: 60 min)
  - [ ] Refresh token generation (exp: 7 days)
  - [ ] Test with Swagger

- [ ] Refresh Token (`POST /api/v1/auth/refresh`)
  - [ ] Model: RefreshRequest (refreshToken)
  - [ ] Token validation & rotation
  - [ ] Return new JWT + refresh token
  - [ ] Test with Swagger

- [ ] Logout (`POST /api/v1/auth/logout`)
  - [ ] Invalidate refresh token
  - [ ] Clear any server-side session
  - [ ] Test with Swagger

- [ ] Forgot Password (`POST /api/v1/auth/forgot-password`)
  - [ ] Generate OTP (6 digits)
  - [ ] Send email with OTP
  - [ ] Store OTP with expiration (15 min)
  - [ ] Test with Swagger

- [ ] Reset Password (`POST /api/v1/auth/reset-password`)
  - [ ] Model: ResetPasswordRequest (email, otp, newPassword)
  - [ ] Validate OTP
  - [ ] Update password
  - [ ] Invalidate all refresh tokens (force re-login)
  - [ ] Test with Swagger

### Role-Based Access Control ?
- [ ] Enum: UserRole (Admin, Manager, Staff)
- [ ] Claims-based authorization
- [ ] Authorization policies created:
  - [ ] [Authorize(Policy = "AdminOnly")]
  - [ ] [Authorize(Policy = "ManagerOrAdmin")]
  - [ ] [Authorize(Policy = "StaffOrManagerOrAdmin")]
- [ ] Test with different roles

### Members CRUD ?
- [ ] GET `/api/v1/members` (paginated, filterable)
  - [ ] Query parameters: page, pageSize, search, status
  - [ ] Pagination response with total count
  - [ ] Filter by active/inactive
  - [ ] Search by email/name/phone

- [ ] POST `/api/v1/members` (Admin/Manager only)
  - [ ] Model: CreateMemberRequest
  - [ ] Generate unique member ID
  - [ ] Assign membership plan
  - [ ] Test validation

- [ ] GET `/api/v1/members/{id}`
  - [ ] Return member details
  - [ ] Include subscription info
  - [ ] Include membership card number

- [ ] PUT `/api/v1/members/{id}` (Admin/Manager or self)
  - [ ] Update member info
  - [ ] Update status
  - [ ] Update plan (if allowed)

- [ ] DELETE `/api/v1/members/{id}` (Admin only)
  - [ ] Soft delete or hard delete?
  - [ ] Decide on business logic

- [ ] Member filtering endpoints:
  - [ ] GET `/api/v1/members/renewal-soon` - members expiring soon
  - [ ] GET `/api/v1/members/by-subscription/{subId}` - by subscription type

### Employees CRUD ?
- [ ] GET `/api/v1/employees` (paginated)
- [ ] POST `/api/v1/employees` (Admin only)
- [ ] GET `/api/v1/employees/{id}`
- [ ] PUT `/api/v1/employees/{id}` (Admin)
- [ ] DELETE `/api/v1/employees/{id}` (Admin - deactivate)
- [ ] Department assignment logic
- [ ] Role assignment logic

### Departments CRUD ?
- [ ] GET `/api/v1/departments`
- [ ] POST `/api/v1/departments` (Admin)
- [ ] GET `/api/v1/departments/{id}`
- [ ] PUT `/api/v1/departments/{id}` (Admin)
- [ ] DELETE `/api/v1/departments/{id}` (Admin)

### Database & Migrations ?
- [ ] AppUser entity with all required fields
- [ ] RefreshToken entity
- [ ] Member entity
- [ ] Employee entity
- [ ] Department entity
- [ ] Initial migration created
- [ ] Test migration with SQLite
- [ ] Test migration with PostgreSQL

### Testing & Documentation ?
- [ ] Swagger updated with all endpoints
- [ ] All endpoints tested via Swagger UI
- [ ] Authentication tests pass
- [ ] Authorization tests pass
- [ ] Unit tests for AuthService
- [ ] Integration tests for API endpoints
- [ ] API documentation includes:
  - [ ] Request/response models
  - [ ] Error responses
  - [ ] Example values
  - [ ] Required headers (Authorization: Bearer {token})

---

## PHASE 2: Core Club Features (Sprints 3-4)

### Sports CRUD ??
- [ ] GET `/api/v1/sports` (paginated, filterable)
- [ ] POST `/api/v1/sports` (Manager/Admin)
- [ ] GET `/api/v1/sports/{id}`
- [ ] PUT `/api/v1/sports/{id}`
- [ ] DELETE `/api/v1/sports/{id}`
- [ ] Include coach assignment
- [ ] Include facilities/schedules

### Services CRUD ??
- [ ] GET `/api/v1/services` (with pagination)
- [ ] POST `/api/v1/services` (Manager/Admin)
- [ ] GET `/api/v1/services/{id}`
- [ ] PUT `/api/v1/services/{id}`
- [ ] DELETE `/api/v1/services/{id}`
- [ ] Status field (available/unavailable)

### Subscriptions CRUD ??
- [ ] GET `/api/v1/subscriptions` (public & paginated)
- [ ] POST `/api/v1/subscriptions` (Admin)
- [ ] GET `/api/v1/subscriptions/{id}`
- [ ] PUT `/api/v1/subscriptions/{id}` (Admin)
- [ ] DELETE `/api/v1/subscriptions/{id}` (Admin)
- [ ] Linked sports/services array
- [ ] Pricing tiers
- [ ] Duration configuration

### Tickets CRUD ??
- [ ] GET `/api/v1/tickets` (public, with filters)
- [ ] POST `/api/v1/tickets` (Admin/Manager)
- [ ] GET `/api/v1/tickets/{id}`
- [ ] PUT `/api/v1/tickets/{id}` (Admin/Manager)
- [ ] DELETE `/api/v1/tickets/{id}` (Admin)
- [ ] Quantity tracking
- [ ] Availability windows
- [ ] Booking tracking

### Reservations CRUD ??
- [ ] GET `/api/v1/reservations` (user's own only)
- [ ] POST `/api/v1/reservations` (create booking)
  - [ ] Conflict detection (no double bookings)
  - [ ] Time slot validation
  - [ ] Member availability check

- [ ] GET `/api/v1/reservations/{id}` (user's own or admin)
- [ ] PUT `/api/v1/reservations/{id}` (modify, user's own or admin)
- [ ] DELETE `/api/v1/reservations/{id}` (cancel, user's own or admin)
- [ ] Admin filter: GET `/api/v1/reservations?sportId=X&date=Y`

### Ticket Booking ??
- [ ] POST `/api/v1/tickets/{id}/book` (create booking)
- [ ] GET `/api/v1/bookings` (user's tickets)
- [ ] DELETE `/api/v1/bookings/{id}` (cancel ticket)

### Stores & Offers CRUD ??
- [ ] GET `/api/v1/stores` (paginated, public)
- [ ] POST `/api/v1/stores` (Admin/Manager)
- [ ] GET `/api/v1/stores/{id}`
- [ ] PUT `/api/v1/stores/{id}`
- [ ] DELETE `/api/v1/stores/{id}`

- [ ] GET `/api/v1/offers` (paginated, active only)
- [ ] POST `/api/v1/offers` (Admin/Manager)
- [ ] GET `/api/v1/offers/{id}`
- [ ] PUT `/api/v1/offers/{id}`
- [ ] DELETE `/api/v1/offers/{id}`

### Announcements CRUD ??
- [ ] GET `/api/v1/announcements` (public)
- [ ] POST `/api/v1/announcements` (Admin/Manager)
- [ ] GET `/api/v1/announcements/{id}`
- [ ] PUT `/api/v1/announcements/{id}` (Admin/Manager)
- [ ] DELETE `/api/v1/announcements/{id}` (Admin)
- [ ] Target audience field (All / Members / Staff)

### Push Notifications Service ??
- [ ] Firebase Cloud Messaging (FCM) integration
- [ ] Store FCM tokens per user
- [ ] Broadcast endpoint: `POST /api/v1/notifications/broadcast`
- [ ] Targeted notifications: `POST /api/v1/notifications/send`
- [ ] Renewal reminder trigger
- [ ] Topic-based subscriptions
- [ ] Test with Swagger

### File Upload Service ??
- [ ] POST `/api/v1/files/upload` (multipart form)
- [ ] Integration with Azure Blob Storage or MinIO
- [ ] Profile image uploads
- [ ] Club asset uploads
- [ ] Store product photos
- [ ] Return file URL
- [ ] Delete endpoint: DELETE `/api/v1/files/{id}`

---

## PHASE 3: Analytics & Advanced Features (Sprints 5-6)

### Analytics & KPI Endpoints ??
- [ ] GET `/api/v1/analytics/dashboard` (home KPIs)
  - [ ] Active members count
  - [ ] Total subscriptions
  - [ ] Revenue (this month, this year)
  - [ ] Upcoming renewals count

- [ ] GET `/api/v1/analytics/members-growth` (chart data)
  - [ ] Member count by month
  - [ ] New members this month

- [ ] GET `/api/v1/analytics/revenue` (financials)
  - [ ] By subscription type
  - [ ] By date range

- [ ] GET `/api/v1/analytics/occupancy` (reservations)
  - [ ] Sports facility usage rate
  - [ ] Peak times

### Branches CRUD ??
- [ ] GET `/api/v1/branches` (paginated, public)
- [ ] POST `/api/v1/branches` (Admin)
- [ ] GET `/api/v1/branches/{id}`
- [ ] PUT `/api/v1/branches/{id}` (Admin)
- [ ] DELETE `/api/v1/branches/{id}` (Admin)
- [ ] Assign sports/services/stores to branches

### FAQs CRUD ??
- [ ] GET `/api/v1/faqs` (public)
- [ ] POST `/api/v1/faqs` (Admin)
- [ ] PUT `/api/v1/faqs/{id}` (Admin)
- [ ] DELETE `/api/v1/faqs/{id}` (Admin)

### Email Service Integration ??
- [ ] Mailgun or SendGrid integration
- [ ] Email templates for:
  - [ ] Account verification
  - [ ] Password reset
  - [ ] Renewal reminder
  - [ ] Booking confirmation
  - [ ] Offer notification

- [ ] Test email sending

### Background Jobs (Hangfire) ??
- [ ] Setup Hangfire dashboard
- [ ] Renewal reminder job:
  - [ ] Run daily
  - [ ] Find members expiring in 7 days
  - [ ] Send FCM + email

- [ ] Cleanup job:
  - [ ] Archive old reservations
  - [ ] Delete expired refresh tokens

- [ ] Health check job
- [ ] Test job execution

### Health Check Endpoints ??
- [ ] GET `/api/v1/health` (basic)
- [ ] GET `/api/v1/health/detailed` (comprehensive)
  - [ ] Database connectivity
  - [ ] FCM availability
  - [ ] File storage connectivity
  - [ ] Email service connectivity

---

## PHASE 4: Polish & Optimization (Sprints 7-8)

### Rate Limiting ??
- [ ] Configure AspNetCoreRateLimit
- [ ] Per-user limits
- [ ] Per-IP limits
- [ ] Endpoint-specific limits
- [ ] Test rate limiting

### Audit Logging ??
- [ ] Log all sensitive operations:
  - [ ] User creation/deletion
  - [ ] Role changes
  - [ ] Member status changes
  - [ ] Subscription updates

- [ ] Audit log table
- [ ] Query audit logs: GET `/api/v1/audit-logs`
- [ ] Filter by user/action/date

### Performance Optimization ??
- [ ] Database query optimization
- [ ] Add indexes for frequently queried fields
- [ ] Implement caching:
  - [ ] In-memory cache for subscriptions
  - [ ] Redis for distributed cache

- [ ] Pagination optimization
- [ ] Profiling: N+1 query detection
- [ ] Load test APIs

### Swagger Documentation ??
- [ ] All endpoints documented
- [ ] Request/response examples
- [ ] Error responses documented
- [ ] Authentication requirements clear
- [ ] API versioning clear

### Input Validation ??
- [ ] FluentValidation rules for all DTOs
- [ ] Global validation error handling
- [ ] Custom error messages
- [ ] Test validation

### Error Handling ??
- [ ] Global exception middleware
- [ ] Consistent error response format
- [ ] HTTP status codes appropriate
- [ ] Detailed logging of errors
- [ ] Test error scenarios

---

## PHASE 5: Production & Launch (Sprints 9-10)

### CI/CD Pipeline ??
- [ ] GitHub Actions workflow
- [ ] Automated build on push
- [ ] Automated tests on build
- [ ] Automated deployment to staging
- [ ] Automated deployment to production (manual approval)
- [ ] Rollback procedures

### Environment Configuration ??
- [ ] appsettings.Development.json
- [ ] appsettings.Staging.json
- [ ] appsettings.Production.json
- [ ] Secrets management (Azure Key Vault)
- [ ] Environment variables for CI/CD

### Database Management ??
- [ ] EF Core migration automation in CI/CD
- [ ] Data seeding for production
- [ ] Backup procedures
- [ ] Restore procedures tested
- [ ] Migration rollback tested

### Monitoring & Logging ??
- [ ] Serilog setup
- [ ] Application Insights
- [ ] Error tracking (Sentry)
- [ ] Performance monitoring
- [ ] Log aggregation
- [ ] Alerts configured

### Security Hardening ??
- [ ] HTTPS enforced
- [ ] CORS configured properly
- [ ] SQL injection prevention verified
- [ ] XSS prevention verified
- [ ] CSRF protection if needed
- [ ] Security headers added
- [ ] Dependency vulnerabilities checked

### Documentation for DevOps ??
- [ ] Deployment guide
- [ ] Configuration guide
- [ ] Troubleshooting guide
- [ ] Database backup/restore guide
- [ ] Monitoring setup guide

### Deployment to Production ??
- [ ] Choose hosting: Fly.io / Render / Railway
- [ ] Configure DNS
- [ ] Configure SSL/TLS
- [ ] Configure database (PostgreSQL)
- [ ] Configure backups
- [ ] Configure monitoring
- [ ] Test production endpoints
- [ ] UAT with stakeholders
- [ ] Go-live ??

---

## ?? Overall Completion Status

| Phase | Sprints | Completion | Status |
|-------|---------|-----------|--------|
| Foundation & Auth | 1-2 | 10% | ? Current |
| Core Features | 3-4 | 0% | ?? Next |
| Advanced | 5-6 | 0% | ?? Planned |
| Polish | 7-8 | 0% | ?? Blocked |
| Launch | 9-10 | 0% | ?? Blocked |
| **TOTAL** | **1-10** | **10%** | **? In Progress** |

---

## ?? Next Actions

### THIS SPRINT (Sprint 1):
1. ? Setup complete (DONE)
2. ? Start Authentication endpoints
3. ? Start Members CRUD
4. ? Update Swagger docs

### NEXT SPRINT (Sprint 2):
1. ? Complete all Auth endpoints
2. ? Complete Members CRUD
3. ? Complete Employees CRUD
4. ? Hand off to frontend/mobile teams

---

**Total Endpoint Count Target: ~50+ endpoints**
**Estimated Lines of Code: ~10,000-15,000**
**Current Progress: ~5%**

Good luck! ??
