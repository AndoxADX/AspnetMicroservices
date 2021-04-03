# AspnetMicroservices
Microservices

## How to run
docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml up -d

# postgres cli commands
docker exec -it discountdb psql -U admin
\l
create database DiscountDb;