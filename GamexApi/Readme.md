# Gamex API

## 1. Working endpoints (tested with Postman):
### Register Local

	`POST /api/account/`
content-type: `application/json`
```json
{
  "Username": "user02",
  "Email": "user02@gmail.com", //allow null
  "Password": "1qazXSW@",
  "ConfirmPassword": "1qazXSW@",
  "FirstName": "Bill",
  "LastName": "Gates"
}
```

### Login - get access token
	
	`GET /token`

content-type: `x-www-form-urlencoded`
```
grant_type:password
username:user02
password:1qazXSW@
```

*response*:
```
{
    "access_token": "<access token string>",
    "token_type": "bearer",
    "expires_in": 1209599,
    "userName": "user02",
    ".issued": "Mon, 25 Feb 2019 13:50:02 GMT",
    ".expires": "Mon, 11 Mar 2019 13:50:02 GMT"
}
```

*Note*: `access_token`'s expire time is 10 day. Considering apply `refresh_token` and reduce `access_token`'s expire time

### Get user info

	`GET /api/account/userinfo`

*header*:
```
authorization: Bearer <access_token>
```

*response*:
```
{
    "email": "user02",
    "hasRegistered": true,
    "loginProvider": null
}
```

### Register External

#### Step 1. Get external providers
	`GET /api/Account/ExternalLogins?returnUrl=%2F&generateState=true`
*response*
```json
[
    {
        "name": "Facebook",
        "url": "/api/Account/ExternalLogin?provider=Facebook&response_type=token&client_id=self&redirect_uri=https%3A%2F%2Flocalhost%3A44319%2F&state=IAYuLNJGOag1hFv-hX6FWXcvvV5fsLlEcHoghybSUnU1",
        "state": "IAYuLNJGOag1hFv-hX6FWXcvvV5fsLlEcHoghybSUnU1"
    }
]
```
#### Step 2. Access url in the response above
	`GET aboveResponse.url`
- You will receive a web view for Facebook login.
- After login successfully and approve access data, you will be redirected back to the app with the following info:

`https://localhost:44319/#access_token=SO-YuP6ERrmbPnRWCBcANs8cN7IVaBn4y9rT4Ho1tgghYDht0flLh_QmrndG_9oj5aV_2_MctwYeFIK4A59dF9QAXoI49NOSy-uLuAzjyYKChepN56Hre_DXE5keffDw1Fn5q6pzLH6yL5UtKPc9i_Mggv0sbwrQnIWIiB9X8AS4LeuYyPxPJUJe2r5LY7SyyE1zlg8V5MFJadYwTyv7mcM71QIH6Uvm9jzUm3kHFWk2F3xpMPVbYwHHZcBWmMbpLZ72j7JZQPW1DLAvMRs-ZNviYiYd40E4-tkNBdmxYXE55yeolpqelzr5yN4S35STLmEmOdf729RTbSjMp1j8cYTCyCJHPoB6S0sWMSON3Vw&token_type=bearer&expires_in=864000&state=Bbn88C6rMCxKKRSGz1aHUpCxSIa6PVEp4k__-FbI64E1`
- Extract the access_token, you can use it to get user info
- If this account has registered, the `.AspNet.Cookies` is set and we done.
- Else, only the `.AspNet.ExternalCookie` is set, and need to be registered.
#### Step 3. Register External
```
POST /api/Account/RegisterExternal
authorization: Bearer <access token from url above>
content-type: application/json

{
	"Username": "johnwick",
	"Email": "john_wick@gmail.com",
	"FirstName": "John",
	"LastName": "Wick"
}
```
*Important: POST with .AspNet.ExternalCookie cookie. Don't know if the cookie is automatic added or not. Try sending without explicitly adding cookie first. Good luck :v*

*Response*: `200 OK`
User now has registered a local account

#### Step 4. Get access token
	`GET api/Account/ExternalLogin?provider=Facebook&response_type=token&client_id=self&redirect_uri=https%3A%2F%2Flocalhost%3A44319%2F	
*Note: Send with `.AspNet.Cookies` cookie*
We will get a url as step 2 above, which contains an `access_token`. Use this token to access authorized resources.

## 2. Exhibition
`GET /api/exhibition?id=<id>`

`GET /api/exhibitions?list=<list>&type=<type>&take=<take>&skip=<skip>&lat=<lat>&lng=<lng>`

    `list` = "checked-in". Include this parameter to get the checked-in exhibition list of logged in user.

    `type` = "ongoing", "upcoming", "near-you". Default to "ongoing". "near-you" default distance is 5000 meters.
    
    `take` default to 5

    `skip` default to 0

    `lat` and `lng` is of your location. 