# Cleaning Service API
This repository contains a cleaning service API for homeowners to publish cleaning assignments and cleaners to list and bid on assignments.
The API uses ASP.Net 8 for the API and SQLite for the database.

## How to run
There is a docker compose file at the root that can be run to start the service on port 5555 like this:
```sh
docker compose up -d --build
```
You probably need to trust the dev certificate located under `CleaningService.Api/local-certs/` if you want a "secure" connection.
The root path redirects to a Swagger UI which can be used to test out the API manually.
## How to test

You can run all the tests with the following commands:
```sh
docker compose build
docker compose run --rm cleaning-service test
```
You can also run the project without docker like a caveman with the following command:
```sh
dotnet run --project CleaningService.Api
```

### Testing the notification web hook
The API uses web hooks for notifying cleaners of new assignments. This can be tested like this:
```sh
# Start service
docker compose up -d --build
# Setup simple nc server to listen to port 69420 inside container
docker exec -it cleaningservice-cleaning-service-1 nc -l -p 69420
# From a second terminal run curl to subscribe
curl -k -H "Content-Type: application/json" https://localhost:5555/api/notification -d '{"name": "nc", "webHook": "http://localhost:69420"}'
# Post new assignment
curl -k -H "Content-Type: application/json" https://localhost:5555/api/assignment -d '{"user": "user1", "description": "please clean :S"}'
# Observe the content in the first terminal
```
## Assumptions and decisions made
* I Chose to use raw SQL and not Entity Framework as that is what I am the most familiar with.
* In a bigger project I would probably split the `Databse.cs` into smaller DAOs for separation of concern, but for this small project I think it's fine.
* I chose to use the same DTOs for the database and the API this is a trade off between simplicity and flexibility / quality of life.
* I would probably want to have the bid controller nested in the assignment path ie. `/api/assignment/{id}/bid` as a bid doesn't make sense without an assignment. I did it this way because of the way I accept bids. I didn't want to handle the case where a person entered an invalid assignment id for the given bid or vice versa.
* There is no authentication implemented
* I would like to have more time to write tests. I only test the happy cases via integration tests.