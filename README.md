# AspnetMicroservices
Microservices

## How to run
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up -d

# postgres cli commands
docker exec -it discountdb psql -U admin
\l
create database DiscountDb;

## Create certificate before running
dotnet dev-certs https --clean
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password
dotnet dev-certs https --trust