# Sporting Club System - Project Analysis & Remaining Phases

## ?? Current Status

? **COMPLETED:**
- Backend project initialized with .NET 7
- Clean Architecture structure established
- SQLite database configured for local development
- JWT authentication framework in place
- Basic CRUD APIs functional
- Swagger/OpenAPI documentation setup
- Local development environment working

---

## ?? Project Overview

**Total Duration:** 20 Weeks  
**Methodology:** Agile Scrum (2-week sprints)  
**Total Sprints:** 10  
**Tech Stack:**
- Backend: .NET 8 (C#) - Clean Architecture
- Frontend: React 18 + TypeScript
- Mobile: Flutter 3 (iOS/Android)
- Database: PostgreSQL (production) / SQLite (local dev)

**Three User-Facing Systems:**
1. ??? Internal Dashboard (React) - For employees & admins
2. ?? Mobile App (Flutter) - For club members
3. ?? Public Website (React) - For marketing to visitors

---

## ?? BACKEND SCOPE (Your Responsibility)

### ? COMPLETED (Sprint 1 Setup)
- [x] Project structure & Clean Architecture layers
- [x] Database context & EF Core setup
- [x] SQLite local development database
- [x] API versioning infrastructure
- [x] Swagger/OpenAPI documentation
- [x] Basic middleware setup (auth, logging, rate limiting)

### ?? IN PROGRESS / TODO (Priority Order)

#### **PHASE 1: Authentication & Core Users (Sprints 1-2) — HIGH PRIORITY**
- [ ] **User Registration API** (POST `/api/v1/auth/register`)
  - Email validation & verification
  - Password hashing (BCrypt)
  - Email verification flow

- [ ] **User Login API** (POST `/api/v1/auth/login`)
  - Email/password validation
  - JWT token generation
  - Refresh token mechanism

- [ ] **Password Management**
  - Forgot password endpoint
  - Reset password with OTP
  - Password update for authenticated users

- [ ] **Role-Based Access Control (RBAC)**
  - Three roles: Admin, Manager, Staff
  - Claim-based authorization
  - Protected endpoints by role

#### **PHASE 2: Core Entity CRUD APIs (Sprints 2-3) — HIGH PRIORITY**

Build full CRUD endpoints for:

1. **Members CRUD** (GET, POST, PUT, DELETE)
   - Pagination & filtering
   - Search by email/name/phone
   - Member status (active/inactive)
   - Membership card data

2. **Employees CRUD**
   - Department assignment
   - Role assignment
   - Photo upload integration
   - Deactivation logic

3. **Departments CRUD**
   - Organizational structure
   - Employee assignment

4. **Sports CRUD**
   - Name, description, schedule
   - Assigned coaches
   - Facility management

5. **Services CRUD**
   - Service name & description
   - Availability status
   - Pricing/fees

6. **Subscriptions CRUD**
   - Plan creation & pricing tiers
   - Duration configuration
   - Linked sports/services
   - Renewal dates

7. **Tickets CRUD**
   - Event ticket creation
   - Quantity management
   - Pricing
   - Availability windows
   - Booking tracking

8. **Reservations CRUD**
   - Time slot management
   - Conflict detection
   - Confirmation workflow
   - Cancellation logic

**Key Requirements for All APIs:**
- ? Pagination (limit, offset, page)
- ? Filtering (status, date range, type)
- ? Sorting (by name, date, price)
- ? Search functionality
- ? Input validation (FluentValidation)
- ? Error handling (global exception middleware)

#### **PHASE 3: Advanced Features (Sprints 3-4) — MEDIUM PRIORITY**

- [ ] **Push Notification Service (FCM)**
  - Firebase Cloud Messaging integration
  - Topic-based broadcasts
  - Targeted notifications by member/role
  - Renewal reminders
  - Announcement delivery

- [ ] **File Upload Service**
  - Profile image uploads
  - Club asset storage
  - Store product photos
  - Integration with Azure Blob Storage or MinIO

- [ ] **Additional CRUD Entities**
  - Stores (internal club stores)
  - Offers & Discounts
  - Announcements/News
  - Branches (multi-branch support)
  - FAQs

- [ ] **Email Service**
  - Account verification emails
  - Password reset emails
  - Notification emails
  - Integration with Mailgun/SendGrid

#### **PHASE 4: Analytics & Reporting (Sprint 5) — MEDIUM PRIORITY**

- [ ] **KPI Endpoints** (for dashboard)
  - Active members count
  - Total subscriptions
  - Revenue metrics
  - Upcoming renewals
  - Member growth chart data

- [ ] **Report Generation**
  - Revenue reports (by subscription type, period)
  - Member demographics
  - Reservation occupancy rates
  - Export to Excel/PDF

#### **PHASE 5: Background Jobs & Infrastructure (Sprints 5-6) — LOWER PRIORITY**

- [ ] **Renewal Reminder System** (Hangfire)
  - Scheduled task to find expiring memberships
  - Auto-trigger FCM notifications
  - Email reminders

- [ ] **Cleanup Tasks**
  - Orphaned data cleanup
  - Old reservation archiving

- [ ] **Health Checks**
  - Database connectivity check
  - External service health
  - API responsiveness monitoring

#### **PHASE 6: Polish & Optimization (Sprints 7-8) — FINAL TOUCHES**

- [ ] **Rate Limiting**
  - Prevent API abuse
  - Per-user/IP rate limits

- [ ] **Audit Logging**
  - Log sensitive operations (user creation, deletions, data changes)
  - Track who did what and when

- [ ] **Performance Optimization**
  - Query optimization
  - Caching strategies
  - Database indexing

- [ ] **API Documentation**
  - Swagger/OpenAPI maintenance
  - SDK documentation
  - Integration guides for frontend/mobile teams

#### **PHASE 7: Production Readiness (Sprints 9-10)**

- [ ] **Environment Configuration**
  - Development, staging, production configs
  - Secrets management
  - Connection string management

- [ ] **CI/CD Pipeline**
  - Automated builds
  - Automated tests
  - Deployment to staging/production
  - Rollback procedures

- [ ] **Database Migration Strategy**
  - EF Core migration automation
  - Data seeding for production
  - Backup procedures

- [ ] **Monitoring & Logging**
  - Application Insights/Serilog setup
  - Error tracking
  - Performance monitoring

---

## ?? Frontend Scope (Mohamed Alaa)

### Dashboard (React)
- Login & authentication screens
- Member management CRUD
- Employee management CRUD
- Sports/Services management
- Subscriptions management
- Tickets & Reservations management
- KPI dashboard with charts
- Notifications broadcast UI
- Reports & analytics export

### Public Website (React)
- Home page with hero & highlights
- About the club
- Services listing
- Sports page with schedule
- Membership info & plan comparison
- Club branches with map
- News & announcements
- Contact form
- App download section
- RTL Arabic/English support

---

## ?? Mobile Scope (Khaled Rashed)

- Project setup with Flutter flavors
- Authentication screens (register, login, OTP, forgot password)
- Membership card with QR code
- Home feed with announcements
- Sports explorer
- Subscription plans browser
- Reservation flow (calendar + time picker)
- Ticket booking
- Offers/stores browsing
- Notifications centre
- Settings (password, language, logout)
- Biometric authentication

---

## ??? Sprint Breakdown (20 Weeks = 10 Sprints)

### **Milestone 1: Foundation & Auth (Weeks 1-4, Sprints 1-2)**
Your tasks:
- ? Clean Architecture setup
- ? Database schema & EF Core
- ? Auth APIs (register, login, JWT, refresh)
- ? Members, Employees, Departments CRUD
- ? Swagger documentation
**Story Points:** ~18 points

### **Milestone 2: Core Features (Weeks 5-8, Sprints 3-4)**
Your tasks:
- ? Sports, Services, Subscriptions APIs
- ? Tickets & Reservations APIs
- ? Stores & Offers CRUD
- ? FCM push notifications
- ? File upload service
**Story Points:** ~22 points

### **Milestone 3: Advanced Features (Weeks 9-12, Sprints 5-6)**
Your tasks:
- ? Analytics/KPI endpoints
- ? Branches CRUD
- ? Announcements API
- ? Background jobs (renewal reminders)
- ? Email service
- ? Health checks
**Story Points:** ~18 points

### **Milestone 4: Optimization (Weeks 13-16, Sprints 7-8)**
Your tasks:
- ? Rate limiting
- ? Audit logging
- ? Performance optimization
- ? API documentation finalization
- ? Integration testing
**Story Points:** ~12 points

### **Milestone 5: Launch (Weeks 17-20, Sprints 9-10)**
Your tasks:
- ? Production deployment
- ? CI/CD setup
- ? Environment config
- ? Monitoring setup
- ? UAT bug fixes
- ?? Go-live
**Story Points:** ~8 points

---

## ?? Your Immediate Next Steps (Sprint 1-2)

### THIS WEEK:
1. **Implement Authentication Endpoints**
   - Create `RegisterAsync()` in AuthService
   - Create `LoginAsync()` in AuthService
   - Add email verification logic
   - Test with Swagger UI

2. **Create Members CRUD**
   - MembersController with GET, POST, PUT, DELETE
   - Pagination & filtering support
   - Input validation

3. **Create Employees CRUD**
   - EmployeesController
   - Department assignment logic

4. **Database Migrations**
   - Create initial schema migrations
   - Test with SQLite locally

### BY END OF SPRINT 2:
- All core CRUD endpoints working
- All endpoints tested via Swagger
- Pagination/filtering implemented
- Ready for frontend team to consume

---

## ?? Technical Decisions Already Made

? **Architecture:** Clean Architecture (Domain ? Application ? Infrastructure ? API)  
? **Pattern:** CQRS with MediatR for command/query separation  
? **ORM:** Entity Framework Core with migrations  
? **Authentication:** JWT with refresh tokens  
? **Validation:** FluentValidation  
? **Logging:** Serilog (recommended)  
? **API Docs:** Swagger/OpenAPI  
? **Database:** PostgreSQL (prod) / SQLite (dev)  
? **Deployment:** Fly.io / Render.com / Railway (free tier)  

---

## ?? Key Integration Points

### Frontend ? Backend
- Dashboard consumes all CRUD APIs
- KPI endpoints for dashboard home
- User authentication via JWT
- File uploads for profile images

### Mobile ? Backend
- Member registration & login
- Membership card data endpoint
- Sports & subscriptions listing
- Reservation booking API
- Ticket booking API
- Push notification tokens
- Announcements endpoint

### Notifications Flow
- Backend: Generate notifications (FCM)
- Mobile: Receive & display push notifications
- Dashboard: Broadcast notifications UI

---

## ?? Database Entities (ER Overview)

```
AppUser (Members & Employees)
??? Id, Email, FullName, PasswordHash, Role, EmailVerified, CreatedAt

RefreshToken
??? Id, Token, UserId, ExpiresAtUtc, IsRevoked

Sport
??? Id, Name, Description, Schedule, CoachId

Subscription
??? Id, Name, Price, DurationDays, LinkedSports[], CreatedAt

Member
??? Id, UserId, SubscriptionId, StartDate, EndDate, Status, MembershipCardNumber

Reservation
??? Id, MemberId, SportId, ReservationDate, TimeSlot, Status, CreatedAt

Ticket
??? Id, EventName, Quantity, Price, AvailableSince, AvailableUntil

Store
??? Id, Name, Description, BranchId, CreatedAt

Offer
??? Id, StoreId, Description, DiscountPercentage, StartDate, EndDate

Announcement
??? Id, Title, Content, TargetAudience (All/Members/Staff), CreatedAt, CreatedBy
```

---

## ?? Success Criteria

### By End of Week 1:
- [ ] All basic CRUD endpoints working
- [ ] Authentication endpoints tested
- [ ] Swagger docs updated

### By End of Sprint 1 (Week 2):
- [ ] Frontend team can integrate with auth endpoints
- [ ] Mobile team has API specs

### By End of Sprint 2 (Week 4):
- [ ] All P1 (high priority) APIs complete
- [ ] Ready for feature development in Sprints 3-4

---

## ?? Blockers / Risks

1. **Database Design** - Ensure schema matches all features
2. **API Versioning** - Plan for v2 from day 1
3. **Security** - JWT token expiration, password hashing, CORS
4. **Testing** - Unit tests for business logic, integration tests for APIs
5. **Deployment** - CI/CD must be set up early to catch issues

---

## ?? Quick Reference: API Endpoints To Build

### Sprint 1-2 (CRITICAL):
```
POST   /api/v1/auth/register
POST   /api/v1/auth/login
POST   /api/v1/auth/refresh
POST   /api/v1/auth/logout
POST   /api/v1/auth/forgot-password
POST   /api/v1/auth/reset-password

GET    /api/v1/members
POST   /api/v1/members
GET    /api/v1/members/{id}
PUT    /api/v1/members/{id}
DELETE /api/v1/members/{id}

GET    /api/v1/employees
POST   /api/v1/employees
GET    /api/v1/employees/{id}
PUT    /api/v1/employees/{id}
DELETE /api/v1/employees/{id}

GET    /api/v1/departments
POST   /api/v1/departments
```

### Sprint 3-4 (CORE FEATURES):
```
GET    /api/v1/sports
POST   /api/v1/sports
PUT    /api/v1/sports/{id}
DELETE /api/v1/sports/{id}

GET    /api/v1/subscriptions
POST   /api/v1/subscriptions
PUT    /api/v1/subscriptions/{id}

GET    /api/v1/reservations
POST   /api/v1/reservations
PUT    /api/v1/reservations/{id}
DELETE /api/v1/reservations/{id}

GET    /api/v1/tickets
POST   /api/v1/tickets
POST   /api/v1/tickets/{id}/book
```

### Sprint 5-6 (ADVANCED):
```
POST   /api/v1/notifications/broadcast
GET    /api/v1/analytics/kpis
GET    /api/v1/analytics/revenue
POST   /api/v1/file-upload
GET    /api/v1/branches
```

---

## ? Recommendation

**Start with Sprint 1-2 priorities immediately:**
1. Complete authentication endpoints
2. Implement Members CRUD
3. Implement Employees CRUD  
4. Test everything thoroughly with Swagger

This will unblock the frontend and mobile teams to start integration. You'll have 2 weeks per sprint, so aim to complete 1 major feature group per week.

Good luck! ??
