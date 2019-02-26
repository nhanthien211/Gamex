Gamex API

1. Working endpoints (tested with Postman):
- POST /api/account/register
  
json body:
```
{
  "Email": "user02@gmail.com",
  "Password": "1qazXSW@",
  "ConfirmPassword": "1qazXSW@",
  "FirstName": "Bill",
  "LastName": "Gates",
  "Point": 0,
  "TotalPointEarned": 0
}
```

- GET /token

body: x-www-form-urlencoded
```
grant_type:password
username:user02@gmail.com
password:1qazXSW@
```

response:
```
{
    "access_token": "V1s1RToqK9-6D0YEeABnHafrbFWXd5lXiXOG4ZGdv-_jK37DMZ9xEh7GiFTDwtDQNJw_CcTexwR5nFvBIZW9iKpWDT2zeZiAZQWWFQqaFfkTKtgJPnbCn599KBqBQO2acgOVXHYLFYv_1n6jHvsmQ-Afj1gh6c9o8McRZRao49VephIxUNzqkx5UVAg7PWQcGXgWuDbQMBcikdn6Epr-CAm3XcWft-arxvy749p1pttFvtr1rvC4dDKhPvP-VT6bDd5Bjo3ky7bZHWAI4ZhH6f8kCUfSyWgEIvorlF86otdu5g3pF3EsFS3jYAFccfCfM573cKLfnBM187PCgVjHy-3xNzX80j1SuGrH03dKPxqDEGFdcS9kxCKkX7VdYaoTKW2n-MGFTBdlXQ8WaJEYUfy79YOkqFgnagnZStJ1b7kIyptm2XzdUiIXvHRUsiZV5EosnsCvv0LU3Dlz_n4fUw3G8rjisaeswikzQB52VJR2xRXuw0Wf0CpejbUD4mt3",
    "token_type": "bearer",
    "expires_in": 1209599,
    "userName": "user02@gmail.com",
    ".issued": "Mon, 25 Feb 2019 13:50:02 GMT",
    ".expires": "Mon, 11 Mar 2019 13:50:02 GMT"
}
```

note: `access_token`'s expire time is 10 day. Considering apply `refresh_token` and reduce `access_token`'s expire time

- GET /api/account/userinfo

header:
```
authorization: bearer <access_token>
```

response:
```
{
    "email": "user02@gmail.com",
    "hasRegistered": true,
    "loginProvider": null
}
```