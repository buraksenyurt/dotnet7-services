# .Net 7 App Services

In this repo, I include the codes that I try to examine the service development strategies on the .Net 7 side through a sample solution.

Solution uses a simple and popular database, chinook. Database provider is Postgresql and reside on docker container.

## Some Useful Informations

```shell
# to use Postgresql
docker run --name postgresql -e POSTGRES_DB=chinook POSTGRES_USER=scoth -e POSTGRES_PASSWORD=tiger -p 5432:5432 -v /data:/var/lib/postgresql/data -d postgres

# to use ef tool
dotnet tool install -g dotnet-ef
# or update the existing dotnet-ef tool
dotnet tool update -g dotnet-ef

# create model classes from Chinook Db
# execute on RockShop.Common.Models project library
dotnet ef dbcontext scaffold "Host=localhost;Database=chinook;Username=scoth;Password=tiger" Npgsql.EntityFrameworkCore.Postgresql --namespace RockShop.Shared --data-annotations

# start existing container
docker start postgresql
# to working container list
docker ps -a
# to stop container
docker stop postgresql
# drop container
docker rm [container_id]

# if you want you can use docker-compose file
docker-compose up -d
```

## Run

```bash
# to run
dotnet run

# to run with https profile
dotnet run --launch-profile https
```

### Sample Requests

```bash
# Ping - Pong
curl -X 'GET' 'http://localhost:5221/ping' -H 'accept: text/plain'

# Get albums with paging
curl -X 'GET' 'http://localhost:5221/api/albums?page=5' -H 'accept: application/json'

# Get albums by artist name
curl -X 'GET' 'http://localhost:5221/api/albums/mfö' -H 'accept: application/json'

# Get artists with paging
curl -X 'GET' 'http://localhost:5221/api/artists?page=10' -H 'accept: application/json'

# Get album by id
curl -X 'GET' 'http://localhost:5221/api/albums/45' -H 'accept: application/json'

# Get customers which living in Portugal
curl -X 'GET' 'http://localhost:5221/api/customers/Portugal' -H 'accept: application/json'

# Top * total sales by country
curl -X 'GET' 'http://localhost:5221/api/invoices/totalsales/top/5' -H 'accept: application/json'
curl -X 'GET' 'http://localhost:5221/api/invoices/totalsales/top/3' -H 'accept: application/json'
curl -X 'GET' 'http://localhost:5221/api/invoices/totalsales/top/-1' -H 'accept: application/json'

# Get Some Trucks informations with paging
curl -X 'GET' 'http://localhost:5221/api/tracks?page=25' -H 'accept: application/json'

# Add artist with albums
curl -X 'POST' \
  'http://localhost:5221/api/artists' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "name": "MFÖ",
  "albums": [
    {
      "title": "Ele Gün Karşı Yapayalnız"
    },

    {
      "title": "Peki peki anladık"
    },
    {
      "title": "No Problem"
    },
    {
      "title": "Vak The Rock"
    },
    {
      "title": "Geldiler"
    },
    {
      "title": "M.V.A.B."
    }
  ]
}'

# Delete album with id
curl -X 'DELETE' 'http://localhost:5221/api/albums/1234' -H 'accept: */*'

# Update artist name
curl -X 'PUT' \
  'http://localhost:5221/api/artists/284' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "name": "Mazhar Fuat Özkan"
}'
```

Also we can use [Postman collection](Chinook%20Rest%20Service%20[Net%207].postman_collection.json) to test service endpoints.