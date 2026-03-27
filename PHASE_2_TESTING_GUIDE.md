# ?? Phase 2 Testing Guide

## Quick Start

### 1. Run the API
```bash
cd D:\Work\AI-Projects\Sporting-Club
.\run-api.bat
```

The API will start on `http://localhost:5244`

### 2. Open Swagger UI
```
http://localhost:5244/swagger
```

---

## Testing Each Phase 2 API

### Authentication First (Required)
1. Go to Swagger UI
2. Find `AuthController`
3. Execute `POST /api/v1/auth/register` with:
```json
{
  "email": "testuser@example.com",
  "fullName": "Test User",
  "password": "TestPassword123!",
  "role": "Member"
}
```
4. Copy the `accessToken` from response
5. Click "Authorize" button (top right)
6. Paste: `Bearer {accessToken}`

---

## Testing Sports API

### Create a Sport
```
POST /api/v1/sports
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Football",
  "description": "Indoor football league",
  "schedule": "Monday & Friday 6-8 PM",
  "coachId": null,
  "branchId": null
}
```

### List Sports
```
GET /api/v1/sports?page=1&pageSize=10&search=football&sortBy=name&sortDirection=asc
```

### Get Sport Details
```
GET /api/v1/sports/{id}
```

### Update Sport
```
PUT /api/v1/sports/{id}
Authorization: Bearer {token}

{
  "name": "Football - Updated",
  "description": "Updated description",
  "schedule": "Monday & Friday 6-9 PM",
  "coachId": null,
  "branchId": null
}
```

### Delete Sport
```
DELETE /api/v1/sports/{id}
Authorization: Bearer {token} (Admin required)
```

---

## Testing Reservations API (? Most Important)

### 1. Get Available Slots
```
GET /api/v1/reservations/sport/{sportId}/available-slots?date=2025-02-01
```

Response:
```json
[
  "09:00-10:00",
  "10:00-11:00",
  "11:00-12:00",
  "12:00-13:00",
  "13:00-14:00",
  "14:00-15:00",
  "15:00-16:00",
  "16:00-17:00",
  "17:00-18:00"
]
```

### 2. Check for Conflict
```
GET /api/v1/reservations/sport/{sportId}/check-conflict?date=2025-02-01&timeSlot=09:00-10:00
```

Response:
```json
{
  "hasConflict": false
}
```

### 3. Create a Reservation
```
POST /api/v1/reservations
Authorization: Bearer {token}
Content-Type: application/json

{
  "memberId": "{userId}",
  "sportId": "{sportId}",
  "reservationDateUtc": "2025-02-01T09:00:00Z",
  "timeSlot": "09:00-10:00",
  "notes": "Football match"
}
```

Response (201 Created):
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "memberId": "...",
  "sportId": "...",
  "reservationDateUtc": "2025-02-01T09:00:00Z",
  "timeSlot": "09:00-10:00",
  "status": "Confirmed",
  "notes": "Football match",
  "createdAtUtc": "2025-01-27T12:00:00Z",
  "updatedAtUtc": "2025-01-27T12:00:00Z"
}
```

### 4. Try to Book Same Slot (Should Fail)
```
POST /api/v1/reservations
Authorization: Bearer {token}

{
  "memberId": "{anotherUserId}",
  "sportId": "{same sportId}",
  "reservationDateUtc": "2025-02-01T09:00:00Z",
  "timeSlot": "09:00-10:00",
  "notes": "Try to book same slot"
}
```

Response (400 Bad Request):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Time slot is already booked"
}
```

### 5. List Reservations
```
GET /api/v1/reservations?page=1&pageSize=10&sortBy=date
Authorization: Bearer {token} (Staff+ required)
```

### 6. Cancel Reservation
```
DELETE /api/v1/reservations/{id}
Authorization: Bearer {token}
```

---

## Testing Tickets API

### Create a Ticket
```
POST /api/v1/tickets
Authorization: Bearer {token} (Manager+ required)

{
  "eventName": "Cup Final 2025",
  "quantity": 100,
  "price": 50.00,
  "availableSinceUtc": "2025-01-27T00:00:00Z",
  "availableUntilUtc": "2025-02-15T23:59:59Z"
}
```

### List Available Tickets
```
GET /api/v1/tickets?page=1&pageSize=10&search=cup
```

### Book a Ticket
```
POST /api/v1/tickets/{ticketId}/book
Authorization: Bearer {token}
Content-Type: application/json

3
```

Response:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "ticketId": "...",
  "userId": "...",
  "quantityBooked": 3,
  "bookedAtUtc": "2025-01-27T12:00:00Z"
}
```

### Get My Bookings
```
GET /api/v1/tickets/my-bookings
Authorization: Bearer {token}
```

---

## Testing Offers API

### Create an Offer
```
POST /api/v1/offers
Authorization: Bearer {token} (Manager+ required)

{
  "storeId": "{storeId}",
  "description": "50% off all sports equipment",
  "discountPercentage": 50.0,
  "startDateUtc": "2025-01-27T00:00:00Z",
  "endDateUtc": "2025-02-28T23:59:59Z"
}
```

### List All Offers
```
GET /api/v1/offers?page=1&pageSize=10
```

### List Active Offers Only
```
GET /api/v1/offers/active?page=1&pageSize=10
```

Active = startDate <= now <= endDate

---

## Authorization Testing

### Public Endpoints (No Auth Required)
? GET /sports
? GET /services
? GET /subscriptions
? GET /tickets (only active)
? GET /offers/active
? GET /stores

### Member Only (Authenticated)
? POST /reservations
? POST /tickets/{id}/book
? GET /tickets/my-bookings

### Staff+ (Staff, Manager, Admin)
? GET /reservations (list all)

### Manager+ (Manager, Admin)
? POST /sports
? POST /services
? POST /tickets
? POST /offers
? PUT /sports/{id}
? PUT /services/{id}
? PUT /offers/{id}

### Admin Only
? DELETE /sports/{id}
? DELETE /services/{id}
? DELETE /tickets/{id}
? DELETE /offers/{id}
? POST /subscriptions
? PUT /subscriptions/{id}
? DELETE /subscriptions/{id}

---

## Common Test Scenarios

### Scenario 1: Full Reservation Flow
1. ? Create a Sport (as Manager)
2. ? Get available slots for that sport
3. ? Create a reservation (as Member)
4. ? Get available slots again (should have one less)
5. ? Try to book same slot (should fail)
6. ? Cancel reservation
7. ? Slot becomes available again

### Scenario 2: Ticket Booking
1. ? Create a ticket with 10 quantity
2. ? Book 3 tickets (user 1)
3. ? Book 5 tickets (user 2)
4. ? Try to book 3 more (user 3) - should fail (only 2 left)

### Scenario 3: Offer Management
1. ? Create store
2. ? Create offer starting tomorrow
3. ? GET /offers/active - should not appear
4. ? Create offer that starts today
5. ? GET /offers/active - should appear

---

## Troubleshooting

### "401 Unauthorized"
? You need to authenticate first
? Use the Authorize button in Swagger with a valid token

### "403 Forbidden"
? Your role doesn't have permission
? Try with an Admin account

### "404 Not Found"
? The resource doesn't exist
? Check the ID is correct

### "400 Bad Request"
? Business logic validation failed (e.g., slot already booked)
? Check the error message in response

### "500 Internal Server Error"
? Something went wrong on the server
? Check the API console logs

---

## Performance Testing

### Pagination Test
```
GET /api/v1/sports?page=1&pageSize=5
GET /api/v1/sports?page=2&pageSize=5
```

### Search Test
```
GET /api/v1/sports?search=football
GET /api/v1/sports?search=xyz (should be empty)
```

### Sorting Test
```
GET /api/v1/sports?sortBy=name&sortDirection=asc
GET /api/v1/sports?sortBy=name&sortDirection=desc
GET /api/v1/sports?sortBy=created&sortDirection=desc
```

---

## Next Steps

- ? Test all Phase 2 endpoints manually
- ? Verify authorization on each endpoint
- ? Test conflict detection in reservations
- ? Test pagination, search, sorting
- ? Integration testing with frontend/mobile
- ? Load testing if needed

**All Phase 2 endpoints are ready for testing!** ??
