# test 
[string]$base = "http://localhost:1607/api"
[string]$chat = "room1"

# chat room creation
$R = Invoke-WebRequest -Uri "$base/room/$chat" -Method Post -ContentType "application/json" -Body (ConvertTo-Json @{"description"="Lorem ipsum dolor sit amet"})

if ($R.StatusCode -eq 201)
{
	$room = ConvertFrom-Json $R.Content
	"Successfuly room created."
	"Resource location:".PadLeft(20).PadRight(21) + $R.Headers.Location
	"Room info".PadLeft(11)
	"id".PadLeft(6).PadRight(7) + "'" + $room.id + "'"
	"alias".PadLeft(9).PadRight(10)+ "'" + $room.alias + "'"
	"description".PadLeft(15).PadRight(16)+ "'" + $room.description + "'"
}
else
{
	"Error creating room"
}

# first user
[string]$user1_id = "d0a0ee8d-e08b-48ad-92ea-f2a9f3fd25d8"

$R = Invoke-WebRequest -Uri "$base/profile/$user1_id" -Method Post -ContentType "application/json" -Body (ConvertTo-Json @{"name"="Lorem ipsum dolor sit amet"})

if ($R.StatusCode -eq 201)
{
	$user = ConvertFrom-Json $R.Content
	"Successfuly profile updated."
	"Resource location:".PadLeft(20).PadRight(21) + $R.Headers.Location
}
else
{
	"Error creating room"
}

# put user into chat

$R = Invoke-WebRequest -Uri "$base/chat/$chat" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"user"=$user1_id})

if ($R.StatusCode -eq 200)
{
	"Successfuly added user to chat."
}

# publish message into chat stream

$R = Invoke-WebRequest -Uri "$base/message/$chat" -Method Post -ContentType "application/json" -Body (ConvertTo-Json @{"author"=$user1_id; "text"="Lorem ipsum dolor sit amet"})

if ($R.StatusCode -eq 201)
{
	$user = ConvertFrom-Json $R.Content
	"Successfuly message posted."
	"Resource location:".PadLeft(20).PadRight(21) + $R.Headers.Location
}

#second user
#$user2 = "9027498e-7572-4f70-a2b2-70257f94abb1"
#$R = Invoke-WebRequest -Uri "$base/profile/$user2" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"name"="Consectetur adipiscing elit"})
#$R = Invoke-WebRequest -Uri "$base/chat/$chat" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"user"=$user2})

# third user
#$user3 = "4bfecf95-10ce-4878-acb6-cae90f2ae1a2"
#$R = Invoke-WebRequest -Uri "$base/profile/$user3" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"name"="Integer auctor purus vel"})
#$R = Invoke-WebRequest -Uri "$base/chat/$chat" -Method Put -ContentType "application/json" -Body (ConvertTo-Json @{"user"=$user3})

# who is in room1?
#Invoke-WebRequest -Uri "$base/chat/$chat" -Method Get -Headers @{"Accept"="application/json"} | Select-Object -ExpandProperty Content

#Invoke-WebRequest -Uri "$base/poll/$chat" -Method Get -Headers @{"Accept"="application/json"} | Select-Object -ExpandProperty Content