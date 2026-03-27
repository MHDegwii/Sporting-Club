# ?? PHASE 2 DETAILED ESTIMATION & CURRENT STATUS

## Current Phase 1 Status

### ? COMPLETED in Phase 1
- [x] Clean Architecture setup (4 layers: Domain, Application, Infrastructure, API)
- [x] Database context with EF Core (SQLite/PostgreSQL support)
- [x] API versioning framework
- [x] Swagger/OpenAPI documentation
- [x] JWT authentication framework (Program.cs configured)
- [x] Base domain entities (AppUser, RefreshToken, ResourceItem)
- [x] Generic ResourcesController (CRUD pattern established)
- [x] Role-based access control policies (Admin, Manager, Staff)
- [x] Service interfaces (IAuthService, IResourceService, etc.)
- [x] Middleware setup (exception handling, audit logging, rate limiting)
- [x] Dependency injection configured
- [x] Local development environment working

**Phase 1 Completion:** ~80% complete ? (Foundation solid, auth endpoints need final implementation)

---

## PHASE 2: Core Club Features - Detailed Estimation

### ?? Overview
**Duration:** 4 weeks (Weeks 5-8)  
**Total Estimated Hours:** 65-80 hours  
**Per Sprint:** 32-40 hours/sprint  
**Sprints:** 3-4

---

## SPRINT 3 (Weeks 5-6) - 28-34 hours

### 1. Sports CRUD API (8-10 hours)

#### Database Layer
- [ ] Create `Sport` entity class
  - Fields: Name, Description, Schedule, CoachId, BranchId
  - **Time:** 1 hour

- [ ] EF Core migration
  - Add DbSet<Sport>
  - Configure relationships
  - **Time:** 1 hour

#### Service Layer
- [ ] Create `ISportService` interface
  - Define GetAllAsync, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync
  - **Time:** 0.5 hours

- [ ] Create `EfCoreSportService` implementation
  - All CRUD operations with filtering/sorting
  - **Time:** 3-4 hours
  - Includes: Pagination, sorting, filtering by CoachId/BranchId

#### API Layer
- [ ] Create `SportsController`
  - GET (paginated), POST, PUT, DELETE endpoints
  - Authorization policies
  - **Time:** 2-3 hours

#### Testing & Documentation
- [ ] Test endpoints via Swagger
- [ ] Update Swagger docs
- [ ] **Time:** 1-2 hours

**Subtotal: 8-10 hours**

---

### 2. Services CRUD API (5-6 hours)

**Similar structure to Sports but simpler:**
- [ ] Create `Service` entity
  - Fields: Name, Description, Status, BranchId
  - **Time:** 0.5 hours

- [ ] EF Core migration
  - **Time:** 0.5 hours

- [ ] Service layer (ISportService)
  - **Time:** 2-3 hours

- [ ] Controller
  - **Time:** 1.5-2 hours

- [ ] Testing
  - **Time:** 0.5-1 hour

**Subtotal: 5-6 hours**

---

### 3. Subscriptions CRUD API (7-8 hours)

**More complex - has linked entities:**
- [ ] Create `Subscription` entity
  - Fields: Name, Price, DurationDays, CreatedAt, LinkedSportIds (JSON or relationship)
  - **Time:** 1 hour

- [ ] EF Core migration
  - Consider many-to-many relationship with Sports
  - **Time:** 1-1.5 hours

- [ ] Service layer
  - Handle linked sports/services
  - **Time:** 3-4 hours

- [ ] Controller
  - **Time:** 1.5-2 hours

- [ ] Testing
  - **Time:** 0.5-1 hour

**Subtotal: 7-8 hours**

---

### 4. Tickets API (8-10 hours)

**Includes booking logic:**
- [ ] Create `Ticket` entity
  - Fields: EventName, Quantity, Price, AvailableSince, AvailableUntil, CreatedBy
  - **Time:** 1 hour

- [ ] Create `TicketBooking` entity
  - Fields: TicketId, UserId, Quantity, BookedAt
  - **Time:** 0.5 hours

- [ ] EF Core migration
  - Relationships with Ticket and AppUser
  - **Time:** 1 hour

- [ ] Service layer
  - List tickets (filter by availability)
  - Book tickets (quantity validation)
  - **Time:** 4-5 hours

- [ ] Controller
  - GET, POST (book), DELETE
  - **Time:** 1.5-2 hours

- [ ] Testing
  - **Time:** 0.5-1 hour

**Subtotal: 8-10 hours**

---

**SPRINT 3 TOTAL: 28-34 hours** ?

---

## SPRINT 4 (Weeks 7-8) - 34-44 hours

### 5. Reservations CRUD API (10-12 hours) ? MOST COMPLEX

**Requires conflict detection & time slot logic:**

#### Database Layer
- [ ] Create `Reservation` entity
  - Fields: MemberId, SportId, ReservationDate, TimeSlot, Status, Notes
  - **Time:** 1 hour

- [ ] Create `TimeSlot` value object
  - Handle 09:00-10:00 format parsing
  - **Time:** 1-2 hours

- [ ] EF Core migration
  - **Time:** 1 hour

#### Service Layer
- [ ] Create `IReservationService` interface
  - **Time:** 0.5 hours

- [ ] Create `EfCoreReservationService` implementation
  - **Get all reservations (paginated, filterable)**
    - Time: 2 hours

  - **Get available slots for a sport/date**
    - Conflict detection logic
    - Time: 3-4 hours

  - **Book a reservation**
    - Validate availability, create booking
    - Time: 2-3 hours

  - **Cancel reservation**
    - Mark as cancelled
    - Time: 1 hour

- [ ] Implement conflict detection algorithm
  - Check for overlapping time slots
  - **Time:** 2-3 hours

#### API Layer
- [ ] Create `ReservationsController`
  - GET (own reservations or all if admin)
  - POST (create reservation)
  - PUT (update)
  - DELETE (cancel)
  - **Time:** 2-3 hours

#### Testing & Validation
- [ ] Test conflict detection scenarios
- [ ] Test edge cases (same time, adjacent slots)
- [ ] **Time:** 1-2 hours

**Subtotal: 10-12 hours** ? Longest task in Phase 2

---

### 6. Stores & Offers CRUD API (6-8 hours)

#### Stores CRUD
- [ ] Create `Store` entity
  - Fields: Name, Description, BranchId, CreatedBy
  - **Time:** 0.5 hours

- [ ] Create `Offer` entity
  - Fields: StoreId, Description, DiscountPercentage, StartDate, EndDate
  - **Time:** 0.5 hours

- [ ] EF Core migration
  - **Time:** 1 hour

- [ ] Service layer (both Stores and Offers)
  - **Time:** 2-3 hours

- [ ] Controllers (StoresController, OffersController)
  - **Time:** 2-3 hours

- [ ] Testing
  - **Time:** 0.5-1 hour

**Subtotal: 6-8 hours**

---

### 7. FCM Push Notifications Service (8-10 hours) ? EXTERNAL INTEGRATION

**Firebase Cloud Messaging integration:**

#### Setup
- [ ] Install Firebase Admin SDK NuGet package
  - **Time:** 0.5 hours

- [ ] Create `NotificationRequest` model
  - **Time:** 0.5 hours

- [ ] Create `INotificationService` interface (if not exists)
  - **Time:** 0.5 hours

#### Implementation
- [ ] Implement `FcmNotificationService`
  - Initialize Firebase
  - Send to topic (broadcast all users)
  - Send to user ID (targeted)
  - Handle response/errors
  - **Time:** 4-5 hours

- [ ] Create `NotificationsController`
  - POST /broadcast (send to all)
  - POST /send (send to specific users)
  - **Time:** 2-3 hours

- [ ] Store FCM tokens
  - Add `FcmToken` field to AppUser
  - Store/update tokens on login
  - EF Core migration
  - **Time:** 2-3 hours

#### Testing
- [ ] Mock FCM in tests
- [ ] **Time:** 1 hour

**Subtotal: 8-10 hours** ? External integration, potential unknowns

---

### 8. File Upload Service (6-8 hours) ? EXTERNAL INTEGRATION

**Azure Blob Storage or MinIO integration:**

#### Setup
- [ ] Install Azure Blob Storage SDK / MinIO SDK
  - **Time:** 0.5 hours

- [ ] Create `IFileStorageService` interface (if not exists)
  - **Time:** 0.5 hours

#### Implementation
- [ ] Implement `AzureBlobStorageService` OR `MinIoStorageService`
  - Upload file
  - Delete file
  - Get URL
  - Handle errors/timeouts
  - **Time:** 3-4 hours

- [ ] Create `FilesController`
  - POST /upload (multipart form)
  - DELETE /{id}
  - GET /{id} (redirect to URL)
  - **Time:** 1-2 hours

- [ ] Add configuration
  - Connection strings
  - Container/bucket names
  - **Time:** 1 hour

#### Testing
- [ ] Test uploads
- [ ] **Time:** 1 hour

**Subtotal: 6-8 hours** ? External integration, potential unknowns

---

### 9. Integration Testing & Polish (4-6 hours)

- [ ] Manual testing of all new endpoints via Swagger
  - **Time:** 2-3 hours

- [ ] Swagger documentation updates
  - All endpoint descriptions
  - Request/response examples
  - **Time:** 1-2 hours

- [ ] Error handling refinement
  - **Time:** 0.5-1 hour

- [ ] Database seeding (sample data)
  - **Time:** 0.5 hours

**Subtotal: 4-6 hours**

---

**SPRINT 4 TOTAL: 34-44 hours** ?

---

## PHASE 2 COMPLETE SUMMARY

| Component | Sprint | Hours | Complexity | External? |
|-----------|--------|-------|-----------|-----------|
| Sports CRUD | 3 | 8-10 | Medium | No |
| Services CRUD | 3 | 5-6 | Low | No |
| Subscriptions CRUD | 3 | 7-8 | Medium | No |
| Tickets API | 3 | 8-10 | Medium | No |
| Reservations CRUD | 4 | 10-12 | **Very High** | No |
| Stores & Offers | 4 | 6-8 | Low-Medium | No |
| FCM Notifications | 4 | 8-10 | High | **YES** |
| File Upload | 4 | 6-8 | High | **YES** |
| Integration Testing | 4 | 4-6 | Medium | No |
| **TOTAL** | **3-4** | **62-78** | - | - |

**Average: ~70 hours**

---

## Detailed Task Breakdown for Implementation

### ? Must-Do First (Foundation)
1. Create all domain entities for Phase 2
2. Run EF Core migrations
3. Implement service interfaces
4. Create controllers

### ?? Integration Points (Watch out!)
1. **FCM Setup** - Requires Google Firebase account + credentials
2. **File Storage** - Requires Azure/MinIO account + configuration
3. **Reservations Logic** - Most complex business logic

### ?? Can Parallelize
- Sports, Services, Subscriptions, Tickets (all similar CRUD patterns)
- They can be built independently
- Only depends on entities being defined

### ?? Risk Areas
1. **Reservation Conflict Detection** - Algorithm complexity
2. **FCM Integration** - External service dependency
3. **File Upload** - Network/storage reliability
4. **Data Seeding** - Sample data generation

---

## Effort by Task Complexity

### ? Quick Tasks (1-2 hours each)
- Services CRUD (simple)
- Stores CRUD
- Database migrations

### ?? Medium Tasks (3-5 hours each)
- Sports CRUD
- Subscriptions CRUD
- Tickets API
- File Upload basic

### ?? Complex Tasks (6-12 hours each)
- Reservations CRUD (conflict detection)
- FCM Notifications (external integration)
- Integration testing

---

## Critical Path

```
Phase 2 Critical Path:
1. Create all domain entities (2 hours)
2. Run migrations (1 hour)
3. Build CRUD services (parallel, 6-8 hours)
   - Sports
   - Services
   - Subscriptions
   - Tickets
4. Build Reservations (8-10 hours) ? Longest
5. Integrate FCM (6-8 hours)
6. Integrate File Upload (4-6 hours)
7. Testing & Polish (4-6 hours)
```

**Critical Path Duration: ~30-35 hours of sequential work**
**Total with parallelization: ~70 hours**

---

## External Dependencies

### Firebase Cloud Messaging (FCM)
**What you need:**
- Google Cloud project
- Firebase setup
- Service account credentials
- Admin SDK

**When:** Should be ready before Sprint 4 Week 7
**Lead Time:** 2-3 hours to setup, then 4-5 hours to integrate

### File Storage (Azure/MinIO)
**What you need:**
- Azure account OR MinIO instance
- Connection credentials
- Container/bucket creation

**When:** Should be ready before Sprint 4 Week 7
**Lead Time:** 2-3 hours to setup, then 3-4 hours to integrate

---

## Updated IMPLEMENTATION_CHECKLIST for Phase 2

### Sprint 3 (Week 5-6)
- [ ] Create Sport entity
  - [ ] Database migration
  - [ ] Service interface & implementation
  - [ ] Controller with CRUD endpoints
  - [ ] Test via Swagger

- [ ] Create Service entity
  - [ ] Database migration
  - [ ] Service & Controller
  - [ ] Test

- [ ] Create Subscription entity
  - [ ] Database migration
  - [ ] Service with linked sports/services
  - [ ] Controller
  - [ ] Test

- [ ] Create Ticket entities (Ticket, TicketBooking)
  - [ ] Database migrations
  - [ ] Service with booking logic
  - [ ] Controller (list, book, cancel)
  - [ ] Test availability logic

### Sprint 4 (Week 7-8)
- [ ] Create Reservation entity with conflict detection
  - [ ] Database migration
  - [ ] Service with slot availability & conflict detection
  - [ ] Controller (get, book, cancel)
  - [ ] Comprehensive testing

- [ ] Create Store & Offer entities
  - [ ] Database migrations
  - [ ] Services & Controllers
  - [ ] Test

- [ ] Setup Firebase Cloud Messaging
  - [ ] Create Google Cloud project
  - [ ] Install Admin SDK
  - [ ] Implement NotificationService
  - [ ] Controller (broadcast, targeted send)
  - [ ] Store FCM tokens

- [ ] Setup File Upload Service
  - [ ] Install Azure/MinIO SDK
  - [ ] Implement storage service
  - [ ] Controller (upload, delete)
  - [ ] Test uploads

- [ ] Integration Testing
  - [ ] Test all endpoints
  - [ ] Update Swagger docs
  - [ ] Performance testing

---

## Realistic Phase 2 Timeline (65-80 hours)

### Week 5 (Mon-Fri) - 15-18 hours
- Day 1-2: Create entities & migrations (4-5 hrs)
- Day 2-3: Build Sports CRUD (6-7 hrs)
- Day 4-5: Build Services & start Subscriptions (5-6 hrs)

### Week 6 (Mon-Fri) - 13-16 hours
- Day 1-2: Complete Subscriptions (4-5 hrs)
- Day 3-4: Build Tickets API (6-8 hrs)
- Day 5: Start Reservations (3-4 hrs)

### Week 7 (Mon-Fri) - 18-22 hours
- Day 1-2: Complete Reservations logic (6-7 hrs)
- Day 3: Setup FCM & Implement (4-5 hrs)
- Day 4-5: File Upload & Stores/Offers (8-10 hrs)

### Week 8 (Mon-Fri) - 18-24 hours
- Day 1-2: Complete File Upload (4-6 hrs)
- Day 3-4: Stores & Offers CRUD (4-6 hrs)
- Day 5: Testing & Integration (6-8 hrs)

**Total: 64-80 hours** ?

---

## Phase 2 Completion Criteria

? All 8 CRUD APIs working (Sports, Services, Subscriptions, Tickets, Stores, Offers, Branches planned)
? Reservations with conflict detection working
? FCM push notifications sending successfully
? File uploads working to Azure/MinIO
? All endpoints tested via Swagger
? Frontend team can integrate with Phase 2 APIs
? Mobile team can integrate with Phase 2 APIs
? Documentation complete and up-to-date

---

## How to Proceed

### Start Phase 2 Now:
1. Create all domain entities first
2. Run EF migrations
3. Begin with Sports CRUD (simplest, builds pattern)
4. Build Services & Subscriptions in parallel
5. Move to Tickets
6. Tackle Reservations (longest)
7. Setup external services (FCM, Storage)
8. Integration testing

### Suggested Commit Order:
```
1. "feat: Add Phase 2 domain entities (Sport, Service, Subscription, etc)"
2. "feat: Implement Sports CRUD API"
3. "feat: Implement Services CRUD API"
4. "feat: Implement Subscriptions CRUD API"
5. "feat: Implement Tickets API with booking"
6. "feat: Implement Reservations API with conflict detection"
7. "feat: Integrate FCM push notifications"
8. "feat: Integrate file upload service"
9. "feat: Add Stores and Offers CRUD APIs"
10. "test: Phase 2 integration testing complete"
```

---

## Estimated Completion

**Starting NOW (Week 5):**
- Week 5-6: Sprint 3 complete (28-34 hrs)
- Week 7-8: Sprint 4 complete (34-44 hrs)
- **Phase 2 DONE: End of Week 8** ?

**Then ready for:**
- Frontend integration (Week 9)
- Mobile integration (Week 9)
- Phase 3 starts (Week 9)

---

**You're ready! Phase 1 foundation is solid. Phase 2 is well-defined and achievable in 4 weeks.**
