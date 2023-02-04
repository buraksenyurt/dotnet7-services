# .Net 7 App Services

In this repo, I include the codes that I try to examine the service development strategies on the .Net 7 side through a sample solution.

Solution uses a simple and popular database, chinook. Database provider is Postgresql and reside on docker container.

## Some Useful Informations

```shell
# to use Postgresql
docker run --name postgresql -e POSTGRES_USER=scoth -e POSTGRES_PASSWORD=tiger -p 5432:5432 -v /data:/var/lib/postgresql/data -d postgres

# to use ef tool
dotnet tool install -g dotnet-ef
# or update the existing dotnet-ef tool
dotnet tool update -g dotnet-ef

# create model classes from Chinook Db
# execute on RockShop.Common.Models project library
dotnet ef dbcontext scaffold "Host=localhost;Database=postgres;Username=scoth;Password=tiger" Npgsql.EntityFrameworkCore.Postgresql --namespace RockShop.Shared --data-annotations

# start existing container
docker start postgresql
# to working container list
docker ps -a
# to stop container
docker stop postgresql
# drop container
docker rm [container_id]

# if you want you can use docker-compose file
docker-compose up
```

### Sample Request

```text
HTTP Get
http://localhost:5221/ping

# Get Albums with paging
HTTP Get
http://localhost:5221/api/albums
http://localhost:5221/api/albums?page=3
```
