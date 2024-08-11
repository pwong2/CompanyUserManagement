API Documentation<br /><br />

*** Authentication and Authorization ***<br />
**1. POST /api/auth/login**<br />
Description: Authenticates a user and returns a JWT token.

Request:
{
  "username": "string",
  "password": "string"
}

Responses:

200 OK: Returns the JWT token.
{
  "token": "string"
}

400 Bad Request: Invalid username or password.
{
  "error": "Invalid username or password."
}

500 Internal Server Error:
Server encountered an error while processing the request.
<br /><br />

-----------------------------------------------------------------------------------------------------------------------------------------------------------

*** Users Management ***<br />
**2. GET /api/users**<br />
Description: Retrieves a list of users belonging to the current user's company. If the current user is not an admin, the response will exclude admin users.

Authorization: JWT Token (Bearer token required)

Responses:

200 OK: Returns a list of users.
[
  {
    "id": 1,
    "username": "string",
    "role": "string",
    "companyId": 1,
    "createdAt": "2024-08-11T12:34:56Z",
    "updatedAt": "2024-08-11T12:34:56Z"
  }
]

401 Unauthorized:
Invalid or missing user ID in claims.
{
  "message": "Invalid or missing user ID in claims."
}

500 Internal Server Error: Server encountered an error while processing the request.

-----------------------------------------------------------------------------------------------------------------------------------------------------------

*** Users Management ***<br />
**3. GET /api/users/all**<br />
Description: Retrieves a list of all users across all companies. Only accessible by admin users.

Authorization:
JWT Token (Bearer token required), Admin Role

Responses:

200 OK: Returns a list of all users.
[
  {
    "id": 1,
    "username": "string",
    "role": "string",
    "companyId": 1,
    "createdAt": "2024-08-11T12:34:56Z",
    "updatedAt": "2024-08-11T12:34:56Z"
  }
]

401 Unauthorized: Non-admin users attempting to access this endpoint.
{
  "message": "You do not have the required permissions to access this resource."
}

500 Internal Server Error:
Server encountered an error while processing the request.

-----------------------------------------------------------------------------------------------------------------------------------------------------------

*** Users Management ***<br />
**4. PUT /api/users/{id}**<br />
Description: Updates the user details for the specified user ID.

Authorization: JWT Token (Bearer token required), Admin Role <br />
Request:
{
  "username": "string",       // optional
  "password": "string",       // optional
  "companyId": 1,             // optional
  "role": "Admin"             // optional
}

Responses:
200 OK:
User updated successfully.
{
  "message": "User updated successfully"
}

400 Bad Request:
Invalid data provided.
{
  "error": "Invalid data provided."
}

401 Unauthorized:
Non-admin users attempting to update user details.
{
  "message": "Only Admin users can update users."
}

404 Not Found:
User not found.
{
  "error": "User not found."
}

500 Internal Server Error:
Server encountered an error while processing the request.

-----------------------------------------------------------------------------------------------------------------------------------------------------------

*** Users Management ***<br />
**5. DELETE /api/users/{id}**<br />
Description: Deletes the user with the specified ID.

Authorization: JWT Token (Bearer token required), Admin Role <br />

Responses:

200 OK:
User deleted successfully.
{
  "message": "User deleted successfully"
}

400 Bad Request:
Invalid user ID.
{
  "error": "Invalid user ID."
}

401 Unauthorized:
Non-admin users attempting to delete a user.
{
  "message": "Only Admin users can delete users."
}

404 Not Found:
User not found.
{
  "error": "User not found."
}

500 Internal Server Error:
Server encountered an error while processing the request.

-----------------------------------------------------------------------------------------------------------------------------------------------------------

*** Throttling ***<br />
**6. ThrottlingMiddleware<br />**
Description: Limits the number of requests a user can make to the API to 10 requests per minute.

Responses:

429 Too Many Requests:
Request limit exceeded.
{
  "message": "Request limit exceeded. Try again later."
}
