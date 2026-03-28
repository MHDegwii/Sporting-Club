# ?? Postman Collection - Phase 2 Complete Guide

## ?? File Location

```
D:\Work\AI-Projects\Sporting-Club\Postman_Collection_Phase2_Complete.json
```

**File Name:** `Postman_Collection_Phase2_Complete.json`  
**Size:** ~34 KB  
**Version:** 2.0 - Phase 2 Complete  
**Last Updated:** March 28, 2026

---

## ?? Collection Overview

### ? Features

? **50+ Endpoints** organized by module  
? **Sample Requests & Responses** for each endpoint  
? **Auto Token Saving** - Token automatically saved after login  
? **Environment Variables** - Easy configuration  
? **Complete Documentation** - Descriptions for all endpoints  
? **Error Examples** - Shows what errors look like  
? **Role-Based Access** - Notes about required roles  

---

## ?? Quick Start

### 1. Import the Collection

**In Postman:**
1. Click **Import** (top-left)
2. Select **Upload Files**
3. Choose: `Postman_Collection_Phase2_Complete.json`
4. Click **Import**

### 2. Set Environment Variables

**In Postman:**
1. Create a new Environment
2. Set variables:
   - `baseUrl`: `http://localhost:5244`
   - `token`: *(leave empty, auto-fills after login)*
   - `refreshToken`: *(leave empty, auto-fills after login)*

### 3. Start Testing

1. **Register or Login** first (in Auth section)
2. Token automatically saves to environment
3. Use all other endpoints with auto-filled token

---

## ?? Collection Structure

### 1. ?? SETUP & AUTHENTICATION (3 endpoints)
- **Register** - Create new user account
- **Login** - Get access token (auto-saves token)
- **Refresh Token** - Get new access token

### 2. ? SPORTS (5 endpoints)
- List Sports
- Get Sport by ID
- Create Sport
- Update Sport
- Delete Sport

### 3. ??? SERVICES (Multiple endpoints)
- List Services
- Create Service

### 4. ?? SUBSCRIPTIONS (Multiple endpoints)
- List Subscriptions
- Create Subscription

### 5. ??? TICKETS (5+ endpoints)
- List Tickets
- Create Ticket
- Book Tickets ?
- My Ticket Bookings

### 6. ?? RESERVATIONS ? (6+ endpoints - Most Complex!)
- **Get Available Slots** - See free time slots
- **Check Conflict** - Check if slot is booked
- **List Reservations** - All reservations
- **Create Reservation** - With conflict detection
- **Update Reservation** - Can change time/date
- **Cancel Reservation** - Cancel booking

### 7. ?? STORES (Multiple endpoints)
- List Stores
- Create Store

### 8. ?? OFFERS (Multiple endpoints)
- List All Offers
- List Active Offers ?
- Create Offer

### 9. ?? ANALYTICS (1 endpoint)
- Dashboard Summary

### 10. ?? OPERATIONS (1 endpoint)
- Health Check - Ready

---

## ?? Authentication Flow

### Step 1: Register or Login

**First Time:**
```
1. Go to "SETUP & AUTHENTICATION"
2. Click "1. Auth - Register"
3. Update email/password
4. Send request
5. Token auto-saved! ?
```

**Subsequent Times:**
```
1. Go to "SETUP & AUTHENTICATION"
2. Click "2. Auth - Login"
3. Update email/password
4. Send request
5. Token auto-saved! ?
```

### Step 2: Use Other Endpoints

All other endpoints automatically use `{{token}}` from environment.

---

## ?? Sample Requests & Responses

### Auth - Login
**Request:**
```json
{
  "email": "testuser@example.com",
  "password": "TestPass@123"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eye2FsY2lzIn0...",
  "expiresAtUtc": "2026-03-28T01:47:00Z"
}
```

### Reservations - Check Conflict
**Request:**
```
GET /api/v1/reservations/sport/{sportId}/check-conflict?date=2026-02-01&timeSlot=09:00-10:00
```

**Response (No Conflict):**
```json
{
  "hasConflict": false
}
```

**Response (Conflict Exists):**
```json
{
  "hasConflict": true
}
```

### Reservations - Get Available Slots
**Request:**
```
GET /api/v1/reservations/sport/{sportId}/available-slots?date=2026-02-01
```

**Response:**
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

### Tickets - Book
**Request Body:** Just the number (not JSON!)
```
3
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "ticketId": "550e8400-e29b-41d4-a716-446655440001",
  "userId": "550e8400-e29b-41d4-a716-446655440099",
  "quantityBooked": 3,
  "bookedAtUtc": "2026-03-27T01:47:00Z"
}
```

---

## ?? Key Endpoints to Test

### 1. Start with Auth
```
1. Register (Auth - Register) OR
2. Login (Auth - Login)
```

### 2. Test Health
```
Health - Ready (in Operations)
```

### 3. Test Each Module
```
? Sports ? List Sports
??? Services ? List Services
?? Subscriptions ? List Subscriptions
??? Tickets ? List Tickets
?? Reservations ? List Reservations
?? Stores ? List Stores
?? Offers ? List Active Offers
?? Analytics ? Dashboard Summary
```

### 4. Test Complex Flows
```
Reservations:
1. Get Available Slots (see what's free)
2. Check Conflict (confirm slot is free)
3. Create Reservation (book the slot)
4. Update Reservation (change details)
5. Cancel Reservation (cancel booking)

Tickets:
1. List Tickets (see available)
2. Book Tickets (reserve quantity)
3. My Bookings (see your bookings)
```

---

## ?? Environment Variables

### Default Variables
```
baseUrl        = http://localhost:5244
token          = (auto-filled after login)
refreshToken   = (auto-filled after login)
```

### How to Change baseUrl
1. Click environment icon (top-right in Postman)
2. Click "Edit"
3. Change `baseUrl` value
4. Save

---

## ?? Authorization

### All Endpoints Require Token EXCEPT:
- Auth - Register (no token needed)
- Auth - Login (no token needed)
- Health - Ready (no token needed)

### Adding Token to Headers
Token is automatically added to all requests via:
```
Authorization: Bearer {{token}}
```

---

## ?? Reservation Conflict Detection (? Star Feature)

### How It Works
1. **Check Available Slots** - See what times are free
2. **Check Conflict** - Verify before booking
3. **Create Reservation** - System checks conflict again
4. **Auto Rejects** - If slot taken, returns 400 error

### Example Flow
```
1. GET /reservations/sport/{id}/available-slots?date=2026-02-01
   Response: ["09:00-10:00", "10:00-11:00", ...]

2. GET /reservations/sport/{id}/check-conflict?date=2026-02-01&timeSlot=09:00-10:00
   Response: {"hasConflict": false}

3. POST /reservations
   Body: {..., "timeSlot": "09:00-10:00"}
   Response: 201 Created ?

4. Try same slot again:
   Response: 400 Bad Request - "Time slot is already booked"
```

---

## ??? Tickets Booking

### Important Notes
- **Book Request Body** - Just send the NUMBER, not JSON
- **Example:** Send `3` (not `{"quantity": 3}`)
- **Response** - Returns booking ID and details

### Example
```
POST /api/v1/tickets/{ticketId}/book
Content-Type: application/json

3

Response:
{
  "id": "...",
  "quantityBooked": 3,
  "bookedAtUtc": "2026-03-27T01:47:00Z"
}
```

---

## ??? Role-Based Access

### Public (No Auth Required)
- Auth Register
- Auth Login
- Health Check

### Member
- All Sports endpoints
- All Tickets endpoints
- All Reservations endpoints
- View Offers/Stores

### Staff
- Everything Member can do
- List Reservations
- View all bookings

### Manager
- Everything Staff can do
- Create/Update/Delete Sports, Services, Stores, Offers
- Create/Update Subscriptions
- Create/Update Tickets

### Admin
- Everything
- Delete any resource
- Manage users

---

## ?? Common Errors

### 401 Unauthorized
**Cause:** Token missing or expired  
**Fix:** Login again (Auth - Login)

### 400 Bad Request
**Cause:** Invalid data or conflict  
**Example:** Trying to book same time slot twice  
**Response:**
```json
{
  "status": 400,
  "detail": "Time slot is already booked"
}
```

### 404 Not Found
**Cause:** Resource doesn't exist  
**Fix:** Check ID is correct

### 403 Forbidden
**Cause:** Don't have permission  
**Fix:** Need higher role (e.g., Manager or Admin)

---

## ?? Tips & Tricks

### 1. Save IDs for Later Use
When you create a resource, save the ID:
```json
// Response includes ID
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Football",
  ...
}
```

Use this ID in URL for other requests.

### 2. Use Environment Variables
Replace hardcoded IDs with variables:
```
Original: /api/v1/sports/550e8400-e29b-41d4-a716-446655440000
Better:   /api/v1/sports/{{sportId}}
```

### 3. Run Collection (Test All at Once)
1. Click on collection name
2. Click "Run" button
3. Executes all requests in order

### 4. Use Tests Tab
Each request has sample assertions in Tests tab to verify responses.

### 5. View Request History
Click "History" tab to see all previous requests and responses.

---

## ?? Related Documentation

- **Testing Guide:** `docs/phase2/PHASE_2_TESTING_GUIDE.md`
- **API Dashboard:** `docs/phase2/PHASE_2_DASHBOARD.md`
- **Deliverables:** `docs/phase2/PHASE_2_DELIVERABLES.md`
- **Resource Index:** `docs/phase2/PHASE_2_RESOURCE_INDEX.md`

---

## ? Verification Checklist

After importing, verify:
- [ ] Collection imported successfully
- [ ] Can see all 10 module folders
- [ ] Can see 50+ endpoints
- [ ] Base URL set to `http://localhost:5244`
- [ ] Can run Health Check (no auth needed)
- [ ] Can login and get token
- [ ] Token auto-saves to environment
- [ ] Can run authenticated endpoints

---

## ?? Testing Workflow

### Quick Test (5 minutes)
```
1. Import collection
2. Login (Auth - Login)
3. Run Health Check (Operations - Ready)
4. List Sports (Sports - List Sports)
5. Done! ?
```

### Complete Test (30 minutes)
```
1. Import collection
2. Login
3. Test each module:
   - Sports (create, update, list)
   - Services (list, create)
   - Subscriptions (list)
   - Tickets (list, book)
   - Reservations (check conflict, create, list)
   - Stores (list)
   - Offers (list active)
   - Analytics (dashboard)
```

### Advanced Test (1+ hour)
```
1. Complete workflow for each module
2. Test error scenarios (invalid data, conflicts)
3. Test role-based access
4. Test pagination
5. Test filtering and sorting
6. Test complete reservation flow with conflicts
7. Test ticket booking flow
```

---

## ?? Quick Links

| Purpose | Path |
|---------|------|
| **Collection File** | `Postman_Collection_Phase2_Complete.json` |
| **Testing Guide** | `docs/phase2/PHASE_2_TESTING_GUIDE.md` |
| **API Docs** | `http://localhost:5244/swagger` |
| **Project Docs** | `docs/README.md` |

---

## ?? Support

### API Not Responding?
1. Ensure API is running: `.\run-api.bat`
2. Check base URL: Should be `http://localhost:5244`
3. Check health: `GET {{baseUrl}}/api/v1/operations/ready`

### Token Issues?
1. Logout and login again
2. Use Auth - Refresh Token
3. Check token expiry in response

### Endpoint Not Found?
1. Verify endpoint path
2. Check query parameters
3. See PHASE_2_TESTING_GUIDE.md for examples

---

## ? Summary

**What You Have:**
- ? 50+ API endpoints
- ? All sample requests & responses
- ? Complete authentication flow
- ? Auto token management
- ? Comprehensive documentation
- ? Ready to test immediately

**What You Can Do:**
- ? Test all Phase 2 APIs
- ? Manage authentication
- ? Test complex reservation flows
- ? Verify error handling
- ? Integrate with frontend

---

**Happy Testing!** ??

For more info, see: `docs/phase2/PHASE_2_TESTING_GUIDE.md`
