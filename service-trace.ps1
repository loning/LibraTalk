# test 
$base = "http://localhost:1607/api"
$chat = "room1"

# first user
$user1 = "d0a0ee8d-e08b-48ad-92ea-f2a9f3fd25d8"
$R = Invoke-WebRequest -Uri "$base/profile/$user1" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"name"="Lorem ipsum dolor sit amet"})
$R = Invoke-WebRequest -Uri "$base/chat/$chat" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"user"=$user1})

#second user
$user2 = "9027498e-7572-4f70-a2b2-70257f94abb1"
$R = Invoke-WebRequest -Uri "$base/profile/$user2" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"name"="Consectetur adipiscing elit"})
$R = Invoke-WebRequest -Uri "$base/chat/$chat" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"user"=$user2})

# third user
$user3 = "4bfecf95-10ce-4878-acb6-cae90f2ae1a2"
$R = Invoke-WebRequest -Uri "$base/profile/$user3" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"name"="Integer auctor purus vel"})
$R = Invoke-WebRequest -Uri "$base/chat/$chat" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"user"=$user3})

# who is in room1?
Invoke-WebRequest -Uri "$base/chat/$chat" -Method Get -Headers @{"Accept"="application/json"} | Select-Object -ExpandProperty Content